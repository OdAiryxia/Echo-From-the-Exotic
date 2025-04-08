using System.Collections;
using UnityEngine;

public class TestUnit : Unit
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    #region Attack
    public override void PerformAttack()
    {
        if (gameObject.tag == "TeamPlayer")
        {
            if (BattleManager.instance.selectedEnemyUnit != null)
            {
                Debug.Log($"{unitName} used {attackName}");
                StartCoroutine(MoveAndAttack(BattleManager.instance.selectedEnemyUnit));
            }
        }
        if (gameObject.tag == "TeamEnemy")
        {
            if (BattleManager.instance.selectedPlayerUnit != null)
            {
                Debug.Log($"{unitName} used {attackName}");
                StartCoroutine(MoveAndAttack(BattleManager.instance.selectedPlayerUnit));
            }
        }
    }

    IEnumerator MoveAndAttack(Unit unit)
    {
        originalPosition = transform.position;
        yield return StartCoroutine(MoveToPosition(unit.transform.position - unit.transform.forward * -attackDistance));

        GainUltimateEnergy(energyGainOnAttack);

        unit.TakeDamage(CalculateDamage().damage, CalculateDamage().isCrit);
        BattleManagerUI.instance.Impulse(0.2f);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));

        BattleManager.instance.RemoveUnit();
        BattleManager.instance.OnPlayerActionComplete();
    }
    #endregion

    #region Skill
    public override void PerformSkill()
    {
        if (BattleManager.instance.selectedEnemyUnit != null)
        {
            Debug.Log($"{unitName} used {skillName}");
            remainingSkillUses--;
            StartCoroutine(MoveAndSkill(BattleManager.instance.selectedEnemyUnit));
        }
    }

    IEnumerator MoveAndSkill(Unit unit)
    {
        originalPosition = transform.position;
        yield return StartCoroutine(MoveToPosition(unit.transform.position - unit.transform.forward * -attackDistance));

        GainUltimateEnergy(energyGainOnSkill);

        unit.TakeDamage(CalculateDamage().damage * 1.5f, CalculateDamage().isCrit);
        BattleManagerUI.instance.Impulse(0.2f);

        yield return new WaitForSeconds(0.2f);

        // 對其他敵人造成 0.5 倍的範圍傷害
        foreach (Unit enemy in BattleManager.instance.teamEnemyUnits.ToArray())
        {
            if (enemy != null)
            {
                float areaDamage = CalculateDamage().damage * 0.5f;
                enemy.TakeDamage(areaDamage, CalculateDamage().isCrit);
            }
        }
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));

        BattleManager.instance.RemoveUnit();
        BattleManager.instance.OnPlayerActionComplete();
    }
    #endregion

    #region Ultimate
    public override void PerformUltimate()
    {
        if (BattleManager.instance.selectedEnemyUnit != null)
        {
            Debug.Log($"{unitName} used {ultimateName}!");
            StartCoroutine(MoveAndUltimate(BattleManager.instance.selectedEnemyUnit));
        }
    }

    IEnumerator MoveAndUltimate(Unit unit)
    {
        originalPosition = transform.position;
        yield return StartCoroutine(MoveToPosition(unit.transform.position - unit.transform.forward * -attackDistance));

        ultimateEnergy = 0f;

        unit.TakeDamage(CalculateDamage().damage * 5f, CalculateDamage().isCrit);
        BattleManagerUI.instance.Impulse(0.2f);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));

        BattleManager.instance.RemoveUnit();
        BattleManager.instance.OnPlayerActionComplete();
    }
    #endregion
}
