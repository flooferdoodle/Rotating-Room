using UnityEngine;

public class RandyMovement3D : MonoBehaviour
{
    // Movement speed in units per second
    public float moveSpeed = 5f;
    private Vector3 upDirection = new Vector3(1, 0, 1).normalized; private Vector3 downDirection = new Vector3(-1, 0, -1).normalized;
    private Vector3 leftDirection = new Vector3(-1, 0, 1).normalized; private Vector3 rightDirection = new Vector3(1, 0, -1).normalized;

    // Camera that sprite tracks
    public Camera mainCamera;

    // Find camera
    /*void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }*/

    // Using FixedUpdate for smooth physics-based movement
    void FixedUpdate()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir += upDirection;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir += downDirection;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir += leftDirection;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir += rightDirection;
        }

        // Only move if a key is pressed
        if (moveDir != Vector3.zero)
        {
            moveDir.Normalize();
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }
}