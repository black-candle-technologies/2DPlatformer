using UnityEngine;

public class CharacterController : MonoBehaviour
{

    // Animator
    [SerializeField] private Animator animator;

    // Character Collider
    [SerializeField] private BoxCollider2D characterCollider;
    private float colliderSizeHorizontal = 0.25f;
    private float colliderSizeVertical = 0.35f;

    // Character Walk/Run
    private float horizontal;

    private bool isFacingRight = true;

    [SerializeField] private float speed = 4f;
    [SerializeField] private float jumpingPower = 8f;
    [SerializeField] private float groundCheckRange = 0.1f;

    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Character Sliding
    private bool isSliding = false;
    
    // How long the character has been sliding
    private float slideTimer = 0f;

    // The maximum amount of time the character can slide for
    [SerializeField] private float maxSlideTime = 1.5f;

    void Update()
    {
        checkUserInput();
    }

    // Fixed update is called at intervals
    void FixedUpdate()
    {
        updateUserVelocity();
    }

    // Check for user input
    void checkUserInput () 
    {
        // Check which direction the player should be moving
        horizontal = Input.GetAxisRaw("Horizontal");


        // Flip the character's sprite if they are moving in the opposite direction of which they face
        flipCharacterSprite();

        // Check if the 'w' or Space buttons are pressed down and if the user s on the ground
        if(Input.GetButtonDown("Jump") && isGrounded())
        {
            // Make the user jump
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpingPower);
        }

        // Check if the 'w' or Space button is not pressed down and if the user is moving upwards
        if (Input.GetButtonUp("Jump") && rigidBody.velocity.y > 0f)
        {
            // Make the user slowly lose momentum upwards
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y * 0.5f);
        }

        // Check if the 's' or Left Control buttons are pressed down
        if(Input.GetButtonDown("Slide") && !isSliding) startSlide();

        if(isSliding)
        {
            if(Input.GetButtonUp("Slide") || horizontal == 0f) cancelSlide();

            slideTimer += Time.deltaTime;
            if(slideTimer > maxSlideTime) cancelSlide();
        }
    }

    void startSlide()
    {
        isSliding = true;
        slideTimer = 0f;
        
        // play slide animation

        // resize character collider    
        Vector2 size = characterCollider.size;
        size.x = colliderSizeVertical;
        size.y = colliderSizeHorizontal;
        characterCollider.size = size;
    }

    void cancelSlide()
    {
        isSliding = false;
        Vector2 size = characterCollider.size;
        size.x = colliderSizeHorizontal;
        size.y = colliderSizeVertical;
        characterCollider.size = size;
    }

    void updateUserVelocity()
    {
        // Make the user move in the direction they are indicating
        if(isSliding) horizontal *= 1.5f;
        rigidBody.velocity = new Vector2(horizontal * speed, rigidBody.velocity.y);
        if(horizontal != 0f) animator.SetBool("isRunning", true);
        else animator.SetBool("isRunning", false);
    }

    // Check if the user is on the ground
    private bool isGrounded()
    {
        // Check if the Ground Check object is in proximity to the ground
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRange, groundLayer);
    }

    private void flipCharacterSprite()
    {
        // Check if the user is facing in the same direction that they're moving
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            // Flip the character model if not
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

}
