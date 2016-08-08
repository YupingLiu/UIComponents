using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
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
    /// wenhuo:对强哥btn组件的扩展 支持interactable=true/false时状态切换以及事件移除
    [RequireComponent(typeof(EventTrigger))]
    public class UIBtnChangeImageEx : MoreBehaviour
    {
        public UIBtnEventType buttonType;
        public GameObject[] NormalImage;
        public GameObject[] PressImage;
        public GameObject[] DisableImage;
        public Image GrayImage;
        public float m_saturateValue = 0.0f;
        private Material m_grayMoreMaterial;//需要自己实例化出来的MoreSaturate的材质
        private Material m_grayMaterial;
        private EventTrigger m_trigger;
        private List<EventTrigger.Entry> m_eventTriggerEntryList = new List<EventTrigger.Entry>();
        private bool m_clickDouble = false;
        private Material m_defaultImageMat;
        private bool m_grayEnable = false;
        private bool m_interactable = false;

        private bool m_needAutoSet = true;

        void Awake()
        {
            m_trigger = GetComponent<EventTrigger>();
            if (null != NormalImage && NormalImage.Length > 0)
            {
                Image iNormal = NormalImage[0].GetComponent<Image>();
                if (null != iNormal)
                {
                    m_defaultImageMat = iNormal.material;
                }
            }            

            if (null != GrayImage)
            {
                m_grayEnable = true;
                if(null != GrayImage.material)
                {
                    //bool isMoreSprite = GrayImage.sprite.name.Contains(".alpha");
                    //Material moreMaterial = MoreMaterialsManager.Instance.GetMaterial(GrayImage.material, isMoreSprite);
                    //m_grayImageMaterial = Instantiate(moreMaterial) as Material;
                    m_grayMoreMaterial = MoreMaterialsManager.Instance.GetMaterial(GrayImage.material,true);
                    m_grayMoreMaterial.SetFloat("_Saturate", m_saturateValue);//设置为灰色的材质
                    m_grayMaterial = MoreMaterialsManager.Instance.GetMaterial(GrayImage.material, false);
                    m_grayMaterial.SetFloat("_Saturate", m_saturateValue);//设置为灰色的材质
                    //Debug.LogError("m_grayImageMaterial : " + m_grayImageMaterial);
                }
            }
        }

        void Start()
        {
            if (m_needAutoSet)
            {
                interactable = true;
            }
            else
            {
                interactable = m_interactable;
            }
        }


        void OnDestroy()
        {
            m_eventTriggerEntryList.Clear();
        }


        private void AddTrigger()
        {
            if (buttonType == UIBtnEventType.ONE_UP_ONE_DOWN)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener(OnClickUp);
                m_trigger.triggers.Add(entry);
                m_eventTriggerEntryList.Add(entry);

                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                entry2.eventID = EventTriggerType.PointerDown;
                entry2.callback.AddListener(OnClickDown);
                m_trigger.triggers.Add(entry2);
                m_eventTriggerEntryList.Add(entry2);
            }
            else if (buttonType == UIBtnEventType.CLICK_CHANGE_CLICK_BACK)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener(OnClick);
                m_trigger.triggers.Add(entry);
                m_eventTriggerEntryList.Add(entry);
            }
        }

        private void RemoveTrigger()
        {
            for (int i = 0; i < m_eventTriggerEntryList.Count; ++i)
            {
                m_trigger.triggers.Remove(m_eventTriggerEntryList[i]);
            }
            m_eventTriggerEntryList.Clear();
        }


        public bool interactable
        {
            get
            {
                return m_interactable;
            }

            set
            {
                if (m_interactable)
                {
                    if (!value)
                    {
                        RemoveTrigger();
                    }
                }
                else
                {
                    if (value)
                    {
                        AddTrigger();
                    }
                }
                m_interactable = value;
                UpdateState(value);
                m_needAutoSet = false;
            }
        }

        public void UpdateState(bool interactable = true)
        {
            if (m_grayEnable)
            {
                if (interactable)
                {
                    if (null != DisableImage && DisableImage.Length > 0)
                    {
                        for (int i = 0; i < DisableImage.Length; ++i)
                        {
                            if (null != DisableImage[i])
                            {
                                DisableImage[i].SetActive(false);
                            }
                        }
                    }

                    if (null != PressImage && PressImage.Length > 0)
                    {
                        for (int i = 0; i < PressImage.Length; ++i)
                        {                            
                            if (null != PressImage[i])
                            {
                                Image im = PressImage[i].GetComponent<Image>();
                                if (null != im)
                                {
                                    im.material = m_defaultImageMat;
                                }
                            }
                        }
                    }

                    if (null != NormalImage && NormalImage.Length > 0)
                    {
                        for (int i = 0; i < NormalImage.Length; ++i)
                        {
                            if (null != NormalImage[i])
                            {
                                NormalImage[i].SetActive(true);
                                Image im = NormalImage[i].GetComponent<Image>();
                                if (null != im)
                                {
                                    im.material = m_defaultImageMat;
                                }
                            }
                        }
                    }
                }
                else
                {
                    m_grayMoreMaterial.SetFloat("_Saturate", m_saturateValue);//设置为灰色的材质
                    m_grayMaterial.SetFloat("_Saturate", m_saturateValue);//设置为灰色的材质

                    if (null != NormalImage && NormalImage.Length > 0)
                    {                      
                        for (int i = 0; i < NormalImage.Length; ++i)
                        {
                            if (null != NormalImage[i])
                            {
                                NormalImage[i].SetActive(false);
                            }
                        }
                    }

                    if (null != DisableImage && DisableImage.Length > 0)
                    {
                        for (int i = 0; i < DisableImage.Length; ++i)
                        {
                            if (null != DisableImage[i])
                            {
                                DisableImage[i].SetActive(true);
                                Image im = DisableImage[i].GetComponent<Image>();
                                if (null != im)
                                {
                                    //Debug.LogError(gameObject.name + " : -------------m_grayImageMaterial : " + m_grayImageMaterial + " , folat " + m_grayImageMaterial.GetFloat("_Saturate"));
                                    if (im.sprite != null && im.sprite.name.Contains(".alpha"))
                                    {
                                        im.material = m_grayMoreMaterial;
                                    }
                                    else
                                    {
                                        im.material = m_grayMaterial;
                                    }
                                    //Debug.LogError("im.material : " + im.material + " , folat " + im.material.GetFloat("_Saturate"));
                                    im.SetAllDirty();                                   
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (interactable)
                {
                    if (null != DisableImage && DisableImage.Length > 0)
                    {
                        for (int i = 0; i < DisableImage.Length; ++i)
                        {
                            if (null != DisableImage[i])
                            {
                                DisableImage[i].SetActive(false);
                            }
                        }
                    }

                    if (null != NormalImage && NormalImage.Length > 0)
                    {
                        for (int i = 0; i < NormalImage.Length; ++i)
                        {
                            if (null != NormalImage)
                            {
                                NormalImage[i].SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    if (null != NormalImage && NormalImage.Length > 0)
                    {
                        for (int i = 0; i < NormalImage.Length; ++i)
                        {
                            if (null != NormalImage[i])
                            {
                                NormalImage[i].SetActive(false);
                            }
                        }
                    }

                    if (null != DisableImage && DisableImage.Length > 0)
                    {
                        for (int i = 0; i < DisableImage.Length; ++i)
                        {
                            if (null != DisableImage[i])
                            {
                                DisableImage[i].SetActive(true);
                            }
                        }
                    }
                }
            }
        }

        public void OnClick(BaseEventData evd)
        {
            if (m_clickDouble)
            {
                if (null != NormalImage && NormalImage.Length > 0)
                {
                    for (int i = 0; i < NormalImage.Length; ++i)
                    {
                        if (null != NormalImage[i])
                        {
                            NormalImage[i].SetActive(false);
                        }
                    }
                }
                if (null != PressImage && PressImage.Length > 0)
                {
                    for (int i = 0; i < PressImage.Length; ++i)
                    {
                        if (null != PressImage[i])
                        {
                            PressImage[i].SetActive(true);
                        }
                    }
                }
            }
            else
            {
                if (null != PressImage && PressImage.Length > 0)
                {
                    for (int i = 0; i < PressImage.Length; ++i)
                    {
                        if (null != PressImage[i])
                        {
                            PressImage[i].SetActive(false);
                        }
                    }
                }
                if (null != NormalImage && NormalImage.Length > 0)
                {
                    for (int i = 0; i < NormalImage.Length; ++i)
                    {
                        if (null != NormalImage[i])
                        {
                            NormalImage[i].SetActive(true);
                        }
                    }
                }                
            }
            m_clickDouble = !m_clickDouble;
        }

        public void OnClickDown(BaseEventData evd)
        {
            if (null != NormalImage && NormalImage.Length > 0)
            {
                for (int i = 0; i < NormalImage.Length; ++i)
                {
                    if (null != NormalImage[i])
                    {
                        NormalImage[i].SetActive(false);
                    }
                }
            }
            if (null != PressImage && PressImage.Length > 0)
            {
                for (int i = 0; i < PressImage.Length; ++i)
                {
                    if (null != PressImage[i])
                    {
                        PressImage[i].SetActive(true);
                    }
                }
            }
        }

        public void OnClickUp(BaseEventData evd)
        {
            if (null != PressImage && PressImage.Length > 0)
            {
                for (int i = 0; i < PressImage.Length; ++i)
                {
                    if (null != PressImage[i])
                    {
                        PressImage[i].SetActive(false);
                    }
                }
            }
            if (null != NormalImage && NormalImage.Length > 0)
            {
                for (int i = 0; i < NormalImage.Length; ++i)
                {
                    if (null != NormalImage[i])
                    {
                        NormalImage[i].SetActive(true);
                    }
                }
            }                
        }
    }
}