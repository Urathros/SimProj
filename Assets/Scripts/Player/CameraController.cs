using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour, IReflectable
{
    /*************************************************************************/
    #region Constants
    /*************************************************************************/
    /// <summary>
    /// Unique identifier used by the GameFlowManager to track
    /// camera movement update operations.
    /// </summary>
    private const string MovementUpdateToken = "CameraMovement";

    /// <summary>
    /// Index of the first element within the native arrays.
    /// Since all arrays are allocated with a length of one,
    /// this constant is used to improve readability.
    /// </summary>
    private const byte NativeArrFirstIdx = 0;

    #endregion
    /*************************************************************************/



    /*************************************************************************/
    #region Fields
    /*************************************************************************/
    /// <summary>
    /// Reference to the camera component controlled by this controller.
    /// </summary>
    [ SerializeField, 
      Tooltip("Reference to the camera component controlled by this controller.") ]
    private Camera _cam = null;

    /// <summary>
    /// Configuration asset containing initial camera settings
    /// such as position, rotation and movement speed.
    /// </summary>
    [ SerializeField,
      Tooltip("Configuration asset containing the camera's initial position, rotation and movement speed.")]
    private CameraSettings _settings = null;


    /*-----------------------------------------------------------------------*/
    #region System
    /*-----------------------------------------------------------------------*/

    /// <summary>
    /// Provides access to all camera-related input actions.
    /// </summary>
    [Inject ]
    [ field: Header("System") ]
    [ field: SerializeField,
             Tooltip("Provides access to all camera-related input actions.") ]
    public InputController Input { get; set; } = null;

    /// <summary>
    /// Handles conditional update execution and movement flow processing.
    /// </summary>
    [Inject]
    public GameFlowManager Flow { get; set; } = null;

    #endregion
    /*-----------------------------------------------------------------------*/


    /*-----------------------------------------------------------------------*/
    #region Movement
    /*-----------------------------------------------------------------------*/

    /// <summary>
    /// Current movement direction of the camera.
    /// </summary>
    [Header("Movement")]
    [ SerializeField,
      Tooltip("Current movement direction of the camera.")]
    private Vector3 _direction = Vector3.zero;

    /// <summary>
    /// Current movement speed applied to the camera.
    /// </summary>
    [ SerializeField,
      Tooltip("Movement speed applied when the camera is moving.") ]
    private float _speed = 0.0f;

    /// <summary>
    /// Screen position where a mouse drag movement started.
    /// </summary>
    [ SerializeField,
      Tooltip("Screen position where the current mouse drag started.") ]
    private Vector2 _startPosition = Vector2.zero;
    #endregion
    /*-----------------------------------------------------------------------*/


    /*-----------------------------------------------------------------------*/
    #region Native
    /*-----------------------------------------------------------------------*/
    /// <summary>
    /// Native array used by movement conditions to determine
    /// whether movement updates should continue running.
    /// </summary>
    private NativeArray<Vector3> _flowDirectionCondition;

    /// <summary>
    /// Native array containing the current movement direction
    /// used by movement execution steps.
    /// </summary>
    private NativeArray<float3> _flowDirection;

    /// <summary>
    /// Native array containing the camera position
    /// used during movement calculations.
    /// </summary>
    private NativeArray<float3> _flowPosition;
    #endregion
    /*-----------------------------------------------------------------------*/

    #endregion
    /*************************************************************************/


    /// <summary>
    /// Gets or sets the current movement direction.
    /// Setting this value updates the native movement data
    /// and starts a movement update if required.
    /// </summary>
    public Vector3 Direction
    {
        get => _direction;
        set
        {
            _direction = value;

            Flow.Complete(MovementUpdateToken);

            UpdateNativeFlowDirection();

            StartConditionalMovementUpdate();
        }
    }



    /*************************************************************************/
    #region Callbacks
    /*************************************************************************/
    /// <summary>
    /// Handles keyboard movement input and updates
    /// the camera movement direction.
    /// </summary>
    /// <param name="context">Input callback context.</param>
    private void HandleKeyboardMovement(CallbackContext context)
        => Direction = context.ReadValue<Vector3>();

    /// <summary>
    /// Handles the beginning of a mouse drag movement operation.
    /// Stores the initial mouse position and starts movement processing.
    /// </summary>
    /// <param name="context">Input callback context.</param>
    private void HandleStartMouseMovement(CallbackContext context)
    {
        _startPosition = context.ReadValue<Vector2>();

        UpdateNativeFlowDirection();
        StartConditionalMovementUpdate();
    }

    /// <summary>
    /// Handles mouse drag updates and converts the drag delta
    /// into a camera movement direction.
    /// </summary>
    /// <param name="context">Input callback context.</param>
    private void HandlePerformMouseMovement(CallbackContext context)
    {
        var dir = (_startPosition - context.ReadValue<Vector2>()).normalized * -1f;
        _direction = new(dir.x, Direction.y, dir.y);

        Flow.Complete(MovementUpdateToken);
        UpdateNativeFlowDirection();
    }

    /// <summary>
    /// Handles the cancellation of a mouse drag movement.
    /// Stops active movement updates and resets movement state.
    /// </summary>
    /// <param name="context">Input callback context.</param>
    private void HandleCancelMouseMovement(CallbackContext context)
    {
        Flow.Complete(MovementUpdateToken);
        Flow.Stop(MovementUpdateToken);

        _startPosition = Vector2.zero;
        _direction = Vector3.zero;

        UpdateNativeFlowDirection();
    }

    #endregion
    /*************************************************************************/


    /// <summary>
    /// Starts a conditional movement update process if one
    /// is not already running.
    /// </summary>
    private void StartConditionalMovementUpdate()
    {
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
                    MovementUpdateToken,
                    applyHandler: () =>
                    {
                        Flow.Complete(MovementUpdateToken);
                        transform.position = _flowPosition[NativeArrFirstIdx];
                    }
                );
    }

    /// <summary>
    /// Synchronizes the current movement direction with the
    /// native arrays used by the movement system.
    /// </summary>
    private void UpdateNativeFlowDirection()
    {
        _flowDirectionCondition[NativeArrFirstIdx] = _direction;
        _flowDirection[NativeArrFirstIdx] = _direction;
    }

    /// <summary>
    /// Registers all required input callbacks.
    /// </summary>
    private void AddInput()
    {
        if (Input != null)
        {
            Input.MovementAction += HandleKeyboardMovement;
            Input.MovementStartAction += HandleStartMouseMovement;
            Input.MovementPerformAction += HandlePerformMouseMovement;
            Input.MovementCancelAction += HandleCancelMouseMovement;
        }
    }

    /// <summary>
    /// Unregisters all previously registered input callbacks.
    /// </summary>
    private void RemoveInput()
    {
        if (Input != null)
        {
            Input.MovementCancelAction -= HandleCancelMouseMovement;
            Input.MovementPerformAction -= HandlePerformMouseMovement;
            Input.MovementStartAction -= HandleStartMouseMovement;
            Input.MovementAction -= HandleKeyboardMovement;
        }
    }

    /*************************************************************************/
    #region ExecutionOroder
    /*************************************************************************/
    private void Awake()
    {
        if (_cam == null) _cam = GetComponent<Camera>();

        _flowDirectionCondition = new(1, Allocator.Persistent);
        _flowPosition = new(1, Allocator.Persistent);
        _flowDirection = new(1, Allocator.Persistent);
    }

    private void OnEnable() => AddInput();

    private void OnDisable() => RemoveInput();

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
    #endregion
    /*************************************************************************/
}
