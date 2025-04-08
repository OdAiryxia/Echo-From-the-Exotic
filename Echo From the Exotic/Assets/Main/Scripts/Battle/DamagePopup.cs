using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    void LateUpdate()
    {
        if (BattleManager.instance.mainCamera != null)
        {
            transform.LookAt(BattleManager.instance.mainCamera.transform);
            transform.RotateAround(transform.position, transform.up, 180f);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
