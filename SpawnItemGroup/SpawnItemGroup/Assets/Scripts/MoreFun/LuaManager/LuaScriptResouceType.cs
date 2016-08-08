using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreFun
{
    /// <summary>
    /// Lua脚本的资源类型定义
    /// </summary>
    public enum LuaScriptResouceType
    {
        String = 1,
        Resouces,
        StreamingAssets,
        PersistantDataPath,
        AssetBundle
    }
}
