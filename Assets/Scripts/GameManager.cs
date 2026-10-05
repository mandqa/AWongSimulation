using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    InputAction leftMouse;

    public GameObject foodObj;

    public List<GameObject> allFood = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftMouse = InputSystem.actions.FindAction("MouseClick");

    }

    // Update is called once per frame
    void Update()
    {
        if (leftMouse.WasReleasedThisFrame())
        {
            //create a food
            MakeFood();

        }
    }

    void MakeFood()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        newPos.z = 0;
        allFood.Add(Instantiate(foodObj, newPos, Quaternion.identity));
        Debug.Log("Food added" + allFood.Count);
    }
}