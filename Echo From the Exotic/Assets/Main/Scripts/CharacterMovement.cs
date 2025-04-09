using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController controller;
    private Renderer[] playerRenderers;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CanvasGroup aimCanvas;
    [SerializeField] private Image aimDot;
    [SerializeField] private Volume aimVolume;
    [SerializeField] private Animator animator;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float gravityMultiplier = 1f;

    private CinemachineFreeLook.Orbit[] originalOrbits;
    private float zoomPercent = 1f;
    private float scroll;
    private float moveSpeedValue = 0f;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private float renderDelay = 0.2f;
    private Vector3 velocity;
    private bool isAiming = false;

    void Awake()
    {
        ProgressManager.instance.player = this.gameObject;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
        playerRenderers = GetComponentsInChildren<Renderer>();

        virtualCamera.Priority = 10;
        freeLookCamera.Priority = 20;
        aimCanvas.alpha = 0;
        aimVolume.priority = -1;
        originalOrbits = new CinemachineFreeLook.Orbit[freeLookCamera.m_Orbits.Length];
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++)
        {
            originalOrbits[i].m_Height = freeLookCamera.m_Orbits[i].m_Height;
            originalOrbits[i].m_Radius = freeLookCamera.m_Orbits[i].m_Radius;
        }
    }

    void Update()
    {
        if (!ProgressManager.instance.isStory)
        {
            MoveCharacter();
            HandleAiming();
            HandleShooting();
            ZoomCamera();
        }
    }

    void MoveCharacter()
    {
        float speed = isAiming ? moveSpeed * 0.5f : (Input.GetKey(KeyCode.LeftShift) ? moveSpeed * sprintMultiplier : moveSpeed);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        velocity.y = -9.81f * gravityMultiplier;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float targetSpeed = 0f;

        if (isAiming)
        {
            float cameraY = mainCamera.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, cameraY, 0f);

            Vector3 moveDir = transform.right * horizontal + transform.forward * vertical;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            targetSpeed = moveDir.magnitude > 0 ? 0.5f : 0f;
        }
        else
        {
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

    void HandleAiming()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
            aimCanvas.alpha = 1;
            aimVolume.priority = 1;
            virtualCamera.Priority = 20;
            freeLookCamera.Priority = 10;

            // 讓 aimCamera 旋轉與 thirdPersonCamera 保持一致
            var aimPOV = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
            var thirdPersonPOV = freeLookCamera.GetComponent<CinemachineFreeLook>();

            if (aimPOV != null && thirdPersonPOV != null)
            {
                aimPOV.m_HorizontalAxis.Value = thirdPersonPOV.m_XAxis.Value;
                aimPOV.m_VerticalAxis.Value = thirdPersonPOV.m_YAxis.Value;
            }

            StopAllCoroutines();
            StartCoroutine(DisablePlayerRenderer());

        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            aimCanvas.alpha = 0;
            aimVolume.priority = -1;
            virtualCamera.Priority = 10;
            freeLookCamera.Priority = 20;

            StopAllCoroutines();
            StartCoroutine(EnablePlayerRenderer());
        }
    }

    void HandleShooting()
    {
        if (isAiming)
        {
            RaycastHit hit;
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

            if (Physics.Raycast(ray, out hit, 300f))
            {
                if (hit.collider.CompareTag("BattleTrigger"))
                {
                    aimDot.color = Color.red;

                    if (Input.GetMouseButtonDown(0))
                    {
                        BattleTrigger target = hit.collider.GetComponent<BattleTrigger>();
                        if (target != null)
                        {
                            target.ExecuteBattle();
                        }
                    }
                }
                else
                {
                    aimDot.color = Color.white;
                }
            }
            else
            {
                aimDot.color = Color.white;
            }
        }
    }

    void ZoomCamera()
    {
        for (int i = 0; i < freeLookCamera.m_Orbits.Length; i++)
        {
            freeLookCamera.m_Orbits[i].m_Height = originalOrbits[i].m_Height * zoomPercent;
            freeLookCamera.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * zoomPercent;
        }
        scroll = Input.mouseScrollDelta.y;
        if (zoomPercent >= 0.5f && zoomPercent <= 1f)
        {
            zoomPercent += -scroll * 0.05f;
        }
        if (zoomPercent < 0.5f)
        {
            zoomPercent = 0.5f;
        }

        if (zoomPercent > 1f)
        {
            zoomPercent = 1f;
        }
    }

    IEnumerator DisablePlayerRenderer()
    {
        yield return new WaitForSeconds(renderDelay);
        foreach (Renderer renderer in playerRenderers)
        {
            renderer.enabled = false;
        }
    }

    IEnumerator EnablePlayerRenderer()
    {
        yield return new WaitForSeconds(renderDelay * 0.1f);
        foreach (Renderer renderer in playerRenderers)
        {
            renderer.enabled = true;
        }
    }
}
