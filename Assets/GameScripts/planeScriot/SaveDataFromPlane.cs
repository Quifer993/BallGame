using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveDataFromPlane {
    public const string fileName = "data.txt";

    public static void SaveToFile(int[] standartInput)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (int element in standartInput)
            {
                writer.WriteLine(element);
            }
        }
    }

    public static void ReadFromFile(int[] standartInput)
    {
        if (File.Exists(fileName))
        {
            string[] lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i++)
            {
                int value;
                if (int.TryParse(lines[i], out value))
                {
                    standartInput[i] = value;
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
    }
}
