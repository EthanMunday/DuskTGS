using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerState
{
    public virtual void handleInput(Player thisObj) { }

    public virtual void calculateMovement(Player thisObj)
    {
        thisObj.xVelocity = Mathf.Clamp(thisObj.xVelocity + (thisObj.xInput * thisObj.acceleration), -thisObj.maxSpeed, thisObj.maxSpeed);
        if (thisObj.xInput == 0.0f || thisObj.xInput * thisObj.xVelocity < 0)
        {
            if (thisObj.xVelocity > 0.0f)
            {
                thisObj.xVelocity = Mathf.Clamp(thisObj.xVelocity - thisObj.groundDrag, 0.0f, thisObj.maxSpeed);
            }

            else
            {
                thisObj.xVelocity = Mathf.Clamp(thisObj.xVelocity + thisObj.groundDrag, -thisObj.maxSpeed, 0.0f);
            }
        }

        if (thisObj.isGrounded && thisObj.yInput == 1.0f)
        {
            thisObj.rb.AddForce(new Vector2(0, thisObj.jumpHeight));
        }
        thisObj.yVelocity = Mathf.Clamp(thisObj.rb.velocity.y, -thisObj.jumpMaxSpeed, thisObj.jumpMaxSpeed);

        Vector2 newVelocity = new Vector2(thisObj.xVelocity, thisObj.yVelocity);
        thisObj.rb.velocity = newVelocity;
    }

    public virtual void report(Player thisObj) { }

}

public class GroundedState : PlayerState
{
    public override void handleInput(Player thisObj)
    {
        if (Input.GetKey(KeyCode.D))
        {
            thisObj.xInput = 1.0f;

            if (Input.GetKey(KeyCode.A))
            {
                thisObj.xInput = 0.0f;
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            thisObj.xInput = -1.0f;
        }

        else
        {
            thisObj.xInput = 0.0f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            thisObj.yInput = 1.0f;
        }

        else
        {
            thisObj.yInput = 0.0f;
        }

    }
    public override void report(Player thisObj)
    {
        //Debug.Log("Idle");
    }
}


public class Player : MonoBehaviour
{
    private PlayerState currentState;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D collider;
    [HideInInspector] public float xVelocity;
    [HideInInspector] public float yVelocity;
    [HideInInspector] public float xInput;
    [HideInInspector] public float yInput;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector2 velocityRef;
    public float acceleration;
    public float jumpHeight;
    public float maxSpeed;
    public float jumpMaxSpeed;
    public float groundDrag;
    public float airDrag;
    public float gravity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        xVelocity = 0.0f;
        yVelocity = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentState = new GroundedState();
        InvokeRepeating("Report", 0.0f, 3.0f);
    }

    void Report()
    {
        currentState.report(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.handleInput(this);
        currentState.calculateMovement(this);

        if(Physics2D.BoxCast(transform.position,collider.size,0.0f,Vector2.down,0.1f, 0))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
