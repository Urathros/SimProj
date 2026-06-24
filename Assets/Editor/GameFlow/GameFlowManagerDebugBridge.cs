#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class GameFlowManagerDebugBridge
{
    static GameFlowManagerDebugBridge()
    {
        GameFlowManagerDebugHooks.RegisterDel += GameFlowManagerDebuggerWindow.SetTarget;
    }
}
#endif