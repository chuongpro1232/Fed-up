using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DraggableAtom : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 startPosition;

    [Tooltip("Weight value from 1 to 6. Put a smaller number for lighter atoms.")]
    public int weight = 1; 
    public Transform currentSlot;

    void Start()
    {
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - mousePosition;
        isDragging = true;
        
        // Unparent when we pick it up
        transform.SetParent(null);
        
        if (currentSlot != null)
        {
            AtomSlot slotInfo = currentSlot.GetComponent<AtomSlot>();
            if (slotInfo != null) slotInfo.currentAtom = null;
            currentSlot = null;
            
            if (BalanceScaleManager.Instance != null) BalanceScaleManager.Instance.EvaluateScale();
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        
        AtomSlot[] allSlots = FindObjectsOfType<AtomSlot>();
        AtomSlot closestSlot = null;
        float minDistance = 1.5f; // Giảm lại khoảng hít để tránh click là bị hút lên bục ngay lập tức.

        foreach (var slot in allSlots)
        {
            if (slot.currentAtom == null)
            {
                float dist = Vector2.Distance(transform.position, slot.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closestSlot = slot;
                }
            }
        }

        if (closestSlot != null)
        {
            transform.position = new Vector3(closestSlot.transform.position.x, closestSlot.transform.position.y, transform.position.z);
            currentSlot = closestSlot.transform;
            closestSlot.currentAtom = this;
            
            // Gắn vào slot để nếu slot có mấp mô (cái cân nghiêng) thì atom đi theo
            transform.SetParent(closestSlot.transform);
            
            if (WeighingScalePuzzle.Instance != null) WeighingScalePuzzle.Instance.CheckWinCondition();
            if (BalanceScaleManager.Instance != null) BalanceScaleManager.Instance.EvaluateScale();
        }
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
    }
}
