using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;

    public TMP_Text yearUI;
    public TMP_Text populationUI;
    public TMP_Text foodUI;
    public TMP_Text woodUI;
    public TMP_Text stoneUI;
    public TMP_Text scoreUI;

    public Button shelterButton;
    public Button barracksButton;
    public Button farmButton;
    public Button lumberyardButton;
    public Button mineButton;

    public GameObject buttonMenu;
    public GameObject endMenu;
    public GameObject pauseMenu;

    public TMP_Text[] namesUI;
    public TMP_Text[] scoresUI;
   
    public Image fadeImage;
    public float fadeSpeed = 0.1f;

    bool isPaused = false;
    bool isFading;

    private void Awake()
    {
        isFading = true;
    }

    private void Update()
    {
        yearUI.text = gameManager.year.ToString();
        populationUI.text = gameManager.population.ToString();
        foodUI.text = gameManager.food.ToString();
        woodUI.text = gameManager.wood.ToString();
        stoneUI.text = gameManager.stone.ToString();

        if(gameManager.wood < 200 || gameManager.stone < 200)
        {
            shelterButton.interactable = false;
            barracksButton.interactable = false;
            farmButton.interactable = false;
            lumberyardButton.interactable = false;
            mineButton.interactable = false;
        }
        else
        {
            shelterButton.interactable = true;
            barracksButton.interactable = true;
            farmButton.interactable = true;
            lumberyardButton.interactable = true;
            mineButton.interactable = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (isFading)
        {
            Fade();
        }
    }

    public void PlaceBuidling(int type)
    {
        GameObject prefab = GetPrefab(type);
        gameManager.buildingToPlace = prefab;
        gameManager.cameraController.icon.color = prefab.GetComponent<SpriteRenderer>().color;
        gameManager.isPlacingBuilding = true;
    }

    public void SellBuildinh()
    {
        gameManager.isSelling = true;
    }

    GameObject GetPrefab(int i)
    {
        switch (i)
        {
            case 0:
                return gameManager.shelterPrefab;
            case 1:
                return gameManager.barracksPrefab;
            case 2:
                return gameManager.farmPrefab;
            case 3:
                return gameManager.lumberyardPrefab;
            default:
                return gameManager.minePrefab;
        }
    }

    public void Pause()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(false);
            buttonMenu.SetActive(true);
            gameManager.ResumeGame();
            isPaused = false;
            AudioManager.instance.StartMusic();
        }
        else
        {
            pauseMenu.SetActive(true);
            buttonMenu.SetActive(false);
            gameManager.PauseGame();
            isPaused = true;
            AudioManager.instance.StopMusic();
        }
    }

    public void Restart()
    {
        if (endMenu.activeSelf)
        {
            endMenu.SetActive(false);
        }

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        AudioManager.instance.StartMusic();
    }

    public void Quit(string scene)
    {
        if (endMenu.activeSelf)
        {
            endMenu.SetActive(false);
        }
        SceneManager.LoadScene(scene);
    }

    void Fade()
    {
        fadeImage.color = Color.Lerp(Color.black, Color.clear, Time.timeSinceLevelLoad * fadeSpeed);
        if(Mathf.Approximately(fadeImage.color.a, Color.clear.a))
        {
            isFading = false;
            fadeImage.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        int score = gameManager.score + gameManager.year * gameManager.maxPopulation;

        SendScore(score);
        SetLeaderboard();
        scoreUI.text = score.ToString();

        Time.timeScale = 0;
        endMenu.SetActive(true);
    }

    void SendScore(int score)
    {
        HighScores.UploadScore("Bob", score);
    }

    void SetLeaderboard()
    {

    }
}
