using UnityEngine;
using TMPro;
using System.Collections;

public class SpawnPlayerOnEnter : MonoBehaviour
{
    [Header("Пул префабов игрока")]
    [SerializeField] private GameObject[] playerPrefabs;

    [Header("Точка спавна")]
    [SerializeField] private Transform spawnPoint;

    [Header("Партикль при спавне")]
    [SerializeField] private GameObject spawnParticlePrefab;

    [Header("Звук при спавне")]
    [SerializeField] private AudioClip spawnSound;
    [Tooltip("Громкость звука (0–1)")]
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    [Header("UI Текст")]
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private float blinkSpeed = 2f;

    [Header("Настройки спавна")]
    [SerializeField] private bool autoDestroyParticle = true;
    [SerializeField] private float spawnImpulseForce = 10f;
    [Tooltip("Минимальный и максимальный угол поворота")]
    [SerializeField] private float minRotation = -45f;
    [SerializeField] private float maxRotation = 45f;

    private GameObject currentPlayer;
    private bool gameStarted = false;
    private int lastSpawnedIndex = -1;

    void Start()
    {
        // Останавливаем игру до нажатия Enter
        Time.timeScale = 0f;
        Debug.Log("Нажмите ENTER для старта игры!");

        // Запускаем мигание текста
        if (startText != null)
        {
            StartCoroutine(BlinkText());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!gameStarted)
            {
                // Первый запуск игры
                gameStarted = true;
                Time.timeScale = 1f;
                Debug.Log("Игра началась!");

                // Скрываем текст
                if (startText != null)
                {
                    startText.gameObject.SetActive(false);
                }
            }

            if (playerPrefabs == null || playerPrefabs.Length == 0)
            {
                Debug.LogWarning("Пул префабов игрока пуст!");
                return;
            }

            SpawnRandomPlayer();
        }
    }

    IEnumerator BlinkText()
    {
        if (startText == null) yield break;

        Color originalColor = startText.color;

        while (!gameStarted)
        {
            // Плавное мигание от прозрачного к полному
            float alpha = Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1f);
            startText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }

    public void SpawnRandomPlayer()
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        SpawnRandomPlayerAt(position, true);
    }

    public void SpawnRandomPlayerAt(Vector3 position, bool applyImpulse = true)
    {
        // Выбираем случайный префаб, НО не тот же самый что и в прошлый раз
        int selectedIndex;

        if (playerPrefabs.Length == 1)
        {
            // Если префаб всего один, используем его
            selectedIndex = 0;
        }
        else
        {
            // Выбираем случайный индекс, исключая предыдущий
            do
            {
                selectedIndex = Random.Range(0, playerPrefabs.Length);
            }
            while (selectedIndex == lastSpawnedIndex);
        }

        lastSpawnedIndex = selectedIndex;
        GameObject selectedPrefab = playerPrefabs[selectedIndex];

        // Рандомный поворот наискось (по оси Z для 2D)
        float randomRotation = Random.Range(minRotation, maxRotation);
        Quaternion rotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Спавним игрока в указанной позиции с рандомным поворотом
        currentPlayer = Instantiate(selectedPrefab, position, rotation);

        // Применяем импульс в рандомную сторону
        if (applyImpulse)
        {
            Rigidbody2D rb = currentPlayer.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-0.5f, 1f)
                ).normalized;

                rb.AddForce(randomDirection * spawnImpulseForce, ForceMode2D.Impulse);
            }
        }

        // Партикль эффект
        if (spawnParticlePrefab != null)
            SpawnParticle(position);

        // Звук спавна
        if (spawnSound != null)
            PlaySpawnSound();

        Debug.Log($"Игрок заспавнен в позиции: {position}, префаб: {selectedPrefab.name} (#{selectedIndex}), угол: {randomRotation}°");
    }

    private void SpawnParticle(Vector3 position)
    {
        GameObject particle = Instantiate(spawnParticlePrefab, position, Quaternion.identity);

        if (autoDestroyParticle)
        {
            var ps = particle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(particle, duration);
            }
            else
            {
                Destroy(particle, 3f);
            }
        }
    }

    private void PlaySpawnSound()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = spawnSound;
        source.volume = soundVolume;
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D звук

        source.Play();

        // Удаляем компонент после проигрывания
        Destroy(source, spawnSound.length + 0.1f);
    }
}
