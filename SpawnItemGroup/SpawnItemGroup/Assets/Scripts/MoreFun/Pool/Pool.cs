using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreFun
{
    /// <summary>
    /// <para>简单的、通用的用于暂存任意对象的缓存池。</para>
    /// <para>用户从Pool中（通过Pop()）尝试获取对象，获取到的对象将不再缓存于Pool中。</para>
    /// <para>如果获取不到，用户需要负责创建具体对象。</para>
    /// <para>当对象暂不需要使用的时候，可以（通过Push()）将其暂存于Pool中。</para>
    /// <para>创建Pool时，建议指定Pool支持的对象类型。这种Pool会对存入对象进行类型检查。</para>
    /// <para>不指定特定类型的Pool支持存入任意类型对象。但任意类型的Pool往往是没意义的</para>
    /// </summary>
    public class Pool
    {
        #region Pool static class definition
        private static Dictionary<string, Pool> ms_pools = new Dictionary<string, Pool>();

        /// <summary>
        /// <para>以poolId和poolObjectType来创建一个Pool。</para>
        /// <para>poolObjectType指的是pool所允许存储的数据类型。</para>
        /// </summary>
        /// <param name="poolId">pool的id</param>
        /// <param name="poolObjectType">pool所允许存储的数据类型。
        /// 当传入null时，pool将允许存储任意数据类型</param>
        /// <returns>成功创建的的pool</returns>
        public static Pool CreatePool(string poolId, Type poolObjectType = null)
        {
            Pool pool = GetPool(poolId);
            if(null == pool)
            {
                pool = new Pool(poolObjectType);
                ms_pools[poolId] = pool;
            }

            return pool;
        }

        /// <summary>
        /// 通过poolId移除指定的pool。
        /// </summary>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public static Pool RemovePool(string poolId)
        {
            Pool pool = null;
            if(ms_pools.ContainsKey(poolId))
            {
                pool = ms_pools[poolId];
                ms_pools.Remove(poolId);
            }
            else
            {
#if DEV_BUILD
                Debug.LogWarning("移除操作未进行。尝试移除不存在的Pool：" + poolId);
#endif
            }

            return pool;
        }

        /// <summary>
        /// <para>通过<code>poolId</code>获取Pool.</para>
        /// </summary>
        /// <param name="poolId">pool的id</param>
        /// <returns>拥有该poolId的pool</returns>
        public static Pool GetPool(string poolId)
        {
            if(ms_pools.ContainsKey(poolId))
            {
                return ms_pools[poolId];
            }

            return null;
        }
        #endregion


        #region Pool class definition
        private Type m_poolObjectType;
        private List<System.Object> m_objects;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="poolObjectType">指定Pool所支持的对象类型</param>
        public Pool(Type poolObjectType = null)
        {
            m_poolObjectType = poolObjectType;
            Clear();
        }
        public void Clear()
        {
            m_objects = new List<System.Object>();
        }
        public void Push(System.Object obj)
        {
            if (null != obj)
            {
                if (null == m_poolObjectType)
                {
                    m_objects.Add(obj);
                }
                else
                {
                    Type objType = obj.GetType();
                    if (objType == m_poolObjectType || objType.IsSubclassOf(m_poolObjectType))
                    {
                        m_objects.Add(obj);
                    }
                    else
                    {
#if DEV_BUILD
#if DEV_BUILD
                        Debug.LogWarning("Can NOT push object to a typed pool: " +
#endif
                            "object " + obj.ToString() + " is NOT type " + m_poolObjectType.ToString());
#endif
                    }
                }
            }
        }
        public System.Object Pop()
        {
            if(m_objects.Count > 0)
            {
                System.Object ret = m_objects[m_objects.Count - 1];
                m_objects.RemoveAt(m_objects.Count - 1);

                return ret;
            }
            else
            {
                return null;
            }
        }

        public bool Remove(System.Object obj)
        {
            int foundIndex = m_objects.IndexOf(obj);
            if(0 <= foundIndex)
            {
                m_objects.RemoveAt(foundIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetCount()
        {
            return m_objects.Count;
        }
        #endregion

    }

}
