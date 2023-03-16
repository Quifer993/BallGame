using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Text textWin;

    bool isEnd = false;
    public void endGame(float timeoutRestart, string typeEndGame)
    {
        if (!isEnd)
        {
            isEnd = true;
            Debug.Log("GAME OVER");
            if (typeEndGame == "fail")
            {
                Invoke("restart", timeoutRestart);
 //               restart();
            }
            else if (typeEndGame == "End")
            {
                textWin.text = "YOU WON!";
                Invoke("restart", timeoutRestart);
//                restart();
            }

        }
    }

    void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
