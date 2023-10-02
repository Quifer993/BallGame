using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScript : MonoBehaviour
{
    void OnTriggerEnter()
    {
        FindObjectOfType<GameManager>().endGame(3f, "End");
    }
}

