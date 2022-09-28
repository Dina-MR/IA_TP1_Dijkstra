using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    bool movement = false;
    [SerializeField] float hurtForce = 4.0f;
    // Shooting
    public GameObject bulletPrefab;
    private Vector3 gunPosition;
    public bool hit = false;
    public bool touchEnemy = false;
    public bool touchDeathZone = false;
    // for healing
    public bool touchCoin = false;
    public bool gameOver = false;

    // HP
    public float maxHp = 100;
    public float currentHp;

    // Movement speeds & force
    [Header("Speed & Forces")]
    public float walkingSpeed;
    public float runningSpeed;
    public float jumpForce;
    public float dashForce;
    
    [Header("Collisions")]
    public Rigidbody2D rb;
    // For collision
    public LayerMask groundLayer;

    [Header("Animation")]
    // For animation
    public Animator anim;
    public enum State {idle, running, jumping, falling, hurting};
    public State state = State.idle;

    [Header("Movements")]
    // Directional movements
    public float horizontalMovement;
    public float verticalMovement;
    private Vector3 velocity = Vector3.zero;

    [Header("Jump & Gravity")]
    public float gravity = 1;
    public float linearDrag = 4f;
    public float fallMultiplier = 5;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    // State of the player, used for animations
    public bool isGrounded;
    public bool isDashing = false;
    public bool isRunning = false;
    private bool isJumping = false;
    // For audio shoot
    private PlayerSound playerSound;

    public float groundDistance = 1.0f;
    bool isChangingScene = false;
    void Awake() {
        currentHp = maxHp;
        DontDestroyOnLoad(this.gameObject);
        anim = GetComponent<Animator>();
        playerSound = GetComponent<PlayerSound>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            Destroy(this.gameObject);
        }
        if (currentHp <=0 ) {
            gameOver = true;
        }
        gunPosition = gameObject.transform.GetChild(0).position;
        if (state != State.hurting) {
            SecondaryMovement();
        }
    }

    void FixedUpdate() 
    {
        float currentSpeed = walkingSpeed; //by default, the speed is the one for walking

        // Update de speed when the player is about to run
        if(isRunning)
        {
            currentSpeed = runningSpeed;
        }
        // When the player is about to jump
        if(jumpTimer > Time.time && isGrounded)
        {
            isJumping = true;
        }
        
        horizontalMovement = Input.GetAxis("Horizontal") * currentSpeed * Time.fixedDeltaTime;
        verticalMovement = Input.GetAxis("Vertical") * currentSpeed * Time.fixedDeltaTime;
        if (horizontalMovement > 0) {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (horizontalMovement < 0) {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayer);
        if (state != State.hurting) {
            MovePlayer(horizontalMovement, verticalMovement);
        }
        ChangeState();
        anim.SetInteger("State", (int)state);
    }
    void SecondaryMovement() {
        // Check if player is on ground
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayer);

        // For jumping
        if(Input.GetKeyDown(KeyCode.X))
        {
            //isJumping = true;
            jumpTimer = Time.time +  jumpDelay;
            state = State.jumping;
        }
        // For dashing (Z key pressed)
        if(Input.GetKeyDown("z") && !isDashing)
        {
            isDashing = true;
            playerSound.PlaySoundEffect(playerSound.dashClip);
        }
        // For running (Z key down)
        if(Input.GetKey("z"))
        {
            isRunning = true;
        }
        // For walking again
        if(Input.GetKeyUp("z") && isRunning)
        {
            isRunning = false;
        }
        // For shooting
        if (Input.GetKeyDown(KeyCode.C)) {
            Debug.Log("Shooting");
            Instantiate(bulletPrefab, gunPosition, transform.rotation);
            playerSound.PlayShoot();
        }
    }

    void MovePlayer(float _horizontalMovement, float _verticalMovement)
    {
        // Horizontal movement
        Vector3 targetVelocity = new Vector2(_horizontalMovement, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

        // When the player is about to jump
        if(isJumping)
        {
            playerSound.PlaySoundEffect(playerSound.jumpClip);
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = false;
        }
        // When the player is about to dash
        if(isDashing)
        {
            // Dashing is always horizontal, even if the player is in the air
            rb.AddForce(new Vector2(_horizontalMovement, 0) * dashForce);
            isDashing = false;
        }
    }

    // Gravity will vary depending on the distance between the player and the ground
    void ManageGravity()
    {
        if(isGrounded)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if(rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            // The player will fall quicker if they only press the jump button
            else if(rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    // FOR SCENE ONLY
    /// Display the distance between the center of the player and their bottom
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet")) {
            state = State.hurting;
            Destroy(other.gameObject);
            hit = true;
            if (other.gameObject.transform.position.x > transform.position.x) {
                //push left
                rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
            } else {
                //push right
                rb.velocity = new Vector2(hurtForce, rb.velocity.y);
            }
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy")) {
            touchEnemy = true;
            state = State.hurting;
            if (other.gameObject.transform.position.x > transform.position.x) {
                //push left
                rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
            } else {
                //push right
                rb.velocity = new Vector2(hurtForce, rb.velocity.y);
            }
        }
    }
    void ChangeState() {
        if (state == State.jumping) {
            if (rb.velocity.y < 0.1f) {
                state = State.falling;
            }
        }
        else if (state == State.falling) {
            if (isGrounded) {
                playerSound.PlaySoundEffect(playerSound.jumpEndClip);
                state = State.idle;
            }
        }
        else if (state == State.hurting) {
            if (Mathf.Abs(rb.velocity.x) < 0.1f) {
                state = State.idle;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f) {
            //Moving
            playerSound.PlaySoundEffect(playerSound.footStepClip);
            state = State.running;
        }
        else {
            state = State.idle;
        }
    }
}
