using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public MapGrid mapGrid;
    public Renderer overlayRenderer;

    float radius = 24.5f;

    public void SetZone(Tile tile, bool isSafeZone)
    {
        Vector2 center = new Vector2(tile.x, tile.y);
        int top = Mathf.CeilToInt(center.y - radius);
        int bottom = Mathf.FloorToInt(center.y + radius);
        List<Vector2Int> pixels = new List<Vector2Int>();

        for (int y = top; y <= bottom; y++)
        {
            int dY = y - (int)center.y;
            float dX = Mathf.Sqrt(radius * radius - dY * dY);
            
            int left = Mathf.CeilToInt(center.x - dX);
            int right = Mathf.FloorToInt(center.x + dX);
            
            for (int x = left; x <= right; x++)
            {
                if(!mapGrid.GetTile(x, y).inSafeZone)
                {
                    mapGrid.GetTile(x, y).inSafeZone = true;
                    pixels.Add(new Vector2Int(x, y));
                }
            }
        }

        mapGrid.ChangePixels(pixels);
    }

    public void SetZombieSpawns()
    {

    }
}
