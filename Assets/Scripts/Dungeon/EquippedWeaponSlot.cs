using UnityEngine;
using UnityEngine.UI;

public class EquippedWeaponSlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image cooldownImage;

    public ItemInstance WeaponData { get; private set; }
    private float cooldown;
    private float timer;
    public bool IsReady => timer <= 0f;

    public void Init(ItemInstance weapon)
    {
        WeaponData = weapon;

        //icon.sprite = Resources.Load<Sprite>(weapon.Data.IconPath);
        cooldownImage.fillAmount = 0f;
        timer = 0f;
    }

    public void StartCooldown()
    {
        cooldown = WeaponData.GetTotalInterval();
        timer = cooldown;
    }

    public void Tick(float deltaTime)
    {
        if (timer > 0f)
        {
            timer -= deltaTime;
            cooldownImage.fillAmount = timer / cooldown;
        }
    }
}
