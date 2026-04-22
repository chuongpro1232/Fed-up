using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MoleculeAtom : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private AtomHook draggingHook = null;
    
    [Tooltip("Danh sách các móc câu của atom này. Tự động tìm khi Start.")]
    public List<AtomHook> hooks = new List<AtomHook>();

    [Header("Audio")]
    [Tooltip("Âm thanh khi nhấc nguyên tử lên")]
    public AudioClip grabSound;
    [Tooltip("Âm thanh khi thả nguyên tử xuống")]
    public AudioClip dropSound;

    void Start()
    {
        // Vô hiệu hóa va chạm vật lý để các khối không bị đẩy văng ra khi ghép
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // Tự động gán tất cả AtomHook là con của GameObject này
        hooks.AddRange(GetComponentsInChildren<AtomHook>());
        foreach(var hook in hooks)
        {
            hook.parentAtom = this;
        }
    }

    void OnMouseDown()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Kiểm tra xem chuột có click trúng con chuột móc câu nào không
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePosition);
        foreach(var hit in hits)
        {
            AtomHook hook = hit.GetComponent<AtomHook>();
            if (hook != null)
            {
                // Nếu click trúng móc câu, nhường quyền điều khiển cho móc câu ngay lập tức
                draggingHook = hook;
                hook.ManualOnMouseDown(mousePosition);
                return; 
            }
        }

        draggingHook = null;
        
        // Chuẩn bị trạng thái kéo cho toàn bộ phân tử
        offset = transform.position - mousePosition;
        foreach(var atom in GetConnectedAtoms())
        {
            atom.PrepareDrag(mousePosition);
        }

        if (grabSound != null)
        {
            AudioSource.PlayClipAtPoint(grabSound, Camera.main.transform.position);
        }
    }

    void OnMouseDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Nếu đang kéo móc câu thông qua Atom
        if (draggingHook != null)
        {
            draggingHook.ManualOnMouseDrag(mousePosition);
            return;
        }

        // Kéo phân tử
        foreach(var atom in GetConnectedAtoms())
        {
            atom.DoDrag(mousePosition);
        }
    }

    void OnMouseUp()
    {
        if (draggingHook != null)
        {
            draggingHook.ManualOnMouseUp();
            draggingHook = null;
            return;
        }

        foreach(var atom in GetConnectedAtoms())
        {
            atom.EndDrag();
        }

        if (dropSound != null)
        {
            AudioSource.PlayClipAtPoint(dropSound, Camera.main.transform.position);
        }
    }

    public void PrepareDrag(Vector3 mousePos)
    {
        offset = transform.position - mousePos;
        isDragging = true;
    }

    public void DoDrag(Vector3 mousePos)
    {
        if (isDragging)
        {
            transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
        }
    }

    public void EndDrag()
    {
        isDragging = false;
    }

    public List<MoleculeAtom> GetConnectedAtoms()
    {
        HashSet<MoleculeAtom> visited = new HashSet<MoleculeAtom>();
        Queue<MoleculeAtom> queue = new Queue<MoleculeAtom>();
        
        queue.Enqueue(this);
        visited.Add(this);

        while(queue.Count > 0)
        {
            var curr = queue.Dequeue();
            foreach(var hook in curr.hooks)
            {
                if(hook.connectedHook != null && hook.connectedHook.parentAtom != null)
                {
                    if(!visited.Contains(hook.connectedHook.parentAtom))
                    {
                        visited.Add(hook.connectedHook.parentAtom);
                        queue.Enqueue(hook.connectedHook.parentAtom);
                    }
                }
            }
        }
        return new List<MoleculeAtom>(visited);
    }
}
