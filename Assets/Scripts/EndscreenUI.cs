using TMPro;
using UnityEngine;

public class EndscreenUI : MonoBehaviour
{
    [SerializeField] private ScoreSO scoreSO;
    [SerializeField] private TextMeshProUGUI textMeshScore;

    private void Start()
    {
        textMeshScore.text = "Final Score: " + scoreSO.Score;
    }
}
