using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float yOffset = 3f;

    private Transform player;

    void Start()
    {
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

        // Камера точно следует за игроком
        transform.position = new Vector3(transform.position.x, player.position.y + yOffset, transform.position.z);
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
