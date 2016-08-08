using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using MoreFun;

namespace MoreFun.UI
{
    /// <summary>
    /// UIButtonChangeImage用于在按下按钮的时候切换按钮上的图片，分为两种类型:
    /// 1. 按下换图，弹起恢复；
    /// 2. 按下换图，再次按下重新换图。
    ///  
    /// 需要指定normal状态和select状态下的图片
    /// </summary>
    public class UIBtnChangeSprite : MoreBehaviour
    {
        public UIBtnEventType buttonType;
        public Image m_Image;
        public Sprite m_NormalSprite;
        public Sprite m_SelectSprite;
        public Sprite m_NotClickSprite;

        public Text m_text;
        public Color m_tNormalColor = new Color(227/255.0f,235/255.0f,255/255.0f,1.0f);
        public Color m_tSelectColor = Color.white;
        public Color m_tNotClickColor = new Color(153 / 255.0f, 156 / 255.0f, 153 / 255.0f, 1.0f);

        private EventTrigger m_trigger;
        private bool m_clickDouble = false;
        private bool m_isCanClick = true;
        void Awake()
        {
            m_trigger = gameObject.AddMissingComponent<EventTrigger>();
            if (buttonType == UIBtnEventType.ONE_UP_ONE_DOWN)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener(OnClickUp);
                m_trigger.triggers.Add(entry);

                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                entry2.eventID = EventTriggerType.PointerDown;
                entry2.callback.AddListener(OnClickDown);
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

        public void SetCanClick(bool isCanClick)
        {
            if(m_isCanClick == isCanClick)
            {
                return;
            }
            m_trigger.enabled = isCanClick;
            if (m_Image != null)
            {
                if (isCanClick)
                {
                    if (m_NormalSprite != null)
                        m_Image.sprite = m_NormalSprite;
                    if (null != m_text)
                        m_text.color = m_tNormalColor;
                }
                else
                {
                    if (m_NotClickSprite != null)
                        m_Image.sprite = m_NotClickSprite;
                    if (null != m_text)
                        m_text.color = m_tNotClickColor;
                }
            }
            m_isCanClick = isCanClick;
        }

        public void OnClick(BaseEventData evd)
        {
            if (m_Image != null)
            {
                if (m_clickDouble)
                {
                    if (m_NormalSprite != null)
                        m_Image.sprite = m_NormalSprite;

                    if (null != m_text)
                        m_text.color = m_tNormalColor;
                }
                else
                {
                    if (m_SelectSprite != null)
                        m_Image.sprite = m_SelectSprite;

                    if (null != m_text)
                        m_text.color = m_tSelectColor;
                }
                m_clickDouble = !m_clickDouble;
            }
        }

        public void OnClickDown(BaseEventData evd)
        {
            if (m_Image != null)
            {
                if (m_SelectSprite != null)
                    m_Image.sprite = m_SelectSprite;
                if (null != m_text)
                    m_text.color = m_tSelectColor;
            }
        }

        public void OnClickUp(BaseEventData evd)
        {
            if (m_Image != null)
            {
                if (m_NormalSprite != null)
                    m_Image.sprite = m_NormalSprite;
                if (null != m_text)
                    m_text.color = m_tNormalColor;
            }
        }
    }
}