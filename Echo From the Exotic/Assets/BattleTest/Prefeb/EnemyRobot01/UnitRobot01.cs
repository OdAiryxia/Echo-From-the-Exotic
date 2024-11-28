using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRobot01 : Unit
{
    Animator animator;

    private Vector3 originalPosition; // 保存角色的原始位置
    private Quaternion originalRotation; // 保存角色的原始旋轉角度

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        originalPosition = transform.position; // 存儲初始位置
        originalRotation = transform.rotation; // 存儲初始旋轉
    }

    public override void PerformAttack()
    {
        if (UnitActionSystem.instance.selectedPlayerUnit != null)
        {
            Debug.Log($"{unitName} used {attackName}");
            StartCoroutine(MoveAndAttack(UnitActionSystem.instance.selectedPlayerUnit));
        }
    }

    private IEnumerator MoveAndAttack(Unit playerUnit)
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // 旋轉面向選定的敵方單位
        Vector3 directionToEnemy = (playerUnit.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        transform.rotation = lookRotation; // 設定面向敵人

        yield return StartCoroutine(MoveToPosition(playerUnit.transform.position - playerUnit.transform.forward * -attackDistance));

        animator.Play("Attack");

        yield return new WaitForSeconds(1f);

        GainUltimateEnergy(energyGainOnAttack);
        playerUnit.TakeDamge(CritDamage().damage, CritDamage().isCrit);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MoveToPosition(originalPosition));
        transform.rotation = originalRotation;

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
