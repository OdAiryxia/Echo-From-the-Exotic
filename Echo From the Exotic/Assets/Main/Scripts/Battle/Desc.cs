using UnityEngine;
using UnityEngine.EventSystems;

public class Desc : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    enum atkType
    {
        atk,
        skl,
        utm
    }

    [SerializeField] private atkType attackType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (attackType)
        {
            case atkType.atk:
                BattleManagerUI.instance.desc.text = BattleManagerUI.instance.currentUnit.attackDesc;
                break;
            case atkType.skl:
                BattleManagerUI.instance.desc.text = BattleManagerUI.instance.currentUnit.skillDesc;
                break;
            case atkType.utm:
                BattleManagerUI.instance.desc.text = BattleManagerUI.instance.currentUnit.ultimateDesc;
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManagerUI.instance.desc.text = "";
    }
}
