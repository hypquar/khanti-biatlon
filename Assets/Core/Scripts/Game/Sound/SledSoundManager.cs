using Sleds;
using System.Collections.Generic;
using UnityEngine;

public class SledSoundManager : MonoBehaviour
{
    [SerializeField] private List<LoopinVaryingSound> _sledSounds;
    [SerializeField] private SledInputController _sledInput;

    private void Update()
    {
        if (_sledInput.Status == SledStatus.Moving)
        {
            foreach (var sound in _sledSounds)
            {
                sound.IsMovingExternally = true;
            }
        }
        else if (_sledInput.Status == SledStatus.Halt)
        {
            foreach (var sound in _sledSounds)
            {
                sound.IsMovingExternally = false;
            }
        }
    }
}
