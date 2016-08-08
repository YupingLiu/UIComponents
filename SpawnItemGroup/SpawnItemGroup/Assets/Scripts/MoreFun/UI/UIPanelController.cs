using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

namespace MoreFun.UI
{
    /// <summary>
    /// UIPanelController用于控制Panel朝特定的方向进行动画隐藏和显示,专用于UGUI
    /// 无论朝哪个方向进行移动，先要记住Panel的当前位置，然后根据移动的方向进行target position的设定
    /// 暂时设定为可以朝八个方向进行移动操作，规则如下：
    /// 1. 上方，隐藏位置y为originalY - Panel's height；
    /// 2. 下方，隐藏位置y为originalY + Panel's height；
    /// 3. 左测，隐藏位置x为originalX - Panel's width；
    /// 4. 右侧，隐藏位置x为originalX + Panel's width；
    /// 5. 左上，隐藏位置x同左侧x隐藏位置，y同上侧y隐藏位置；
    /// 6. 下同
    /// 该类向外提供两个接口使用，分别是隐藏和显示函数。
    /// 同时支持在移出去的同时进行旋转操作
    /// UIPanelController是使用localPostion进行缓动的
    /// </summary>
    public class UIPanelController : MoreBehaviour
    {
        public enum PanelDirection
        {
            LEFT,
            RIGHT,
            TOP,
            BOTTOM,
            TOP_LEFT,
            TOP_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_RIGHT,
            CENTER,
        }
        public PanelDirection panelDirection = PanelDirection.TOP_LEFT;
        public float animateTime = 0.5f;                                // 动画时间
        public float rotateAngle = 0;                                   // 旋转出去的角度
        [SerializeField]
        [Tooltip("是否出去的时间单独控制")]
        private bool outSeprateControl = false;
        [SerializeField]
        [Tooltip("动画出去的时间")]
        private float outAnimateTime = 0.2f;

        private Tweener m_tween;
        private Vector3 m_originalLocalPos;
        private Vector3 m_targetLocalPos;
        private CanvasGroup m_cvsGp;
        private bool started = false;
        private readonly int GAP = 50;

        void Awake()
        {
            m_cvsGp = gameObject.AddMissingComponent<CanvasGroup>();
        }

        void Start()
        {
            m_originalLocalPos = cachedRectTransform.localPosition;         /* localPosition必须在Start中才会生效 */
            setTargetLocalPos();
            started = true;
            cachedRectTransform.localPosition = m_targetLocalPos;
            Show();
        }

        void OnDestroy()
        {
            if(null != m_tween)
            {
                m_tween.Kill();
            }
        }

        public void SetInteractable( bool enabled )
        {
            if (null != m_cvsGp)
            {
                m_cvsGp.interactable = enabled;
                m_cvsGp.blocksRaycasts = enabled;
            }
        }

        public void Deactive()
        {
            gameObject.SetActive(false);
            if (m_tween != null)
            {
                m_tween.onComplete -= Deactive;
                m_tween = null;
            }
        }

        private void setTargetLocalPos()
        {
            m_targetLocalPos = m_originalLocalPos;
            switch (panelDirection)
            {
                case PanelDirection.LEFT:
                    m_targetLocalPos.x -= (cachedRectTransform.rect.width + GAP);
                    break;
                case PanelDirection.RIGHT:
                    m_targetLocalPos.x += (cachedRectTransform.rect.width + GAP);
                    break;
                case PanelDirection.TOP:
                    m_targetLocalPos.y += (cachedRectTransform.rect.height + GAP);
                    break;
                case PanelDirection.BOTTOM:
                    m_targetLocalPos.y -= (cachedRectTransform.rect.height + GAP);
                    break;
                case PanelDirection.TOP_LEFT:
                    m_targetLocalPos.y += (cachedRectTransform.rect.height + GAP);
                    m_targetLocalPos.x -= (cachedRectTransform.rect.width + GAP);
                    break;
                case PanelDirection.TOP_RIGHT:
                    m_targetLocalPos.y += (cachedRectTransform.rect.height + GAP);
                    m_targetLocalPos.x += (cachedRectTransform.rect.width + GAP);
                    break;
                case PanelDirection.BOTTOM_LEFT:
                    m_targetLocalPos.x -= (cachedRectTransform.rect.width + GAP);
                    m_targetLocalPos.y -= (cachedRectTransform.rect.height + GAP);
                    break;
                case PanelDirection.BOTTOM_RIGHT:
                    m_targetLocalPos.y -= (cachedRectTransform.rect.height + GAP);
                    m_targetLocalPos.x += (cachedRectTransform.rect.width + GAP);
                    break;
            }
        }

