using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Vector3 asteroidPosition;
    public Vector3 asteroidDirection;
    public Vector3 asteroidVelocity;

    [SerializeField]
    public float asteroidMaxSpeed;

    Camera mainCamera;
    public float camHeight;
    public float camWidth;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        camHeight = 2f * mainCamera.orthographicSize;
        camWidth = camHeight * mainCamera.aspect;

        camHeight += 1;
        camWidth += 1;
    }

    // Update is called once per frame
    void Update()
    {
        asteroidVelocity += asteroidDirection;
        asteroidVelocity = Vector3.ClampMagnitude(asteroidVelocity, asteroidMaxSpeed);
        asteroidPosition += asteroidVelocity;

        transform.position = asteroidPosition;
        WrapBackInBounds();
    }

    protected void WrapBackInBounds()
    {
        if (asteroidPosition.x < -camWidth / 2)
        {
            asteroidPosition.x = camWidth / 2;
        }
        else if (asteroidPosition.x > camWidth / 2)
        {
            asteroidPosition.x = -camWidth / 2;
        }

        if (asteroidPosition.y < -camHeight / 2)
        {
            asteroidPosition.y = camHeight / 2;
        }
        else if (asteroidPosition.y > camHeight / 2)
        {
            asteroidPosition.y = -camHeight / 2;
        }
    }
}
