using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleGameManagerUI : MonoBehaviour
{
    public static BattleGameManagerUI instance;

    private Unit currentUnit;

    [SerializeField] private GameObject buttonCanvas;

    [SerializeField] private Button attackButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button ultimateButton;

    [SerializeField] private TextMeshProUGUI attackButtonText;
    [SerializeField] private TextMeshProUGUI skillButtonText;
    [SerializeField] private TextMeshProUGUI ultimateButtonText;

    [SerializeField] private List<Image> skillPointImages;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 顯示當前角色的絕招
    public void ShowUnitButtonPanel(Unit unit)
    {
        currentUnit = unit;

        buttonCanvas.gameObject.SetActive(true);

        attackButtonText.text = unit.attackName;
        skillButtonText.text = unit.skillName;
        ultimateButtonText.text = unit.ultimateName;

        // 更新技能按鈕狀態和技能點顯示
        skillButton.interactable = unit.remainingSkillUses > 0;
        UpdateSkillPointDisplay(unit.remainingSkillUses, unit.maxSkillUses);

        // 更新大招按鈕狀態
        ultimateButton.interactable = unit.ultimateEnergy >= unit.maxUltimateEnergy;
    }

    private void UpdateSkillPointDisplay(int remainingUses, int maxUses)
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
}
