using UnityEngine;

namespace MoreFun
{
    /// <summary>
    /// MoreBehaviour = Morefun + MonoBehaviour。
    /// MoreBehaviour继承自Unity的MonoBehaviour，并添加Morefun内部的一些功能接口。
    /// </summary>
    public class MoreBehaviour : MonoBehaviour
    {
        private Transform m_cachedTransform;
        private RectTransform m_cachedRectTransform;

        private bool m_destroyed = false;

        /// <summary>
        /// it's very risky to mistype message function name, especially "OnDestroy", 
        /// you can override this OnDestroy rather to type it.
        /// </summary>
        protected virtual void OnDestroy()
        {
            m_destroyed = true;
        }

        public bool destroyed
        {
            get{ return m_destroyed; }
        }

        /// <summary>
        /// MonoBehaviour的transform接口和GetComponent<Transform>()是一样不高效的。
        /// 为了防止在高频函数里调用它们产生性能消耗，用户可以使用这个cachedTransform。
        /// </summary>
        public Transform cachedTransform
        {
            get
            {
                if(m_cachedTransform == null)
                {
                    m_cachedTransform = gameObject.transform;
                }

                return m_cachedTransform;
            }
        }
        /// <summary>
        /// MonoBehaviour的rectTransform接口和GetComponent<RectTransform>()是一样不高效的。
        /// 为了防止在高频函数里调用它们产生性能消耗，用户可以使用这个cachedRectTransform。
        /// </summary>
        public RectTransform cachedRectTransform
        {
            get
            {
                if (m_cachedRectTransform == null)
                {
                    m_cachedRectTransform = gameObject.transform as RectTransform;
                }

                return m_cachedRectTransform;
            }
        }
        
        /// <summary>
        /// Gets the module. 
        /// You should the module after you call <code>GetModule()</code>.
        /// Because <code>GetModule()</code> is design for validity, NOT for performance. 
        /// 
        /// </summary>
        /// <returns>The module.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetModule<T>() where T : Component
		{
			return ModuleHost.GetModule<T>();
		}
    }
}
