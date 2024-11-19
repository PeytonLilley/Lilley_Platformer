using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float walkSpeed = 5;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        Vector2 playerInput = new Vector2();
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        float walking = playerInput.x;
        walking = Input.GetAxis("Horizontal");
        Debug.Log(walking);

        if (walking > 0)
        {
            Debug.Log("right");
            rb.AddForce(new Vector2(walking * walkSpeed, 0), ForceMode2D.Force);
        }

        if (walking < 0)
        {
            Debug.Log("left");
            rb.AddForce(new Vector2(walking * walkSpeed, 0), ForceMode2D.Force);
        }
    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
