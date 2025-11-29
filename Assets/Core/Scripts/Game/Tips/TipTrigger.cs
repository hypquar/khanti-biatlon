using ContextTips;
using UnityEngine;

public class TipTrigger : MonoBehaviour
{
    [ContextMenu("Trigger next tip")]
    public void TriggerNexTip()
    {
        ContextTipsManager.Instance.ShowNextTip();
    }
}
