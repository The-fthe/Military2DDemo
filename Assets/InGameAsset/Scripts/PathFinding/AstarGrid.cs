using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;

public class AstarGrid : MonoBehaviour
{
    public bool _displayGridGizmos;
    public Vector2 GridWorldSize;
    public float NodeRadius;
    public TerrainType[] WalkableRegions;
    public int ObstacleProximityPenalty = 10;
    Dictionary<string, int> walkableRegionsDictionary = new Dictionary<string, int>();
    Node[,] _grid;
    
    float _nodeDiameter;
    int _gridSizeX, _gridSizeY;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;
    
    BoundsInt _area;
    TileBase[] _allTiles;

    Vector2 objPos;
    void Awake()
    {
        _nodeDiameter = NodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);
        foreach (var region in WalkableRegions)
        {
            walkableRegionsDictionary.Add(region.tilemap.name, region.terrainPenalty);
            region.tilemap.CompressBounds();
        }
        
        //CreateGrid();
        objPos = transform.position;
    }
    
    public void AddTileMapToDictionary()
    {
        foreach (var region in WalkableRegions)
        {
            walkableRegionsDictionary.Add(region.tilemap.name, region.terrainPenalty);
            region.tilemap.CompressBounds();
        }
    }

    public int MaxSize => _gridSizeX * _gridSizeY;
    public void UpdateGrid ()
    {
        //AddTileMapToDictionary();
        CreateGrid();
    }

    #region GridSetUp

    void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;
        int[,] penaltierHorizontalPass = new int[_gridSizeX, _gridSizeY];
        int[,] penaltierVerticalPass = new int[_gridSizeX, _gridSizeY];

        for (int y = 0; y < _gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltierHorizontalPass[0, y] += _grid[sampleX, y].MovementPanelty;
            }

            for (int x = 1; x < _gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, _gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, _gridSizeX - 1);
                penaltierHorizontalPass[x, y] = penaltierHorizontalPass[x - 1, y] -
                                                _grid[removeIndex, y].MovementPanelty +
                                                _grid[addIndex, y].MovementPanelty;
            }
        }

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltierVerticalPass[x, 0] += penaltierHorizontalPass[x, sampleY];
            }

            int blurredPenality = Mathf.RoundToInt((float) penaltierVerticalPass[x, 0] / (kernelSize * kernelSize));
            _grid[x, 0].MovementPanelty = blurredPenality;
            for (int y = 1; y < _gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, _gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, _gridSizeY - 1);
                penaltierVerticalPass[x, y] = penaltierVerticalPass[x, y - 1] -
                                              penaltierHorizontalPass[x, removeIndex] +
                                              penaltierHorizontalPass[x, addIndex];
                blurredPenality = Mathf.RoundToInt((float) penaltierVerticalPass[x, y] / (kernelSize * kernelSize));
                _grid[x, y].MovementPanelty = blurredPenality;
                if (blurredPenality > penaltyMax)
                {
                    penaltyMax = blurredPenality;
                }

                if (blurredPenality < penaltyMin)
                {
                    penaltyMin = blurredPenality;
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.GridX + x;
                int checkY = node.GridY + y;
                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    #endregion
    void CreateGrid()
    {
        _grid = new Node[_gridSizeX, _gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 -
                                  Vector3.up * GridWorldSize.y / 2;
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + NodeRadius) +
                                     Vector3.up * (y * _nodeDiameter + NodeRadius);
                Vector3 worldPointTile = worldBottomLeft + Vector3.right * (x * _nodeDiameter) +
                                         Vector3.up * (y * _nodeDiameter);
                int movementPenalty = 0;
                bool walkable = true;
                foreach (var walkableRegion in WalkableRegions)
                {
                    Vector3Int localPos = walkableRegion.tilemap.WorldToCell(worldPointTile);
                    if(walkableRegion.tilemap.HasTile(localPos))
                        walkableRegionsDictionary.TryGetValue(walkableRegion.tilemap.name, out movementPenalty);
                    if (movementPenalty >= 50)
                        walkable = false;
                    if (!walkable)
                        movementPenalty += ObstacleProximityPenalty;
                    _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }
        }
        BlurPenaltyMap(2);
    }
    // public Node NodeFromWorldPoint(Vector3 worldPosition)
    // {
    //     float percentX = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
    //     float percentY = ( worldPosition.y + GridWorldSize.y / 2) / GridWorldSize.y;
    //     percentX = Mathf.Clamp01(percentX);
    //     percentY = Mathf.Clamp01(percentY);
    //     int x = Mathf.FloorToInt(Mathf.Min(_gridSizeX * percentX, _gridSizeX - 1));
    //     int y = Mathf.FloorToInt(Mathf.Min(_gridSizeX * percentY, _gridSizeY - 1));
    //     return _grid[x, y];
    // }
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
       worldPosition= WorldToLocal(worldPosition);
        float percentX = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float percentY = (worldPosition.y + GridWorldSize.y / 2) / GridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.FloorToInt(Mathf.Min( _gridSizeX * percentX,  _gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Min(  +_gridSizeY * percentY,  _gridSizeY - 1));
        return _grid[x, y];
    }
    Vector2 WorldToLocal(Vector2 worldPt)
    {
        Vector2 relPoint = worldPt - objPos;
        float x = Vector2.Dot(relPoint, Vector2.right);
        float y = Vector2.Dot(relPoint, Vector2.up);
        return new Vector2(x, y);
    }
    void OnDrawGizmos()
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 -
                                  Vector3.up * GridWorldSize.y / 2;
        Vector3 worldPoint = worldBottomLeft + Vector3.right * (1 * _nodeDiameter + NodeRadius) +
                             Vector3.up * (1 * _nodeDiameter + NodeRadius);
        Debug.DrawRay(worldPoint, Vector3.left * 10, Color.green);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.y, 1));
        if (_grid != null && _displayGridGizmos)
        {
            foreach (var node in _grid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black,
                    Mathf.InverseLerp(penaltyMin, penaltyMax, node.MovementPanelty));
                Gizmos.color = (node._walkable) ? Gizmos.color : Color.red;
                Gizmos.DrawCube(node._worldPosition, Vector3.one * (_nodeDiameter));
                //Gizmos.DrawWireCube(node._worldPosition, Vector3.one * (_nodeDiameter));
            }
        }
    }
}

[System.Serializable]
public class TerrainType
{
    public Tilemap tilemap;
    public int terrainPenalty;
}