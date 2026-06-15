// Runtime assembly
using System;

public static class GameFlowManagerDebugHooks
{
    public static Action<IDebugUI> RegisterDel = ui => { };

    public static Action UpdateDel = () => { };


    public static void Register(IDebugUI target) => RegisterDel.Invoke(target);

    public static void Update() => UpdateDel.Invoke();
}