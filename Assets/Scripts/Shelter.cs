using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelter : MonoBehaviour, IEntity
{
    Tile tile;
    int strength = 350;
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
        GameManager.instance.safeZone.SetZone(tile, true);

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
            GameManager.instance.shelters.Remove(this);
            GameManager.instance.shelters.TrimExcess();
            GameManager.instance.safeZone.SetZone(tile, false);
            GameManager.instance.safeZone.SetZombieSpawns();

            Destroy(gameObject);
        }
    }
}
