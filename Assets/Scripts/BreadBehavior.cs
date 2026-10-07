using UnityEngine;

public class BreadBehavior : MonoBehaviour
{
    public Animator animator;
    float fullnessVal = 3f;
    float stateTimer;
    float needsTime;
    public float needsTimeReset;
    public float needsTimeStep;
    public GameManager myManager;
    
    //bread behavior state
    enum BreadState
    {
        Kneading,
        Rising,
        Baking
    }
    Vector3 targetPos;
    bool moving;
    //movement
    public float moveSpeed = 0.7f;
    public float wanderDistance = 2f;
   
    GameObject targetFood;

    BreadState currentState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        needsTime = needsTimeReset;
        SetState(BreadState.Kneading);
        ChooseNewSpot();
    }

    // Update is called once per frame
    void Update()
    {
        //bread slowly gets hungry while kneading
        if (currentState == BreadState.Kneading)
        {
            needsTime -= needsTimeStep * Time.deltaTime;
            if (needsTime <= 0)
            {
                IncrementalNeeds();
            }
        }
        //moving towards dough
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            //reached dough
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                moving = false;
                if (targetFood != null)
                {
                    myManager.allFood.Remove(targetFood);
                    Destroy(targetFood);
                    targetFood = null;
                    
                    //restoru fullness
                    fullnessVal = 3f;
                    needsTime = needsTimeReset;
                    StartRising();
                }
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
        if (currentState == BreadState.Baking)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                StartKneadingAgain();
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
            FindFood();
        }
    }

    bool FindFood()
    {
        float dist = 2000f;
        GameObject closestFood = null;

        foreach (GameObject food in myManager.allFood)
        {
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
        if (fullnessVal <= 0)
        {
            if (FindFood())
            {
                return;
            }
        }

        float randomX = Random.Range(-wanderDistance, wanderDistance);
        float randomY = Random.Range(-wanderDistance, wanderDistance);

        targetPos = transform.position + new Vector3(randomX, randomY, 0);
        moving = true;
    }   
    
    //rising state
    void StartRising()
    {
        SetState(BreadState.Rising);
        stateTimer = 4f;
    }

    //baking state
    void StartBaking()
    {
        SetState(BreadState.Baking);
        stateTimer = 3f;
    }

    //knead again
    void StartKneadingAgain()
    {
        SetState(BreadState.Kneading);
        ChooseNewSpot();
    }

    void SetState(BreadState newState)
    {
        currentState = newState;
        UpdateAnimation();
    }
    
    //update anim
    void UpdateAnimation()
    {
        if (currentState == BreadState.Kneading)
        {
            animator.SetInteger("BreadState", 0);
        }
        else if (currentState == BreadState.Rising)
        {
            animator.SetInteger("BreadState", 1);
        }
        else if (currentState == BreadState.Baking)
        {
            animator.SetInteger("BreadState", 2);
        }
    }
}
