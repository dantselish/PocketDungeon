using System.Collections.Generic;

public class EnemyManager : CharacterManager
{
    private Outline[] _outlines;

    public override bool IsHero => false;


    public virtual void Init(LevelManager levelManager)
    {
        base.Init(levelManager);
        Move(new List<Tile>(){GM.GridManager.GetTileByCoordinates(startCoordinates)}, true);
        Invoke(nameof(LookDown), 0.1f);

        _outlines = GetComponentsInChildren<Outline>();
        SetOutlinesActive(false);

        CharacterDied += OnDeath;
    }

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
        TryAttack(heroManager, true);
    }

    private void OnMouseEnter()
    {
        SetOutlinesActive(true);
    }

    private void OnMouseExit()
    {
        SetOutlinesActive(false);
    }

    private void SetOutlinesActive(bool isActive)
    {
        if (isDead)
        {
            isActive = false;
        }

        foreach (Outline outline in _outlines)
        {
            outline.enabled = isActive;
        }
    }

    private void OnDeath(CharacterManager _)
    {
        SetOutlinesActive(false);
    }
}
