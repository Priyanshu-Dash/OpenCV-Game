using UnityEngine;
using UnityEngine.UI;

public class MagneticGameManager : MonoBehaviour
{
    [Header("Image Components")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    
    [Header("Sprite Arrays")]
    [SerializeField] private Sprite[] magneticSprites;
    [SerializeField] private Sprite[] nonMagneticSprites;
    
    void Start()
    {
        // Check if images are assigned
        if (leftImage == null)
            Debug.LogWarning("Left Image not assigned!");
            
        if (rightImage == null)
            Debug.LogWarning("Right Image not assigned!");
            
        // Randomly assign sprites to images
        AssignRandomSprites();
    }
    
    void AssignRandomSprites()
    {
        // Randomly decide which side gets magnetic and which gets non-magnetic
        bool leftGetsMagnetic = Random.Range(0, 2) == 0;
        
        if (leftImage != null)
        {
            if (leftGetsMagnetic && magneticSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, magneticSprites.Length);
                leftImage.sprite = magneticSprites[randomIndex];
            }
            else if (!leftGetsMagnetic && nonMagneticSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, nonMagneticSprites.Length);
                leftImage.sprite = nonMagneticSprites[randomIndex];
            }
        }
        
        if (rightImage != null)
        {
            if (!leftGetsMagnetic && magneticSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, magneticSprites.Length);
                rightImage.sprite = magneticSprites[randomIndex];
            }
            else if (leftGetsMagnetic && nonMagneticSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, nonMagneticSprites.Length);
                rightImage.sprite = nonMagneticSprites[randomIndex];
            }
        }
    }
}
