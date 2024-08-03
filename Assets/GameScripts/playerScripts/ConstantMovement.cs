using UnityEngine;

internal class ConstantMovement : TypeOfMovement{
    private float y;
    public void move(Rigidbody rb, Vector3 movement, float speed) {
        //rb.MovePosition(rb.position + movement * speed * (float)0.03);
        if (Physics.CheckSphere(rb.position - new Vector3(0f, 0.5f, 0f), 0.1f, ~LayerMask.GetMask("Player")))
        {
            y = -0.1f;
        }
        else
        {
            y -= 9.81f * (float)0.004;
        }
        var newMovement = new Vector3(movement.x, y, movement.z);
        rb.velocity = newMovement * speed;
    }
}