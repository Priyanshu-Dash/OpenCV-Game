using UnityEngine;
using UnityEngine.UI;

public class MagneticGameManager : MonoBehaviour
{
    [Header("Image Components")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    
    [Header("Sprite Arrays")]
    [SerializeField] public Sprite[] magneticSprites;      // Made public for access
    [SerializeField] public Sprite[] nonMagneticSprites;  // Made public for access
    
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
    
    // Public method to check if a sprite is magnetic
    public bool IsSpriteMagnetic(Sprite sprite)
    {
        if (sprite == null)
            return false;
            
        // Check if sprite is in magnetic array
        if (magneticSprites != null)
        {
            foreach (var magneticSprite in magneticSprites)
            {
                if (magneticSprite == sprite)
                    return true;
            }
        }
        
        // Check if sprite is in non-magnetic array
        if (nonMagneticSprites != null)
        {
            foreach (var nonMagneticSprite in nonMagneticSprites)
            {
                if (nonMagneticSprite == sprite)
                    return false;
            }
        }
        
        // Fallback: assume non-magnetic if we can't determine
        return false;
    }
    
    // Public method to reshuffle sprites
    public void ReshuffleSprites()
    {
        AssignRandomSprites();
    }
}
