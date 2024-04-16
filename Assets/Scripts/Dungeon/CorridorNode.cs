﻿using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CorridorNode : Node
{
    private Node structure1;
    private Node structure2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;

        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePositionOfStructure2 = CheckPositionStructure2AgainstStructure1();

        switch (relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomInRelationUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomInRelationUpOrDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomInRelationRightOrLeft(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomInRelationRightOrLeft(this.structure2, this.structure1);
                break;
        }
    }

    private void ProcessRoomInRelationUpOrDown(Node structure1, Node structure2)
    {
        Node bottomStructure = null;
        List<Node> structureBottomChildren = StructureHelper.TraverseGraphToExtractLowestLeafes(structure1);

        Node topStructure = null;
        List<Node> structureAboveChildren = StructureHelper.TraverseGraphToExtractLowestLeafes(structure2);

        var sortedBottomStructure = structureBottomChildren.OrderByDescending(child => child.TopRightAreaCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottomChildren[0];
        }
        else 
        {
            int maxY = sortedBottomStructure[0].TopLeftAreaCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.TopLeftAreaCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        var possibleNeighboursInTopStructure = structureAboveChildren.Where(
            child => GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                child.BottomLeftAreaCorner,
                child.BottomRightAreaCorner
            ) != -1).OrderBy(child => child.BottomRightAreaCorner.y).ToList();

        if (possibleNeighboursInTopStructure.Count == 0)
        {
            topStructure = structure2;
        }
        else 
        {
            topStructure = possibleNeighboursInTopStructure[0];
        }

        int x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftAreaCorner,
                bottomStructure.TopRightAreaCorner,
                topStructure.BottomLeftAreaCorner,
                topStructure.BottomRightAreaCorner);

        while (x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(child => child.TopLeftAreaCorner.x != topStructure.TopLeftAreaCorner.x).ToList();
            bottomStructure = sortedBottomStructure[0];

            x = GetValidXForNeighbourUpDown(
                    bottomStructure.TopLeftAreaCorner,
                    bottomStructure.TopRightAreaCorner,
                    topStructure.BottomLeftAreaCorner,
                    topStructure.BottomRightAreaCorner);
        }

        BottomLeftAreaCorner = new Vector2Int(x, bottomStructure.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + this.corridorWidth, topStructure.BottomLeftAreaCorner.y);
    }

    private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft, Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0 ),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall,0)
                ).x;
        }

        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall,0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall,0)
                ).x;
        }

        if (bottomNodeLeft.x >= topNodeLeft.x && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall,0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall,0)
                ).x;
        }

        if (bottomNodeRight.y <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall,0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall,0)
                ).x;
        }

        return -1;
    }

    private void ProcessRoomInRelationRightOrLeft(Node structure1, Node structure2)
    {
        Node leftStructure = null;
        List<Node> leftStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeafes(structure1);

        var sortedLeftStructures = leftStructureChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();

        if (sortedLeftStructures.Count == 1)
        {
            leftStructure = sortedLeftStructures[0];
        }
        else 
        {
            int maxX = sortedLeftStructures[0].TopRightAreaCorner.x;
            sortedLeftStructures = sortedLeftStructures.Where(children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftStructures.Count);
            leftStructure = sortedLeftStructures[index];
        }

        Node rightStructure = null;
        List<Node> rightStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeafes(structure2);

        var possibleNeighboursInRightStructureList = rightStructureChildren.Where(
                child => GetValidYForNeighbourLeftRight
                (
                    leftStructure.TopRightAreaCorner,
                    leftStructure.BottomRightAreaCorner,
                    child.TopLeftAreaCorner,
                    child.BottomLeftAreaCorner
                    ) != -1
            ).ToList();

        if (possibleNeighboursInRightStructureList.Count <= 0)
        {
            rightStructure = structure2;
        }
        else 
        {
            rightStructure = possibleNeighboursInRightStructureList[0];
        }

        int y = GetValidYForNeighbourLeftRight(leftStructure.TopLeftAreaCorner, leftStructure.BottomRightAreaCorner, rightStructure.TopLeftAreaCorner, rightStructure.BottomLeftAreaCorner);

        while (y == -1 && sortedLeftStructures.Count > 1)
        {
            sortedLeftStructures = sortedLeftStructures.Where(child => child.TopLeftAreaCorner.y != leftStructure.TopLeftAreaCorner.y).ToList();
            leftStructure = sortedLeftStructures[0];
            y = GetValidYForNeighbourLeftRight(leftStructure.TopLeftAreaCorner, leftStructure.BottomRightAreaCorner, rightStructure.TopLeftAreaCorner, rightStructure.BottomLeftAreaCorner);
        }

        BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x, y);
        TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, y + this.corridorWidth);
    }

    private int GetValidYForNeighbourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        return -1;
    }

    private RelativePosition CheckPositionStructure2AgainstStructure1()
    {
        Vector2 middlePointStructure1Temp = ((Vector2)structure1.TopLeftAreaCorner + structure1.BottomLeftAreaCorner) / 2;
        Vector2 middlePointStructure2Temp = ((Vector2)structure2.TopLeftAreaCorner + structure2.BottomLeftAreaCorner) / 2;
        float angle = CalculateAngle(middlePointStructure1Temp, middlePointStructure2Temp);

        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if (angle > 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if (angle > -135 && angle < 45)
        {
            return RelativePosition.Down;
        }
        else 
        {
            return RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 middlePointStructure1Temp, Vector2 middlePointStructure2Temp)
    {
        return Mathf.Atan2(middlePointStructure2Temp.y-middlePointStructure1Temp.y, 
            middlePointStructure2Temp.x - middlePointStructure1Temp.x) * Mathf.Rad2Deg;
    }
}