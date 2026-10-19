using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public CookieBehavior cookieObj;
    public BreadBehavior breadObj;

    public Slider hungerBar;
    public Slider hpBar;

    public TMP_Text stateText;

    public bool showBread = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cookieObj == null)
        {
            cookieObj = GameObject.Find("Cookie").GetComponent<CookieBehavior>();
        }
        if (hpBar != null)
        {
            hpBar.minValue = 0;
            hpBar.minValue = showBread ? 3: 5;
        }

        if (hungerBar)
        {
            hungerBar.minValue = 0;
            hungerBar.maxValue = 5;
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
                hungerBar.value = cookieObj.fullnessVal;
            }

            if (hpBar != null)
            {
                hpBar.value = cookieObj.hpVal;
            }

            if (stateText != null)
            {
                if (cookieObj.currentState == CookieBehavior.CookieState.Searching)
                {
                    stateText.text = "Searching";
                }
                else if (cookieObj.currentState == CookieBehavior.CookieState.Baking)
                {
                    stateText.text = "Baking";
                }
                else if (cookieObj.currentState == CookieBehavior.CookieState.Resting)
                {
                    stateText.text = "Resting";
                }

                else if (cookieObj.currentState == CookieBehavior.CookieState.Exhausted)
                {
                    stateText.text = "Exhausted";
                }
            }
        }
    }
}
