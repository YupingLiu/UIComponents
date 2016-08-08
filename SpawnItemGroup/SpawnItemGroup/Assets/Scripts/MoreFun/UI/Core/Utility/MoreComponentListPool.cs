using System.Collections.Generic;
using UnityEngine;

namespace MoreFun.UI
{
    internal static class MoreComponentListPool
    {
        // Object pool to avoid allocations.
        private static readonly MoreObjectPool<List<Component>> s_ComponentListPool = new MoreObjectPool<List<Component>>(null, l => l.Clear());
        
        public static List<Component> Get()
        {
            return s_ComponentListPool.Get();
        }
        
        public static void Release(List<Component> toRelease)
        {
            s_ComponentListPool.Release(toRelease);
        }
    }
}
