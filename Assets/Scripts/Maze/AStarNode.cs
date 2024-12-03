using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode 
{
    public Tile currentTile { get; set; }
    public AStarNode before { get; set; }
    public bool visited { get; set; }
    public int moveCount { get; set; }

    public AStarNode(Tile currentTile)
    {
        this.currentTile = currentTile;
        this.before = null;
        this.visited = false;
        this.moveCount = 0;
    }
}
