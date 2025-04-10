using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("單位名稱")]
    public string unitName;
    public GameObject unitObject;
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
    public int maxSkillUses = 5;
    public int remainingSkillUses;
    [Header("大招")]
    public string ultimateName = "Ultimate Attack";
    public float ultimateEnergy = 40f;
    public float maxUltimateEnergy = 100f;
    [Space(5)]
    public float energyGainOnAttack = 10f;
    public float energyGainOnSkill = 20f;
    public float energyGainOnDamage = 5f;

    [Header("移動")]
    public float moveSpeed = 20f;
    public float attackDistance = 10f;

    void Awake()
    {
        remainingSkillUses = maxSkillUses;
    }

    public virtual void PerformAttack()
    {
        Debug.Log($"{unitName} used {attackName}");
        GainUltimateEnergy(energyGainOnAttack);

        BattleManager.instance.OnPlayerActionComplete();
    }

    public virtual void PerformSkill()
    {
        if (remainingSkillUses > 0)
        {
            remainingSkillUses--;
            Debug.Log($"{unitName} used {skillName}");
            GainUltimateEnergy(energyGainOnSkill);

            BattleManager.instance.OnPlayerActionComplete();
        }
    }

    public virtual void PerformUltimate()
    {
        if (ultimateEnergy >= maxUltimateEnergy)
        {
            Debug.Log($"{unitName} used {ultimateName}!");
            ultimateEnergy = 0f;

            BattleManager.instance.OnPlayerActionComplete();
        }
        else
        {
            Debug.Log($"{unitName} doesn't have enough ultimate energy!");
        }
    }

    public virtual void TakeDamage(float damage, bool isCrit)
    {
        float defenceFactor = defence / (defence + 50); // C = 50，可自行調整
        float finalDamage = Mathf.Max(damage * (1 - defenceFactor), 1); // 計算最終傷害（確保不低於 1）

        BattleManager.instance.GenerateDamagePopup(((int)finalDamage), this.transform.position + new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(1f, 2f), 0), isCrit);
        GainUltimateEnergy(energyGainOnDamage);

        health -= ((int)finalDamage);
        if (health <= 0)
        {
            BattleManager.instance.MarkRemovalUnit(this);
        }
    }

    public void GainUltimateEnergy(float energyAmount)
    {
        ultimateEnergy += energyAmount;

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
