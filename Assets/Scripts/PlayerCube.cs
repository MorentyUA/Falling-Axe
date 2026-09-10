using UnityEngine;
using System.Collections;

public class PlayerCube : MonoBehaviour
{
    [Header("Physics Settings")]
    public float fallGravity = 2f;
    public float angularDrag = 0.5f;

    [Header("Self-Explosion Settings")]
    public float idleTimeToIgnite = 2f;
    public float explosionRadius = 3f;
    public float explosionForce = 20f;
    [Range(0.3f, 0.8f)]
    public float horizontalRandomness = 0.5f;
    public Color normalColor = Color.white;
    public Color ignitedColor = Color.red;

    [Header("Effects")]
    public GameObject explosionParticles;
    public AudioClip igniteSound;
    public AudioClip explosionSound;

    [Header("Respawn Settings")]
    public bool respawnAfterExplosion = true;
    public float respawnDelay = 0.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float idleTimer = 0f;
    private bool isIgnited = false;
    private bool isExploding = false;
    private Vector3 originalScale;
    private Color originalColor;
    private const float velocityThreshold = 0.1f;
    private SpawnPlayerOnEnter spawner;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = fallGravity;
        rb.angularDamping = angularDrag;
        rb.constraints = RigidbodyConstraints2D.None;

        originalScale = transform.localScale;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Находим спавнер сразу при старте
        spawner = FindFirstObjectByType<SpawnPlayerOnEnter>();
        if (spawner == null)
        {
            Debug.LogWarning("SpawnPlayerOnEnter не найден на сцене!");
        }
    }

    void Update()
    {
        // Проверяем, стоит ли куб на месте
        if (rb.linearVelocity.magnitude < velocityThreshold && !isIgnited && !isExploding)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeToIgnite)
            {
                StartIgnition();
            }
        }
        else if (!isIgnited)
        {
            idleTimer = 0f;
        }
    }

    void StartIgnition()
    {
        if (isIgnited) return;

        isIgnited = true;
        PlaySoundAtPosition(igniteSound, transform.position);
        StartCoroutine(IgnitionEffect());
    }

    IEnumerator IgnitionEffect()
    {
        float ignitionDuration = 1f;
        float timer = 0f;

        while (timer < ignitionDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.PingPong(timer * 8f, 1f);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(originalColor, ignitedColor, t);
            }

            float scale = 1f + (timer / ignitionDuration) * 0.15f;
            transform.localScale = originalScale * scale;

            yield return null;
        }

        Explode();
    }

    void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        Vector3 explosionPosition = transform.position;

        // Создаём эффект взрыва
        if (explosionParticles != null)
        {
            Instantiate(explosionParticles, explosionPosition, Quaternion.identity);
        }

        PlaySoundAtPosition(explosionSound, explosionPosition);

        // Находим все объекты в радиусе взрыва
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // Пропускаем самого себя
            if (hitCollider.gameObject == gameObject)
                continue;

            DestructibleCube cube = hitCollider.GetComponent<DestructibleCube>();
            if (cube != null)
            {
                // Брейнрот — неприкасаемый
                if (cube.oreType == OreType.Brainrot)
                    continue;

                // Если это взрывчатый куб — цепная реакция
                if (cube.isExplosive)
                {
                    cube.InstantExplode();
                }
                else
                {
                    // Наносим 11 урона
                    for (int i = 0; i < 11; i++)
                    {
                        cube.TakeDamage();
                    }
                }
            }
        }

        // Респавн нового игрока через временный объект
        if (respawnAfterExplosion && spawner != null)
        {
            GameObject tempObject = new GameObject("RespawnTimer");
            RespawnTimer timer = tempObject.AddComponent<RespawnTimer>();
            timer.Initialize(spawner, respawnDelay, explosionPosition);
        }

        // Уничтожаем текущего игрока
        Destroy(gameObject);
    }

    // Вспомогательный класс для респавна
    private class RespawnTimer : MonoBehaviour
    {
        private SpawnPlayerOnEnter spawner;
        private float delay;
        private Vector3 respawnPosition;

        public void Initialize(SpawnPlayerOnEnter spawnerRef, float respawnDelay, Vector3 position)
        {
            spawner = spawnerRef;
            delay = respawnDelay;
            respawnPosition = position;
            StartCoroutine(RespawnAfterDelay());
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(delay);

            if (spawner != null)
            {
                // Спавним с импульсом (параметр true)
                spawner.SpawnRandomPlayerAt(respawnPosition, true);
                Debug.Log($"Новый игрок заrespawnен на месте взрыва: {respawnPosition}");
            }
            else
            {
                Debug.LogError("Spawner потерян! Невозможно заrespawnить игрока.");
            }

            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем на разрушаемые блоки
        if (collision.gameObject.CompareTag("Destructible") || collision.gameObject.CompareTag("Brainrot"))
        {
            DestructibleCube cube = collision.gameObject.GetComponent<DestructibleCube>();
            if (cube != null)
            {
                cube.TakeDamage();
            }
        }
    }

    void PlaySoundAtPosition(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        GameObject tempAudio = new GameObject("TempAudio_" + clip.name);
        tempAudio.transform.position = position;

        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 0f;
        audioSource.Play();

        Destroy(tempAudio, clip.length);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
