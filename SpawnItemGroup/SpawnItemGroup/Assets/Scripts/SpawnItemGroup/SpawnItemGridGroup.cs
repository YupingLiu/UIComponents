using UnityEngine;
using UnityEngine.UI;

namespace MoreFun.UI
{
    public enum LayoutDirection
    {
        vertical = 0,
        horizontal = 1,
    }

    public class SpawnItemGridGroup : SpawnItemGroup
    {
        #region Layout
        [SerializeField]
        Vector2 m_cellSize = new Vector2(100f, 100f);
        [SerializeField]
        private float m_horizontalSpacing;
        [SerializeField]
        private float m_verticalSpacing;
        [SerializeField]
        private LayoutDirection m_layoutDirection = LayoutDirection.vertical;

        private float m_originalGroupWidth;
        private float m_originalGroupHeight;
        [SerializeField]
        private RectTransform m_parentContainerRect;

        private Vector2 m_tmpVectorValue;
        private Vector2 m_invalidVector;
        #region Vertical Drag
        private int m_horizontalCellCount;
        private int m_maxVerticalCellCount;
        private int m_maxVerticalCellColoumLastIndex;
        private int m_lastRowItemsCount;
        #endregion
        #region Horizontal Drag
        private int m_verticalCellCount;
        private int m_maxHorizontalCellCount;
        private int m_maxHorizontalCellColoumLastIndex;
        private int m_lastColumnItemsCount;
        #endregion

        protected override void Awake()
        {
            m_tmpVectorValue = new Vector2();
            m_invalidVector = new Vector2(-1, -1);
            m_itemGroupTrans = transform as RectTransform;
            if (!m_parentContainerRect.IsValid())
            {
                // 默认itemgroup即为container
                m_parentContainerRect = m_itemGroupTrans;
            }
            m_originalGroupWidth = m_itemGroupTrans.rect.width;
            m_originalGroupHeight = m_itemGroupTrans.rect.height;
            base.Awake();
            if (null != GetComponent<LayoutElement>())
            {
                m_itemGroupSize.y = GetComponent<LayoutElement>().minHeight;
                m_itemGroupSize.x = GetComponent<LayoutElement>().minWidth;
            }
            else
            {
                m_itemGroupSize.x = m_originalGroupWidth;
                m_itemGroupSize.y = m_originalGroupHeight;
            }
            m_horizontalCellCount = GetHorizontalCellCount();
            m_verticalCellCount = GetVerticalCellCount();
        }

        /// <summary>
        /// Item prefab adjust to the cell size
        /// </summary>
        protected override void InitItemPrefab()
        {
            if (null != m_itemPrefab)
            {
                RectTransform itemPrefabTrans = m_itemPrefab.transform as RectTransform;
                if (null != itemPrefabTrans)
                {
                    ItemWidth = m_cellSize.x;
                    ItemHeight = m_cellSize.y;

                    ResetRectTransform(itemPrefabTrans, ItemWidth, ItemHeight);
                }
            }
        }

        protected virtual int GetHorizontalCellCount()
        {
            float itemTotalWidth = ItemWidth + m_horizontalSpacing;
            int count = Mathf.FloorToInt((m_itemGroupSize.x + m_horizontalSpacing) / itemTotalWidth);
            return count == 0 ? (count += 1) : count;
        }

        protected virtual int GetVerticalCellCount()
        {
            float itemTotalHeight = ItemHeight + m_verticalSpacing;
            int count = Mathf.FloorToInt((m_itemGroupSize.y + m_verticalSpacing) / itemTotalHeight);
            return count == 0 ? (count += 1) : count;
        }

