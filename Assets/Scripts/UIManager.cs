using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public CookieBehavior cookieObj;
    public BreadBehavior breadObj;
    public CupcakeBehavior cupcakeObj;

    public Slider hungerBar;
    public Slider hpBar;

    public TMP_Text stateText;

    public bool showBread = false;

    public bool showCupcake = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (hpBar != null)
        {
            hpBar.minValue = 0;
            hpBar.maxValue = 10;
        }

        UpdateSliders();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSliders();
    }

    public void UpdateSliders()
    {
        if (showBread)
        {
            if (breadObj == null)
                return;
            if (hungerBar != null)
            {
                hungerBar.minValue = 0;
                hungerBar.maxValue = breadObj.startingFullness;
                hungerBar.value = breadObj.fullnessVal;
            }

            if (hpBar != null)
            {
                hpBar.minValue = 0;
                hpBar.maxValue = breadObj.maxHp;
                hpBar.value = breadObj.hpVal;
            }

            if (stateText != null)
            {
                stateText.text = breadObj.currentState.ToString();
            }
        }
        else if (showCupcake)
        {

            if (cupcakeObj == null)
                return;
            if (hungerBar != null)
            {
                hungerBar.minValue = 0;
                hungerBar.maxValue = cupcakeObj.startingFullness;
                hungerBar.value = cupcakeObj.fullnessVal;
            }

            if (hpBar != null)
            {
                hpBar.value = cupcakeObj.hpVal;
            }

            if (stateText != null)
            {
                stateText.text = cupcakeObj.currentState.ToString();
            }
        }
        else
        {
            if (cookieObj == null)
                return;
            if (hungerBar != null)
            {
                hungerBar.minValue = 0;
                hungerBar.maxValue = 5;
                hungerBar.value = cookieObj.fullnessVal;
            }

            if (hpBar != null)
            {
                hpBar.value = cookieObj.hpVal;
            }

            if (stateText != null)
            {
                stateText.text = cookieObj.currentState.ToString();
            }
            else if (showBread)
            {
                if (breadObj == null)
                    return;
                if (hungerBar != null)
                {
                    hungerBar.minValue = 0;
                    hungerBar.minValue = breadObj.startingFullness;
                    hungerBar.value = breadObj.fullnessVal;
                }

                if (hpBar != null)
                {
                    hpBar.value = breadObj.hpVal;
                }

                if (stateText != null)
                {
                    stateText.text = breadObj.currentState.ToString();
                }
            }
            else
            {
                if (cookieObj == null)
                    return;
                if (hungerBar != null)
                {
                    hungerBar.minValue = 0;
                    hungerBar.maxValue = 5;
                    hungerBar.value = cookieObj.fullnessVal;
                }

                if (hpBar != null)
                {
                    hpBar.value = cookieObj.hpVal;
                }

                if (stateText != null)
                {
                    stateText.text = cookieObj.currentState.ToString();
                }
            }
        }
    }
}
