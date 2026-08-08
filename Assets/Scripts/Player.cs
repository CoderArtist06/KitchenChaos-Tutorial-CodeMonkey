using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Movement tuning (editable in inspector)
    [SerializeField] private float moveSpeed = 7f;

    // Input System action exposed in Inspector for binding
    [SerializeField] private InputAction moveAction;

    // Current input value (x = left/right, y = foward/backward)
    [SerializeField] private Vector2 inputVector;

    private void Start()
    {
        // Enable the MoveAction so it starts reading input
        moveAction.Enable();
    }

    private void Update()
    {
        // Read the 2D vector form the MoveAction (x: horizontal, y: vertical)
        inputVector = moveAction.ReadValue<Vector2>();

        // Move the player forward/backward along local Z using the y component
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * inputVector.y);
        // Move the player left/right along local X using the x component
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed * inputVector.x);
    }
}
