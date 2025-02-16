using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlayer1 : Unit
{
    private Vector3 originalPosition; // 保存角色的原始位置
    private void Start()
    {
        originalPosition = transform.position; // 存儲初始位置
    }

    public override void PerformAttack()
    {
        if (UnitActionSystem.instance.selectedEnemyUnit != null)
        {
            Debug.Log($"{unitName} used {attackName}");
            StartCoroutine(MoveAndAttack(UnitActionSystem.instance.selectedEnemyUnit));
        }
    }

    private IEnumerator MoveAndAttack(Unit enemyUnit)
    {
        originalPosition = transform.position;
        yield return StartCoroutine(MoveToPosition(enemyUnit.transform.position - enemyUnit.transform.forward * attackDistance));

        GainUltimateEnergy(energyGainOnAttack);

        enemyUnit.TakeDamage(CalculateDamage().damage, CalculateDamage().isCrit);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));

        BattleGameManager.instance.OnPlayerActionComplete();
    }

    public override void PerformSkill()
    {
        if (UnitActionSystem.instance.selectedEnemyUnit != null)
        {
            Debug.Log($"{unitName} used {skillName}");
            remainingSkillUses--;
            StartCoroutine(MoveAndSkill(UnitActionSystem.instance.selectedEnemyUnit));
        }
    }

    private IEnumerator MoveAndSkill(Unit enemyUnit)
    {
        originalPosition = transform.position;
        yield return StartCoroutine(MoveToPosition(enemyUnit.transform.position - enemyUnit.transform.forward * attackDistance));

        GainUltimateEnergy(energyGainOnSkill);

        enemyUnit.TakeDamage(CalculateDamage().damage * 1.5f, CalculateDamage().isCrit);

        yield return new WaitForSeconds(0.2f);

        // 對其他敵人造成 0.5 倍的範圍傷害
        foreach (Unit enemy in UnitActionSystem.instance.allEnemyUnits)
        {
            float areaDamage = CalculateDamage().damage * 0.5f;
            enemy.TakeDamage(areaDamage, CalculateDamage().isCrit);
        }
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));

        BattleGameManager.instance.OnPlayerActionComplete();
    }

    public override void PerformUltimate()
    {
        if (UnitActionSystem.instance.selectedEnemyUnit != null)
        {
            Debug.Log($"{unitName} used {ultimateName}!");
            StartCoroutine(MoveAndUltimate(UnitActionSystem.instance.selectedEnemyUnit));
        }
    }

    private IEnumerator MoveAndUltimate(Unit enemyUnit)
    {
        originalPosition = transform.position;
        yield return StartCoroutine(MoveToPosition(enemyUnit.transform.position - enemyUnit.transform.forward * attackDistance));

        ultimateEnergy = 0f;

        enemyUnit.TakeDamage(CalculateDamage().damage * 5f, CalculateDamage().isCrit);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));

        BattleGameManager.instance.OnPlayerActionComplete();
    }

    // 移動到指定位置的協程
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
