using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace MoreFun.UI
{
    /// <summary>
    /// UIScrollViewSelector用于挂接在ScrollView控件上，通知孩子节点发生了点击事件
    /// 使用方法：
    /// 1. ScrollView控件上需要挂接UIScrollViewSelector Component；
    /// 5. 每个ScrollViewItem山需要挂接IScrollViewSelector Component,并实现其中的Select方法；
    /// 6. 当发生点击事件后，ScrollViewItem会通知下面的每个IScrollViewSelector，并通知点击点和其中心点的offset(UI坐标系)。
    /// 
    /// 7.TNQIANG ADD,增加点击按下状态，如果需要响应按下状态，则需要添加PointerClickDown事件
    /// 8.TNQIANG ADD,事件不需要自己来添加了，在Start的时候会自动进行添加. 注意：不要自己来进行添加事件响应函数，否则效果会与想象不同
    /// </summary>
    [RequireComponent(typeof(EventTrigger))]
    public class UIScrollViewSelector : MonoBehaviour
    {
        public int resolutionWidth = 1136;
        public int resolutionHeight = 640;
        private Vector2 UIScreen;
        private bool isDrag = false;
        private EventTrigger m_trigger;
        private List<IScrollViewSelector> m_lstSelector = new List<IScrollViewSelector>();

        // Use this for initialization
        void Start()
        {
            UIScreen = new Vector2(resolutionHeight * Screen.width / Screen.height, resolutionHeight);
            m_trigger = GetComponent<EventTrigger>();

            EventTrigger.Entry entry1 = new EventTrigger.Entry();
            entry1.eventID = EventTriggerType.PointerDown;
            entry1.callback.AddListener(OnClickDown);
            m_trigger.triggers.Add(entry1);

            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerUp;
            entry2.callback.AddListener(OnClickUp);
            m_trigger.triggers.Add(entry2);

            EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry3.eventID = EventTriggerType.Drag;
            entry3.callback.AddListener(OnDrag);
            m_trigger.triggers.Add(entry3);
        }

        // 注意：之前使用的是值相等进行判断，当selector的值发生变化后可能导致相等判断不通过
        // 现在改成引用相等的方式进行判断，相当于使用地址进行判断。
        public void AddISelector(IScrollViewSelector selector)
        {
            foreach (IScrollViewSelector item in m_lstSelector)
            {
                if (ReferenceEquals(item, selector))
                {
                    m_lstSelector.Remove(item);
                    break;
                }
            }
            m_lstSelector.Add(selector);
        }

        public void RemoveSelector(IScrollViewSelector selector)
        {
            foreach (IScrollViewSelector item in m_lstSelector)
            {
                if (ReferenceEquals(item, selector))
                {
                    m_lstSelector.Remove(item);
                    break;
                }
            }
        }

        // 这个函数需要响应ScrollRect所在Componet所在层的Drag事件
        public void OnDrag(BaseEventData eventData)
        {
            isDrag = true;
        }

        // 这个函数需要响应ScrollRect所在Component所在层的PointerDown事件
        public void OnClickDown(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector2 pos = ped.position;

            pos.x = pos.x * UIScreen.x / Screen.width - UIScreen.x / 2;
            pos.y = pos.y * UIScreen.y / Screen.height - UIScreen.y / 2;
            foreach (IScrollViewSelector item in m_lstSelector)
            {
                if (item != null)
                {
                    RectTransform trans = item.transform as RectTransform;
                    Vector2 itemPos = UITools.GetUIPositionInWorld(trans);
                    Vector2 offset;
                    offset.x = pos.x - itemPos.x;
                    offset.y = pos.y - itemPos.y;
                    item.Selected(offset, SelectAction.TOUCH_DOWN);
                }
            }
            isDrag = false;
        }

        // 这个函数需要响应ScrollRect所在Component所在层的PointerUp事件
        public void OnClickUp(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector2 pos = ped.position;

            if (isDrag)
            {
                foreach (IScrollViewSelector item in m_lstSelector)
                {
                    if (item != null)
                    {
                        RectTransform trans = item.transform as RectTransform;
                        Vector2 itemPos = UITools.GetUIPositionInWorld(trans);
                        Vector2 offset;
                        offset.x = pos.x - itemPos.x;
                        offset.y = pos.y - itemPos.y;
                        item.Selected(offset, SelectAction.DRAG_UP);
                    }
                }
            }
            else
            {
                pos.x = pos.x * UIScreen.x / Screen.width - UIScreen.x / 2;
                pos.y = pos.y * UIScreen.y / Screen.height - UIScreen.y / 2;
                foreach (IScrollViewSelector item in m_lstSelector)
                {
                    if (item != null)
                    {
                        RectTransform trans = item.transform as RectTransform;
                        Vector2 itemPos = UITools.GetUIPositionInWorld(trans);
                        Vector2 offset;
                        offset.x = pos.x - itemPos.x;
                        offset.y = pos.y - itemPos.y;
                        item.Selected(offset, SelectAction.TOUCH_UP);
                    }
                }
            }
        }
    }
}