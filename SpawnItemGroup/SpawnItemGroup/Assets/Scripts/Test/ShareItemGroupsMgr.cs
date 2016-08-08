using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreFun.UI;

public class ShareItemGroupsMgr : MonoBehaviour {

    [SerializeField]
    private List<LayoutTest> m_layOutDataSetter;
	// Use this for initialization
	public void OnSetDataList () {

        for (int i = 0; i < m_layOutDataSetter.Count; i++)
        {
            m_layOutDataSetter[i].SetData();
        }
	}
	
}
