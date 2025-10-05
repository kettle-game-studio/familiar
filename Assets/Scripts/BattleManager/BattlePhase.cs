using UnityEngine;

public class BattlePhase : MonoBehaviour
{
    public bool phaseOver = false;
    protected virtual void Start()
    {
        enabled = false;
    }
    public virtual void PhaseEnter() { }
    public virtual void PhaseExit() { }
}
