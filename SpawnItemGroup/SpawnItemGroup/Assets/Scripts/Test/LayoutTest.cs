using UnityEngine;
using System.Collections.Generic;
using MoreFun.UI;
using UnityEngine.UI;

public class LayoutTest : MonoBehaviour {

    private List<object> m_testDataLst;
    private int m_testDataCount;
    [SerializeField]
    protected SpawnItemGroup m_spawnItemGroup;
    [SerializeField]
    protected ScrollRect m_scrollRect;
    [SerializeField]
    protected Text m_scrollRate;
    protected float m_rate;

    void Awake()
    {
        m_testDataLst = new List<object>();
    }

    public void SetData()
    {
        for (int i = 0; i < 7; i++)
        {
            m_testDataLst.Add(i);
        }
        m_testDataCount = m_testDataLst.Count;
        m_spawnItemGroup.SetDataLst(m_testDataLst);
        m_scrollRect.normalizedPosition = new Vector2(0.0f, 1.0f);
    }

    public void OnClickToAdd()
    {
        m_spawnItemGroup.AddItem(m_testDataCount);
        m_testDataLst.Add(m_testDataCount);
        m_testDataCount++;
        m_scrollRect.normalizedPosition = new Vector2(0.0f, 1.0f);
    }

    public void OnClickToRemove()
    {
        m_spawnItemGroup.RemoveItem(m_testDataCount - 1);
        if (m_testDataLst.Contains(m_testDataCount - 1))
        {
            m_testDataLst.Remove(m_testDataCount - 1);
            m_testDataCount--;
        }
        m_scrollRect.normalizedPosition = new Vector2(0.0f, 1.0f);
    }

    public void OnScrollRateChange()
    {
        if (float.TryParse((m_scrollRate.text), out m_rate))
        {
            m_spawnItemGroup.ScrollAtNormalizedPos(0, m_rate);
        }
    }
}
