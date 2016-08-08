using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoreFun
{
    /// <summary>
    /// 实现ILayoutElement，且这个ILayoutElement被重新更新时，会派发OnRecalculate事件。
    /// </summary>
    public class LayoutElementNotifier : MonoBehaviour, ILayoutElement
    {
        private event EventHandler m_OnRecalculate;
		public event EventHandler OnRecalculate
		{
			add { m_OnRecalculate -= value; m_OnRecalculate += value; }
			remove { m_OnRecalculate -= value; }
		}



        #region ILayoutElement implementation

        public void CalculateLayoutInputHorizontal()
        {
            if (null != m_OnRecalculate)
            {
                m_OnRecalculate(this, null);
            }
        }

        public void CalculateLayoutInputVertical()
        {
        }

        public float minWidth
        {
            get
            {
                return 0;
            }
        }

        public float preferredWidth
        {
            get
            {
                return 0;
            }
        }

        public float flexibleWidth
        {
            get
            {
                return 0;
            }
        }

        public float minHeight
        {
            get
            {
                return 0;
            }
        }

        public float preferredHeight
        {
            get
            {
                return 0;
            }
        }

        public float flexibleHeight
        {
            get
            {
                return 0;
            }
        }

        public int layoutPriority
        {
            get
            {
                return 0;
            }
        }

        #endregion
    }

}