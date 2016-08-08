using UnityEngine;
using System.Collections;
using MoreFun;
public class UIIgnoreTouch : MonoBehaviour {

    private CanvasGroup m_CanvasGroup;
    public bool m_interactable;
    public bool m_blocksRaycasts;
    public bool m_ignoreParentGroups;
    void Awake()
    {
        m_CanvasGroup = gameObject.AddMissingComponent<CanvasGroup>();
        m_CanvasGroup.interactable = m_interactable;
        m_CanvasGroup.ignoreParentGroups = m_blocksRaycasts;
        m_CanvasGroup.blocksRaycasts = m_ignoreParentGroups;
    }

    void OnEnable()
    {
        m_CanvasGroup.interactable = m_interactable;
        m_CanvasGroup.ignoreParentGroups = m_blocksRaycasts;
        m_CanvasGroup.blocksRaycasts = m_ignoreParentGroups;
    }


}
