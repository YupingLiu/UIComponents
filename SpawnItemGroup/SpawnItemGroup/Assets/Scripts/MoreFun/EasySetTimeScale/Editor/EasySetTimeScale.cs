using UnityEditor;
using UnityEngine;
namespace MoreFunEditor
{
    public class EasySetTimeScale
    {
        [MenuItem("MoreFun/EasySetTimeScale/timeScale reset to 1.0 &0")]
        private static void EasySetTimeScaleReset()
        {
            Time.timeScale = 1.0f;
#if DEV_BUILD
            Debug.Log("timeScale Reseted. timeScale=" + Time.timeScale);
#endif
        }

        [MenuItem("MoreFun/EasySetTimeScale/timeScale set to 0.1")]
        private static void EasySetTimeScaleTo01()
        {
            Time.timeScale = 0.1f;
#if DEV_BUILD
            Debug.Log("timeScale set to 0.1f. timeScale=" + Time.timeScale);
#endif
        }

        [MenuItem("MoreFun/EasySetTimeScale/timeScale set to 0.5")]
        private static void EasySetTimeScaleTo05()
        {
            Time.timeScale = 0.5f;
#if DEV_BUILD
            Debug.Log("timeScale set to 0.5f. timeScale=" + Time.timeScale);
#endif
        }

        [MenuItem("MoreFun/EasySetTimeScale/timeScale up by 0.1 &=")]
        private static void EasySetTimeScaleUp()
        {
            Time.timeScale = Mathf.Min(3.0f, Time.timeScale + 0.1f);
#if DEV_BUILD
            Debug.Log("timeScale up by 0.05f, timeScale=" + Time.timeScale);
#endif
        }

        [MenuItem("MoreFun/EasySetTimeScale/timeScale down by 0.1 &-")]
        private static void EasySetTimeScaleDown()
        {
            Time.timeScale = Mathf.Max(0.0f, Time.timeScale - 0.1f);
#if DEV_BUILD
            Debug.Log("timeScale down by 0.05f, timeScale=" + Time.timeScale);
#endif
        }

    }
}
