using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovementScript : MonoBehaviour
{
    public Transform player;
    public CharacterController controller;
    public Animator animation;
    public Transform cam;
    [SerializeField] private float walkSpeed = 2f, runSpeed = 6f, turnSmoothTime = 0.1f;

    [SerializeField] float gravity = 0;
    float ySpeed = 0;
    [SerializeField] float maxGravitySpeed = 10;
    [SerializeField] float distanceToFloor = 0.1f;

    float horizontal,
        vertical,
        targetAngle,
        angle,
        turnSmoothVelocity,
        speed,
        y;

    private Vector3 direction, moveDirection;
    private bool resetRotation = false;

    private Coroutine jump;

    private void Awake()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            // angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, turnSmoothTime);
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, angle, 0f).normalized * Vector3.forward;

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                controller.Move(moveDirection.normalized * walkSpeed * Time.deltaTime);
                speed = walkSpeed;
            }
            else
            {
                controller.Move(moveDirection.normalized * runSpeed * Time.deltaTime);
                speed = runSpeed;
            }
        }
        else
            speed = 0;

        animation.SetFloat("Speed", speed);
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, -transform.up, out hit, distanceToFloor))
        {
            if (hit.transform == null)
            {
                ySpeed += gravity * Time.fixedDeltaTime;
                if (ySpeed > maxGravitySpeed)
                    ySpeed = maxGravitySpeed;
            }
            else
            {
                ySpeed = 0;
            }
        }
        else
        {
            ySpeed += gravity * Time.fixedDeltaTime;
            if (ySpeed > maxGravitySpeed)
                ySpeed = maxGravitySpeed;
        }
        controller.Move(new Vector3(0, -ySpeed, 0));
    }
}
