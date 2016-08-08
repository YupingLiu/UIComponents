using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoreFun.UI
{
    public static class AllowDeactiveLayoutUtility
    {
        public static float GetMinSize(RectTransform rect, int axis, bool allowDeactive)
        {
            if (axis == 0)
                return GetMinWidth(rect, allowDeactive);
            return GetMinHeight(rect, allowDeactive);
        }
        
        public static float GetPreferredSize(RectTransform rect, int axis, bool allowDeactive)
        {
            if (axis == 0)
                return GetPreferredWidth(rect, allowDeactive);
            return GetPreferredHeight(rect, allowDeactive);
        }
        
        public static float GetFlexibleSize(RectTransform rect, int axis, bool allowDeactive)
        {
            if (axis == 0)
                return GetFlexibleWidth(rect, allowDeactive);
            return GetFlexibleHeight(rect, allowDeactive);
        }
        
        public static float GetMinWidth(RectTransform rect, bool allowDeactive)
        {
            return GetLayoutProperty(rect, e => e.minWidth, 0, allowDeactive);
        }
        
        public static float GetPreferredWidth(RectTransform rect, bool allowDeactive)
        {
            return Mathf.Max(GetLayoutProperty(rect, e => e.minWidth, 0, allowDeactive), 
                             GetLayoutProperty(rect, e => e.preferredWidth, 0, allowDeactive));
        }
        
        public static float GetFlexibleWidth(RectTransform rect, bool allowDeactive)
        {
            return GetLayoutProperty(rect, e => e.flexibleWidth, 0, allowDeactive);
        }
        
        public static float GetMinHeight(RectTransform rect, bool allowDeactive)
        {
            return GetLayoutProperty(rect, e => e.minHeight, 0, allowDeactive);
        }
        
        public static float GetPreferredHeight(RectTransform rect, bool allowDeactive)
        {
            return Mathf.Max(GetLayoutProperty(rect, e => e.minHeight, 0, allowDeactive), 
                             GetLayoutProperty(rect, e => e.preferredHeight, 0, allowDeactive));
        }
        
        public static float GetFlexibleHeight(RectTransform rect, bool allowDeactive)
        {
            return GetLayoutProperty(rect, e => e.flexibleHeight, 0, allowDeactive);
        }
        
        public static float GetLayoutProperty(RectTransform rect, System.Func<ILayoutElement, float> property, float defaultValue,
                                              bool allowDeactive)
        {
            ILayoutElement dummy;
            return GetLayoutProperty(rect, property, defaultValue, out dummy, allowDeactive);
        }
        
        public static float GetLayoutProperty(RectTransform rect, System.Func<ILayoutElement, float> property, float defaultValue, out ILayoutElement source,
                                              bool allowDeactive)
        {
            source = null;
            if (rect == null)
                return 0;
            float min = defaultValue;
            int maxPriority = System.Int32.MinValue;
            var components = MoreComponentListPool.Get();
            rect.GetComponents(typeof(ILayoutElement), components);
            
            for (int i = 0; i < components.Count; i++)
            {
                var layoutComp = components[i] as ILayoutElement;
                if (!allowDeactive && layoutComp is Behaviour && (!(layoutComp as Behaviour).enabled || !(layoutComp as Behaviour).isActiveAndEnabled))
                    continue;
                
                int priority = layoutComp.layoutPriority;
                // If this layout components has lower priority than a previously used, ignore it.
                if (priority < maxPriority)
                    continue;
                float prop = property(layoutComp);
                // If this layout property is set to a negative value, it means it should be ignored.
                if (prop < 0)
                    continue;
                
                // If this layout component has higher priority than all previous ones,
                // overwrite with this one's value.
                if (priority > maxPriority)
                {
                    min = prop;
                    maxPriority = priority;
                    source = layoutComp;
                }
                // If the layout component has the same priority as a previously used,
                // use the largest of the values with the same priority.
                else if (prop > min)
                {
                    min = prop;
                    source = layoutComp;
                }
            }
            
            MoreComponentListPool.Release(components);
            return min;
        }
    }
}
