public class DataManager
{
    // 아이템 관련 데이터 로더
    public ItemDataLoader ItemLoader { get; private set; }
    public CraftingDataLoader CraftingLoader { get; private set; }

    // 던전 관련 데이터 로더
    public DungeonDataLoader DungeonDataLoader { get; private set; }

    // 제자 관련 데이터 로더
    public AssistantDataLoader AssistantLoader { get; private set; }
    public PersonalityDataLoader PersonalityLoader { get; private set; }
    public SpecializationDataLoader SpecializationLoader { get; private set; }

    // 광산 관련 데이터 로더
    public MineLoader MineLoader { get; private set; }

    // 손님 관련 데이터 로더
    public CustomerDataLoader CustomerDataLoader { get; private set; }
    public RegularDataLoader RegularDataLoader { get; private set; }

    public DataManager()
    {
        // 아이템 관련 데이터
        ItemLoader = new ItemDataLoader();
        CraftingLoader = new CraftingDataLoader();

        // 던전 관련 데이터
        DungeonDataLoader = new DungeonDataLoader();

        // 제자 관련 데이터
        AssistantLoader = new AssistantDataLoader();
        PersonalityLoader = new PersonalityDataLoader();
        SpecializationLoader = new SpecializationDataLoader();

        // 광산 관련 데이터
        MineLoader = new MineLoader();

        // 손님 관련 데이터
        CustomerDataLoader = new CustomerDataLoader();
        RegularDataLoader = new RegularDataLoader();
    }
}
