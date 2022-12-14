using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Joystick input;
    public float moveSpeed = 10f;
    public float maxRotation = 25f;

    private Rigidbody rb;
    private float minX, maxX, minY, maxY;

    public int maxHealth = 4;
    private int currentHealth;

    public żnGameManager inGameManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetBoundries();

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        RotatePlayer();
        CalculateBoundries();
    }

    private void RotatePlayer()
    {
        float currentX = transform.position.x;
        float newRotationZ;

        if(currentX < 0)
        {
            newRotationZ = Mathf.Lerp(0f, -maxRotation, currentX / minX);

        }
        else
        {
            newRotationZ = Mathf.Lerp(0f, maxRotation, currentX / maxX);
        }

        Vector3 currentRotationVector3 = new Vector3(0f, 0f, newRotationZ);
        Quaternion newRotation = Quaternion.Euler(currentRotationVector3);
        transform.localRotation = newRotation;

    }

    private void CalculateBoundries()
    {
        Vector3 currentPosition = transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);

        transform.position = currentPosition;
    }

    private void SetBoundries()
    {
        float cameraDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorners = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, cameraDistance));
        Vector2 topCorners = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, cameraDistance));

        //calculate the size of the gameobject
        Bounds gameObjectBouds = GetComponent<Collider>().bounds;
        float objectWidth = gameObjectBouds.size.x;
        float objectHeight = gameObjectBouds.size.y;

        minX = bottomCorners.x + objectWidth;
        maxX = topCorners.x - objectWidth;

        minY = bottomCorners.y + objectHeight;
        maxY = topCorners.y - objectHeight;

        //set up the asteroid manager
        AsteroidManager.Instance.maxX = maxX;
        AsteroidManager.Instance.minX = minX;
        AsteroidManager.Instance.minY = minY;
        AsteroidManager.Instance.maxY = maxY;


    }

    private void MovePlayer()
    {
        float horizontalMovement = input.Horizontal;
        float verticalMovement = input.Vertical;

        Vector3 movementVector = new Vector3(horizontalMovement, verticalMovement, 0f);
        rb.velocity = movementVector * moveSpeed;
    }

    public void OnAsteroidImpact()
    {
        currentHealth--;

        inGameManager.ChangeHealthBar(maxHealth, currentHealth);

        if(currentHealth == 0)
        {
            OnPlayerDeath();
        }
    }

    private void OnPlayerDeath()
    {
        Debug.Log("player died");
    }
}
