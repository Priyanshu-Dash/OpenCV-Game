using UnityEngine;
using UnityEngine.UI;

public class Simple2DGame : MonoBehaviour
{
    [Header("Image Components")]
    public Image image1;
    public Image image2;
    
    [Header("Sprite Arrays")]
    public Sprite[] magneticSprites;
    public Sprite[] nonMagneticSprites;
    
    private void Start()
    {
        AssignRandomSprites();
    }
    
    private void AssignRandomSprites()
    {
        // Check if arrays have sprites
        if (magneticSprites.Length == 0 || nonMagneticSprites.Length == 0)
        {
            Debug.LogWarning("Please add sprites to both magnetic and non-magnetic arrays!");
            return;
        }
        
        // Randomly decide which image gets magnetic and which gets non-magnetic
        bool image1GetsMagnetic = Random.Range(0, 2) == 0;
        
        if (image1GetsMagnetic)
        {
            // Image1 gets magnetic sprite, Image2 gets non-magnetic sprite
            if (image1 != null)
            {
                image1.sprite = magneticSprites[Random.Range(0, magneticSprites.Length)];
            }
            if (image2 != null)
            {
                image2.sprite = nonMagneticSprites[Random.Range(0, nonMagneticSprites.Length)];
            }
        }
        else
        {
            // Image1 gets non-magnetic sprite, Image2 gets magnetic sprite
            if (image1 != null)
            {
                image1.sprite = nonMagneticSprites[Random.Range(0, nonMagneticSprites.Length)];
            }
            if (image2 != null)
            {
                image2.sprite = magneticSprites[Random.Range(0, magneticSprites.Length)];
            }
        }
    }
}
