using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem instance;
    [SerializeField] private Camera mainCamera;

    public Unit selectedEnemyUnit;
    public List<Unit> allEnemyUnits = new List<Unit>();  // 存儲所有敵方單位
    public Outline enemyOutline;

    public Unit selectedPlayerUnit;
    public List<Unit> allPlayerUnits = new List<Unit>(); // 存儲所有玩家單位
    public Outline playerOutline;

    [SerializeField] private LayerMask unitLayerMask;

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
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryHandleUnitSelection()) return;
        }
    }

    public void SetupUnitsForNewRound()
    {
        // 清空之前的單位列表
        allEnemyUnits.Clear();
        allPlayerUnits.Clear();

        // 找到場景中所有有 Unit 組件的物體
        List<Unit> allUnitsInScene = BattleGameManager.instance.allUnits;

        // 根據標籤或其他屬性來分類單位
        foreach (Unit unit in allUnitsInScene)
        {
            if (unit.CompareTag("PlayerTeam"))
            {
                allPlayerUnits.Add(unit);
            }
            else if (unit.CompareTag("EnemyTeam"))
            {
                allEnemyUnits.Add(unit);
            }
        }
    }

    private bool TryHandleUnitSelection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
        {
            if (raycastHit.transform.CompareTag("PlayerTeam"))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    playerOutline.OutlineMode = Outline.Mode.OutlineHidden;

                    selectedPlayerUnit = unit;
                    playerOutline = unit.unitObject.GetComponent<Outline>();
                    playerOutline.OutlineMode = Outline.Mode.OutlineAll;

                    return true;
                }
            }
            else if (raycastHit.transform.CompareTag("EnemyTeam"))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    enemyOutline.OutlineMode = Outline.Mode.OutlineHidden;

                    selectedEnemyUnit = unit;
                    enemyOutline = unit.unitObject.GetComponent<Outline>();
                    enemyOutline.OutlineMode = Outline.Mode.OutlineAll;

                    return true;
                }
            }
        }
        return false;
    }
}
