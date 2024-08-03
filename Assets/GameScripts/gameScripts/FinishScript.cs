using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class FinishScript : MonoBehaviour
{
    DateTime timeStart;
    private string dirResult = "Solved_result";

    private void Start()
    {
        timeStart = DateTime.Now;
    }

    private void SaveResultToFile()
    {
        TimeSpan resultTime = DateTime.Now.Subtract(timeStart);
        Directory.CreateDirectory(dirResult);
        using (StreamWriter writer = new StreamWriter(
            dirResult + "\\" + SceneManager.GetActiveScene().name + " " +
            timeStart.ToShortDateString() + "_" + timeStart.Hour + "." +
            timeStart.Minute + "." + timeStart.Second +
            ".txt"))
        {
            writer.WriteLine("Time to complete the game - \t" + resultTime.TotalSeconds + " seconds");
            var neighbors = GameObject.FindGameObjectsWithTag("Pick Up");
            writer.WriteLine("Бонусов осталось на уровне - \t" + neighbors.Length);
        }
    }

    void OnTriggerEnter()
    {
        FindObjectOfType<GameManager>().endGame(3f, "End");
        SaveResultToFile();
    }
}

