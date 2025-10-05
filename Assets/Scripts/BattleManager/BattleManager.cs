using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public AudioSource soundtrackPlayer;
    List<BattlePhase> phases = new();
    int activePhasesIndex;

    void Start()
    {
        foreach (BattlePhase phase in gameObject.GetComponents(typeof(BattlePhase)))
        {
            Debug.Log($"Register battle phase: {phase.GetType()}");
            phases.Add(phase);
        }

        Debug.Log($"Phase count: {phases.Count}");

        ActivatePhase(0);
    }

    void Update() { UpdatePhase(); }

    void FixedUpdate() { UpdatePhase(); }

    void UpdatePhase()
    {
        if (phases[activePhasesIndex].phaseOver)
        {
            phases[activePhasesIndex].PhaseExit();
            phases[activePhasesIndex].enabled = false;
            activePhasesIndex += 1;
            if (activePhasesIndex >= phases.Count)
                return;
            ActivatePhase(activePhasesIndex);
        }
    }

    public void ActivatePhase(int index)
    {
        activePhasesIndex = index;
        if (activePhasesIndex >= phases.Count)
            return;
        var soundtrack = phases[activePhasesIndex].soundtrack;
        Debug.Log(soundtrack.name);
        if (soundtrack != null)
        {
            soundtrackPlayer.clip = soundtrack;
            soundtrackPlayer.Play();
        }
        phases[activePhasesIndex].enabled = true;
        phases[activePhasesIndex].PhaseEnter();
    }
}
