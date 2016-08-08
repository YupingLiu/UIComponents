using UnityEngine;

namespace MoreFun.UI
{
    [AddComponentMenu("Layout/Allow Deactive Vertical Layout Group", 151)]
    public class AllowDeactiveVerticalLayoutGroup : AllowDeactiveHorizontalOrVerticalLayoutGroup
    {
        protected AllowDeactiveVerticalLayoutGroup()
        { }
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, true);
        }
        
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, true);
        }
        
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, true);
        }
        
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, true);
        }
    }
}
