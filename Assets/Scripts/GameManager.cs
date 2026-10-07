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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            //create a food
            MakeFood();

        }
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
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        newPos.z = 0;
        GameObject newFood = Instantiate(selectedFood, newPos, Quaternion.identity);
        allFood.Add(newFood);
        Debug.Log("Food added" + allFood.Count);
    }
}