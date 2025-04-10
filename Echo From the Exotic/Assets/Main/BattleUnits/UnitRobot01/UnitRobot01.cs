using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRobot01 : Unit
{
    [Header("動畫")]
    [SerializeField] private Animator animator;

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
        originalRotation = transform.rotation;

        // 旋轉面向選定的敵方單位
        Vector3 directionToEnemy = (unit.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        transform.rotation = lookRotation; // 設定面向敵人

        yield return StartCoroutine(MoveToPosition(unit.transform.position - unit.transform.forward * -attackDistance));

        animator.Play("Attack");

        yield return new WaitForSeconds(0.75f);

        GainUltimateEnergy(energyGainOnAttack);

        var (damage, isCrit) = CalculateDamage();
        unit.TakeDamage(damage, isCrit);

        BattleManagerUI.instance.Impulse(0.2f);

        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(MoveToPosition(originalPosition));
        transform.rotation = originalRotation;

        BattleManager.instance.RemoveUnit();
        BattleManager.instance.OnPlayerActionComplete();
    }
    #endregion

    public override void TakeDamage(float damage, bool isCrit)
    {
        animator.Play("Being Hit");
        base.TakeDamage(damage, isCrit);
    }
}
