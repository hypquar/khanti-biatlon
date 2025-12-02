using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// В XRI 3.x пространства имен могут отличаться, проверьте using
using UnityEngine.InputSystem;

public class VehicleSeat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform seatPoint;
    [SerializeField] private GameObject playerXROrigin; // Ссылка на XR Origin
    [SerializeField] private GameObject locomotionSystem; // Объект "Locomotion" с провайдерами

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private Transform originalParent;
    private CharacterController _controller;

    private void Start()
    {
        if (!playerXROrigin.TryGetComponent(out _controller))
        {
            Debug.Log("Контроллер на ориджин не найден");
        }
    }

    public void EnterVehicle()
    {
        // 1. Сохраняем исходного родителя (если нужно вернуться в мир)
        originalParent = playerXROrigin.transform.parent;

        // 2. Сажаем игрока
        playerXROrigin.transform.SetParent(seatPoint);
        playerXROrigin.transform.localPosition = Vector3.zero;
        playerXROrigin.transform.localRotation = Quaternion.identity;

        // 3. Отключаем систему локомоции XRI
        if (locomotionSystem != null)
            locomotionSystem.SetActive(false);

        _controller.enabled = false;

        // 4. Переключаем Action Map (псевдокод)
        // inputActions.FindActionMap("XRI RightHand Locomotion").Disable();
        // inputActions.FindActionMap("VehicleControls").Enable();
    }

    public void ExitVehicle()
    {
        // Возвращаем все как было
        playerXROrigin.transform.SetParent(originalParent);

        _controller.enabled = true;

        if (locomotionSystem != null)
            locomotionSystem.SetActive(true);

        // inputActions.FindActionMap("VehicleControls").Disable();
        // inputActions.FindActionMap("XRI RightHand Locomotion").Enable();
    }
}

