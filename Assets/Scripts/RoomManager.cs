using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private static RoomManager Instance;

    private Dictionary<RoomLayout, bool[]> roomVariations;

    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomParent;
    [SerializeField] private RoomConfig[] roomConfig;

    public void Init(GameManager gameManager)
    { 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Optionally make it persist across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject);  // Destroy the new instance, not the existing one
            return;

        }

        roomVariations = new Dictionary<RoomLayout, bool[]>()
        {
            { RoomLayout.UDLR, new bool[] { true, true, true, true } },
            { RoomLayout.UDL, new bool[] { true, true, true, false } },
            { RoomLayout.UDR, new bool[] { true, true, false, true } },
            { RoomLayout.ULR, new bool[] { true, false, true, true } },
            { RoomLayout.DLR, new bool[] { false, true, true, true } },
            { RoomLayout.UD, new bool[] { true, true, false, false } },
            { RoomLayout.UL, new bool[] { true, false, true, false } },
            { RoomLayout.UR, new bool[] { true, false, false, true } },
            { RoomLayout.DL, new bool[] { false, true, true, false } },
            { RoomLayout.DR, new bool[] { false, true, false, true } },
            { RoomLayout.LR, new bool[] { false, false, true, true } },
            { RoomLayout.U, new bool[] { true, false, false, false } },
            { RoomLayout.D, new bool[] { false, true, false, false } },
            { RoomLayout.L, new bool[] { false, false, true, false } },
            { RoomLayout.R, new bool[] { false, false, false, true } }
        };
    }

    public bool[] GetDoorStates(RoomLayout layout)
    {
        if (Instance.roomVariations.ContainsKey(layout))
            return Instance.roomVariations[layout];
        else
            return new bool[] { false, false, false, false };  // Default state if layout is not found
    }

    #region Properties

    public GameObject RoomPrefab { get { return Instance.roomPrefab;} }
    public Transform RoomParent { get { return Instance.roomParent;} }
    public RoomConfig[] RoomConfigs { get { return Instance.roomConfig; } }

    #endregion
}
