using UnityEngine;

public class GameSceneManager : SceneInjector
{
    [field: SerializeField]
    public CameraController Camera { get; set; } = null;

    [field: SerializeField]
    public InputController Input { get; set; } = null;

    [field: SerializeField]
    public GameFlowManager Flow { get; set; } = null;

    [field: SerializeField]
    public InGameHUD HUD { get; set; } = null;
    
    protected void Awake()
    {
        base.Awake();
        GameFlowPlayerLoopRunner.Initialize(Flow);
        GameFlowManagerDebugHooks.Register(Flow);
    }
    //private void Update() => Flow.Tick();

    protected void OnDestroy()
    {
        GameFlowPlayerLoopRunner.Finalize();

        base.OnDestroy();
        Flow.Dispose();
    }
}
