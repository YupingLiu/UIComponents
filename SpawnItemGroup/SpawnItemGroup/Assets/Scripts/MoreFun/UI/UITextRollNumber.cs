using UnityEngine;
using UnityEngine.UI;

namespace MoreFun.UI
{
    /// <summary>
    /// 进行整型数字的滚动显示。
    /// 要求UnityEngine.UI.Text和本脚本处于同一个GameObject身上
    /// </summary>
    
    [RequireComponent(typeof(Text))]
    public class UITextRollNumber : MoreBehaviour
    {
        private Text m_text;

        public delegate void OnRollNumberEventHandler();

        event OnRollNumberEventHandler OnNumberRollEnd;
        event OnRollNumberEventHandler OnNumberRoll;

        [Range(0.01f, 1.0f)]
        public float rollInterval = 0.03f;
        [Range(0.01f, 10.0f)]
        public float rollDuration = 1.0f;

        private float m_numCurr = 2342.0f;
        private float m_numTarget = 0.0f;

        private float m_time = 0.0f;
        private float m_endTime = 0.0f;
        private float m_step = 0;

        private bool m_isRunning = false;

        private string m_prefix = "";
        private string m_postfix = "";
        private int m_floatDotNum = 0;

        public float GetTextWidth()
        {
            return m_text.preferredWidth;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        void Awake()
        {
            m_text = GetComponent<Text>();
            DoShowNumber(m_numCurr);
        }

        void Start()
        {
            
        }

        public void Clear()
        {
            m_text.text = "";
            m_isRunning = false;
            m_endTime = Time.unscaledTime;
        }

        public void AddRollEndCallBack(OnRollNumberEventHandler handler)
        {
            OnNumberRollEnd = handler;
        }

        public void RemoveRollEndCallBack(OnRollNumberEventHandler handler)
        {
            OnNumberRollEnd = null;
        }


        public void AddRollEventCallBack(OnRollNumberEventHandler handler)
        {
            OnNumberRoll = handler;
        }

        public void RemoveRollEventCallBack(OnRollNumberEventHandler handler)
        {
            OnNumberRoll = null;
        }

        public void SetFloatDotNum(int num)
        {
            m_floatDotNum = num;
        }

        public void SetPrefix(string prefix)
        {
            m_prefix = prefix;
            DoShowNumber(m_numCurr);
        }

        public void SetPostfix(string postfix)
        {
            m_postfix = postfix;
            DoShowNumber(m_numCurr);
        }

        public void SetNumber(float number, bool roll = true)
        {
            if (roll)
            {
                if (m_numCurr != number)
                {
                    m_numTarget = number;

                    float stepCount = rollDuration / rollInterval;
                    m_step = (m_numTarget - m_numCurr) / stepCount;
                    m_time = Time.unscaledTime;
                    m_endTime = m_time + rollDuration;

                    m_isRunning = true;
                }
                else
                {
                    StopRoll();
                }
            }
            else
            {
                m_numCurr = m_numTarget = number;
                DoShowNumber(m_numCurr);
            }
        }

        private void StopRoll()
        {
            m_numCurr = m_numTarget;
            m_isRunning = false;
            
            if (null != OnNumberRollEnd)
            {
                OnNumberRollEnd();
            }
        }

        void Update()
        {
            if(m_isRunning)
            {
                float now = Time.unscaledTime;

                if (now >= m_endTime)
                {
                    StopRoll();
                }
                else
                {
                    if (now - m_time > rollInterval)
                    {
                        m_numCurr += m_step;
                        m_time = now;
                    }
                }

                DoShowNumber(m_numCurr);
            }
        }

        private void DoShowNumber(float number)
        {
            /*float finalNum = 0.0f;           
            switch (m_floatDotNum)
            {
                case 0:
                    finalNum = Mathf.Round(m_numCurr);
                    break;
                case 1:
                    finalNum = Mathf.Round(m_numCurr * 10);
                    finalNum *= 0.1f;                    
                    break;
                case 2:
                    finalNum = Mathf.Round(m_numCurr * 100);
                    finalNum *= 0.01f;                   
                    break;
            }
            
            m_text.text = m_prefix +
                       finalNum.ToString() +
                       m_postfix;*/
            float w = m_text.preferredWidth;
            m_text.text = m_prefix +
                       System.Math.Round(m_numCurr, m_floatDotNum).ToString("F" + m_floatDotNum.ToString()) +
                       m_postfix;
            if (Mathf.Abs(w - m_text.preferredWidth) > 1.0f)
            {
                if (null != OnNumberRoll)
                {
                    OnNumberRoll();
                }
            }
        }
    }
}
