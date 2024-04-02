using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using System;

public class MainMenuManager : MonoBehaviour
{
    public TMPro.TMP_Text infoText;
    bool isError = false;

    private void Update()
    {
        if (isError) {
            infoText.color = new Color32(222, 22, 22, 255);
        }
        else {
            infoText.color = new Color32(244, 255, 181, 255);
        }
    }

    private bool checkError()
    {
        int[] arr = new int[5];

        PlaneScript plane = new PlaneScript();
        string port = SaveDataFromPlane.ReadFromFile(arr);
        Debug.Log(port);
        if (port.Equals("") || !plane.openComPort(port))
        {
            var myThread = new Thread(changeColor);
            myThread.Start();
            plane.closeSerialPort();
            return false;
        }
        plane.closeSerialPort();

        return true;
    }

    public void uploadLevel(int numberLevel) {
        if (checkError()) {
            PlayerPrefs.SetString("error", "");
            SceneManager.LoadScene("level" + numberLevel, LoadSceneMode.Single);
        }
        /*
        Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        Text textClickedButton = clickedButton.GetComponent<Text>();
        var componentsClickedButton = clickedButton.GetComponents(typeof(Component));
        Button textClickedButton = clickedButton.GetComponentInChildren<Button>();
        Debug.Log(clickedButton);
        Debug.Log(componentsClickedButton);

        if (textClickedButton != null)
        {
            // Получаем текст из компонента Text и выводим в консоль
            Debug.Log(textClickedButton.GetComponentInChildren<Text>());
        }*/
    }

    private void changeColor(object obj)
    {
        isError = true;
        Thread.Sleep(3500);
        isError = false;
    }

    public void loadSettingPlane()
    {
        checkError();
        AutomaticlyGame automaticlyGame = new AutomaticlyGame();
        automaticlyGame.startTime();
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
