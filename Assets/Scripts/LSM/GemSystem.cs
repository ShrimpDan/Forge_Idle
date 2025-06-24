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
            // ���� ���� ���߱�
            while (weapon.equippedGems.Count < weapon.maxGemSlots)
                weapon.equippedGems.Add(null);
        }
        weapon.equippedGems[socketIndex] = gem;
        // �߰��� �κ��丮/������ ���� �ʿ�� ���⿡
    }
}
