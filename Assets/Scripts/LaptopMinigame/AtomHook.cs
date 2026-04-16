using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class AtomHook : MonoBehaviour
{
    [HideInInspector] public MoleculeAtom parentAtom;
    [HideInInspector] public AtomHook connectedHook = null;
    
    [Header("Cài đặt")]
    [Tooltip("Khoảng cách hít dính để tự động nối 2 móc câu lại với nhau")]
    public float snapDistance = 1.0f;
    
    private bool isDragging = false;
    private float radius = 0f;
    private float initialAngleOffset = 0f;

    void Start()
    {
        // Tránh bị va chạm đẩy nhau ra xa
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        if (transform.parent != null)
        {
            Vector3 diff = transform.position - transform.parent.position;
            diff.z = 0;
            radius = diff.magnitude;
            
            if (radius > 0.0001f)
            {
                // Lưu lại độ lệch góc mặc định ban đầu của hình ảnh sprite so với trục X
                float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                initialAngleOffset = transform.eulerAngles.z - angle;
            }
        }
    }

    public void ManualOnMouseDown(Vector3 mousePos)
    {
        Disconnect();
        isDragging = true;
    }

    void OnMouseDown()
    {
        ManualOnMouseDown(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void ManualOnMouseDrag(Vector3 mousePos)
    {
        if (isDragging && parentAtom != null)
        {
            mousePos.z = 0;
            Vector3 dir = mousePos - parentAtom.transform.position;
            dir.z = 0;

            if (dir.sqrMagnitude > 0.001f)
            {
                dir.Normalize();
                
                // Toán học tuyệt đối giúp cho hình ảnh quay quanh tâm không bao giờ bị lệch
                transform.position = parentAtom.transform.position + dir * radius;
                
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, 0, angle + initialAngleOffset);
            }
        }
    }

    void OnMouseDrag()
    {
        ManualOnMouseDrag(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void ManualOnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        
        AtomHook[] allHooks = Object.FindObjectsByType<AtomHook>(FindObjectsSortMode.None);
        AtomHook closest = null;
        float minDist = snapDistance;

        foreach(var h in allHooks)
        {
            if (h != this && h.parentAtom != this.parentAtom && h.connectedHook == null)
            {
                float dist = Vector2.Distance(transform.position, h.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = h;
                }
            }
        }

        if (closest != null)
        {
            ConnectTo(closest);
        }
    }

    void OnMouseUp()
    {
        ManualOnMouseUp();
    }

    public void ConnectTo(AtomHook other)
    {
        // Tính toán hướng chĩa ra của móc câu ĐÍCH (cái đang đứng yên)
        Vector3 dirOther = other.transform.position - other.parentAtom.transform.position;
        dirOther.z = 0;
        if (dirOther.sqrMagnitude < 0.0001f) dirOther = Vector3.right;
        dirOther.Normalize();
        
        // Móc câu này PHẢI chĩa cắm ngược lại 180 độ vào móc câu kia để tạo 1 đường thẳng hoàn hảo
        Vector3 dirThis = -dirOther;
        
        // Vị trí lý tưởng của cái Atom đang cầm để 2 móc câu vừa chạm đỉnh nhau
        Vector3 idealAtomPos = other.transform.position - dirThis * this.radius;
        
        Vector3 offset = idealAtomPos - this.parentAtom.transform.position;
        offset.z = 0;
        
        // Dịch chuyển toàn bộ cụm Molecule hiện tại
        List<MoleculeAtom> myGroup = this.parentAtom.GetConnectedAtoms();
        if (!myGroup.Contains(other.parentAtom))
        {
            foreach(var atom in myGroup)
            {
                atom.transform.position += offset;
            }
        }

        // Khóa chết góc xoay và vị trí đỉnh của móc câu này cho khớp tuyệt đối với cái móc đang đứng im
        this.transform.position = other.transform.position;
        float finalAngle = Mathf.Atan2(dirThis.y, dirThis.x) * Mathf.Rad2Deg;
        this.transform.eulerAngles = new Vector3(0, 0, finalAngle + this.initialAngleOffset);

        this.connectedHook = other;
        other.connectedHook = this;

        // Dàn trải các gậy nối nếu có liên kết đôi hoặc liên kết ba
        OrganizeBondsBetween(this.parentAtom, other.parentAtom);
    }

    public void Disconnect()
    {
        if (connectedHook != null)
        {
            MoleculeAtom otherAtom = connectedHook.parentAtom;
            connectedHook.connectedHook = null;
            this.connectedHook = null;
            
            OrganizeBondsBetween(this.parentAtom, otherAtom);
        }
    }

    // Hàm tự động dàn đều các đường nối thành liên kết song song
    public static void OrganizeBondsBetween(MoleculeAtom atomA, MoleculeAtom atomB)
    {
        if (atomA == null || atomB == null) return;

        List<AtomHook> hooksA = new List<AtomHook>();
        List<AtomHook> hooksB = new List<AtomHook>();

        foreach (var h in atomA.hooks)
        {
            if (h.connectedHook != null && h.connectedHook.parentAtom == atomB)
            {
                hooksA.Add(h);
                hooksB.Add(h.connectedHook); // Luôn tương ứng chéo nhau
            }
        }

        int count = hooksA.Count;
        if (count == 0) return;

        Vector3 baseDir = (atomB.transform.position - atomA.transform.position);
        baseDir.z = 0;
        if (baseDir.sqrMagnitude < 0.0001f) baseDir = Vector3.right;
        baseDir.Normalize();

        Vector3 tangent = new Vector3(-baseDir.y, baseDir.x, 0); // Trục vuông góc
        float spacing = 0.2f; // Khoảng cách giãn ra giữa các đoạn nối (liên kết đôi/ba)

        for (int i = 0; i < count; i++)
        {
            var hA = hooksA[i];
            var hB = hooksB[i];

            Vector3 offset = Vector3.zero;
            if (count == 2)
            {
                offset = (i == 0) ? tangent * spacing : -tangent * spacing;
            }
            else if (count == 3)
            {
                if (i == 1) offset = tangent * spacing;
                if (i == 2) offset = -tangent * spacing;
            }

            // Gán lại vị trí cho móc hA (tịnh tiến theo trục vuông góc)
            hA.transform.position = atomA.transform.position + baseDir * hA.radius + offset;
            float angleA = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
            hA.transform.eulerAngles = new Vector3(0, 0, angleA + hA.initialAngleOffset);

            // Gán lại vị trí cho móc hB (chĩa ngược lại hA)
            hB.transform.position = hA.transform.position; // Nối cứng vào hA
            float angleB = Mathf.Atan2(-baseDir.y, -baseDir.x) * Mathf.Rad2Deg;
            hB.transform.eulerAngles = new Vector3(0, 0, angleB + hB.initialAngleOffset);
        }
    }
}
