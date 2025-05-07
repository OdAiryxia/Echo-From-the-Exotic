using UnityEngine;
using System.Collections;

public class OiiaCat : MonoBehaviour
{
    [Header("旋轉設定")]
    public float slowRotationSpeed = 360f;
    public float fastRotationSpeed = 960f;
    public float slowDuration = 2.2f; // 慢速模式持續時間
    public float fastDuration = 1.7f; // 快速模式持續時間
    public float pauseDuration = 2.5f; // 兩種模式之間的暫停時間

    [Header("旋轉軸向")]
    public Vector3 rotationAxis = Vector3.up; // 預設Y軸旋轉

    [Header("音效設定")]
    public AudioClip slowModeSound;
    public AudioClip fastModeSound;

    [Tooltip("音效音量")]
    [Range(0f, 1f)]
    public float volume = 0.2f;

    private AudioSource audioSource;
    private bool isPlaying = false;

    void Start()
    {
        // 確保有AudioSource組件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = volume;

        // 開始循環動畫
        StartCoroutine(AnimationLoop());
    }

    // 主循環動畫
    IEnumerator AnimationLoop()
    {
        while (true)
        {
            // 進入慢速模式
            yield return StartCoroutine(RotateWithMode(slowRotationSpeed, slowDuration, slowModeSound));

            // 暫停
            yield return new WaitForSeconds(pauseDuration);

            // 進入快速模式
            yield return StartCoroutine(RotateWithMode(fastRotationSpeed, fastDuration, fastModeSound));

            // 暫停
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    // 按指定速度旋轉並播放音效
    IEnumerator RotateWithMode(float speed, float duration, AudioClip sound)
    {
        // 播放音效
        PlaySound(sound);

        float timer = 0f;
        while (timer < duration)
        {
            // 旋轉物體
            transform.Rotate(rotationAxis * speed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        // 停止音效
        StopSound();
        transform.rotation = Quaternion.identity;
    }

    // 播放音效
    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // 停止音效
    void StopSound()
    {
        audioSource.Stop();
    }

    // 允許在運行時暫停/繼續動畫
    public void ToggleAnimation()
    {
        if (isPlaying)
        {
            StopAllCoroutines();
            StopSound();
        }
        else
        {
            StartCoroutine(AnimationLoop());
        }
        isPlaying = !isPlaying;
    }
}