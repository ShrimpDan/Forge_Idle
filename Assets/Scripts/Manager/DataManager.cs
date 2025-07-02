public class DataManager
{
    public ItemDataLoader ItemLoader { get; private set; }
    public CraftingDataLoader CraftingLoader{ get; private set; }
    public DungeonDataLoader DungeonDataLoader { get; private set; }
    public PersonalityDataLoader PersonalityLoader{ get; private set; }
    public SpecializationDataLoader SpecializationLoader{ get; private set; }
    public MineLoader MineLoader { get; private set; }

    public DataManager()
    {
        // 아이템 관련 데이터
        ItemLoader = new ItemDataLoader();
        CraftingLoader = new CraftingDataLoader();

        // 던전 관련 데이터
        DungeonDataLoader = new DungeonDataLoader();

        // 제자 관련 데이터
        PersonalityLoader = new PersonalityDataLoader();
        SpecializationLoader = new SpecializationDataLoader();

        // 광산 관련 데이터
        MineLoader = new MineLoader();
    }
}
