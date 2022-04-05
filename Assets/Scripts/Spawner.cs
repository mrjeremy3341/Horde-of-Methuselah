using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IEntity
{
    public GameObject spawnPrefab;

    Tile tile;
    int strength = 250;
    int age;
    bool isZombie;
    bool isBuilding = true;

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
        if(age > 4)
        {
            Tile newTile = NextTile();
            if (newTile != null)
            {
                GameManager.instance.SpawnEntity(spawnPrefab, newTile);
            }

            age = 0;
        }

        if(GameManager.instance.food == 0)
        {
            strength -= 10;
        }

        age++;
    }

    private void Update()
    {
        if (strength <= 0)
        {
            tile.entities.Remove(this);
            tile.entities.TrimExcess();
            GameManager.instance.SetWalkable(tile, this);
            GameManager.instance.entities.Remove(this);
            GameManager.instance.entities.TrimExcess();
            Destroy(gameObject);
        }
    }

    Tile NextTile()
    {
        List<Tile> choices = new List<Tile>();
        Tile n = GameManager.instance.mapGrid.GetTile(tile.x, tile.y + 2);
        if (n.entities.Count == 0 || !n.entities[0].IsZombie)
        {
            if (n.isWalkable && n.inSafeZone)
            {
                Tile t = GameManager.instance.mapGrid.GetTile(n.x, n.y - 1);
                choices.Add(t);
            }
        }

        Tile e = GameManager.instance.mapGrid.GetTile(tile.x + 2, tile.y);
        if (e.entities.Count == 0 || !e.entities[0].IsZombie)
        {
            if (e.isWalkable && e.inSafeZone)
            {
                Tile t = GameManager.instance.mapGrid.GetTile(e.x - 1, e.y);
                choices.Add(t);
            }
        }

        Tile s = GameManager.instance.mapGrid.GetTile(tile.x, tile.y - 2);
        if (s.entities.Count == 0 || !s.entities[0].IsZombie)
        {
            if (s.isWalkable && s.inSafeZone)
            {
                Tile t = GameManager.instance.mapGrid.GetTile(s.x, s.y + 1);
                choices.Add(t);
            }
        }

        Tile w = GameManager.instance.mapGrid.GetTile(tile.x - 2, tile.y);
        if (w.entities.Count == 0 || !w.entities[0].IsZombie)
        {
            if (w.isWalkable && w.inSafeZone)
            {
                Tile t = GameManager.instance.mapGrid.GetTile(w.x + 1, w.y);
                choices.Add(t);
            }
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
