using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

[Serializable]
public sealed class GameFlowManager : IDisposable, IReflectable, IDebugUI
{
    private readonly Dictionary<string, IFlow> _activeFlows = new();

    private bool _isDisposed;

    public event Action DebugStateChangedDel = () => { };

    public bool IsRunning(string flowID) => _activeFlows.ContainsKey(flowID);

    private void NotifyDebugStateChanged()
    {
#if UNITY_EDITOR
        DebugStateChangedDel?.Invoke();
#endif
    }

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
        NotifyDebugStateChanged();

        return true;
    }

    public void Complete(string flowID)
    {
        if (!_activeFlows.TryGetValue(flowID, out var flow)) return;

        flow.Complete();
    }

    public void Stop(string flowID)
    {
        if (!_activeFlows.Remove(flowID, out var flow)) return;

        flow.Dispose();

        NotifyDebugStateChanged();
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
        NotifyDebugStateChanged();
    }

    public void CompleteAll()
    {
        ThrowIfDisposed();

        foreach (var flow in _activeFlows.Values)
        {
            flow.Complete();
        }
    }

    public GameFlowManagerDebugSnapshot CreateDebugSnapshot()
    {
        var flows = new List<GameFlowDebugInfo>();

        foreach (var pair in _activeFlows)
        {
            flows.Add(new GameFlowDebugInfo(
                pair.Key,
                pair.Value.GetType().Name,
                pair.Value != null
            ));
        }

        return new GameFlowManagerDebugSnapshot(
            _isDisposed,
            _activeFlows.Count,
            flows
        );
    }
}
