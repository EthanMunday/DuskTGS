using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class PlayerState
{
    public virtual void handleInput(Player thisObj) 
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

            if (Input.GetKey(KeyCode.S))
            {
                thisObj.yInput = 0.0f;
            }
        }

        else if (Input.GetKey(KeyCode.S))
        {
            thisObj.yInput = -1.0f;
        }

        else
        {
            thisObj.yInput = 0.0f;
        }

        thisObj.jumpPressed = Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.Space);
    }

    public virtual void caclulateMovement(Player thisObj) { }

    public virtual void report(Player thisObj) { }

    public virtual void detectHit(Player thisObj, Collision2D collision) { }
    public virtual void exitHit(Player thisObj, Collision2D collision) { }

}

public class GroundedState : PlayerState
{
    public override void caclulateMovement(Player thisObj)
    {
        if (thisObj.isTouchingWall && thisObj.jumpPressed && thisObj.wallClimbCount > 0)
        {
            Debug.Log("Swapped from Grounded to Climbing");
            thisObj.currentWallClimbCount -= 1;
            thisObj.currentState = new ClimbingState();
        }

        if (!thisObj.controller.isGrounded)
        {
            Debug.Log("Swapped from Grounded to Airborne");
            thisObj.currentState = new AirborneState();
        }
        
        if (thisObj.currentState.ToString() == "GroundedState")
        {
            thisObj.controller.MoveGround();
        }
    }

    public override void report(Player thisObj)
    {
        Debug.Log("Grounded");
    }

    public override void detectHit(Player thisObj, Collision2D collision)
    {
        if (thisObj.controller.GroundedCheck(collision.collider) && thisObj.controller.WallCheck(collision))
        {
            thisObj.controller.AutoStep(thisObj, collision);
            thisObj.isTouchingWall = true;
            thisObj.currentWall = collision.gameObject;
            thisObj.wallDirection = collision.GetContact(0).normal;
        }
    }

    public override void exitHit(Player thisObj, Collision2D collision)
    {
        if (collision.gameObject == thisObj.currentWall)
        {
            thisObj.isTouchingWall = false;
        }
    }
}

public class LandingState : GroundedState
{
    float landTimer = 0.3f;
    public override void caclulateMovement(Player thisObj)
    {
        landTimer -= Time.deltaTime;

        if (landTimer <= 0)
        {
            Debug.Log("Swapped from Landing to Grounded");
            thisObj.currentState = new GroundedState();
        }

        if (thisObj.isTouchingWall && thisObj.jumpPressed && thisObj.wallClimbCount > 0)
        {
            Debug.Log("Swapped from Landing to Climbing");
            thisObj.currentWallClimbCount -= 1;
            thisObj.currentState = new ClimbingState();
        }

        if (!thisObj.controller.isGrounded)
        {
            Debug.Log("Swapped from Landing to Airborne");
            thisObj.rb.velocity = new Vector2(thisObj.rb.velocity.x * 2f, thisObj.rb.velocity.y);
            thisObj.currentState = new AirborneState();
        }

        if (thisObj.currentState.ToString() == "LandingState")
        {
            thisObj.controller.MoveGround();
        }
    }

    public override void report(Player thisObj)
    {
        Debug.Log("Landing");
    }
}


public class AirborneState : PlayerState
{
    bool isOnFloor = false;

    public override void caclulateMovement(Player thisObj)
    {
        if (thisObj.isTouchingWall && thisObj.yInput == 1.0f && thisObj.currentWallClimbCount > 0)
        {
            Debug.Log("Swapped from Airborne to Climbing");
            thisObj.currentWallClimbCount -= 1;
            thisObj.currentState = new ClimbingState();
        }

        if (isOnFloor && thisObj.controller.isGrounded)
        {
            Debug.Log("Swapped from Airborne to Landing");
            thisObj.currentJumpCount = thisObj.jumpCount;
            thisObj.currentDashCount = thisObj.dashCount;
            thisObj.currentWallClimbCount = thisObj.wallClimbCount;
            thisObj.currentState = new LandingState();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            thisObj.controller.Dash();
        }

        if (thisObj.currentState.ToString() == "AirborneState")
        {
            thisObj.controller.MoveAir();
        }
    }

    public override void report(Player thisObj)
    {
        Debug.Log("Airborne");
    }

    public override void detectHit(Player thisObj, Collision2D collision)
    {
        if (thisObj.controller.GroundedCheck(collision.collider) && thisObj.controller.WallCheck(collision))
        {
            thisObj.isTouchingWall = true;
            thisObj.currentWall = collision.gameObject;
            thisObj.wallDirection = collision.GetContact(0).normal;
        }

        if (thisObj.controller.FloorCheck(collision))
        {
            isOnFloor = true;
        }
        
    }

    public override void exitHit(Player thisObj, Collision2D collision)
    {
        if (collision.gameObject == thisObj.currentWall)
        {
            thisObj.isTouchingWall = false;
            isOnFloor = false;
        }
    }
}


