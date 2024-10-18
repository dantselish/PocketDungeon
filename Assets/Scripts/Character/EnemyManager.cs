using System.Collections.Generic;

public class EnemyManager : CharacterManager
{
    public override bool IsHero => false;


    public void MoveToPosition(Tile tile)
    {
        List<Tile> path = GM.GridManager.GetPathToTile(GetMyTile().Coordinates, tile.Coordinates, Stats.RemainingSpeed.Value, true, out int distance);

        if (Stats.TrySpendMovement(distance))
        {
            Move(path);
        }
    }

    public void TryAttackPlayer(HeroManager heroManager)
    {
        TryAttack(heroManager, heroManager.CurrentTile, true);
    }
}
