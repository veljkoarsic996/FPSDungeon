using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    [Range(0,2)]
    [SerializeField] private int roomOffset;


    public void Init(GameManager gameManager)
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }

        CreateDungeon();
    }

    private void CreateDungeon()
    {
        dungeonGenerator.Init(dungeonWidth,dungeonLength);
        var listOfRooms = dungeonGenerator.CalculateDungeon(maxIterations, roomWidthMin, roomLengthMin, roomBottomCornerModifier, roomTopCornerModifier, roomOffset, corridorWidth);

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftVertex = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightVertex = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);

        Vector3 topLeftVertex = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightVertex = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
            {
                topLeftVertex,
                topRightVertex, 
                bottomLeftVertex, 
                bottomRightVertex
            };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
            {
                0,
                1,
                2,
                2,
                1,
                3
            };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh : " + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
    }
}
