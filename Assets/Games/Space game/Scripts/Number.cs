using UnityEngine;

public class Number : MonoBehaviour
{
    public int value; // Number value
    public TextMesh textMesh; // TextMesh to display the number
    private BoxCollider boxCollider;
    public float destroyDistance = 10f; // Distance beyond which the number is destroyed

    private Transform player;
    public bool IsValid = false;

    public float lifetime = 50f;
    float time = 0f;

    [Header("Settings")]
    public int maxFontSize = 30; // Maximum allowed font size
    public float maxTextWidth = 5f; // Maximum allowed width for the text collider
    public Color outlineColor = Color.black; // Color of the outline
    public float outlineWidth = 0.1f; // Width of the outline
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boxCollider = GetComponent<BoxCollider>();

        if (textMesh != null)
        {
            AdjustTextSize();
        }

        if (textMesh != null && boxCollider != null)
        {
            ResizeCollider();
        }
    }

    void AdjustTextSize()
    {
        if (textMesh == null) return;

        // Temporarily store the original font size
        int originalFontSize = textMesh.fontSize;

        // Start with the maximum font size
        textMesh.fontSize = maxFontSize;

        // Force the TextMesh to update its bounds
        textMesh.GetComponent<Renderer>().enabled = false; // Temporarily disable rendering
        textMesh.GetComponent<Renderer>().enabled = true;  // Re-enable rendering to refresh bounds

        // Reduce font size until the text fits within the maximum width
        while (textMesh.GetComponent<Renderer>().bounds.size.x > maxTextWidth && textMesh.fontSize > 1)
        {
            textMesh.fontSize -= 1;
            textMesh.GetComponent<Renderer>().enabled = false; // Force refresh again
            textMesh.GetComponent<Renderer>().enabled = true;
        }

        // If the font size ends up at 0, revert to the original size as a safety measure
        if (textMesh.fontSize <= 0)
        {
            Debug.LogWarning("Font size ended up at 0! Reverting to original size.");
            textMesh.fontSize = originalFontSize;
        }
        AddTextOutline();
    }

    void AddTextOutline()
    {
        if (textMesh == null) return;

        // Create 4 duplicate TextMesh objects for the outline
        for (int i = 0; i < 4; i++)
        {
            GameObject outline = Instantiate(textMesh.gameObject, textMesh.transform.parent);
            TextMesh outlineTextMesh = outline.GetComponent<TextMesh>();

            // Set the outline color
            outlineTextMesh.color = outlineColor;

            // Position offsets for the outline
            Vector3 offset = Vector3.zero;
            switch (i)
            {
                case 0: offset = new Vector3(outlineWidth, 0, 0); break; // Right
                case 1: offset = new Vector3(-outlineWidth, 0, 0); break; // Left
                case 2: offset = new Vector3(0, outlineWidth, 0); break; // Top
                case 3: offset = new Vector3(0, -outlineWidth, 0); break; // Bottom
            }

            // Adjust the position
            outline.transform.localPosition += offset;

            // Ensure outline is rendered behind the main text
            outlineTextMesh.GetComponent<Renderer>().sortingOrder = textMesh.GetComponent<Renderer>().sortingOrder - 1;
        }
    }
    void ResizeCollider()
    {
        // Get the bounds of the text
        Renderer textRenderer = textMesh.GetComponent<Renderer>();
        if (textRenderer != null)
        {
            Bounds textBounds = textRenderer.bounds;

            // Set the BoxCollider size and center to match the text bounds
            boxCollider.size = new Vector3(textBounds.size.x, textBounds.size.y, 0.5f); // Adjust Z-axis as needed
            boxCollider.center = new Vector3(0, 0, 0); // Ensure the collider is centered
        }
    }

    public void SetValue(int number)
    {
        value = number;
        textMesh.text = value.ToString();
    }
    public void SetValue(string s)
    {
        textMesh.text = s;
    }

    void Update()
    {
        time += Time.deltaTime;
        // Destroy the number if it's behind the player
        if (transform.position.z < player.position.z - destroyDistance)
        {
            Destroy(gameObject);
        }
        if (time > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CheckNumber(IsValid);
            Destroy(gameObject);
        }
    }
}
