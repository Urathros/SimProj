using UnityEngine;

[CreateAssetMenu( menuName = "Game/Settings/Game Settings",
                  fileName = "GameSettings" )]
public class TestGameSettings : ScriptableSingleton<TestGameSettings>
{
    [field: SerializeField, Min(30)]
    public int TargetFramerate { get; private set; } = 60;

    [field: SerializeField]
    public bool EnableTelemetry { get; private set; }
}
