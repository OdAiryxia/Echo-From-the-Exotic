using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using UnityEngine.EventSystems;
using System.Threading;

public class BattleManagerUI : MonoBehaviour
{
    public static BattleManagerUI instance;

    public Unit currentUnit;

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
    [SerializeField] private Slider ultimateSlider;
    [SerializeField] private TMP_Text speedText;

    public TMP_Text desc;

    [Space(10)]
    [SerializeField] private Image battleStatusImage;
    [SerializeField] private TextMeshProUGUI battleStatusText;
    [SerializeField] private Animator battleStatusAnimator;

    [Space(20)]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [SerializeField] private ModalWindowTemplate[] tutorial_1;
    [SerializeField] private ModalWindowTemplate[] tutorial_2;
    private ModalWindowTemplate[] modalWindowTemplates;

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

        StartCoroutine(Tutorial());

        ShowBattleStatus("戰鬥開始");
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

        ultimateSlider.maxValue = currentUnit.maxUltimateEnergy;
        ultimateSlider.value = currentUnit.ultimateEnergy;

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

    public void ShowBattleStatus(string text)
    {
        battleStatusImage.gameObject.SetActive(true);
        battleStatusText.text = text;
        battleStatusAnimator.SetTrigger("Play");
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

    private void Update()
    {
        if (BattleManager.instance.inBattle)
        {
            string displayText = "Next:\n";
            int[] fontSizes = { 36, 30, 24 };
            var units = BattleManager.instance.teamAllUnits;

            int count = Mathf.Min(units.Count, 3); // 保證最多只處理 3 個，避免超出索引

            for (int i = 0; i < count; i++)
            {
                var unit = units[i];

                if (unit != null)
                {
                    int fontSize = fontSizes[i];
                    displayText += $"<size={fontSize}><b>{unit.unitName}</b> - <color=#FF8800>[{unit.actionValue}]</color></size>\n";
                }
            }

            speedText.text = displayText.TrimEnd('\n');
        }
    }

    public IEnumerator Tutorial()
    {
        yield return new WaitForSeconds(1.6f);
        switch (DataContainer.instance.battleCount)
        {
            case 0:
                modalWindowTemplates = tutorial_1;
                ShowCurrentModal();
                DataContainer.instance.battleCount++;
                break;
            case 1:
                modalWindowTemplates = tutorial_2;
                ShowCurrentModal();
                DataContainer.instance.battleCount++;
                break;
            default:
                break;
        }
        yield return null;
    }

    public int currentIndex = 0;
    private void ShowCurrentModal()
    {
        if (currentIndex >= modalWindowTemplates.Length)
        {
            ModalWindowManager.instance.Close();
            return;
        }

        ModalWindowTemplate currentTemplate = modalWindowTemplates[currentIndex];

        ModalWindowManager.instance.ShowVertical(
            currentTemplate.title,
            currentTemplate.image,
            currentTemplate.context,
            currentTemplate.confirmText, () =>
            {
                currentIndex++;
                ModalWindowManager.instance.Close();
                ShowCurrentModal(); // 顯示下一個
            },
            currentTemplate.declineText, () =>
            {
                ModalWindowManager.instance.Close();
            },
            currentTemplate.alternateText, () =>
            {
                ModalWindowManager.instance.Close();
            }
        );
    }
}
