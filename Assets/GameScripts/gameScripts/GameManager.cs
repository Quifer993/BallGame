using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public Text textWin;
    public PlayerController playerController;

    bool isEnd = false;
    bool isEndGame = false;
    public void endGame(float timeoutRestart, string typeEndGame)
    {
        if (!isEnd)
        {
            isEnd = true;
            Debug.Log("GAME OVER");
            playerController.StopMove();
            playerController.abortThread();
            if (typeEndGame == "fail" && !isEndGame)
            {
                Invoke("restart", timeoutRestart);
            }
            else if (typeEndGame == "End")
            {
                textWin.text = "Вы выйграли!";
                isEndGame = true;
//                if (PlayerPrefs.GetString("Automatic").Equals("true")) {

                    Invoke("loadLevel", timeoutRestart);
/*                }
                else {
                    Invoke("loadMainMenu", timeoutRestart);
                }*/
            }
             
        }
    }

    private void OnApplicationQuit()
    {
        playerController.abortThread();
    }

    void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    void loadLevel()
    {
        int num = int.Parse(string.Join("", SceneManager.GetActiveScene().name.Split("level")));
        String nameNextScene = "level" + ++num;

        if (nameNextScene.Equals("level7")) {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }else {
            SceneManager.LoadScene(nameNextScene, LoadSceneMode.Single);
        }
    }
}
