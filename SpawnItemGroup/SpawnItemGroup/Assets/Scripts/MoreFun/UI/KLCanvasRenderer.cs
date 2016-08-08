using UnityEngine;
using System.Collections;

/// <summary>
/// doTween的Lamda表达式需要.x属性，而CanvasRenderer的alpha只能通过GetAlpha()和SetAlpha()方法进行，所以简单封装了一下
/// </summary>

namespace MoreFun.UI
{
    public class KLCanvasRenderer
    {
        private CanvasRenderer m_render;

        public KLCanvasRenderer(CanvasRenderer render)
        {
            m_render = render;
        }

        public float Alpha
        {
            get
            {
                return m_render.GetAlpha();
            }
            set
            {
                m_render.SetAlpha(value);
            }
        }
    }
}

