
using UnityEngine;
namespace MoreFun
{
    /// <summary>
    /// 将本GameObject在Awake时，重新设置自己的Parent。
    /// </summary>
    public class SetParent : MoreBehaviour
    {
        /// <summary>
        /// 目标Parent的Transform
        /// </summary>
        public Transform m_parentTaget;
        /// <summary>
        /// 如果目标Parent是不能直接拖的话，可以通过本字符串，让SetParent在Awake进行Parent搜索
        /// </summary>
        public string parentTargetToSearch = "";

        /// <summary>
        /// 是否保持本GameObject在世界坐标系中的位置
        /// </summary>
        public bool worldPositionStays = false;

        /// <summary>
        /// 如果worldPositionStays是false，需否重设本GameObject的局部坐标系中的新位置、新旋转
        /// </summary>
        public bool hasNewLocalTransform = true;
        /// <summary>
        /// 如果worldPositionStays是false，且hasNewLocalTransform为true，
        /// 局部坐标系中的新位置
        /// </summary>
        public Vector3 newLocalPosition;
        /// <summary>
        /// 如果worldPositionStays是false，且hasNewLocalTransform为true，
        /// 局部坐标系中的新旋转
        /// </summary>
        public Vector3 newLocalRotation;



        void Awake()
        {
            if (null == m_parentTaget)
            {
                if(null != parentTargetToSearch && 0 < parentTargetToSearch.Length)
                {
                    GameObject parentObj = GameObject.Find(parentTargetToSearch);
                    if(null != parentObj)
                    {
                        m_parentTaget = parentObj.transform;
                    }
                }
            }

            if(null != m_parentTaget)
            {
                cachedTransform.SetParent(m_parentTaget, worldPositionStays);
                if (false == worldPositionStays)
                {
                    if (true == hasNewLocalTransform)
                    {
                        cachedTransform.localPosition = newLocalPosition;
                        cachedTransform.localRotation = Quaternion.Euler(newLocalRotation);
                    }
                }
            }
        }
        
        
    }
}
