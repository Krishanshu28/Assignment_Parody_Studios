using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    
    [Header("Ground Detection")]
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;
    public float maxAirTime = 3.0f; 

    public Transform cameraTransform;
    
    private Rigidbody rb;
    [SerializeField]
    private GravityManager gravManager;
    [SerializeField]
    private GameManager gameManager;
    private Animator animator; 
    
    private bool isGrounded;
    private float airTimeTimer = 0f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGrounded();
        CheckFalling(); 
        if (animator != null)
        {
            Vector3 gravityUp = gravManager.GetGravityUp();
            Vector3 velocityOnPlane = Vector3.ProjectOnPlane(rb.linearVelocity, gravityUp);
            float currentSpeed = velocityOnPlane.magnitude;

            animator.SetFloat("Speed", currentSpeed);
            animator.SetBool("IsGrounded", isGrounded);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded) Jump();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void CheckGrounded()
    {
        Vector3 gravityUp = gravManager.GetGravityUp();
        Vector3 startPoint = transform.position + (gravityUp * 0.1f);
        isGrounded = Physics.Raycast(startPoint, -gravityUp, groundCheckDistance, groundLayer);
    }

    //Fall check for game over
    void CheckFalling()
    {
        if (!isGrounded)
        {
            airTimeTimer += Time.deltaTime;//if in air for more than 3 sec than player fall or game over
            if (airTimeTimer > maxAirTime && gameManager != null)
            {
                gameManager.PlayerFell(); 
                this.enabled = false;//also can use Time.timeScale = 0 
            }
        }
        else
        {
            airTimeTimer = 0f; 
        }
    }

    void Move()
    {
        float h = 0; float v = 0;
        if (Input.GetKey(KeyCode.W)) v += 1;
        if (Input.GetKey(KeyCode.S)) v -= 1;
        if (Input.GetKey(KeyCode.D)) h += 1;
        if (Input.GetKey(KeyCode.A)) h -= 1;

        Vector3 gravityUp = gravManager.GetGravityUp();
        Vector3 camFwd = cameraTransform ? cameraTransform.forward : transform.forward;
        Vector3 camRight = cameraTransform ? cameraTransform.right : transform.right;

        camFwd = Vector3.ProjectOnPlane(camFwd, gravityUp).normalized;
        camRight = Vector3.ProjectOnPlane(camRight, gravityUp).normalized;

        Vector3 moveDir = (camFwd * v + camRight * h).normalized;
        Vector3 targetMoveVelocity = moveDir * moveSpeed;
        Vector3 currentFallVelocity = Vector3.Project(rb.linearVelocity, -gravityUp); 
        
        rb.linearVelocity = targetMoveVelocity + currentFallVelocity;

        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, gravityUp);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.fixedDeltaTime));
        }
    }

    void Jump()
    {
        Vector3 gravityUp = gravManager.GetGravityUp();
        Vector3 velocityOnPlane = Vector3.ProjectOnPlane(rb.linearVelocity, gravityUp);
        rb.linearVelocity = velocityOnPlane;
        rb.AddForce(gravityUp * jumpForce, ForceMode.Impulse);
    }
}