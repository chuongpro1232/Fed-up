using UnityEngine;
using UnityEngine.SceneManagement;

public class LaptopGameManager : MonoBehaviour
{
    public static LaptopGameManager Instance;

    [Header("Đồ Họa Game Cân (State 3)")]
    [Tooltip("Bạn hãy kéo thả TẤT CẢ các đồ nghề của Game Cân (Atoms, Ranking, Cái cân, Chữ, v.v...) vào đây để tớ giấu chúng đi khi không cần thiết")]
    public GameObject[] state3Graphics;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void Start()
    {
        // Tự động bật đúng các Component sự kiện dựa trên State để phòng trường hợp người dùng lỡ tay TẮT chúng ở ngoài Inspector
        int state = CurrentState;


        // Quét tìm tất cả các script trong scene (kể cả những script đang nằm trên GameObject bị tắt)
        SteamIconEvent[] steams = Resources.FindObjectsOfTypeAll<SteamIconEvent>();
        foreach (var s in steams)
        {
            if (s.gameObject.scene.isLoaded)
            {
                bool active = (state == 1);
                s.gameObject.SetActive(active);
                // Ép tắt luôn cả cái mảng background bị vứt riêng lẻ bên ngoài
                if (s.steamBackground != null) s.steamBackground.SetActive(active);
            }
        }

        TrollJoingameController[] trolls = Resources.FindObjectsOfTypeAll<TrollJoingameController>();
        foreach (var t in trolls)
        {
            if (t.gameObject.scene.isLoaded)
            {
                bool active = (state == 2 || state == 3); // Cần sống ở cả State 2 và 3 để thao tác bảng CSGO bự
                t.gameObject.SetActive(active);
                // Ép tắt luôn cả tập đoàn rác quấy rối nếu không phải là State của nó
                if (!active)
                {
                    if (t.joingameParent != null) t.joingameParent.gameObject.SetActive(false);
                    if (t.csgoBackground != null) t.csgoBackground.SetActive(false);
                    if (t.exitButton != null) t.exitButton.gameObject.SetActive(false);
                }
            }
        }

        // Ẩn/Hiện riêng các đồ vật của Game Cân (chỉ hiển thị ở State 3 và sau đó)
        bool showPuzzle = (state >= 3);
        if (state3Graphics != null)
        {
            foreach (var go in state3Graphics)
            {
                if (go != null) go.SetActive(showPuzzle);
            }
        }
    }

    public int CurrentState
    {
        get { return PlayerPrefs.GetInt("LaptopPuzzleState", 0); }
        set { PlayerPrefs.SetInt("LaptopPuzzleState", value); }
    }

    // Call this if developing/debugging
    public void ResetState()
    {
        CurrentState = 0;
    }
}
