using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.StartAmbience();
    }

    public void LoadGame(string scene)
    {
        AudioManager.instance.StartMusic();
        SceneManager.LoadScene(scene);
    }
}
