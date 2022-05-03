using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private float movementSpeed = 0;
    private float walkingSpeed = 2.0f;
    private float runningSpeed = 5.0f;
    private float rotationSensitivity = 100.0f;
    private float yRotation = 0.0f;
    private Rigidbody playerBody;
    private Vector3 jumpVec;
    private float jumpSpeed = 1.0f;
    private bool onGround;
    private Transform bow;
    private Transform arrowTransform;
    private float forwardDist = 0.6f;
    private float rightDist = 0.5f;
    private GameObject gameOverScreen;
    private CountdownTimer countdownTimer;
    public Camera playerCamera;
    public GameObject arrowObj;
    public AudioSource movementAudio;

    private void Start()
    {
        // Initially set our game over screen to inactive
        gameOverScreen = GameObject.Find("Panel_PlayGame_GameOver");
        gameOverScreen.SetActive(false);

        // Get a reference to the time left object so we can initiate a game over if we hit 0 seconds
        countdownTimer = GameObject.Find("CountdownTimer").GetComponent(typeof(CountdownTimer)) as CountdownTimer;

        playerBody = GetComponent<Rigidbody>();
        jumpVec = new Vector3(0.0f, 3.0f, 0.0f);
        bow = GameObject.Find("Bow").GetComponent<Transform>();
        arrowTransform = GameObject.Find("Arrow").GetComponent<Transform>();

        // Played whenever moving in game
        movementAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // This will rotate our view from the x-axis
        transform.Rotate(Input.GetAxis("Horizontal") * Vector3.up * rotationSensitivity * Time.deltaTime);

        // This will allow us to move the mouse up and down to adjust where we are looking at in the y-axis
        yRotation += Input.GetAxis("Mouse Y") * rotationSensitivity * Time.deltaTime * -1.0f;
        yRotation = Mathf.Clamp(yRotation, -45, 45);
        playerCamera.GetComponent<Transform>().localEulerAngles = new Vector3(yRotation, 0.0f, 0.0f);

        // This will keep the bow/arrow in the same spot in front of the camera with the same angle
        bow.position = Camera.main.transform.position + Camera.main.transform.forward * forwardDist  + Camera.main.transform.right * rightDist;
        bow.localEulerAngles = new Vector3(0.0f, -90.0f, -yRotation);
        arrowTransform.position = Camera.main.transform.position + Camera.main.transform.forward * (forwardDist-0.1f) + Camera.main.transform.right * (rightDist-0.1f);
        arrowTransform.localEulerAngles = new Vector3(0.0f, -90.0f, -yRotation-90);

        // This will move our player forward/backwards
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = runningSpeed;
        }
        else
        {
            movementSpeed = walkingSpeed;
        }

        transform.Translate(0, 0, Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime);

        // Play our movement audio whenver moving, and stop it whenever we stop moving
        if (Input.GetAxis("Vertical") != 0)
        {
            if (!movementAudio.isPlaying)
            {
                movementAudio.Play();
            }
        }
        else
        {
            movementAudio.Stop();
        }

        // If the user presses the space bar and is on the ground, jump
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            playerBody.AddForce(jumpVec * jumpSpeed, ForceMode.Impulse);

            // Set on ground to false so we can't jump again mid-air
            onGround = false;
        }

        // If user presses left mouse button, fire arrow
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        // If the timer hits 0, game over
        if (countdownTimer.countdownTime == 0)
        {
            gameOverScreen.SetActive(true);
            var gameOverText = GameObject.Find("Text_PlayGame_GameOver_Info").GetComponent<Text>();
            gameOverText.text = "You ran out of time";

            // Destroy this object so user can't move after game over screen appears and stop the movement audio
            DestroyPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If we jumped over the edge into the water, activate game over screen with info on why its a game over
        if (collision.gameObject.tag == "Water")
        {
            gameOverScreen.SetActive(true);
            var gameOverText = GameObject.Find("Text_PlayGame_GameOver_Info").GetComponent<Text>();
            gameOverText.text = "You jumped in the water";

            // Destroy this object so user can't move after game over screen appears and make sure movement audio stops\
            DestroyPlayer();
        }
        else
        {
            // Checks to make sure we are on the ground so we know if we can jump
            onGround = true;
        }
    }

    private void Fire()
    {
        // Shoot a ray out of your camera
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        Vector3 rayHit;

        // If it hit something, get that position, if not set rayhit to a large number
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            rayHit = hit.point;
        }
        else
        {
            rayHit = ray.GetPoint(100);
        }

        // Get the direction our arrow will shoot towards
        Vector3 arrowShotDirection = rayHit - bow.position;

        // Instantiate an arrow at your bow and normalize the vector
        GameObject arrow = Instantiate(arrowObj, bow.position, Quaternion.identity);
        arrow.transform.forward = arrowShotDirection.normalized;

        // Rotate our arrow from vertical to the screen to horizontal
        arrow.transform.eulerAngles = new Vector3(arrow.transform.eulerAngles.x + 90, arrow.transform.eulerAngles.y, arrow.transform.eulerAngles.z);

        // Shoot the arrow
        arrow.GetComponent<Rigidbody>().AddForce(arrowShotDirection.normalized * 20, ForceMode.Impulse);
    }

    // Destroy the player movement/countdown timer scripts when the user wins or game over happens and stop any movement audio
    public void DestroyPlayer()
    {
        var countdownTimer = GameObject.Find("CountdownTimer").GetComponent(typeof(CountdownTimer)) as CountdownTimer;
        movementAudio.Stop();
        Destroy(countdownTimer);
        Destroy(this);
    }
}