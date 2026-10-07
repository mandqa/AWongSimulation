using UnityEngine;

public class CookieBehavior : MonoBehaviour
{
    public Animator animator;
    float fullnessVal = 3f;
    float stateTimer;

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
        Resting
    }
    
    CookieState currentState;
    GameObject targetFood;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        needsTime = needsTimeReset;  
        
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
        float dist = 2000f;
        GameObject closestFood = null;
        foreach(GameObject food in myManager.allFood)
        {
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
        stateTimer = 3f;
        Debug.Log("Cookie is baking");
       
    }

    //restinf
    void StartResting()
    {
        SetState(CookieState.Resting);
        stateTimer = 3f;
        Debug.Log("Cookie is resting");
        
    }

    //search for food again
    void StartSearchingAgain()
    {
        fullnessVal = 5f;
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
        if (currentState == CookieState.Searching)
        {
            animator.SetInteger("CookieState", 0);
        }
        else if (currentState == CookieState.Baking)
        {
            animator.SetInteger("CookieState", 1);
        }
        else if (currentState == CookieState.Resting)
        {
            animator.SetInteger("CookieState", 2);
        }
    }
}