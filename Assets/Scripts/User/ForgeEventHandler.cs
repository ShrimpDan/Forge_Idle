using System;

public class ForgeEventHandler
{
    public event Action<int> OnGoldChanged;
    public event Action<int> OnDiaChanged;
    public event Action<int, int> OnFameChanged;
    public event Action<int> OnTotalFameChanged;
    public event Action<int> OnLevelChanged;
    public event Action<TraineeData, bool> OnAssistantChanged;
    public event Action<CustomerJob, int> OnWeaponCrafted;

    public void RaiseGoldChanged(int gold) => OnGoldChanged?.Invoke(gold);
    public void RaiseDiaChanged(int dia) => OnDiaChanged?.Invoke(dia);
    public void RaiseFameChanged(int currentFame, int maxFame) => OnFameChanged?.Invoke(currentFame, maxFame);
    public void RasieTotalFameChanged(int totalFame) => OnTotalFameChanged?.Invoke(totalFame);
    public void RaiseLevelChanged(int level) => OnLevelChanged?.Invoke(level);
    public void RaiseAssistantChanged(TraineeData assi, bool isActive) => OnAssistantChanged?.Invoke(assi, isActive);
    public void RaiseWeaponCrafted(CustomerJob jobType, int price) => OnWeaponCrafted?.Invoke(jobType, price);
}
