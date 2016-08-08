using UnityEngine;
using System.Collections;

namespace MoreFun.UI
{
    public class SpawnItemBase : MonoBehaviour
    {
        private int m_indexOfItemList;

        public int IndexOfItemList
        {
            get { return m_indexOfItemList; }
            set { m_indexOfItemList = value; }
        }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {

        }

        public virtual void SetData(object data)
        { }

        public virtual void SetView(object data)
        {
            // TODO: Call customer SetData for item view
        }

        public virtual void Reset() { }
    }
}

