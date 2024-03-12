using HuggingFace.API.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float direction = 0f;
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float jumpForce = 5;
    int jumpCount = 1;
    public float dashForce = 10f; // Added dash force
    private bool isDashing = false; // Added dash state
    public Animator animator;
    public SpriteRenderer playerSprite;
    bool isDying = false;

    public SpeechRecognitionExample sre;


    [SerializeField] Transform playerLight;
    //[SerializeField] RespawnObject respawnObject;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (sre._response.ToLower().Contains("run"))
        {
            walkSpeed = runSpeed;
            animator.speed = 3f;
        }
        if (sre._response.ToLower().Contains("walk"))
        {
            animator.speed = 1f;
            walkSpeed = 5f;
        }

        // Dash input
        if (Input.GetKeyDown(KeyCode.W) && !isDashing)
        {
            animator.SetBool("isDashing", true);
            StartCoroutine(Dash());
        }


        if (sre._response.ToLower().Contains("right")) direction = 1f;
        if (sre._response.ToLower().Contains("left")) direction = -1f;
        if (sre._response.ToLower().Contains("stop")) direction = 0;

        //direction = Input.GetAxisRaw("Horizontal");

        // Normal movement
        if (!isDashing)
        {
            rb.velocity = new Vector2(direction * walkSpeed, rb.velocity.y);
        }

        // Flip sprite
        if (direction != 0)
        {
            playerSprite.flipX = direction < 0;
            animator.SetBool("isWalking", true);
            if (direction > 0)
            {
                playerLight.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else 
            {
                playerLight.localRotation = Quaternion.Euler(0, 180, -90);
            }
            
        }
        else
        {
            animator.SetBool("isWalking", false);
            
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) || sre._response.ToLower().Contains("jump"))
        {
            sre._response = "";
            if (jumpCount > 0 && !isDashing)
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                animator.SetBool("isJumping", true);
                jumpCount = 0;
            }
        }
    }

    

    public void StopDying()
    {
        animator.SetBool("isDying", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            animator.SetBool("isJumping", false);
            jumpCount = 1;
        }
        if(collision.gameObject.tag.Equals("Hazard")){
            animator.SetBool("isDying", true);
            isDying = true;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        float dashTime = 0.2f; // Adjust the dash duration as needed
        rb.velocity = new Vector2(playerSprite.flipX ? -dashForce : dashForce, rb.velocity.y);
        yield return new WaitForSeconds(dashTime);
        rb.velocity = new Vector2(0, rb.velocity.y);
        isDashing = false;
        animator.SetBool("isDashing", false);
    }
}
