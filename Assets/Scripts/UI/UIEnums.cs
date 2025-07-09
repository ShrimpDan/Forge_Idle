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
    Dungeon,
    Gem,      // 추가
    Refine,   // 추가
    MineDetail, // 추가
    MineMiniGame, //Add
    Collection,
    Setting
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
    public const string CollectionWindow = "CollectionWindow";
    public const string SettingWindow = "SettingWindow";
  

    // Popup UI
    public const string InventoryPopup = "InventoryPopup";
    public const string AssistantPopup = "AssistantPopup";
    public const string Forge_Inventory_Popup = "Forge_Inventory_Popup";
    public const string Forge_AssistantPopup = "Forge_AssistantPopup";
    public const string Forge_Recipe_Popup = "Forge_Recipe_Popup";
    public const string DungeonPopup = "DungeonPopup";
    public const string CollectionPopup = "CollectionPopup"; 
    public const string SellWeaponPopup = "SellWeaponPopup";
    public const string AssistantSelectPopup = "AssistantSelectPopup";
    public const string Mine_AssistantPopup = "Mine_AssistantPopup";
    public const string Gem_Weapon_Popup = "Gem_Weapon_Popup";

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
            ButtonType.Collection => CollectionWindow,
            ButtonType.Setting => SettingWindow,
            _ => string.Empty
        };
    }
}