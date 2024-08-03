using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    public PlayerController playerController;


    public void loadMainMenu()
    {
        playerController.abortThread();

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
