
using UnityEngine;
namespace MoreFun
{
    public class UIToggleSendString : MonoBehaviour
    {
        public GameObject target;
        public string content;

        public void OnToggleValueChanged(bool isSelected)
        {
            if(isSelected)
            {
                target.SendMessage("OnToggleSendString", content, SendMessageOptions.DontRequireReceiver);
            }
        }

    }
}
