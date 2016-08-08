using UnityEngine;
using System.Collections;
using MoreFun.UI;
using UnityEngine.UI;

public class SpawnItemBaseTest : SpawnItemBase {

    [SerializeField]
    private Text m_text;
    public override void SetView(object data)
    {
        m_text.text = data.ToString();
    }
}
