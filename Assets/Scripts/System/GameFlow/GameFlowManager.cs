using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

[Serializable]
public sealed class GameFlowManager : IDisposable, IReflectable
{
    private readonly Dictionary<string, IFlow> _activeFlows = new();

    private bool _isDisposed;

    public bool IsRunning(string flowID) => _activeFlows.ContainsKey(flowID);

    private void ThrowIfDisposed()
    {
        if (_isDisposed)
        {
            throw new ObjectDisposedException(nameof(GameFlowManager));
        }
    }

    public bool TryStartConditionalUpdate<TCondition, TStep>(TCondition condition,
                                                            TStep step,
                                                            string flowID,
                                                            Action applyHandler = null,
                                                            [CallerMemberName] string callerName = "",
                                                            [CallerLineNumber] int lineNum = 0)
        where TCondition : struct, IGameFlowCondition
        where TStep : struct, IGameFlowStep
    {
        ThrowIfDisposed();

        flowID ??= $"{callerName}:{lineNum}";

        if (_activeFlows.ContainsKey(flowID)) return false;

        var flow = new Flow<TCondition, TStep>(condition, step, applyHandler);
        flow.ScheduleNext();

        _activeFlows.Add(flowID, flow);

        return true;
    }

    public void Stop(string flowID)
    {
        if (!_activeFlows.Remove(flowID, out var flow)) return;

        flow.Dispose();
    }

    public void Tick()
    {
        ThrowIfDisposed();

        var completedFlows = new List<string>();

        foreach (var pair in _activeFlows)
        {
            bool isCompleted = pair.Value.Tick();

            if (!isCompleted) pair.Value.Apply();

            if (isCompleted) completedFlows.Add(pair.Key);
        }

        foreach (var flowId in completedFlows)
        {
            Stop(flowId);
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        foreach (var flow in _activeFlows.Values)
        {
            flow.Dispose();
        }

        _activeFlows.Clear();
        _isDisposed = true;
    }

    public void CompleteAll()
    {
        ThrowIfDisposed();

        foreach (var flow in _activeFlows.Values)
        {
            flow.Complete();
        }
    }
}
