using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveDataFromPlane {
    public const string fileName = "data.txt";

    public static void SaveToFile(int[] standartInput, string planePort)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            writer.WriteLine(planePort);
//            Debug.Log(planePort);
            foreach (int element in standartInput)
            {
                writer.WriteLine(element);
//                Debug.Log(element);
            }
        }
    }

    public static string ReadFromFile(int[] standartInput) {
        string comPort = "";
        if (File.Exists(fileName))
        {
            string[] lines = File.ReadAllLines(fileName);
            comPort = lines[0];
            for (int i = 1; i < lines.Length; i++)
            {
                int value;
                if (int.TryParse(lines[i], out value))
                {
                    standartInput[i - 1] = value;
                }
                else
                {
                    Console.WriteLine("Invalid value at line " + (i + 1));
                }
            }
        }
        else
        {
            Console.WriteLine("File does not exist");
        }
        return comPort;
    }
}
