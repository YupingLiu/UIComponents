using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MoreFun
{
    /// <summary>
    /// 最纯粹的自销毁脚本。
    /// 不支持对象池、或者GameTimeTree的功能
    /// </summary>
    public class AutoDestroy : MonoBehaviour
    {
        public float duration;

        private float m_startTime;
        void Start()
        {
            m_startTime = Time.time;
        }

        void Update()
        {
            if(Time.time - m_startTime > duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
