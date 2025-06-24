public enum UIType
{
    Fixed,
    Window,
    Popup
}

public enum InvenSlotType
{
    Resource,
    Useable,
}

public enum ButtonType
{
    Sell,
    Craft,
    Upgrade,
    Quest,
    Mine,
    Dungeon
}

public enum ObjectType
{
    Item,
    Assistant,
    Weapon
}

public static class UIName
{
    public const string MainUI = "MainUI";
    public const string SellWeaponWindow = "SellWeaponWindow";
    public const string CraftWeaponWindow = "CraftWeaponWindow";
    public const string UpgradeWeaponWindow = "UpgradeWeaponWindow";
    public const string QuestWindow = "QuestWindow";
    public const string MineWindow = "MineWindow";
    public const string DungeonWindow = "DungeonWindow";
    public const string InventoryPopup = "InventoryPopup";

    public static string GetUINameByType(ButtonType type)
    {
        return type switch
        {
            ButtonType.Sell => SellWeaponWindow,
            ButtonType.Craft => CraftWeaponWindow,
            ButtonType.Upgrade => UpgradeWeaponWindow,
            ButtonType.Quest => QuestWindow,
            ButtonType.Mine => MineWindow,
            ButtonType.Dungeon => DungeonWindow,
            _ => string.Empty
        };
    }
}