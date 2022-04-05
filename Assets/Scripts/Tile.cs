using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Water, Ground, Field, Rock, Tree
}

public class Tile
{
    public TileType type;
    public List<IEntity> entities;
    public bool isWalkable;
    public bool inSafeZone;
    public int x, y;
    public Vector3 worldPos;

    public Tile(TileType type, int x, int y)
    {
        this.type = type;
        this.entities = new List<IEntity>();
        this.inSafeZone = false;
        this.x = x;
        this.y = y;
        this.worldPos = GetPosition(x, y);
        this.isWalkable = type != TileType.Water;
    }

    Vector3 GetPosition(int x, int y)
    {
        float xPos = (x - 300) / 6f;
        float yPos = (y - 300) / 6f;

        return new Vector3(xPos, yPos, 0);
    }
}