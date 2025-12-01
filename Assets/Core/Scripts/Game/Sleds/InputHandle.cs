using Sleds;
using UnityEngine;

namespace Sleds
{
    public class InputHandle : MonoBehaviour
    {
        [SerializeField] private HandleSide _handleSide;
        [SerializeField] private HandleStatus _status;

        public HandleSide Side
        {
            get { return _handleSide; } 
        }
        public HandleStatus Status
        {
            get 
            {
                return _status; 
            }
            private set
            {
                _status = value;
            }
        }

        public void ChangeStatus(HandleStatus status)
        {
            _status = status;
        }
    }
}
