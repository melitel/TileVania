using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    Vector2 moveImput;
    Rigidbody2D m_RigidBody;
    Animator m_Animator;
    CapsuleCollider2D m_BodyCollider;
    BoxCollider2D m_FeetCollider;

    float startingGravity;
    bool isAlive = true;
    
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();
        m_BodyCollider = GetComponent<CapsuleCollider2D>();
        m_FeetCollider = GetComponent<BoxCollider2D>();
        startingGravity = m_RigidBody.gravityScale;
    }
   
    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        CLimbLAdder();
        Die();
    }

    void OnMove(InputValue value) 
    {
        if (!isAlive) { return; }
        moveImput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        int layerMask = LayerMask.GetMask("Ground");
        bool isTouchingGround = m_FeetCollider.IsTouchingLayers(layerMask);
        
        if (value.isPressed && isTouchingGround)
        {
            m_RigidBody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        if (value.isPressed) 
        {
            Instantiate(bullet, gun.position, transform.rotation);
        }

    }

    void Run()
    {        
        Vector2 playerVelocity = new Vector2 (moveImput.x * runSpeed, m_RigidBody.velocity.y);
        m_RigidBody.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(m_RigidBody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            m_Animator.SetBool("isRunning", true);
        }
        else {
            m_Animator.SetBool("isRunning", false);
        }
    }

    void CLimbLAdder()
    {        
        bool isTouchingLadder = m_BodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        bool playerHasVerticalSpeed = Mathf.Abs(m_RigidBody.velocity.y) > Mathf.Epsilon;

        if (isTouchingLadder)
        {            
            Vector2 climbVelocity = new Vector2(m_RigidBody.velocity.x, moveImput.y * climbSpeed);
            m_RigidBody.velocity = climbVelocity;
            m_RigidBody.gravityScale = 0f;
            m_Animator.SetBool("isClimbing", playerHasVerticalSpeed);
        }
        else {
            m_Animator.SetBool("isClimbing", false);
            m_RigidBody.gravityScale = startingGravity;
        }
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(m_RigidBody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed) 
        {
            transform.localScale = new Vector2 (Mathf.Sign(m_RigidBody.velocity.x), 1f);
        }
    
    }

    void Die()
    {
        if (m_BodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            m_Animator.SetTrigger("Dying");
            m_RigidBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
