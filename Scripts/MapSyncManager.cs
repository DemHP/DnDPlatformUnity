using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Application = UnityEngine.Application;
using System.IO.Compression;
using System.Text;
using System;

public class MapSyncManager : NetworkBehaviour
{
    public static MapSyncManager Instance;

    [Header("Save Manager")]
    public SaveManager saveManager;

    [Header("Transfer Settings")]
    public float transferTimeout = 30f;

    public int readyClients = 0;
    public ulong currentTransferId = 0;
    public Coroutine timeoutCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    // Host 
    public void HostOpenAndSend()
    {
        // Only host can do this
        if (!IsHost) return;

        string path = saveManager.OpenMapDialog();
        
        // if path doesn't exist, don't
        if (string.IsNullOrEmpty(path)) return;

        StartCoroutine(HostReadCompressSend(path));

        saveManager.LoadMapFromPath(path);
    }

    private IEnumerator HostReadCompressSend(string localPath)
    {
        yield return null;

        string json = File.ReadAllText(localPath);

        byte[] compress = CompressString(json);
        string base64 = Convert.ToBase64String(compress);

        currentTransferId++;
        readyClients = 0;

        ReceiveMapCompressedClientRpc(base64, currentTransferId);

        saveManager.LoadMapFromPath(localPath);

        if (timeoutCoroutine != null)
            StopCoroutine(timeoutCoroutine);

        timeoutCoroutine = StartCoroutine(TransferTimeoutWatcher(currentTransferId));
    }
    
    // Cleint Receives.
    [ClientRpc]
    void ReceiveMapCompressedClientRpc(string base64Compressed, ulong transferId)
    {
        StartCoroutine(ClientReceiveProcess(base64Compressed, transferId));
    }
    
    private IEnumerator ClientReceiveProcess(string base64Compressed, ulong transferId)
    {
        byte[] compressedBytes = Convert.FromBase64String(base64Compressed);
        string json = DecompressString(compressedBytes);

        string folder = Path.Combine(Application.persistentDataPath, "Maps");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string savePath = Path.Combine(folder, "currentMap.json");
        File.WriteAllText(savePath, json);

        saveManager.LoadMapFromPath(savePath);

        yield return null;

        NotifyHostReadyServerRpc(transferId);
    }

    // Notify we did the thingy thang
    [ServerRpc(RequireOwnership = false)]
    void NotifyHostReadyServerRpc(ulong transferId, ServerRpcParams rpcParams = default)
    {
        if (transferId != currentTransferId) return;

        readyClients++;

        int required = NetworkManager.Singleton.ConnectedClientsList.Count - 1;

        if (readyClients >= required)
        {
            if (timeoutCoroutine != null)
                StopCoroutine(timeoutCoroutine);

            Debug.Log("All clients finished loading map.");
        }
    }

    // Timeout
    private IEnumerator TransferTimeoutWatcher(ulong transferId)
    {
        float timer = 0f;

        while (timer < transferTimeout)
        {
            if (transferId != currentTransferId)
                yield break;

            int required = NetworkManager.Singleton.ConnectedClientsList.Count - 1;
            if (readyClients >= required)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning($"Map transfer timed out. Only {readyClients} received the map!");
    }

    // Helper functions
    private byte[] CompressString(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);

        using MemoryStream output = new MemoryStream();
        using (GZipStream gzip = new GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
        {
            gzip.Write(bytes, 0, bytes.Length);
        }

        return output.ToArray();
    }

    private string DecompressString(byte[] compressed)
    {
        using MemoryStream input = new MemoryStream(compressed);
        using GZipStream gzip = new GZipStream(input, CompressionMode.Decompress);
        using MemoryStream output = new MemoryStream();

        gzip.CopyTo(output);

        return Encoding.UTF8.GetString(output.ToArray());
    }
}