using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 2;
    [SerializeField] float velocity;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private CharacterController2D charController;
    private bool isJumping;
    
    // Start is called before the first frame update
    void Start()
    {
        if (animator == null)
           animator = GetComponent<Animator>();

        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        velocity = Input.GetAxisRaw("Horizontal") * speed;

        if(Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            animator.SetTrigger("Jump");
        }
    }

    private void FixedUpdate()
    {
       charController.Move(velocity * Time.fixedDeltaTime, false, isJumping);
        animator.SetFloat("Velocity", Mathf.Abs(velocity));
        if (rigidbody.velocity.y < -0.2f)
            animator.SetBool("IsFalling", true);
        else
            animator.SetBool("IsFalling", false);
    }

    public void OnLand()
    {
        Debug.Log("Landed");
        animator.SetBool("IsFalling", false);
        isJumping = false;
    }
}
