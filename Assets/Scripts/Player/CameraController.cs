using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour, IReflectable
{
    [SerializeField]
    private Camera _cam = null;

    [SerializeField]
    private CameraSettings _settings = null;

    [Inject]
    [field: SerializeField]
    public InputController Input { get; set; } = null;

    [Inject]
    [field: SerializeField]
    public GameFlowManager Flow { get; set; } = null;

    [Header("Movement")]
    [SerializeField]
    private Vector3 _direction = Vector3.zero;

    [SerializeField]
    private float _speed = 0.0f;



    private NativeArray<Vector3> _flowDirectionCondition;
    private NativeArray<float3> _flowDirection;

    private NativeArray<float3> _flowPosition;

    public Vector3 Direction
    {
        get => _direction; 
        set
        {
            _direction = value;

            Flow.CompleteAll(); 

            _flowDirectionCondition[0] = _direction;
            _flowDirection[0] = _direction;
            Flow.TryStartConditionalUpdate
            (
                new MovementCondition
                {
                    Direction = _flowDirectionCondition
                },
                new MoveCameraStep
                {
                    Position = _flowPosition,
                    Direction = _flowDirection,
                    Speed = _speed,
                    DeltaTime = Time.deltaTime,
                },
                "CameraMovement",
                applyHandler: () => {
                    Flow.CompleteAll();
                    transform.position = _flowPosition[0];
                }
            );
        }
    }



    private void HandleMovement(CallbackContext context)
        => Direction = context.ReadValue<Vector3>();

    private void Awake()
    {
        if (_cam == null) _cam = GetComponent<Camera>();

        _flowDirectionCondition = new NativeArray<Vector3>(1, Allocator.Persistent);
        _flowPosition = new NativeArray<float3>(1, Allocator.Persistent);
        _flowDirection = new NativeArray<float3>(1, Allocator.Persistent);
    }

    private void OnEnable()
    {
        if(Input != null) Input.MovementAction += HandleMovement;
    }

    private void OnDisable()
    {
        if (Input != null) Input.MovementAction -= HandleMovement;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Input != null) Input.MovementAction += HandleMovement;

        if (_settings != null)
        {
            transform.position = _settings.Position;
            transform.rotation = Quaternion.Euler(_settings.Rotation);
            _speed = _settings.MoveSpeed;
        }
    }
}
