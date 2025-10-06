using UnityEngine;


public class HomeBottlePhase : BattlePhase
{
    public Bottle[] bottles;

    void Update()
    {
        foreach (Bottle bottle in bottles)
            if (bottle != null)
                return;
        phaseOver = true;
    }
}
