using UnityEngine;

public class GemSystem : MonoBehaviour
{
    public static GemSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AttachGem(Weapon weapon, Gem gem, int socketIndex)
    {
        if (weapon == null || gem == null) return;
        if (weapon.equippedGems.Count < weapon.maxGemSlots)
        {
            // 소켓 개수 맞추기
            while (weapon.equippedGems.Count < weapon.maxGemSlots)
                weapon.equippedGems.Add(null);
        }
        weapon.equippedGems[socketIndex] = gem;
        // 추가로 인벤토리/데이터 갱신 필요시 여기에
    }
}
