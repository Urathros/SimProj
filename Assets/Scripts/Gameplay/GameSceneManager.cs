using UnityEngine;

public class GameSceneManager : SceneInjector
{
    [field: SerializeField]
    public CameraController Camera { get; set; } = null;

    [field: SerializeField]
    public InputController Input { get; set; } = null;
}
