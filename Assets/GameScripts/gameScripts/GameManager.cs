using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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
            playerController.abortThread();
            if (typeEndGame == "fail" && !isEndGame)
            {
                Invoke("restart", timeoutRestart);
            }
            else if (typeEndGame == "End")
            {
                textWin.text = "YOU WON!";
                isEndGame = true;
                if (PlayerPrefs.GetString("Automatic").Equals("true")) {

                    Invoke("loadLevel", timeoutRestart);
                }
                else {
                    Invoke("loadMainMenu", timeoutRestart);
                }
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
        Debug.Log(num);
        SceneManager.LoadScene("level" + ++num, LoadSceneMode.Single);
    }
}
