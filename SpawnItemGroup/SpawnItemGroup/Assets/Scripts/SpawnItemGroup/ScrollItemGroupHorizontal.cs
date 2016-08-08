using UnityEngine;
using UnityEngine.UI;

namespace MoreFun.UI
{
    public class ScrollItemGroupHorizontal : SpawnItemHorizontalGroup
    {
        [SerializeField]
        private ScrollRect m_scrollRect;

        protected override void Awake()
        {
            base.Awake();

            m_scrollRect.onValueChanged.AddListener(OnValueChange);
        }

        protected override void OnDestroy()
        {
            m_scrollRect.onValueChanged.RemoveListener(OnValueChange);
 	        base.OnDestroy();
        }

        private void OnValueChange(Vector2 vector)
        {
            StartCoroutine(StartUpdateView());
        }
    }
}
