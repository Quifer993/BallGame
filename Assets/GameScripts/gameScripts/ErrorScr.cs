using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public static class ErrorScr
{
    public static void showError()
    {
        var errorImage = GameObject.FindGameObjectWithTag("ErrorImage");
        Debug.Log(errorImage);
        TextMeshProUGUI textMesh = errorImage.GetComponent<TextMeshProUGUI>();

        errorImage.SetActive(true);
        Task.Factory.StartNew(() =>
        {
            System.Threading.Thread.Sleep(6000);
            errorImage.SetActive(false);
        });
        textMesh.text = PlayerPrefs.GetString("error");
    }


}
