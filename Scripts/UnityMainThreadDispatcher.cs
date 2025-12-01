using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher instance;

    public static void Initialize()
    {
        if (instance != null) return;

        GameObject obj = new GameObject("UnityMainThreadDispatcher");
        instance = obj.AddComponent<UnityMainThreadDispatcher>();
        DontDestroyOnLoad(obj);
    }

    public static void Enqueue(Action action)
    {
        if (instance == null)
        {
            Initialize();
        }

        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }
}
