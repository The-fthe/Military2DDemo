using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using InGameAsset.Scripts.Managers;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] AstarGrid m_AstarGrid;
    void Start()
    {
        m_AstarGrid ??= FindObjectOfType<AstarGrid>();
        Initialize();
        //m_AstarGrid.UpdateGrid();

//        FindObjectOfType<PlayerManager>().OnPlayerSpawn +=Initialize;
    }

    public void Initialize()
    {
        Debug.Log("Pathfinding is Initialize!");
        m_AstarGrid ??= FindObjectOfType<AstarGrid>();
        m_AstarGrid.UpdateGrid();

    }
    
    public void  FindPath(PathRequest request,Action<PathResult> callback)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        Node startNode = m_AstarGrid.NodeFromWorldPoint(request._pathStart);
        Node targetNode = m_AstarGrid.NodeFromWorldPoint(request._pathEnd);
        startNode.Parent = startNode;
        if (startNode._walkable && targetNode._walkable)
        {
            Heap<Node> openSet = new Heap<Node>(m_AstarGrid.MaxSize); 
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closeSet.Add(currentNode);
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    //print("Path found:" + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }

                foreach (var neighbour in m_AstarGrid.GetNeighbours(currentNode))
                {
                    if (!neighbour._walkable || closeSet.Contains(neighbour)) continue;
                    int newMovementCostToNeighbour = currentNode.Gcost + GetDistance(currentNode, neighbour)+ neighbour.MovementPanelty;
                    if (newMovementCostToNeighbour < neighbour.Gcost || !openSet.Contains(neighbour))
                    {
                        neighbour.Gcost = newMovementCostToNeighbour;
                        neighbour.Hcost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }
            callback(new PathResult(waypoints, pathSuccess, request._callback));
        }
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;
            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew =
                    new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i]._worldPosition);
                }

                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);
            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }