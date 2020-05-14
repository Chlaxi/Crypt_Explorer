using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum EnemyState { Patrolling, Following, Chasing, Attacking, Waiting}

public class EnemyAI : MonoBehaviour
{

    public Transform target;

    public EnemyState state = EnemyState.Waiting;
    public float view = 3f;
    public LayerMask hostileMask;


    public float speed = 2f;
    public float nextWaypointDistance = 3f;
    [SerializeField] GameObject GFX;
    [SerializeField] bool canFly = false;
    public bool AIEnabled = true;

  
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rigid;
    [SerializeField] private Animator animator;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolDelay = 0.1f;
    private int patrolIndex = 0;
    private bool updatingPatrol = false;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rigid = GetComponent<Rigidbody2D>();

        FlightCheck();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
            seeker.StartPath(rigid.position, target.position, OnPathComplete);
    }



    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null || !AIEnabled)
            return;

        if (state != EnemyState.Chasing)
        {
            Collider2D other = Physics2D.OverlapCircle(transform.position, view, hostileMask);
            if (other != null)
            {
                Debug.Log("Found hostile " + other.gameObject.name);
                target = other.gameObject.transform;

            }
        }

        if(currentWaypoint >= path.vectorPath.Count)
        {
            if (state == EnemyState.Patrolling && !reachedEndOfPath)
            {
                Debug.Log("Waiting " + patrolDelay + " seconds to move to next patrolpoint");
                StartCoroutine("NextPatrol");
            }

            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }


        //move
        Move();

        float distance = Vector2.Distance(rigid.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        


    }

    private void Move()
    {
        Vector2 waypoint = ((Vector2)path.vectorPath[currentWaypoint] - rigid.position).normalized;

        if (!canFly)
        {
            waypoint.y = 0;
            //waypoint.Normalize();
        }

        Vector2 force = waypoint * speed * 100f * Time.deltaTime;
        Debug.Log("Force: " + force);


        rigid.AddForce(force);

        animator.SetFloat("Speed", force.x);

        if (force.x >= 0.01f)
        {
            GFX.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            GFX.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private IEnumerator NextPatrol()
    {
        if (!updatingPatrol)
        {

            updatingPatrol = true;

            state = EnemyState.Waiting;
            yield return new WaitForSeconds(patrolDelay);
            state = EnemyState.Patrolling;

            patrolIndex++;

            if (patrolIndex>=patrolPoints.Length){
                patrolIndex = 0;
            }

            target = patrolPoints[patrolIndex];
            updatingPatrol = false;
        }
    }

    private void FlightCheck()
    {
        if (canFly)
        {
            rigid.gravityScale = 0;
        }
        else
        {
            rigid.gravityScale = 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, nextWaypointDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, view);
    }

public void ClearPath()
    {
        path.path.Clear();
    }    
}
