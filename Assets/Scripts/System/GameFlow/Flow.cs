using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

//TODO: Serialisieren -> Event Sourcing
public sealed class Flow<TCondition, TStep> : IFlow
        where TCondition : struct, IGameFlowCondition
        where TStep : struct, IGameFlowStep
{
    private readonly NativeArray<byte> _shouldContinue;
    private readonly TCondition _condition;
    private TStep _step;

    private JobHandle _handle;
    private bool _isScheduled;

    private readonly Action _applyHandler;

    public Flow(TCondition condition, TStep step, Action applyHandler)
    {
        _condition = condition;
        _step = step;
        _shouldContinue = new NativeArray<byte>(1, Allocator.Persistent);
        _applyHandler = applyHandler;
    }

    public void ScheduleNext()
    {
        var job = new ConditionalFlowJob<TCondition, TStep>
        {
            Condition = _condition,
            Step = _step,
            ShouldContinue = _shouldContinue
        };

        _handle = job.Schedule();
        _isScheduled = true;
    }

    public bool Tick()
    {
        if (!_isScheduled)
        {
            ScheduleNext();
            return false;
        }

        _handle.Complete();
        _isScheduled = false;

        if (_shouldContinue[0] == 0) return true;

        ScheduleNext();
        return false;
    }

    public void Dispose()
    {
        if (_isScheduled)
        {
            _handle.Complete();
            _isScheduled = false;
        }

        if(_shouldContinue.IsCreated)
        {
            _shouldContinue.Dispose();
        }
    }

    public void Complete()
    {
        if (!_isScheduled)
            return;

        _handle.Complete();
        _isScheduled = false;
    }

    public void Apply() => _applyHandler?.Invoke();
}
