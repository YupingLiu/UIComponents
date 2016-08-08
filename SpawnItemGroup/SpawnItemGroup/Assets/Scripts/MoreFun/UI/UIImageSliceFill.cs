using UnityEngine;
using System.Collections;
using MoreFun;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIImageSliceFill : MoreBehaviour
{
    [SerializeField]
    private float m_MaxWidth = 0.0f;
    [SerializeField][Range(0.0f, 1.0f)]
    private float m_Progress;

    private float m_ExcuteProgress;
    void Awake()
    {
        if (0.0f == m_MaxWidth)
        {
            m_MaxWidth = cachedRectTransform.sizeDelta.x;
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {
        cachedRectTransform.pivot = new Vector2(0.0f, 0.5f);
        cachedRectTransform.anchorMin = new Vector2(0.0f, 0.5f);
        cachedRectTransform.anchorMax = new Vector2(0.0f, 0.5f);

        //cachedRectTransform.anchoredPosition = Vector2.zero;
    }
	
    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_Progress != m_ExcuteProgress)
        {
            SetProgress(m_Progress);
        }
    }

    public virtual void SetProgress( float progress )
    {
        if(null != cachedRectTransform.parent && null != cachedRectTransform.parent.GetComponent<Mask>())
        {
            Rect parentRect = (cachedRectTransform.parent.transform as RectTransform).rect;
            m_MaxWidth = Mathf.Abs(parentRect.width);
        }

        m_Progress = m_ExcuteProgress = progress;
        cachedRectTransform.sizeDelta = new Vector2(m_MaxWidth * m_Progress, cachedRectTransform.sizeDelta.y);
    }
}

