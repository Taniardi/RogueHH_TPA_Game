using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        player.animator.Play(player.PLAYER_IDLE_ANIMATION_NAME);
    }

    public override void ExitState()
    {
        
    }

    public override void Update()
    {
        if (player.gameManager.Turn == GameManager.TURN.PLAYERTURN && !player.isInAction)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, player.TileLayerMask))
            {
                Tile temp = player.mazeGenerator.GetTile(hit.transform.position);
                if (temp == null) return;
                if (player.gameManager.listEnemy.Any(x => Utils.GetInstance().CheckPositionXZ(temp, x.currentTile)))
                { 
                    player.ChangeColorOriginal(player.ColoringTile);

                    if (Input.GetMouseButtonDown(0) )
                    {
                        Debug.Log("ada musuh yang ke raycast");
                        List<int> moveX = new List<int>() { 1, -1, 0, 0 };
                        List<int> moveY = new List<int>() { 0, 0, 1, -1 };

                        Tile enemyRaycastTile = player.gameManager.listEnemy.Where(x => Utils.GetInstance().CheckPositionXZ(temp, x.currentTile)).FirstOrDefault().currentTile;
                        List<Tile> validUserTile = new List<Tile>();

                        for (int i = 0; i < 4; i++)
                        {
                            int newX = (int)player.currentTile.position.x + moveX[i];
                            int newY = (int)player.currentTile.position.y + moveY[i];

                            if (newX < 0 || newX >= player.gameManager.mazeGenerator.widthMap) continue;

                            if (newY < 0 || newY >= player.gameManager.mazeGenerator.heigthMap) continue;

                            if (player.gameManager.mazeGenerator.Map[newY][newX] == null) continue;

                            Tile.TileCondition tileCondition = player.gameManager.mazeGenerator.Map[newY][newX].tileCondition;
                            if (tileCondition != Tile.TileCondition.CLEAR && tileCondition != Tile.TileCondition.PATTERN && tileCondition != Tile.TileCondition.GATE) continue;

                            validUserTile.Add(player.gameManager.mazeGenerator.Map[newY][newX]);
                        }

                        if(validUserTile.Any(x => x == enemyRaycastTile))
                        {
                            player.PlayerAttack(player.gameManager.listEnemy.Where(x => Utils.GetInstance().CheckPositionXZ(temp, x.currentTile)).FirstOrDefault());
                        }
                    }
                    return;
                }
                if (temp != null)
                {
                    if (temp.tileCondition == Tile.TileCondition.DECORETED) return;

                    player.aStar.Initiate(player.mazeGenerator.Map, player.currentTile, temp, player.gameManager.listEnemy.Select(x => x.currentTile).ToList());
                    List<Tile> tempColor = player.aStar.Solve();
                    
                    bool isSolved = tempColor.Any();

                    if (!isSolved)
                    {
                        tempColor.Clear();
                        tempColor.Add(temp);
                    }

                    player.ChangeColorOriginal(player.ColoringTile.Where(x => !tempColor.Any(y => y == x)).ToList());
                    player.ColoringTile = tempColor;
                    player.ChangeColor(tempColor);

                    if (Input.GetMouseButtonDown(0) && tempColor.Count >= 1)
                    {
                        if (isSolved)
                        {
                            Debug.Log("masuk");
                            List<Tile> tempMove = new List<Tile>(tempColor);
                            tempMove.Reverse();
                            player.ChangeColorOriginal(player.ColoringTile);
                            player.gameManager.AddUserAction(player.PlayerMove, tempMove, player.gameManager.listEnemy.Any(x => x.enemyState == Enemy.EnemyState.AGRO));
                        }
                    }
                }
            }
            else
            {
                player.ChangeColorOriginal(player.ColoringTile);
                player.ColoringTile.Clear();
            }
        }
        if (player.rigitBody.velocity.magnitude > 0.1f)
        {
            stateMachine.ChangeState(player.walkingState);
        }
    }
}
