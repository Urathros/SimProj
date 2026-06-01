using System;
using System.Collections.Generic;
using UnityEngine;

internal static class SingletonResetRegistry
{
    private static readonly HashSet<Action> ResetActions = new();

    public static void Register(Action resetAction)
    {
        if (resetAction == null) return;

        ResetActions.Add(resetAction);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAll()
    {
        foreach (var reset in ResetActions)
        {
            reset?.Invoke();
        }

        ResetActions.Clear();
    }
}
