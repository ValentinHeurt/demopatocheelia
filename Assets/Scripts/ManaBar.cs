using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] Image image;
    public void UpdateMana(float fillAmount)
    {
        //FillAmount est entre 0 et 1
        image.fillAmount = fillAmount;
    }
}