public class ClimbingState : PlayerState
{
    float flyTimer;
    Vector2 direction;
    public override void caclulateMovement(Player thisObj)
    {
        flyTimer -= Time.deltaTime;
        if (flyTimer <= 0.0f)
        {
            Debug.Log("Swapped from Climbing to Airborne");
            thisObj.currentState = new AirborneState();
        }

        if (thisObj.xInput != 0 && Input.GetKeyDown(KeyCode.J) && thisObj.dashCount > 0)
        {
            Debug.Log("Swapped from Climbing to Airborne");
            thisObj.controller.Dash(0.6f, 0.0f);
            thisObj.currentState = new AirborneState();
        }

        if (!thisObj.isTouchingWall)
        {
            Debug.Log("Swapped from Climbing to Mantle");
            thisObj.currentJumpCount = thisObj.jumpCount;
            thisObj.currentDashCount = thisObj.dashCount;
            thisObj.currentWallClimbCount = thisObj.wallClimbCount;
            thisObj.currentState = new MantleState();
        }

        if (thisObj.currentState.ToString() == "ClimbingState")
        {
            thisObj.controller.LinearMovement(direction);
        }
    }

    public override void report(Player thisObj)
    {
        Debug.Log("Climbing");
    }

    public override void detectHit(Player thisObj, Collision2D collision)
    {
        if (thisObj.controller.GroundedCheck(collision.collider) && thisObj.controller.WallCheck(collision))
        {
            thisObj.isTouchingWall = true;
            thisObj.currentWall = collision.gameObject;
        }
    }

    public override void exitHit(Player thisObj, Collision2D collision)
    {
        if (collision.gameObject == thisObj.currentWall)
        {
            thisObj.isTouchingWall = false;
        }
    }

    public ClimbingState()
    {
        flyTimer = 0.6f;
        direction = new Vector2(0.0f,15.0f);
    }
    public ClimbingState(float inputTimer)
    {
        flyTimer = inputTimer;
        direction = new Vector2(0.0f, 30.0f);
    }
    public ClimbingState(float inputTimer, Vector2 inputDirection)
    {
        flyTimer = inputTimer;
        direction = inputDirection;
    }
}

public class MantleState : PlayerState
{
    float mantleTime = 0.05f;
    public override void caclulateMovement(Player thisObj)
    {
        mantleTime -= Time.deltaTime;
        if (mantleTime > 0)
        {
            thisObj.controller.LinearMovement(0.0f, 10.0f);
            return;
        }
        Debug.Log("Swapped from Mantle to Airborne");
        thisObj.rb.velocity = new Vector2(-thisObj.wallDirection.x * 80, 0f);
        thisObj.currentState = new AirborneState();
    }

    public override void report(Player thisObj)
    {
        Debug.Log("Mantle");
    }
}


public class Player : MonoBehaviour
{
    //Components
    [HideInInspector] public PlayerState currentState;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public BoxCollider2D collider;
    [HideInInspector] public PlayerController controller;

    //Details
    [HideInInspector] public float xInput;
    [HideInInspector] public float yInput;
    [HideInInspector] public bool jumpPressed;
    [HideInInspector] public bool isTouchingWall;
    [HideInInspector] public GameObject currentWall;
    [HideInInspector] public Vector2 wallDirection;

    //Customisables
    [HideInInspector] public float jumpCount;
    [HideInInspector] public float currentJumpCount;
    [HideInInspector] public float dashCount;
    [HideInInspector] public float currentDashCount;
    [HideInInspector] public float wallClimbCount;
    [HideInInspector] public float currentWallClimbCount;


    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        jumpCount = 0;
        currentJumpCount = jumpCount;
        dashCount = 0;
        currentDashCount = dashCount;
        wallClimbCount = 0;
        currentWallClimbCount = wallClimbCount;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        currentState.handleInput(this);
        currentState.caclulateMovement(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentState.detectHit(this, collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        currentState.exitHit(this, collision);
    }

    public void AddSpeed(float addedSpeed)
    {
        controller.acceleration += addedSpeed / 20;
        controller.maxSpeed += addedSpeed;
        controller.jumpMaxSpeed += addedSpeed;
    }
    public void MinusSpeed(float addedSpeed)
    {
        controller.acceleration -= addedSpeed / 20;
        controller.maxSpeed -= addedSpeed;
        controller.jumpMaxSpeed -= addedSpeed;
    }

    public void AddJumps()
    {
        jumpCount++;
    }
    public void AddJumps(int addedJumps)
    {
        jumpCount += addedJumps;
    }
    public void MinusJumps()
    {
        jumpCount--;
    }
    public void MinusJumps(int minusJumps)
    {
        jumpCount -= minusJumps;
    }

    public void AddWallClimbs()
    {
        wallClimbCount++;
    }
    public void AddWallClimbs(int addedWallClimbs)
    {
        wallClimbCount += addedWallClimbs;
    }
    public void MinusWallClimbs()
    {
        wallClimbCount--;
    }
    public void MinusWallClimbs(int minusWallClimbs)
    {
        wallClimbCount -= minusWallClimbs;
    }

    public void AddDashes()
    {
        dashCount++;
    }
    public void AddDashes(int addedDashes)
    {
        dashCount += addedDashes;
    }
    public void MinusDashes()
    {
        dashCount--;
    }
    public void MinusDashes(int minusDashes)
    {
        dashCount -= minusDashes;
    }
}

