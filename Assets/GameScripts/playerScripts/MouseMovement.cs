using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : TypeOfMovement{
    private float y;
    public void move(Rigidbody rb, Vector3 movement, float speed) {
        Vector3 mousePos = Input.mousePosition;
        
        if (Physics.CheckSphere(rb.position - new Vector3(0f, 0.5f, 0f), 0.1f, ~LayerMask.GetMask("Player"))){
            y = -0.1f;
        }else {
            y -= 9.81f * (float)0.004;
        }
        Vector3 mousePosCenter = new Vector3(mousePos.x / Screen.width - 0.5f, y, mousePos.y / Screen.height - 0.5f);
        rb.velocity = mousePosCenter * speed * (float)1.5;
    }
}
