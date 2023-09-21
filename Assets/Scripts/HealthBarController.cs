using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Slider slider;

    public void SetHealth(int currentHP, int maxHP)
    {
        slider.maxValue = maxHP;
        slider.value = currentHP;
    }
}
