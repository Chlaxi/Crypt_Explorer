using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private Vector2 attackOffset;
    [SerializeField] private Rigidbody2D Projectile;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private float attackSpeed = 1f;
    private float _attackTimer;
    private bool canAttack = true;


    [SerializeField] private float speed = 2;
    private float velocity;


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

        if (Input.GetButtonDown("Attack")){
            Attack();
        }

        if (!canAttack)
        {
            staminaSlider.value = _attackTimer;
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0)
            {
                canAttack = true;
            }
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

    private void Attack()
    {
        if (!canAttack)
            return;

        //Actual attack.



        Rigidbody2D _projectile = Instantiate(Projectile, GetAttackPoint() , transform.rotation);

        Debug.Log("Attack!");

        canAttack = false;
        _attackTimer = attackSpeed;
    }

    public Vector3 GetAttackPoint()
    {
        Vector3 attackPoint = transform.position;
        attackPoint.y += attackOffset.y;
        attackPoint.x += charController.GetDirection() * attackOffset.x;
        return attackPoint;
    }

    public void OnLand()
    {
        animator.SetBool("IsFalling", false);
        isJumping = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetAttackPoint(), 0.1f);
    }
}
