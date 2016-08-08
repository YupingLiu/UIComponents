using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace MoreFun.UI
{
    public class SpawnItemGroup : UIBehaviour, ILayoutElement, ILayoutGroup
    {
        private Vector2 invalidVisibleRange = new Vector2(-1, -1);

        protected Vector2 InvalidVisibleRange
        {
            get { return invalidVisibleRange; }
        }
        
        protected bool m_ifDeactivateDespawned = false;
        public string poolName;
        private SpawnPool m_itemPool;
        protected virtual int preloadCount { get { return 10; } }

        #region About initializing things
        protected virtual void Awake()
        {
            // data
            m_dataLst   = new List<object>();
            m_objLst    = new List<object>();
            m_dataCount = 0;

            // view
            m_itemGroupSize          = Vector2.zero;
            m_visibleItemsRange      = invalidVisibleRange;
            m_currentShownItemsRange = invalidVisibleRange;
            m_tmpVector2             = Vector2.zero;
            m_spawnIndexLst          = new List<int>();

            m_rectCorners = new Vector3[4];

            // layout
            InitAlignMap();
            m_itemGroupTrans = transform as RectTransform;
            m_itemsList      = new List<SpawnItemBase>();
            m_itemRectList   = new List<RectTransform>();

            SetGroupParent();
            InitItemPrefab();

            // Prefab Pool TODO:
            // 1、允许不deactive
            // 2、允许不broadcast
            // 3、RemovePool
            m_itemPool = SpawnPool.CreateSpawnPool(poolName);
        }

        protected virtual void OnEnable()
        {
            if (m_needRemarkWhenEnable)
            {
                m_needRemarkWhenEnable = false;
                LayoutRebuilder.MarkLayoutForRebuild(m_itemGroupTrans);
            }
        }


        protected virtual void OnDestroy()
        {
            SpawnPool.RemovePool(poolName);
        }

        #endregion 

        #region About data list
        protected List<object> m_dataLst;
        private List<object> m_objLst;
        protected int m_dataCount;
        protected bool m_isDataChanged = false;
        protected bool m_onlyRefreshView = false;
        private bool m_needRemarkWhenEnable = false;

        protected virtual void SetOriginalDataList(List<object> dataList)
        {
            if (null != dataList)
            {
                if (dataList.Count != m_dataCount)
                {
                    m_dataCount = dataList.Count;
                }
                else
                {
                    m_onlyRefreshView = true;
                }

                m_dataLst.Clear();
                m_dataLst.AddRange(dataList);
            }
            else
            {
#if LOG_DETAIL
                ViewDebug.LogError("m_objLst is NULL");
#endif
            }

            if (!m_onlyRefreshView)
            {
                int dataCountDiff = m_dataCount - m_itemsList.Count;
                for (int i = 0; i < dataCountDiff; i++)
                {
                    if (null != m_itemPrefab)
                    {
                        m_itemsList.Add(null);
                    }
                }
            }

            OnDataChange();
        }

        /// <summary>
        /// If the current view 's visible range may not intersect with last view's visible range, you should use this method to reset view
        /// It can happen at circumstances like:
        /// 1 SpawnItemGroup's container include sth except spawn item base, like mop up result popup;
        /// 2 Switch totally bigger count item list and scroll at visible range that bigger than next item list' count in the share view. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList"></param>
        public void SetDataLst<T>(IList<T> dataList)
        {
            // TODO: isvalid
            if (null != dataList)
            {
                if (dataList.Count < m_dataCount)
                {
                    ResetView();
                }

                GetOriginalDataList(dataList);
                SetOriginalDataList(m_objLst);
            }
            else
            {
                RemoveAll();
            }
        }

        public void SetDataLst<T>(IList<T> dataList, bool ifRefreshView)
        {
            if (ifRefreshView)
            {
                SetDataLst<T>(dataList);
            }
            else
            {
                if (null != dataList)
                {
                    GetOriginalDataList(dataList);
                    SetOriginalDataList(m_objLst);
                }
                else
                {
                    RemoveAll();
                }
            }
        }

        private List<object> GetOriginalDataList<T>(IList<T> originalLst)
        {
            m_objLst.Clear();
            for (int i = 0; i < originalLst.Count; i++)
            {
                m_objLst.Add(originalLst[i]);
            }
            return m_objLst;
        }

        protected virtual void OnDataChange()
        {
            // Set Dirty
            m_isDataChanged = true;

            // StartCoroutine(StartUpdateView());
            if (m_onlyRefreshView)
            {
                m_onlyRefreshView = false;
                RefreshView();
            }
            else
            {
                LayoutRebuilder.MarkLayoutForRebuild(m_itemGroupTrans);
                if (!isActiveAndEnabled)
                {
                    m_needRemarkWhenEnable = true;
                }
            }
        }

        protected IEnumerator StartUpdateView()
        {
            yield return new WaitForFixedUpdate();
            UpdateView();
        }

        public virtual void AddItem(object data)
        {
            if (null == data)
            {
                return;
            }
            m_dataLst.Add(data);
            m_dataCount++;
            if (null != m_itemPrefab)
            {
                m_itemsList.Add(null);
            }

            OnDataChange();
        }

        public virtual void AddItemRange(int index, List<object> datas)
        {
            if (datas == null || datas.Count == 0)
                return;

            int count = datas.Count;
            m_dataLst.InsertRange(index, datas);
            m_dataCount += count;
            if (m_itemPrefab != null)
            {
                for (int i = 0; i < count; i++)
                {
                    m_itemsList.Add(null);
                }
            }

            OnDataChange();
        }

        public void RemoveAll()
        {
            if (0 != m_dataCount)
            {
                ResetView();
                m_objLst.Clear();
                SetOriginalDataList(m_objLst);
            }
        }

        protected virtual void ResetView()
        {
            for (int i = 0; i < m_itemRectList.Count; i++)
            {
                m_itemRectList[i].gameObject.MoreSetActive(false, this);
            }
            m_currentShownItemsRange = invalidVisibleRange;
        }

        /// <summary>
        /// No recommended if the obj is the same but fields of the object is different will cause bug
        /// </summary>
        /// <param name="data"></param>
        public void RemoveItem(object data)
        {
            if (!m_dataLst.Contains(data))
            {
                return;
            }

            m_dataLst.Remove(data);
            m_dataCount--;
            
            OnDataChange();
        }

        public virtual void RemoveIndexAt(int index)
        {
            if (index < 0 || index >= m_dataCount)
            {
                return;
            }

            m_dataLst.RemoveAt(index);
            m_dataCount--;

            OnDataChange();
        }

        public void RemoveItemRange(int index, int count)
        {
            if (index < 0 || index >= m_dataCount)
            {
                return;
            }

            m_dataLst.RemoveRange(index, count);
            m_dataCount -= count;

            OnDataChange();
        }

        #endregion

        #region About visible change control
        [SerializeField]
        protected RectTransform m_viewPortRect;
        protected RectTransform m_itemGroupTrans;
        protected Vector2 m_itemGroupSize;
        protected Vector2 m_visibleItemsRange;
        protected Vector2 m_currentShownItemsRange;
        protected Vector2 m_tmpVector2;
        private Vector3[] m_rectCorners;
        protected Vector3 m_itemLossyScale = Vector3.one;
        protected List<int> m_spawnIndexLst;

        protected bool m_allDeactived = false;

        protected void GetItemLossyScale()
        {
            m_itemLossyScale = m_itemGroupTrans.lossyScale;
        }

        protected virtual void UpdateView()
        {
            // Get visible items index range
            m_visibleItemsRange = GetVisibleItemIndexRange(m_viewPortRect);
            // One view can contain all datas
            if (m_visibleItemsRange.y - m_visibleItemsRange.x > m_dataCount)
            {
                if (m_visibleItemsRange.y >0)
	            {
                    if (m_visibleItemsRange.x < 0)
                    {
                        m_visibleItemsRange.x = 0;
                    }
                    m_visibleItemsRange.y = m_visibleItemsRange.x + m_dataCount;
	            }
            }
            // Null item to show
            if (Mathf.Abs(m_visibleItemsRange.x) < 0.1f && Mathf.Abs(m_visibleItemsRange.y) < 0.1f)
            {
                m_visibleItemsRange.x = m_visibleItemsRange.y = -1;
            }
            // Recycle invisible items
            for (int i = (int)m_currentShownItemsRange.x; i < (int)m_currentShownItemsRange.y; i++)
            {
                if (i < m_visibleItemsRange.x || i >= m_visibleItemsRange.y || i >= m_dataCount)
                {
                    DespawnItemAtIndex(i);
                }
            }
            // Spawn new items
            for (int i = (int)m_visibleItemsRange.x; i < (int)m_visibleItemsRange.y; i++)
            {
                if (i < m_currentShownItemsRange.x || i >= m_currentShownItemsRange.y)
                {
                    SpawnItemAtIndex(i);
                }
            }
            
            if (m_isDataChanged)
            {
                m_isDataChanged = false;
                RefreshView();
                if (AlignmentType.Center == m_childAlignment)
                {
                    RefreshLastRowPosition();
                }
            }

            m_currentShownItemsRange = m_visibleItemsRange;
        }

        protected virtual void RefreshLastRowPosition()
        {

        }

        protected virtual void RefreshView()
        {
            for (int i = (int)m_visibleItemsRange.x; i < m_visibleItemsRange.y && i < m_dataCount; i++)
            {
                if (i > -1 && m_itemsList[i].IsValid() && !m_spawnIndexLst.Contains(i))
                {
                    m_itemsList[i].IndexOfItemList = i;
                    SetDataAtViewIndex(i);
                }
            }
            m_spawnIndexLst.Clear();
        }

        protected void DeactiveAllDespawnedItems()
        {
            for (int i = 0; i < m_itemRectList.Count; i++)
            {
                if (m_itemRectList[i].gameObject.activeInHierarchy)
                {
                    m_itemRectList[i].gameObject.MoreSetActive(false, this);  
                }          
            }
            m_allDeactived = true;
        }

        public virtual void ScrollAtNormalizedPos(float x, float y)
        {

        }

        protected Vector3 GetCornerAtIndex(RectTransform rectTrans, int index)
        {
            rectTrans.GetWorldCorners(m_rectCorners);
            return m_rectCorners[index];
        }
        #endregion

        #region About item's spawn and despawn 
        [SerializeField]
        protected GameObject m_itemPrefab;
        protected List<SpawnItemBase> m_itemsList;
        private SpawnItemBase m_tmpSpawnItemBase;

        /// <summary>
        /// Logically item base list, the index of item is same as the index in datalist
        /// </summary>
        public List<SpawnItemBase> ItemsList
        {
            get { return m_itemsList; }
        }
        protected List<RectTransform> m_itemRectList;

        protected virtual void SpawnItemAtIndex(int index)
        {
            if (0 > index || index >= m_dataLst.Count)
            {
                return;
            }

            if (m_isDataChanged)
            {
                m_spawnIndexLst.Add(index);
            }
            if (!m_itemsList[index].IsValid())
            {
                m_itemsList[index] = Spawn();
                RectTransform rect = m_itemsList[index].transform as RectTransform;
                rect.anchoredPosition = GetItemPosition(index);
                if (!m_itemRectList.Contains(rect))
                {
                    m_itemRectList.Add(rect);
                }
            }
            if (!m_itemsList[index].gameObject.activeInHierarchy)
            {
                m_itemsList[index].gameObject.SetActive(true);
            }
            m_itemsList[index].IndexOfItemList = index;
            SetDataAtViewIndex(index);
        }

        protected virtual void SetDataAtViewIndex(int index)
        {
            m_itemsList[index].SetView(m_dataLst[index]);
        }

        protected virtual void DespawnItemAtIndex(int index)
        {
            if (index < 0 || index >= m_itemsList.Count)
            {
                return;
            }

            if (null != m_itemsList[index])
            {
                m_itemsList[index].Reset();

                if (index >= m_dataCount)
                {
                    m_itemsList[index].gameObject.SetActive(false);
                }
                Despawn(m_itemsList[index]);
                m_itemsList[index] = null;
            }
        }

        protected SpawnItemBase Spawn()
        {
            RectTransform rect = null;

            // TODO: Spawn里应该把Instantiate和CreatePool拆分开
            GameObject ret = m_itemPool.Spawn(m_itemPrefab) as GameObject;
            rect = ret.transform as RectTransform;

            if (rect.parent != m_itemGroupTrans)
            {
                rect.SetParent(m_itemGroupTrans);
            }
            rect.localScale = Vector3.one;
            m_tmpSpawnItemBase = rect.GetComponent<SpawnItemBase>();
            if (null == m_tmpSpawnItemBase)
            {
                m_tmpSpawnItemBase = rect.gameObject.AddMissingComponent<SpawnItemBase>();
            }
            return m_tmpSpawnItemBase;
        }

        private void Despawn(SpawnItemBase itemBase)
        {
            m_itemPool.Despawn(itemBase.transform);
            itemBase = null;
        }
        #endregion

        #region About layout
        protected enum AlignmentType
        {
            // default is Left
            Left = 0,
            Center = 1,
            Right = 2,
        }

        protected Dictionary<AlignmentType, Vector2> m_alignAnchorMap;
        private void InitAlignMap()
        {
            m_alignAnchorMap = new Dictionary<AlignmentType, Vector2>();
            m_alignAnchorMap.Add(AlignmentType.Left, new Vector2(0, 1.0f));
            m_alignAnchorMap.Add(AlignmentType.Center, new Vector2(0.5f, 0.5f));
        }

        private float m_itemWidth;

        public float ItemWidth
        {
            get { return m_itemWidth; }
            set { m_itemWidth = value; }
        }
        private float m_itemHeight;

        public float ItemHeight
        {
            get { return m_itemHeight; }
            set { m_itemHeight = value; }
        }
        [SerializeField]
        protected float m_leftPadding;
        [SerializeField]
        protected float m_rightPadding;
        [SerializeField]
        protected float m_topPadding;
        [SerializeField]
        protected float m_bottomPadding;
        [SerializeField]
        protected AlignmentType m_childAlignment = AlignmentType.Left;
        protected float m_horizontalPadding;
        protected float m_verticalPadding;
        protected int m_startPaddingIndex = -1;

        /// <summary>
        /// x of the vector2 means the min index of visible item, y of the vector2 means the max index of the visible item
        /// </summary>
        /// <param name="viewportRect"></param>
        /// <returns></returns>
        protected virtual Vector2 GetVisibleItemIndexRange(RectTransform viewportRect)
        {
            return Vector2.zero;
        }

        protected virtual int GetVisibleItemsCount()
        {
            return 0;
        }

        protected virtual Vector2 GetItemGroupSize(int dataCount)
        {
            return Vector2.zero;
        }

        protected virtual Vector2 GetItemPosition(int index)
        {
            return Vector2.zero;
        }


        protected virtual void InitItemPrefab()
        {
            if (null != m_itemPrefab)
            {
                RectTransform itemPrefabTrans = m_itemPrefab.transform as RectTransform;
                if (null != itemPrefabTrans)
                {
                    m_itemWidth = itemPrefabTrans.rect.width;
                    m_itemHeight = itemPrefabTrans.rect.height;
                    ResetRectTransform(itemPrefabTrans, m_itemWidth, m_itemHeight);
                }
            }
        }


        protected void ResetRectTransform(RectTransform rect, float width, float height)
        {
            Vector2 alignVector = m_alignAnchorMap[AlignmentType.Left];
            rect.pivot = alignVector;
            rect.anchorMin = alignVector;
            rect.anchorMax = alignVector;
            rect.offsetMin = alignVector;
            rect.offsetMax = alignVector;

            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        /// <summary>
        /// <para>If the item group share the container with other, you should override this make it's parent is container not the view port</para>
        /// <para>Default: not sharing, the group itself is the container.</para>
        /// </summary>
        protected virtual void SetGroupParent()
        {
            //if (null == m_itemGroupTrans.parent.GetComponent(typeof(ILayoutGroup)))
            //{
            //    // set parent and align
            //    m_itemGroupTrans.SetParent(m_viewPortRect, false);
            //}
            ResetRectTransform(m_itemGroupTrans, 0, 0);
        }

        #endregion

        #region ILayoutElement

        public virtual float flexibleHeight
        {
            get { return 0; }
        }

        public virtual float flexibleWidth
        {
            get { return 0; }
        }

        public virtual int layoutPriority
        {
            get { return 0; }
        }

        public virtual float minHeight
        {
            get 
            {
                return m_itemGroupSize.y; 
            }
        }

        public virtual float minWidth
        {
            get
            {
                return m_itemGroupSize.x;
            }
        }

        public virtual float preferredHeight
        {
            get { return 0; }
        }

        public virtual float preferredWidth
        {
            get { return 0; }
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
            // The minWidth, preferredWidth, and flexibleWidth values may be calculated in this callback.
            // Refresh the item group size 
            if (m_isDataChanged)
            {
                m_itemGroupSize = GetItemGroupSize(m_dataCount);
                if (null != m_itemGroupTrans)
                {
                    m_tmpVector2.x = m_itemGroupSize.x;
                    m_tmpVector2.y = 0;
                    m_itemGroupTrans.offsetMax = m_tmpVector2;

                    m_tmpVector2.x = 0;
                    m_tmpVector2.y = -m_itemGroupSize.y;
                    m_itemGroupTrans.offsetMin = m_tmpVector2;
                }
            }
        }

        public virtual void CalculateLayoutInputVertical()
        {
            // The minHeight, preferredHeight, and flexibleHeight values may be calculated in this callback.

        }
        #endregion

        #region ILayoutController
        public virtual void SetLayoutHorizontal()
        {
            StartCoroutine(StartUpdateView());
        }

        public virtual void SetLayoutVertical()
        {
        }
        #endregion
    }
}
