using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   private const float moveSpeed = 7.5f;
    private const float cameraSpeed = 3.0f;

    public Quaternion TargetRotation { private set; get; }
    
    private Vector3 moveVector = Vector3.zero;
    private float moveY = 0.0f;

    private new Rigidbody rigidbody; 
public float Speed;
    //  Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Speed * Time.deltaTime);
        }

        // Rotate the camera.
        var rotation = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        var targetEuler = TargetRotation.eulerAngles + (Vector3)rotation * cameraSpeed;
        if(targetEuler.x > 180.0f)
        {
            targetEuler.x -= 360.0f;
        }
        targetEuler.x = Mathf.Clamp(targetEuler.x, -75.0f, 75.0f);
        TargetRotation = Quaternion.Euler(targetEuler);

        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, 
            Time.deltaTime * 15.0f);
    }
}