using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle : MonoBehaviour
{
    // Position of Vehicle in world space
    public Vector3 vehiclePosition;

    // Direction the vehicle is facing
    public Vector3 direction = new Vector3(1, 0, 0);
    private Vector3 velocity = new Vector3 (0, 0, 0);

    // Accel vector will cause the rate of change per second
    Vector3 acceleration = new Vector3(0, 0, 0);
    public float accelerationRate;

    // Speed limit to avoid going too fast to control
    public float maximumSpeed;

    // Speed of the vehicle as it turns
    public float turnSpeed;

    // Get WASD inputs
    private Vector2 playerInput;

    // Allow for setting bounds for the camera/window
    Camera mainCamera;
    public float camHeight;
    public float camWidth;

    // Start is called before the first frame update
    void Start()
    {
        vehiclePosition = transform.position;

        // Get the size of the window as made by the main camera
        mainCamera = Camera.main;

        camHeight = 2f * mainCamera.orthographicSize;
        camWidth = camHeight * mainCamera.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        // Logic to alter the direction based on left/right input
        if (playerInput.x > 0)
        {
            // Turn right
            direction = Quaternion.Euler(0, 0, -turnSpeed) * direction;
        }
        else if (playerInput.x < 0)
        {
            // Turn left
            direction = Quaternion.Euler(0, 0, turnSpeed) * direction;
        }

        // Movement logic
        if (playerInput.y > 0)
        {
            // Accelerate
            acceleration = direction * accelerationRate;
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude(velocity, maximumSpeed);
            vehiclePosition += velocity;
        }
        else
        {
            // If it's not accelerating, it should be slowing to a stop
            velocity /= 1.1f;
            velocity = Vector3.ClampMagnitude(velocity, maximumSpeed);

            vehiclePosition += velocity;
        }

        // Logic to ensure the car does not go off screen
        if (vehiclePosition.x < -camWidth/2)
        {
            vehiclePosition.x = camWidth/2;
        }
        else if (vehiclePosition.x > camWidth/2)
        {
            vehiclePosition.x = -camWidth/2;
        }

        if (vehiclePosition.y < -camHeight/2)
        {
            vehiclePosition.y = camHeight/2;
        }
        else if (vehiclePosition.y > camHeight/2)
        {
            vehiclePosition.y = -camHeight/2;
        }

        // Move position of the vehicle to new position
        transform.position = vehiclePosition;

        // Set the vehicle's rotation to match the direction
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    public void OnMove(InputValue value)
    {
        playerInput = value.Get<Vector2>();
    }
}
