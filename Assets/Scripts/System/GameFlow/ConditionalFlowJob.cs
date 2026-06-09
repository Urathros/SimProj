using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

[BurstCompile]
public struct ConditionalFlowJob<TCondition, TStep> : IJob
    where TCondition : struct, IGameFlowCondition
    where TStep : struct, IGameFlowStep
{
    [ReadOnly] public TCondition Condition;
    public TStep Step;

    public NativeArray<byte> ShouldContinue; // native bool

    public void Execute()
    {
        if(!Condition.ShouldContinue())
        {
            ShouldContinue[0] = 0;
            return;
        }

        Step.Execute();
        ShouldContinue[0] = 1;
    }
}
