using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoreFun.UI
{
    /// <summary>
    /// MonoBehaviour版本的UITextStep还没测试。
    /// 但由于其使用的纯逻辑TextStep已测试过，所以就算有问题也很容易改。
    /// 要求UnityEngine.UI.Text和本脚本处于同一个GameObject身上
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class UITextStep : MonoBehaviour
    {
        private Text m_text;

        private TextStep m_textStep;

        void Awake()
        {
            m_textStep = new TextStep();
        }

        void Start()
        {
            m_text = GetComponent<Text>();
        }
        
        


        /// <summary>
        /// 设置数据。进度退回到刚开始。
        /// </summary>
        /// <param name="arrParagraph">段落文本数据</param>
        /// <param name="wordTime">文字显示间隔时间</param>
        /// <param name="endParagraphTime">段落显示间隔时间</param>
        public void SetData(string[] arrParagraph, float wordTime = 0.5f, float endParagraphTime = 1.0f)
        {
            m_textStep.SetData(arrParagraph, wordTime, endParagraphTime);
        }

        /// <summary>
        /// 保持数据的情况下，将进度退回到刚开始。
        /// </summary>
        public void Reset()
        {
            m_textStep.Reset();
        }

        /// <summary>
        /// 开始播放
        /// </summary>
        public void Play()
        {
            m_textStep.Play();
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause()
        {
            m_textStep.Pause();
        }

        /// <summary>
        /// 恢复播放
        /// </summary>
        public void Resume()
        {
            m_textStep.Resume();
        }
        /// <summary>
        /// 获取当前是否已经播完。
        /// </summary>
        /// <returns></returns>
        public bool GetIsCompleted()
        {
            return m_textStep.GetIsCompleted();
        }

        /// <summary>
        /// 手动调用，直接跳到当前段落的最后一个字。
        /// </summary>
        public void EndOneParagraph()
        {
            m_textStep.EndOneParagraph();
        }

        void Update()
        {
            m_textStep.Update();
            m_text.text = m_textStep.GetText();
        }


    }
    /// <summary>
    /// 纯逻辑、非UI的文字逐步显示功能。
    /// 包含多段文字的逐字显示。
    /// 可以设置文字间的显示间隔时间、
    /// 可以设置段落间的等待时间。
    /// </summary>
    public class TextStep
    {
        private event EventHandler m_OnCompleted;
		public event EventHandler OnCompleted
		{
			add { m_OnCompleted -= value; m_OnCompleted += value; }
			remove { m_OnCompleted -= value; }
		}

        private event EventHandler m_OnOneParaStart;
		public event EventHandler OnOneParaStart
		{
			add { m_OnOneParaStart -= value; m_OnOneParaStart += value; }
			remove { m_OnOneParaStart -= value; }
		}

        private event EventHandler m_OnOneParaCompleted;
		public event EventHandler OnOneParaCompleted
		{
			add { m_OnOneParaCompleted -= value; m_OnOneParaCompleted += value; }
			remove { m_OnOneParaCompleted -= value; }
		}


        private string[] m_arrParagraph;

        private int m_paraIndex = 0;
        private int m_wordIndex = 0;

        private float m_startTime = 0.0f;
        private float m_wordTime = 0.5f;
        private float m_endParaTime = 1.0f;
        private float m_waitTime = 0.5f;

        private bool m_isRunning = false;
        private bool m_isCompleted = false;

        public int CurParaIndex
        {
            get {return m_paraIndex;}
        }

        /// <summary>
        /// 设置数据。进度退回到刚开始。
        /// </summary>
        /// <param name="arrParagraph">段落文本数据</param>
        /// <param name="wordTime">文字显示间隔时间</param>
        /// <param name="endParagraphTime">段落显示间隔时间</param>
        public void SetData(string[] arrParagraph, float wordTime = 0.5f, float endParagraphTime = 1.0f)
        {
            Reset();

            m_arrParagraph = arrParagraph;
            m_wordTime = wordTime;
            m_endParaTime = endParagraphTime;
        }

        /// <summary>
        /// 保持数据的情况下，将进度退回到刚开始。
        /// </summary>
        public void Reset()
        {
            m_paraIndex = 0;
            m_wordIndex = 0;

            m_startTime = 0.0f;
        }

        /// <summary>
        /// 获取当前进度的文字
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if (null != m_arrParagraph)
            {
                string para = m_arrParagraph[m_paraIndex];

                if (m_wordIndex + 1 <= para.Length)
                {
                    return para.Substring(0, m_wordIndex + 1);
                }

                return para;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 开始播放
        /// </summary>
        public void Play()
        {
            if (null != m_arrParagraph && 0 < m_arrParagraph.Length &&
                null != m_arrParagraph[0] && 0 < m_arrParagraph[0].Length)
            {
                m_startTime = Time.realtimeSinceStartup;

                m_waitTime = m_wordTime;

                m_isRunning = true;
                m_isCompleted = false;
            }
            else
            {
#if DEV_BUILD
                Debug.LogWarning("Can NOT start because the paragraphs are invalid!");
#endif
            }

        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause()
        {
            m_isRunning = false;
        }

        /// <summary>
        /// 恢复播放
        /// </summary>
        public void Resume()
        {
            m_isRunning = true;
        }

        /// <summary>
        /// 获取当前是否已经播完。
        /// </summary>
        /// <returns></returns>
        public bool GetIsCompleted()
        {
            return m_isCompleted;
        }

        /// <summary>
        /// 手动调用，直接跳到当前段落的最后一个字。
        /// </summary>
        public void EndOneParagraph()
        {
            // 被外界调度，直接去到段末。所以直接设置段末的状态，包括：
            m_wordIndex = m_arrParagraph[m_paraIndex].Length - 1;
            m_waitTime = m_endParaTime;
            m_startTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 驱动播放。需要用户调用。
        /// </summary>
        public void Update()
        {
            if (m_isRunning)
            {
                float timeNow = Time.realtimeSinceStartup;
                float timeElapsed = timeNow - m_startTime;

                if (timeElapsed >= m_waitTime)
                {
                    m_startTime = timeNow;
                    ++m_wordIndex;

                    if (m_wordIndex == 1)
                    {
                        DoOneParaStart();
                    }

                    if (m_wordIndex == m_arrParagraph[m_paraIndex].Length - 1)
                    {
                        //到达一个段落的结束了，更改等待时间为段落时间
                        m_waitTime = m_endParaTime;
                    }
                    else
                    {
                        m_waitTime = m_wordTime;

                        if (m_wordIndex >= m_arrParagraph[m_paraIndex].Length)
                        {
                            // 已经超过段落了，去到下一行
                            DoOneParaCompleted();

                            m_wordIndex = 0;
                            ++m_paraIndex;
                            
                            if (m_paraIndex >= m_arrParagraph.Length)
                            {
                                //已经结束，保留最后的合法wordIndex、paraIndex，并结束。
                                m_paraIndex = m_arrParagraph.Length - 1;
                                m_wordIndex = m_arrParagraph[m_paraIndex].Length - 1;
                                DoComplete();
                            }

                        }
                    }
                }
            }
        }

        private void DoOneParaStart()
        {
            if (m_OnOneParaStart != null)
            {
                m_OnOneParaStart(this, null);
            }
        }

        private void DoOneParaCompleted()
        {
            if (m_OnOneParaCompleted != null)
            {
                m_OnOneParaCompleted(this, null);
            }
        }

        private void DoComplete()
        {
            m_isCompleted = true;
            m_isRunning = false;

            if (null != m_OnCompleted)
            {
                m_OnCompleted(this, null);
            }
        }
    }
}
