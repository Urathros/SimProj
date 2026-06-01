using UnityEngine;
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

    [Header("Movement")]
    [SerializeField]
    private Vector3 _direction = Vector3.zero;

    [SerializeField]
    private float _speed = 0.0f;

    private void HandleMovement(CallbackContext context)
        => _direction = context.ReadValue<Vector3>();

    private void Awake()
    {
        if (_cam == null) _cam = GetComponent<Camera>();


        //transform.position = new(0f, 10f, 0f);
        //transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void OnEnable()
    {
        if(Input != null)
        {
            Input.MovementAction += HandleMovement;
        }
    }

    private void OnDisable()
    {

        if (Input != null)
        {
            Input.MovementAction -= HandleMovement;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Input != null)
        {
            Input.MovementAction += HandleMovement;
        }

        if (_settings != null)
        {
            transform.position = _settings.Position;
            transform.rotation = Quaternion.Euler(_settings.Rotation);
            _speed = _settings.Speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }
}
