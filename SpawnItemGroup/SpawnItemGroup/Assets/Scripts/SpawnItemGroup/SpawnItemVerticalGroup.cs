using UnityEngine;
using System.Collections;

namespace MoreFun.UI
{
    public class SpawnItemVerticalGroup : SpawnItemGroup
    {
        #region Layout
        [SerializeField]
        private float m_verticalSpacing;
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
            return new Vector2(m_leftPadding + ItemWidth + m_rightPadding, (ItemHeight + m_verticalSpacing) * dataCount - m_verticalSpacing);
        }

        protected override Vector2 GetItemPosition(int index)
        {
            return new Vector2(m_leftPadding, -index * (ItemHeight + m_verticalSpacing) - m_topPadding);
        }

        protected override Vector2 GetVisibleItemIndexRange(RectTransform viewportRect)
        {
            // Get the view port size
            float viewPortHeight = viewportRect.rect.height;
            // Get upper invisible height
            Vector2 upperInvisibleHeight = GetCornerAtIndex(m_itemGroupTrans, 1) - GetCornerAtIndex(viewportRect, 1);
            GetItemLossyScale();
            if (Vector3.zero != m_itemLossyScale)
            {
                upperInvisibleHeight.x /= m_itemLossyScale.x;
                upperInvisibleHeight.y /= m_itemLossyScale.y;
            }        
            Vector2 indexRange = new Vector2();
            indexRange.x = Mathf.FloorToInt(upperInvisibleHeight.y / (ItemHeight + m_verticalSpacing));
            indexRange.y = Mathf.FloorToInt((upperInvisibleHeight.y + viewPortHeight) / (ItemHeight + m_verticalSpacing)) + 1;
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

