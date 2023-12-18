
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void play() {
        PlayerPrefs.SetString("error", "");
        
        SceneManager.LoadScene("level1", LoadSceneMode.Single);
    }

    public void getParamsPlane() {
        PlaneScript planeScr = new PlaneScript();
        planeScr.getParamsPlane();

    }

    public void exitGame() {
        Application.Quit();
    }
}
