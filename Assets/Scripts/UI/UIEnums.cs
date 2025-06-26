public enum UIType
{
    Fixed,
    Window,
    Popup
}

public enum ItemType
{
    Weapon = 0,
    Resource = 1,
    Gem = 2,
}

public enum ButtonType
{
    Sell,
    Craft,
    Upgrade,
    Quest,
    Mine,
    Dungeon,
    Gem,      // 추가
    Refine,   // 추가
    MineDetail // 추가
}

public static class UIName
{
    // Fixed UI
    public const string MainUI = "MainUI";

    // Window UI
    public const string SellWeaponWindow = "SellWeaponWindow";
    public const string CraftWeaponWindow = "CraftWeaponWindow";
    public const string UpgradeWeaponWindow = "UpgradeWeaponWindow";
    public const string QuestWindow = "QuestWindow";
    public const string MineWindow = "MineWindow";
    public const string DungeonWindow = "DungeonWindow";
    public const string GemsSystemWindow = "GemSystemWindow";
    public const string RefineSystemWindow = "RefineSystemWindow";
    public const string MineDetailWindow = "MineDetailWindow";

    // Popup UI
    public const string InventoryPopup = "InventoryPopup";
    public const string AssistantPopup = "AssistantPopup";
    public const string Forge_Inventory_Popup = "Forge_Inventory_Popup";
    public const string Forge_AssistantPopup = "Forge_AssistantPopup";
    public const string Forge_Recipe_Popup = "Forge_Recipe_Popup";
    public const string DungeonPopup = "DungeonPopup";

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
            ButtonType.Gem => GemsSystemWindow,
            ButtonType.Refine => RefineSystemWindow,
            ButtonType.MineDetail => MineDetailWindow,
            _ => string.Empty
        };
    }
}