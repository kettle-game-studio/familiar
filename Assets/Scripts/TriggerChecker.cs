using UnityEngine;
using System.Collections.Generic;

public class TriggerChecker: MonoBehaviour
{
    HashSet<Collider2D> set = new();

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log($"OnTriggerEnter2D ({set.Count} colliders)");
        set.Add(col);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log($"OnTriggerExit2D ({set.Count} colliders)");
        set.Remove(col);
    }

    public bool isTriggered() {
        return set.Count > 0;
    }
}
