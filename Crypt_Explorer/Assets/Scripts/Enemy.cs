using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    protected int health;


    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D collider;
    [SerializeField] private Rigidbody2D rigidbody;
    private EnemyAI AI;
    private void Start()
    {
        AI = GetComponent<EnemyAI>();
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Damage taken: " + damage);
        health -= damage;

        animator.SetTrigger("Hit");

        if(health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        rigidbody.gravityScale = 1;
        AI.ClearPath();
        AI.AIEnabled = false;
        //rigidbody.simulated = false;
        //collider.enabled = false;
        Debug.Log("Dead");
        
        //Play death animation

    }
}
