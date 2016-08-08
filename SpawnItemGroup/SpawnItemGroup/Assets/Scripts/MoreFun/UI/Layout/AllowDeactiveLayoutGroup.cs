using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace MoreFun.UI
{
    public abstract class AllowDeactiveLayoutGroup : LayoutGroup
    {
        [SerializeField]
        protected bool allowDeactiveChild = true;

        public override void CalculateLayoutInputHorizontal()
        {
            rectChildren.Clear();
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                RectTransform rect = rectTransform.GetChild(i) as RectTransform;
                if (rect == null)
                    continue;
                ILayoutIgnorer ignorer = rect.GetComponent(typeof(ILayoutIgnorer)) as ILayoutIgnorer;
                if ((allowDeactiveChild || rect.gameObject.activeInHierarchy) &&
                    !(ignorer != null && ignorer.ignoreLayout))
                    rectChildren.Add(rect);
            }
            
            m_Tracker.Clear();
        }

    }
    

}