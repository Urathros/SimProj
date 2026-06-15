using UnityEngine;

public struct GameFlowDebugInfo
{
    public string FlowId { get; set; }
    public string FlowType { get; set; }
    public bool IsValid { get; set; }

    public GameFlowDebugInfo(string flowId, string flowType, bool isValid)
    {
        FlowId = flowId;
        FlowType = flowType;
        IsValid = isValid;
    }
}
