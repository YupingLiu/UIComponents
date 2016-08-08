using UnityEngine;

namespace MoreFun.UI
{
    [AddComponentMenu("Layout/Allow Deactive Horizontal Layout Group", 150)]
    public class AllowDeactiveHorizontalLayoutGroup : AllowDeactiveHorizontalOrVerticalLayoutGroup
    {
        protected AllowDeactiveHorizontalLayoutGroup()
        { }
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, false);
        }
        
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, false);
        }
        
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, false);
        }
        
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, false);
        }
    }
}
