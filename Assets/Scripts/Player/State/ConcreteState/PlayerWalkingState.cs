using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public PlayerWalkingState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        player.animator.Play(player.PLAYER_MOVE_ANIMATION_NAME);
    }

    public override void ExitState()
    {

    }

    public override void Update()
    { 
        if(player.gameManager.Turn == GameManager.TURN.PLAYERTURN )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, player.TileLayerMask))
            {
                Tile temp = player.mazeGenerator.GetTile(hit.transform.position);
                if (temp != null)
                {
                    if (player.gameManager.listEnemy.Any(x => Utils.GetInstance().CheckPositionXZ(temp, x.currentTile))) return;
                    if (temp.tileCondition == Tile.TileCondition.DECORETED) return;

                    List<Tile> tempColor = new List<Tile>();
                    tempColor.Add(temp);
                    player.ChangeColorOriginal(player.ColoringTile.Where(x => !tempColor.Any(y => y == x)).ToList());
                    player.ColoringTile = tempColor;
                    player.ChangeColor(player.ColoringTile);

                    if (Input.GetMouseButtonDown(0))
                    {
                        player.breakCoroutine =  true;
                        player.stateMachine.currentState = player.idleState; 
                    }
                }
            }
            else
            {
                player.ChangeColorOriginal(player.ColoringTile);
                player.ColoringTile.Clear();
            }
        }
        if (player.rigitBody.velocity.magnitude < 0.1f)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
