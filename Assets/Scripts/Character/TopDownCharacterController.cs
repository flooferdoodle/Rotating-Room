using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterController : MonoBehaviour
{
    // Movement speed in units per second
    public float moveSpeed = 5f;
    public float verticalMoveScale = 0.5f;

    // Pre-calculated directional vectors for a typical isometric view.
    // Adjust these if your camera rotation differs.
    // Up arrow: moves diagonally (1, 0, 1)
    // Down arrow: moves diagonally (-1, 0, -1)
    // Left arrow: moves diagonally (-1, 0, 1)
    // Right arrow: moves diagonally (1, 0, -1)
    private Vector3 upDirection = new Vector3(1, 1, 0).normalized;
    private Vector3 rightDirection = new Vector3(1, -1, 0).normalized;

    // Using FixedUpdate for smooth physics-based movement
    void FixedUpdate()
    {
        Vector2 moveDir = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += Vector2.down;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += Vector2.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector2.right;
        }

        // Only move if a key is pressed
        if (moveDir != Vector2.zero)
        {
            moveDir.Normalize();
            Vector3 movement = moveDir.x * rightDirection + moveDir.y * upDirection * verticalMoveScale;
            transform.position += movement * moveSpeed * Time.deltaTime;
        }
    }
}
