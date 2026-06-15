using System;

public interface IDebugUI
{
    event Action DebugStateChangedDel;
    GameFlowManagerDebugSnapshot CreateDebugSnapshot();
}
