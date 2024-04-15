using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject doorUp;
    [SerializeField] private GameObject doorDown;
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;

    [SerializeField] private Door[] spawner;
    [SerializeField] private RoomLayout roomLayout;

    void Start()
    {
        SetDoors(roomLayout);
    }

    public void SetNewRoom(RoomLayout newRoomLayout)
    {
        SetDoors(newRoomLayout);
    }

    private void SetDoors(RoomLayout layout)
    {
        // Check if the RoomManager instance is available
        if (GameManager.Instance != null)
        {
            // Retrieve door states from RoomManager
            bool[] doorStates = GameManager.RoomManager.GetDoorStates(layout);

            for (int i = 0; i < doorStates.Length; i++)
            { 
                spawner[i].SetRoom(doorStates[i]);
            }
        }
        else
        {
            // Optionally handle the case where RoomManager is not available
            Debug.LogError("RoomManager instance not found, unable to set door states.");
        }
    }

    public void DestroyCorrespondingDoor(Origin comingFrom)
    {
        switch (comingFrom)
        {
            case Origin.Up:
                Destroy(doorDown);
                break;
            case Origin.Down:
                Destroy(doorUp);
                break;
            case Origin.Left:
                Destroy(doorRight);
                break;
            case Origin.Right:
                Destroy(doorLeft);
                break;
        }
    }

    private RoomLayout GetRandomRoomLayout()
    {
        int enumLength = System.Enum.GetValues(typeof(RoomLayout)).Length; // Get the number of enum values
        int randomValue;

        // Generate a random value until it's not equal to the last enum value (None)
        do
        {
            randomValue = Random.Range(0, enumLength);
        } while (randomValue == (int)RoomLayout.None);

        // Convert the random integer to the corresponding enum value
        return (RoomLayout)randomValue;
    }

    #region Properties

    public RoomLayout RoomLayout { get { return roomLayout; } }

    #endregion
}
