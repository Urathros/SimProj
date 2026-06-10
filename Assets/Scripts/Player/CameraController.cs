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

    [SerializeField]
    private Vector2 _startPosition = Vector2.zero;



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


    private void HandleStartMovement(CallbackContext context)
        => _startPosition = context.ReadValue<Vector2>();

    private void HandlePerformMovement(CallbackContext context)
    {
        var dir = (_startPosition - context.ReadValue<Vector2>()).normalized * -1f;
        Direction = new(dir.x, Direction.y, dir.y);
    }

    private void HandleCancelMovement(CallbackContext context)
    {
        _startPosition = Vector2.zero;
        Direction = Vector3.zero;
    }


    private void AddInput()
    {
        if (Input != null)
        {
            Input.MovementAction += HandleMovement;
            Input.MovementStartAction += HandleStartMovement;
            Input.MovementPerformAction += HandlePerformMovement;
            Input.MovementCancelAction += HandleCancelMovement;
        }
    }

    private void RemoveInput()
    {
        if (Input != null)
        {
            //Input.Movement2Action -= HandleMovement2;
            Input.MovementPerformAction -= HandlePerformMovement;
            Input.MovementStartAction -= HandleStartMovement;
            Input.MovementAction -= HandleMovement;
        }
    }


    private void Awake()
    {
        if (_cam == null) _cam = GetComponent<Camera>();

        _flowDirectionCondition = new (1, Allocator.Persistent);
        _flowPosition = new (1, Allocator.Persistent);
        _flowDirection = new (1, Allocator.Persistent);
    }

    private void OnEnable() => AddInput();

    private void OnDisable() => RemoveInput();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddInput();

        if (_settings != null)
        {
            transform.position = _settings.Position;
            transform.rotation = Quaternion.Euler(_settings.Rotation);
            _speed = _settings.MoveSpeed;
        }
    }
}
