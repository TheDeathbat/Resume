using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 bulletPosition;
    public Vector3 bulletDirection;
    private Vector3 bulletVelocity;
    public float bulletMaxSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bulletVelocity += bulletDirection * bulletMaxSpeed;
        bulletVelocity = Vector3.ClampMagnitude(bulletVelocity, bulletMaxSpeed * 2);
        bulletPosition += bulletVelocity;

        transform.position = bulletPosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, bulletDirection);
    }
}
