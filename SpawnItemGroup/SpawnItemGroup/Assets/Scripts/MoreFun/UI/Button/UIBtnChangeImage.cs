using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using MoreFun;

namespace MoreFun.UI
{
    public delegate void EventTriggerHandler(BaseEventData evd);
    /// <summary>
    /// UIButtonChangeImage用于在按下按钮的时候切换按钮上的图片，分为两种类型:
    /// 1. 按下换图，弹起恢复；
    /// 2. 按下换图，再次按下重新换图。
    ///  
    /// 需要指定normal状态和select状态下的图片
    /// </summary>
    [RequireComponent(typeof(EventTrigger))]
    public class UIBtnChangeImage : MoreBehaviour
    {
        public UIBtnEventType buttonType;
        public GameObject NormalImage;
        public GameObject SelectImage;
        public GameObject DisableImage;
        public Image GrayImage;
        public bool NeedClearTrigger = false;
        private EventTrigger m_trigger;
        private bool m_clickDouble = false;
        private Image m_normalImage;
        private Image m_selectImage;
        private Material m_normalImageMat;
        private Material m_selectImageMat;
        private bool m_grayEnable = false;
        private bool m_interactable = false;
        private bool m_enable = true;

        private bool m_needAutoSet = true;

        void Awake()
        {
            m_trigger = GetComponent<EventTrigger>();
            if (null != NormalImage)
            {
                m_normalImage = NormalImage.GetComponent<Image>();
                if (null != m_normalImage)
                {
                    m_normalImageMat = m_normalImage.material;
                }
            }
            if (null != SelectImage)
            {
                m_selectImage = SelectImage.GetComponent<Image>();
                if (null != m_selectImage)
                {
                    m_selectImageMat = m_selectImage.material;
                }
            }
            if (null != GrayImage)
            {
                m_grayEnable = true;
            }
        }

        void Start()
        {
            if (NeedClearTrigger)
            {
                
            }
            else
            {
                AddTrigger();
            }

            if (m_needAutoSet)
            {
                interactable = true;
            }
            else
            {
                interactable = m_interactable;
            }
        }


        private void AddTrigger()
        {
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


        public bool interactable
        {
            get
            {
                return m_interactable;
            }

            set
            {
                if (NeedClearTrigger)
                {
                    if (value && !m_interactable)
                    {
                        AddTrigger();
                    }
                }
                m_interactable = value;
                UpdateState(value);
                m_needAutoSet = false;
            }
        }

        public bool opEnable
        {
            get
            {
                return m_enable;
            }

            set
            {
                m_enable = value;
                if (value)
                {
                    if (NormalImage != null) NormalImage.SetActive(true);
                    if (SelectImage != null) SelectImage.SetActive(false);
                }
                else
                {
                    if (NormalImage != null) NormalImage.SetActive(false);
                    if (SelectImage != null) SelectImage.SetActive(false);
                }
            }
        }

        public void UpdateState(bool interactable = true)
        {
            if (m_grayEnable)
            {
                if (interactable)
                {
                    if (null != m_normalImage)
                    {
                        m_normalImage.material = m_normalImageMat;
                    }

                    if (null != m_selectImage)
                    {
                        m_selectImage.material = m_selectImageMat;
                    }
                }
                else
                {
                    GrayImage.material.SetFloat("_Saturate", 0.0f);

                    if (null != m_normalImage)
                    {
                        m_normalImage.material = GrayImage.material;
                        m_normalImage.SetAllDirty();
                    }

                    if (null != m_selectImage)
                    {
                        m_selectImage.material = GrayImage.material;
                        m_selectImage.SetAllDirty();
                    }
                }
            }
            else
            {
                if (interactable)
                {
                    if (m_enable)
                    {
                        if (NormalImage != null) NormalImage.SetActive(true);
                        if (SelectImage != null) SelectImage.SetActive(false);
                    }                    
                    if (DisableImage != null) DisableImage.SetActive(false);
                }
                else
                {
                    if (m_enable)
                    {
                        if (NormalImage != null) NormalImage.SetActive(false);
                        if (SelectImage != null) SelectImage.SetActive(false);
                    }
                    if (DisableImage != null) DisableImage.SetActive(true);
                }
            }
        }

        public void OnClick(BaseEventData evd)
        {
            if (m_enable)
            {
                if (m_clickDouble)
                {
                    if (NormalImage != null) NormalImage.SetActive(true);
                    if (SelectImage != null) SelectImage.SetActive(false);
                }
                else
                {
                    if (NormalImage != null) NormalImage.SetActive(false);
                    if (SelectImage != null) SelectImage.SetActive(true);
                }
                m_clickDouble = !m_clickDouble;
            }
        }

        public void OnClickDown(BaseEventData evd)
        {
            if (m_enable)
            {
                if (NormalImage != null) NormalImage.SetActive(false);
                if (SelectImage != null) SelectImage.SetActive(true);
            }
        }

        public void OnClickUp(BaseEventData evd)
        {
            if (m_enable)
            {
                if (NormalImage != null) NormalImage.SetActive(true);
                if (SelectImage != null) SelectImage.SetActive(false);
            }
        }
    }
}