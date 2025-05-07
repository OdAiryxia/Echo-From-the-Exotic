using System.Collections;
using UnityEngine;

public class TestUnit : Unit
{
    [Header("動畫")]
    [SerializeField] private Animator animator;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        unitName = DataContainer.instance.playerName;

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

        animator.SetTrigger("Attack");
        yield return StartCoroutine(MoveToPosition(unit.transform.position - unit.transform.forward * -attackDistance));

        GainUltimateEnergy(energyGainOnAttack);

        var (damage, isCrit) = CalculateDamage();
        unit.TakeDamage(damage, isCrit);

        BattleManagerUI.instance.Impulse(0.2f);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));
        transform.rotation = originalRotation;

        BattleManager.instance.RemoveUnit();
        BattleManager.instance.OnPlayerActionComplete();
    }
    #endregion

    #region Skill
    public override void PerformSkill()
    {
        if (gameObject.tag == "TeamPlayer")
        {
            if (BattleManager.instance.selectedEnemyUnit != null)
            {
                Debug.Log($"{unitName} used {attackName}");
                remainingSkillUses--;
                StartCoroutine(MoveAndSkill(BattleManager.instance.selectedEnemyUnit));
            }
        }
        if (gameObject.tag == "TeamEnemy")
        {
            if (BattleManager.instance.selectedPlayerUnit != null)
            {
                Debug.Log($"{unitName} used {attackName}");
                remainingSkillUses--;
                StartCoroutine(MoveAndSkill(BattleManager.instance.selectedPlayerUnit));
            }
        }
    }

    IEnumerator MoveAndSkill(Unit unit)
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // 旋轉面向選定的敵方單位
        Vector3 directionToEnemy = (unit.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        transform.rotation = lookRotation; // 設定面向敵人

        animator.SetTrigger("Skill");
        yield return StartCoroutine(MoveToPosition(unit.transform.position - unit.transform.forward * -attackDistance));

        GainUltimateEnergy(energyGainOnSkill);

        var (damage, isCrit) = CalculateDamage();

        unit.TakeDamage(damage * 1.5f, isCrit);
        BattleManagerUI.instance.Impulse(0.2f);
        yield return new WaitForSeconds(0.2f);

        // 對其他敵人造成 0.5 倍的範圍傷害
        foreach (Unit enemy in BattleManager.instance.teamEnemyUnits.ToArray())
        {
            if (enemy != null)
            {
                float areaDamage = damage * 0.5f;
                enemy.TakeDamage(areaDamage, isCrit);
            }
        }
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));
        transform.rotation = originalRotation;

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
        originalRotation = transform.rotation;

        // 旋轉面向選定的敵方單位
        Vector3 directionToEnemy = (unit.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToEnemy);
        transform.rotation = lookRotation; // 設定面向敵人

        animator.SetTrigger("Ultimate");
        yield return StartCoroutine(MoveToPosition(unit.transform.position - unit.transform.forward * -attackDistance));

        ultimateEnergy = 0f;

        yield return new WaitForSeconds(0.2f);

        var (damage, isCrit) = CalculateDamage();
        unit.TakeDamage(damage * 5f, isCrit);
        if (remainingSkillUses < maxSkillUses)
        {
            remainingSkillUses++;
        }
        BattleManagerUI.instance.Impulse(0.2f);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoveToPosition(originalPosition));
        transform.rotation = originalRotation;

        BattleManager.instance.RemoveUnit();
        BattleManager.instance.OnPlayerActionComplete();
    }
    #endregion

    public override void TakeDamage(float damage, bool isCrit)
    {
        animator.SetTrigger("Hit");
        base.TakeDamage(damage, isCrit);
    }
}
