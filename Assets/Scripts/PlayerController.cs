using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Collider2D boxCollider;
    [HideInInspector] public float xVelocity;
    [HideInInspector] public float yVelocity;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector2 velocityRef;
    public float acceleration;
    public float jumpHeight;
    public float maxSpeed;
    public float jumpMaxSpeed;
    public float groundDrag;
    public float airDrag;
    public float gravity;
    public LayerMask groundMask;
    // Default Unity Messages
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        xVelocity = 0.0f;
        yVelocity = 0.0f;
        this.GetComponent<Rigidbody2D>().gravityScale = gravity;
    }
    private void Update()
    {
        GroundedCheck();
    }
    // Ground Movement
    public void MoveGround()
    {
        MoveGround(this.GetComponent<Player>());
    }
    public void MoveGround(Player thisObj)
    {
        xVelocity = (thisObj.rb.velocity.x + (thisObj.xInput * acceleration));
        if (xVelocity > maxSpeed + 2 || xVelocity < -maxSpeed - 2)
        {
            xVelocity *= 0.95f;
        }
        else
        {
            xVelocity = Mathf.Clamp(xVelocity, -maxSpeed, maxSpeed);
        }

        if (thisObj.xInput == 0.0f || thisObj.xInput * xVelocity < 0)
        {
            if (xVelocity > 0.0f)
            {
                xVelocity = Mathf.Clamp(xVelocity - airDrag, 0.0f, maxSpeed);
            }

            else
            {
                xVelocity = Mathf.Clamp(xVelocity + airDrag, -maxSpeed, 0.0f);
            }
        }

        if (isGrounded && thisObj.yInput == 1.0f)
        {
            Jump();
        }

        yVelocity = thisObj.rb.velocity.y;
        if (yVelocity > maxSpeed + 2 || yVelocity < -maxSpeed - 2)
        {
            yVelocity *= 0.9f;
        }
        else
        {
            Mathf.Clamp(thisObj.rb.velocity.y, -jumpMaxSpeed, jumpMaxSpeed);
        }

        Vector2 newVelocity = new Vector2(xVelocity, yVelocity);
        thisObj.rb.velocity = Vector2.SmoothDamp(thisObj.rb.velocity, newVelocity, ref velocityRef, Time.deltaTime);
    }
    // Aerial Movement
    public void MoveAir()
    {
        MoveAir(GetComponent<Player>());
    }
    public void MoveAir(Player thisObj)
    {
        xVelocity = (thisObj.rb.velocity.x + (thisObj.xInput * acceleration));
        if (xVelocity > maxSpeed+2 || xVelocity < -maxSpeed-2)
        {
            xVelocity *= 0.95f;
        }
        else
        {
            xVelocity = Mathf.Clamp(xVelocity, -maxSpeed, maxSpeed);
        }

        if (thisObj.xInput == 0.0f || thisObj.xInput * xVelocity < 0)
        {
            if (xVelocity > 0.0f)
            {
                xVelocity = Mathf.Clamp(xVelocity - airDrag, 0.0f, maxSpeed);
            }

            else
            {
                xVelocity = Mathf.Clamp(xVelocity + airDrag, -maxSpeed, 0.0f);
            }
        }

        if (thisObj.jumpPressed)
        {
            Jump(1.5f);
        }

        yVelocity = thisObj.rb.velocity.y;
        if (yVelocity > maxSpeed + 2 || yVelocity < -maxSpeed - 2)
        {
            yVelocity *= 0.9f;
        }
        else
        {
            Mathf.Clamp(thisObj.rb.velocity.y, -jumpMaxSpeed, jumpMaxSpeed);
        }
        Vector2 newVelocity = new Vector2(xVelocity, yVelocity);
        thisObj.rb.velocity = Vector2.SmoothDamp(thisObj.rb.velocity, newVelocity, ref velocityRef, Time.deltaTime);
    }
    // Jump
    public void Jump()
    {
        Jump(1.0f, GetComponent<Player>());
    }
    public void Jump(float jumpMultiplier)
    {
        Jump(jumpMultiplier, GetComponent<Player>());
    }
    public void Jump(float jumpMultiplier, Player thisObj)
    {
        if (thisObj.currentJumpCount > 0)
        {
            thisObj.rb.velocity = (new Vector2(thisObj.rb.velocity.x, jumpHeight * jumpMultiplier));
            thisObj.currentJumpCount -= 1;
        }
    }
    // Dash
    public void Dash()
    {
        Dash(0.6f, 0.6f, GetComponent<Player>());
    }
    public void Dash(float xScale, float yScale)
    {
        Dash(xScale, yScale, GetComponent<Player>());
    }
    public void Dash(float xScale, float yScale, Player thisObj)
    {
        if (thisObj.currentDashCount > 0)
        {
            thisObj.rb.AddForce(new Vector2(thisObj.xInput * xScale, thisObj.yInput * yScale));
            thisObj.currentDashCount -= 1;
        }
    }
    // Enable Gravity
    public void EnableGravity()
    {
        EnableGravity(GetComponent<Player>());
    }
    public void EnableGravity(float gravityChange)
    {
        EnableGravity(GetComponent<Player>(),gravityChange);
    }
    public void EnableGravity(Player thisObj)
    {
        thisObj.rb.gravityScale = gravity;
    }
    public void EnableGravity(Player thisObj, float gravityChange)
    {
        thisObj.rb.gravityScale = gravityChange;
    }
    // Disable Gravity
    public void DisableGravity()
    {
        DisableGravity(GetComponent<Player>());
    }
    public void DisableGravity(Player thisObj)
    {
        thisObj.rb.gravityScale = 0;
    }
    // Move Linearly in a Direction
    public void LinearMovement(float x, float y)
    {
        LinearMovement(x, y, GetComponent<Player>());
    }
    public void LinearMovement(Vector2 direction)
    {
        LinearMovement(direction.x, direction.y, GetComponent<Player>());
    }
    public void LinearMovement(float x, float y, Player thisObj)
    {
        Vector2 newVelocity = new Vector2(x, y);
        thisObj.rb.velocity = thisObj.rb.velocity = Vector2.SmoothDamp(thisObj.rb.velocity, newVelocity, ref velocityRef, Time.deltaTime);
    }
    // Automatically Step Over Walls
    public void AutoStep(Player thisObj, Collision2D collision)
    {
        float x = thisObj.transform.position.y - thisObj.collider.bounds.size.y / 2;
        float y = collision.transform.position.y + collision.collider.bounds.size.y / 2;
        if (y - x < 2.0f)
        {
            thisObj.rb.MovePosition(new Vector2(thisObj.rb.position.x, thisObj.rb.position.y + (y - x) + 0.1f));
        }
    }
    public void AutoStep(Player thisObj, Collision2D collision, float sizeDifference)
    {
        float x = thisObj.transform.position.y - thisObj.collider.bounds.size.y / 2;
        float y = collision.transform.position.y + collision.collider.bounds.size.y / 2;
        if (y - x < sizeDifference)
        {
            thisObj.rb.MovePosition(new Vector2(thisObj.rb.position.x, thisObj.rb.position.y + (y - x) + 0.1f));
        }
    }
    // Check if a layer is "Ground"
    private void GroundedCheck()
    {
        RaycastHit2D cast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down * 0.1f, 0.1f, groundMask);
        isGrounded = (cast.collider != null);
    }
    public bool GroundedCheck(Collider2D collider)
    {
        return collider.gameObject.layer == Mathf.Log(groundMask.value, 2.0f);
    }
    // Check if a collision happens with a wall
    public bool WallCheck(Collision2D collision)
    {
        if (collision.contacts.Length != 0)
        {
            return Mathf.Approximately(collision.GetContact(0).normal.y, 0.0f);
        }
        return false;
    }
    // Check if a collision happens with the floor
    public bool FloorCheck(Collision2D collision)
    {
        if (collision.contacts.Length != 0)
        {
            return Mathf.Approximately(collision.GetContact(0).normal.y, 1.0f);
        }
        return false;
    }
}
