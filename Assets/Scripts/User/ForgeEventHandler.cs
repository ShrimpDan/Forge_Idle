using System;

public class ForgeEventHandler
{
    public event Action<int> OnGoldChanged;
    public event Action<int> OnDiaChanged;
    public event Action<float, float> OnFameChanged;
    public event Action<int> OnLevelChanged;

    public void RaiseGoldChanged(int gold) => OnGoldChanged?.Invoke(gold);
    public void RaiseDiaChanged(int dia) => OnDiaChanged?.Invoke(dia);
    public void RaiseFameChanged(float currentFame, float maxFame) => OnFameChanged?.Invoke(currentFame, maxFame);
    public void RaiseLevelChanged(int level) => OnLevelChanged?.Invoke(level);
}
