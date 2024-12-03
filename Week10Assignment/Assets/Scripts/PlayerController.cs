using UnityEngine;

public enum PlayerDirection
{
    left, right
}

public enum PlayerState
{
    idle, walking, jumping, dead
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    private PlayerDirection currentDirection = PlayerDirection.right;
    public PlayerState currentState = PlayerState.idle;
    public PlayerState previousState = PlayerState.idle;

    [Header("Horizontal")]
    public float maxSpeed = 5f;
    public float accelerationTime = 0.25f;
    public float decelerationTime = 0.15f;

    [Header("Vertical")]
    public float apexHeight = 3f;
    public float apexTime = 0.5f;

    [Header("Ground Checking")]
    public float groundCheckOffset = 0.5f;
    public Vector2 groundCheckSize = new(0.3f, 0.1f);
    public LayerMask groundCheckMask;

    private float accelerationRate;
    private float decelerationRate;

    private float gravity;
    private float initialJumpSpeed;

    private bool isGrounded = false;
    public bool isDead = false;

    private Vector2 velocity;

    public float dashForce = 50;
    public float dashLength = 0.5f;
    public float dashTimer = 0f;

    public float wallDistance = 0.5f;
    public float climbSpeed = 5f;

    public void Start()
    {
        body.gravityScale = 0;

        accelerationRate = maxSpeed / accelerationTime;
        decelerationRate = maxSpeed / decelerationTime;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        initialJumpSpeed = 2 * apexHeight / apexTime;
    }

    public void Update()
    {
        previousState = currentState;

        CheckForGround();

        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        if (isDead)
        {
            currentState = PlayerState.dead;
        }

        switch(currentState)
        {
            case PlayerState.dead:
                // do nothing - we ded.
                break;
            case PlayerState.idle:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (velocity.x != 0) currentState = PlayerState.walking;
                break;
            case PlayerState.walking:
                if (!isGrounded) currentState = PlayerState.jumping;
                else if (velocity.x == 0) currentState = PlayerState.idle;
                break;
            case PlayerState.jumping:
                if (isGrounded)
                {
                    if (velocity.x != 0) currentState = PlayerState.walking;
                    else currentState = PlayerState.idle;
                }
                break;
        }

        MovementUpdate(playerInput);
        JumpUpdate();

        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = 0;

        body.velocity = velocity;

        Dash();
        TouchWall();
        WallClimb();
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (playerInput.x < 0)
            currentDirection = PlayerDirection.left;
        else if (playerInput.x > 0)
            currentDirection = PlayerDirection.right;

        if (playerInput.x != 0)
        {
            velocity.x += accelerationRate * playerInput.x * Time.deltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocity.x > 0)
            {
                velocity.x -= decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            else if (velocity.x < 0)
            {
                velocity.x += decelerationRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }
    }

    private void Dash()  // dash mechanic
    {
        

        Debug.Log(dashTimer);

        if (Input.GetKey(KeyCode.Z) && currentDirection == PlayerDirection.left && dashTimer < dashLength)  // if the player is facing left and z is pressed, dash left
        {
            dashTimer += Time.deltaTime;
            body.AddForce(Vector2.left * dashForce, ForceMode2D.Impulse);
        }
        
        if (Input.GetKey(KeyCode.Z) && currentDirection == PlayerDirection.right && dashTimer < dashLength)  // otherwise, if they are facing right and press z, dash right
        {
            dashTimer += Time.deltaTime;
            body.AddForce(Vector2.right * dashForce, ForceMode2D.Impulse);
        }

        if (dashTimer > dashLength)  // if the dash timer runs out, stop the player's movement
        {
            velocity = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.Z))  // reset the dash timer when the player stops pressing the dash button
        {
            dashTimer = 0;
        }
    }

    public bool TouchWall()  // raycasts for the player's proximity to the wall
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallDistance);
        Debug.DrawRay(transform.position, Vector2.left * wallDistance, Color.red);

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallDistance);
        Debug.DrawRay(transform.position, Vector2.right * wallDistance, Color.red);

        if (hitLeft.collider == null && hitRight.collider != null)  // returns true if the right side of the player is touching the wall
        {
            Debug.Log("touching wall");
            return true;
        }
        else if (hitRight.collider == null && hitLeft.collider != null)  // returns true if the left side of the player is touching the wall
        {
            Debug.Log("touching wall");
            return true; ;
        }
        else  // returns false if the player is not touching the wall on either side
        {
            Debug.Log("not touching wall");
            return false;
        }
    }

    private void WallClimb()  // wall climb mechanic
    {
        if (Input.GetKey(KeyCode.C) && TouchWall())  // adds upward force if the player is touching the wall and presses the climb button
        {
            gravity = 0;  // stops gravity while the player is climbing so the two forces aren't pushing against each other
            velocity.x = 0;
            body.AddForce(Vector2.up * climbSpeed, ForceMode2D.Force);
        }
        if (Input.GetKeyUp(KeyCode.C))  // adds gravity back in when the player stops pressing the climb button
        {
            gravity = -2 * apexHeight / (apexTime * apexTime);
        }
    }

    private void JumpUpdate()
    {
        if (isGrounded && Input.GetButton("Jump"))
        {
            velocity.y = initialJumpSpeed;
            isGrounded = false;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)  // bounce and slide mechanics
    {
         if (collision.gameObject.tag == "Bouncy")  // detects if the player is on the bounce tile 
        {
            Debug.Log("bounce");
            //body.AddForce(Vector2.up * 0, ForceMode2D.Impulse);
        }
         if (collision.gameObject.tag == "Slide")  // detects if the player is on the slide tile
        {
            Debug.Log("slide");
            body.AddForce(Vector2.left * 1000, ForceMode2D.Impulse);
            //velocity.x = velocity.x * 50;
        }
    }

    private void CheckForGround()
    {
        isGrounded = Physics2D.OverlapBox(
            transform.position + Vector3.down * groundCheckOffset,
            groundCheckSize,
            0,
            groundCheckMask);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
    }

    public bool IsWalking()
    {
        return velocity.x != 0;
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public PlayerDirection GetFacingDirection()
    {
        return currentDirection;
    }
}
