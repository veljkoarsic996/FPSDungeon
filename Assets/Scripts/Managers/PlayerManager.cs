using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;

    private GameManager gameManager;

    [SerializeField] private GameObject player;

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
        this.gameManager = gameManager;

        GameManager.RoomManager.OnDungeonCreated.AddListener(SpawnPlayer);
    }

    private void SpawnPlayer() 
    {
        Room randomRoom = GameManager.RoomManager.GetRandomRoom().GetComponent<Room>();

        Vector3 spawnPoint = GameManager.RoomManager.GetRandomPointOnMesh(randomRoom.DungeonFloor.GetComponent<MeshCollider>());

        Instantiate(player, spawnPoint, Quaternion.identity);
    }

}
