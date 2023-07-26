
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float groundedAcceleration = 1;
    [SerializeField] private float groundedAccelerationDecrease = 1;
    [SerializeField] private float aerialAcceleration = 1;
    [SerializeField] private float groundedFriction = 1;
    [SerializeField] private float aerialDrag = 1;

    [SerializeField] private float diveSpeed = 1;
    [SerializeField] private float dashSpeed = 1;
    [SerializeField] private float afterDashVelocity = 1;
    [SerializeField] private float diveBounceSpeed = 3;
    [SerializeField] private float boxCastDistance = 0.2f;

    [SerializeField] private Vector2 boxCastSize = new Vector2(0.15f, 0.15f);


    [SerializeField] private float gravity = 1;

    [SerializeField] private float dashTime = 1;


    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private BoxCollider2D collisionBox;


    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private LayerMask groundMask;

    [Space()]
    [Header("Sprite")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite attackingSprite;
    [SerializeField] Color defaultColor;
    [SerializeField] Color attackingColor;

    private Vector2 pausedVelocity;

    private PlayerControls controls;
    private Camera mainCamera;


    private int dashes;
    [SerializeField] int maxDashCount = 3;
    private bool hasDivedSinceLastOnGround = false;
    private bool shouldSlowVelocityAfterDash = false;
    private bool isOnIce = false;
    private bool levelIsComplete = false;
    public bool isPaused = false;

    private Vector2 movement;
    private float dive;
    private float dash;
    private float timeLastOnGround;


    private float timeLastDashed = -99;

    private void Awake()
    {
        dashes = maxDashCount;
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx =>
        {
            movement = ctx.ReadValue<Vector2>();
        };
        controls.Player.Move.canceled += ctx => movement = Vector2.zero;

        controls.Player.Dive.started += ctx =>
        {
            if (!isPaused) dive = ctx.ReadValue<float>();
        };
        controls.Player.Dive.canceled += ctx => dive = 0;

        controls.Player.Dash.started += ctx =>
        {
            dash = ctx.ReadValue<float>();
        };

        controls.Player.Dash.canceled += ctx => dash = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!(levelIsComplete || isPaused))
        {
            HandleSprite();
            HandleDiving();
            HandleDashing();
            // If the player hasn't dashed recently, apply gravity and acceleration
            if (timeLastDashed + dashTime < Time.time)
            {
                HandleAcceleration();
                HandleGravity();
            }
        }
    }

    void HandleSprite()
    {
        this.spriteRenderer.sprite = Attacking() ? this.attackingSprite : this.defaultSprite;
        this.spriteRenderer.color = Attacking() ? this.attackingColor : this.defaultColor;
    }

    private void HandleDiving()
    {

        if (dive > 0)
        {
            if (!IsOnGround())
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x / 10, -diveSpeed);
                hasDivedSinceLastOnGround = true;
            }
        }
        dive = 0;
    }

    private void HandleDashing()
    {
        if (dashes < 1)
        {
            if (IsOnGround())
            {
                dashes = maxDashCount;

            }
        }

        if (dash > 0 && dashes > 0 && !IsOnGround())
        {
            Vector2 directionToMouse = GetDirectionToCursor();
            rigidBody2D.velocity = directionToMouse * dashSpeed;
            dash = 0;
            timeLastDashed = Time.time;
            dashes -= 1;
            shouldSlowVelocityAfterDash = true;
            hasDivedSinceLastOnGround = false;
        }

        if (timeLastDashed + dashTime < Time.time && shouldSlowVelocityAfterDash)
        {
            rigidBody2D.velocity = rigidBody2D.velocity.normalized * afterDashVelocity;
            shouldSlowVelocityAfterDash = false;
        }
    }

    private Vector2 GetDirectionToCursor()
    {
        // Get the position of the cursor in screen space
        Vector2 cursorScreenPos = Mouse.current.position.ReadValue();

        // Convert the screen position to world position on the same plane as the GameObject
        Vector3 cursorWorldPos = mainCamera.ScreenToWorldPoint(cursorScreenPos);
        cursorWorldPos.z = transform.position.z; // Keep the same z position as the GameObject

        // Calculate the direction from the GameObject to the cursor
        Vector2 directionToCursor = (cursorWorldPos - transform.position).normalized;

        return directionToCursor;
    }

    private void HandleAcceleration()
    {
        if (IsOnGround())
        {
            rigidBody2D.velocity += new Vector2((movement.x * groundedAcceleration) * Time.deltaTime, 0);
            HandleDrag();
        }
        else
        {
            rigidBody2D.velocity += new Vector2(movement.x * aerialAcceleration * Time.deltaTime, 0);
            HandleDrag();
        }
    }

    private void HandleFriction()
    {

        // If the force of the friction would NOT cause the velocity vector to switch directions (velocity is not too small)
        if (rigidBody2D.velocity.magnitude > groundedFriction * Time.deltaTime)
        {
            rigidBody2D.velocity -= rigidBody2D.velocity.normalized * groundedFriction * Time.deltaTime;
        }
        else
        {
            rigidBody2D.velocity = Vector2.zero;
        }

    }

    private void HandleDrag()
    {
        if (!hasDivedSinceLastOnGround)
        {
            rigidBody2D.velocity -= rigidBody2D.velocity.normalized * rigidBody2D.velocity.sqrMagnitude * aerialDrag * Time.deltaTime * 1f;
        }

    }

    private void HandleGravity()
    {
        rigidBody2D.velocity -= new Vector2(0, gravity * Time.deltaTime);
    }

    bool Attacking()
    {
        return hasDivedSinceLastOnGround || timeLastDashed + dashTime > Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.TryGetComponent(out EnemyScript enemy))
        {
            if (Attacking())
            {
                enemy.Kill();
            }else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

        }
        if (IsOnGround())
        {
            timeLastDashed = 0;
            if (IsOnGround()) {
                //TODO: play landing sound
            }
        }
        
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!levelIsComplete)
        {
            if (IsOnGround())
            {
                if (hasDivedSinceLastOnGround)
                {
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, diveBounceSpeed);
                    hasDivedSinceLastOnGround = false;
                }

                timeLastOnGround = Time.time;
            }

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isOnIce = collision.tag.Equals("Ice");
        if (collision.tag.Equals("No-Jump Surface")) {
            hasDivedSinceLastOnGround = false;
        }
    }

    public bool IsOnGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCastSize * 0.9f, 0, Vector2.down, boxCastDistance, groundMask);
        if (hit.collider != null) {
        }
        return hit;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {

        controls.Disable();
    }

    public void SetPause(bool value)
    {
        isPaused = value;
        if (value)
        {
            pausedVelocity = rigidBody2D.velocity;
            rigidBody2D.velocity = Vector2.zero;
        } else
        {
            rigidBody2D.velocity = pausedVelocity;
        }
    }
}
