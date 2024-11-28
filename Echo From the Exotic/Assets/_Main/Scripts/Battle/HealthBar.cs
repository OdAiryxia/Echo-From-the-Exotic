using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private TextMeshPro unitNametag;
    [SerializeField] private TextMeshPro unitHealth;

    public Slider healthSlider;
    public Slider easeHealthSlider;

    private float maxHealth = 100f;
    private float lerpSpeed = 0.025f;

    private void Awake()
    {
        unitNametag.text = unit.unitName;

        maxHealth = unit.health;
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
        unitHealth.text = unit.health + "/" + maxHealth;
    }

    private void Update()
    {
        if (healthSlider.value != unit.health)
        {
            healthSlider.value = unit.health;
        }

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, unit.health, lerpSpeed);
        }

        unitHealth.text = unit.health + "/" + maxHealth;
    }
}
