using UnityEngine;
using Enums;
using System.Linq;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform newRoomLocation;
    [SerializeField] private Origin comingFrom;

    public void SetRoom(bool isAvailable)
    {
        if (isAvailable)
        {
            // Check if the position for the new room is available
            if (!IsPositionOccupied(newRoomLocation.position))
            {
                // Instantiate the new room
                GameObject newRoomGO = Instantiate(GameManager.RoomManager.RoomPrefab);
                Room newRoom = newRoomGO.GetComponent<Room>();

                // Position the new room and set its parent
                newRoomGO.transform.position = newRoomLocation.position;
                newRoomGO.transform.parent = GameManager.RoomManager.RoomParent;

                // Set the new room's layout and destroy the corresponding door
                newRoom.SetNewRoom(GetRandomRoomLayout(comingFrom));
                newRoom.DestroyCorrespondingDoor(comingFrom);

                // Destroy the door
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Failed to generate room: Position already occupied.");
            }
        }
        else
        {
            Destroy(newRoomLocation.gameObject);
        }
    }

    // Check if a position is occupied by another room
    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f); // Adjust the radius as needed

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Room")) // Assuming room objects have a "Room" tag
            {
                return true;
            }
        }

        return false;
    }

    // Get a random room layout for the given origin
    private RoomLayout GetRandomRoomLayout(Origin cameFrom)
    {
        if (GameManager.RoomManager.RoomConfigs.Count() == 0)
        {
            Debug.LogWarning("No room configurations available.");
            return RoomLayout.None;
        }

        foreach (RoomConfig roomConfig in GameManager.RoomManager.RoomConfigs)
        {
            if (roomConfig.CameFrom == cameFrom)
            {
                int randomIndex = Random.Range(0, roomConfig.AvailableLayouts.Length);
                return roomConfig.AvailableLayouts[randomIndex];
            }
        }

        return RoomLayout.None;
    }
}
