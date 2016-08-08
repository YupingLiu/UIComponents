namespace MoreFun.UI
{
    /// <summary>
    /// UIBtnEventType定义了button控制的类型
    /// 各类型解释如下：
    /// 1. ONE_UP_ONE_DOWN表示按下和弹起的Image类型不一样
    /// 2. CLICK_CHANGE_CLICK_BACK表示点击后切换，再次点击后回复
    /// </summary>
    public enum UIBtnEventType
    {
        ONE_UP_ONE_DOWN,
        CLICK_CHANGE_CLICK_BACK,
    }
}
