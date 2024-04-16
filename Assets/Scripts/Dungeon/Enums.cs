
using Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enums
{
    public enum Orientation
    { 
        Horizontal =0,
        Vertical = 1,
    }

    public enum RelativePosition
    { 
        Up,
        Down,
        Left,
        Right,
    }
    
}

public class RoomNode : Node 
{
    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parentNode, int index, bool isCorridor) : base(parentNode)
    { 
        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);
        this.isCorridor = isCorridor;
        this.TreeLayerIndex = index;
    }

    public int Width { get => (int)(TopRightAreaCorner.x - BottomLeftAreaCorner.x); }
    public int Length { get => (int)(TopRightAreaCorner.y - BottomLeftAreaCorner.y); }
}

public abstract class Node
{
    private List<Node> childrenNodeList;

    public List<Node> ChildrenNodeList { get => childrenNodeList; }

    public bool Visited { get; set; }
    public Vector2Int BottomLeftAreaCorner { get; set; }
    public Vector2Int BottomRightAreaCorner { get; set; }
    public Vector2Int TopRightAreaCorner { get; set; }
    public Vector2Int TopLeftAreaCorner { get; set; }
    public int TreeLayerIndex { get; set; }

    public bool isCorridor { get; set; }
    public Node Parent { get; set; }

    public Node(Node parentNode)
    { 
        childrenNodeList = new List<Node>();
        this.Parent = parentNode;
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    public void AddChild(Node node)
    {
        childrenNodeList.Add(node);
    }

    public void RemoveChilld(Node node) 
    {
        childrenNodeList.Remove(node);
    }
}
