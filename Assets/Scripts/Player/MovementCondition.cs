using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct MovementCondition : IGameFlowCondition
{
    [ReadOnly]
    private NativeArray<Vector3> _direction;

    public NativeArray<Vector3> Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }


    public bool ShouldContinue()
    {
        return _direction[0] != Vector3.zero;
    }
}