        /// <summary>
        /// Don't adjust the group size like SpawnItemVerticalGroup
        /// </summary>
        /// <param name="dataCount"></param>
        /// <returns></returns>
        protected override Vector2 GetItemGroupSize(int dataCount)
        {
            switch (m_layoutDirection)
            {
                case LayoutDirection.vertical:
                    if (0 == m_horizontalCellCount)
                    {
                        return m_invalidVector;
                    }
                    m_maxVerticalCellCount =  Mathf.CeilToInt(dataCount / m_horizontalCellCount);
                    m_maxVerticalCellColoumLastIndex = dataCount - (m_maxVerticalCellCount - 1) * m_horizontalCellCount - 1;
                    m_tmpVectorValue.x = m_itemGroupSize.x;
                    m_tmpVectorValue.y = m_topPadding + m_bottomPadding + 
                                            Mathf.CeilToInt((float)dataCount / m_horizontalCellCount) * (ItemHeight + m_verticalSpacing) 
                                            - m_verticalSpacing;
                    break;
                case LayoutDirection.horizontal:
                    if (0 == m_verticalCellCount)
                    {
                        return m_invalidVector;
                    }
                    m_maxHorizontalCellCount = Mathf.CeilToInt(dataCount / m_verticalCellCount);
                    m_maxHorizontalCellColoumLastIndex = dataCount - (m_maxHorizontalCellCount - 1) * m_verticalCellCount - 1;
                    m_tmpVectorValue.x = m_leftPadding + m_rightPadding +
                                            Mathf.CeilToInt((float)dataCount / m_verticalCellCount) * (ItemWidth + m_horizontalSpacing)
                                            - m_horizontalSpacing;
                    m_tmpVectorValue.y = m_itemGroupSize.y;
                    break;
                default:
                    break;
            }
           
            return m_tmpVectorValue;
        }

        private int m_coloumnIndex;
        private int m_rowIndex;
        private int m_dataIndex;
        protected override void SetDataAtViewIndex(int index)
        {
            m_dataIndex = index;
            m_itemsList[index].SetView(m_dataLst[m_dataIndex]);
        }

        protected override Vector2 GetItemPosition(int index)
        {
            switch (m_layoutDirection)
            {
                case LayoutDirection.vertical:
                    if (0 == m_horizontalCellCount)
                    {
                        return m_invalidVector;
                    }
                    m_tmpVectorValue.x = m_leftPadding + (ItemWidth + m_horizontalSpacing) * (index % m_horizontalCellCount);
                    m_tmpVectorValue.y = -m_topPadding -(index / m_horizontalCellCount) * (ItemHeight + m_verticalSpacing);
                    if (index >= m_startPaddingIndex)
                    {
                        m_tmpVectorValue.x += m_horizontalPadding;
                    }
                    break;
                case LayoutDirection.horizontal:
                    if (0 == m_verticalCellCount)
                    {
                        return m_invalidVector;
                    }
                    m_tmpVectorValue.x = m_topPadding + (index / m_verticalCellCount) * (ItemWidth + m_horizontalSpacing);
                    m_tmpVectorValue.y = -m_topPadding - (ItemHeight + m_verticalSpacing) * (index % m_verticalCellCount);
                    if (index >= m_startPaddingIndex)
                    {
                        m_tmpVectorValue.y += m_verticalPadding;
                    }
                    break;
                default:
                    break;
            }
            return m_tmpVectorValue;
        }

        protected override Vector2 GetVisibleItemIndexRange(RectTransform viewportRect)
        {
            Vector2 indexRange = new Vector2();
            switch (m_layoutDirection)
            {
                case LayoutDirection.vertical:
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
                    indexRange.x = Mathf.FloorToInt(upperInvisibleHeight.y / (ItemHeight + m_verticalSpacing)) * m_horizontalCellCount;
                    indexRange.y = Mathf.CeilToInt((upperInvisibleHeight.y + viewPortHeight + m_verticalSpacing) / (ItemHeight + m_verticalSpacing)) * m_horizontalCellCount;
                    break;
                case LayoutDirection.horizontal:
                    // Get the view port size
                    float viewPortWidth = viewportRect.rect.width;
                    // Get upper invisible height
                    Vector2 frontInvisibleWidth = GetCornerAtIndex(viewportRect, 1) - GetCornerAtIndex(m_itemGroupTrans, 1);
                    GetItemLossyScale();
                    if (Vector3.zero != m_itemLossyScale)
                    {
                        frontInvisibleWidth.x /= m_itemLossyScale.x;
                        frontInvisibleWidth.y /= m_itemLossyScale.y;
                    }
                    indexRange.x = Mathf.FloorToInt(frontInvisibleWidth.x / (ItemWidth + m_horizontalSpacing)) * m_verticalCellCount;
                    indexRange.y = Mathf.CeilToInt((frontInvisibleWidth.x + viewPortWidth + m_horizontalSpacing) / (ItemWidth + m_horizontalSpacing)) * m_verticalCellCount;
                    break;
                default:
                    break;
            }

            return indexRange;
        }

