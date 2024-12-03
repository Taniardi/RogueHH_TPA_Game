using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2 position { get; set; }
    public GameObject tileGameObject { get; set; }
    public TileCondition tileCondition { get; set; }
    public bool isRoomTiles { get; set; }

    public Tile(Vector2 position, bool isRoomTiles)
    {
        this.position = position;
        tileCondition = TileCondition.CLEAR;
        this.isRoomTiles = isRoomTiles;
    }

    public enum TileCondition
    {
        CLEAR,
        PATTERN,
        DECORETED,
        GATE
    }
}
