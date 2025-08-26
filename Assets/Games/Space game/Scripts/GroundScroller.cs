using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public Transform player;
    public float recycleOffset = 50f; // When to recycle ground pieces
    private Vector3 startPosition;

    public float lifetime = 50f;
    float time = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (player.position.z > transform.position.z + recycleOffset)
        {
            // Recycle ground by moving it ahead
            transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z + 100f);
        }
        if(time > lifetime){
            Destroy(gameObject);
        }
    }
}
