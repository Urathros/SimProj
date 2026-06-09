using System;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public static class GameFlowPlayerLoopRunner
{
    private static GameFlowManager _flow = null;
    private static bool _isInitialized = false;


    private static void InsertBefore<TTarget>(ref PlayerLoopSystem root, PlayerLoopSystem system)
    {
        if (root.subSystemList == null) return;

        for (int i = 0; i < root.subSystemList.Length; i++)
        {
            if (root.subSystemList[i].type == typeof(TTarget))
            {
                var list = root.subSystemList;
                Array.Resize(ref list, list.Length + 1);

                Array.Copy(list, i, list, i + 1, list.Length - i - 1);
                list[i] = system;

                root.subSystemList = list;
                return;
            }


            // Rekursion
            var child = root.subSystemList[i];
            InsertBefore<TTarget>(ref child, system);
            root.subSystemList[i] = child;
        }

    }
    private static void Tick() { if (_flow != null) _flow.Tick(); }

    public static void Initialize(GameFlowManager flow)
    {
        if (_isInitialized) return;

        if (_flow == null) _flow = flow;
        else return;

        var loop = PlayerLoop.GetCurrentPlayerLoop();

        var system = new PlayerLoopSystem
        {
            type = typeof(GameFlowPlayerLoopRunner),
            updateDelegate = Tick
        };

        InsertBefore<PreLateUpdate>(ref loop, system);

        PlayerLoop.SetPlayerLoop(loop);

        _isInitialized = true;
    }

    public static void Finalize()
    {
        if (!_isInitialized) return;

        PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());

        _flow = null;
        _isInitialized = false;
    }
}
