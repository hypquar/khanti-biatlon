using UnityEngine;

public class test : MonoBehaviour
{

    void Start()
    {
        TargetPoints.OnTargetHit += CHeckLog;
    }
    void CHeckLog(GameObject target, int points)
    {
        Debug.Log($"Увидел попадание, показываю вещи Target - {target}, points - {points}");
    }
}
