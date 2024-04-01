using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAMEOVER , GS_OPTIONS};


public class GameManager: MonoBehaviour
{
    public GameState currentGameState = GameState.GS_GAME;
    public static GameManager instance;
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text enemyText;
    public TMP_Text endScoreText;
    public TMP_Text endHighScoreText;
    public TMP_Text qualityLevel;
    public Image[] keysTab;
    public Image[] livesTab;

    public float foxXPosition = 0;
    public bool bossFightStarted = false;
    public bool bossDefeated = false;
    public GameObject bossDoor;

    public Slider volumeSlider;

    public Canvas pauseMenuCanvas;
    public Canvas inGameCanvas;
    public Canvas levelCompletedCanvas;
    public Canvas optionsCanvas;
    public Canvas tutorialCanvas;
    public Canvas gameOverCanvas;
    public Canvas notEnoughKeysCanvas;

    private float timer = 0;
    private int enemyKillCount = 0;
    private int score = 0;
    private int keysFound = 0;
    private int lives = 3;
    private bool canPause = true;
    private static string keyHighScore = "HighScore196722_197055";

    public void OnResumeButtonClicked()
    {
        InGame();
    }
    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnPlusButtonClicked()
    {
        QualitySettings.IncreaseLevel();
        qualityLevel.text = QualitySettings.names[QualitySettings.GetQualityLevel()];

    }
    public void ONMinusButtonClicked()
    {
        QualitySettings.DecreaseLevel();
        qualityLevel.text = QualitySettings.names[QualitySettings.GetQualityLevel()];

    }
    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
    }
    public void OnReturnToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void AddPoints(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }
    public void Death()
    {
            lives -= 1;
            livesTab[lives].enabled = false;
    }

    public void KillEnemy()
    {
        enemyKillCount++;
        enemyText.text = enemyKillCount.ToString();
    }
    public int GetLives()
    {
        return lives;   
    }
    public void Heal()
    {
        livesTab[lives].enabled = true;
        lives++;
    }
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this; 
        scoreText.enabled = true;
        timeText.enabled = true;
        enemyText.enabled = true;
        pauseMenuCanvas.enabled = false;
        optionsCanvas.enabled = false;
        tutorialCanvas.enabled = true;
        gameOverCanvas.enabled = false;
        notEnoughKeysCanvas.enabled = false;
        if(volumeSlider!= null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        SetVolume(0.5f);
        InGame();
        for(int i = 0; i < keysTab.Length; i++)
        {
            keysTab[i].color = Color.grey;
        }
        if(!PlayerPrefs.HasKey(keyHighScore))
        {
            PlayerPrefs.SetInt(keyHighScore, 0);
        }
     }
    void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        inGameCanvas.enabled = (currentGameState == GameState.GS_GAME);
        pauseMenuCanvas.enabled = (currentGameState == GameState.GS_PAUSEMENU);
        levelCompletedCanvas.enabled = (currentGameState == GameState.GS_LEVELCOMPLETED);
        optionsCanvas.enabled = (currentGameState==GameState.GS_OPTIONS);
        gameOverCanvas.enabled = (currentGameState == GameState.GS_GAMEOVER);
        if (currentGameState == GameState.GS_LEVELCOMPLETED)
        {
            endScoreText.text = score.ToString();
            Scene currentScene = SceneManager.GetActiveScene();
            int sceneIndex = SceneUtility.GetBuildIndexByScenePath("Scenes/Main Menu");
            if(sceneIndex >=0)
            {
                SceneManager.LoadSceneAsync(sceneIndex);
            }
            else if(currentScene.name == "196722_197055")
            {
                int highScore = PlayerPrefs.GetInt(keyHighScore);
                if (highScore < score){
                    highScore = score;
                    PlayerPrefs.SetInt(keyHighScore, highScore);
                }
                endHighScoreText.text = PlayerPrefs.GetInt(keyHighScore).ToString();
            }
        }
    }
    public void AddKeys()
    {
        keysTab[keysFound].color = Color.white;
        keysFound++;
    }

    public int GetNumberOfKeysFound()
    {
        return keysFound;
    }

    public void NotEnoughKeys()
    {
        notEnoughKeysCanvas.enabled = true;
    }
    public void Enoughkeys()
    {
        notEnoughKeysCanvas.enabled = false;
    }
    public void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
    }

    public void Options()
    {
        Time.timeScale = 0;
        SetGameState(GameState.GS_OPTIONS);
    }
    public void InGame()
    {
        Time.timeScale = 1.0f;
        SetGameState(GameState.GS_GAME);
    }
    public void LevelCompleted()
    {
        SetGameState(GameState.GS_LEVELCOMPLETED);
    }
    public void GameOver()
    {
        SetGameState(GameState.GS_GAMEOVER);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Assign the formatted time to the timeText
        timeText.text = formattedTime;
        if (Input.GetKey(KeyCode.Escape) && canPause)
        {
            if (currentGameState == GameState.GS_PAUSEMENU)
            {
                InGame();
            }
            else
            {
                PauseMenu();
            }

            StartCoroutine(BlockPause());
        }
    }

    IEnumerator BlockPause()
    {
        canPause = false;
        yield return new WaitForSeconds(0.1f);
        canPause = true;
    }
}