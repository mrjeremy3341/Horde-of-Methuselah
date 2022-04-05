using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public Color waterColorD;
    public Color groundColorD;
    public Color fieldColorD;
    public Color rockColorD;
    public Color treeColorD;

    public Color waterColorL;
    public Color groundColorL;
    public Color fieldColorL;
    public Color rockColorL;
    public Color treeColorL;
    
    public Renderer mapRenderer;

    public Texture2D texture;
    Tile[,] mapTiles;
    int mapSize = 600;

    public void GenerateMap()
    {
        mapTiles = new Tile[mapSize, mapSize];
        float[,] cells = Noise.GenerateHeightMap(mapSize, 50, 8, .5f, 2f);

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                if(cells[x,y] < 0.15f)
                {
                    mapTiles[x, y] = new Tile(TileType.Water, x, y);
                }
                else if(cells[x, y] < .25f)
                {
                    mapTiles[x, y] = new Tile(TileType.Ground, x, y);
                }
                else if (cells[x, y] < .45f)
                {
                    mapTiles[x, y] = new Tile(TileType.Field, x, y);
                }
                else if (cells[x, y] < .65f)
                {
                    mapTiles[x, y] = new Tile(TileType.Tree, x, y);
                }
                else
                {
                    mapTiles[x, y] = new Tile(TileType.Rock, x, y);
                }
            }
        }

        texture = new Texture2D(mapSize, mapSize);
        texture.filterMode = FilterMode.Point;
        Color[] colorMap = new Color[mapSize * mapSize];
        
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                Color color;
                if(mapTiles[x,y].type == TileType.Water)
                {
                    color = waterColorD;
                }
                else if(mapTiles[x, y].type == TileType.Ground)
                {
                    color = groundColorD;
                }
                else if (mapTiles[x, y].type == TileType.Field)
                {
                    color = fieldColorD;
                }
                else if (mapTiles[x, y].type == TileType.Tree)
                {
                    color = treeColorD;
                }
                else
                {
                    color = rockColorD;
                }

                colorMap[y * mapSize + x] = color;
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();
        mapRenderer.material.mainTexture = texture;
    }

    public Tile GetTile(int x, int y)
    {
        return mapTiles[x, y];
    }

    public List<Tile> DarkTiles()
    {
        List<Tile> tiles = new List<Tile>();
        foreach (Tile t in mapTiles)
        {
            if (!t.inSafeZone && t.isWalkable)
            {
                if (t.entities.Count == 0 || t.entities[0].IsZombie)
                {
                    tiles.Add(t);
                }
            }
        }
        return tiles;
    }

    public void ChangePixels(List<Vector2Int> pixels)
    {
        foreach(Vector2Int p in pixels)
        {
            Color start = texture.GetPixel(p.x, p.y);
            Color end;

            if(CheckColors(start, waterColorD))
            {
                end = waterColorL;
            }
            else if (CheckColors(start, groundColorD))
            {
                end = groundColorL;
            }
            else if (CheckColors(start, fieldColorD))
            {
                end = fieldColorL;
            }
            else if (CheckColors(start, treeColorD))
            {
                end = treeColorL;
            }
            else
            {
                end = rockColorL;
            }
            texture.SetPixel(p.x, p.y, end);
        }

        texture.Apply();
        mapRenderer.material.mainTexture = texture;
    }

    bool CheckColors(Color colorA, Color colorB)
    {
        bool r = FastApproximately(colorA.r, colorB.r, .01f);
        bool g = FastApproximately(colorA.g, colorB.g, .01f);
        bool b = FastApproximately(colorA.b, colorB.b, .01f);

        if(r && g && b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }
}
