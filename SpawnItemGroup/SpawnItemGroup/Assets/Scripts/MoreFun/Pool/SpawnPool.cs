using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreFun
{

    /// <summary>
    /// <para>专门用于存放UnityEngine.Object的缓存池。</para>
    /// <para>用户使用SpawnPool的Spawn()和Despawn()来代替Unity的Instantiate()和Destroy()。从而实现对象的实例化、缓存和重用的功能。</para>
    /// <para>对象若为GameObject且挂接了MonoBehaviour脚本组件，则其脚本可增加OnSpawn()方法和OnDespawn()方法，即可分别响应Spawn和Despawn两个时机。</para>
    /// <para>使用Clear()进行SpawnPool清理缓存。</para>
    /// <para>SpawnPool和Pool是不同的两个机制，互不相干。</para>
    /// </summary>

    public class SpawnPool
    {
        #region SpawnPool static class definition
        private static Dictionary<string, SpawnPool> ms_pools = new Dictionary<string, SpawnPool>();

        private static bool logDetail = false;

        /// <summary>
        /// <para>以poolId来创建一个Pool。</para>
        /// </summary>
        /// <param name="poolId">pool的id</param>
        /// <returns>成功创建的的pool</returns>
        public static SpawnPool CreateSpawnPool(string poolId)
        {
            SpawnPool pool = GetSpawnPool(poolId);
            if (null == pool)
            {
                pool = new SpawnPool(poolId, logDetail);
                ms_pools[poolId] = pool;
            }

            return pool;
        }

        /// <summary>
        /// 通过poolId移除指定的pool。
        /// </summary>
        /// <param name="poolId"></param>
        /// <returns></returns>
        public static SpawnPool RemovePool(string poolId)
        {
            SpawnPool pool = null;
            if (ms_pools.ContainsKey(poolId))
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
        /// <para>通过<code>poolId</code>获取Pool。如无，返回null。</para>
        /// </summary>
        /// <param name="poolId">pool的id</param>
        /// <returns>拥有该poolId的pool</returns>
        public static SpawnPool GetSpawnPool(string poolId)
        {
            if (ms_pools.ContainsKey(poolId))
            {
                return ms_pools[poolId];
            }

            return null;
        }
        #endregion

        private static readonly string OnSpawnMessage = "OnSpawn";
        private static readonly string OnDespawnMessage = "OnDespawn";
        private static readonly string OnReclaimMessage = "OnReclaim";

        #region SpawnPool class definition
        private string m_poolId;
        /// <summary>
        /// <para>用户可以使用同一个SpawnPool来Spawn任意的Prefab的对象，
        /// 所以SpawnPool里必须要有一个PrefabPool的列表。</para>
        /// <para>详见PrefabPool说明。</para>
        /// </summary>
        private List<PrefabPool> m_lstPrefabPool;

        private bool m_logDetail = false;

        public SpawnPool(string poolId)
        {
            m_poolId = poolId;
            Clear();
        }

        public SpawnPool(string poolId, bool logDetail)
        {
            m_logDetail = logDetail;
            m_poolId = poolId;
            Clear();
        }

        /// <summary>
        /// 清除SpawnPool、Destory里面所有缓存的对象。
        /// </summary>
        public void Clear()
        {
            #if DEV_BUILD
            Debug.Log(ToString() + ".Clear");
            #endif
            if (null != m_lstPrefabPool)
            {
                for (int i = 0; i < m_lstPrefabPool.Count; ++i)
                {
                    PrefabPool pool = m_lstPrefabPool[i];
                    if (null != pool)
                    {
                        try
                        {
                            pool.Clear();
                        } catch (System.Exception ex) {
                            Debug.LogError("Exception catched during Clear! " +
                                      ex);
                        }
                    }
                }
            }
            m_lstPrefabPool = new List<PrefabPool>();
        }

        
        
        /// <summary>
        /// Call Reclaim() to despawn all spawned objects.
        /// </summary>
        public void Reclaim()
        {
            #if DEV_BUILD
            Debug.Log(ToString() + ".Reclaim");
            #endif
            if (null != m_lstPrefabPool)
            {
                for (int i = 0; i < m_lstPrefabPool.Count; ++i)
                {
                    PrefabPool pPool = m_lstPrefabPool[i];
                    Object[] spawned = pPool.GetSpawnedListCopy();
                    if (null != spawned)
                    {
                        for(int j = 0; j < spawned.Length; ++j)
                        {
                            try
                            {
                                Despawn(spawned[j]);
                            }
                            catch (System.Exception ex)
                            {
                                Debug.LogError("Exception catched during Reclaim! " +
                                          ex);
                            }
                        }
                    }
                }
            }
        }
        
        
        /// <summary>
        /// Call Retain() to make all despawned object DontDestroyOnLoad,
        /// and set their parent to null.
        /// </summary>
        public void Retain()
        {
            #if DEV_BUILD
            Debug.Log(ToString() + ".Retain");
            #endif
            if (null != m_lstPrefabPool)
            {
                for (int i = 0; i < m_lstPrefabPool.Count; ++i)
                {
                    PrefabPool pool = m_lstPrefabPool[i];
                    if (null != pool)
                    {
                        try {
                            pool.Retain();
                        } catch (System.Exception ex) {
                            Debug.LogError("Exception catched during Retain! " +
                                      ex);
                            
                        }
                    }
                }
            }
        }

        public void ClearIf(int maxSize)
        {
            #if DEV_BUILD
            Debug.Log(ToString() + ".ClearIf");
            #endif
            if (null != m_lstPrefabPool)
            {
                for(int i = m_lstPrefabPool.Count - 1; i >= 0; --i)
                {
                    PrefabPool pPool = m_lstPrefabPool[i];
                    if(pPool.ClearIf(maxSize))
                    {
                        m_lstPrefabPool.RemoveAt(i);
                    }
                }
            }
        }

        private UnityEngine.Object SpawnWorker(UnityEngine.Object prefab)
        {
            UnityEngine.Object ret = null;

            if (null != prefab)
            {
                //搜寻是否已有PrefabPool是和现在传入的prefab对象对应
                PrefabPool prefabPool = GetPrefabPool(prefab);

                if (null == prefabPool)
                {
                    prefabPool = new PrefabPool(prefab);
                    m_lstPrefabPool.Add(prefabPool);
                }

                ret = prefabPool.Spawn();
            }

            return ret;
        }

        private UnityEngine.Object SpawnWorker(UnityEngine.Object prefab, Vector3 position, Quaternion rotation)
        {
            UnityEngine.Object ret = null;

            if (null != prefab)
            {
                //搜寻是否已有PrefabPool是和现在传入的prefab对象对应
                PrefabPool prefabPool = GetPrefabPool(prefab);

                if (null == prefabPool)
                {
                    prefabPool = new PrefabPool(prefab);
                    m_lstPrefabPool.Add(prefabPool);
                }

                ret = prefabPool.Spawn(position, rotation);

#if DEV_BUILD
                if(null == ret)
                {
                    this.MoreLogError("spawn failed with null spawned! prefab=" +
                                      prefab);
                }
#endif

#if UNITY_EDITOR
                if(m_logDetail)
                {
                    Debug.Log(ToString() + ".SpawnWorker, " + prefab);
                }
#endif
            }

            return ret;
        }

        private PrefabPool GetPrefabPool(Object prefab)
        {
            PrefabPool onePool = null;
            for (int i = 0; i < m_lstPrefabPool.Count; ++i)
            {
                onePool = m_lstPrefabPool[i];
                if (onePool.GetPrefab().Equals(prefab))
                {
                    return onePool;
                }
            }

            return null;
        }

        /// <summary>
        /// 预加载的时候preWarm对象
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public UnityEngine.Object PreWarm(UnityEngine.Object prefab, int count = 1)
        {
            UnityEngine.Object ret = null;

            if (null != prefab)
            {
                PrefabPool prefabPool = null;
                PrefabPool onePool;
                for (int i = 0; i < m_lstPrefabPool.Count; ++i)
                {
                    onePool = m_lstPrefabPool[i];
                    if (onePool.GetPrefab().Equals(prefab))
                    {
                        prefabPool = onePool;
                        break;
                    }
                }

                if (null == prefabPool)
                {
                    prefabPool = new PrefabPool(prefab);
                    m_lstPrefabPool.Add(prefabPool);
                }

                count = Mathf.Max(1, count);

                for(int i = 0; i < count; ++i)
                {
                    ret = prefabPool.PreWarm(Vector3.zero, Quaternion.identity);
                    
                    GameObject go = ret as GameObject;
                    
                    if (null != go)
                    {
                        go.SetActive(false);
                    }
                }
            }
            else
            {
#if DEV_BUILD
                Debug.LogWarning(ToString() + "无法PreWarm。因为传入prefab为null。");
#endif
            }

            return ret;
        }

        /// <summary>
        /// Check whether this pool has cached the specified prefab with the cachedCount.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <param name="cachedCount">Cached count.</param>
        public bool Check(Object prefab, int cachedCount)
        {
            PrefabPool pool = GetPrefabPool(prefab);
            if(null == pool)
            {
                return false;
            }

            if(pool.GetDespawnedListCount() >= cachedCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <para>用法相似于UnityEngine.Object.Instantiate()。</para>
        /// </summary>
        /// <param name="prefab">准备Spawn的模版对象</param>
        /// <returns></returns>
        public UnityEngine.Object Spawn(UnityEngine.Object prefab)
        {
            UnityEngine.Object ret = null;

            if(null != prefab)
            {
                ret = SpawnWorker(prefab);

                GameObject go = ret as GameObject;
                if (null != go)
                {
                    go.SetActive(true);
                    go.BroadcastMessage(OnSpawnMessage, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
#if DEV_BUILD
                Debug.LogWarning(ToString() + "无法Spawn。因为传入prefab为null。");
#endif
            }

            return ret;
        }
        /// <summary>
        /// <para>用法相似于UnityEngine.Object.Instantiate()。</para>
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public UnityEngine.Object Spawn(UnityEngine.Object prefab, Vector3 position, Quaternion rotation)
        {
            UnityEngine.Object ret = null;

            if (null != prefab)
            {
                //搜寻是否已有PrefabPool是和现在传入的prefab对象对应
                ret = SpawnWorker(prefab, position, rotation);

                GameObject go = ret as GameObject;
                if (null != go)
                {
                    go.SetActive(true);
                    go.BroadcastMessage(OnSpawnMessage, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
#if DEV_BUILD
                Debug.LogWarning(ToString() + "无法Spawn。因为传入prefab为null。");
#endif
            }

            return ret;
        }


        /// <summary>
        /// <para>用法相似于UnityEngine.Object.Instantiate()。创建的时候可以同时设定新Object的Parent。</para>
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public UnityEngine.Object Spawn(UnityEngine.Object prefab, Transform parent, bool worldPositionStays = false)
        {
            UnityEngine.Object ret = null;

            if (null != prefab)
            {
                //搜寻是否已有PrefabPool是和现在传入的prefab对象对应
                ret = SpawnWorker(prefab);

                GameObject go = ret as GameObject;
                if (null != go)
                {
                    go.SetActive(true);
                    if (null != go.transform)
                    {
                        go.transform.SetParent(parent, worldPositionStays);
                    }
                    go.BroadcastMessage(OnSpawnMessage, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
#if DEV_BUILD
                Debug.LogWarning(ToString() + "无法Spawn。因为传入prefab为null。");
#endif
            }

            return ret;
        }


        /// <summary>
        /// <para>用法相似于UnityEngine.Object.Instantiate()。创建的时候可以同时设定位置、旋转、新Object的Parent。</para>
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        /// <returns></returns>
        public UnityEngine.Object Spawn(UnityEngine.Object prefab, Vector3 position, Quaternion rotation,
            Transform parent, bool worldPositionStays = false)
        {
            UnityEngine.Object ret = null;

            if (null != prefab)
            {
                //搜寻是否已有PrefabPool是和现在传入的prefab对象对应
                ret = SpawnWorker(prefab, position, rotation);

                GameObject go = ret as GameObject;
                if (null != go)
                {
                    go.SetActive(true);
                    if (null != go.transform)
                    {
                        go.transform.SetParent(parent, worldPositionStays);
                    }
                    try
                    {
                        go.BroadcastMessage(OnSpawnMessage, SendMessageOptions.DontRequireReceiver);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("Exception catched during OnSpawn! " + ex);
                    }
                }
            }
            else
            {
#if DEV_BUILD
                Debug.LogWarning(ToString() + "无法Spawn。因为传入prefab为null。");
#endif
            }

            return ret;
        }

        /// <summary>
        /// <para>用法相似于UnityEngine.Object.Destroy()。注意只有从一个SpawnPool Spawn出来的对象，必须使用同一个SpawnPool进行Despawn</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Despawn(UnityEngine.Object obj)
        {
            #if UNITY_EDITOR
            if(m_logDetail)
            {
                Debug.Log(ToString() + ".Despawn, " + obj);
            }
            #endif

            if (null != obj)
            {
                //当传入的是Component的时候，用户其实是想Despawn这个Component对应的GameObject
                Component comp = obj as Component;
                if(null != comp)
                {
                    obj = comp.gameObject;
                }

                PrefabPool foundPool = null;
                PrefabPool onePool;
                for (int i = 0; i < m_lstPrefabPool.Count; ++i)
                {
                    onePool = m_lstPrefabPool[i];
                    if(onePool.IsObjectSpawnedFromPool(obj))
                    {
                        foundPool = onePool;
                        break;
                    }
                }

                if(null == foundPool)
                {
#if UNITY_EDITOR

                    string log = ToString() + "will NOT Despawn " + obj.ToString();

                    bool isInPool = false;
                    for (int i = 0; i < m_lstPrefabPool.Count; ++i)
                    {
                        onePool = m_lstPrefabPool[i];
                        if(onePool.IsObjectInPool(obj))
                        {
                            isInPool = true;
                            log += " because it's already in pool: " + onePool.ToString();
                        }
                    }

                    if(isInPool)
                    {
                        if(m_logDetail)
                        {
                            Debug.Log(log);
                        }
                    }
                    else
                    {
                        log += " because it's NOT spawned from this pool. AllPool=\n";
                        for (int i = 0; i < m_lstPrefabPool.Count; ++i)
                        {
                            onePool = m_lstPrefabPool[i];
                            log += onePool.ToString() + "\n";
                        }

                        Debug.LogWarning(log);
                    }
#endif
                    return false;
                }

                foundPool.Despawn(obj);

                UnityEngine.GameObject go = obj as UnityEngine.GameObject;
                if (null != go)
                {
                    try
                    {
                        go.BroadcastMessage(OnDespawnMessage, SendMessageOptions.DontRequireReceiver);
                        go.SetActive(false);
                    } catch (System.Exception ex)
                    {
                        Debug.LogError("Exception catched during OnDespawn! " + ex);
                    }

                }

                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// <para>用法相似于UnityEngine.Object.Destroy()。注意只有从一个SpawnPool Spawn出来的对象，必须使用同一个SpawnPool进行Despawn</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Despawn(UnityEngine.Object obj, float delay)
        {
            if(delay > 0.0f)
            {
                GlobalObject.Instance.StartCoroutine(DespawnCoroutine(obj, delay));
                return true;
            }
            else
            {
                return Despawn(obj);
            }
        }

        private IEnumerator DespawnCoroutine(UnityEngine.Object obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Despawn(obj);
        }

        public override string ToString()
        {
            return string.Format("[SpawnPool({0})]", m_poolId);
        }

        #endregion

    }

    /// <summary>
    /// <para>用户可以使用同一个SpawnPool来Spawn任意的Prefab的对象，
    /// 而且Unity的不同Prefab创造出来的多个GameObject，除了语言层面的对象实例比较（==）之外，
    /// 是无法通过Unity逻辑层面严谨进行比较的（通过GameObject.name"名字(Clone)"和Prefab.name来比较是不严谨的），
    /// 所以SpawnPool里必须要有一个PrefabPool的列表。</para>
    /// <para>PrefabPool以prefab为key，对应了两个列表：spawnList和despawnList。</para>
    /// <para>spawnList记录了从本PrefabPool Spawn出去、目前还没归还（Despawn）的对象列表，
    /// 用于SpawnPool.Despawn(obj)时判别，该obj是否从本PrefabPool产生、是否该由本PrefabPool接管；</para>
    /// <para>despawnList记录了本PrefabPool缓存住的对象列表，用于SpawnPool.Spawn(prefab)时，重用对象。</para>
    /// </summary>
    internal class PrefabPool
    {
        private UnityEngine.Object m_prefab;

        private List<UnityEngine.Object> spawnedList;
        private List<UnityEngine.Object> despawnedList;

        private bool m_logDetail = false;

        public PrefabPool(UnityEngine.Object prefab)
        {
            Clear();
            m_prefab = prefab;
        }
        
        public PrefabPool(UnityEngine.Object prefab, bool logDetail)
        {
            m_logDetail = logDetail;
            Clear();
            m_prefab = prefab;
        }

        public UnityEngine.Object GetPrefab()
        {
            return m_prefab;
        }


        /// <summary>
        /// Call Retain() to make all despawned object DontDestroyOnLoad,
        /// and set their parent to null.
        /// </summary>
        public void Retain()
        {
#if DEV_BUILD
            if(m_logDetail)
            {
                Debug.Log(ToString() + ".Retain, despawnedList=" +
                          (null != despawnedList ? despawnedList.Count.ToString() : "null"));
            }
#endif
            for(int i = 0; i < despawnedList.Count; ++i)
            {
                GameObject go = despawnedList[i] as GameObject;
                if(null != go)
                {
                    go.transform.SetParent(null);
                    GameObject.DontDestroyOnLoad(go);
                }
            }
        }

        public void Clear()
        {
            #if DEV_BUILD
            if(m_logDetail)
            {
                Debug.Log(ToString() + ".Clear, despawnedList=" +
                          (null != despawnedList ? despawnedList.Count.ToString() : "null"));
            }
            #endif
            spawnedList = new List<UnityEngine.Object>();

            if (null != despawnedList)
            {
                UnityEngine.Object obj;
                for (int i = 0; i < despawnedList.Count; ++i)
                {
                    obj = despawnedList[i];
                    UnityEngine.Object.Destroy(obj);
                }
            }
            despawnedList = new List<UnityEngine.Object>();

            m_prefab = null;
        }

        
        public bool ClearIf(int maxSize)
        {
            #if DEV_BUILD
            if(m_logDetail)
            {
                Debug.Log(ToString() + ".ClearIf, despawnedList=" +
                          (null != despawnedList ? despawnedList.Count.ToString() : "null"));
            }
            #endif
            if(null != despawnedList && despawnedList.Count >= maxSize)
            {
                Clear();
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 预加载的时候需要提前在池里面存好指定个数的非激活对象，避免初始化产生卡顿
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public UnityEngine.Object PreWarm(Vector3 position, Quaternion rotation)
        {
            UnityEngine.Object ret;

            ret = UnityEngine.Object.Instantiate(m_prefab, position, rotation);

            despawnedList.Add(ret);

            return ret;
        }

        public UnityEngine.Object Spawn()
        {
            UnityEngine.Object ret;

            if (despawnedList.Count > 0)
            {
                ret = despawnedList[despawnedList.Count - 1];
                despawnedList.RemoveAt(despawnedList.Count - 1);
                GameObject go = ret as GameObject;
                #if UNITY_EDITOR
                if(m_logDetail)
                {
                    Debug.Log(ToString() + ".Spawn() from despawned: " + ret);
                }
                #endif
            }
            else
            {
                ret = UnityEngine.Object.Instantiate(m_prefab);
                #if UNITY_EDITOR
                if(m_logDetail)
                {
                    Debug.Log(ToString() + ".Spawn() new object: " + ret);
                }
                #endif
            }

            spawnedList.Add(ret);

            return ret;
        }

        public UnityEngine.Object Spawn(Vector3 position, Quaternion rotation)
        {
            UnityEngine.Object ret;

            if (despawnedList.Count > 0)
            {
                ret = despawnedList[despawnedList.Count - 1];
                despawnedList.RemoveAt(despawnedList.Count - 1);
                GameObject go = ret as GameObject;
                if(null != go)
                {
                    if(null != go.transform)
                    {
                        go.transform.position = position;
                        go.transform.rotation = rotation;
                    }
                }
                #if UNITY_EDITOR
                if(m_logDetail)
                {
                    Debug.Log(ToString() + ".Spawn() from despawned: " + ret);
                }
                #endif
            }
            else
            {
                ret = UnityEngine.Object.Instantiate(m_prefab, position, rotation);
                #if UNITY_EDITOR
                if(m_logDetail)
                {
                    Debug.Log(ToString() + ".Spawn() new object: " + ret);
                }
                #endif
            }
            
            #if DEV_BUILD
            if(null == ret)
            {
                this.MoreLogError(ToString() + " spawn failed with null spawned!");
            }
            #endif

            spawnedList.Add(ret);


            return ret;
        }

        public void Despawn(UnityEngine.Object obj)
        {
            #if UNITY_EDITOR
            if(m_logDetail)
            {
                Debug.Log(ToString() + ".Despawn(): " + obj);
            }
            #endif

            spawnedList.Remove(obj);
            despawnedList.Add(obj);
        }

        public Object[] GetSpawnedListCopy()
        {
            return spawnedList.ToArray();
        }

        public int GetDespawnedListCount()
        {
            if(null == despawnedList)
            {
                return 0;
            }
            else
            {
                return despawnedList.Count;
            }
        }

        public bool IsObjectSpawnedFromPool(UnityEngine.Object obj)
        {
            int foundIndex = spawnedList.IndexOf(obj);
            if(0 > foundIndex)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public bool IsObjectInPool(UnityEngine.Object obj)
        {
            int foundIndex = despawnedList.IndexOf(obj);
            if(0 > foundIndex)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override string ToString()
        {
            return string.Format("[PrefabPool({0}, {1})]", m_prefab,
                                 (null != m_prefab ? m_prefab.GetInstanceID().ToString() : "null"));
        }
    }
}
