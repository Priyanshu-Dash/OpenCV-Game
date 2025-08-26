using UnityEngine;

public class CartCatcher : MonoBehaviour
{
    public CatchEggsManager eggsManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EggOptionTag tag = collision.GetComponent<EggOptionTag>();
        if (tag != null)
        {
            eggsManager.OnEggCaught(tag.value, tag.isCorrect);
            Destroy(collision.gameObject);
        }
    }
}
