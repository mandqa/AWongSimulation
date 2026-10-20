using UnityEngine;

public class CookieBehavior : MonoBehaviour
{
    public Animator animator;
    public float fullnessVal = 5f;
    public float hpVal= 10f;
    public float healthLossRate = 1f;
    public float healthLossAmount = 2f;
    public float healthLossInterval = 2f;
    float healthLossTimer;
    float stateTimer;
    float searchTimer = 0f;
    float needsTime;

    public float needsTimeReset;

    public float needsTimeStep;
    float reviveIdleTimer = 0f;

    public GameManager myManager;

    Vector3 targetPos;

    bool moving;
    
    SpriteRenderer spriteRenderer;
    Color normalColor;
    
    //behavior states 
    public enum CookieState
    {
        Searching, 
        Baking,
        Resting,
        Exhausted
    }
    
    public CookieState currentState;
    GameObject targetFood;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //orginal sprite color
        if (spriteRenderer != null)
        {
            normalColor = spriteRenderer.color;
        }
        needsTime = needsTimeReset;
        moving = false;
        searchTimer = 0f;
        //start seraching state
        SetState(CookieState.Searching);
        FindFood();
    }

    // Update is called once per frame
    void Update()
    {
        if (reviveIdleTimer > 0f)
        {
            reviveIdleTimer -= Time.deltaTime;
            if (animator != null)
            {
                animator.SetInteger("CookieState", 0);
            }
        }
        //exhausted-health decreases until reaches 0
        if(currentState == CookieState.Exhausted)
        {
            healthLossTimer -= Time.deltaTime;
            
           
            if (healthLossTimer <=0f)
            {
                hpVal -= healthLossAmount;
                hpVal = Mathf.Max(0f, hpVal);
                healthLossTimer = healthLossInterval;
            }

            //stops normal behavior when exhausted
            return;
        }
        //searching - hunger decreases over time
        if (currentState == CookieState.Searching)
        {
            needsTime -= needsTimeStep * Time.deltaTime;
           //removes hunger point
            if (needsTime < 0)
            {
                IncrementNeeds();
            }
        }

        //move towards food
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 2f * Time.deltaTime);


            //reached food
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                moving = false;
                
                //eat the food
                if (targetFood != null)
                {
                    myManager.allFood.Remove(targetFood);
                    //gets rid of food that sprite goes towards
                    Destroy(targetFood);
                    targetFood = null;
                }
                //nextstate
                StartBaking();
            }
        }

        //look for new spawned food while idled
        if (currentState == CookieState.Searching && !moving)
        {
            searchTimer -= Time.deltaTime;
            //checks for food every half sec
            if (searchTimer <= 0f)
            {
                FindFood();
                searchTimer = 0.5f;
            }
        }
        //baking timer
        if (currentState == CookieState.Baking)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                StartResting();
            }
        }

        //resting timer
        if (currentState == CookieState.Resting)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                StartSearchingAgain();
            }
        }

        UpdateAnimation();

    }

    //fullness
    void IncrementNeeds()
    {
        fullnessVal -= 1;
        needsTime = needsTimeReset;
        Debug.Log("Cookie fullness:" + fullnessVal);
        //exhaust cookie when hunger is zero
        if (fullnessVal <= 0 && currentState != CookieState.Exhausted)
        {
            fullnessVal = 0;
            moving = false;
            SetState(CookieState.Exhausted);
        }
    }
    
    //finds closest food avaiable 
    void FindFood()
    {
        moving = false;
        targetFood = null;
        //stop if food list is missing or gamemanger
        if (myManager == null || myManager.allFood == null)
            return;
        float dist = 2000f;
        GameObject closestFood = null;
        foreach(GameObject food in myManager.allFood)
        {
            if (food == null)
                continue;
            FoodScript foodScript = food.GetComponent<FoodScript>();
            if (foodScript != null && foodScript.foodType == FoodScript.FoodType.Cookie)
            {
                float distance = Vector3.Distance(transform.position, food.transform.position);

                if (distance < dist)
                {
                    dist = distance;
                    closestFood = food;
                }
            }
        }
        //go to the food - found it
        if (closestFood != null)
        {
            targetFood = closestFood;
            targetPos = closestFood.transform.position;
            moving = true;
        }
        

    }

    //baking process
    void StartBaking()
    {
        SetState(CookieState.Baking);
        stateTimer = 2f;
        Debug.Log("Cookie is baking");
       
    }

    //restinf
    void StartResting()
    {
        if (myManager != null)
        {
            myManager.EarnCoins(15);
        }

        SetState(CookieState.Resting);
        stateTimer = 3f;
        Debug.Log("Cookie is resting");
        
    }

    //search for food again
    void StartSearchingAgain()
    {
        fullnessVal = 5f;
        needsTime = needsTimeReset;
        searchTimer = 0f;
        SetState(CookieState.Searching); 
        Debug.Log("Cookie is searching");
        FindFood();
    }

    //change its state
    void SetState(CookieState newState)
    {
        currentState = newState;
        //make cookie look gray when exhausted
        if (spriteRenderer != null)
        {
            if (currentState == CookieState.Exhausted)
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
    //updates animator based on behavior state
    void UpdateAnimation()
    {
        if (animator == null)
            return;
        //searching uses idle or walk animations
        if (currentState == CookieState.Searching)
        {
            if (moving)
            {
                animator.SetInteger("CookieState", 1); //walking state
            }
            else
            {
                animator.SetInteger("CookieState", 0); //idle
            }
        }
        else if (currentState == CookieState.Baking)
        {
            animator.SetInteger("CookieState", 2);
        }
        else if (currentState == CookieState.Resting)
        {
            animator.SetInteger("CookieState", 3);
        }
        else if (currentState == CookieState.Exhausted)
        {
            animator.SetInteger("CookieState", 4);
        }
    }

    public void ReviveCookie()
    {
        //checks if cookie is exhausted
        if (currentState != CookieState.Exhausted)
            return;
        //restore health and hunger
        hpVal = 10f;
        fullnessVal = 5f;
        
        //reset timers
        healthLossTimer = healthLossInterval;
        needsTime = needsTimeReset;
        searchTimer = 0f;
        stateTimer = 0f;

        //stop previous movement
        moving = false;
        targetFood = null;

        SetState(CookieState.Searching);

        reviveIdleTimer = 1f;
        //force idle animation
        if (animator != null)
        {
            animator.Play("CookieIdle", 0, 0f);
            animator.SetInteger("CookieState", 0);
        }
        
        //look for food again
        FindFood();
    }
}