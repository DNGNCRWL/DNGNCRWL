using UnityEngine;
using UnityEngine.AI;
using static Navigation;
using UnityEngine.SceneManagement;

public class enemy_AI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    
    public LayerMask whatIsGround, whatIsPlayer;
    public Vector3 currentLoc;
    Animator animator;

    public float xmove;
    public float zmove;


    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float sightRange;

    public bool isInRange;

    private void Start()
    {
        player = GameObject.FindWithTag("Navigator 1").transform;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Navigator 1").transform;
        agent = GetComponent<NavMeshAgent>();
        currentLoc = transform.position;
    }

    private void OnEnable()
    {
        player = GameObject.FindWithTag("Navigator 1").transform;
    }
    private void Update()
    {
        isInRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        xmove = currentLoc.x - transform.position.x;
        zmove = currentLoc.z - transform.position.z;

         if(zmove + xmove >= 4 || zmove + xmove <=-4)
          {
               //animator.SetBool("New Bool", false);
               currentLoc = transform.position;
               zmove = 0;
               xmove = 0;
               Patroling();
          }

        if (!isInRange)
        {
            Patroling();
        }

        else
        {
            //      if (Navigation.isMoving)
            //    {
            //animator.SetBool("New Bool", true);
            chasePlayer();
            //    }
        }

    }

    private void Patroling()
    {
        if (!walkPointSet) searchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 disToWalkPoint = transform.position - walkPoint;

        if (disToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void searchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void chasePlayer()
    {
        currentLoc = transform.position;
        agent.SetDestination(player.position);
        //Navigation.isMoving = false;
    }
    

}

