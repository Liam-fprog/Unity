using UnityEngine;
using System;
using System.Collections.Generic;

[DefaultExecutionOrder(-100)] // initialise TÔT avant d'autres Start()
public class PathRequestManager : MonoBehaviour
{
    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> cb)
        { pathStart = start; pathEnd = end; callback = cb; }
    }

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;
    bool isProcessingPath;

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

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        if (instance == null)
        {
            Debug.LogError("PathRequestManager : aucune instance active dans la scène au moment de RequestPath.");
            return;
        }
        var newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;

            if (pathfinding == null)
            {
                Debug.LogError("PathRequestManager : Pathfinding est null.");
                isProcessingPath = false;
                return;
            }
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback?.Invoke(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }
}
