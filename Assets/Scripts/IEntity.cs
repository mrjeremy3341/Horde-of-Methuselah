using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    Tile Tile { get; set; }
    int Strength { get; set; }
    int Age { get; set; }
    bool IsZombie { get; set; }
    bool IsBuilding { get; set; }
    void Rules();
}
