using UnityEngine;

public class GameSceneManager : SceneInjector
{
    [field: SerializeField]
    public CameraController Camera { get; set; } = null;

    [field: SerializeField]
    public InputController Input { get; set; } = null;

    [field: SerializeField]
    public GameFlowManager Flow { get; set; } = null;

    private void Update() => Flow.Tick();

    protected void OnDestroy()
    {
        base.OnDestroy();
        Flow.Dispose();
    }
}
