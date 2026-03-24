using UnityEngine;
using UnityEngine.InputSystem;


public class MovingCamScript : MonoBehaviour
{
    public CameraScript cameraScript;
    public float moveSpeed = 5f;
    public bool canMove = true;

    Rigidbody2D rb;
    Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        moveInput = Vector2.zero;
        if (canMove)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;           
        }

        if (Input.anyKeyDown &&
        !Input.GetMouseButtonDown(0) &&
        !Input.GetMouseButtonDown(1) &&
        !Input.GetMouseButtonDown(2))
            cameraScript.isInformationBoxActive = false;

        moveInput = moveInput.normalized;
        
        if(keyboard.shiftKey.isPressed)
            moveSpeed = 15f;
        else
            moveSpeed = 5f;
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }
    
}
