using UnityEngine;

public class CupcakeBehavior : MonoBehaviour
{
    public Animator animator;
    
    //behavior states
    enum CupcakeState
    {
        Exploring, 
        Eating,
        Sleeping
    }
    
    CupcakeState currentState;
    
    Vector3 targetPos;
    bool moving;
    
    //how far cupcake can wander off
    public float wanderDistance = 3f;
    
    //how fast cupcake moves
    public float moveSpeed = 1.5f;
    
    //state timer
    float stateTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetState(CupcakeState.Exploring);
        ChooseNewSpot();
    }

    // Update is called once per frame
    void Update()
    {
        //exploring
        if (currentState == CupcakeState.Exploring)
        {
            if (moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                //reached a new spot
                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                {
                    moving = false;
                    //wait then choose another spot
                    stateTimer = 1f;
                }
            }
            else
            {
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    ChooseNewSpot();
                }
            }
        }
        
        //eating state
        if (currentState == CupcakeState.Eating)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                StartSleeping();
            }
        }
        
        //sleeping state
        if (currentState == CupcakeState.Sleeping)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                StartExploring();
            }
        }
    }
    
    //choose random spot
    void ChooseNewSpot()
    {
       float randomX = Random.Range(-wanderDistance, wanderDistance);
       float randomY = Random.Range(-wanderDistance, wanderDistance);

       targetPos = transform.position + new Vector3(randomX, randomY,0);
       moving = true;
       SetState(CupcakeState.Exploring);
       Debug.Log("Cupcake exploring!");
    }
    
    //eating 
    void StartEating()
    {
        SetState(CupcakeState.Eating);
        stateTimer = 2f;
        Debug.Log("Cupcake is eating");
    }
    
    //sleeping
    void StartSleeping()
    {
        SetState(CupcakeState.Sleeping);
        stateTimer = 3f;
        Debug.Log("Cupcake is sleeping");
    }
    
    //exploring again
    void StartExploring()
    {
        SetState(CupcakeState.Exploring);
        Debug.Log("Cupcake wokeup");
        ChooseNewSpot();
    }
    
    //change state
    void SetState(CupcakeState newState)
    {
        currentState = newState;
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (currentState == CupcakeState.Exploring)
        {
            animator.SetInteger("CupcakeState", 0);
        }
        else if (currentState == CupcakeState.Eating)
        {
            animator.SetInteger("CupcakeState", 1);
        }
        else if (currentState == CupcakeState.Sleeping)
        {
            animator.SetInteger("CupcakeState", 2);
        }
    }
}