        public override void ScrollAtNormalizedPos(float xRate = 0, float yRate = 0)
        {
            Vector2 pos = new Vector2(0, 0);
            int indexOfChild = m_itemGroupTrans.GetSiblingIndex();
            switch (m_layoutDirection)
            {
                case LayoutDirection.vertical:
                    if (indexOfChild > 0)
                    {
                        pos.y += (m_itemGroupSize.y * yRate + m_parentContainerRect.GetChild(indexOfChild - 1).GetComponent<RectTransform>().rect.height);
                    }
                    else
                    {
                        pos.y += m_itemGroupSize.y * yRate;
                    }
                    break;
                case LayoutDirection.horizontal:
                    if (indexOfChild > 0)
                    {
                        pos.x += (m_itemGroupSize.x * xRate + m_parentContainerRect.GetChild(indexOfChild - 1).GetComponent<RectTransform>().rect.width);
                    }
                    else
                    {
                        pos.x += m_itemGroupSize.x * xRate;
                    }
                    break;
                default:
                    break;
            }
            m_parentContainerRect.anchoredPosition = pos;
           
            StartCoroutine(StartUpdateView());
        }

        protected override void RefreshLastRowPosition()
        {
            switch (m_layoutDirection)
            {
                case LayoutDirection.vertical:
                    for (int i = m_dataCount - 1; i >= m_dataCount - m_lastRowItemsCount; i--)
                    {
                        if (ItemsList[i].IsValid())
                        {
                            RectTransform rect = ItemsList[i].transform as RectTransform;
                            rect.anchoredPosition = GetItemPosition(i);
                        }
                    }
                    break;
                case LayoutDirection.horizontal:
                    for (int i = m_dataCount - 1; i >= m_dataCount - m_lastColumnItemsCount; i--)
                    {
                        if (ItemsList[i].IsValid())
                        {
                            RectTransform rect = ItemsList[i].transform as RectTransform;
                            rect.anchoredPosition = GetItemPosition(i);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Data
        protected override void OnDataChange()
        {
            base.OnDataChange();
            if (AlignmentType.Center == m_childAlignment)
            {
                switch (m_layoutDirection)
                {
                    case LayoutDirection.vertical:
                        // 最后一行占了几个，0表示满
                        m_lastRowItemsCount = m_dataCount % m_horizontalCellCount;
                        if (0 == m_lastRowItemsCount)
                        {
                            m_startPaddingIndex = -1;
                            m_horizontalPadding = 0;
                        }
                        else
                        {
                            // 最后一行需要开始填空的index
                            m_startPaddingIndex = m_dataCount - m_lastRowItemsCount;
                            // 最后一行的padding
                            m_horizontalPadding = (m_itemGroupSize.x - (ItemWidth + m_horizontalSpacing) * m_lastRowItemsCount + m_horizontalSpacing) / 2.0f;
                        }
                        break;
                    case LayoutDirection.horizontal:
                        // 最后一列占了几个，0表示满
                        m_lastColumnItemsCount = m_dataCount % m_verticalCellCount;
                        if (0 == m_lastColumnItemsCount)
                        {
                            m_startPaddingIndex = -1;
                            m_verticalPadding = 0;
                        }
                        else
                        {
                            // 最后一列需要开始填空的index
                            m_startPaddingIndex = m_dataCount - m_lastColumnItemsCount;
                            // 最后一列的padding
                            m_verticalPadding = (m_itemGroupSize.y - (ItemHeight + m_verticalSpacing) * m_lastColumnItemsCount) + m_verticalSpacing / 2.0f;
                        }
                        break;
                    default:
                        break;
                }

            }
        }
        #endregion
    }
}

