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

        if (thisObj.controller.yVelocity != 0.0f)
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
        }

        thisObj.controller.MoveAir();
    }

    public override void report(Player thisObj)
    {
        Debug.Log("Airborne");
    }
}



public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerState currentState;
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D collider;
    [HideInInspector] public float xInput;
    [HideInInspector] public float yInput;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
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
