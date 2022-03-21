using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static TextMeshProUGUI healthText;

    void Awake()
    {
        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
        healthText.text = "Health : " + GameObject.Find("RolloPlayer").GetComponent<Health>().health.ToString();

    }
}
