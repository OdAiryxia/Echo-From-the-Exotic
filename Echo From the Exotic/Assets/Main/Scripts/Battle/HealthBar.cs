using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private TextMeshPro unitNametag;
    [SerializeField] private TextMeshPro unitHealth;

    public Slider fillHealthSlider;
    public Slider easeHealthSlider;

    private float maxHealth = 100f;
    private float lerpSpeed = 0.025f;

    [SerializeField] private bool isPlayer;

    void Awake()
    {
        if (isPlayer)
        {
            unitNametag.text = DataContainer.instance.playerName;
        }
        else
        {
            unitNametag.text = unit.unitName;
        }

        maxHealth = unit.health;
        fillHealthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
        unitHealth.text = unit.health + "/" + maxHealth;
    }

    void Update()
    {
        if (fillHealthSlider.value != unit.health)
        {
            fillHealthSlider.value = unit.health;
        }

        if (fillHealthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, unit.health, lerpSpeed);
        }

        unitHealth.text = unit.health + "/" + maxHealth;
    }
    void LateUpdate()
    {
        transform.LookAt(BattleManager.instance.mainCamera.transform);
        transform.RotateAround(transform.position, transform.up, 180f);
    }
}
