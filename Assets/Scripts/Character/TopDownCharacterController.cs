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
    private Vector3 downDirection = new Vector3(-1, -1, 0).normalized;
    private Vector3 leftDirection = new Vector3(-1, 1, 0).normalized;
    private Vector3 rightDirection = new Vector3(1, -1, 0).normalized;

    // Using FixedUpdate for smooth physics-based movement
    void FixedUpdate()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir += upDirection * verticalMoveScale;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir += downDirection * verticalMoveScale;
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
