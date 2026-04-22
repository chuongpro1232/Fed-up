using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    public Color outlineColor = Color.yellow;
    
    [Tooltip("How thick the outline is (usually 0.05 to 0.1 for pixel art)")]
    public float outlineThickness = 0.05f;

    private SpriteRenderer mainSpriteRenderer;
    private GameObject outlineContainer;

    void Start()
    {
        Debug.Log("SpriteOutline Start() is running on " + gameObject.name);
        mainSpriteRenderer = GetComponent<SpriteRenderer>();
        CreateOutline();
    }

    void CreateOutline()
    {
        // 1. Create an empty object to hold our outline pieces
        outlineContainer = new GameObject("Outline");
        outlineContainer.transform.SetParent(transform);
        outlineContainer.transform.localPosition = Vector3.zero;
        outlineContainer.transform.localScale = Vector3.one;

        // 2. Define the 4 directions (Up, Down, Left, Right)
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(0, outlineThickness, 0),
            new Vector3(0, -outlineThickness, 0),
            new Vector3(-outlineThickness, 0, 0),
            new Vector3(outlineThickness, 0, 0)
        };

        // 3. Create 4 copies of the sprite and push them slightly outward!
        for (int i = 0; i < 4; i++)
        {
            GameObject piece = new GameObject("OutlinePiece_" + i);
            piece.transform.SetParent(outlineContainer.transform);
            piece.transform.localPosition = offsets[i];
            piece.transform.localScale = Vector3.one;

            // Copy the sprite renderer settings
            SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
            sr.sprite = mainSpriteRenderer.sprite;
            sr.color = outlineColor;
            
            // CRITICAL: Copy the exact material so it renders correctly in URP 2D!
            sr.material = mainSpriteRenderer.material;
            
            // Make sure the outline renders BEHIND the main blackboard
            sr.sortingLayerID = mainSpriteRenderer.sortingLayerID;
            sr.sortingOrder = mainSpriteRenderer.sortingOrder - 1; 
        }
    }
}
