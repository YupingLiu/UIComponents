using UnityEditor;
using UnityEngine;
namespace MoreFunEditor
{
    public class EasySaveProject
    {
        private static float ms_lastSaveTime;

        [MenuItem("MoreFun/EasySaveProject/Save Project &s")]
        private static void SaveProject()
        {
            EditorApplication.SaveAssets();
            ms_lastSaveTime = Time.realtimeSinceStartup;
#if DEV_BUILD
            Debug.Log("Saved Project");
#endif
        }

        [MenuItem("MoreFun/EasySaveProject/Enable Auto Save Project")]
        private static void EnableAutoSaveProject()
        {
            EditorApplication.update += OnEditorUpdate;
            ms_lastSaveTime = Time.realtimeSinceStartup;
#if DEV_BUILD
            Debug.Log("Enabled Auto Save Project");
#endif
        }

        [MenuItem("MoreFun/EasySaveProject/Disable Auto Save Project")]
        private static void DisableAutoSaveProject()
        {
            EditorApplication.update -= OnEditorUpdate;
#if DEV_BUILD
            Debug.Log("Disabled Auto Save Project");
#endif
        }

        private static void OnEditorUpdate()
        {
            if(Time.realtimeSinceStartup - ms_lastSaveTime > 5.0f)
            {
                EditorApplication.SaveAssets();
            }
        }
    }
}
