using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    public int height { get; set; }
    public int width { get; set; }
    public int startX { get; set; }
    public int startY { get; set; }
    public List<Tile> tiles { get; set; }
    public Room(int height, int width, int startX, int startY)
    {
        tiles = new List<Tile>();

        this.height = height;
        this.width = width;
        this.startX = startX;
        this.startY = startY;
    }
}
