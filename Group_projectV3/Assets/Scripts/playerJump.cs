using UnityEngine;

public class playerJump : MonoBehaviour
{
    public Rigidbody2D;

    public float jumpAmount = 35;

    public float gravityScale = 10;

    public float fallingGravityScale = 40;

    void Update()

    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           object p = rb.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
        }

        if (rb.velocity.y >= 0)
        {
            rb.gravityScale = gravityScale;
        }

        else if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallingGravityScale;
        }
    }
}
  