using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private GameObject dungeonFloor, dungeonRoof;    

    internal void InitRoom(GameObject dungeonFloor, GameObject dungeonRoof)
    {
        this.dungeonFloor = dungeonFloor;
        this.dungeonRoof = dungeonRoof;
    }

    public GameObject DungeonFloor { get { return dungeonFloor; } }
    public GameObject DungeonRoof { get { return dungeonRoof; } }
}
