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
        Debug.Log("PlayerScript started");
    }

    void Update()
    {
        moveInput = Vector2.zero;
        if (canMove)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;           
        }
        /*
        if(Input.anyKeyDown)
        {
            cameraScript.isInformationBoxActive = false;
        }*/
        moveInput = moveInput.normalized;
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }
    
}
