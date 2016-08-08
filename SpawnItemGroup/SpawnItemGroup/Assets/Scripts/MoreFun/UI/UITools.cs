using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MoreFun.UI
{
    public class UITools
    {
        /// <summary>
        /// Add the gameobject to the specified parent.
        /// </summary>
        public static GameObject AddChild(Transform parent, GameObject go)
        {
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                RectTransform rt = t as RectTransform;
                t.SetParent(parent, false);
                go.layer = parent.gameObject.layer;

            }
            return go;
        }

        /// <summary>
        /// 将 child 作为自己的孩子，并且和自己等大
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static void AddChildWithSameSize(RectTransform parent, RectTransform child)
        {
            if (parent != null && child != null)
            {
                child.SetParent(parent, false);
                child.localPosition = Vector3.zero;
                child.localRotation = Quaternion.identity;
                child.localScale = Vector3.one;
                child.gameObject.layer = parent.gameObject.layer;
                child.anchoredPosition = Vector2.zero;
            }
        }

        public static void ChangeObjActive(GameObject obj, bool active)
        {
            if (null != obj && obj.activeSelf != active)
            {
                obj.SetActive(active);
            }
        }
        public static void AddChidldWithProperty(GameObject parent, GameObject go)
        {
            if (parent == null || go == null)
            {
                return;
            }

            if (parent.transform == null || go.transform == null)
            {
                return;
            }
            
            RectTransform child = go.transform as RectTransform;
            Vector2 ancMin = child.anchorMin;
            Vector2 ancMax = child.anchorMax;
            Vector2 anchorP = child.anchoredPosition;
            Vector2 formalSizeDelta = child.sizeDelta;
            Vector3 scale = child.localScale;
            Vector3 localPo = child.localPosition;
            Quaternion qu = child.localRotation;
            child.SetParent(parent.transform);
            child.localRotation = qu;
            child.localPosition = localPo;
            child.localScale = scale;
            child.gameObject.layer = parent.gameObject.layer;
            child.anchorMin = ancMin;
            child.anchorMax = ancMax;
            child.anchoredPosition = anchorP;
            child.sizeDelta = formalSizeDelta;
        }

        /// <summary>
        /// Add a go to the specified parent.
        /// </summary>
        public static GameObject AddChild(GameObject parent, GameObject go)
        {
            if (go != null && parent != null)
            {
                AddChild(parent.transform, go);
            }
            return go;
        }


        /// <summary>
        /// Instantiate an object and add it to the specified parent.
        /// </summary>
        public static GameObject InstantiateChild(GameObject parent, GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            AddChild(parent, go);
            return go;
        }

        /// <summary>
        /// Instantiate an object and add it to the specified parent.
        /// </summary>
        public static GameObject InstantiateChild(Transform parent, GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            AddChild(parent, go);
            return go;
        }

        /// <summary>
        /// 获取trans在UI世界坐标系中的位置
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static Vector2 GetUIPositionInWorld(RectTransform trans)
        {
            Vector3 pos = trans.localPosition;
            Transform parent = trans.parent;

            // 保证Canvas的LocalPosition不会影响
            while (parent != null && parent.parent != null)
            {
                pos += parent.localPosition;
                parent = parent.parent;
            }

            return new Vector2(pos.x, pos.y);
        }

        /// <summary>
        /// 计算两个UI之间的相对UI位置
        /// </summary>
        /// <param name="trans1">第一个UI</param>
        /// <param name="trans2">第二个UI</param>
        /// <returns>trans1相对于trans2的位移, 即trans1移动该距离会到trans2的位置</returns>
        public static Vector2 GetTwoUIRelativePos(RectTransform trans1, RectTransform trans2)
        {
            Vector2 pos1 = GetUIPositionInWorld(trans1);
            Vector2 pos2 = GetUIPositionInWorld(trans2);
            return pos1 - pos2;
        }

        private static float ms_pixelToInchRate;
        public static void UpdateReferencePPI(float referencePPI = 326.0f)
        {
            
            #if LOG_DETAIL
            MoreDebug.Log<UITools>("referencePPI=" + referencePPI +
                          ", Screen.dpi=" + Screen.dpi +
                          ", width=" + Screen.width +
                          ", height=" + Screen.height);
            #endif

            float dpi = Screen.dpi;

            if (Mathf.Approximately(dpi, 0.0f))
            {
                #if LOG_DETAIL
                MoreDebug.Log<UITools>("it's dpi is NOT known");
                #endif
                // if the dpi is not valid, p2i is:
                ms_pixelToInchRate = 1.0f;
            }
            else
            {
                float widthPixel = Screen.width;
                float heightPixel = Screen.height;
                if(Mathf.Approximately(widthPixel, 0.0f) || Mathf.Approximately(heightPixel, 0.0f))
                {
                    #if LOG_DETAIL
                    MoreDebug.Log<UITools>("it's size is NOT known");
                    #endif
                    // if the width or height is not valid, p2i is:
                    ms_pixelToInchRate = referencePPI / dpi;
                }
                else
                {
                    float widthInch = widthPixel / dpi;
                    float heightInch = heightPixel / dpi;

                    const float tabletDiagonalInch = 6.0f;
                    float currDiagonalInch = Mathf.Pow((widthInch * widthInch + heightInch * heightInch), 0.5f);
                    if(currDiagonalInch < tabletDiagonalInch)
                    {
                        // if current device is a phone, p2i is:
                        #if LOG_DETAIL
                        MoreDebug.Log<UITools>("currDiagonalInch= " + currDiagonalInch + ", it is a phone");
                        #endif
                        ms_pixelToInchRate = Mathf.Min(1.0f, referencePPI / dpi);
                    }
                    else
                    {
                        #if LOG_DETAIL
                        MoreDebug.Log<UITools>("currDiagonalInch= " + currDiagonalInch + ", it is a tablet");
                        #endif
                        // if current device is a tablet, p2i is:
                        ms_pixelToInchRate = referencePPI / dpi;
                    }
                }
            }

            #if LOG_DETAIL
            MoreDebug.Log<UITools>("referencePPI=" + referencePPI +
                      ", Screen.dpi=" + Screen.dpi +
                      ", ms_pixelToInchRate=" + ms_pixelToInchRate);
            #endif

        }

        /// <summary>
        /// 传入横向纵向像素数，返回横向纵向英寸距离。
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public static Vector2 PixelToInch(Vector2 pixels)
        {
            return pixels * ms_pixelToInchRate;
        }

        /// <summary>
        /// 获取一个RectTransform的UI大小，与缩放方式无关
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Vector2 GetRectTransfromSize(RectTransform transform)
        {
            Vector3[] tmpCorner = new Vector3[4];
            transform.GetLocalCorners(tmpCorner);
            Vector3 size = tmpCorner[2] - tmpCorner[0];
            return size;
        }

        /// <summary>
        /// 两个 RectTransform 是否发生重叠
        /// 算法思想：
        /// 判断第二个 RectTransform 的四个角是否在第一个 RectTransform 的范围之内
        /// 一旦有一个在内部则返回true，否则返回false
        /// 防止一个完全在另一个内部，所以第二个循环也要判断
        /// </summary>
        /// <param name="transform1"></param>
        /// <param name="tranform2"></param>
        /// <returns></returns>
        public static bool GetRectTransformCollide(RectTransform transform1, RectTransform tranform2)
        {
            Vector3[] corners1 = new Vector3[4];
            transform1.GetWorldCorners(corners1);

            Vector3[] corners2 = new Vector3[4];
            tranform2.GetWorldCorners(corners2);

            for (int i = 0; i < 4; ++i)
            {
                Vector3 pos = corners2[i];
                if ((pos.x > corners1[0].x && pos.x < corners1[3].x) &&
                    pos.y > corners1[0].y && pos.y < corners1[1].y)
                {
                    return true;
                }
            }

            for (int i = 0; i < 4; ++i)
            {
                Vector3 pos = corners1[i];
                if ((pos.x > corners2[0].x && pos.x < corners2[3].x) &&
                    pos.y > corners2[0].y && pos.y < corners2[1].y)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
