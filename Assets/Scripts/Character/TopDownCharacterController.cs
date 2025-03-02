using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPickup))]
public class TopDownCharacterController : MonoBehaviour
{
    // Singleton Setup
    #region Singleton
    private static TopDownCharacterController _instance;
    public static TopDownCharacterController Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    // Movement speed in units per second
    public float moveSpeed = 5f;
    public float verticalMoveScale = 0.5f;

    private PlayerPickup _playerPickup;
    public bool DialInUse = false;

    private void Start()
    {
        _playerPickup = GetComponent<PlayerPickup>();
    }

    // Pre-calculated directional vectors for a typical isometric view.
    private Vector3 upDirection = new Vector3(1, 1, 0).normalized;
    private Vector3 rightDirection = new Vector3(1, -1, 0).normalized;

    void Update()
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

    public void UseDial()
    {
        // Using dial prevents pickup, so force player to drop item
        _playerPickup.DropCurrentObject();
    }
}
