using UnityEngine;
using System.IO;
using System.Text;

internal class GameLogger : MonoBehaviour
{
    string filename = "logfile.txt";

    private void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        using(FileStream fs = File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.Read))
        {
            var bytes = Encoding.UTF8.GetBytes("[" + System.DateTime.Now + "] " + logString + "\n");
            fs.Write(bytes, 0, bytes.Length);
        }
        
    }
}