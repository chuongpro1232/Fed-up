using UnityEngine;

public class BalanceScaleManager : MonoBehaviour
{
    public static BalanceScaleManager Instance;

    [Tooltip("Khoảng trống đặt nguyên tử bên đĩa trái")]
    public Transform leftPanSlot;
    
    [Tooltip("Khoảng trống đặt nguyên tử bên đĩa phải")]
    public Transform rightPanSlot;
    
    [Tooltip("Thanh ngang của cân để bị nghiêng (Cái cân2_0)")]
    public Transform scaleBeam; 
    
    [Tooltip("Góc xoay tối đa khi một bên nặng hơn")]
    public float maxTiltAngle = 15f; 
    
    public float tiltSpeed = 2f;
    
    private float targetAngle = 0f;

    void Awake()
    {
        Instance = this;
        
        // Tự động thêm AtomSlot nếu chưa có
        if (leftPanSlot != null && leftPanSlot.GetComponent<AtomSlot>() == null) leftPanSlot.gameObject.AddComponent<AtomSlot>();
        if (rightPanSlot != null && rightPanSlot.GetComponent<AtomSlot>() == null) rightPanSlot.gameObject.AddComponent<AtomSlot>();
    }

    void Update()
    {
        if (scaleBeam != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            scaleBeam.rotation = Quaternion.Lerp(scaleBeam.rotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
    }

    public void EvaluateScale()
    {
        int leftWeight = 0;
        int rightWeight = 0;

        if (leftPanSlot != null)
        {
            AtomSlot lSlot = leftPanSlot.GetComponent<AtomSlot>();
            if (lSlot != null && lSlot.currentAtom != null) leftWeight = lSlot.currentAtom.weight;
        }

        if (rightPanSlot != null)
        {
            AtomSlot rSlot = rightPanSlot.GetComponent<AtomSlot>();
            if (rSlot != null && rSlot.currentAtom != null) rightWeight = rSlot.currentAtom.weight;
        }

        // Nếu 1 trong 2 đĩa trống hoặc 2 bên bằng nhau thì cân bằng
        if ((leftWeight == 0 && rightWeight == 0) || leftWeight == rightWeight)
        {
            targetAngle = 0f; 
        }
        else if (leftWeight > rightWeight)
        {
            // Bên trái nặng hơn, quay trục Z (quay góc dương để nghiêng trái)
            // Tuy nhiên, tuỳ vào gốc toạ độ của hình bạn, có thể cần đổi chiều âm dương
            targetAngle = maxTiltAngle; 
        }
        else if (rightWeight > leftWeight)
        {
            // Bên phải nặng hơn
            targetAngle = -maxTiltAngle; 
        }
        
        Debug.Log($"[BalanceScale] Left: {leftWeight} | Right: {rightWeight} => Angle: {targetAngle}");
    }
}
