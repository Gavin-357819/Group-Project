using UnityEngine;

public class Crouch : MonoBehaviour
{

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        controller.height = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            controller.height = 1.0f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            controller.height = 2.0f;
        }

    }
}
