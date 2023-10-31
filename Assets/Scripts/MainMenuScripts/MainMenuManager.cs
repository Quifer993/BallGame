using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void uploadLevel(int numberLevel) {
        SceneManager.LoadScene("level" + numberLevel, LoadSceneMode.Single);



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
}
