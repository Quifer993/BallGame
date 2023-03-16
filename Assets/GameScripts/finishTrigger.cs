/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;


public class finishTrigger : MonoBehaviour{
    void OnTriggerEnter()
    {
        
        FindObjectOfType<GameManager>().endGame(3f, "End");
    }
}
