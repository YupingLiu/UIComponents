

using UnityEngine;
namespace MoreFun
{
    public class UIToggleObjectActive : MonoBehaviour
    {
        public GameObject target;

        public void OnToggleValueChanged(bool isSelected)
        {
            if(null != target)
            {
                target.SetActive(isSelected);
            }
        }
    }
}
