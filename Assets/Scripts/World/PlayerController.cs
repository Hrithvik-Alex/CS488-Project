using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created with help from https://www.youtube.com/watch?v=PmIPqGqp8UY&t=1118s
public class PlayerController : MonoBehaviour
{

    [SerializeField] Transform playerCamera;    // Start is called before the first frame update
    [SerializeField] float mouseSens = 3.5f;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.3f;



    [SerializeField] bool lockCursor = true;

    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseVelocity = Vector2.zero;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseVelocity, mouseSmoothTime);

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSens);

        transform.Rotate(-Vector3.right * currentMouseDelta.y * mouseSens);

    }

    void UpdateMovement()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentVelocity, moveSmoothTime);

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed;

        controller.Move(velocity * Time.deltaTime);
    }


}
