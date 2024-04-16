using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private MeshRenderer wallRenderer;

    public MeshRenderer WallRenderer { get { return wallRenderer; } }
}
