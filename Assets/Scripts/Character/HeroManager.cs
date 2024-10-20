﻿using System.Collections.Generic;
using UnityEngine;

public class HeroManager : CharacterManager
{
    public override bool IsHero => true;


    public void EndHeroTurn()
    {
        GM.LevelManager.EndHeroTurn();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CanMoveOrAttack && GM.LevelManager.TurnState == TurnState.HERO)
            {
                if (GM.LevelManager.TryGetHighlightedEnemy(out EnemyManager enemyManager))
                {
                    TryAttack(enemyManager);
                }
                else
                if (GM.GridManager.TryGetHighlightedTile(out Tile tile))
                {
                    if (!tile.TryGetEnemy(out _))
                    {
                        List<Tile> path = GM.GridManager.GetPathToTile(GetMyTile().Coordinates, tile.Coordinates, Stats.RemainingSpeed.Value, false, out int distance);

                        if (Stats.TrySpendMovement(distance))
                        {
                            Move(path);
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            EndHeroTurn();
        }
    }

    protected override void Die()
    {
        base.Die();

        Debug.Log("Hero Is Dead");
    }
}
