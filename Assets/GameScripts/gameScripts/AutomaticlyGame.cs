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
    const int MAX_TIME = 5;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI againText;
    float gameTime = 0f;
    int lifeTime;
    bool isTimeStart = false;
    public Slider slider;

    public Image imageError;

    PlaneScript planeScr;
    int[] movementVector = { 0, 0, 0, 0 };

    byte[] buffer = new byte[Constants.MESSAGE_LENGHT];
    int ir = 0;
    
    private int errorCount = 0;
    int countTry = 1;

    // Update is called once per frame
    void LateUpdate()
    {
        if (errorCount >= MAX_TIME * 2)
        {
            Debug.Log("error with platform");
            againText.text = "������� ����� : " + ++countTry;

            stopTime();
            localStartTime();
        }

        if (isTimeStart)
        {
            gameTime += 1 * Time.deltaTime;

            if (checkValuesFromCom()) {
                //logic error handler
                againText.text = "again";
                if (againText.color == Color.red)
                {
                    againText.color = Color.blue;
                }
                else
                {
                    againText.color = Color.red;
                }
                stopTime();
                localStartTime();

            }
        }
        if (gameTime >= 1)
        {
            lifeTime -= 1;
            gameTime -= 1f;
            timerText.text = lifeTime + "";
            slider.value = lifeTime;
            if (lifeTime == 0) {
                stopTime();
                timerText.text = "stop";
                //�������� ����
                PlayerPrefs.SetString("Automatic", "true");

                SceneManager.LoadScene("level1", LoadSceneMode.Single);
            }
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

        buffer = new byte[Constants.MESSAGE_LENGHT];
        ir = 0;

        //������ �����
        if (planeScr.getParamsPlane())
        {
            string comPort = SaveDataFromPlane.ReadFromFile(planeScr.standartInput);
            planeScr.openComPort(comPort);


            isTimeStart = true;
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
            //Debug.Log(1);
        }
        if (sum > 5000)
        {
            //Debug.Log(sum);
            errorCount++;
            Debug.Log("error with platform");
            //return true;
        }
        return false;
    }

    public void writeToMovementt(int value, int i)
    {
        Debug.Log("fff");
        if (i != 4)
        {
            movementVector[i] = value - planeScr.standartInput[i];
        }
    }

    public void startTime()
    {
        localStartTime();
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
