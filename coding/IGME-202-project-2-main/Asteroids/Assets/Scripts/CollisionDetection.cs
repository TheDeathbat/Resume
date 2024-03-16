using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField]
    GameObject spaceshipObject;

    GameObject spaceship;

    List<GameObject> asteroids = new List<GameObject>();

    List<GameObject> smallAsteroids = new List<GameObject>();

    List<Bullet> bullets = new List<Bullet>();

    [SerializeField]
    GameObject bullet;

    [SerializeField]
    GameObject asteroidObject;

    [SerializeField]
    GameObject smallAsteroidObject;

    [SerializeField]
    GameObject healthImage;

    List<GameObject> healthbar = new List<GameObject>();

    [SerializeField]
    Text currentScore, hitText;

    Camera mainCamera;
    public float camHeight;
    public float camWidth;

    float score;
    float damageTaken;
    bool isGameOver = false;

    bool isDamageLocked = false;
    float damageLockout = 3f;
    bool areWeaponsLocked = false;
    float bulletLockout = .3f;
    float damageTimer = 0f;
    float weaponTimer = 0f;

    bool wasAsteroidHit = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        camHeight = 2f * mainCamera.orthographicSize;
        camWidth = camHeight * mainCamera.aspect;

        spaceship = Instantiate(spaceshipObject, Vector3.zero, Quaternion.identity);

        for(int i = 0; i < 3; i++)
        {
            CreateAsteroid(Random.Range(0f, 4f));
        }

        for (int i = 0; i < 3; i++)
        {
            healthbar.Add(Instantiate(healthImage, Vector3.zero, Quaternion.identity));
            healthbar[i].transform.position = new Vector3((-camWidth / 2.1f) + 1f * i, (-camHeight / 2) + 0.5f, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver)
        {
            currentScore.text = "Finished with a score of: " + score;
            hitText.text = "Press R to play again!";

            if(Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }
        }
        else
        {
            if (isDamageLocked)
            {
                damageTimer += Time.deltaTime;
                spaceship.GetComponent<SpriteRenderer>().color = Color.red;

                if (damageTimer > damageLockout)
                {
                    damageTimer = 0f;
                    isDamageLocked = false;
                    spaceship.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            else
            {
                for (int i = 0; i < asteroids.Count; i++)
                {
                    if (CircleCollision(spaceship, asteroids[i]))
                    {
                        damageTaken++;
                        hitText.text = "Damage Taken: " + damageTaken;
                        isDamageLocked = true;
                        Destroy(healthbar[healthbar.Count - 1]);
                        healthbar.RemoveAt(healthbar.Count - 1);

                        if (healthbar.Count == 0)
                        {
                            isGameOver = true;
                            Destroy(spaceship);
                        }

                        break;
                    }
                }

                for (int i = 0; i < smallAsteroids.Count; i++)
                {
                    if (CircleCollision(spaceship, smallAsteroids[i]))
                    {
                        damageTaken++;
                        hitText.text = "Damage Taken: " + damageTaken;
                        isDamageLocked = true;
                        Destroy(healthbar[healthbar.Count - 1]);
                        healthbar.RemoveAt(healthbar.Count - 1);

                        if (healthbar.Count == 0)
                        {
                            isGameOver = true;
                            Destroy(spaceship);
                        }

                        break;
                    }
                }
            }

            if (areWeaponsLocked)
            {
                weaponTimer += Time.deltaTime;

                if (weaponTimer > bulletLockout)
                {
                    weaponTimer = 0;
                    areWeaponsLocked = false;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    bullets.Add(Instantiate(bullet.GetComponent<Bullet>(), spaceship.transform.position, Quaternion.identity));
                    bullets[bullets.Count - 1].bulletDirection = spaceship.GetComponent<Vehicle>().direction;
                    bullets[bullets.Count - 1].bulletPosition = spaceship.GetComponent<Vehicle>().vehiclePosition;
                    bullets[bullets.Count - 1].bulletMaxSpeed = spaceship.GetComponent<Vehicle>().maximumSpeed;
                    areWeaponsLocked = true;
                }
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                if (CheckOutOfBounds(bullets[i]))
                {
                    Destroy(bullets[i].gameObject);
                    bullets.RemoveAt(i);
                    break;
                }

                for (int j = 0; j < asteroids.Count; j++)
                {
                    if (CircleCollision(bullets[i].gameObject, asteroids[j]))
                    {
                        SplitAsteroid(asteroids[j]);
                        Destroy(asteroids[j].gameObject);
                        asteroids.RemoveAt(j);
                        Destroy(bullets[i].gameObject);
                        bullets.RemoveAt(i);
                        score += 20;
                        currentScore.text = "Current Score: " + score;
                        wasAsteroidHit = true;
                        break;
                    }
                }

                if (false == wasAsteroidHit)
                {
                    for (int j = 0; j < smallAsteroids.Count; j++)
                    {
                        if (CircleCollision(bullets[i].gameObject, smallAsteroids[j]))
                        {
                            Destroy(smallAsteroids[j].gameObject);
                            smallAsteroids.RemoveAt(j);
                            Destroy(bullets[i].gameObject);
                            bullets.RemoveAt(i);
                            score += 50;
                            currentScore.text = "Current Score: " + score;
                            break;
                        }
                    }
                }

                wasAsteroidHit = false;
            }

            if (asteroids.Count < 2 && smallAsteroids.Count < 4)
            {
                CreateAsteroid(Random.Range(0f, 4f));
            }
        }
    }

    // AABB Testing, Currently unused
    bool AABBCollision(GameObject ship, GameObject asteroid)
    {
        // if statement to see if the objects are intersecting

        if (ship.GetComponent<SpriteRenderer>().bounds.min.x < asteroid.GetComponent<SpriteRenderer>().bounds.max.x 
         && ship.GetComponent<SpriteRenderer>().bounds.max.x > asteroid.GetComponent<SpriteRenderer>().bounds.min.x
         && ship.GetComponent<SpriteRenderer>().bounds.min.y < asteroid.GetComponent<SpriteRenderer>().bounds.max.y
         && ship.GetComponent<SpriteRenderer>().bounds.max.y > asteroid.GetComponent<SpriteRenderer>().bounds.min.y)
        {
            return true;
        }

        return false;
    }

    bool CircleCollision(GameObject ship, GameObject asteroid)
    {
        // if statement to see if the objects are intersecting
        
        if(ship.GetComponent<SpriteRenderer>().bounds.extents.magnitude + asteroid.GetComponent<SpriteRenderer>().bounds.extents.magnitude / 3 
         > CalculateDistance(ship.GetComponent<SpriteRenderer>().bounds, asteroid.GetComponent<SpriteRenderer>().bounds))
        {
            return true;
        }

        return false;
    }

    public float CalculateDistance(Bounds ship, Bounds asteroid)
    {
        float differenceInX = ship.center.x - asteroid.center.x;
        float distanceXSquare = differenceInX * differenceInX;
        float differenceInY = ship.center.y - asteroid.center.y;
        float distanceYSquare = differenceInY * differenceInY;

        // Calculate the distance using the previous calculations
        return Mathf.Sqrt(distanceXSquare + distanceYSquare);
    }
    
    void CreateAsteroid(float initialSide)
    {
        if (initialSide < 1)
        {
            Vector3 tempPos = new Vector3(camWidth / 2 + 1, Random.Range(-camHeight / 2, camHeight / 2), 0);
            GameObject temp = Instantiate(asteroidObject, tempPos, Quaternion.identity);
            temp.GetComponent<Asteroid>().asteroidPosition = tempPos;
            temp.transform.position = tempPos;
            temp.GetComponent<Asteroid>().asteroidDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            asteroids.Add(temp);
        }
        else if (initialSide < 2)
        {
            Vector3 tempPos = new Vector3(-camWidth / 2 - 1, Random.Range(-camHeight / 2, camHeight / 2), 0);
            GameObject temp = Instantiate(asteroidObject, tempPos, Quaternion.identity);
            temp.GetComponent<Asteroid>().asteroidPosition = tempPos;
            temp.transform.position = tempPos;
            temp.GetComponent<Asteroid>().asteroidDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            asteroids.Add(temp);
        }
        else if (initialSide < 3)
        {
            Vector3 tempPos = new Vector3(Random.Range(-camWidth / 2, camWidth / 2), camHeight / 2 + 1, 0);
            GameObject temp = Instantiate(asteroidObject, tempPos, Quaternion.identity);
            temp.GetComponent<Asteroid>().asteroidPosition = tempPos;
            temp.transform.position = tempPos;
            temp.GetComponent<Asteroid>().asteroidDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            asteroids.Add(temp);
        }
        else
        {
            Vector3 tempPos = new Vector3(Random.Range(-camWidth / 2, camWidth / 2), -camHeight / 2 - 1, 0);
            GameObject temp = Instantiate(asteroidObject, tempPos, Quaternion.identity);
            temp.GetComponent<Asteroid>().asteroidPosition = tempPos;
            temp.transform.position = tempPos;
            temp.GetComponent<Asteroid>().asteroidDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            asteroids.Add(temp);
        }
    }

    private bool CheckOutOfBounds(Bullet bullet)
    {
        if (bullet.bulletPosition.x < -camWidth / 2)
        {
            return true;
        }
        else if (bullet.bulletPosition.x > camWidth / 2)
        {
            return true;
        }

        if (bullet.bulletPosition.y < -camHeight / 2)
        {
            return true;
        }
        else if (bullet.bulletPosition.y > camHeight / 2)
        {
            return true;
        }

        return false;
    }

    void SplitAsteroid(GameObject asteroid)
    {
        GameObject miniOne = Instantiate(smallAsteroidObject, asteroid.GetComponent<Asteroid>().asteroidPosition, Quaternion.identity);
        GameObject miniTwo = Instantiate(smallAsteroidObject, asteroid.GetComponent<Asteroid>().asteroidPosition, Quaternion.identity);

        miniOne.GetComponent<Asteroid>().asteroidPosition = asteroid.GetComponent<Asteroid>().asteroidPosition;
        miniTwo.GetComponent<Asteroid>().asteroidPosition = asteroid.GetComponent<Asteroid>().asteroidPosition;

        miniOne.transform.position = miniOne.GetComponent<Asteroid>().asteroidPosition;
        miniTwo.transform.position = miniTwo.GetComponent<Asteroid>().asteroidPosition;

        miniOne.GetComponent<Asteroid>().asteroidDirection = asteroid.GetComponent<Asteroid>().asteroidDirection + Vector3.right / 3;
        miniTwo.GetComponent<Asteroid>().asteroidDirection = asteroid.GetComponent<Asteroid>().asteroidDirection - Vector3.right / 3;

        miniOne.GetComponent<Asteroid>().asteroidVelocity = asteroid.GetComponent<Asteroid>().asteroidVelocity;
        miniTwo.GetComponent<Asteroid>().asteroidVelocity = asteroid.GetComponent<Asteroid>().asteroidVelocity;

        miniOne.GetComponent<Asteroid>().asteroidMaxSpeed = asteroid.GetComponent<Asteroid>().asteroidMaxSpeed * 1.5f;
        miniTwo.GetComponent<Asteroid>().asteroidMaxSpeed = asteroid.GetComponent<Asteroid>().asteroidMaxSpeed * 1.5f;

        smallAsteroids.Add(miniOne);
        smallAsteroids.Add(miniTwo);
    }

    void Reset()
    {
        spaceship = Instantiate(spaceshipObject, Vector3.zero, Quaternion.identity);

        for (int i = asteroids.Count - 1; i >= 0; i--)
        {
            Destroy(asteroids[i]);
            asteroids.RemoveAt(i);
        }

        if(smallAsteroids.Count > 0)
        {
            for (int i = smallAsteroids.Count - 1; i >= 0; i--)
            {
                Destroy(smallAsteroids[i]);
                smallAsteroids.RemoveAt(i);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            CreateAsteroid(Random.Range(0f, 4f));
        }

        score = 0f;
        damageTaken = 0f;
        isDamageLocked = false;
        areWeaponsLocked = false;
        damageTimer = 0;
        weaponTimer = 0;

        currentScore.text = "Current Score: " + score;
        hitText.text = "Damage Taken: " + damageTaken;

        for (int i = 0; i < 3; i++)
        {
            healthbar.Add(Instantiate(healthImage, Vector3.zero, Quaternion.identity));
            healthbar[i].transform.position = new Vector3((-camWidth / 2.1f) + 1f * i, (-camHeight / 2) + 0.5f, 0);
        }

        isGameOver = false;
    }
}
