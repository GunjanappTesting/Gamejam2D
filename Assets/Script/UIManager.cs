using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text runTimeScoreText;
    public Text bestScoreText;
    public GameObject replayPanel,playPanal;
    public Button replayButton, playButton;

    private float score = 0f;
    private float bestScore = 0f;

    void Awake()
    {
        InitializeUI();
    }

    private void OnEnable()
    {
        GameManager.GameEnd += GameOver;
    }

    private void OnDisable()
    {
        GameManager.GameEnd -= GameOver;
    }

    void Update()
    {
        if (GameManager.isAlive)
        {
            UpdateScore();
        }
    }

    private void InitializeUI()
    {
        replayButton.onClick.AddListener(ReplayGame);
        //playButton.onClick.AddListener(ReplayGame);
        replayPanel.SetActive(false);
        //playPanal.SetActive(true);
        bestScore = PlayerPrefs.GetFloat("BestScore", 0f);
        bestScoreText.text = "Best Score: " + bestScore.ToString("0");

    }

    private void UpdateScore()
    {
        score += Time.deltaTime;
        scoreText.text = "Score: " + score.ToString("0");
        runTimeScoreText.text = "Score: " + score.ToString("0");
    }

    public void GameOver()
    {
        replayPanel.SetActive(true);
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetFloat("BestScore", bestScore);
        }
    }

    private void ReplayGame()
    {
        Time.timeScale = 1;
        GameManager.isAlive = true;
        replayPanel.SetActive(false);
        //playPanal.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
}
