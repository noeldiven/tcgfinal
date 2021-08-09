using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firtpersoncontroller : MonoBehaviour
{
    CharacterController charactercontroller;
    public float walkspeed = 6.0f;
    public float runspeed = 10.0f;
    public float jumpspeed = 8.0f;
    public float gravity = 20.0f;

    public Camera cam;
    public float mouseHorizontal = 3.0f;
    public float mouseVertical = 2.0f;
    public float minrotation = -65.0f;
    public float maxrotation = 60.0f;
    float h_mouse, v_mouse;

    private Vector3 move = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        charactercontroller = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        h_mouse = mouseHorizontal * Input.GetAxis("Mouse X");
        v_mouse += mouseVertical * Input.GetAxis("Mouse Y");

        v_mouse = Mathf.Clamp(v_mouse, minrotation, maxrotation);
        cam.transform.localEulerAngles = new Vector3(-v_mouse, 0, 0);

        transform.Rotate(0, h_mouse, 0);

        if (charactercontroller.isGrounded)
        {
            move = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            if (Input.GetKey(KeyCode.LeftShift))
            {
                move = transform.TransformDirection(move) * runspeed;
            }
            else
            {
                move = transform.TransformDirection(move) * walkspeed;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                move.y = jumpspeed;
            }
        }
        move.y -= gravity * Time.deltaTime;
        charactercontroller.Move(move * Time.deltaTime);
    }
}
