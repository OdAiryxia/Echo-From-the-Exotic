using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GameObject unitObject;
    [Header("單位名稱")]
    public string unitName;
    [Header("行動速度")]
    public float actionValue = 100f;
    public float speed = 10f;
    [Header("屬性值")]
    public float health;
    public float attack;
    public float defence;
    [Space(5)]
    public float critRate;
    public float critDamage;
    [Header("攻擊")]
    public string attackName = "Attack";
    [Header("技能")]
    public string skillName = "Skill";
    public int maxSkillUses = 5; // 技能使用次數限制
    public int remainingSkillUses; // 剩餘技能使用次數
    [Header("大招")]
    public string ultimateName = "Ultimate Attack";
    public float ultimateEnergy = 40f; // 當前大招能量
    public float maxUltimateEnergy = 100f; // 大招能量最大值
    public float energyGainOnAttack = 10f; // 每次攻擊獲得的能量
    public float energyGainOnSkill = 20f; // 每次技能獲得的能量
    public float energyGainOnDamage = 5f; // 受到攻擊時獲得的能量

    private Vector3 targetPosition;
    [Header("移動")]
    public float moveSpeed = 20f;      // 設置移動速度
    public float attackDistance = 10f; // 移動到敵人前的距離

    private void Awake()
    {
        targetPosition = transform.position;
        remainingSkillUses = maxSkillUses; // 初始化剩餘技能次數
    }

    private void Update()
    {
        
    }


    // 設定角色行動後的行動值回到初始速度值
    public void ActionComplete()
    {
        actionValue = 100f;
    }

    public virtual void PerformAttack()
    {
        Debug.Log($"{unitName} used {attackName}");
        // 添加攻擊邏輯

        // 增加大招能量
        GainUltimateEnergy(energyGainOnAttack);

        // 通知隊伍管理器，玩家行動已完成
        BattleGameManager.instance.OnPlayerActionComplete();
    }

    // 技能
    public virtual void PerformSkill()
    {
        if (remainingSkillUses > 0)
        {
            remainingSkillUses--;
            Debug.Log($"{unitName} used {skillName}");
            // 添加技能邏輯

            // 增加大招能量
            GainUltimateEnergy(energyGainOnSkill);

            // 通知隊伍管理器，玩家行動已完成
            BattleGameManager.instance.OnPlayerActionComplete();
        }
    }

    // 大招
    public virtual void PerformUltimate()
    {
        if (ultimateEnergy >= maxUltimateEnergy)
        {
            Debug.Log($"{unitName} used {ultimateName}!");
            // 添加大招邏輯

            // 施放後大招能量歸零
            ultimateEnergy = 0f;

            // 通知隊伍管理器，玩家行動已完成
            BattleGameManager.instance.OnPlayerActionComplete();
        }
        else
        {
            Debug.Log($"{unitName} doesn't have enough ultimate energy!");
        }
    }

    public void TakeDamage(float damage, bool isCrit)
    {
        float defenceFactor = defence / (defence + 50); // C = 50，可自行調整
        // 計算最終傷害（確保不低於 1）
        float finalDamage = Mathf.Max(damage * (1 - defenceFactor), 1);

        BattleGameManager.instance.GenerateDamagePopup(((int)finalDamage), this.transform.position, isCrit);
        health -= ((int)finalDamage);

        if (health <= 0)
        {
            BattleGameManager.instance.RemoveUnit(this);
        }

        // 受到攻擊時增加大招能量
        GainUltimateEnergy(energyGainOnDamage);
    }

    // 增加大招能量
    public void GainUltimateEnergy(float energyAmount)
    {
        ultimateEnergy += energyAmount;

        // 保證大招能量不會超過最大值
        if (ultimateEnergy > maxUltimateEnergy)
            ultimateEnergy = maxUltimateEnergy;

        Debug.Log($"{unitName} gained {energyAmount} ultimate energy. Current energy: {ultimateEnergy}/{maxUltimateEnergy}");
    }

    public (float damage, bool isCrit) CalculateDamage()
    {
        bool isCrit = critRate >= Random.Range(0f, 100f);
        float rawDamage = isCrit ? attack * ((100 + critDamage) / 100) : attack;

        // 計算最終傷害（考慮敵人的防禦）
        float defenceFactor = defence / (defence + 100);
        float finalDamage = Mathf.Max(rawDamage * (1 - defenceFactor), 1); // 確保最低傷害為 1

        return (finalDamage, isCrit);
    }
}
