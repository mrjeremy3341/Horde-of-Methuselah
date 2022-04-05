using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : MonoBehaviour, IEntity
{
    Tile tile;
    int strength = 150;
    int age;
    bool isZombie = true;
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
        if (age % 3 == 0)
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
        }

        Attack();

        age++;
        if (!tile.inSafeZone)
        {
            strength--;
        }
    }

    private void Update()
    {
        if (age > strength)
        {
            tile.entities.Remove(this);
            tile.entities.TrimExcess();
            GameManager.instance.entities.Remove(this);
            GameManager.instance.entities.TrimExcess();
            Destroy(gameObject);
        }
    }

    Tile NextTile()
    {
        List<Tile> choices = new List<Tile>();
        Tile n = GameManager.instance.mapGrid.GetTile(tile.x, tile.y + 1);
        if (n.isWalkable) { choices.Add(n); }
        Tile e = GameManager.instance.mapGrid.GetTile(tile.x + 1, tile.y);
        if (e.isWalkable) { choices.Add(e); }
        Tile s = GameManager.instance.mapGrid.GetTile(tile.x, tile.y - 1);
        if (s.isWalkable) { choices.Add(s); }
        Tile w = GameManager.instance.mapGrid.GetTile(tile.x - 1, tile.y);
        if (w.isWalkable) { choices.Add(w); }

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

    void Attack()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != tile.x || y != tile.y)
                {
                    Tile t = GameManager.instance.mapGrid.GetTile(tile.x + x, tile.y + y);
                    if (t.entities.Count > 0 && !t.entities[0].IsZombie)
                    {
                        t.entities[0].Strength -= strength;
                        if (t.entities[0].IsBuilding)
                        {
                            t.entities[0].Strength -= strength;
                        }
                    }
                }
            }
        }
    }
}
