using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image image;
    public void UpdateHealth(float fillAmount)
    {
        //Le fill amount est entre 0 et 1
        image.fillAmount = fillAmount;
    }
}
