using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    [Header("References")]
    public Rigidbody rb;
    public Transform head;
    public Camera camera;
    public Transform hand;

    [Header("Configuration")]
    public float walkSpeed;
    public float runSpeed;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 2f);
    }

    void FixedUpdate()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        //hand.localRotation = Quaternion.Euler(inputX, inputZ, 0);

        Vector3 move = (transform.forward * inputZ + transform.right * inputX).normalized * speed;

        Vector3 newVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        rb.linearVelocity = newVelocity;
    }

    void LateUpdate()
    {
        Vector3 e = head.eulerAngles;
        e.x -= Input.GetAxis("Mouse Y") * 2f;
        e.x = RestrictAngle(e.x, -85f, 85f);
        head.eulerAngles = e;
        hand.rotation = head.rotation;
    }

    public static float RestrictAngle(float angle, float angleMin, float angleMax)
    {
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        if (angle > angleMax)
            angle = angleMax;
        if (angle < angleMin)
            angle = angleMin;

        return angle;

    }
}
