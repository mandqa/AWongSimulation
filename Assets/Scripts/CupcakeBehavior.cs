using UnityEngine;

public class CupcakeBehavior : MonoBehaviour
{
    public Animator animator;
    public GameManager myManager;

    float fullnessVal = 3f;
    float needsTime;

    public float needsTimeReset;
    public float needsTimeStep;

    GameObject targetFood;
    
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
        needsTime = needsTimeReset;
        SetState(CupcakeState.Exploring);
        ChooseNewSpot();
    }

    // Update is called once per frame
    void Update()
    {
        //exploring
        if (currentState == CupcakeState.Exploring)
        {
            needsTime -= needsTimeStep * Time.deltaTime;
            if (needsTime <= 0)
            {
                IncrementNeeds();
            }
        }
        if (currentState == CupcakeState.Exploring)
        {
        if (moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                //reached a new spot
                if (Vector3.Distance(transform.position, targetPos) < 0.1f)
                {
                    moving = false;
                    //once target food found- go onto next state
                    if (targetFood != null)
                    {
                        myManager.allFood.Remove(targetFood);
                        Destroy(targetFood);
                        targetFood = null;
                        fullnessVal = 3f;
                        needsTime = needsTimeReset;
                        StartEating();
                    }
                    else
                    {
                        //wait then choose another spot
                        stateTimer = 1f;
                    }
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

        void IncrementNeeds()
        {
            fullnessVal -= 1;
            needsTime = needsTimeReset;
            Debug.Log("Cupcake fullness:" + fullnessVal);
            if (fullnessVal <= 0)
            {
                FindFood();
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
        if (fullnessVal <= 0)
        {

            if (FindFood())
            {
                return;
            }
        }

        float randomX = Random.Range(-wanderDistance, wanderDistance);
       float randomY = Random.Range(-wanderDistance, wanderDistance);

       targetPos = transform.position + new Vector3(randomX, randomY,0);
       moving = true;
       //SetState(CupcakeState.Exploring);
       Debug.Log("Cupcake exploring!");
    }

    bool FindFood()
    {
        float closestDistance = 2000f;
        GameObject closestFood = null;

        foreach (GameObject food in myManager.allFood)
        {
            FoodScript foodScript = food.GetComponent<FoodScript>();
            if (foodScript != null && foodScript.foodType == FoodScript.FoodType.Cupcake)
            {
                float distance = Vector3.Distance(transform.position, food.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFood = food;
                }
            }
            
        }

        if (closestFood != null)
        {
            targetFood = closestFood;
            targetPos = closestFood.transform.position;
            moving = true;
            
            Debug.Log("Cupcake found");
            return true;
        }

        return false;
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
