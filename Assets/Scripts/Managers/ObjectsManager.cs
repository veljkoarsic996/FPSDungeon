using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectsManager : MonoBehaviour
{
    private static ObjectsManager instance;

    [SerializeField] private List<GameObject> listOfObjects;

    [Range(0, 5)]
    [SerializeField] private int minObjectsPerRoom = 0;
    [Range(6, 15)]
    [SerializeField] private int maxObjectsPerRoom = 10;

    public void Init(GameManager gameManager)
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GameManager.RoomManager.OnDungeonCreated.AddListener(SpawnRandomObjects);
    }

    public void SpawnRandomObjects()
    {
        foreach(GameObject room in GameManager.RoomManager.RoomList)
        {
            Room currentRoom = room.GetComponent<Room>();

            int numberOfObjects = Random.Range(minObjectsPerRoom, maxObjectsPerRoom);

            for (int i = 0; i < numberOfObjects; i++)
            {
                Vector3 spawnPoint = GameManager.RoomManager.GetRandomPointOnMesh(currentRoom.DungeonFloor.GetComponent<MeshCollider>());

                spawnPoint.y = 1f;

                Instantiate(listOfObjects[Random.Range(0, listOfObjects.Count)], spawnPoint, Quaternion.Euler(-90,0,0), room.transform) ;
            }
        }
    }

}
