﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using MoreFun;

namespace MoreFun.UI
{
    /// <summary>
    /// UIBtnImageChangeColor用于按下和弹起处理的缩放
    /// 使用该脚本需要挂接EventTrigger，处理函数为OnClick
    /// </summary>
    [RequireComponent(typeof(EventTrigger))]
    public class UIBtnImageChangeColor : MoreBehaviour
    {
        public GameObject targetObject;
        public Color clickColor;
        public UIBtnEventType buttonType;
        private EventTrigger m_trigger;
        private bool m_clickDouble = false;
        private Image m_image;
        private Color m_originalColor;

        void Awake()
        {
            m_trigger = GetComponent<EventTrigger>();
            m_image = targetObject.GetComponent<Image>();
            m_originalColor = m_image.color;
        }

        void Start()
        {
            if (buttonType == UIBtnEventType.ONE_UP_ONE_DOWN)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener(OnClick);
                m_trigger.triggers.Add(entry);

                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                entry2.eventID = EventTriggerType.PointerDown;
                entry2.callback.AddListener(OnClick);
                m_trigger.triggers.Add(entry2);
            }
            else if (buttonType == UIBtnEventType.CLICK_CHANGE_CLICK_BACK)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener(OnClick);
                m_trigger.triggers.Add(entry);
            }
        }

        public void OnClick(BaseEventData evd)
        {
            if (m_clickDouble)
            {
                m_image.color = m_originalColor;
            }
            else
            {
                m_image.color = clickColor;
            }
            m_clickDouble = !m_clickDouble;
        }
    }
}