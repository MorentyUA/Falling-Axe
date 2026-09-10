using UnityEngine;
using System.Collections;

public enum OreType
{
    None,
    Brainrot,
    Coal,
    Copper,
    Iron,
    Gold,
    Lapis,
    Redstone,
    Diamond,
    Emerald
}

public class DestructibleCube : MonoBehaviour
{
    [Header("Тип руды")]
    public OreType oreType = OreType.None;

    [Header("Cube Settings")]
    public int hitsToDestroy = 1;
    public Color normalColor = Color.white;
    public Color damagedColor = Color.red;

    [Header("Explosive Cube")]
    public bool isExplosive = false;
    public float explosionDelay = 2f;
    public float explosionRadius = 3f;
    public float explosionForce = 20f;
    [Range(0.3f, 0.8f)]
    public float horizontalRandomness = 0.5f;
    public Color explosiveColor = Color.yellow;
    public Color ignitedColor = Color.red;

    [Header("Effects")]
    public GameObject destructionParticles;
    public GameObject explosionParticles;
    public AudioClip destructionSound;
    public AudioClip hitSound;
    public AudioClip igniteSound;
    public AudioClip explosionSound;

    private int currentHits = 0;
    private SpriteRenderer spriteRenderer;
    private bool isIgnited = false;
    private bool isExploded = false;
    private Vector3 originalScale;
    private bool destroyedByPlayer = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;

        if (spriteRenderer != null)
        {
            if (isExplosive)
            {
                spriteRenderer.color = explosiveColor;
            }
            else
            {
                spriteRenderer.color = normalColor;
            }
        }
    }

    public void TakeDamage()
    {
        destroyedByPlayer = true; // Помечаем что урон от игрока
        currentHits++;

        if (isExplosive && !isIgnited && !isExploded)
        {
            IgniteExplosive();
            return;
        }

        if (currentHits >= hitsToDestroy)
        {
            DestroyWithEffects();
        }
        else
        {
            if (spriteRenderer != null)
            {
                float t = (float)currentHits / hitsToDestroy;
                spriteRenderer.color = Color.Lerp(normalColor, damagedColor, t);
            }

            PlaySoundAtPosition(hitSound, transform.position);
        }
    }

    public void InstantExplode()
    {
        if (isExploded) return;

        destroyedByPlayer = false; // Уничтожен взрывом

        if (isExplosive)
        {
            StopAllCoroutines();
            Explode();
        }
        else
        {
            // Брейнрот блоки НЕ уничтожаются взрывом!
            if (oreType == OreType.Brainrot)
            {
                return; // Просто выходим, не уничтожаем
            }

            DestroyWithEffects();
        }
    }

    void IgniteExplosive()
    {
        isIgnited = true;
        PlaySoundAtPosition(igniteSound, transform.position);
        StartCoroutine(ExplosionCountdown());
    }

    IEnumerator ExplosionCountdown()
    {
        float timer = 0f;

        while (timer < explosionDelay)
        {
            timer += Time.deltaTime;

            float t = Mathf.PingPong(timer * 5f, 1f);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(explosiveColor, ignitedColor, t);
            }

            float scale = 1f + (timer / explosionDelay) * 0.2f;
            transform.localScale = originalScale * scale;

            yield return null;
        }

        Explode();
    }

    void Explode()
    {
        if (isExploded) return;
        isExploded = true;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            DestructibleCube cube = hitCollider.GetComponent<DestructibleCube>();
            if (cube != null && cube != this)
            {
                if (cube.isExplosive)
                {
                    cube.InstantExplode();
                }
                else
                {
                    cube.InstantExplode();
                }
            }

            if (hitCollider.CompareTag("Player"))
            {
                Rigidbody2D playerRb = hitCollider.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 direction = (hitCollider.transform.position - transform.position).normalized;
                    float randomX = Random.Range(-horizontalRandomness, horizontalRandomness);

                    if (Mathf.Abs(direction.x) < 0.2f)
                    {
                        direction.x = Random.value > 0.5f ? 0.5f : -0.5f;
                    }

                    direction.x += randomX;
                    direction.Normalize();

                    playerRb.linearVelocity = direction * explosionForce;
                }
            }
        }

        if (explosionParticles != null)
        {
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
        }

        PlaySoundAtPosition(explosionSound, transform.position);

        // Добавляем очки за взорванный блок
        AddScoreForOre();

        Destroy(gameObject);
    }

    void DestroyWithEffects()
    {
        if (destructionParticles != null)
        {
            Instantiate(destructionParticles, transform.position, Quaternion.identity);
        }

        PlaySoundAtPosition(destructionSound, transform.position);

        // Добавляем очки за уничтоженную руду
        AddScoreForOre();

        Destroy(gameObject);
    }

    void AddScoreForOre()
    {
        if (ScoreManager.Instance == null) return;

        // Брейнрот блоки добавляют очки ТОЛЬКО если уничтожены игроком
        if (oreType == OreType.Brainrot)
        {
            if (destroyedByPlayer)
            {
                ScoreManager.Instance.AddBrainrot();
            }
            return;
        }

        // Все остальные руды добавляют очки всегда (и от игрока, и от TNT)
        switch (oreType)
        {
            case OreType.Coal:
                ScoreManager.Instance.AddCoal();
                break;

            case OreType.Copper:
                ScoreManager.Instance.AddCopper();
                break;

            case OreType.Iron:
                ScoreManager.Instance.AddIron();
                break;

            case OreType.Gold:
                ScoreManager.Instance.AddGold();
                break;

            case OreType.Lapis:
                ScoreManager.Instance.AddLapis();
                break;

            case OreType.Redstone:
                ScoreManager.Instance.AddRedstone();
                break;

            case OreType.Diamond:
                ScoreManager.Instance.AddDiamond();
                break;

            case OreType.Emerald:
                ScoreManager.Instance.AddEmerald();
                break;
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
        if (isExplosive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
