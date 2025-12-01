using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class WrapContentInView : LayoutGroup
{
    public float spacingX = 10f;
    public float spacingY = 10f;
    public float maxWidth = 500f; // width of the scroll area or container

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        ArrangeElements();
    }

    public override void CalculateLayoutInputVertical()
    {
        ArrangeElements();
    }

    public override void SetLayoutHorizontal() { }
    public override void SetLayoutVertical() { }

    private void ArrangeElements()
    {
        float x = padding.left;
        float y = -padding.top;
        float rowHeight = 0f;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];
            float w = LayoutUtility.GetPreferredWidth(child);
            float h = LayoutUtility.GetPreferredHeight(child);

            if (x + w + padding.right > maxWidth)
            {
                // New row
                x = padding.left;
                y -= rowHeight + spacingY;
                rowHeight = 0f;
            }

            SetChildAlongAxis(child, 0, x, w);
            SetChildAlongAxis(child, 1, -y, h);

            x += w + spacingX;
            rowHeight = Mathf.Max(rowHeight, h);
        }

        // Set content height for ScrollView
        float totalHeight = Mathf.Abs(y) + rowHeight + padding.bottom;
        SetLayoutInputForAxis(totalHeight, totalHeight, -1, 1);
    }
}
