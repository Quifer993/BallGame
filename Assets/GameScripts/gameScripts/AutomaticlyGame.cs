using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Threading;

public class AutomaticlyGame : MonoBehaviour
{
    const int MAX_TIME = 3;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI againText;
    public TextMeshProUGUI aboutText;
    public TextMeshProUGUI partParsingText;
    float gameTime = 0f;
    int lifeTime;
    bool isTimeStart = false;
    public Slider slider;

    public Image imageError;

    PlaneScript planeScr;
    int[] movementVector = { 0, 0, 0, 0 };
    bool isError;

    byte[] buffer = new byte[Constants.MESSAGE_LENGHT];
    int ir = 0;
    
    private int errorCount = 0;
    int countTry = 1;

    // Update is called once per frame
    void Update() {
        if (isError) {
            aboutText.text = "Присоедините стабилометрическую платформу к компьютеру и попробуйте снова";
            partParsingText.text = "Ошибка";
        }
        else { 
            aboutText.text = "Не вставайте на платформу. Настройка платформы закончится через ";
        }
        try {
            if (errorCount >= MAX_TIME * 2) {
                Debug.Log("error with platform");
                againText.text = "Попытка номер : " + ++countTry;

                stopTime();
                localStartTime();
            }

            if (isTimeStart) {
                partParsingText.text = "(ШАГ 2/2) Проверка данных";
                gameTime += 1 * Time.deltaTime;

                if (checkValuesFromCom()) {
                    //logic error handler

                    stopTime();
                    localStartTime();

                }
            }
            if (gameTime >= 1) {
                lifeTime -= 1;
                gameTime -= 1f;
                timerText.text = lifeTime + "";
                slider.value = lifeTime;
                if (lifeTime == 0) {
                    stopTime();
                    timerText.text = "stop";
                    //загрузка меню
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
            }
        }
        catch (ArgumentNullException e) {
            Debug.Log(e.Message);
        }
        
    }

    private void localStartTime()
    {
        planeScr = new PlaneScript();
        slider.maxValue = MAX_TIME;
        lifeTime = MAX_TIME;
        slider.value = (float)lifeTime;
        timerText.text = lifeTime + "";
        gameTime = 0f;
        errorCount = 0;
        isError = false;

        buffer = new byte[Constants.MESSAGE_LENGHT];
        ir = 0;

        //логика стабилометрической платформы
        partParsingText.text = "(ШАГ 1/2) Настройка платформы";
        var threadPlane = new Thread(planeParsing);
        Debug.Log("Запуск planeParsing в потоке\n");
        threadPlane.Start();
    }

    private void planeParsing() {
        if (planeScr.getParamsPlane())
        {
            string comPort = SaveDataFromPlane.ReadFromFile(planeScr.standartInput);
            planeScr.openComPort(comPort);


            isTimeStart = true;
        }
        else
        {
            isError = true;
        }
    }

    private bool checkValuesFromCom()
    {

        Console.Write("\n");
        planeScr.workWithSp(planeScr.putCoords, writeToMovementt, buffer, ref ir);

        float sum = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += (float)Math.Abs(movementVector[i]);
        }
        if (sum > 5000)
        {
            errorCount++;
            Debug.Log("Sum-error platform");
            //return true;
        }
        return false;
    }

    public void writeToMovementt(int value, int i) {
        if (i != 4)
        {
            movementVector[i] = value - planeScr.standartInput[i];
        }
    }

    public void startTime() {
        Debug.Log("start setting plane\n");
        try
        {
            localStartTime();
        }
        catch (ArgumentNullException e)
        {
            Debug.Log(e.Message);
        }
        
        againText.text = "";
        countTry = 1;
    }

    public void stopTime()
    {
        planeScr.closeSerialPort();
        isTimeStart = false;
    }

    private void updateTimeAndSlider()
    {
        
    }
}
