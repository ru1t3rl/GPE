using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


namespace Ru1t3rl.MeshGen.ProceduralCave
{
    // Cellular Automata
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] Vector2Int gridSize;

        [SerializeField] string seed;
        [SerializeField] bool useRandomSeed;

        [SerializeField, Range(0, 100)] int randomFillPercent = 50;

        [SerializeField] int smoothing = 5;

        int[,] map;

        void Start()
        {
            GenerateMap();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GenerateMap();
            }
        }

        void GenerateMap()
        {
            map = new int[gridSize.x, gridSize.y];
            RandomFillMap();

            for (int i = 0; i < smoothing; i++)
            {
                SmoothMap();
            }
        }

        void RandomFillMap()
        {
            if (useRandomSeed)
            {
                seed = Time.time.ToString();
            }

            Random pseudoRandom = new Random(seed.GetHashCode());

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (x == 0 || x == gridSize.x - 1 || y == 0 || y == gridSize.y - 1)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        // If smaller as fill percent enable wall else disable
                        map[x, y] = pseudoRandom.Next(0, 100) < randomFillPercent ? 1 : 0;
                    }
                }
            }
        }

        void SmoothMap()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    int neighbourWallCount = GetSurrondingWallCount(x, y);

                    if (neighbourWallCount > 4)
                        map[x, y] = 1;
                    else if (neighbourWallCount < 4)
                        map[x, y] = 0;
                }
            }
        }

        int GetSurrondingWallCount(int gridX, int gridY)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < gridSize.x && neighbourY >= 0 && neighbourY < gridSize.y)
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            wallCount += map[neighbourX, neighbourY];
                        }
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }

            return wallCount;
        }

        void OnDrawGizmos()
        {
            if (map != null)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                        Vector3 pos = new Vector3(-gridSize.x / 2 + x + .5f, 0, -gridSize.y / 2 + y + .5f);
                        Gizmos.DrawCube(pos, Vector3.one);
                    }
                }
            }
        }
    }
}