using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    InputAction leftMouse;

    public GameObject cookieFood;
    public GameObject cupcakeFood;
    public GameObject breadFood;

    public int coins = 0;
    public TMPro.TMP_Text coinText;

    public Collider2D foodPlacementArea;

    public List<GameObject> allFood = new List<GameObject>();
    
    GameObject selectedFood;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftMouse = InputSystem.actions.FindAction("MouseClick");
        
        //start w cookie food selected
        selectedFood = cookieFood;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftMouse.WasReleasedThisFrame())
        {
            Debug.Log("MouseClick detected");
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            //create a food
            MakeFood();

        }
    }

    public void EarnCoins(int amount)
    {
        coins += amount;
        if (coinText != null)
        {
            coinText.text = " " + coins;
        }
        Debug.Log("Total coins" + coins);
    }
    public void SelectedCookieFood()
    {
        selectedFood = cookieFood;
    }

    public void SelectedCupcakeFood()
    {
        selectedFood = cupcakeFood;
    }

    public void SelectedBreadFood()
    {
        selectedFood = breadFood;
    }
    void MakeFood()
    {
        
        if (foodPlacementArea == null)
        {
            return;
        }
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -Camera.main.transform.position.z));
        Vector2 foodPos = new Vector2(mousePos.x, mousePos.y);
        
        if (!foodPlacementArea.OverlapPoint(foodPos))
        {
            return;
        }
        mousePos.z = 0f;
        GameObject newFood = Instantiate(selectedFood, mousePos, Quaternion.identity);
        allFood.Add(newFood);
        Debug.Log("Food added" + allFood.Count);
        
    }
    
    public void ReviveCookieWithCoins(CookieBehavior cookie)
    {
        if (cookie == null)
            return;

        if (cookie.currentState != CookieBehavior.CookieState.Exhausted)
            return;

        if (coins >= 50)
        {
            coins -= 50;

            if (coinText != null)
            {
                coinText.text = " " + coins;
                cookie.ReviveCookie();
            }
        }
    }
    
    public void ReviveBreadWithCoins(BreadBehavior bread)
    {
        if (bread == null)
            return;

        if (bread.currentState != BreadBehavior.BreadState.Exhausted)
            return;

        if (coins >= 50)
        {
            coins -= 50;

            if (coinText != null)
            {
                coinText.text = " " + coins;
                bread.ReviveBread();
            }
            
        }
    }

    public void ReviveCupcakeWithCoins(CupcakeBehavior cupcake)
    {
        if (cupcake == null)
            return;
        if (cupcake.currentState != CupcakeBehavior.CupcakeState.Exhausted)
            return;
        if (coins >= 50)
        {
            coins -= 50;
            if (coinText != null)
            {
                coinText.text = " " + coins;
            }

            cupcake.ReviveCupcake();
        }
        
    }
}