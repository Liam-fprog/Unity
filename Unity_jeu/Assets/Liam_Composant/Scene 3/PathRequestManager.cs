using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

[DefaultExecutionOrder(-100)] // initialise TÔT avant d'autres Start()
public class PathRequestManager : MonoBehaviour
{

    Queue<PathResult> results = new Queue<PathResult>();


    static PathRequestManager instance;
    Pathfinding pathfinding;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this); // évite plusieurs instances
            return;
        }
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
        if (pathfinding == null)
            Debug.LogError("PathRequestManager : Pathfinding manquant sur le même GameObject.");
        // (optionnel) garde le manager entre les scènes :
        // DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (results.Count > 0)
        {
            int itemsInQueue = results.Count;
            lock (results)
            {
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

    public static void RequestPath(PathRequest request)
    {
        ThreadStart threadStart = delegate
        {
            instance.pathfinding.FindPath(request, instance.FinishedProcessingPath);
        };
        threadStart.Invoke();
    }



    public void FinishedProcessingPath(PathResult result)
    {
        lock (results)
        {
            results.Enqueue(result);
        }

    }

}

public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback)
    {
        this.path = path;
        this.success = success;
        this.callback = callback;
    }
}

public struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;
    public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> cb)
    { pathStart = start; pathEnd = end; callback = cb; }
}