using UnityEngine;


[RequireComponent(typeof(Toad))]
public class ToadWinPhase : BattlePhase
{
    Toad toad;

    protected override void Start()
    {
        base.Start();
        toad = GetComponent<Toad>();
    }

    public override void PhaseEnter()
    {
        Debug.Log("Win!!!");
    }
}
