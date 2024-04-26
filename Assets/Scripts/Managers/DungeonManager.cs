using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{
    private static DungeonManager instance;

    [Header("Generator")]
    [SerializeField] private DungeonGenerator dungeonGenerator;

    [Header("Fields")]
    [SerializeField] private int dungeonWidth;
    [SerializeField] private int dungeonLength;
    [SerializeField] private int roomWidthMin;
    [SerializeField] private int roomLengthMin;
    [SerializeField] private int maxIterations;
    [SerializeField] private int corridorWidth;
    [SerializeField] private Material material;
    [Range(.0f, .3f)]
    [SerializeField] private float roomBottomCornerModifier;
    [Range(.7f, 1f)]
    [SerializeField] private float roomTopCornerModifier;
    [Range(0, 2)]
    [SerializeField] private int roomOffset;

    [SerializeField] private GameObject wallVertical;
    [SerializeField] private GameObject wallHorizontal;

    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;

    [SerializeField] private Wall wall;
    private float wallHeight;

    private GameObject rooms;

    [SerializeField] List<GameObject> roomList;

    private UnityEvent onDungeonCreated;

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
        onDungeonCreated = new UnityEvent();

        StartCoroutine(CreateDungeon());

        onDungeonCreated.AddListener(GenerateProps);
    }

    private IEnumerator CreateDungeon()
    {
        yield return null;

        DestroyAllChildren();

        dungeonGenerator.Init(dungeonWidth, dungeonLength);

        var listOfRooms = dungeonGenerator.CalculateDungeon(maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset,
            corridorWidth);

        rooms = new GameObject("RoomParent");
        rooms.transform.parent = transform;

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = rooms.transform;

        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        if (wall != null)
        {
            wallHeight = wall.WallRenderer.bounds.size.y;
        }

        roomList.Clear();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(i + 1, listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner, listOfRooms[i].isCorridor);
        }
        CreateWalls(wallParent);

        yield return null;
        onDungeonCreated.Invoke();
        yield return null;
    }

    private void GenerateProps()
    { 
        foreach (var room in roomList)
        {
            
        }
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
    }

    private void CreateMesh(int index, Vector2 bottomLeftCorner, Vector2 topRightCorner, bool isCorridor)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV,
            // Repeating the same vertices for the bottom face
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            // Top face
            0, 1, 2,
            2, 1, 3,
            // Bottom face
            6, 5, 4, // Reversed winding
            6, 7, 5
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject holder = new GameObject();

        if (!isCorridor)
        {
            holder.name = "Room " + index;
            holder.transform.position = Vector3.zero;
            holder.transform.parent = rooms.transform;

            roomList.Add(holder);
        }
        else
        {
            holder.name = "Corridor " + index;
            holder.transform.position = Vector3.zero;
            holder.transform.parent = rooms.transform;
        }

        GameObject dungeonFloor = new GameObject("Floor" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = new Vector3(1, 0.01f, 1);
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.transform.parent = holder.transform;
        dungeonFloor.GetComponent<MeshCollider>().sharedMesh = mesh;
        dungeonFloor.GetComponent<MeshCollider>().convex = true;

        GameObject dungeonRoof = new GameObject("Roof" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        dungeonRoof.transform.position = new Vector3(dungeonFloor.transform.position.x, dungeonFloor.transform.position.y + wallHeight, dungeonFloor.transform.position.z);
        dungeonRoof.transform.localScale = new Vector3(1, 0.01f, 1);
        dungeonRoof.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        dungeonRoof.transform.parent = holder.transform;
        dungeonRoof.GetComponent<MeshFilter>().mesh = mesh;
        dungeonRoof.GetComponent<MeshRenderer>().material = material;
        dungeonRoof.GetComponent<MeshCollider>().sharedMesh = mesh;
        dungeonRoof.GetComponent<MeshCollider>().convex = true;

        if (!isCorridor)
        {
            holder.AddComponent<Room>().InitRoom(dungeonFloor, dungeonRoof);
        }

        dungeonRoof.tag = "Concrete";
        dungeonFloor.tag = "Concrete";

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }

    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }

    public Vector3 GetRandomPointOnMesh(MeshCollider collider)
    {
        Mesh mesh = collider.sharedMesh;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        // Random triangle index
        int randomTriangleIndex = Random.Range(0, triangles.Length / 3);
        int vertexIndex1 = triangles[randomTriangleIndex * 3];
        int vertexIndex2 = triangles[randomTriangleIndex * 3 + 1];
        int vertexIndex3 = triangles[randomTriangleIndex * 3 + 2];

        Vector3 point1 = vertices[vertexIndex1];
        Vector3 point2 = vertices[vertexIndex2];
        Vector3 point3 = vertices[vertexIndex3];

        // Generate random barycentric coordinates
        float a = Random.value;
        float b = Random.value * (1 - a);
        float c = 1 - a - b;

        // Calculate the random point inside the triangle
        Vector3 randomPoint = a * point1 + b * point2 + c * point3;

        // Transform the point to world space
        return collider.transform.TransformPoint(randomPoint);
    }

    public GameObject GetRandomRoom()
    {
        return instance.roomList[Random.Range(0, instance.roomList.Count)];
    }

    public List<GameObject> RoomList { get { return roomList; } }

    public UnityEvent OnDungeonCreated { get { return onDungeonCreated; } }
}
