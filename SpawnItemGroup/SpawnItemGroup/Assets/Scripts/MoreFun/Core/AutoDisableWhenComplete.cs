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
    
    public enum Disabletype
    {
        Self = 0,
        Parent = 1,
        Auto
    }
    public class AutoDisableWhenComplete : MonoBehaviour
    {
        public Disabletype type = Disabletype.Self;
        [Range(0, 1.0f)]
        public float disableNormalizedTime = 1.0f;

        private Animator m_ani;
        public void msgDisableMe()
        {
            if(type == Disabletype.Self)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                if(this.transform.parent != null)
                {
                    this.transform.parent.gameObject.SetActive(false);
                }
            }
        }

        void Start()
        {
            m_ani = GetComponent<Animator>();
        }

        void Update()
        {
            if (type == Disabletype.Auto)
            {
                if (m_ani.GetCurrentAnimatorStateInfo(0).normalizedTime > disableNormalizedTime)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
