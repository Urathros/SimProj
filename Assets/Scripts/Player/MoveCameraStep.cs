using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct MoveCameraStep : IGameFlowStep
{
    private NativeArray<float3> _position;

    public NativeArray<float3> Position
    {
        get => _position; 
        set => _position = value; 
    }


    [field: ReadOnly]
    public NativeArray<float3> Direction { get; set; }

    public float Speed { get; set; }
    public float DeltaTime { get; set; }

    public void Execute()
    {
        _position[0] += Direction[0] * Speed * DeltaTime;
    }
}
