using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    [Header("General Settings")]
    public float moveSpeed = 5f;
    public Tilemap tilemap;
    public GameObject playerImage;
    public PlayerStatsManager playerStats;
    private bool statFound = false;

    [Header("DM Settings")]
    public GameObject selectedNPCInstance;  // what to move

    public List<GameObject> npcPrefabs = new List<GameObject>();

    public int selectedNPCPrefabIndex = -1;

    private Camera cam;

    // player movement state
    private Vector3 targetPos;
    private bool hasTarget = false;

    // NPC movement state
    private Vector3 npcTargetPos;
    private bool npcHasTarget = false;

    // ROLE ENUM
    public enum PlayerRole
    {
        PC,
        DM
    }

    // NETWORK SYNCED ROLE
    public NetworkVariable<PlayerRole> Role = new NetworkVariable<PlayerRole>(PlayerRole.PC);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            if (NetworkManager.Singleton.IsHost && OwnerClientId == NetworkManager.Singleton.LocalClientId)
                Role.Value = PlayerRole.DM;
            else
                Role.Value = PlayerRole.PC;
        }

        Role.OnValueChanged += OnRoleChanged;
        ApplyTag(Role.Value);
    }

    private void OnRoleChanged(PlayerRole oldRole, PlayerRole newRole)
    {
        ApplyTag(newRole);
    }

    private void ApplyTag(PlayerRole role)
    {
        gameObject.tag = role == PlayerRole.DM ? "DM" : "PC";
    }

    private void Start()
    {
        if (tag == "DM")
        {
            if (playerImage != null) Destroy(playerImage);
        }

    }

    private void Update()
    {
        if (!IsOwner) return;

        FindCamera();
        FindTileMap();

        HandleMouse();

        if (tag == "DM")
        {
            HandleDMControls();
            MoveNPC();
        }
        else if (tag == "PC")
        {
            MovePlayer();
        }

        if (statFound == false)
        {
            playerStats = FindAnyObjectByType<PlayerStatsManager>();
            
            if (playerStats != null) statFound = true;
        }
    }

    private void FindCamera()
    {
        if (!cam) cam = Camera.main;
    }

    private void FindTileMap()
    {
        if (!tilemap)
        {
            GameObject tilemapObj = GameObject.FindWithTag("Player");
            if (tilemapObj != null) tilemap = tilemapObj.GetComponent<Tilemap>();
        }
    }

    // ================= PLAYER CONTROLS =================

    private void HandleMouse()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            Vector3Int cell = tilemap.WorldToCell(mouseWorld);
            targetPos = tilemap.GetCellCenterWorld(cell);

            hasTarget = true;
        }
    }

    private void MovePlayer()
    {
        if (!hasTarget) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

    // ================= DM CONTROLS =================

    private void HandleDMControls()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // ESC : Deselect NPC and clear spawn selection
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedNPCInstance = null;
            selectedNPCPrefabIndex = -1;
            npcHasTarget = false;
            return;
        }

        // LEFT CLICK : Select / Deselect NPC
        if (Input.GetMouseButtonDown(0))
        {
            HandleDMSelection(mouseWorld);
        }

        // RIGHT CLICK : Move NPC OR Spawn NPC
        if (Input.GetMouseButtonDown(1))
        {
            HandleDMAction(mouseWorld);
        }
    }

    private void HandleDMSelection(Vector3 mouseWorld)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("NPC"))
        {
            // Turn OFF previous selection
            if (selectedNPCInstance != null)
            {
                ToggleSelectionVisual(selectedNPCInstance, false);
            }

            // Select new NPC
            SelectNPCInstance(hit.collider.gameObject);

            // Turn ON new selection
            ToggleSelectionVisual(selectedNPCInstance, true);
        }
        else if (hit.collider != null && hit.collider.CompareTag("PC"))
        {
            Debug.Log($"{hit.transform.name} Clicked!");
            playerStats.FetchPlayerStats(hit.transform.gameObject);
            playerStats.OpenPlayerStatsMenu();
        }
        else
        {
            // Turn OFF previous selection
            if (selectedNPCInstance != null)
            {
                ToggleSelectionVisual(selectedNPCInstance, false);
            }

            selectedNPCInstance = null;
            npcHasTarget = false;
        }
    }

    private void HandleDMAction(Vector3 mouseWorld)
    {
        if (selectedNPCInstance != null)
            SetNPCMoveTarget(mouseWorld);
        else
            SpawnNPC(mouseWorld);
    }

    public void FetchPlayerStats()
    {

    }

    // ================= NPC SPAWNING =================

    private void SpawnNPC(Vector3 position)
    {
        if (selectedNPCPrefabIndex == -1) return;

        Vector3Int cell = tilemap.WorldToCell(position);
        Vector3 spawnPos = tilemap.GetCellCenterWorld(cell);

        SpawnNPCServerRpc(selectedNPCPrefabIndex, spawnPos);

        // SHIFT = Multi place
        bool multiPlace = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (!multiPlace)
        {
            selectedNPCPrefabIndex = -1;
        }
    }

    [ServerRpc]
    private void SpawnNPCServerRpc(int prefabIndex, Vector3 spawnPos)
    {
        GameObject prefab = npcPrefabs[prefabIndex];

        GameObject npc = Instantiate(prefab, spawnPos, Quaternion.identity);
        npc.GetComponent<NetworkObject>().Spawn();
    }

    private void SelectNPCInstance(GameObject npc)
    {
        selectedNPCInstance = npc;
        Debug.Log($"Selected NPC instance: {npc.name}");
    }

    // ================= NPC MOVEMENT =================

    private void SetNPCMoveTarget(Vector3 position)
    {
        if (!selectedNPCInstance) return;

        Vector3Int cell = tilemap.WorldToCell(position);
        npcTargetPos = tilemap.GetCellCenterWorld(cell);

        npcHasTarget = true;
    }

    private void MoveNPC()
    {
        if (!selectedNPCInstance || !npcHasTarget) return;

        selectedNPCInstance.transform.position = Vector3.MoveTowards(
            selectedNPCInstance.transform.position,
            npcTargetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(selectedNPCInstance.transform.position, npcTargetPos) < 0.05f)
        {
            npcHasTarget = false;
        }
    }

    private void ToggleSelectionVisual(GameObject npc, bool state)
    {
        Transform[] children = npc.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child.CompareTag("Select"))
            {
                child.gameObject.SetActive(state);
                return;
            }
        }
    }
}