using UnityEngine;
using System.Collections;

namespace MoreFun.UI
{
    /// <summary>
    /// 面板控制接口类，各种不同面板的显示均需要继承该接口，实现该接口
    /// 这样子就可以在大面板中控制所有孩子面板的动画过程
    /// </summary>
    public abstract class IPanelCtrl : MoreBehaviour
    {
        public bool ease = false;                                          // 是否需要缓动
        public bool delayToPlay = false;                                   // 是否需要延迟播放
        public float delayMinTime = 0.6f;                                  // 等待时间
        public float delayMaxTime = 0.8f;                                  // delayTime浮动值
        public float animationTime = 0.5f;                                 // 动画时间

        private float mLastAnimateTime = 0;

        public void Show(bool isNeedRestart = false)
        {
            float thisTime = Time.time;

            if (isNeedRestart || ((thisTime - mLastAnimateTime) > (delayMaxTime + animationTime)))
            {
                DoShow();
            }
            mLastAnimateTime = Time.time;
        }

        protected abstract void DoShow();

        protected float delayTime
        {
            get
            {
                float delay = 0;
                if (delayToPlay)
                    delay = Random.Range(0, 100) / (float)100f * (delayMaxTime - delayMinTime) + delayMinTime;
                return delay;
            }
        }
    }
}