using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceForce = 30f; // Огромная сила отталкивания
    public bool bounceOppositeDirection = true; // Отталкивать в противоположную сторону

    [Header("Direction Override (Optional)")]
    public bool useCustomDirection = false;
    public Vector2 customDirection = Vector2.up; // Своё направление если нужно

    [Header("Effects")]
    public AudioClip bounceSound;
    public GameObject bounceEffect; // Партиклы при отталкивании

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 bounceDirection;

                if (useCustomDirection)
                {
                    // Используем заданное направление
                    bounceDirection = customDirection.normalized;
                }
                else if (bounceOppositeDirection)
                {
                    // Отталкиваем в противоположную сторону от точки контакта
                    Vector2 contactPoint = collision.contacts[0].point;
                    bounceDirection = (collision.transform.position - (Vector3)contactPoint).normalized;
                }
                else
                {
                    // Отталкиваем по нормали поверхности
                    bounceDirection = collision.contacts[0].normal;
                }

                // Применяем силу
                playerRb.linearVelocity = bounceDirection * bounceForce;

                // Эффекты
                if (bounceSound != null)
                {
                    AudioSource.PlayClipAtPoint(bounceSound, transform.position);
                }

                if (bounceEffect != null)
                {
                    Instantiate(bounceEffect, collision.contacts[0].point, Quaternion.identity);
                }
            }
        }
    }
}
