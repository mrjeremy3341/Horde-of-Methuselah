using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collecter : MonoBehaviour, IEntity
{
    public TileType resource;

    Tile tile;
    int strength = 50;
    int age;
    bool isZombie;
    bool isBuilding;

    public Tile Tile
    {
        get { return tile; }
        set { tile = value; }
    }

    public int Strength
    {
        get { return strength; }
        set { strength = value; }
    }

    public int Age
    {
        get { return age; }
        set { age = value; }
    }

    public bool IsZombie
    {
        get { return isZombie; }
        set { isZombie = value; }
    }

    public bool IsBuilding
    {
        get { return isBuilding; }
        set { isBuilding = value; }
    }

    public void Rules()
    {
        Tile newTile = NextTile();
        if (newTile != null)
        {
            tile.entities.Remove(this);
            tile.entities.TrimExcess();
            tile = newTile;
            tile.entities.Add(this);
            transform.position = tile.worldPos;
        }

        if(resource == TileType.Field)
        {
            int count = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Tile n = GameManager.instance.mapGrid.GetTile(tile.x + x, tile.y + y);
                    if (n.type == resource)
                    {
                        count++;
                    }
                }
            }
            if (count > 5)
            {
                GameManager.instance.CollectResource(resource);
            }
        }
        else if (resource == TileType.Rock)
        {
            if(age % 3 == 0)
            {
                int count = 0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Tile n = GameManager.instance.mapGrid.GetTile(tile.x + x, tile.y + y);
                        if (n.type == resource)
                        {
                            count++;
                        }
                    }
                }
                if (count > 5)
                {
                    GameManager.instance.CollectResource(resource);
                }
            }
        }
        else
        {
            if(age % 4 == 0)
            {
                int count = 0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Tile n = GameManager.instance.mapGrid.GetTile(tile.x + x, tile.y + y);
                        if (n.type == resource)
                        {
                            count++;
                        }
                    }
                }
                if (count > 5)
                {
                    GameManager.instance.CollectResource(resource);
                }
            }
        }

        if(age % 3 == 0)
        {
            if (GameManager.instance.food > 0)
            {
                GameManager.instance.food--;
            }
            else
            {
                strength -= 25;
            }
        }
        
        age++;
    }

    private void Update()
    {
        if (age > strength)
        {
            tile.entities.Remove(this);
            tile.entities.TrimExcess();
            GameManager.instance.entities.Remove(this);
            GameManager.instance.entities.TrimExcess();
            GameManager.instance.population--;
            Destroy(gameObject);
        }
    }

    Tile NextTile()
    {
        List<Tile> choices = new List<Tile>();
        Tile n = GameManager.instance.mapGrid.GetTile(tile.x, tile.y + 1);
        if (n.entities.Count == 0 || !n.entities[0].IsZombie)
        {
            if (n.isWalkable && n.inSafeZone) { choices.Add(n); }
        }

        Tile e = GameManager.instance.mapGrid.GetTile(tile.x + 1, tile.y);
        if (e.entities.Count == 0 || !e.entities[0].IsZombie)
        {
            if (e.isWalkable && e.inSafeZone) { choices.Add(e); }
        }

        Tile s = GameManager.instance.mapGrid.GetTile(tile.x, tile.y - 1);
        if (s.entities.Count == 0 || !s.entities[0].IsZombie)
        {
            if (s.isWalkable && s.inSafeZone) { choices.Add(s); }
        }

        Tile w = GameManager.instance.mapGrid.GetTile(tile.x - 1, tile.y);
        if (w.entities.Count == 0 || !w.entities[0].IsZombie)
        {
            if (w.isWalkable && w.inSafeZone) { choices.Add(w); }
        }

        if (choices.Count > 0)
        {
            int r = Random.Range(0, choices.Count);
            return choices[r];
        }
        else
        {
            return null;
        }
    }
}
