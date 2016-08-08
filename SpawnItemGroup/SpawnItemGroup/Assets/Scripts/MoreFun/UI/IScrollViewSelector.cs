using UnityEngine;
using System.Collections;
using MoreFun;

namespace MoreFun.UI
{
    /// <summary>
    /// IScrollViewSelector用于ScrollViewItem接收鼠标单击事件
    /// 每个想要实现ScrollViewItem点击的Item需要挂接该interface的一个实现
    /// 在初始化时需要向上层的UIScrollViewSelector注册自身
    /// </summary>
    public enum SelectAction
    {
        TOUCH_DOWN,
        DRAG_UP,
        TOUCH_UP
    }
    public abstract class IScrollViewSelector : MoreBehaviour
    {
        public abstract void Selected(Vector2 offset, SelectAction action);
    }
}