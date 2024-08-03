using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class ErrorImageScript : MonoBehaviour
{
    public TextMeshProUGUI textError;
   
    public void showError()
    {
        gameObject.SetActive(true);
        Task.Factory.StartNew(() =>
        {
            System.Threading.Thread.Sleep(6000);
            gameObject.SetActive(false);
        });
        textError.text = PlayerPrefs.GetString("error");
    }
}
