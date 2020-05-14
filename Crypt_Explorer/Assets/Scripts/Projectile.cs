using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileInfo info;
    private float lifeTime;
    private Rigidbody2D rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * info.speed;
        lifeTime = info.lifeTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {

            enemy.TakeDamage(info.damage);
            Destroy(gameObject);
        }
        
        if (info.canBounce)
        {
            //Reduce bounce
            return;
        }
        else
        {
           
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        //clean?
    }
}
