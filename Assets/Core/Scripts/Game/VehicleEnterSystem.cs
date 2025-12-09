using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class VehicleEntrySystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform vehicleTransform;
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform seatPosition;
    [SerializeField] private Transform exitPosition;

    [Header("Locomotion Components")]
    [SerializeField] private LocomotionMediator locomotionMediator;
    [SerializeField] private ContinuousMoveProvider continuousMoveProvider;
    [SerializeField] private ContinuousTurnProvider continuousTurnProvider;
    [SerializeField] private TeleportationProvider teleportationProvider;
    [SerializeField] private ClimbProvider climbProvider;                       // Добавлено
    [SerializeField] private GrabMoveProvider grabMoveProvider;                 // Добавлено
    [SerializeField] private TwoHandedGrabMoveProvider twoHandedGrabMoveProvider; // Добавлено
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject _locomotionObject;

    [Header("Input Actions")]
    [SerializeField] private InputActionProperty enterExitAction;

    [Header("Settings")]
    [SerializeField] private float interactionDistance = 8f;

    [SerializeField] private LoopinVaryingSound _walkingLoopSound;

    private bool isInVehicle = false;
    private bool isPlayerNearVehicle = false;
    private Transform originalParent;

    public bool IsInVehicle => isInVehicle;

    private void OnEnable()
    {
        if (enterExitAction.action != null)
        {
            enterExitAction.action.Enable();
            enterExitAction.action.performed += OnEnterExitPressed;
        }
    }

    private void OnDisable()
    {
        if (enterExitAction.action != null)
        {
            enterExitAction.action.performed -= OnEnterExitPressed;
            enterExitAction.action.Disable();
        }
    }

    private void Update()
    {
        if (xrOrigin == null || vehicleTransform == null) return;

        Vector3 playerPos = playerCamera != null
            ? playerCamera.transform.position
            : xrOrigin.position;

        float distance = Vector3.Distance(playerPos, vehicleTransform.position);

        if (!isInVehicle)
        {
            isPlayerNearVehicle = distance <= interactionDistance;
        }

        //Debug.Log($"XR Origin pos: {xrOrigin.position}, Vehicle pos: {vehicleTransform.position}, Distance: {distance}, Threshold: {interactionDistance}");
    }

    private void OnEnterExitPressed(InputAction.CallbackContext context)
    {
        Debug.Log($"Button Pressed! Near Vehicle: {isPlayerNearVehicle}, In Vehicle: {isInVehicle}");

        if (isInVehicle)
            ExitVehicle();
        else if (isPlayerNearVehicle)
            EnterVehicle();
    }

    private void EnterVehicle()
    {
        _walkingLoopSound.enabled = false;
        originalParent = xrOrigin.parent;
        DisableLocomotion();

        // Сохраняем смещение камеры ДО любых изменений (в локальных координатах XR Origin)
        Vector3 cameraLocalOffset = xrOrigin.InverseTransformPoint(playerCamera.transform.position);
        cameraLocalOffset.y = 0; // Игнорируем высоту

        // Привязываем к транспорту
        xrOrigin.SetParent(vehicleTransform);

        // Ставим XR Origin на позицию сиденья
        xrOrigin.rotation = seatPosition.rotation;
        xrOrigin.position = seatPosition.position;

        // Сдвигаем назад на величину смещения камеры (в мировых координатах)
        Vector3 worldOffset = xrOrigin.TransformVector(cameraLocalOffset);
        xrOrigin.position -= worldOffset;

        isInVehicle = true;
    }

    private void ExitVehicle()
    {
        _walkingLoopSound.enabled = true;
        Vector3 cameraLocalOffset = xrOrigin.InverseTransformPoint(playerCamera.transform.position);
        cameraLocalOffset.y = 0;

        xrOrigin.SetParent(originalParent);

        xrOrigin.rotation = exitPosition.rotation;
        xrOrigin.position = exitPosition.position;

        Vector3 worldOffset = xrOrigin.TransformVector(cameraLocalOffset);
        xrOrigin.position -= worldOffset;

        EnableLocomotion();
        isInVehicle = false;
    }


    private void DisableLocomotion()
    {
        if (locomotionMediator != null) locomotionMediator.enabled = false;
        if (characterController != null) characterController.enabled = false;

        // Movement providers
        if (continuousMoveProvider != null) continuousMoveProvider.enabled = false;
        if (grabMoveProvider != null) grabMoveProvider.enabled = false;
        if (twoHandedGrabMoveProvider != null) twoHandedGrabMoveProvider.enabled = false;

        // Climbing
        if (climbProvider != null) climbProvider.enabled = false;

        // Turning & Teleportation
        if (continuousTurnProvider != null) continuousTurnProvider.enabled = false;
        if (teleportationProvider != null) teleportationProvider.enabled = false;

        if (_locomotionObject != null) _locomotionObject.SetActive(false);
    }

    private void EnableLocomotion()
    {
        if (_locomotionObject != null) _locomotionObject.SetActive(true);

        if (locomotionMediator != null) locomotionMediator.enabled = true;
        if (characterController != null) characterController.enabled = true;

        if (continuousMoveProvider != null) continuousMoveProvider.enabled = true;
        if (grabMoveProvider != null) grabMoveProvider.enabled = true;
        if (twoHandedGrabMoveProvider != null) twoHandedGrabMoveProvider.enabled = true;

        if (climbProvider != null) climbProvider.enabled = true;

        if (continuousTurnProvider != null) continuousTurnProvider.enabled = true;
        if (teleportationProvider != null) teleportationProvider.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (vehicleTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(vehicleTransform.position, interactionDistance);
        }
    }
}

