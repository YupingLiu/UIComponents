//using LuaInterface;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MoreFun
{
    /// <summary>
    /// 对uLua的一个封装管理器。
    ///     - 负责管理一个LuaState。
    ///     - 负责加载基础的Lua脚本
    ///     - 可以持续添加新的Lua脚本
    /// </summary>
    public class LuaManager
    {
        public static readonly LuaManager me = new LuaManager();

        //private LuaState m_mainLuaState;

        //private string m_baseLuaCfgPath;

        //public virtual void Initialize(LuaScriptResouceType scriptResourceType, string baseLuaCfgPath)
        //{
        //    m_mainLuaState = new LuaState();

        //    InitializeBaseLua(scriptResourceType, baseLuaCfgPath);
        //}

        //public void DoScript(LuaScript script)
        //{
        //    if(null != script)
        //    {
        //        m_mainLuaState.DoString(script.GetCode(), script.GetName(), null);
        //    }
        //}

        //public LuaFunction GetFunction(string fullPath)
        //{
        //    return m_mainLuaState.GetFunction(fullPath);
        //}

        //public LuaTable GetTable(string fullPath)
        //{
        //    LuaTable table = m_mainLuaState.GetTable(fullPath);
        //    return table;
        //}

        ///// <summary>
        ///// 初始化所有基础Lua脚本
        ///// </summary>
        //private void InitializeBaseLua(LuaScriptResouceType scriptResourceType, string baseLuaCfgPath)
        //{
        //    m_baseLuaCfgPath = baseLuaCfgPath;

        //    List<string> arrAllBaseLuaPath = null;

        //    switch(scriptResourceType)
        //    {
        //        case LuaScriptResouceType.StreamingAssets:
        //            arrAllBaseLuaPath = ReadBaseLuaPath_StreammingAssets();
        //            if (null != arrAllBaseLuaPath && 0 < arrAllBaseLuaPath.Count)
        //            {
        //                foreach (string oneBaseLuaPath in arrAllBaseLuaPath)
        //                {
        //                    LuaScript oneLua = new LuaScript();
        //                    oneLua.ReadFromStreamingAsset(oneBaseLuaPath);

        //                    DoScript(oneLua);
        //                }
        //            }
        //            break;
        //        case LuaScriptResouceType.Resouces:
        //            arrAllBaseLuaPath = ReadBaseLuaPath_Resources();
        //            if (null != arrAllBaseLuaPath && 0 < arrAllBaseLuaPath.Count)
        //            {
        //                foreach (string oneBaseLuaPath in arrAllBaseLuaPath)
        //                {
        //                    LuaScript oneLua = new LuaScript();
        //                    oneLua.ReadFromResources(oneBaseLuaPath);

        //                    DoScript(oneLua);
        //                }
        //            }
        //            break;

        //        default:
        //            break;
        //    }
            
        //}

        //private List<string> ReadBaseLuaPath_StreammingAssets()
        //{
        //    using (FileStream fs = File.OpenRead(m_baseLuaCfgPath))
        //    {
        //        using (StreamReader reader = new StreamReader(fs))
        //        {
        //            List<string> lstPath = new List<string>();
        //            while (reader.Peek() >= 0)
        //            {
        //                string oneLine = reader.ReadLine();
        //                lstPath.Add(oneLine);
        //            }

        //            lstPath = ReadBaseLuaPath(lstPath);

        //            return lstPath;
        //        }
        //    }
        //}
        //private List<string> ReadBaseLuaPath_Resources()
        //{
        //    string cfgPath = m_baseLuaCfgPath;
        //    TextAsset cfg = Resources.Load<TextAsset>(cfgPath);


        //    string[] arrPath = cfg.text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

        //    List<string> lstPath = ReadBaseLuaPath(new List<string>(arrPath));
        //    return lstPath;
        //}

        //private List<string> ReadBaseLuaPath(List<string> source)
        //{
        //    List<string> lstPath = new List<string>();

        //    foreach (string oneLine in source)
        //    {
        //        if (null != oneLine && 0 < oneLine.Length)
        //        {
        //            if (0 != oneLine.IndexOf("--"))
        //            {
        //                lstPath.Add(oneLine);
        //            }
        //        }
        //    }

        //    return lstPath;
        //}

    }
}
