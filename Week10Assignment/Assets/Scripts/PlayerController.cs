using JetBrains.Annotations;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;

    public float walkSpeed = 5f;
    float playerWalking;
    public float groundedDistance = 0.8f;

    public enum FacingDirection
    {
        left, right
    }

    public float recentDirection = 0;

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

        playerWalking = Input.GetAxis("Horizontal");

        Vector2 playerInput = new Vector2();
        MovementUpdate(playerInput);
        IsWalking();
        //Debug.Log(IsWalking());
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        float walking = playerInput.x;
        walking = Input.GetAxis("Horizontal");
        //Debug.Log(walking);

        if (walking > 0)
        {
            //Debug.Log("right");
            rb.AddForce(new Vector2(walking * walkSpeed, 0), ForceMode2D.Force);
        }

        if (walking < 0)
        {
            //Debug.Log("left");
            rb.AddForce(new Vector2(walking * walkSpeed, 0), ForceMode2D.Force);
        }
    }

    public bool IsWalking()
    {
        if (playerWalking > 0 || playerWalking < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, groundedDistance);
        Debug.DrawRay(transform.position, Vector2.down * groundedDistance, Color.yellow);
        if (hitInfo.collider == null)
        {
            Debug.Log("not grounded");
            return false;
        }
        else
        {
            Debug.Log("grounded");
            return true;
        }

    }

    public FacingDirection GetFacingDirection()
    {
        if (playerWalking > 0)
        {
            recentDirection = 1;
            return FacingDirection.right;
        }
        if (playerWalking < 0)
        {
            recentDirection = 0;
            return FacingDirection.left;
        }
        if ((playerWalking == 0) && (recentDirection == 1))
        {
            return FacingDirection.right;
        }
        if ((playerWalking == 0) && (recentDirection == 0))
        {
            return FacingDirection.left;
        }
        else return FacingDirection.left;
    }
}
