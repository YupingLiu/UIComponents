using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    // Button that's meant to work with mouse or touch-based devices.
    [AddComponentMenu("UI/ScrollRectButton", 30)]
    public class ScrollRectButton : Selectable, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
        [FormerlySerializedAs("onClickDown")]
        [SerializeField]
        private ButtonClickedEvent m_OnClickDown = new ButtonClickedEvent();

        [FormerlySerializedAs("onClickUp")]
        [SerializeField]
        private ButtonClickedEvent m_OnClickUp = new ButtonClickedEvent();

        protected ScrollRectButton()
        { }

        public ButtonClickedEvent onClickDown
        {
            get { return m_OnClickDown; }
            set { m_OnClickDown = value; }
        }

        public ButtonClickedEvent onClickUp
        {
            get { return m_OnClickUp; }
            set { m_OnClickUp = value; }
        }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            m_OnClick.Invoke();
        }

        // Trigger all registered callbacks.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive() || !IsInteractable())
                return;

            m_OnClickDown.Invoke();
        }

        // Trigger all registered callbacks.
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive() || !IsInteractable())
                return;

            m_OnClickUp.Invoke();
        }
    }
}
