using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections;

public class BattleGameManager : MonoBehaviour
{
    public static BattleGameManager instance;

    public BattleScene battleScene;

    [Header("隊伍設定")]
    public List<GameObject> playerPrefabs; // 玩家角色的Prefab
    public List<Transform> playerSpawnPoints; // 玩家角色生成點
    public List<GameObject> enemyPrefabs; // 敵方角色的Prefab
    public List<Transform> enemySpawnPoints; // 敵方角色生成點

    public List<Unit> playerTeam;
    public List<Unit> enemyTeam;
    public List<Unit> allUnits;

    private bool isPlayerActionComplete = true;
    private bool isActionInProgress = false; // 控制行動是否正在進行
    private bool battleStarted = false;

    public float actionValueDecreaseRate = 10f; // 行動值減少速率


    [Header("UI 元素")]
    [SerializeField] private GameObject damagePopupPrefeb;

    private void Awake()
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
    }

    void Start()
    {
        allUnits = new List<Unit>();
        allUnits.AddRange(playerTeam);
        allUnits.AddRange(enemyTeam);

        InitializeTeams();
        StartBattle();
    }

    private void InitializeTeams()
    {

        playerPrefabs = new List<GameObject>();
        enemyPrefabs = new List<GameObject>();
        playerTeam = new List<Unit>();
        enemyTeam = new List<Unit>();
        allUnits = new List<Unit>();

        foreach (var prefab in DataContainer.instance.battlePrefebPlayer)
        {
            if (prefab != null)
            {
                playerPrefabs.Add(prefab);
            }
        }

        foreach (var prefab in DataContainer.instance.battlePrefebEnemy)
        {
            if (prefab != null)
            {
                enemyPrefabs.Add(prefab);
            }
        }

        // 生成玩家隊伍
        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            if (i < playerSpawnPoints.Count)
            {
                // 在對應的生成點生成玩家物件
                GameObject playerObj = Instantiate(playerPrefabs[i], playerSpawnPoints[i].position, playerSpawnPoints[i].rotation, playerSpawnPoints[i]);
                Unit playerUnit = playerObj.GetComponent<Unit>();
                playerTeam.Add(playerUnit);
                allUnits.Add(playerUnit);

                // 設定玩家角色輪廓為隱藏
                Outline playerOutline = playerUnit.unitObject.GetComponent<Outline>();
                playerOutline.OutlineMode = Outline.Mode.OutlineHidden;
            }
        }

        // 生成敵人隊伍
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            if (i < enemySpawnPoints.Count)
            {
                // 在對應的生成點生成敵人物件
                GameObject enemyObj = Instantiate(enemyPrefabs[i], enemySpawnPoints[i].position, enemySpawnPoints[i].rotation, enemySpawnPoints[i]);
                Unit enemyUnit = enemyObj.GetComponent<Unit>();
                enemyTeam.Add(enemyUnit);
                allUnits.Add(enemyUnit);

                // 設定敵人角色輪廓為隱藏
                Outline enemyOutline = enemyUnit.unitObject.GetComponent<Outline>();
                enemyOutline.OutlineMode = Outline.Mode.OutlineHidden;
            }
        }

        // 排序所有單位的行動值
        allUnits = allUnits.OrderBy(c => c.actionValue).ToList();

        UnitActionSystem.instance.selectedEnemyUnit = enemyTeam[0];
        UnitActionSystem.instance.selectedPlayerUnit = playerTeam[0];
        UnitActionSystem.instance.enemyOutline = enemyTeam[0].unitObject.GetComponent<Outline>();
        UnitActionSystem.instance.playerOutline = playerTeam[0].unitObject.GetComponent<Outline>();
        UnitActionSystem.instance.enemyOutline.OutlineMode = Outline.Mode.OutlineAll;
        UnitActionSystem.instance.playerOutline.OutlineMode = Outline.Mode.OutlineAll;
    }

    void Update()
    {
        if (battleStarted && isPlayerActionComplete && !isActionInProgress)
        {
            // 持續減少行動值，直到有一個角色的行動值達到 0
            ReduceActionValuesOverTime();

            // 檢查輪到哪個角色行動
            allUnits = allUnits.OrderBy(c => c.actionValue).ToList();

            if (allUnits.Count > 0 && allUnits[0].actionValue <= 0)
            {
                Unit currentUnit = allUnits[0];
                currentUnit.ActionComplete();
                if (playerTeam.Contains(currentUnit))
                {
                    BattleGameManagerUI.instance.ShowUnitButtonPanel(currentUnit);
                }

                // 處理當前角色的行動
                HandleUnitAction(currentUnit);
                UnitActionSystem.instance.SetupUnitsForNewRound();

                // 將當前角色移到隊伍的最後
                allUnits.RemoveAt(0);
                allUnits.Add(currentUnit);
            }

            if (playerTeam.Count <= 0 || enemyTeam.Count <= 0)
            {
                EndBattle();
            }
        }
    }

    public void GenerateDamagePopup(int damage, Vector3 pos, bool isCrit)
    {
        var damagePopup = Instantiate(damagePopupPrefeb, new Vector3(Random.Range(pos.x - 0.4f, pos.x + 0.4f), Random.Range(pos.y - 0.1f, pos.y + 0.1f), pos.z), Quaternion.identity);

        var textMesh = damagePopup.GetComponent<TextMeshPro>();
        textMesh.text = damage.ToString();
        textMesh.color = isCrit ? Color.red : Color.white;
    }

    void HandleUnitAction(Unit unit)
    {
        // 停止減少行動值，直到該角色行動完畢
        isActionInProgress = true;

        // 在這裡添加角色行動邏輯
        if (playerTeam.Contains(unit))
        {
            // 玩家角色行動
            isPlayerActionComplete = false;

            UnitActionSystem.instance.playerOutline.OutlineMode = Outline.Mode.OutlineHidden;
            UnitActionSystem.instance.selectedPlayerUnit = unit;
            UnitActionSystem.instance.playerOutline = unit.unitObject.GetComponent<Outline>();
            UnitActionSystem.instance.playerOutline.OutlineMode = Outline.Mode.OutlineAll;

            UnitActionSystem.instance.enemyOutline.OutlineMode = Outline.Mode.OutlineHidden;
            UnitActionSystem.instance.selectedEnemyUnit = enemyTeam[0];
            UnitActionSystem.instance.enemyOutline = enemyTeam[0].unitObject.GetComponent<Outline>();
            UnitActionSystem.instance.enemyOutline.OutlineMode = Outline.Mode.OutlineAll;

            Debug.Log($"{unit.unitName} from player team is taking action.");
            // 在這裡添加玩家行動的具體邏輯
        }
        else if (enemyTeam.Contains(unit))
        {
            // 敵方角色行動
            AutoEnemyAttack(unit);
            Debug.Log($"{unit.unitName} from enemy team is taking action.");
            // 在這裡添加敵方行動的具體邏輯
        }
    }

    void AutoEnemyAttack(Unit enemyUnit)
    {
        // 隨機選擇一個玩家隊伍中的目標
        if (playerTeam.Count > 0)
        {
            Unit target = playerTeam[Random.Range(0, playerTeam.Count)];

            UnitActionSystem.instance.playerOutline.OutlineMode = Outline.Mode.OutlineHidden;
            UnitActionSystem.instance.selectedPlayerUnit = target;
            UnitActionSystem.instance.playerOutline = target.unitObject.GetComponent<Outline>();
            UnitActionSystem.instance.playerOutline.OutlineMode = Outline.Mode.OutlineAll;

            enemyUnit.PerformAttack(); // 執行攻擊
        }
    }

    // 減少所有單位的行動值，直到有一個角色可以行動
    void ReduceActionValuesOverTime()
    {
        foreach (var unit in allUnits)
        {
            unit.actionValue -= actionValueDecreaseRate * Time.deltaTime * unit.speed;
        }
    }

    public void OnPlayerActionComplete()
    {
        // 玩家行動完畢，恢復減少行動值的過程
        isPlayerActionComplete = true;
        isActionInProgress = false;
    }

    public void RemoveUnit(Unit unit)
    {
        if (playerTeam.Contains(unit))
        {
            playerTeam.Remove(unit);
        }
        else if (enemyTeam.Contains(unit))
        {
            enemyTeam.Remove(unit);
            unit.gameObject.SetActive(false);
        }

        allUnits.Remove(unit);
        Debug.Log($"{unit.unitName} has been removed from the battle.");
    }

    private void StartBattle()
    {
        battleStarted = true;
    }

    private void EndBattle()
    {
        battleStarted = false;
        string result = playerTeam.Count > 0 ? "You Win!" : "You Lose!";

        Debug.Log(result);
        StartCoroutine(ExitBattle());
    }

    private IEnumerator ExitBattle()
    {
        yield return new WaitForSeconds(3);
        if (battleScene == BattleScene.SchoolOutdoor)
        {
            GameManager.instance.LoadWorldSchoolOutdoor();
        }
        else if (battleScene == BattleScene.Parkinglot)
        {
            GameManager.instance.LoadWorldParkinglot();
        }
    }
}
