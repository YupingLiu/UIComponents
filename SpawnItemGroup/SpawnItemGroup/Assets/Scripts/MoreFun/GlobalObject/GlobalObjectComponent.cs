using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreFun
{
    public class GlobalObjectComponent : MoreBehaviour
    {
        void Awake()
        {
            this.MoreLog();
        }

        void Start()
        {
            this.MoreLog();
        }

        void OnEnable()
        {
            this.MoreLog();
        }

        void OnDisable()
        {
            this.MoreLog();
        }

        void Update()
        {
            GlobalObject.Instance._DispatchUpdate();
        }

        public void FixedUpdate()
        {
            GlobalObject.Instance._DispathFixedUpdate();
        }
        void LateUpdate()
        {
            GlobalObject.Instance._DispatchLateUpdate();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            this.MoreLog(pauseStatus);
            GlobalObject.Instance._DispatchApplicationPause(pauseStatus);
        }

        void OnLevelWasLoaded(int level)
        {
            this.MoreLog(level);
            GlobalObject.Instance._DispatchLevelWasLoaded(level);
        }
        void OnDestroy()
        {
            this.MoreLog();
        }
        void OnApplicationQuit()
        {
            this.MoreLog();
        }
    }
}
