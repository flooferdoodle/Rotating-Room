using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    public float speed = 5f;
    public Transform spriteTransform; // Assign the sprite child in the inspector
    public Camera mainCamera;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Ensure a Rigidbody is attached
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Auto-assign the main camera
        }
    }

    void Update()
    {
        // Get movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Move the character in the X-Z plane
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        rb.velocity = moveDirection * speed;

        // Rotate the sprite to always face the camera
        if (mainCamera != null)
        {
            spriteTransform.LookAt(mainCamera.transform);
            spriteTransform.rotation = Quaternion.Euler(0, spriteTransform.rotation.eulerAngles.y + 180, 0);
        }
    }
}

