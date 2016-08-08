

using MoreFun.Utils;
using System.Collections.Generic;
using UnityEngine;
namespace MoreFun
{
    public class RendererGroup : MonoBehaviour
    {
        private Renderer[] m_lstRenderer;


        [SerializeField]
        private string m_colorName = "_Ambient";
        private int m_colorId;

        private float m_alpha = -1.0f;
        private float m_alphaBegin;
        private float m_alphaTarget;
        private float m_alphaLerpRatio = 0.0f;
        private bool m_isTweeningAlpha = false;
        private float m_alphaTweenSpeed = 10.0f;
        private string m_alphaTweenOverShader = null;

        void Awake()
        {
            SetColorName(m_colorName);
        }

        void Update()
        {
            if(m_isTweeningAlpha)
            {
                if(gameObject.name == "Cube")
                {
                    int i = 0;
                    ++i;
                }

                m_alphaLerpRatio += Time.deltaTime * m_alphaTweenSpeed;
                float newAlpha = Mathf.Lerp(m_alphaBegin, m_alphaTarget, m_alphaLerpRatio);
                SetAlpha(newAlpha);

                if(m_alphaLerpRatio >= 1.0f)
                {
                    m_isTweeningAlpha = false;
                    SetAlpha(m_alphaTarget);

                    if(null != m_alphaTweenOverShader)
                    {
                        ChangeAllShader(m_alphaTweenOverShader);
                    }
                }
            }
        }

        public void SetRenderEnabled(bool value)
        {
            TryUpdateRenderer();

            for (int i = 0; i < m_lstRenderer.Length; ++i)
            {
                if (CollectionUtil.IsValidIndex(m_lstRenderer, i))
                {
                    m_lstRenderer[i].enabled = value;
                }
            }
        }


        public void TryUpdateRenderer()
        {
            if (null == m_lstRenderer || 0 == m_lstRenderer.Length)
            {
                m_lstRenderer = GetComponentsInChildren<Renderer>();
            }
        }

        /// <summary>
        /// set all renderer use light probe or not
        /// </summary>
        /// <param name="value"></param>
        public void UseLightProbe(bool value)
        {
            TryUpdateRenderer();

            for (int i = 0; i < m_lstRenderer.Length; ++i)
            {
                if (CollectionUtil.IsValidIndex(m_lstRenderer, i))
                {
                    m_lstRenderer[i].useLightProbes = value;
                }
            }
        }

        /// <summary>
        /// modify all shaders of all renderers in the GameObject and its children.
        /// </summary>
        /// <param name="shaderName"></param>
        public void ChangeAllShader(string shaderName)
        {
            TryUpdateRenderer();

            if (null == m_lstRenderer)
            {
                return;
            }

            Shader customShader = Shader.Find(shaderName);
            if (null == customShader)
            {
                return;
            }

            for (int i = 0; i < m_lstRenderer.Length; ++i)
            {
                if (CollectionUtil.IsValidIndex(m_lstRenderer, i))
                {
                    Material[] lstMat = m_lstRenderer[i].materials;
                    if (null != lstMat)
                    {
                        for (int j = 0; j < lstMat.Length; ++j)
                        {
                            if (CollectionUtil.IsValidIndex(lstMat, j))
                            {
                                Material mat = lstMat[j];
                                mat.shader = customShader;
                            }
                        }
                    }
                }
            }
        }

        public void SetColorName(string colorName)
        {
            m_colorName = colorName;
            m_colorId = Shader.PropertyToID(m_colorName);
        }

        public void SetColor(Color color)
        {
            TryUpdateRenderer();

            if (null == m_lstRenderer)
            {
                return;
            }

            for (int i = 0; i < m_lstRenderer.Length; ++i)
            {
                if (CollectionUtil.IsValidIndex(m_lstRenderer, i))
                {
                    Material[] lstMat = m_lstRenderer[i].materials;
                    if (null != lstMat)
                    {
                        for (int j = 0; j < lstMat.Length; ++j)
                        {
                            if (CollectionUtil.IsValidIndex(lstMat, j))
                            {
                                Material mat = lstMat[j];
                                mat.SetColor(m_colorId, color);
                            }
                        }
                    }
                }
            }
        }

        public float GetAlpha()
        {
            return m_alpha;
        }

        public void SetAlpha(float alpha)
        {
            TryUpdateRenderer();
            if (null == m_lstRenderer || 0 == m_lstRenderer.Length)
            {
                return;
            }

            alpha = Mathf.Clamp01(alpha);

            if (Mathf.Abs(m_alpha - alpha) < 0.01f)
            {
                return;
            }


            for (int i = 0; i < m_lstRenderer.Length; ++i)
            {
                if (CollectionUtil.IsValidIndex(m_lstRenderer, i))
                {
                    Material[] lstMat = m_lstRenderer[i].materials;
                    if (null != lstMat)
                    {
                        for (int j = 0; j < lstMat.Length; ++j)
                        {
                            if (CollectionUtil.IsValidIndex(lstMat, j))
                            {
                                Material mat = lstMat[j];
                                Color color = mat.GetColor(m_colorId);
                                color.a = alpha;
                                mat.SetColor(m_colorId, color);
                            }
                        }
                    }
                }
            }


            m_alpha = alpha;
        }

        public void TweenAlpha(float alphaTarget, float speed = 1.0f, string changeShaderWhenTweenOver = null)
        {
            m_isTweeningAlpha = true;
            m_alphaBegin = Mathf.Clamp01(GetAlpha());
            m_alphaTarget = Mathf.Clamp01(alphaTarget);
            m_alphaLerpRatio = 0.0f;
            m_alphaTweenSpeed = speed;

            m_alphaTweenOverShader = changeShaderWhenTweenOver;
        }

    }
}
