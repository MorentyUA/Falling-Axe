using UnityEngine;

public class FollowPlayerDown : MonoBehaviour
{
    [Header("Follow Settings")]
    public float yOffset = 0f;
    public float followDistance = 5f;

    [Header("Player Knockback")]
    public float knockbackForce = 25f;
    [Range(0.3f, 0.8f)]
    public float horizontalRandomness = 0.5f;

    [Header("Sound Settings")]
    public AudioClip hitSound;
    private AudioSource audioSource;

    private Transform player;
    private float fixedX;
    private float fixedZ;

    void Start()
    {
        fixedX = transform.position.x;
        fixedZ = transform.position.z;

        // Настройка AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        FindPlayer();
    }

    void LateUpdate()
    {
        // Если игрок потерян, пытаемся найти снова
        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distance = transform.position.y - player.position.y;

        if (distance > followDistance)
        {
            float targetY = player.position.y + yOffset + followDistance;
            transform.position = new Vector3(fixedX, targetY, fixedZ);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Уничтожение блоков мгновенно
        if (other.CompareTag("Destructible"))
        {
            DestructibleCube cube = other.GetComponent<DestructibleCube>();
            if (cube != null)
            {
                Destroy(other.gameObject);
            }
        }

        // Отбрасывание игрока
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Направление вниз с рандомным отклонением вбок
                float randomX = Random.Range(-horizontalRandomness, horizontalRandomness);
                Vector2 knockbackDirection = new Vector2(randomX, -1f).normalized;

                playerRb.linearVelocity = knockbackDirection * knockbackForce;

                // Воспроизводим звук удара
                if (hitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"{gameObject.name}: Игрок найден!");
        }
    }
}
