using UnityEngine;
using System.Collections;

public class UIArraySelectorCom : MonoBehaviour 
{
    [SerializeField]
    GameObject[]  m_arrayObjs;

    void Awake()
    {
        Hide();
    }

    public void Hide()
    {
        if (null != m_arrayObjs)
        {
            for (int i = 0; i < m_arrayObjs.Length; ++i)
            {
                m_arrayObjs[i].SetActive(false);
            }
        }
    }

    public void UpdateView(int index = -1)
    {
        Hide();
        if (null != m_arrayObjs && index >= 0 && index < m_arrayObjs.Length)
        {
            m_arrayObjs[index].SetActive(true);
        }
    }
}
