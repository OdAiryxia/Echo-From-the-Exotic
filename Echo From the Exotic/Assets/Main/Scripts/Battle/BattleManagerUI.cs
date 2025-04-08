using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class BattleManagerUI : MonoBehaviour
{
    public static BattleManagerUI instance;

    private Unit currentUnit;

    [SerializeField] private GameObject buttonCanvas;
    [Space(20)]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button ultimateButton;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI attackButtonText;
    [SerializeField] private TextMeshProUGUI skillButtonText;
    [SerializeField] private TextMeshProUGUI ultimateButtonText;
    [Space(10)]
    [SerializeField] private List<Image> skillPointImages;

    [Space(20)]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        attackButton.onClick.AddListener(OnAttackButtonClick);
        skillButton.onClick.AddListener(OnSkillButtonClick);
        ultimateButton.onClick.AddListener(OnUltimateButtonClick);
    }

    public void ShowUnitButtonPanel(Unit unit)
    {
        currentUnit = unit;

        buttonCanvas.gameObject.SetActive(true);

        attackButtonText.text = unit.attackName;
        skillButtonText.text = unit.skillName;
        ultimateButtonText.text = unit.ultimateName;

        skillButton.interactable = unit.remainingSkillUses > 0;
        UpdateSkillPoint(unit.remainingSkillUses, unit.maxSkillUses);

        ultimateButton.interactable = unit.ultimateEnergy >= unit.maxUltimateEnergy;
    }

    void UpdateSkillPoint(int remainingUses, int maxUses)
    {
        for (int i = 0; i < skillPointImages.Count; i++)
        {
            if (i < maxUses)
            {
                skillPointImages[i].gameObject.SetActive(true);
                skillPointImages[i].color = i < remainingUses ? Color.red : Color.gray;
            }
            else
            {
                skillPointImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnAttackButtonClick()
    {
        if (currentUnit != null)
        {
            currentUnit.PerformAttack();
            buttonCanvas.gameObject.SetActive(false);
        }
    }

    public void OnSkillButtonClick()
    {
        if (currentUnit != null)
        {
            currentUnit.PerformSkill();
            buttonCanvas.gameObject.SetActive(false);
        }
    }

    public void OnUltimateButtonClick()
    {
        if (currentUnit != null)
        {
            currentUnit.PerformUltimate();
            buttonCanvas.gameObject.SetActive(false);
        }
    }

    public void Impulse(float force)
    {
        impulseSource.GenerateImpulse(force);
    }
}
