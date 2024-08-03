using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDropdownMenu : MonoBehaviour{
    public void switchMode(int value){
        switch (value){
            case 0:
                PlayerPrefs.SetInt("typeMovement", MovementEnum.FORSE);
                break;
            case 1:
                PlayerPrefs.SetInt("typeMovement", MovementEnum.CONSTANT);
                break;
            default:
                PlayerPrefs.SetInt("typeMovement", MovementEnum.CONSTANT);
                break;
        }
    }

}
