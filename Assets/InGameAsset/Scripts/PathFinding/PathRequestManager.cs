using System;
using System.Collections.Generic;
using System.Threading;
using InGameAsset.Scripts;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequests = new Queue<PathRequest>();
    Queue<PathResult> _results = new Queue<PathResult>(); 
    public static PathRequestManager Instance;
    public Pathfinding _pathfinding;
    Thread thread;
    bool gameRunning = true;

    void Awake()
    {
        Instance = this;
        _pathfinding ??= GetComponent<Pathfinding>();

        ThreadStart ts = delegate { PathRequestsUpdate(); };
        thread = new Thread(ts);
        thread.Start();
        Application.quitting += Quitting;
    }
    
    
    void Update()
    {
        if (_results.Count > 0)
        {
            int itemInQuee = _results.Count;
            lock (_results)
            {
                for (int i = 0; i < itemInQuee; i++)
                {
                    PathResult result = _results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }
    
    public void PathRequestsUpdate()
    {
        PathRequest pathRequest;

        while(gameRunning)
        {
            if (pathRequests.Count > 0)
            {
                lock (pathRequests)
                {
                    pathRequest = pathRequests.Dequeue();
                }
                _pathfinding.FindPath(pathRequest, FinishedProcessingPath);
            }
        }
    }
    
    private void Quitting()
    {
        gameRunning = false;
    }
    
    public static void RequestPath(PathRequest request)
    {
        lock (Instance.pathRequests)
        {
            Instance.pathRequests.Enqueue(request);
        }
    }
    public void FinishedProcessingPath(PathResult result)
    {
        lock (_results)
        {
            _results.Enqueue(result);
        }
    }
}
public struct  PathResult
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
    public Vector3 _pathStart;
    public Vector3 _pathEnd;
    public Action<Vector3[], bool> _callback;

    public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
    {
        _pathStart = start;
        _pathEnd = end;
        _callback = callback;
    }
}
