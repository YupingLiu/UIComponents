using UnityEngine;
using System.Collections.Generic;


public class MoreMaterialsManager
{
    private Material m_DefaultMaterial;
    private Dictionary<string, Material> m_Original2Mores;
    private Dictionary<string, Material> m_More2Originals;


    private static MoreMaterialsManager m_Instance;
    public static MoreMaterialsManager Instance
    {
        get
        {
            if(null == m_Instance)
            {
                m_Instance = new MoreMaterialsManager();
            }
            m_Instance.TryInit();
            return m_Instance;
        }
    }


    private MoreMaterialsManager()
    {
    }

    private void TryInit()
    {
        if (null == m_DefaultMaterial)
        {
            m_Original2Mores = new Dictionary<string, Material>();
            m_More2Originals = new Dictionary<string, Material>();
            m_DefaultMaterial = new Material(Shader.Find("MoreFun/UI-DefaultAlpha"));
            AddMaterialPair(null, m_DefaultMaterial);
            AddMaterialPair("UI/Custom_Default", "MoreFun/UI-DefaultAlpha");
            AddMaterialPair("UI/Default", "MoreFun/UI-DefaultAlpha");
            AddMaterialPair("MoreFun/UI/Add", "MoreFun/UI-MoreAdd");
            AddMaterialPair("MoreFun/UI/Saturate", "MoreFun/UI-MoreSaturate");
        }
    }
    private string GetMaterialKey(Material original)
    {
        string key = original == null ? "null" : (original.shader == null ? "null" : original.shader.name);
        return key;
    }
    public Material GetMaterial(Material original,bool isMoreSprite)
    {
        Material returnMaterial = original;
        string key = GetMaterialKey(original);
        if (isMoreSprite)
        {
            if (m_Original2Mores.ContainsKey(key))
            {
                returnMaterial = m_Original2Mores[key];
            }
            else
            {
                returnMaterial = (original == null ? m_DefaultMaterial : original);
            }
        }
        else
        {
            if (m_More2Originals.ContainsKey(key))
            {
                returnMaterial = m_More2Originals[key];

            }
            else
            {
                returnMaterial = original;
            }
        }
        string returnKey = GetMaterialKey(returnMaterial);
        if (returnKey.CompareTo(key) == 0)//如果替换后的材质与之前的材质是一样的，那么久不替换了
        {
            return original;
        }
        else
        {
            return returnMaterial;
        }
    }
    public Material GetDefaultMaterial(bool isMoreSprite)
    {
        if(isMoreSprite)
        {
            return m_DefaultMaterial;
        }
        else
        {
            return GetMaterial(m_DefaultMaterial, false);
        }
    }
    public void AddMaterialPair(Material original,Material moreMaterial)
    {
        string originalKey = GetMaterialKey(original);
        string moreMaterialKey = GetMaterialKey(moreMaterial);
        if(!m_Original2Mores.ContainsKey(originalKey))
        {
            m_Original2Mores.Add(originalKey, moreMaterial);
        }
        if (!m_More2Originals.ContainsKey(moreMaterialKey))
        {
            m_More2Originals.Add(moreMaterialKey, original);
        }
    }
    public void AddMaterialPair(string shaderOriginal, string moreMaterialShader)
    {
        if (!m_Original2Mores.ContainsKey(shaderOriginal))
        {
            m_Original2Mores.Add(shaderOriginal, new Material(Shader.Find(moreMaterialShader)));
        }
        if (!m_More2Originals.ContainsKey(moreMaterialShader))
        {
            m_More2Originals.Add(moreMaterialShader, new Material(Shader.Find(shaderOriginal)));
        }
    }
}