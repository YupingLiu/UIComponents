using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace MoreFun.UI
{
    /// <summary>
    /// 专用于ScrollView的Panel Control
    /// 当此Panel出现时，需要调用下面每个Item的脚本的Show函数，并且给每个脚本一个时间来控制其出现时间
    /// </summary>
    public class UIPanelScrollViewCtrl : IPanelCtrl
    {
        public float animateTime = 0.25f;
        private GridLayoutGroup m_gridLayout;

        void Awake()
        {
            m_gridLayout = cachedTransform.GetComponent<GridLayoutGroup>();
        }

        protected override void DoShow()
        {
            cachedRectTransform.anchoredPosition = new Vector2(cachedRectTransform.sizeDelta.x / 2, cachedRectTransform.anchoredPosition.y);
            DOTween.To(() => cachedRectTransform.anchoredPosition, x => cachedRectTransform.anchoredPosition = x, new Vector2(0, cachedRectTransform.anchoredPosition.y), animateTime)
                .SetEase(Ease.OutQuart)
                .SetDelay(0);
        }
    }
}