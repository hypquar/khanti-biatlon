using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class VehicleEntrySystem : MonoBehaviour
{
    public UnityEvent OnExit;

    [Header("References")]
    [SerializeField] private Transform _vehicleTransform;
    [SerializeField] private Transform _xrOrigin;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _seatPosition;
    [SerializeField] private Transform _exitPosition;

    [Header("Locomotion Components")]
    [SerializeField] private LocomotionMediator _locomotionMediator;
    [SerializeField] private ContinuousMoveProvider _continuousMoveProvider;
    [SerializeField] private ContinuousTurnProvider _continuousTurnProvider;
    [SerializeField] private TeleportationProvider _teleportationProvider;
    [SerializeField] private ClimbProvider _climbProvider;                       
    [SerializeField] private GrabMoveProvider _grabMoveProvider;                 
    [SerializeField] private TwoHandedGrabMoveProvider _twoHandedGrabMoveProvider;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private GameObject _locomotionObject;

    [Header("Input Actions")]
    [SerializeField] private InputActionProperty _enterExitAction;

    [Header("Settings")]
    [SerializeField] private float _interactionDistance = 8f;

    [SerializeField] private LoopinVaryingSound _walkingLoopSound;

    private bool _isInVehicle = false;
    private bool _isPlayerNearVehicle = false;
    private Transform _originalParent;

    public bool IsInVehicle => _isInVehicle;

    private void OnEnable()
    {
        if (_enterExitAction.action != null)
        {
            _enterExitAction.action.Enable();
            _enterExitAction.action.performed += OnEnterExitPressed;
        }
    }

    private void OnDisable()
    {
        if (_enterExitAction.action != null)
        {
            _enterExitAction.action.performed -= OnEnterExitPressed;
            _enterExitAction.action.Disable();
        }
    }

    private void Update()
    {
        if (_xrOrigin == null || _vehicleTransform == null) return;

        Vector3 playerPos = _playerCamera != null
            ? _playerCamera.transform.position
            : _xrOrigin.position;

        float distance = Vector3.Distance(playerPos, _vehicleTransform.position);

        if (!_isInVehicle)
        {
            _isPlayerNearVehicle = distance <= _interactionDistance;
        }

        //Debug.Log($"XR Origin pos: {xrOrigin.position}, Vehicle pos: {vehicleTransform.position}, Distance: {distance}, Threshold: {interactionDistance}");
    }

    private void OnEnterExitPressed(InputAction.CallbackContext context)
    {
        Debug.Log($"Button Pressed! Near Vehicle: {_isPlayerNearVehicle}, In Vehicle: {_isInVehicle}");

        if (_isInVehicle)
        {
            ExitVehicle();
            OnExit.Invoke();
        }
        else if (_isPlayerNearVehicle)
            EnterVehicle();
    }

    private void EnterVehicle()
    {
        _walkingLoopSound.enabled = false;
        _originalParent = _xrOrigin.parent;
        DisableLocomotion();

        // Сохраняем смещение камеры ДО любых изменений (в локальных координатах XR Origin)
        Vector3 cameraLocalOffset = _xrOrigin.InverseTransformPoint(_playerCamera.transform.position);
        cameraLocalOffset.y = 0; // Игнорируем высоту

        // Привязываем к транспорту
        _xrOrigin.SetParent(_vehicleTransform);

        // Ставим XR Origin на позицию сиденья
        _xrOrigin.rotation = _seatPosition.rotation;
        _xrOrigin.position = _seatPosition.position;

        // Сдвигаем назад на величину смещения камеры (в мировых координатах)
        Vector3 worldOffset = _xrOrigin.TransformVector(cameraLocalOffset);
        _xrOrigin.position -= worldOffset;

        _isInVehicle = true;
    }

    private void ExitVehicle()
    {
        _walkingLoopSound.enabled = true;
        Vector3 cameraLocalOffset = _xrOrigin.InverseTransformPoint(_playerCamera.transform.position);
        cameraLocalOffset.y = 0;

        _xrOrigin.SetParent(_originalParent);

        _xrOrigin.rotation = _exitPosition.rotation;
        _xrOrigin.position = _exitPosition.position;

        Vector3 worldOffset = _xrOrigin.TransformVector(cameraLocalOffset);
        _xrOrigin.position -= worldOffset;

        EnableLocomotion();
        _isInVehicle = false;
    }


    private void DisableLocomotion()
    {
        if (_locomotionMediator != null) _locomotionMediator.enabled = false;
        if (_characterController != null) _characterController.enabled = false;

        // Movement providers
        if (_continuousMoveProvider != null) _continuousMoveProvider.enabled = false;
        if (_grabMoveProvider != null) _grabMoveProvider.enabled = false;
        if (_twoHandedGrabMoveProvider != null) _twoHandedGrabMoveProvider.enabled = false;

        // Climbing
        if (_climbProvider != null) _climbProvider.enabled = false;

        // Turning & Teleportation
        if (_continuousTurnProvider != null) _continuousTurnProvider.enabled = false;
        if (_teleportationProvider != null) _teleportationProvider.enabled = false;

        if (_locomotionObject != null) _locomotionObject.SetActive(false);
    }

    private void EnableLocomotion()
    {
        if (_locomotionObject != null) _locomotionObject.SetActive(true);

        if (_locomotionMediator != null) _locomotionMediator.enabled = true;
        if (_characterController != null) _characterController.enabled = true;

        if (_continuousMoveProvider != null) _continuousMoveProvider.enabled = true;
        if (_grabMoveProvider != null) _grabMoveProvider.enabled = true;
        if (_twoHandedGrabMoveProvider != null) _twoHandedGrabMoveProvider.enabled = true;

        if (_climbProvider != null) _climbProvider.enabled = true;

        if (_continuousTurnProvider != null) _continuousTurnProvider.enabled = true;
        if (_teleportationProvider != null) _teleportationProvider.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (_vehicleTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_vehicleTransform.position, _interactionDistance);
        }
    }
}