        public void Show()
        {
            if (false == started) return;
            gameObject.SetActive(true);
            if (m_tween != null)
            {
                m_tween.onComplete -= Deactive;
                m_tween = null;
            }
            m_tween = DOTween.To(() => cachedRectTransform.localPosition, x => cachedRectTransform.localPosition = x, m_originalLocalPos, animateTime)
                .SetEase(Ease.InOutQuint)
                .SetDelay(animateTime / 6);
            if (rotateAngle > 0)
            {
                cachedTransform.DOLocalRotate(Vector3.zero, animateTime)
                .SetEase(Ease.InOutQuint)
                .SetDelay(animateTime / 6);
            }

            SetInteractable(true);
        }

        public void Hide()
        {
            float hideAnimateTime = outSeprateControl ? outAnimateTime : animateTime;

            m_tween = cachedRectTransform.DOLocalMove(m_targetLocalPos, hideAnimateTime)
                    .SetEase(Ease.InOutQuint)
                    .SetDelay(hideAnimateTime / 6);
            m_tween.onComplete += Deactive;

            switch (panelDirection)
            {
                case PanelDirection.LEFT:
                    if (rotateAngle > 0)
                    {
                        cachedTransform.DOLocalRotate(Quaternion.AngleAxis(-1 * rotateAngle, new Vector3(0, 1, 0)).eulerAngles, hideAnimateTime)
                        .SetEase(Ease.InOutQuint)
                        .SetDelay(hideAnimateTime / 6);
                    }
                    break;
                case PanelDirection.RIGHT:
                    if (rotateAngle > 0)
                    {
                        cachedTransform.DOLocalRotate(Quaternion.AngleAxis(rotateAngle, new Vector3(0, 1, 0)).eulerAngles, hideAnimateTime)
                        .SetEase(Ease.InOutQuint)
                        .SetDelay(hideAnimateTime / 6);
                    }
                    break;
                case PanelDirection.TOP:
                    if (rotateAngle > 0)
                    {
                        cachedTransform.DOLocalRotate(Quaternion.AngleAxis(-1 * rotateAngle, new Vector3(1, 0, 0)).eulerAngles, hideAnimateTime)
                        .SetEase(Ease.InOutQuint)
                        .SetDelay(hideAnimateTime / 6);
                    }
                    break;
                case PanelDirection.BOTTOM:
                    if (rotateAngle > 0)
                    {
                        cachedTransform.DOLocalRotate(Quaternion.AngleAxis(rotateAngle, new Vector3(1, 0, 0)).eulerAngles, hideAnimateTime)
                        .SetEase(Ease.InOutQuint)
                        .SetDelay(hideAnimateTime / 6);
                    }
                    break;
                case PanelDirection.TOP_LEFT:
                    if (rotateAngle > 0)
                    {
                        cachedTransform.DOLocalRotate(Quaternion.AngleAxis(-1 * rotateAngle, new Vector3(1, 1, 0)).eulerAngles, hideAnimateTime)
                        .SetEase(Ease.InOutQuint)
                        .SetDelay(hideAnimateTime / 6);
                    }
                    break;
                case PanelDirection.TOP_RIGHT:
                    if (rotateAngle > 0)
                    {
                        cachedTransform.DOLocalRotate(Quaternion.AngleAxis(rotateAngle, new Vector3(-1, 1, 0)).eulerAngles, hideAnimateTime)
                        .SetEase(Ease.InOutQuint)
                        .SetDelay(hideAnimateTime / 6);
                    }
                    break;
                case PanelDirection.BOTTOM_LEFT:
                    if (rotateAngle > 0)
                    {
                        cachedTransform.DOLocalRotate(Quaternion.AngleAxis(-1 * rotateAngle, new Vector3(-1, 1, 0)).eulerAngles, hideAnimateTime)
                        .SetEase(Ease.InOutQuint)
                        .SetDelay(hideAnimateTime / 6);
                    }
                    break;
                case PanelDirection.BOTTOM_RIGHT:
                    if (rotateAngle > 0)
                    {
                        cachedTransform.DOLocalRotate(Quaternion.AngleAxis(rotateAngle, new Vector3(1, 1, 0)).eulerAngles, hideAnimateTime)
                        .SetEase(Ease.InOutQuint)
                        .SetDelay(hideAnimateTime / 6);
                    }
                    break;
            }

            SetInteractable(false);
        }

        public void HideImmediately()
        {
            if (m_tween != null) m_tween.Kill();
            cachedRectTransform.localPosition = m_targetLocalPos;
            SetInteractable(false);
        }
    }
}