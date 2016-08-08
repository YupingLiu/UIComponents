using UnityEngine;
using System.Collections;
using MoreFun;

namespace MoreFun.UI
{
    /// <summary>
    /// UIGridLayoutGroupHeightAdapter 用于适配GridLayoutGroup的高度
    /// 需要手动调用其UpdateHeight来计算
    /// add: 如果不是GridLayoutGroup则兼任布局重任，布局：anchor(0.5, 1), anchorPos(-height/2 * siblings' heights)
    /// </summary>
    public class UIGridLayoutGroupHeightAdapter : MoreBehaviour
    {
        [SerializeField]
        private bool useGrid;
        [SerializeField]
        private int width;
        [SerializeField]
        private int height;
        [SerializeField]
        private int heightSpace;
        [SerializeField]
        private int extraHeight;
        [SerializeField]
        private int bottomExtraHeight = 0;
        [SerializeField]
        private bool helpLayoutChildren = false;

        [SerializeField]
        private float heightMin = 0;

        void Awake()
        {
            if (heightMin > 0.001f)
            {
                heightMin = gameObject.GetComponent<RectTransform>().rect.height;
            }
        }

        public void UpdateHeight()
        {
            if (true == useGrid)
            {
                int childCnt = cachedTransform.childCount;
                int numPerRow = (int)cachedRectTransform.rect.width / width;
                int rowNum = (childCnt + numPerRow - 1) / numPerRow;
                cachedRectTransform.sizeDelta = new Vector2(cachedRectTransform.sizeDelta.x, height * rowNum + heightSpace * (rowNum - 1) + extraHeight);
            }
            else
            {
                int allHeight = 0;
                for (int i = 0; i < cachedTransform.childCount; ++i)
                {
                    RectTransform trans = cachedTransform.GetChild(i) as RectTransform;
                    if (trans != null)
                    {
                        if (helpLayoutChildren)
                        {
                            trans.anchorMin = new Vector3(0.5f, 1); //向上居中对齐
                            trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, -1 * (allHeight + trans.rect.height*0.5f));
                        }
                        allHeight += ((int)trans.rect.height + heightSpace);
                    }
                }

                float h = allHeight + extraHeight + bottomExtraHeight;
                if (h < heightMin)
                {
                    h = heightMin;
                }
                cachedRectTransform.sizeDelta = new Vector2(cachedRectTransform.sizeDelta.x, h);
                cachedRectTransform.anchoredPosition = new Vector2(cachedRectTransform.anchoredPosition.x, -0.5f * (allHeight + extraHeight));
                if (helpLayoutChildren)
                {
                    cachedRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    cachedRectTransform.anchoredPosition = new Vector2(cachedRectTransform.anchoredPosition.x, cachedRectTransform.sizeDelta.y*-0.5f);
                }
            }

            if (cachedTransform.parent != null)
            {
                UIGridLayoutGroupHeightAdapter adapter = cachedTransform.parent.GetComponent<UIGridLayoutGroupHeightAdapter>();
                if (adapter != null)
                {
                    adapter.UpdateHeight();
                }
            }

        }
    }
}

