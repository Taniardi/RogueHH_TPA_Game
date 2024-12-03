using RPGCharacterAnims.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject FloorTiles;
    [SerializeField] private GameObject InvisibleBox;
    [SerializeField] private LayerMask TileLayerMask;
    [SerializeField] private GameObject Player;

    [SerializeField] private List<GameObject> ListTilePattern;
    [SerializeField] private List<GameObject> ListRoomDecoration;

    public List<List<Tile>> Map;
    public List<Room> Rooms;
    public List<Tuple<Room, Room>> mstRoom;

    public int RoomCount= 15;
    private Vector2 offset;
    private System.Random rand;

    public int widthMap = 80;
    public int heigthMap = 80;

    public int minRoomWidth = 5;
    public int maxRoomWidth = 8;

    public int minRoomHeight = 5;
    public int maxRoomHeight = 8;

    public void InitializeMap()
    {
        for (int i = 0; i < heigthMap; i++)
        {
            List<Tile> temp = new List<Tile>();
            for (int j = 0; j < widthMap; j++)
            {
                temp.Add(null);
            }
            Map.Add(temp);
        }
    }


    //public void ConfigGateList(Room room1, Room room2)
    //{
    //    Vector2 distance = GetDistanceVector(room1, room2);

    //    if (distance.x < distance.y)
    //    {
    //        // TODO : KIRI ATAU KANAN
    //        if(GetMiddlePosition(room1).x < GetMiddlePosition(room2).x)
    //        {
    //            room1.gateList[1] = true;
    //            room2.gateList[3] = true;
    //        }
    //        else
    //        {
    //            room1.gateList[3] = true;
    //            room2.gateList[1] = true;
    //        }
    //    }
    //    else
    //    {
    //        // TODO : ATAS ATAU BAWAH
    //        if (GetMiddlePosition(room1).y < GetMiddlePosition(room2).y)
    //        {
    //            room1.gateList[2] = true;
    //            room2.gateList[0] = true;
    //        }
    //        else
    //        {
    //            room1.gateList[0] = true;
    //            room2.gateList[2] = true;
    //        }
    //    }

    //}

    //public Vector2 GetDistanceVector(Room room1, Room room2)
    //{
    //    Vector2 room1Mid = GetMiddlePosition(room1);
    //    Vector2 room2Mid = GetMiddlePosition(room2);

    //    float xDistance = Math.Abs(room1Mid.x - room2Mid.x);
    //    float yDistance = Math.Abs(room1Mid.y - room2Mid.y);

    //    return new Vector2(xDistance, yDistance);
    //}

    public Tile GetTile(Vector3 position)
    {
        int tempX = Mathf.FloorToInt((position.z + offset.x / 2) / offset.x );
        int tempY = Mathf.FloorToInt((position.x + offset.y / 2) / offset.y ) ;

        //if (Map[tempY][tempX] == null && Map[tempY][tempX - 1] != null)
        //{
        //    tempX -= 1;
        //}
        //if (Map[tempY][tempX] == null && Map[tempY - 1][tempX] != null)
        //{
        //    tempY -= 1;
        //}

        if (tempY >= 0 && tempY < heigthMap && tempX >= 0 && tempX < widthMap && Map[tempY][tempX] != null)
        {
            //Debug.LogWarning($"tile found at : {tempX}, Y: {tempY}");
            return Map[tempY][tempX];
        }
            
        //Debug.LogWarning($"No tile found at X: {tempX}, Y: {tempY}");
        return null;
    }


    private void Awake()
    {
        rand = new System.Random();
        RoomCount = rand.Next(15, 18);

        Rooms = new List<Room>();
        offset = new Vector2(1.2f, 1.2f);
        Map = new List<List<Tile>>();
        mstRoom = new List<Tuple<Room, Room>>();

        if(Player.GetComponent<Player>().userStatus.CurrentFloor == 0)
        {
            RoomCount = 1;
        }

        InitializeMap();
        GenerateRooms();
        MST();
        ConnectRoom();
        GenerateMap();
        GenerateDecoration();
    }
    public Tile GetUserStartPosition()
    {
        Room tempRoom = Rooms.TakeRandom();
        Tile tile = tempRoom.tiles.Where(x => x.tileCondition == Tile.TileCondition.CLEAR).ToList().TakeRandom();

        return tile;
    }

    public void DistributeEnemy(List<Enemy> listEnemy, int index, Enemy enemy, Player player)
    {
        Room tempRoom = Rooms[index%RoomCount];

        Tile tile = tempRoom.tiles.Where(x => x.tileCondition == Tile.TileCondition.CLEAR
        && x.tileGameObject.transform.position != Player.transform.position &&
        !listEnemy.Any(z => z.currentTile != null 
        && z.currentTile.tileGameObject.transform.position == x.tileGameObject.transform.position) && player.currentTile.tileGameObject.transform.position != x.tileGameObject.transform.position).ToList().TakeRandom();

        enemy.currentTile = tile;
        enemy.EnemyRefreshPosition(); 
    }
    
    public Tile GetClearTile(Room room, float rotation)
    {
        List<int> patternX = new List<int>() { 1, 0, -1, 0 };
        List<int> patternY = new List<int>() { 0, -1, 0, 1 };   

        float index = rotation / 90;
        Tile temp = room.tiles.Where(x => x.tileCondition == Tile.TileCondition.CLEAR).ToList().TakeRandom();

        do
        {
            temp = room.tiles.Where(x => x.tileCondition == Tile.TileCondition.CLEAR).ToList().TakeRandom();
            if ((int)temp.position.y + patternY[(int)index] >= room.startY + room.height || (int)temp.position.x + patternX[(int)index] >= room.startX + room.width||
                (int)temp.position.y + patternY[(int)index] < room.startY || (int)temp.position.x + patternX[(int)index] < room.startX) continue;
            if (Map[(int)temp.position.y + patternY[(int)index]][(int)temp.position.x + patternX[(int)index]] == null) break;
        
            if((Map[(int)temp.position.y + patternY[(int)index]][(int)temp.position.x + patternX[(int)index]] != null && Map[(int)temp.position.y + patternY[(int)index]][(int)temp.position.x + patternX[(int)index]].tileCondition != Tile.TileCondition.CLEAR))
            {
                continue;
            }
            else
            {
                Map[(int)temp.position.y + patternY[(int)index]][(int)temp.position.x + patternX[(int)index]].tileCondition = Tile.TileCondition.PATTERN;
                break;
            }
            
        } while (true);
        
        return temp;
    }

    public void GenerateDecoration()
    {
        foreach (var item in Rooms)
        {
            int randValue = rand.Next(Mathf.CeilToInt(item.width * item.width * 0.13f), Mathf.CeilToInt(item.width * item.width * 0.15f));
            for (int i = 0; i < randValue; i++)
            {
                float rotateValue = rand.Next(1, 4) * 90f;

                Tile temp = GetClearTile(item, rotateValue);
                temp.tileCondition = Tile.TileCondition.PATTERN;
                Vector3 tempPosition = temp.tileGameObject.gameObject.transform.position;

                GameObject tilePattern = Instantiate(ListTilePattern[rand.Next(0, ListTilePattern.Count)], new Vector3(tempPosition.x, 1.8f, tempPosition.z), Quaternion.identity, transform);
                tilePattern.transform.Rotate(new Vector3(0, rotateValue, 0));
            }
        }

        foreach (var item in Rooms)
        {
            int randValue = rand.Next(Mathf.CeilToInt(item.width * item.width * 0.1f), Mathf.CeilToInt(item.width * item.width * 0.12f));
            for (int i = 0; i < randValue; i++)
            {    
                float rotateValue = rand.Next(1, 4) * 90f;
                    
                Tile temp = GetClearTile(item, rotateValue);

                GameObject roomDecoration = ListRoomDecoration[rand.Next(0, ListRoomDecoration.Count)];

                temp.tileCondition = Tile.TileCondition.DECORETED;
                Vector3 tempPosition = temp.tileGameObject.gameObject.transform.position;

                GameObject tilePattern = Instantiate(roomDecoration, new Vector3(tempPosition.x, 1.8f, tempPosition.z), Quaternion.identity, transform);
                tilePattern.transform.Rotate(new Vector3(0, rotateValue, 0));
            }
        }

    }

    public void MST()
    {
        List<Room> roomList = new List<Room>(Rooms);
        while (roomList.FirstOrDefault() != null)
        {
            Room firstRoom = roomList.FirstOrDefault();
            roomList.Remove(firstRoom);

            Room secRoom = roomList.FirstOrDefault();

            if (secRoom == null) break;

            foreach (var item in roomList)
            {
                if (GetDistance(firstRoom, secRoom) > GetDistance(firstRoom, item))
                {
                    secRoom = item;
                }
            }

            Tuple<Room, Room> res = new Tuple<Room, Room>(firstRoom, secRoom);
            mstRoom.Add(res);
        }
    }

    public bool CheckIsGate(int y, int x)
    {
        if (!Map[y][x].isRoomTiles) return false;

        List<int> moveX = new List<int>() { 1, -1, 0, 0 };
        List<int> moveY = new List<int>() { 0, 0, 1, -1 };
        for (int i = 0; i < 4; i++)
        {
            if (y + moveY[i] < 0 || y + moveY[i] >= Map.Count) continue;
            if (x + moveX[i] < 0 || x + moveX[i] >= Map.FirstOrDefault().Count) continue;
            if (Map[y + moveY[i]][x + moveX[i]] != null && !Map[y + moveY[i]][x + moveX[i]].isRoomTiles)
            {
                Map[y][x].tileCondition = Tile.TileCondition.GATE;
                return true;
            }
        }
        return false;
    }

    public void ConnectRoom()
    {
       
        foreach (var item in mstRoom)
        {
            Vector2 room1Mid = GetMiddleVector(item.Item1);
            Vector2 room2Mid = GetMiddleVector(item.Item2);

            int room1Y = (int)room1Mid.y;
            int room2Y = (int)room2Mid.y;
            int room1X = (int)room1Mid.x;
            int room2X = (int)room2Mid.x;

            if (room1X < room2X)
            {
                for (int i = room1X; i <= room2X; i++)
                {
                    if (Map[room1Y][i] == null)
                    {
                        Map[room1Y][i] = new Tile(new Vector2(i, room1Y), false);
                    }
                }
            }
            else
            {
                for (int i = room2X; i <= room1X; i++)
                {
                    if (Map[room1Y][i] == null)
                    {
                        Map[room1Y][i] = new Tile(new Vector2(i, room1Y), false);
                    }
                }
            }

            if (room1Y < room2Y)
            {
                for (int i = room1Y; i <= room2Y; i++)
                {
                    if (Map[i][room2X] == null)
                    {
                        Map[i][room2X] = new Tile(new Vector2(room2X, i), false);
                    }
                }
            }
            else
            {
                for (int i = room2Y; i <= room1Y; i++)
                {
                    if (Map[i][room2X] == null)
                    {
                        Map[i][room2X] = new Tile(new Vector2(room2X, i), false);
                    }
                }
            }

        }
    }

    public float GetDistance(Room room1, Room room2)
    {
        Vector2 room1Mid = GetMiddlePosition(room1);
        Vector2 room2Mid = GetMiddlePosition(room2);

        float xDistance = Math.Abs(room1Mid.x - room2Mid.x);
        float yDistance = Math.Abs(room1Mid.y - room2Mid.y);

        double result = Math.Sqrt(Math.Pow(yDistance, 2) + Math.Pow(xDistance, 2));

        return (float)result;
    }

    public Vector2 GetMiddleVector(Room room)
    {
        Vector2 result = new Vector2();

        result.x = room.startX + room.width / 2;
        result.y = room.startY + room.height / 2;

        return result;
    }

    public Vector2 GetMiddlePosition(Room room)
    {
        float x = room.startX + ((float)room.width / 2);
        float y = room.startY + ((float)room.height / 2);

        return new Vector2(x, y);
    }

    public void GenerateMap()
    {
        for (int i = 0; i < heigthMap; i++)
        {
            for (int j = 0; j < widthMap; j++)
            {
                if (Map[i][j] == null)
                {
                    Instantiate(InvisibleBox, new Vector3(i * offset.x, 0.6f, j * offset.y), Quaternion.identity, transform);
                    continue;
                }
                GameObject TileGameObject = Instantiate(FloorTiles, new Vector3(i * offset.x, 0.6f, j * offset.y), Quaternion.identity, transform);
                Map[i][j].tileGameObject = TileGameObject;

                if (Map[i][j].isRoomTiles)
                {
                    CheckIsGate(i, j);
                }
            }
        }
    }

    public bool CheckRoomPosition(int yStart, int xStart, int yEnd, int xEnd)
    {
        int extendedYStart = Mathf.Max(0, yStart - 2);
        int extendedXStart = Mathf.Max(0, xStart - 2);
        int extendedYEnd = Mathf.Min(heigthMap - 1, yEnd + 2);
        int extendedXEnd = Mathf.Min(widthMap - 1, xEnd + 2);

        for (int y = extendedYStart; y <= extendedYEnd; y++)
        {
            for (int x = extendedXStart; x <= extendedXEnd; x++)
            {
                if (Map[y][x] != null)
                {
                    return false;
                }
            }
        }

        return true;
    }


    public Vector2 RandomRoomPosition(int width, int height)
    {
        Vector2 result;
        do
        {
            result.x = rand.Next(0, widthMap - width);
            result.y = rand.Next(0, heigthMap - height);

        } while (!CheckRoomPosition((int)result.y, (int)result.x, (int)result.y + height - 1, (int)result.x + width - 1));

        return result;
    }

    public void GenerateRooms()
    {
        for (int z = 0; z < RoomCount; z++)
        {
            int width = rand.Next(minRoomWidth,maxRoomWidth);
            int height = rand.Next(minRoomHeight, minRoomHeight);

            Vector2 startPosition = RandomRoomPosition(width, height);

            Room tempRoom = new Room(height, width, (int)startPosition.x, (int)startPosition.y);
            for (int y = (int)startPosition.y; y < startPosition.y + height  + 1; y++)
            {
                for (int x = (int)startPosition.x; x < startPosition.x + width + 1; x++)
                {
                    Tile tempTile = new Tile(new Vector2(x, y), true);
                    Map[y][x] = tempTile;
                    tempRoom.tiles.Add(tempTile);
                }
            }

            Rooms.Add(tempRoom);
        }
    }
}

