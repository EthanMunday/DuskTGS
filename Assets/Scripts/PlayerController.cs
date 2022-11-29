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

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        xVelocity = 0.0f;
        yVelocity = 0.0f;
    }

    public void MoveGround()
    {
        MoveGround(this.GetComponent<Player>());
    }

    public void MoveGround(Player thisObj)
    {
        xVelocity = Mathf.Clamp(xVelocity + (thisObj.xInput * acceleration), -maxSpeed, maxSpeed);
        if (thisObj.xInput == 0.0f || thisObj.xInput * xVelocity < 0)
        {
            if (xVelocity > 0.0f)
            {
                xVelocity = Mathf.Clamp(xVelocity - groundDrag, 0.0f, maxSpeed);
            }

            else
            {
                xVelocity = Mathf.Clamp(xVelocity + groundDrag, -maxSpeed, 0.0f);
            }
        }

        if (isGrounded && thisObj.yInput == 1.0f)
        {
            thisObj.rb.velocity = (new Vector2(0, jumpHeight));
        }
        yVelocity = Mathf.Clamp(thisObj.rb.velocity.y, -jumpMaxSpeed, jumpMaxSpeed);

        Vector2 newVelocity = new Vector2(xVelocity, yVelocity);
        thisObj.rb.velocity = Vector2.SmoothDamp(thisObj.rb.velocity, newVelocity, ref velocityRef, Time.deltaTime);
    }

    public void MoveAir()
    {
        MoveAir(GetComponent<Player>());
    }

    public void MoveAir(Player thisObj)
    {
        xVelocity = Mathf.Clamp(xVelocity + (thisObj.xInput * acceleration), -maxSpeed, maxSpeed);
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
        yVelocity = Mathf.Clamp(thisObj.rb.velocity.y, -jumpMaxSpeed, jumpMaxSpeed);
        Vector2 newVelocity = new Vector2(xVelocity, yVelocity);
        thisObj.rb.velocity = Vector2.SmoothDamp(thisObj.rb.velocity, newVelocity, ref velocityRef, Time.deltaTime);
    }

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

    public void DisableGravity()
    {
        DisableGravity(GetComponent<Player>());
    }

    public void DisableGravity(Player thisObj)
    {
        thisObj.rb.gravityScale = 0;
    }

    public void LinearMovement(float x, float y)
    {
        LinearMovement(x, y, GetComponent<Player>());
    }

    public void LinearMovement(float x, float y, Player thisObj)
    {
        Vector2 newVelocity = new Vector2(x, y);
        thisObj.rb.velocity = thisObj.rb.velocity = Vector2.SmoothDamp(thisObj.rb.velocity, newVelocity, ref velocityRef, Time.deltaTime);
    }

    private void GroundedCheck()
    {
        RaycastHit2D cast = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down * 0.1f, 0.1f, groundMask);
        isGrounded = (cast.collider != null);
    }

    // Update is called once per frame
    void Update()
    {
        GroundedCheck();
    }
}
