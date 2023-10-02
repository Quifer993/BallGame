
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour {
    public void play() {
        SceneManager.LoadScene("Roll-a-ball", LoadSceneMode.Single);
    }

    public void getParamsPlane() {
        PlaneScript planeScr = new PlaneScript();
        string[] ports = System.IO.Ports.SerialPort.GetPortNames();
        bool isExistingComPort = false;
        string planePort = "";
        foreach (string port in ports)
        {
            if (planeScr.openComPort(port) && planeScr.getStandartInput()) {
                planePort = port;
                isExistingComPort = true;
                break;
            }
            Debug.Log(port);
        }
        if (!isExistingComPort)
        {
            Debug.Log("Нет порта, сделать вывод этого на экран");
        }
        SaveDataFromPlane.SaveToFile(planeScr.standartInput, planePort);
        SaveDataFromPlane.ReadFromFile(planeScr.standartInput);
        foreach (int i in planeScr.standartInput) {
            Debug.Log(i);
        }
        planeScr.closeSerialPort();

    }

    public void exitGame() {
        Application.Quit();
    }
}
