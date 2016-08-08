using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

namespace MoreFun.UI
{
    /// <summary>
    /// UIPanelDeltaSizeAnimateCtrl用于控制Panel朝特定的方向进行动画隐藏和显示,专用于UGUI
    /// 
    /// 该类向外提供两个接口使用，分别是隐藏和显示函数。
    /// 使用该脚本时需要注释设置Pivot的位置：
    /// 1. NONE方式，表示没有动画效果，仅用于通用的没有动画效果的界面的控制
    /// 2. LEFT方式，pivot应该是在自身的最左边；
    /// 3. RIGHT,TOP和BOTTOM方式以此类推；
    /// 4. TOP_LEFT方式pivot应该在自身的左上方；
    /// 5. TOP_RIGHT，BOTTOM_LEFT，BOTTOM_RIGHT方式以此类推。 
    /// 
    /// FIX_SIZE是使用sizeDelta从original size到0的过程来隐藏，从0到original size的过程来显示
    /// ANCHOR_SIZE则是使用相反的过程来进行隐藏和显示
    /// </summary>
    public class UIPanelDeltaSizeAnimateCtrl : IPanelCtrl
    {
        public enum PanelDirection
        {
            LEFT,
            RIGHT,
            TOP,
            BOTTOM,
            TOP_LEFT,
            TOP_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_RIGHT
        }

        public enum PanelSizeType
        {
            FIX_SIZE,
            ANCHOR_SIZE
        }

        public PanelSizeType panelSizeType = PanelSizeType.FIX_SIZE;
        public PanelDirection panelDirection = PanelDirection.TOP_LEFT;
        public bool endDisableMask = true;

        private RectTransform m_trans;
        private Vector2 m_originalSize;
        private Vector2 m_originalAnchorPos;

        private Mask m_mask;
        private Image m_image;

        // 每个UI Item的sizeDelta和anchoredPosition在Awake的时候已经确定,除了涉及layout类型的
        void Awake()
        {
            m_trans = transform as RectTransform;
            m_originalSize = m_trans.sizeDelta;
            m_originalAnchorPos = m_trans.anchoredPosition;
            m_mask = gameObject.GetComponent<Mask>();
            m_image = gameObject.GetComponent<Image>();
        }

        void Start()
        {
            //SetupAnchorAndPivot();
            Show();
        }
       
        protected override void DoShow()
        {
            float delay = delayTime;

            if (panelSizeType == PanelSizeType.FIX_SIZE)
            {
                switch (panelDirection)
                {
                    case PanelDirection.LEFT:
                    case PanelDirection.RIGHT:
                        cachedRectTransform.sizeDelta = new Vector2(0, m_originalSize.y);
                        break;
                    case PanelDirection.TOP:
                    case PanelDirection.BOTTOM:
                        cachedRectTransform.sizeDelta = new Vector2(m_originalSize.x, 0);
                        break;
                    case PanelDirection.TOP_LEFT:
                    case PanelDirection.TOP_RIGHT:
                    case PanelDirection.BOTTOM_LEFT:
                    case PanelDirection.BOTTOM_RIGHT:
                        cachedRectTransform.sizeDelta = new Vector2(0, 0);
                        break;
                }
                OnShow();
                DOTween.To(() => cachedRectTransform.sizeDelta, x => cachedRectTransform.sizeDelta = x, m_originalSize, delay + animationTime)
                    .SetDelay(delay)
                    .SetEase(getEaseType()).onComplete += OnEnd;
            }
            else
            {
                cachedRectTransform.sizeDelta = -1 * m_originalSize;
                Vector2 target = m_originalAnchorPos;
                target.x *= -1;
                DOTween.To(() => cachedRectTransform.sizeDelta, x => cachedRectTransform.sizeDelta = x, target, delay + animationTime)
                    .SetDelay(delay)
                    .SetEase(getEaseType());
            }
        }

        private void OnShow()
        {
            if (endDisableMask)
            {
                if (m_mask != null)
                    m_mask.enabled = true;
                if (m_image != null)
                    m_image.enabled = true;
            }
        }

        private void OnEnd()
        {
            if (endDisableMask)
            {
                if (m_mask != null)
                    m_mask.enabled = false;
                if (m_image != null)
                    m_image.enabled = false;
            }
        }

        public void Hide()
        {
            switch (panelDirection)
            {
                case PanelDirection.LEFT:
                case PanelDirection.RIGHT:
                    DOTween.To(() => cachedRectTransform.sizeDelta, x => cachedRectTransform.sizeDelta = x, new Vector2(0, m_originalSize.y), animationTime)
                        .SetEase(getEaseType());
                    break;
                case PanelDirection.TOP:
                case PanelDirection.BOTTOM:
                    DOTween.To(() => cachedRectTransform.sizeDelta, x => cachedRectTransform.sizeDelta = x, new Vector2(m_originalSize.x, 0), animationTime)
                        .SetEase(getEaseType());
                    break;
                case PanelDirection.TOP_LEFT:
                case PanelDirection.TOP_RIGHT:
                case PanelDirection.BOTTOM_LEFT:
                case PanelDirection.BOTTOM_RIGHT:
                    DOTween.To(() => cachedRectTransform.sizeDelta, x => cachedRectTransform.sizeDelta = x, new Vector2(0, 0), animationTime)
                        .SetEase(getEaseType());
                    break;
            }
        }

        private void SetupAnchorAndPivot()
        {
            Vector2 pivot = Vector2.zero;
            Vector2 anchorPos = Vector2.zero;

            switch (panelDirection)
            {
                case PanelDirection.LEFT:
                    pivot = new Vector2(0, 0.5f);
                    anchorPos = pivot;
                    break;
                case PanelDirection.RIGHT:
                    pivot = new Vector2(1, 0.5f);
                    anchorPos = pivot;
                    break;
                case PanelDirection.TOP:
                    pivot = new Vector2(0.5f, 1);
                    anchorPos = pivot;
                    break;
                case PanelDirection.BOTTOM:
                    pivot = new Vector2(0.5f, 0);
                    anchorPos = pivot;
                    break;
                case PanelDirection.TOP_LEFT:
                    pivot = new Vector2(0, 1);
                    anchorPos = pivot;
                    break;
                case PanelDirection.TOP_RIGHT:
                    pivot = new Vector2(1, 1);
                    anchorPos = pivot;
                    break;
                case PanelDirection.BOTTOM_LEFT:
                    pivot = new Vector2(0, 0);
                    anchorPos = pivot;
                    break;
                case PanelDirection.BOTTOM_RIGHT:
                    pivot = new Vector2(1, 0);
                    anchorPos = pivot;
                    break;
            }

            cachedRectTransform.pivot = pivot;
            for (int i = 0; i < cachedRectTransform.childCount; ++i)
            {
                RectTransform trans = cachedRectTransform.GetChild(i) as RectTransform;
                trans.anchorMin = trans.anchorMax = anchorPos;
            }
        }

        Ease getEaseType()
        {
            if (ease)
            {
                return Ease.OutBack;
            }
            else
            {
                return Ease.InQuint;
            }
        }
    }
}