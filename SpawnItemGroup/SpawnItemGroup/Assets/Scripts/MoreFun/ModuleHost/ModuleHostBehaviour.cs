
using UnityEngine;

namespace MoreFun
{
    public class ModuleHostBehaviour : MonoBehaviour
    {
        void OnDestroy()
        {
            ModuleHost.Reset();
        }
    }
}

