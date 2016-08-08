using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoreFun
{
    /// <summary>
    /// 跟目标（LayoutElementNotifier）的size进行匹配。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class TargetSizeFitter : MonoBehaviour, ILayoutElement, ILayoutSelfController
    {
        [SerializeField]
        private float padWidth = 0.0f;
        [SerializeField]
        private float padHeight = 0.0f;

        [SerializeField]
        private LayoutElementNotifier m_layoutNotifier;
        private LayoutElementNotifier m_layoutNotifierOld;
        private RectTransform m_target;

        private RectTransform m_selfRectTransform;

        public float minimumWidth = -1;

        void Awake()
        {
            UpdateListener();
            m_selfRectTransform = transform as RectTransform;
        }

        private void UpdateListener()
        {
            if (null != m_layoutNotifierOld)
            {
                m_layoutNotifierOld.OnRecalculate -= OnTargetRecalculate;
            }

            m_target = null;
            if (null != m_layoutNotifier)
            {
                m_layoutNotifier.OnRecalculate += OnTargetRecalculate;
                m_target = m_layoutNotifier.transform as RectTransform;
            }

            m_layoutNotifierOld = m_layoutNotifier;
        }

        private void OnTargetRecalculate(object sender, EventArgs args)
        {
            if (null != m_selfRectTransform)
            {
                m_selfRectTransform = transform as RectTransform;
            }
            LayoutRebuilder.MarkLayoutForRebuild(m_selfRectTransform);
        }
        #region ILayoutElement implementation

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        public float minWidth
        {
            get
            {
                if (null != m_target)
                {
                    return m_target.sizeDelta.x + padWidth;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        public float preferredWidth
        {
            get
            {
                if (null != m_target)
                {
                    return m_target.sizeDelta.x + padWidth;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        public float flexibleWidth
        {
            get
            {
                if (null != m_target)
                {
                    return m_target.sizeDelta.x + padWidth;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        public float minHeight
        {
            get
            {
                if (null != m_target)
                {
                    return m_target.sizeDelta.y + padHeight;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        public float preferredHeight
        {
            get
            {
                if (null != m_target)
                {
                    return m_target.sizeDelta.y + padHeight;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        public float flexibleHeight
        {
            get
            {
                if (null != m_target)
                {
                    return m_target.sizeDelta.y + padHeight;
                }
                else
                {
                    return 0.0f;
                }
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

        #region ILayoutController implementation
        public void SetLayoutHorizontal()
        {
            if (null == m_selfRectTransform)
            {
                m_selfRectTransform = transform as RectTransform;
            }

            if (null != m_target)
            {
                if (-1 != minimumWidth && minimumWidth > LayoutUtility.GetPreferredWidth(m_target))
                {
                    m_selfRectTransform.sizeDelta = new Vector2(
                        minimumWidth + padWidth,
                        m_selfRectTransform.sizeDelta.y + padHeight
                        );
                }
                else
                {
                    m_selfRectTransform.sizeDelta = new Vector2(
                        LayoutUtility.GetPreferredWidth(m_target) + padWidth,
                        m_selfRectTransform.sizeDelta.y + padHeight
                        );
                }
            }

        }
        public void SetLayoutVertical()
        {
            if (null == m_selfRectTransform)
            {
                m_selfRectTransform = transform as RectTransform;
            }
            if (null != m_target)
            {
                m_selfRectTransform.sizeDelta = new Vector2(
                    m_selfRectTransform.sizeDelta.x + padWidth,
                    LayoutUtility.GetPreferredHeight(m_target) + padHeight
                    );
            }
        }
        #endregion


        void OnValidate()
        {
            UpdateListener();
            LayoutRebuilder.MarkLayoutForRebuild(m_selfRectTransform);
        }
    }

}