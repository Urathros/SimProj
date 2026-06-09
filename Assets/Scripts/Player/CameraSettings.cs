using UnityEngine;

[CreateAssetMenu(fileName = "Camera Settings", menuName = "Settings/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [field: SerializeField]
    public Vector3 Position { get; set; } = Vector3.zero;

    [field: SerializeField]
    public Vector3 Rotation { get; set; } = Vector3.zero;

    [field: SerializeField]
    public float MoveSpeed { get; set; }
}
