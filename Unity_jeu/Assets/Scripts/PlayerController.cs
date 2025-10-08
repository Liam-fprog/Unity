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
    public Light flashlight;
    public SphereCollider soundCollision;

    [Header("Configuration")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float mouseSensitivity = 2f;

    private float verticalLookRotation = 0f;

    void Start()
    {
        // Gestion du curseur
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Lampe éteinte au départ
        if (flashlight != null)
            flashlight.enabled = false;

        // Ajout du collider de bruit
        soundCollision = gameObject.AddComponent<SphereCollider>();
        soundCollision.isTrigger = true;
        soundCollision.radius = 0f;

        // Vérifie que le Rigidbody est bien configuré
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleFlashlight();
        HandleSoundRadius();
        Debug.Log("Rayon actuel : " + soundCollision.radius);
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (rb == null) return;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        float inputX = 0f;
        if (Input.GetKey(KeyCode.D)) inputX += 1f;
        if (Input.GetKey(KeyCode.A)) inputX -= 1f;

        float inputZ = 0f;
        if (Input.GetKey(KeyCode.W)) inputZ += 1f;
        if (Input.GetKey(KeyCode.S)) inputZ -= 1f;

        Vector3 moveDir = (transform.forward * inputZ + transform.right * inputX).normalized;

        Vector3 currentVel = rb.linearVelocity;

        Vector3 targetVel = new Vector3(moveDir.x * speed, currentVel.y, moveDir.z * speed);

        rb.linearVelocity = targetVel;
    }

    void HandleMouseLook()
    {
        // Rotation horizontale du corps (gauche/droite)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // Rotation verticale de la tête
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -85f, 85f);

        if (head != null)
        {
            head.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
        }

        if (hand != null && head != null)
        {
            hand.rotation = head.rotation;
        }
    }

    void HandleFlashlight()
    {
        if (flashlight == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    void HandleSoundRadius()
    {
        if (soundCollision == null) return;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            soundCollision.radius = 30f;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            soundCollision.radius = 15f;
        }
        else
        {
            soundCollision.radius = 0f;
        }
    }
}
