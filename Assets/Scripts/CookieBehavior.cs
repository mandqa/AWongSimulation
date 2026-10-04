using UnityEngine;

public class CookieBehavior : MonoBehaviour
{
    float fullnessVal = 5f;

    float needsTime;

    public float needsTimeReset;

    public float needsTimeStep;

    public GameManager myManager;

    Vector3 targetPos;

    bool moving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        needsTime = needsTimeReset;  
    }

    // Update is called once per frame
    void Update()
    {
        needsTime -= needsTimeStep * Time.deltaTime;
        if (needsTime < 0)
        {
            IncrementNeeds();
        }
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 2f * Time.deltaTime);
        }
    }

    void IncrementNeeds()
    {
        fullnessVal -= 1;
        needsTime = needsTimeReset;
        Debug.Log(fullnessVal);
        if (fullnessVal <= 0)
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
            if(Vector3.Distance(transform.position,food.transform.position)<dist)
            {
                dist = Vector3.Distance(transform.position, food.transform.position);
                closestFood = food;
            }
        }
        //go to the food
        if (closestFood != null)
        {
            targetPos = closestFood.transform.position;
            moving = true;
        }
        
    }
}