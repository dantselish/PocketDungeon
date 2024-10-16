using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private DebugUI DebugUI;

    public GridManager  GridManager  { get; private set; }
    public LevelManager LevelManager { get; private set; }
    public DiceManager  DiceManager  { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
        }

        FindReferences();
        Initialize();
    }

    private void FindReferences()
    {
        GridManager = FindObjectOfType<GridManager>();
        LevelManager = FindObjectOfType<LevelManager>();
        DiceManager = FindObjectOfType<DiceManager>();
    }

    private void Initialize()
    {
        GridManager.Init();
        LevelManager.InitLevel();
 
        DebugUI.Init();
    }
}

