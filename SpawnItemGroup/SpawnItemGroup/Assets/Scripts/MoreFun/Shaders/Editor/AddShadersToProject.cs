using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class AddShadersToProject : AssetPostprocessor {


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if(null != importedAssets)
        {
            for(int i = 0 ; i < importedAssets.Length ; i++)
            {
                if(importedAssets[i].EndsWith(".shader"))
                {
                    AddShader(importedAssets[i],true);
                }
            }
        }
        if (null != deletedAssets)
        {
            for (int i = 0; i < deletedAssets.Length; i++)
            {
                if (deletedAssets[i].EndsWith(".shader"))
                {
                    AddShader(deletedAssets[i], false);
                }
            }
        }
    }

    private static void AddShader(string shader,bool isAdd)
    {
        string newGuid = AssetDatabase.AssetPathToGUID(shader);
        string graphicsSettingsPath = Application.dataPath.Replace("Assets", "") + "ProjectSettings/GraphicsSettings.asset";
        if(!File.Exists(graphicsSettingsPath))
        {
            File.Create(graphicsSettingsPath);
        }
        string[] msgs = File.ReadAllLines(graphicsSettingsPath);
        Dictionary<string, int> cacheGuids = new Dictionary<string, int>();
        List<string> endMsgs = new List<string>();
        bool isNeedAdd = true;
        bool isNeedUpdate = false;
        for(int i = 0  ; i < msgs.Length ; i++)
        {
            if (!cacheGuids.ContainsKey(msgs[i]))//只有不存在时才做添加
            {
                if (isAdd)
                {
                    if(msgs[i].Contains(newGuid))
                    {
                        isNeedAdd = false;
                    }
                    cacheGuids.Add(msgs[i],0);
                    endMsgs.Add(msgs[i]);
                }
                else
                {
                    if (msgs[i].Contains(newGuid))
                    {
                        Debug.Log("删除Shader ： " + shader + " 并且在Project graphics Settings中删除！");
                        isNeedUpdate = true;
                    }
                    else
                    {
                        cacheGuids.Add(msgs[i], 0);
                        endMsgs.Add(msgs[i]);
                    }
                }
            }
            else
            {
                Debug.Log("去掉Project graphics Settings 中重复的Shader！");
                isNeedUpdate = true;
            }
        }
        if(isAdd && isNeedAdd)
        {
            endMsgs.Add("  - {fileID: 4800000, guid: "+newGuid + ", type: 3}");
            Debug.Log("添加新建shader ： " + shader + " 到Project graphics Settings 中！");
            isNeedUpdate = true;
        }
        if (isNeedUpdate)
        {
            msgs = new string[endMsgs.Count];
            for (int i = 0; i < msgs.Length; i++)
            {
                msgs[i] = endMsgs[i];
            }
            File.WriteAllLines(graphicsSettingsPath, msgs);
            AssetDatabase.Refresh();
        }
    }


}
