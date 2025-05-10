using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    [Header("UI 元素")]
    public Camera mainCamera;
    [SerializeField] private GameObject damagePopupPrefeb;
    [SerializeField] private LayerMask unitLayerMask;

    [Header("隊伍設定")]
    public List<Transform> playerSpawnpoints;
    public List<Transform> enemySpawnpoints;
    [Space(5)]
    public List<GameObject> playerPrefabs = new List<GameObject>();
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    [Header("隊伍列表")]
    public List<Unit> teamAllUnits = new List<Unit>();
    public List<Unit> teamPlayerUnits = new List<Unit>();
    public List<Unit> teamEnemyUnits = new List<Unit>();

    private List<Unit> pendingRemovalUnits = new List<Unit>();

    private float actionValueDecreaseRate = 10f;
    public bool inBattle = false;
    private bool isPlayerActionComplete = true;
    private bool isActionInProgress = false;
    [Header("目前選擇單位")]
    public Unit selectedPlayerUnit;
    public Unit selectedEnemyUnit;

    #region Initialize
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        inBattle = false;
    }

    void Start()
    {
        foreach (Transform obj in playerSpawnpoints)
        {
            foreach (Transform o in obj)
            {
                Destroy(o.gameObject);
            }
        }

        foreach (Transform obj in enemySpawnpoints)
        {
            foreach(Transform o in obj)
            {
                Destroy(o.gameObject);
            }
        }

        InitializeTeams();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void InitializeTeams()
    {
        playerPrefabs = DataContainer.instance.battlePrefabPlayer;
        enemyPrefabs = DataContainer.instance.battlePrefabEnemy;

        SpawnUnits(playerPrefabs, playerSpawnpoints, teamPlayerUnits);
        SpawnUnits(enemyPrefabs, enemySpawnpoints, teamEnemyUnits);

        foreach (Unit unit in teamPlayerUnits)
        {
            unit.gameObject.tag = "TeamPlayer";
            unit.gameObject.layer = 8;
        }

        foreach (Unit unit in teamEnemyUnits)
        {
            unit.gameObject.tag = "TeamEnemy";
            unit.gameObject.layer = 9;
            Outline outline = unit.unitObject.GetComponent<Outline>();
            outline.OutlineColor = Color.red;
        }

        teamAllUnits = teamPlayerUnits.Concat(teamEnemyUnits).OrderBy(c => c.actionValue).ToList();
        SetInitialSelection();

        inBattle = true;
    }

    void SpawnUnits(List<GameObject> prefabs, List<Transform> spawnpoints, List<Unit> teamUnits)
    {
        for (int i = 0; i < prefabs.Count && i < spawnpoints.Count; i++)
        {
            GameObject unitObj = Instantiate(prefabs[i], spawnpoints[i].position, spawnpoints[i].rotation, spawnpoints[i]);
            Unit unit = unitObj.GetComponent<Unit>();
            teamUnits.Add(unit);
            unit.unitObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
        }
    }

    void SetInitialSelection()
    {
        if (teamEnemyUnits.Count > 0 && teamPlayerUnits.Count > 0)
        {
            SetSelectedUnits(teamPlayerUnits[0], teamEnemyUnits[0]);
        }
    }
    #endregion

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.None && Cursor.visible != true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (inBattle)
        {
            if (isPlayerActionComplete && !isActionInProgress)
            {
                ReduceActionValues();
                ProcessTurn();
            }

            if (Input.GetMouseButtonDown(0))
            {
               HandleUnitSelection();
            }
        }

    }

    void HandleUnitSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitLayerMask))
        {
            if (hit.transform.TryGetComponent<Unit>(out Unit unit))
            {
                if (hit.transform.CompareTag("TeamPlayer"))
                {
                    SetSelectedUnits(unit, selectedEnemyUnit);
                }

                if (hit.transform.CompareTag("TeamEnemy"))
                {
                    SetSelectedUnits(selectedPlayerUnit, unit);
                }
            }
        }

    }

    void SetSelectedUnits(Unit player, Unit enemy)
    {
        foreach (Unit units in teamAllUnits)
        {
            SetOutline(units, false);
        }

        if (player != null) SetOutline(player, true);
        if (enemy != null) SetOutline(enemy, true);
        selectedPlayerUnit = player;
        selectedEnemyUnit = enemy;
    }

    void SetOutline(Unit unit, bool enable)
    {
        if (unit == null || unit.unitObject == null) return;
        Outline outline = unit.unitObject.GetComponent<Outline>();
        if (outline != null)
            outline.OutlineMode = enable ? Outline.Mode.OutlineAll : Outline.Mode.OutlineHidden;
    }

    void ReduceActionValues()
    {
        foreach (var unit in teamAllUnits)
            unit.actionValue -= actionValueDecreaseRate * Time.deltaTime * unit.speed;
    }

    void ProcessTurn()
    {
        teamAllUnits = teamAllUnits.OrderBy(c => c.actionValue).ToList();

        if (teamAllUnits.Count > 0 && teamAllUnits[0].actionValue <= 0)
        {
            Unit currentUnit = teamAllUnits[0];
            currentUnit.actionValue += 100f;

            if (teamPlayerUnits.Contains(currentUnit))
            {
                BattleManagerUI.instance.ShowUnitButtonPanel(currentUnit);
            }

            HandleUnitAction(currentUnit);
            teamAllUnits.RemoveAt(0);
            teamAllUnits.Add(currentUnit);
        }

        if (teamPlayerUnits.Count <= 0)
        {
            EndBattle(false);
            Debug.Log("Lose");
        }
        else if (teamEnemyUnits.Count <= 0)
        {
            EndBattle(true);
            Debug.Log("Win");
        }
    }

    public void EndBattle(bool isVictory)
    {
        inBattle = false;

        if (isVictory)
        {
            if (DataContainer.instance.nextCptAfterBattle)
            {
                ProgressManager.instance.NextChapter();
            }

            BattleManagerUI.instance.ShowBattleStatus("戰鬥勝利");
            foreach (string id in DataContainer.instance.pendingTrigger)
            {
                DataContainer.instance.removalTrigger.Add(id);
            }
            DataContainer.instance.pendingTrigger.Clear();
        }
        else if (!isVictory)
        {
            BattleManagerUI.instance.ShowBattleStatus("戰鬥失敗");
            DataContainer.instance.pendingTrigger.Clear();
        }

        StartCoroutine(ReturnToWorldAfterDelay(3f));
    }

    IEnumerator ReturnToWorldAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.instance.LoadWorld((WorldIndexes)GameManager.instance.previousWorld, "");
    }

    public void OnPlayerActionComplete()
    {
        isPlayerActionComplete = true;
        isActionInProgress = false;
    }

    void HandleUnitAction(Unit unit)
    {
        isActionInProgress = true;

        if (teamPlayerUnits.Contains(unit))
        {
            isPlayerActionComplete = false;
            SetSelectedUnits(unit, selectedEnemyUnit);

            foreach (Unit units in teamAllUnits)
            {
                SetOutline(units, false);
            }

            SetOutline(unit, true);
            SetOutline(selectedEnemyUnit, true);
        }
        else if (teamEnemyUnits.Contains(unit))
        {
            AutoEnemyAttack(unit);
        }
    }

    void AutoEnemyAttack(Unit unit)
    {
        if (teamPlayerUnits.Count == 0) return;
        Unit target = teamPlayerUnits[Random.Range(0, teamPlayerUnits.Count)];
        SetSelectedUnits(target, unit);

        foreach (Unit units in teamAllUnits)
        {
            SetOutline(units, false);
        }

        SetOutline(unit, true);
        SetOutline(target, true);

        // 決定使用攻擊還是技能
        bool useSkill = false;

        // 檢查是否有技能次數可用
        if (unit.remainingSkillUses > 0)
        {
            // 隨機決定是使用普通攻擊還是技能 (50% 機率使用技能)
            useSkill = Random.value < 0.5f;
        }

        // 執行選擇的行動
        if (useSkill)
        {
            Debug.Log($"敵人 {unit.unitName} 決定使用技能!");
            unit.PerformSkill();
        }
        else
        {
            Debug.Log($"敵人 {unit.unitName} 決定使用普通攻擊!");
            unit.PerformAttack();
        }
    }

    public void MarkRemovalUnit(Unit unit)
    {
        if (!pendingRemovalUnits.Contains(unit))
        {
            pendingRemovalUnits.Add(unit);
        }
    }

    public void UpdateSelectedUnitsAfterRemoval()
    {
        if (pendingRemovalUnits.Contains(selectedPlayerUnit))
        {
            Unit newPlayerUnit = teamPlayerUnits.FirstOrDefault(u => !pendingRemovalUnits.Contains(u));
            selectedPlayerUnit = newPlayerUnit;
        }

        if (pendingRemovalUnits.Contains(selectedEnemyUnit))
        {
            Unit newEnemyUnit = teamEnemyUnits.FirstOrDefault(u => !pendingRemovalUnits.Contains(u));
            selectedEnemyUnit = newEnemyUnit;
        }

        foreach (Unit unit in teamAllUnits)
        {
            SetOutline(unit, false);
        }

        if (selectedPlayerUnit != null) SetOutline(selectedPlayerUnit, true);
        if (selectedEnemyUnit != null) SetOutline(selectedEnemyUnit, true);
    }

    public void RemoveUnit()
    {
        UpdateSelectedUnitsAfterRemoval();

        for (int i = pendingRemovalUnits.Count - 1; i >= 0; i--)
        {
            Unit unit = pendingRemovalUnits[i];

            if (teamPlayerUnits.Contains(unit))
            {
                teamPlayerUnits.Remove(unit);
            }
            else if (teamEnemyUnits.Contains(unit))
            {
                teamEnemyUnits.Remove(unit);
            }
            teamAllUnits.Remove(unit);

            unit.gameObject.SetActive(false);
            Debug.Log($"{unit.unitName} has been removed from the battle.");
        }

        pendingRemovalUnits.Clear();
    }

    public void GenerateDamagePopup(int damage, Vector3 pos, bool isCrit)
    {
        GameObject popup = Instantiate(damagePopupPrefeb, pos, Quaternion.identity);
        TextMeshPro textMesh = popup.GetComponent<TextMeshPro>();
        textMesh.text = damage.ToString();
        textMesh.color = isCrit ? Color.red : Color.white;
    }
}
