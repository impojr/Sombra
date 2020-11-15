using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    private Rigidbody2D rb;
    private float xMovement;
    private float yMovement;

    [Space]
    public float speed = 10;

    [Header("Jump")]
    public float jumpForce = 10;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Space]
    public bool canMove;
    public bool wallJumping;

    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateCoordinates();
        ResetBooleansOnGroundTouch();
        ProcessWalk();
        ProcessJump();
        BetterJumping();
    }

    private void UpdateCoordinates()
    {
        xMovement = Input.GetAxis("Horizontal");
        yMovement = Input.GetAxis("Vertical");
    }

    private void ProcessWalk()
    {
        if (!canMove) { return; }
        Vector2 dir = new Vector2(xMovement, yMovement);
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    private void ProcessJump()
    {
        if (!canMove) { return; }
        if (Input.GetButtonDown("Jump") && coll.onGround)
        {
            Debug.Log("Jumping");
            Jump(Vector2.up);
        }
    }

    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }

    private void BetterJumping()
    {
        if (!canMove) { return; };

        //changes the gravity of the player depending on whether or not they are holding the jump buttom
        //this allows for changes in the jump height depending on button press
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void ResetBooleansOnGroundTouch()
    {
        if (coll.onGround)
        {
            if (wallJumping) { canMove = true; }
            wallJumping = false;
        }
    }
}