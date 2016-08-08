
using System;
using UnityEngine;

namespace MoreFun
{
    public delegate void ModuleHostResetHandler();

    public class ModuleHost
    {
        
        public const string ModuleHostTag = "ModuleHost";

        public static event ModuleHostResetHandler OnModuleHostReset;

        public static void Reset()
        {
            if(null != OnModuleHostReset)
            {
                OnModuleHostReset();
            }
        }

        /// <summary>
        /// Gets the module. 
        /// You should cache the module after you call <code>GetModule()</code>.
        /// Because <code>GetModule()</code> is design for validity, NOT for performance. 
        /// 
        /// </summary>
        /// <returns>The module.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetModule<T>() where T : Component
        {
            return GameObjectUtil.GetTagComponentInChildren<T>(ModuleHostTag, false, false);
        }
    }
}

