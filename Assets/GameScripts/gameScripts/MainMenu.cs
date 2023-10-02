using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour {
    public void play() {
        string[] ports = System.IO.Ports.SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            Debug.Log(port);
        }
        SceneManager.LoadScene("Roll-a-ball", LoadSceneMode.Single);
    }

    public void getParamsPlane() {
        PlaneScript planeScr = new PlaneScript();
        planeScr.openComPort();
        planeScr.getStandartInput();
        SaveDataFromPlane.SaveToFile(planeScr.standartInput);
        SaveDataFromPlane.ReadFromFile(planeScr.standartInput);
        foreach (int i in planeScr.standartInput) {
            Debug.Log(i);
        }
        planeScr.abortThread();

    }

    public void exitGame() {
        Application.Quit();
    }
}
