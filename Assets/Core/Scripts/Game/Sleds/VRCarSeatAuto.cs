using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// � XRI 3.x ������������ ���� ����� ����������, ��������� using
using UnityEngine.InputSystem;

public class VehicleSeat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform seatPoint;
    [SerializeField] private GameObject playerXROrigin; // ������ �� XR Origin
    [SerializeField] private GameObject locomotionSystem; // ������ "Locomotion" � ������������

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private Transform originalParent;
    private CharacterController _controller;

    private void Start()
    {
        if (!playerXROrigin.TryGetComponent(out _controller))
        {
            Debug.Log("���������� �� ������� �� ������");
        }
    }

    public void EnterVehicle()
    {
        // 1. ��������� ��������� ���ы����� (���� ����� ��������� � ���)
        originalParent = playerXROrigin.transform.parent;

        // 2. ������ ������
        playerXROrigin.transform.SetParent(seatPoint);
        playerXROrigin.transform.localPosition = Vector3.zero;
        playerXROrigin.transform.localRotation = Quaternion.identity;

        // 3. ��������� ������� ��������� XRI
        if (locomotionSystem != null)
            locomotionSystem.SetActive(false);

        _controller.enabled = false;

        // 4. ����������� Action Map (���������)
        // inputActions.FindActionMap("XRI RightHand Locomotion").Disable();
        // inputActions.FindActionMap("VehicleControls").Enable();
    }

    public void ExitVehicle()
    {
        // ���������� ��� ��� ����
        playerXROrigin.transform.SetParent(originalParent);

        _controller.enabled = true;

        if (locomotionSystem != null)
            locomotionSystem.SetActive(true);

        // inputActions.FindActionMap("VehicleControls").Disable();
        // inputActions.FindActionMap("XRI RightHand Locomotion").Enable();
    }
}

