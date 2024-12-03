using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AStar 
{

    private List<List<AStarNode>> listNode;
    private Tile startTile;
    private Tile targetTile;
    private List<Tile> notMoveableTile;

    public AStar()
    {
        listNode = new List<List<AStarNode>>();
    }

    public AStar Initiate(List<List<Tile>> listOfTile, Tile startTile, Tile targetTile, List<Tile> notMoveableTile)
    {
        this.notMoveableTile = notMoveableTile;

        listNode.Clear();
        for (int i = 0; i < listOfTile.Count; i++)
        {
            List<AStarNode> tempList = new List<AStarNode>();
            for (int j = 0; j < listOfTile[i].Count; j++)
            {
                tempList.Add(new AStarNode(listOfTile[i][j]));
            }
            listNode.Add(tempList);
        }

        this.startTile = startTile;
        this.targetTile = targetTile;

        return this;
    }

    public List<Tile> Solve(Enemy enemy = null)
    {
        List<int> moveX = new List<int>() { 1, -1, 0, 0};   
        List<int> moveY = new List<int>() { 0, 0, 1, -1};
        
        List<AStarNode> listTile = new List<AStarNode>();

        listTile.Add(listNode[(int)startTile.position.y][(int)startTile.position.x]);

        AStarNode temp = listTile.FirstOrDefault();

        while (listTile.Count > 0)
        {
            listTile = listTile.Where(x => x != null && x.currentTile != null && x.currentTile.tileGameObject != null).ToList();
            listTile = listTile.OrderBy(x => x.moveCount).ThenBy(x => Mathf.Abs(x.currentTile.tileGameObject.transform.position.x - targetTile.tileGameObject.transform.position.x) + Mathf.Abs(x.currentTile.tileGameObject.transform.position.z - targetTile.tileGameObject.transform.position.z)).ToList();
           
            temp = listTile.FirstOrDefault();

            if (Utils.GetInstance().CheckPositionXZ(temp.currentTile, targetTile))
            {
                break;
            }

            listTile.Remove(temp);
            temp.visited = true;

            for (int i = 0; i < 4; i++)
            {
                int newX = (int)temp.currentTile.position.x + moveX[i];
                int newY = (int)temp.currentTile.position.y + moveY[i];
                if (newX < 0 || newY < 0 || newY >= listNode.Count || newX >= listNode[0].Count) continue;

                AStarNode checkNode = listNode[newY][newX];


                if (checkNode.currentTile == null || checkNode.visited || listTile.Any(x => x == checkNode) ||
                    (checkNode.currentTile.tileCondition != Tile.TileCondition.CLEAR && checkNode.currentTile.tileCondition != Tile.TileCondition.PATTERN && checkNode.currentTile.tileCondition != Tile.TileCondition.GATE))
                {
                    if (checkNode.currentTile != targetTile) continue;
                }

                Vector3 checkTilePosition = checkNode.currentTile.tileGameObject.transform.position;
                if (notMoveableTile.Any(x => x.tileGameObject.transform.position.x == checkTilePosition.x && x.tileGameObject.transform.position.z == checkTilePosition.z))
                {
                    //Debug.Log("bloked");
                    continue;   
                }

                checkNode.before = temp;
                checkNode.moveCount = temp.moveCount + 1;
                listTile.Add(checkNode);
            }
        }

        if(!Utils.GetInstance().CheckPositionXZ(temp.currentTile, targetTile))
        {
            return new List<Tile>();
        }

        List<Tile> result = new List<Tile>();
        while(temp.before != null)
        {
            result.Add(temp.currentTile);
            temp = temp.before;
            //Debug.Log(temp.currentTile.tileGameObject.transform.position);
        }
        
        return result;
    }

}
