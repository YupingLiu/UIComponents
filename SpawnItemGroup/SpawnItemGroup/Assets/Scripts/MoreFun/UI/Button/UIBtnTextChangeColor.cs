using UnityEngine;
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
    public class UIBtnTextChangeColor : MoreBehaviour
    {
        public GameObject targetObject;
        public Color clickColor;
        public Color disableColor;
        public UIBtnEventType buttonType;
        private EventTrigger m_trigger;
        private bool m_clickDouble = false;
        private Text m_text;
        private Color m_originalColor;

        private bool m_interactable = false;


        void Awake()
        {
            m_trigger = GetComponent<EventTrigger>();
            m_text = targetObject.GetComponent<Text>();
            m_originalColor = m_text.color;
            interactable = true;
        }

        void Start()
        {
            //interactable = true;           
        }

        void AddTrigger()
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

        public bool interactable
        {
            get
            {
                return m_interactable;
            }

            set 
            {               
                Button button = gameObject.GetComponent<Button>();
                if (null != button)
                {
                    button.interactable = value;
                }

                if (value)
                {
                    if (!m_interactable)
                    {
                        AddTrigger();
                    }
                    m_text.color = m_originalColor;
                }
                else
                {
                    m_trigger.triggers.Clear();
                    m_text.color = disableColor;                   
                }
                m_interactable = value;
            }
        }

        public void SetText(string text)
        {
            m_text.text = text;
        }

        public void OnClick(BaseEventData evd)
        {
            if (m_clickDouble)
            {
                m_text.color = m_originalColor;
            }
            else
            {
                m_text.color = clickColor;
            }
            m_clickDouble = !m_clickDouble;
        }
    }
}