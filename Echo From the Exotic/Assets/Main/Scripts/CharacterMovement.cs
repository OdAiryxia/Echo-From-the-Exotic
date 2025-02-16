using Cinemachine;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController controller;
    [SerializeField] private CanvasGroup aimCanvas;
    [SerializeField] private Image aimDot;

    public Camera mainCamera;
    public CinemachineFreeLook thirdPersonCamera;
    public CinemachineVirtualCamera aimCamera;

    public float moveSpeed = 50f;
    public float sprintMultiplier = 1.5f;
    public float gravityMultiplier = 1f;
    public float renderDelay = 0.2f;

    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [SerializeField] Animator animator;

    Vector3 velocity;
    private bool isAiming = false;

    private Renderer[] playerRenderers;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        aimCanvas.alpha = 0;
        //transform.position = DataContainer.instance.playerPositionSchool;
        playerRenderers = GetComponentsInChildren<Renderer>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        aimCamera.Priority = 10; // 降低瞄準相機的優先權
        thirdPersonCamera.Priority = 20; // 恢復第三人稱相機
    }

    void Update()
    {
        HandleAiming();
        MoveCharacter();
        HandleShooting();
    }

    void HandleAiming()
    {
        if (Input.GetMouseButtonDown(1)) // 按下右鍵，進入瞄準模式
        {

            // 讓 aimCamera 旋轉與 thirdPersonCamera 保持一致
            var aimPOV = aimCamera.GetCinemachineComponent<CinemachinePOV>();
            var thirdPersonPOV = thirdPersonCamera.GetComponent<CinemachineFreeLook>();

            if (aimPOV != null && thirdPersonPOV != null)
            {
                aimPOV.m_HorizontalAxis.Value = thirdPersonPOV.m_XAxis.Value; // 同步水平旋轉
                aimPOV.m_VerticalAxis.Value = thirdPersonPOV.m_YAxis.Value;   // 同步垂直旋轉
            }

            aimCanvas.alpha = 1;
            aimCamera.Priority = 20; // 切換到瞄準鏡頭
            thirdPersonCamera.Priority = 5; // 降低第三人稱相機的優先權

            // 進入瞄準模式，禁用角色渲染
            StopAllCoroutines();
            StartCoroutine(DisablePlayerRenderingWithDelay());

            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1)) // 鬆開右鍵，離開瞄準模式
        {
            aimCanvas.alpha = 0;

            aimCamera.Priority = 10; // 降低瞄準相機的優先權
            thirdPersonCamera.Priority = 20; // 恢復第三人稱相機

            // 恢復渲染
            StopAllCoroutines();
            StartCoroutine(EnablePlayerRenderingWithDelay());

            isAiming = false;
        }
    }
    private float moveSpeedValue = 0f;

    void MoveCharacter()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * sprintMultiplier : moveSpeed;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        velocity.y = -9.81f * gravityMultiplier;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float targetSpeed = 0f;

        if (isAiming)
        {
            // 在瞄準模式下，使用 aimCamera 的 Y 軸角度控制角色旋轉
            float cameraY = mainCamera.transform.eulerAngles.y; // 取得 aimCamera 的 Y 軸旋轉角度
            transform.rotation = Quaternion.Euler(0f, cameraY, 0f); // 角色面向相機的 Y 軸

            // 根據角色朝向移動
            Vector3 moveDir = transform.right * horizontal + transform.forward * vertical;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            targetSpeed = moveDir.magnitude > 0 ? 0.5f : 0f;
        }
        else
        {
            // 一般模式下，角色始終朝向相機的前方
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);

                targetSpeed = Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;
            }
        }

        controller.Move(velocity * Time.deltaTime);
        moveSpeedValue = Mathf.Lerp(moveSpeedValue, targetSpeed, Time.deltaTime * 10f);
        animator.SetFloat("Speed", moveSpeedValue);
    }

    void HandleShooting()
    {
        if (isAiming)
        {
            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward); // 從相機位置發射 Raycast

            if (Physics.Raycast(ray, out hit, 300f))
            {
                if (hit.collider.CompareTag("BattleTrigger")) // 只對 "BattleTrigger" 物體生效
                {
                    Debug.Log("擊中 BattleTrigger: " + hit.collider.gameObject.name);
                    aimDot.color = Color.red; // 命中時變紅

                    // **只有在按下左鍵時，才執行動作**
                    if (Input.GetMouseButtonDown(0))
                    {
                        BattleTrigger target = hit.collider.GetComponent<BattleTrigger>();
                        if (target != null)
                        {
                            target.ExecuteAction();
                        }
                    }
                }
                else
                {
                    aimDot.color = Color.white; // 不是 BattleTrigger 時恢復白色
                }
            }
            else
            {
                aimDot.color = Color.white; // 沒有命中任何物體時恢復白色
            }
        }
    }

    // 延遲禁用角色的渲染
    IEnumerator DisablePlayerRenderingWithDelay()
    {
        yield return new WaitForSeconds(renderDelay);
        foreach (Renderer renderer in playerRenderers)
        {
            renderer.enabled = false;
        }
    }

    // 延遲恢復角色的渲染
    IEnumerator EnablePlayerRenderingWithDelay()
    {
        yield return new WaitForSeconds(renderDelay * 0.1f);
        foreach (Renderer renderer in playerRenderers)
        {
            renderer.enabled = true;
        }
    }
}
