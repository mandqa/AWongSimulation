using UnityEngine;

public class CupcakeBehavior : MonoBehaviour
{
    public Animator animator;
    public GameManager myManager;

    public float fullnessVal = 3f;
    public float startingFullness = 3f;
    float needsTime;

    public float needsTimeReset = 5f;
    public float needsTimeStep = 1f;

    GameObject targetFood;
    
    //behavior states
    public enum CupcakeState
    {
        Exploring, 
        Eating,
        Sleeping,
        Exhausted
    }
    
    public CupcakeState currentState;
    
    Vector3 targetPos;
    Vector3 lastPosition;
        float stuckTimer = 0f;
    bool moving;
    
    //how far cupcake can wander off
    public float wanderDistance = 3f;
    
    //how fast cupcake moves
    public float moveSpeed = 1.5f;
    
    //state timer
    float stateTimer;
    
    //exhaustion and revival vars
    public float hpVal = 10f;
    public float maxHp = 10f;

    public float healthLossInterval = 2f;

    public float healthLossAmount = 2f;

    float healthLossTimer;

    bool isExhausted = false;

    SpriteRenderer spriteRenderer;

    Color normalColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        if (spriteRenderer != null)
        {
            normalColor = spriteRenderer.color;
        }

        fullnessVal = startingFullness;
        needsTime = needsTimeReset;
        healthLossTimer = healthLossInterval;

        lastPosition = transform.position;
        
        SetState(CupcakeState.Exploring);
        ChooseNewSpot();
    }

    // Update is called once per frame
    void Update()
    {
        if (isExhausted)
        {
            healthLossTimer -= Time.deltaTime;

            if (healthLossTimer <= 0f)
            {
                hpVal = Mathf.Max(0f, hpVal - healthLossAmount);
                healthLossTimer = healthLossInterval;
            }

            return;
        }
        //exploring
        if (currentState == CupcakeState.Exploring)
        {
            needsTime -= needsTimeStep * Time.deltaTime;
            if (needsTime <= 0)
            {
                IncrementNeeds();
                if (isExhausted)
                    return;
            }
      
        if (moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
                {
                    stuckTimer += Time.deltaTime;
                }
                else
                {
                    stuckTimer = 0f;
                }

                lastPosition = transform.position;

                if (stuckTimer >= 0.5f)
                {
                    stuckTimer = 0f;
                    moving = false;
                    targetFood = null;
                    ChooseNewSpot();
                    return;
                }
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
                        fullnessVal = startingFullness;
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
    void IncrementNeeds()
    {
        needsTime = needsTimeReset;
       
        //fullness decreases over time
        fullnessVal -= 1;
        fullnessVal = Mathf.Max(0f, fullnessVal);
        
        Debug.Log("Cupcake fullness:" + fullnessVal);
        if (fullnessVal <= 0f)
        {
            //look for food before being exhausted
            if (!FindFood())
            {
                BecomeExhausted();
            }
                    
        }

    }
    //choose random spot
    void ChooseNewSpot()
    {
        if (fullnessVal <= 1f)
        {

            if (FindFood())
            {
                return;
            }

            BecomeExhausted();
            return;
        }
        
        float randomX = Random.Range(-wanderDistance, wanderDistance);
       float randomY = Random.Range(-wanderDistance, wanderDistance);

       targetPos = transform.position + new Vector3(randomX, randomY,0);
       targetFood = null;
       moving = true;
      
       Debug.Log("Cupcake exploring!");
    }

    bool FindFood()
    {
        if (myManager == null || myManager.allFood == null)
            return false;
        float closestDistance = 2000f;
        GameObject closestFood = null;

        foreach (GameObject food in myManager.allFood)
        {
            if (food == null)
                continue;
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
        moving = false;
        SetState(CupcakeState.Eating);
        stateTimer = 2f;
        Debug.Log("Cupcake is eating");
    }
    
    //sleeping
    void StartSleeping()
    {
        moving = false;
        SetState(CupcakeState.Sleeping);
        stateTimer = 3f;
        Debug.Log("Cupcake is sleeping");
    }
    
    //exploring again
    void StartExploring()
    {
        if (myManager != null)
        {
            myManager.EarnCoins(10);
        }

        SetState(CupcakeState.Exploring);
        needsTime = needsTimeReset;
        Debug.Log("Cupcake wokeup");
        ChooseNewSpot();
    }
    
    //become exhausted
    void BecomeExhausted()
    {
        fullnessVal = 0f;
        hpVal = maxHp;
        isExhausted = true;
        moving = false;
        targetFood = null;
        
        SetState(CupcakeState.Exhausted);
    }
    
    //revive cupcake
    public void ReviveCupcake()
    {
        if (currentState != CupcakeState.Exhausted)
            return;
        fullnessVal = startingFullness;
        hpVal = maxHp;
        needsTime = needsTimeReset;
        healthLossTimer = healthLossInterval;
        stateTimer = 0f;

        isExhausted = false;
        moving = false;
        targetFood = null;
        
        SetState(CupcakeState.Exploring);
        if (animator != null)
        {
            animator.Play("CupcakeAnimator", 0, 0f);
            animator.SetInteger("CupcakeState", 0);
        }
        ChooseNewSpot();
    }
    //change state
    void SetState(CupcakeState newState)
    {
        currentState = newState;
        if (spriteRenderer != null)
        {
            if (currentState == CupcakeState.Exhausted)
            {
                spriteRenderer.color = Color.gray;
            }
            else
            {
                spriteRenderer.color = normalColor;
            }
        }
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        if (animator == null)
            return;
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
        else if (currentState == CupcakeState.Exhausted)
        {
            animator.SetInteger("CupcakeState", 3);
        }
    }
}
