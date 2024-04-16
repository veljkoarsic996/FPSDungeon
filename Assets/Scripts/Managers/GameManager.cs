using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] DungeonManager roomManager;


    // Start is called before the first frame update
    void Awake()
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

        roomManager.Init(this);

    }

    #region Properties
    public static DungeonManager RoomManager { get { return Instance.roomManager; } }

    #endregion
}
