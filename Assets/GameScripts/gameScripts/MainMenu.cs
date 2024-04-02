
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public TMPro.TMP_Dropdown dropDown;

    private void Start() {
        GameLogger gameLogger;

        int typeMove = PlayerPrefs.GetInt("typeMovement", MovementEnum.FORSE);
        if (typeMove == MovementEnum.FORSE || typeMove == MovementEnum.CONSTANT) {
            dropDown.value = typeMove;
        }else{
            PlayerPrefs.SetInt("typeMovement", MovementEnum.FORSE);
            dropDown.value = MovementEnum.FORSE;
        }
    }

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
