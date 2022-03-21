using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Rollo Player Controller Settings")]
    //Serialized Variables
    [SerializeField]
    float playerMovePower = 8;
    [SerializeField]
    float maxPlayerSpeed = 12;
    [SerializeField]
    float wheelRadius = 0.5f;
    [SerializeField]
    float bodyMultiplier = 0.25f;
    [SerializeField]
    float jumpPower = 5f;
    [SerializeField]
    float distToGround = 0.5f;
    [SerializeField]
    int jumpCount = 2;
    [SerializeField]
    float fallPush = 2;
    [SerializeField]
    float slowMultiplier = 0.5f;
    [SerializeField]
    float cameraMoveDamping = 1f;
    [SerializeField]
    LayerMask playerLayer;

    [Header("Rollo Character Models and Helpers")]
    //Public Variables
    public Transform playerWheel;
    public Transform tiltPoint;
    public Transform mainCamera;
    public Transform l_Eye;
    public Transform r_Eye;
    public Transform eyeTarget;
    public Transform lCameraLowerArm;
    public Transform lCameraUpperArm;
    public Transform RCameraLowerArm;
    public Transform RCameraUpperArm;

    public Text groundedText;
    public Text xVelText;
    public Text yVelText;
    public Text zVelText;
    public Text magText;

    public bool isMoveable = true;


    //Private Variables
    Transform playerTransform;
    Rigidbody playerRB;

    float playerSpeed;
    float moveVector;
    float wheelAngle = 0;
    float lowerTargetAngle;
    float upperTargetAngle;
    float previousHealth;

    int currentJumpCount = 0;

    bool isGrounded;
    bool canWallJump = false;

    Vector3 playerMovementForce = new Vector3();
    Vector3 previousPoint = new Vector3();
    Vector3 currentVelocity = new Vector3();
    Vector3 wallJumpVector = new Vector3();
    Vector3 bodyRotateGoal = new Vector3(0,0,0);
    Vector3 cameraOffset = new Vector3();
    Vector3 camerTransTarget;
    Vector3 targetDirection;
    Vector3 relativePos;
    Vector3 lookTargetOffset;
    Vector3 lookTargetOffsetReverse;
    Vector3 lowerArmAngles = new Vector3(0, 0, 0);
    Vector3 upperArmAngles = new Vector3(0, 0, 0);

    Quaternion LookAtRotation;
    Quaternion bodyRotationTarget;

    Health playerHealth;

    private void Awake()
    {
        // Automatically set some variables
        playerTransform = transform;
        playerRB = gameObject.GetComponent<Rigidbody>();
        currentJumpCount = 0;
        cameraOffset = mainCamera.position - transform.position;
        lookTargetOffset = transform.InverseTransformDirection(eyeTarget.position);
        lookTargetOffsetReverse = lookTargetOffset;
        lookTargetOffsetReverse.z = -lookTargetOffset.z;
        playerHealth = gameObject.GetComponent<Health>();
    }

    private void Start()
    {
        //On Start, Set previousPoint to player's position
        previousPoint = transform.localPosition;
        playerTransform = transform;
    }

    private void FixedUpdate()
    {
        //Set is grounded
        IsGrounded();
        //Find current velocity
        currentVelocity = transform.InverseTransformDirection(playerRB.velocity);
        //Find Player Speed
        playerSpeed = playerRB.velocity.magnitude;


        if (GetUserInput().z != 0)
        {
            //Check if player has reached top speed
            if (playerSpeed < maxPlayerSpeed)
            {
                // Add Force To Player
                playerRB.AddForce(transform.TransformDirection(GetUserInput()), ForceMode.Acceleration);
            }
        }
        else
        {
            if (isGrounded || playerRB.velocity.magnitude == 0)
            {
                currentVelocity.z *= slowMultiplier;
                playerRB.velocity = transform.TransformDirection(currentVelocity);
            }
        }
        fallMultiplier();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check if collision is a bounce or a walljump
        if (collision.contacts[0].normal.y > -0.01f && collision.contacts[0].normal.y < 0.25f)
        {
            // Collision is a wall jump
            canWallJump = true;
            wallJumpVector = collision.contacts[0].point - transform.position;
            wallJumpVector = transform.InverseTransformDirection(wallJumpVector);
            wallJumpVector.x = 0;
            wallJumpVector = transform.TransformDirection(wallJumpVector);
            wallJumpVector = -wallJumpVector.normalized;
        }
        if (isGrounded && collision.contacts[0].normal.y < 0.75f)
        {
            // Collision is a bounce
            currentVelocity = transform.InverseTransformDirection(currentVelocity);
            currentVelocity.x = 0;
            currentVelocity.z = -currentVelocity.z;
            currentVelocity = transform.TransformDirection(currentVelocity);
            playerRB.velocity = currentVelocity;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Clear wall jump
        canWallJump = false;
    }

    void fallMultiplier()
    {
        // Add force so player falls quicker than jump
        if(playerRB.velocity.y > -8 && playerRB.velocity.y < -0.15f && !isGrounded)
        {
            playerRB.AddForce(Vector3.down * fallPush, ForceMode.Acceleration);
        }
    }

    private void Update()
    {
        checkHealth();
        rotateWheel(findMovement());
        RotateBody();
        CheckJump();
        SetEyePositions();
        RotateEyes();
        MoveCamera();
        updateDebugInfo();
        ResetUpdateInfo();
    }

    void checkHealth()
    {
        UIManager.healthText.text = "Health: " + playerHealth.health.ToString();
        if (playerHealth.health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void updateDebugInfo()
    {
        groundedText.text = "IsGround: " + isGrounded;
        xVelText.text = "X-Velocity: " + playerRB.velocity.x;
        yVelText.text = "Y-Velocity: " + playerRB.velocity.y;
        zVelText.text = "Z-Velocity: " + playerRB.velocity.z;
        magText.text = "Velocity Magnitude: " + playerRB.velocity.magnitude;
    }

    void RotateEyes()
    {
        // Set Rotate Target forward is on the ground or falling
        if (isGrounded || playerRB.velocity.y < 0.25f)
        {
            relativePos = eyeTarget.position - l_Eye.position;
        }
        // Set Rotate Target Upward if actively jumping
        else
        {
            relativePos = (Vector3.up * 5) + (tiltPoint.forward * 2);
        }

        // Rotate Eyes
        LookAtRotation = Quaternion.LookRotation(relativePos);

        l_Eye.rotation = Quaternion.Lerp(l_Eye.rotation, Quaternion.Euler(LookAtRotation.eulerAngles.x, l_Eye.rotation.eulerAngles.y, l_Eye.rotation.eulerAngles.z), 5 * Time.deltaTime);
        r_Eye.rotation = Quaternion.Lerp(r_Eye.rotation, Quaternion.Euler(LookAtRotation.eulerAngles.x, r_Eye.rotation.eulerAngles.y, r_Eye.rotation.eulerAngles.z), 5 * Time.deltaTime);

        l_Eye.localEulerAngles = new Vector3(l_Eye.localEulerAngles.x, 180, 0);
        r_Eye.localEulerAngles = new Vector3(l_Eye.localEulerAngles.x, 180, 0);
    }

    void SetEyePositions()
    {
        if (playerRB.velocity.magnitude < 20 && playerRB.velocity.magnitude > 0)
        {
            // Moves Eye Arms
            if (isGrounded)
            {
                // Moves Eye Arms uniform to the speed of movement if on ground
                lowerTargetAngle = Mathf.Lerp(75 - ((playerRB.velocity.magnitude * 8) / 2), lowerArmAngles.x, Time.deltaTime);
                upperTargetAngle = Mathf.Lerp(-150 + (playerRB.velocity.magnitude * 8), upperArmAngles.x, Time.deltaTime);
            }
            else
            {
                // Moves Eye Arms inverse to the speed of movement if in air
                lowerTargetAngle = Mathf.Lerp(22.5f + (Mathf.Abs(playerRB.velocity.y * 15) / 2), lowerArmAngles.x, Time.deltaTime);
                upperTargetAngle = Mathf.Lerp(-45f - Mathf.Abs(playerRB.velocity.y * 15), lowerArmAngles.x, Time.deltaTime);
            }

            //Check if rotation is in limits
            if (lowerTargetAngle > 100) lowerTargetAngle = 100;
            if (upperTargetAngle < -150) upperTargetAngle = -150;

            // Placed correct rotation in target vector
            lowerArmAngles.x = lowerTargetAngle;
            upperArmAngles.x = upperTargetAngle;

            // Apply Eye Arm Rotation
            lCameraUpperArm.localEulerAngles = upperArmAngles;
            lCameraLowerArm.localEulerAngles = lowerArmAngles;
            RCameraUpperArm.localEulerAngles = upperArmAngles;
            RCameraLowerArm.localEulerAngles = lowerArmAngles;
        }
    }

    void MoveCamera()
    {
        // Set Camera to Move with Player
        camerTransTarget = transform.TransformPoint(cameraOffset);
        mainCamera.position = Vector3.Lerp(mainCamera.position, camerTransTarget, cameraMoveDamping * Time.deltaTime);

        //  Set Camera to look at Player
        targetDirection = transform.position - mainCamera.position;
        mainCamera.rotation = Quaternion.Lerp(mainCamera.rotation, Quaternion.LookRotation(targetDirection, Vector3.up), cameraMoveDamping * Time.deltaTime);

    }

    void RotateBody()
    {
        // Sets rotation of body dependant on player's Speed
        bodyRotateGoal.x = Mathf.Abs(transform.InverseTransformDirection(playerRB.velocity).z * 3f);

        // Rotates the body in the direction of motion
        if (Mathf.Abs(transform.InverseTransformDirection(playerRB.velocity).z) > 0.25f)
        {
            if (transform.InverseTransformDirection(playerRB.velocity).z < 0)
            {
                bodyRotateGoal.y = -180;
            }
            else
            {
                bodyRotateGoal.y = 0;
            }
        };

        // Apply the rotation
        tiltPoint.localRotation = Quaternion.Lerp(tiltPoint.localRotation, Quaternion.Euler(bodyRotateGoal), (cameraMoveDamping / 2) * Time.deltaTime);
    }

    private void ResetUpdateInfo()
    {
        //Set previousPoint to player's position for next frame
        previousPoint = transform.localPosition;
    }

    float findMovement()
    {
        //Determine if player is moving foward or backwards
        if (transform.InverseTransformDirection(playerRB.velocity).z < 0) return -Vector3.Distance(transform.localPosition, previousPoint);
        return Vector3.Distance(transform.localPosition, previousPoint);
    }
    
    void rotateWheel(float moveDistance)
    {
        //Spin wheel based on how far player has traveled
        wheelAngle = moveDistance * (180f / Mathf.PI) / wheelRadius;
        playerWheel.localRotation = Quaternion.Euler(Vector3.right * wheelAngle) * playerWheel.localRotation;
    }

    void CheckJump()
    {
        if (isMoveable)
        {
            //Check for keyboard
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.spaceKey.wasPressedThisFrame) jump();
            }
        }
    }

    Vector3 GetUserInput()
    {
        if (isMoveable)
        {
            //Check for keyboard
            Keyboard keyboard = Keyboard.current;

            //reset moveVector
            moveVector = 0;

            //If there is a keyboard check for input
            if (keyboard != null)
            {
                // Check if arrow keys are pressed
                if (!keyboard.rightArrowKey.isPressed && !keyboard.leftArrowKey.isPressed)
                {
                    //if key pressed, no force added
                    playerMovementForce.z = 0;
                }
                else
                {
                    if (isGrounded)
                    {
                        if (keyboard.rightArrowKey.isPressed) moveVector += 1 * playerMovePower;
                        if (keyboard.leftArrowKey.isPressed) moveVector -= 1f * playerMovePower;
                        playerMovementForce.z = moveVector;
                    }
                    else
                    {
                        if (keyboard.rightArrowKey.isPressed) moveVector += 0.3f * playerMovePower;
                        if (keyboard.leftArrowKey.isPressed) moveVector -= 0.3f * playerMovePower;
                        playerMovementForce.z = moveVector;
                    }
                }
            }
            return playerMovementForce;
        }
        return Vector3.zero;
    }

    void jump()
    {
        if (canWallJump)
        {
            playerRB.AddForce(wallJumpVector * jumpPower + (Vector3.up * jumpPower * 1.15f), ForceMode.Impulse);
            currentJumpCount++;
        }
        else
        {
            if (isGrounded || currentJumpCount < jumpCount)
            {
                playerRB.AddForce((Vector3.up * jumpPower), ForceMode.Impulse);
                currentJumpCount++;
            }
        }
    }

    void IsGrounded()
    {
        if (Physics.Raycast(transform.position + (transform.forward * 0.25f), -Vector3.up, distToGround + 0.1f) ||
            Physics.Raycast(transform.position - (transform.forward * 0.25f), -Vector3.up, distToGround + 0.1f) ||
            Physics.Raycast(transform.position + (transform.right * 0.25f), -Vector3.up, distToGround + 0.1f) ||
            Physics.Raycast(transform.position - (transform.right * 0.25f), -Vector3.up, distToGround + 0.1f)
            )
        {
            isGrounded = true;
        }
        else
        {
            if (Physics.CheckSphere(playerWheel.position, wheelRadius + 0.05f, playerLayer))
            {
                Vector3 airContactPoint = Vector3.Normalize(playerWheel.position - Physics.OverlapSphere(playerWheel.position, wheelRadius + 0.05f, playerLayer)[0].ClosestPoint(playerWheel.position));
                if (airContactPoint.y > 0.75f)
                {
                    isGrounded = true;
                }
                else
                {
                    isGrounded = false;
                }
            }
            else
            {
                isGrounded = false;
            }
        }

        if (isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f, playerLayer))
            {
                if (hit.transform.CompareTag("Platform"))
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, hit.transform.rotation.eulerAngles.y, transform.eulerAngles.z);
                }
            }
            currentJumpCount = 0;
        }
    }
}
