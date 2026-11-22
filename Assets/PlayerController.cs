using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private GameLogic gameLogic;

    float timeDiff;
    private InputValue movementValue;

    private float movementX;
    private float movementY;
    private bool dashRequested;
    private bool doubleJump = true;
    [SerializeField]
    private float dashCooldown = 5.0f;
    private float nextDash;

    [SerializeField]
    private Image shift;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float speed = 1;

    [SerializeField]
    private Transform movementCamera;

    [SerializeField]
    private float dashSpeed = 10f;

    [SerializeField]
    private Animator shiftAnimator;


    // Initialize physics before input callbacks can run.
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.sleepThreshold = 0.0f;

        if (movementCamera == null && Camera.main != null)
        {
            movementCamera = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {

        Vector3 forward = Vector3.forward;
        if (movementCamera != null)
        {
            // Ignore the camera's downward tilt so movement stays on the ground plane.
            forward = Vector3.ProjectOnPlane(movementCamera.forward, Vector3.up);
            if (forward.sqrMagnitude < 0.001f)
            {
                forward = Vector3.ProjectOnPlane(movementCamera.up, Vector3.up);
            }
            forward.Normalize();
        }

        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 movement = Vector3.ClampMagnitude(right * movementX + forward * movementY, 1f);
        rb.AddForce(movement * speed);

        if(Time.time > nextDash)
        {
            //shift.color = Color.green;
        }

        if (dashRequested)
        {
            if (Time.time > nextDash) { 
            // Dash toward the input, or camera-forward when standing still.
                Vector3 dashDirection = movement.sqrMagnitude > 0f ? movement.normalized : forward;
                rb.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);
                dashRequested = false;
                nextDash = Time.time + dashCooldown;

                //shift.color = Color.red;
                shiftAnimator.Play("Base Layer.DashCooldown", 0, 0f);
            } else
            {
                dashRequested = false; // stops from dashing double dashing on accident
            }
        }
    }


    // Update is called once per frame
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnJump(InputValue movementValue)
    {
        if (movementValue.isPressed && (CheckIfOnGround(gameObject)))
        {
            rb.AddForce(Vector3.up * rb.mass * 500);
        }
    }

    void Update()
    {
        if (Keyboard.current != null &&
            (Keyboard.current.leftShiftKey.wasPressedThisFrame || Keyboard.current.rightShiftKey.wasPressedThisFrame))
        {
            dashRequested = true;
        }

        if (doubleJump == false && CheckIfOnGround(gameObject))
        {
            doubleJump = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            // Use the Rotator belonging to the pickup we actually touched.
            if (!other.TryGetComponent(out Rotator pickup))
            {
                Debug.LogError("Pickup is missing a Rotator component.", other);
                return;
            }

            pickup.touched(other.gameObject);
            gameLogic.AddCoin();
            gameLogic.SetCountText();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            gameLogic.EndGame("Lose");
        }
    }

    private bool CheckIfOnGround(GameObject gameObject)
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f))
        {
            print("Ground");
            return true;

        }
        else if (doubleJump == true)
        {
            print("Double Jump");
            doubleJump = false;
            return true;
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
            {
                GameObject hitObject = hit.collider.gameObject;
                if ((hitObject.transform.rotation.eulerAngles.x == 0f) && (hitObject.transform.rotation.eulerAngles.z == 0f))
                {
                    print("not a sloped surface");
                    return false;
                }
                else
                {
                    print("SLOPED SURFACE ALLOWING EXTENDED JUMP");
                    return true;
                }
            } else {
                print("Not on ground");
                return false;
            }
                
        }
    }

    public void Pause(bool pause)
    {
        if (pause)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
            timeDiff = nextDash - Time.time;
            nextDash = 500000;
            print("Currtime = " + Time.time + "timeDiff = " +  timeDiff);
        } else
        {
            rb.constraints = RigidbodyConstraints.None;
            nextDash = Time.time + timeDiff;
        }
    }

    void OnPause()
    {
        gameLogic.Pause();
    }

}
