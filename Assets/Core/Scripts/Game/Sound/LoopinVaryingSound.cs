using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LoopinVaryingSound : MonoBehaviour
{
    [Header("Клипы")]
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();

    [Header("Повторение")]
    [Tooltip("Интервал между звуками в секундах, пока объект движется")]
    [SerializeField] private float repeatInterval = 0.4f;

    [Header("Питч")]
    [Tooltip("Базовое значение pitch")]
    [SerializeField] private float basePitch = 1f;
    [Tooltip("Случайное отклонение от базового pitch (например 0.1 = 10%)")]
    [SerializeField] private float pitchRange = 0.1f;

    [Header("Движение")]
    [Tooltip("Минимальная скорость, при которой считается, что объект движется")]
    [SerializeField] private float minSpeedToPlay = 0.1f;

    private AudioSource audioSource;
    private Vector3 lastPosition;
    private float timer;
    private int clipIndex;
    private bool isMoving;

    public bool IsMovingExternally { get; set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false; // важный момент: шаги/скольжение — это не один луп, а отдельные сэмплы[web:12]
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Проверяем движение
        Vector3 currentPosition = transform.position;
        float distance = Vector3.Distance(currentPosition, lastPosition);
        float speed = distance / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPosition = currentPosition;

        if (speed >= minSpeedToPlay || IsMovingExternally)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (!isMoving || clips.Count == 0)
        {
            timer = 0f; // сбрасываем таймер, чтобы звук сразу проигрался при возобновлении движения
            return;
        }

        timer += Time.deltaTime;
        if (timer >= repeatInterval)
        {
            timer = 0f;
            PlayNextClip();
        }
    }

    private void PlayNextClip()
    {
        if (clips.Count == 0) return;

        // Циклический переход к следующему клипу
        var clip = clips[clipIndex];
        clipIndex = (clipIndex + 1) % clips.Count;

        // Небольшая случайная вариация питча вокруг basePitch
        float randomOffset = Random.Range(-pitchRange, pitchRange);
        audioSource.pitch = basePitch + randomOffset; // тренд с рандомизацией питча для шагов[web:12]

        audioSource.PlayOneShot(clip);
    }

    // Опционально: публичный метод, если хочешь вызывать звук не по скорости, а внешне (анимация шага и т.п.)
    public void ForcePlayStep()
    {
        timer = 0f;
        PlayNextClip();
    }
}


