using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public class GridManager : MonoBehaviour
    {
        public Tilemap tilemap;
        public Vector3Int[,] spots;
        AstarGrid _astarGrid;
        BoundsInt bounds;

        void Start()
        {
            tilemap.CompressBounds();
        }

        public void CreateGrid()
        {
            spots = new Vector3Int[bounds.size.x, bounds.size.y];
            for (int x = bounds.xMin, i = 0; i < (bounds.size.x); x++, i++)
            {
                for (int y = bounds.yMin, j = 0; j < (bounds.size.y); y++, j++)
                {
                    if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        spots[i, j] = new Vector3Int(x, y, 0);
                        Gizmos.DrawSphere(spots[i,j],0.2f);
                    }
                    else
                    {
                        spots[i, j] = new Vector3Int(x, y, 1);
                        Gizmos.DrawSphere(spots[i,j],0.2f);
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            if(spots.Length <= 0)
                CreateGrid();
        }
    }
 }