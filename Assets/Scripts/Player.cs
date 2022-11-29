using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerState
{
    public virtual void handleInput(Player thisObj) { }

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
        thisObj.controller.MoveGround();

        if (!thisObj.controller.isGrounded)
        {
            Debug.Log("Swapped from Grounded to Airborne");
            thisObj.currentState = new AirborneState();
        }

    }
    public override void report(Player thisObj)
    {
        Debug.Log("Grounded");
    }
}

public class AirborneState : PlayerState
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

        if (thisObj.controller.isGrounded)
        {
            Debug.Log("Swapped from Airborne to Grounded");
            thisObj.currentState = new GroundedState();
            thisObj.currentJumpCount = thisObj.jumpCount;
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && thisObj.currentJumpCount > 0)
        {
            Debug.Log("Swapped from Airborne to Flying");
            thisObj.currentState = new FlyingState();
            thisObj.currentJumpCount -= 1;
        }

        thisObj.controller.MoveAir();
    }

    public override void report(Player thisObj)
    {
        Debug.Log("Airborne");
    }
}

public class FlyingState : PlayerState
{
    float flytimer = 1.0f;

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

        flytimer -= Time.deltaTime;
        if (flytimer <= 0.0f)
        {
            Debug.Log("Swapped from Flying to Airborne");
            thisObj.currentState = new AirborneState();
        }

        thisObj.controller.LinearMovement(0.0f, 10.0f);
    }

    public override void report(Player thisObj)
    {
        Debug.Log("Flying");
    }
}


public class Player : MonoBehaviour
{
    //Components
    [HideInInspector] public PlayerState currentState;
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D collider;

    //Details
    [HideInInspector] public float xInput;
    [HideInInspector] public float yInput;

    //Customisables
    [HideInInspector] public float jumpCount;
    [HideInInspector] public float currentJumpCount;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        jumpCount = 1;
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
    }
}
