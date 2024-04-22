using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    [SerializeField] private float moveSpeedModifier;
    private float _horizontalAxis;
    private float _verticalAxis;

    private void Awake()
    {
        if (TryGetComponent(out _rb));
        else Debug.LogError("Failed to get the player rigidbody.");
    }

    private void Update() 
    {
        HandleMovementInput();
    }

    private void FixedUpdate()
    {
        ResolveMovement();
    }

    private void HandleMovementInput()
    {
        _verticalAxis = 0f;
        _horizontalAxis = 0f;
        
        if (Input.GetKey(KeyCode.W))
        {
            _verticalAxis = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _verticalAxis = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            _horizontalAxis = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _horizontalAxis = 1f;
        }
    }

    private void ResolveMovement()
    {
        if (_verticalAxis != 0f || _horizontalAxis != 0f)
        {
            _rb.AddForce(new Vector2(_horizontalAxis, _verticalAxis).normalized * moveSpeedModifier);
        }
    }
}
