using UnityEngine;

internal class ForseMovement : TypeOfMovement{
    public void move(Rigidbody rb, Vector3 movement, float speed) {
        rb.AddForce(movement * speed * (float)1.5);
    }
}