using UnityEngine;
using System.Collections;

namespace MoreFun.UI
{
    public class SpawnItemHorizontalGroup : SpawnItemGroup
    {
        #region Layout
        [SerializeField]
        private float m_horizontalSpacing;
        [SerializeField]
        private RectTransform m_parentContainerRect;

        protected override void Awake()
        {
            base.Awake();

            if (!m_parentContainerRect.IsValid())
            {
                // 默认itemgroup即为container
                m_parentContainerRect = m_itemGroupTrans;
            }
        }
        /// <summary>
        /// Adjust the item group size according to dataCount and item size
        /// </summary>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        protected override Vector2 GetItemGroupSize(int dataCount)
        {
            return new Vector2((ItemWidth + m_horizontalSpacing) * dataCount - m_horizontalSpacing, m_topPadding + ItemHeight + m_bottomPadding);
        }

        protected override Vector2 GetItemPosition(int index)
        {
            return new Vector2(index * (ItemWidth + m_horizontalSpacing) + m_leftPadding, -m_topPadding);
        }

        protected override Vector2 GetVisibleItemIndexRange(RectTransform viewportRect)
        {
            // Get the view port size
            float viewPortWidth = viewportRect.rect.width;
            // Get preview invisible width
            Vector2 frontInvisibleWidth = GetCornerAtIndex(viewportRect, 1) - GetCornerAtIndex(m_itemGroupTrans, 1);
            GetItemLossyScale();
            if (Vector3.zero != m_itemLossyScale)
            {
                frontInvisibleWidth.x /= m_itemLossyScale.x;
                frontInvisibleWidth.y /= m_itemLossyScale.y;
            }        
            Vector2 indexRange = new Vector2();
            indexRange.x = Mathf.FloorToInt(frontInvisibleWidth.x / (ItemWidth + m_horizontalSpacing));
            indexRange.y = Mathf.CeilToInt((frontInvisibleWidth.x + viewPortWidth) / (ItemWidth + m_horizontalSpacing));
            return indexRange;
        }

        public override void ScrollAtNormalizedPos(float xRate = 0, float yRate = 0)
        {
            Vector2 pos = new Vector2(0, 0);
            int indexOfChild = m_itemGroupTrans.GetSiblingIndex();
            if (indexOfChild > 0)
            {
                indexOfChild -= 1;
                pos.y += (m_itemGroupSize.y * yRate + m_parentContainerRect.GetChild(indexOfChild).GetComponent<RectTransform>().rect.height);
            }
            else
            {
                pos.y += m_itemGroupSize.y * yRate;
            }
            m_parentContainerRect.anchoredPosition = pos;
            StartCoroutine(StartUpdateView());
        }

        #endregion

        #region Data

        #endregion
    }
}
