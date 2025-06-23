using Unity.VisualScripting;

public enum UIType
{
    Fixed,
    Window,
    Popup
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

public static class UIName
{
    public const string MainUI = "MainUI";
    public const string SellWeaponWindow = "SellWeaponWindow";
    public const string CraftWeaponWindow = "CraftWeaponWindow";
    public const string UpgradeWeaponWindow = "UpgradeWeaponWindow";
    public const string QuestWindow = "QuestWindow";
    public const string MineWindow = "MineWindow";
    public const string DungeonWindow = "DungeonWindow";

    public static string GetUINameByForgeType(ButtonType type)
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