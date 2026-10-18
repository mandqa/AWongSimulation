using UnityEngine;

public class CookieBehavior : MonoBehaviour
{
    public Animator animator;
    float fullnessVal = 3f;
    float stateTimer;
    float searchTimer =0f;

    float needsTime;

    public float needsTimeReset;

    public float needsTimeStep;

    public GameManager myManager;

    Vector3 targetPos;

    bool moving;
    
    //behavior states 
    enum CookieState
    {
        Searching, 
        Baking,
        Resting,
        Exhausted
    }
    
    CookieState currentState;
    GameObject targetFood;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if (currentState == CookieState.Searching)
        {
            needsTime -= needsTimeStep * Time.deltaTime;
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
        if (fullnessVal <= 0 && currentState == CookieState.Searching)
        {
            FindFood();
        }
    }
    
    void FindFood()
    {
        moving = false;
        targetFood = null;
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
            myManager.EarnCoins(5);
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
        UpdateAnimation();
    }
    //updates animator based on behavior state
    void UpdateAnimation()
    {
        if (animator == null)
            return;
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
}