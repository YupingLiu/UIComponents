using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MoreFun
{
    /// <summary>
    /// 对Lua脚本文本资源的一个封装
    /// </summary>
    public class LuaScript
    {
        protected LuaScriptResouceType m_resourceType;

        protected string m_fullPath;
        protected AssetBundle m_luaAssetBundle;
        protected string m_luaCode;
        protected string m_luaName;

        protected const string FILE_POSTFIX = ".txt";

        public void ReadFromCode(string luaCode, string luaName)
        {
            m_luaCode = luaCode;
            m_luaName = luaName;
            m_resourceType = LuaScriptResouceType.String;
        }

        public void ReadFromTextAsset(TextAsset textAsset, string path)
        {
            m_luaCode = textAsset.text;
            m_luaName = GetNameByPath(path);
            m_resourceType = LuaScriptResouceType.String;
        }

        public void ReadFromAssetBundle(AssetBundle luaAssetBundle, string path)
        {
            if (null != m_luaAssetBundle)
            {
                TextAsset luaTextAsset = m_luaAssetBundle.LoadAsset(m_fullPath, typeof(TextAsset)) as TextAsset;
                m_luaCode = luaTextAsset.text;
                m_luaName = GetNameByPath(path);
            }
        }

        public void ReadFromResources(string path)
        {
            int lastDotIndex = path.LastIndexOf(FILE_POSTFIX);
            if(0 <= lastDotIndex)
            {
                path = path.Substring(0, lastDotIndex);
            }
            m_luaCode = Resources.Load<TextAsset>(path).text;
            m_luaName = GetNameByPath(path);
        }

        public void ReadFromStreamingAsset(string path)
        {
            try
            {
                using(FileStream fs = File.OpenRead(Application.streamingAssetsPath + "/" + path))
                {
                    using(StreamReader reader = new StreamReader(fs))
                    {
                        m_luaCode = reader.ReadToEnd();
                        m_luaName = GetNameByPath(path);
                    }
                }
            }
            catch (Exception e)
            {
#if DEV_BUILD
                Debug.LogError(e.Message);
#endif
            }
        }

        public void ReadFromPersistantDataPath(string path)
        {
            using (FileStream fs = File.OpenRead(Application.persistentDataPath + "/" + path))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    m_luaCode = reader.ReadToEnd();
                    m_luaName = GetNameByPath(path);
                }
            }
        }

        public string GetName()
        {
            return m_luaName;
        }

        public string GetCode()
        {
            return m_luaCode;
        }

        /// <summary>
        /// 认为文件名形如“??/???/MyLuaName.lua???“，本函数将返回MyLuaName
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected string GetNameByPath(string path)
        {
            int startIndex = path.LastIndexOf("/") + 1;
            int endIndex = path.IndexOf(".") - 1;

            return path.Substring(startIndex, endIndex - startIndex + 1);
        }
    }
}
