using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image image;
    public void UpdateHealth(float fillAmount)
    {
        image.fillAmount = fillAmount;
    }
}
