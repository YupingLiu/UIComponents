using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIToggleActiveCom : MonoBehaviour
{
    [SerializeField]
    GameObject selectBg;
    [SerializeField]
    bool initState = false;
    [SerializeField]
    Toggle toggle;
    [SerializeField]
    GameObject checkMark;

    void Start()
    {
        if (selectBg != null)
            selectBg.SetActive(initState);
        if (toggle != null)
            toggle.isOn = initState;
        if (checkMark != null)
            checkMark.SetActive(initState);
    }

    public void OnToggle(bool selected)
    {
        if (selectBg != null)
            selectBg.SetActive(selected);
        if (checkMark != null)
            checkMark.SetActive(selected);
    }
}
