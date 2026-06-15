using System.Collections.Generic;
using UnityEngine;

public struct GameFlowManagerDebugSnapshot
{
    public bool IsDisposed { get; set; }
    public int ActiveFlowCount { get; set; }
    public IReadOnlyList<GameFlowDebugInfo> ActiveFlows { get; set; }

    public GameFlowManagerDebugSnapshot(bool isDisposed, int activeFlowCount, IReadOnlyList<GameFlowDebugInfo> activeFlows)
    {
        IsDisposed = isDisposed;
        ActiveFlowCount = activeFlowCount;
        ActiveFlows = activeFlows;
    }
}
