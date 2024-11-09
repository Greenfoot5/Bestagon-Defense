using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// A UI layout that forms a triangle with it's elements
    /// </summary>
    public class TriangleLayout : MonoBehaviour, ILayoutGroup
    {
        [Tooltip("If the layout is flipped along the horizontal axis")]
        [SerializeField]
        private bool isFlippedHorizontally;
        
        [Header("Padding")]
        [Tooltip("Any padding on the left of the layout")]
        [SerializeField]
        private float left;
        [Tooltip("Any padding on the right of the layout")]
        [SerializeField]
        private float right;
        [Tooltip("Any padding at the top of the layout")]
        [SerializeField]
        private float top;
        [Tooltip("Any padding on the bottom of the layout")]
        [SerializeField]
        private float bottom;
        
        [Space(15)]
        
        [Tooltip("The size of each cell in the layout")]
        [SerializeField]
        private Vector2 cellSize = new(50f, 50f);
        [Tooltip("The spacing between each cell in the layout")]
        [SerializeField]
        public Vector2 spacing;
        
        [Tooltip("The alignment of the sprites")]
        [SerializeField]
        private SpriteAlignment alignment;
        
        /// <summary>
        /// Creates the Horizontal Layout for the group
        /// </summary>
        public void SetLayoutHorizontal()
        {
            Debug.Log("Setting Horizontal Layout");
            if (!isFlippedHorizontally)
            {
                // Loops through each of the children
                for (var i = 0; i < transform.childCount; i++)
                {
                    var child = (RectTransform)transform.GetChild(i);
                    
                    // Calculate Offset Based on line
                    int lineIndex = i - TriangleCount(i + 1);
                    float inset = lineIndex * cellSize.x; // Offset from line
                    if (lineIndex != 0)
                        inset += lineIndex * spacing.x; // Offset based on spacing
                    
                    // Set the new rects
                    switch (GetHorizontalEdge())
                    {
                        // From the left edge
                        case RectTransform.Edge.Left:
                            inset += left;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, inset, cellSize.x);
                            break;
                        // From the right edge
                        case RectTransform.Edge.Right:
                            inset += right;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, inset, cellSize.x);
                            break;
                        // Centre based
                        case RectTransform.Edge.Top:
                        case RectTransform.Edge.Bottom:
                        default:
                            float width = ((RectTransform)transform).rect.width;
                            inset += (width - left - right) / 2;
                            inset -= CalculateLine(i + 1) * 0.5f * cellSize.x;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, inset, cellSize.x);
                            break;
                    }
                }
            }
            // The triangle should be horizontally flipped
            else
            {
                // Loop through all the children, but backwards
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    var child = (RectTransform)transform.GetChild(i);
                    
                    // Calculate Offset Based on line
                    int lineIndex = i - TriangleCount(i + 1);
                    float inset = lineIndex * cellSize.x; // Offset from line
                    if (lineIndex != 0)
                        inset += lineIndex * spacing.x; // Offset based on spacing
                    
                    // Set the new rects
                    switch (GetHorizontalEdge())
                    {
                        // From the left edge
                        case RectTransform.Edge.Left:
                            inset += left;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, inset, cellSize.x);
                            break;
                        // From the right edge
                        case RectTransform.Edge.Right:
                            inset += right;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, inset, cellSize.x);
                            break;
                        // Centre based
                        case RectTransform.Edge.Top:
                        case RectTransform.Edge.Bottom:
                        default:
                            float width = ((RectTransform)transform).rect.width;
                            inset += (width - left - right) / 2; // offset all rects to the centre
                            // Offset to the left based on line number
                            inset -= CalculateLine(i + 1) * 0.5f * cellSize.x;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, inset, cellSize.x);
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Creates the vertical element of the layout group
        /// </summary>
        public void SetLayoutVertical()
        {
            Debug.Log("Setting Vertical Layout");
            if (!isFlippedHorizontally)
            {
                // Loop through all the children
                for (var i = 0; i < transform.childCount; i++)
                {
                    var child = (RectTransform)transform.GetChild(i);
                    
                    // Calculate offsets based off line
                    int lineNumber = CalculateLine(i + 1) + 1;
                    float inset = ((lineNumber - 1) * cellSize.y); // So they don't overlap
                    if (lineNumber != 0)
                        inset += lineNumber * spacing.y; // Padding offset
                    
                    // Set the new rects
                    switch (GetVerticalEdge())
                    {
                        // From the top edge
                        case RectTransform.Edge.Top:
                            inset += top;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inset, cellSize.y);
                            break;
                        // From the bottom edge
                        case RectTransform.Edge.Bottom:
                            inset += bottom;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, inset, cellSize.y);
                            break;
                        // Centre based
                        case RectTransform.Edge.Left:
                        case RectTransform.Edge.Right:
                        default:
                            inset += top;
                            // The icons should be moved up slightly as they are offset horizontally by .5
                            inset -= ((lineNumber - 1) * cellSize.y) / 2;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inset, cellSize.y);
                            break;
                    }
                }
            }
            // The triangle should be horizontally flipped
            else
            {
                // Loop through the children
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    var child = (RectTransform)transform.GetChild(i);
                    
                    // Calculate offsets based off line
                    int lineNumber = CalculateLine(transform.childCount) - CalculateLine(i + 1) + 1;
                    float inset = ((lineNumber - 1) * cellSize.y); // So they don't overlap
                    if (lineNumber != 0)
                        inset -= lineNumber * spacing.y; // Padding offset
                    
                    // Set the new rects
                    switch (GetVerticalEdge())
                    {
                        // From the top edge
                        case RectTransform.Edge.Top:
                            inset += top;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inset, cellSize.y);
                            break;
                        // From the bottom edge
                        case RectTransform.Edge.Bottom:
                            inset += bottom;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, inset, cellSize.y);
                            break;
                        // Centre based
                        case RectTransform.Edge.Left:
                        case RectTransform.Edge.Right:
                        default:
                            inset += top;
                            inset -= ((lineNumber - 1) * cellSize.y) / 2;
                            child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, inset, cellSize.y);
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Obtains which horizontal edge to base the group from
        /// </summary>
        /// <returns>Left/Right for horizontal edge alignments, or Top for centre and custom</returns>
        private RectTransform.Edge GetHorizontalEdge()
        {
            switch (alignment)
            {
                // We use the left edge
                case SpriteAlignment.BottomLeft:
                case SpriteAlignment.LeftCenter:
                case SpriteAlignment.TopLeft:
                    return RectTransform.Edge.Left;
                // We use the right edge
                case SpriteAlignment.BottomRight:
                case SpriteAlignment.RightCenter:
                case SpriteAlignment.TopRight:
                    return RectTransform.Edge.Right;
                case SpriteAlignment.Center:
                case SpriteAlignment.TopCenter:
                case SpriteAlignment.BottomCenter:
                case SpriteAlignment.Custom:
                default:
                    // Centre or custom
                    return RectTransform.Edge.Top;
            }
        }
        
        /// <summary>
        /// Obtains which vertical edge to base the group from
        /// </summary>
        /// <returns>Left/Right for horizontal edge alignments, or Top for centre and custom</returns>
        private RectTransform.Edge GetVerticalEdge()
        {
            switch (alignment)
            {
                case SpriteAlignment.BottomLeft:
                case SpriteAlignment.BottomCenter:
                case SpriteAlignment.BottomRight:
                    return RectTransform.Edge.Bottom;
                case SpriteAlignment.TopLeft:
                case SpriteAlignment.TopCenter:
                case SpriteAlignment.TopRight:
                    return RectTransform.Edge.Top;
                case SpriteAlignment.Center:
                case SpriteAlignment.LeftCenter:
                case SpriteAlignment.RightCenter:
                case SpriteAlignment.Custom:
                default:
                    return RectTransform.Edge.Right;
            }
        }
        
        /// <summary>
        /// Calculates the line number an index is on. This is the value as the next triangular number in the sequence
        /// </summary>
        /// <param name="i">The index to check</param>
        /// <returns>The line number</returns>
        private static int CalculateLine(int i)
        {
            var t = 1;
            while (i > TriangleNumber(t))
            {
                t++;
            }

            return t;
        }
        
        /// <summary>
        /// Calculates the triangular number based on an index for the sequence
        /// </summary>
        /// <param name="n">The sequence's index</param>
        /// <returns>The value at the specified index</returns>
        private static int TriangleNumber(int n)
        {
            return (n * (n + 1)) / 2;
        }
        
        /// <summary>
        /// Gets the amount of triangles prior to an index
        /// </summary>
        /// <param name="n">The index on the triangle</param>
        /// <returns>The amount of elements before the index</returns>
        private static int TriangleCount(int n)
        {
            return TriangleNumber(CalculateLine(n) - 1);
        }
    }
}
