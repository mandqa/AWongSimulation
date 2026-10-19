using UnityEngine;

public class BreadBehavior : MonoBehaviour
{
    public Animator animator;
    public float fullnessVal = 3f;
    
    float stateTimer;
    float lifeTimer;
    public float lifespan = 60f;
    public float hpVal= 10f;
    public float maxHp = 10f;
    bool isExhausted = false;
    
    float needsTime;
    float wanderTimer;
    public float needsTimeReset;
    public float needsTimeStep;
    public float startingFullness = 3f;
    
    public GameManager myManager;
    
    //bread behavior state
    public enum BreadState
    {
        Idle,
        Kneading,
        Rising,
        Baking,
        Exhausted
    }
    Vector3 homePosition;
    Vector3 targetPos;
    bool moving;
    //movement
    public float moveSpeed = 0.7f;
    public float wanderDistance = 2f;
    public float wanderWaitTime = 1f;
    GameObject targetFood;

    public BreadState currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        needsTime = needsTimeReset;
        fullnessVal = startingFullness;
        homePosition = transform.position;
        lifeTimer = lifespan;
        
        SetState(BreadState.Idle);
        //ChooseNewSpot();
    }

    // Update is called once per frame
    void Update()
    {
        //stop normal behavior after exhaustion
        if (isExhausted)
        {
            return;
        }

        //count down bread total lifespan
        lifeTimer -= Time.deltaTime;
        //update health bar based on remainin lifepsan
        hpVal = Mathf.Clamp((lifeTimer/lifespan)*maxHp, 0f, maxHp);
        if (lifeTimer <= 0f)
        {
            BecomeExhausted();
            return;
        }

        if (currentState == BreadState.Idle && targetFood == null)
        {
            FindFood();
        }
        //moving towards dough
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            //reached dough
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                moving = false;
                wanderTimer = wanderWaitTime;

                //consume dough when reached
                if (targetFood != null)
                {
                    myManager.allFood.Remove(targetFood);
                    Destroy(targetFood);
                    targetFood = null;

                    fullnessVal = startingFullness;
                    needsTime = needsTimeReset;
                    SetState(BreadState.Kneading);
                    stateTimer = 3f;
                }
            }
        }
        else if (currentState == BreadState.Idle || currentState == BreadState.Kneading)
        {
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0f)
            {
                ChooseNewSpot();
            }
        }
        
        //needs decrease while kneading
        if (currentState == BreadState.Kneading)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                Debug.Log("swithcing to rising");
                StartRising();
            }
            needsTime -= needsTimeStep * Time.deltaTime;
            if (needsTime <= 0f)
            {
                IncrementalNeeds();
            }
        }
        //rising
        if (currentState == BreadState.Rising)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                StartBaking();
            }
        }
        
        //baking
        else if (currentState == BreadState.Baking)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                FinishBaking();
            }
        }
    }

    void IncrementalNeeds()
    {
        fullnessVal -= 1;
        needsTime = needsTimeReset;
        Debug.Log("Bread fullness" + fullnessVal);

        if (fullnessVal <= 0)
        {
            if (!FindFood())
            {
                SetState(BreadState.Idle);
            }
        }
    }

    bool FindFood()
    {
        if (myManager == null)
        {
            return false;
        }
        float dist = 2000f;
        GameObject closestFood = null;

        foreach (GameObject food in myManager.allFood)
        {
            if (food == null)
            {
                continue;
            }
            FoodScript foodScript = food.GetComponent<FoodScript>();

            if (foodScript != null && foodScript.foodType == FoodScript.FoodType.Bread)
            {
                float distance = Vector3.Distance(transform.position, food.transform.position);

                if (distance < dist)
                {
                    dist = distance;
                    closestFood = food;
                }
            }
        }

        if (closestFood != null)
        {
            targetFood = closestFood;
            targetPos = closestFood.transform.position;
            moving = true;
            return true;
        }

        return false;
    }

    //wander around slowly
    void ChooseNewSpot()
    {
        //look for dough if bread needs some
        if (fullnessVal <= 0)
        {
            if (FindFood())
            {
                return;
            }
        }

        float randomX = Random.Range(-wanderDistance, wanderDistance);
        float randomY = Random.Range(-wanderDistance, wanderDistance);

        targetPos = homePosition + new Vector3(randomX, randomY, 0);
        moving = true;
    }   
    
    //rising state
    void StartRising()
    {
        SetState(BreadState.Rising);
        stateTimer = 4f;
        moving = false;
    }

    //baking state
    void StartBaking()
    {
        SetState(BreadState.Baking);
        stateTimer = 3f;
        moving = false;
    }

    void FinishBaking()
    {
        if (myManager != null)
        {
            myManager.EarnCoins(100);
        }

        SetState(BreadState.Idle);

        moving = false;
        targetFood = null;
    }

    void SetState(BreadState newState)
    {
        currentState = newState;
        UpdateAnimation();
    }
    
    //update anim
    void UpdateAnimation()
    {
        if (animator == null)
        {
            return;
        }

        if (currentState == BreadState.Idle)
        {
            animator.SetInteger("BreadState", 0);
        }
       else if (currentState == BreadState.Kneading)
        {
            animator.SetInteger("BreadState", 1);
        }
        else if (currentState == BreadState.Rising)
        {
            animator.SetInteger("BreadState", 2);
        }
        else if (currentState == BreadState.Baking)
        {
            animator.SetInteger("BreadState", 3);
        }
        else if (currentState == BreadState.Exhausted)
        {
            animator.SetInteger("Breadstate", 4);
        }
    }

    void BecomeExhausted()
    {
        lifeTimer = 0f;
        hpVal = 0f;
        isExhausted = true;
        moving = false;
        targetFood = null;
        SetState(BreadState.Exhausted);
    }

    public void ReviveBread()
    {
        if (!isExhausted)
            return;
        
        lifeTimer = lifespan;
        hpVal = maxHp;
        fullnessVal = startingFullness;
        needsTime = needsTimeReset;
        
        isExhausted = false;
        moving = false;
        targetFood = null;
        stateTimer = 0f;
        SetState(BreadState.Idle);
    }
}
