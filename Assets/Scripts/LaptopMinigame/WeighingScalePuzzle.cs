using UnityEngine;
using System.Collections.Generic;

public class AtomSlot : MonoBehaviour
{
    public DraggableAtom currentAtom;
}

public class WeighingScalePuzzle : MonoBehaviour
{
    public static WeighingScalePuzzle Instance;

    [Tooltip("The 6 slots of the Ranking board, from left to right.")]
    public List<Transform> slots = new List<Transform>(); 
    public Canvas winCanvas;

    void Awake()
    {
        Instance = this;
        if (winCanvas != null)
        {
            winCanvas.gameObject.SetActive(false);
        }
        
        // Ensure slots have AtomSlot component
        foreach(var slot in slots)
        {
            if (slot.GetComponent<AtomSlot>() == null)
            {
                slot.gameObject.AddComponent<AtomSlot>();
            }
        }
    }

    public void CheckWinCondition()
    {
        // Đã gỡ bỏ tính năng phải xem hết troll giao diện mới được tính là xếp đúng (để dễ test)

        bool win = true;
        int previousWeight = -1;

        if (slots.Count < 6) return;

        foreach (var slot in slots)
        {
            AtomSlot s = slot.GetComponent<AtomSlot>();
            if (s == null || s.currentAtom == null)
            {
                win = false;
                break;
            }

            if (s.currentAtom.weight <= previousWeight)
            {
                win = false;
                break;
            }
            previousWeight = s.currentAtom.weight;
        }

        if (win && winCanvas != null)
        {
            if (LaptopGameManager.Instance != null) LaptopGameManager.Instance.CurrentState = 6;
            winCanvas.gameObject.SetActive(true);
        }
    }
}
