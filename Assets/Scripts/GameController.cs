using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private GateController gateController;
    private AddNewSheep addNewSheep;
    private TileController tileController;
    
    [SerializeField]
    private ScoreSO scoreSO;

    [SerializeField] private float cycleTime = 120f;
    private float gameTime;

    private int gameCycles = 0;
    private int maxCycles = 2;

    [SerializeField] private TextMeshProUGUI textMeshScore;
    [SerializeField] private TextMeshProUGUI textMeshTime;

    private void Awake()
    {
        gateController = FindAnyObjectByType<GateController>();
        addNewSheep = FindAnyObjectByType<AddNewSheep>();
        tileController = FindAnyObjectByType<TileController>();
    }

    private void Start()
    {
        gameTime = cycleTime;
    }

    private void Update()
    {
        textMeshScore.text = "Score: " + scoreSO.Score;
        textMeshTime.text = "Time: " + (int) gameTime;

        if (gameCycles > maxCycles)
            EndGame();

        gameTime -= Time.deltaTime;


        if (gameTime <= 0)
        {
            AddSheep();
            gameCycles++;
            gameTime = cycleTime;
        }
    }

    private void AddSheep()
    {
        gateController.OpenGate();
        tileController.requiredSheep += 1;
        StartCoroutine(addNewSheep.SpawnNewSheep(2));
    }

    private void EndGame()
    {
        SceneManager.LoadScene("EndGameScene");
    }
}
