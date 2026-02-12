using UnityEngine;
using System.Collections.Generic;
using Sleds;

public class CheckpointManager : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();
    [SerializeField] private float respawnHeight = 2f; // Высота над чекпоинтом при респауне
    
    private Dictionary<GameObject, int> PlayerCheckpoints = new Dictionary<GameObject, int>();
    private Dictionary<GameObject, Vector3> PlayerRespawnPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> PlayerRespawnRotations = new Dictionary<GameObject, Quaternion>();
    
    private void Start()
    {
        // Автоматически найти все чекпоинты, если не назначены вручную
        if (checkpoints.Count == 0)
        {
            checkpoints.AddRange(FindObjectsOfType<Checkpoint>());
            checkpoints.Sort((a, b) => a.Index.CompareTo(b.Index));
        }
        
        Debug.Log($"Найдено {checkpoints.Count} чекпоинтов");
    }
    
    public void CheckpointReached(Checkpoint checkpoint, GameObject vehicle)
    {
        int currentIndex = PlayerCheckpoints.ContainsKey(vehicle) ? PlayerCheckpoints[vehicle] : -1;
        
        // Проверяем, что это следующий чекпоинт по порядку
        if (checkpoint.Index == currentIndex + 1 || checkpoint.Index == 0 && currentIndex == -1)
        {
            PlayerCheckpoints[vehicle] = checkpoint.Index;
            
            // Сохраняем позицию и поворот для респауна
            Vector3 respawnPos = checkpoint.transform.position + Vector3.up * respawnHeight;
            PlayerRespawnPositions[vehicle] = respawnPos;
            PlayerRespawnRotations[vehicle] = checkpoint.transform.rotation;
            
            Debug.Log($"Чекпоинт {checkpoint.Index} пройден!");
        }
    }
    
    public void RespawnPlayer(GameObject vehicle)
    {
        if (PlayerRespawnPositions.ContainsKey(vehicle))
        {
            CharacterController controller = vehicle.GetComponent<CharacterController>();
            
            if (controller != null)
            {
                // Отключаем контроллер для телепортации
                controller.enabled = false;
                vehicle.transform.position = PlayerRespawnPositions[vehicle];
                vehicle.transform.rotation = PlayerRespawnRotations[vehicle];
                controller.enabled = true;
            }
            else
            {
                vehicle.transform.position = PlayerRespawnPositions[vehicle];
                vehicle.transform.rotation = PlayerRespawnRotations[vehicle];
            }
            
            // Сбрасываем скорость
            SuspensionVehicle SV = vehicle.GetComponent<SuspensionVehicle>();
            if (SV != null)
            {
                SV.StopSpeed();
            }
            
            Debug.Log("Респаун на последнем чекпоинте!");
        }
        else
        {
            Debug.LogWarning("Нет сохранённого чекпоинта для респауна!");
        }
    }
    
    public int GetCurrentCheckpoint(GameObject vehicle)
    {
        return PlayerCheckpoints.ContainsKey(vehicle) ? PlayerCheckpoints[vehicle] : -1;
    }
}