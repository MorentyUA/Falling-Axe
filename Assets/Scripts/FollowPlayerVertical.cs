using UnityEngine;

public class FollowPlayerVertical : MonoBehaviour
{
    public float yOffset = 0f;

    private Transform player;
    private float fixedX;
    private float fixedZ;

    void Start()
    {
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
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

        // Следуем за игроком
        transform.position = new Vector3(fixedX, player.position.y + yOffset, fixedZ);
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
