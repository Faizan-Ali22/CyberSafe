using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using DA_Assets.FCU.Attributes;
using DA_Assets.Tools;

#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

namespace DA_Assets.FCU.Model
{
    public struct FObject : IHaveId, IVisible
    {
        [IgnoreDataMember] public SyncData Data { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("id")]
#endif
        public string Id { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("name")]
#endif
        public string Name { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("type")]
#endif
        public NodeType Type { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("rotation")]
#endif
        public float? Rotation { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("blendMode")]
#endif
        public string BlendMode { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("children")]
#endif
        public List<FObject> Children { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("absoluteBoundingBox")]
#endif
        public BoundingBox AbsoluteBoundingBox { get; set; }

        /// <summary>
        /// <para>The actual bounds of a node accounting for drop shadows, thick strokes, and anything else that may fall outside the node's regular bounding box defined in <see cref="BoundingBox.X" />, <see cref="BoundingBox.Y" />, <see cref="BoundingBox.Width" />, and <see cref="BoundingBox.Height" />.</para>
        /// <para>The <see cref="BoundingBox.X" /> and <see cref="BoundingBox.Y" /> inside this property represent the absolute position of the node on the page.</para>
        /// <para>This value will be null if the node is invisible.</para>
        /// </summary>
#if JSONNET_EXISTS
        [JsonProperty("absoluteRenderBounds")]
#endif
        public BoundingBox AbsoluteRenderBounds { get; set; }

        /// <summary>
        /// Horizontal and vertical layout constraints for a node.
        /// </summary>
#if JSONNET_EXISTS
        [JsonProperty("constraints")]
#endif
        public Constraints Constraints { get; set; }

        /// <summary>
        /// <para>The top two rows of a matrix that represents the 2D transform of this node relative to its parent.</para>
        /// <para>The bottom row of the matrix is implicitly always (0, 0, 1).</para>
        /// <para>Use to transform coordinates in geometry. Only present if <paramref name="geometry=paths" /> is passed.</para>
        /// </summary>
#if JSONNET_EXISTS
        [JsonProperty("relativeTransform")]
#endif
        public List<List<float?>> RelativeTransform { get; set; }

        /// <summary>
        /// <para>Width and height of the element. This is different from the width and height of the bounding box in that the absolute bounding box represents the element after scaling and rotation.</para>
        /// <para>Only present if <paramref name="geometry=paths" /> is passed.</para>
        /// </summary>
#if JSONNET_EXISTS
        [JsonProperty("size")]
#endif
        public Vector2 Size { get; set; }

        /// <summary>
        /// <para>Whether or not this node clip content outside of its bounds.</para>
        /// </summary>
#if JSONNET_EXISTS
        [JsonProperty("clipsContent")]
#endif
        public bool? ClipsContent { get; set; }

        /// <summary>
        /// <para>An array of fill paints applied to the node.</para>
        /// </summary>
#if JSONNET_EXISTS
        [JsonProperty("fills")]
#endif
        public List<Paint> Fills { get; set; }

        /// <summary>
        /// <para>An array of stroke paints applied to the node.</para>
        /// </summary>
#if JSONNET_EXISTS
        [JsonProperty("strokes")]
#endif
        public List<Paint> Strokes { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("cornerRadius")]
#endif
        public float? CornerRadius { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("strokeWeight")]
#endif
        public float StrokeWeight { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("individualStrokeWeights")]
#endif
        public IndividualStrokeWeights IndividualStrokeWeights { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("strokeAlign")]
#endif
        public StrokeAlign StrokeAlign { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("effects")]
#endif
        public List<Effect> Effects { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("overflowDirection")]
#endif
        public OverflowDirection OverflowDirection { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("fillGeometry")]
#endif
        public List<FillGeometry> FillGeometry { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("strokeGeometry")]
#endif
        public List<FillGeometry> StrokeGeometry { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("strokeCap")]
#endif
        public StrokeCap StrokeCap { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("strokeJoin")]
#endif
        public string StrokeJoin { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("strokeMiterAngle")]
#endif
        public float? StrokeMiterAngle { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("opacity")]
#endif
        public float? Opacity { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("preserveRatio")]
#endif 
        public bool? PreserveRatio { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("layoutAlign")]
#endif
        public LayoutAlign LayoutAlign { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("layoutGrow")]
#endif
        public float? LayoutGrow { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("characters")]
#endif
        public string Characters { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("style")]
#endif
        public Style Style { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("styleOverrideTable")]
#endif
        public Dictionary<string, Style> StyleOverrideTable { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("layoutMode")]
#endif
        public LayoutMode LayoutMode { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("itemSpacing")]
#endif
        public float? ItemSpacing { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("counterAxisSpacing")]
#endif
        public float? CounterAxisSpacing { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("visible")]
#endif
        public bool? Visible { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("primaryAxisSizingMode")]
#endif
        public PrimaryAxisSizingMode PrimaryAxisSizingMode { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("counterAxisSizingMode")]
#endif
        public CounterAxisSizingMode CounterAxisSizingMode { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("counterAxisAlignContent")]
#endif
        public string CounterAxisAlignContent { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("primaryAxisAlignItems")]
#endif
        public PrimaryAxisAlignItem PrimaryAxisAlignItems { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("counterAxisAlignItems")]
#endif
        public CounterAxisAlignItem CounterAxisAlignItems { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("layoutWrap")]
#endif
        public LayoutWrap LayoutWrap { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("isMask")]
#endif
        public bool? IsMask { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("paddingLeft")]
#endif
        public float? PaddingLeft { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("paddingRight")]
#endif
        public float? PaddingRight { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("paddingTop")]
#endif
        public float? PaddingTop { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("paddingBottom")]
#endif
        public float? PaddingBottom { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("horizontalPadding")]
#endif
        public float? HorizontalPadding { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("verticalPadding")]
#endif
        public float? VerticalPadding { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("rectangleCornerRadii")]
#endif
        public List<float> CornerRadiuses { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("arcData")]
#endif
        public ArcData ArcData { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("strokeDashes")]
#endif
        public List<float> StrokeDashes { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("layoutPositioning")]
#endif
        public LayoutPositioning LayoutPositioning { get; set; }
    }

    public struct FigmaComponent
    {
#if JSONNET_EXISTS
        [JsonProperty("key")]
#endif
        public string Key { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("name")]
#endif
        public string Name { get; set; }
    }

    public struct FigmaProject
    {
#if JSONNET_EXISTS
        [JsonProperty("document")]
#endif
        public FObject Document { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("name")]
#endif
        public string Name { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("components")]
#endif
        public Dictionary<string, FigmaComponent> Components { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("nodes")]
#endif
        public Dictionary<string, FigmaProject> Nodes { get; set; }
    }

    public struct Constraints
    {
#if JSONNET_EXISTS
        [JsonProperty("vertical")]
#endif
        public string Vertical { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("horizontal")]
#endif
        public string Horizontal { get; set; }
    }

    public enum PaintType
    {
        NONE,
        [PaintPriority(1)] SOLID,
        [PaintPriority(2)] GRADIENT_LINEAR,
        [PaintPriority(4)] GRADIENT_RADIAL,
        [PaintPriority(3)] GRADIENT_ANGULAR,
        [PaintPriority(5)] GRADIENT_DIAMOND,
        IMAGE,
        EMOJI,
        VIDEO
    }

    /// <summary>
    /// A solid color, gradient, or image texture that can be applied as fills or strokes
    /// </summary>
    public struct Paint : IVisible
    {
#if JSONNET_EXISTS
        [JsonProperty("blendMode")]
#endif
        public string BlendMode { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("type")]
#endif
        public PaintType Type { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("color")]
#endif
        public Color Color { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("visible")]
#endif
        public bool? Visible { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("scaleMode")]
#endif
        public string ScaleMode { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("scalingFactor")]
#endif
        public string ScalingFactor { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("imageRef")]
#endif
        public string ImageRef { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("gifRef")]
#endif
        public string GifRef { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("imageTransform")]
#endif
        public List<List<float>> ImageTransform { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("gradientHandlePositions")]
#endif
        public List<Vector2> GradientHandlePositions { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("gradientStops")]
#endif
        public List<GradientStop> GradientStops { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("opacity")]
#endif
        public float? Opacity { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("filters")]
#endif
        public Filters Filters { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("rotation")]
#endif
        public float? Rotation { get; set; }
    }

    public enum EffectType
    {
        NONE,
        INNER_SHADOW,
        DROP_SHADOW,
        LAYER_BLUR,
        BACKGROUND_BLUR
    }

    public interface IVisible
    {
        bool? Visible { get; set; }
    }

    /// <summary>
    /// A visual effect such as a shadow or blur.
    /// </summary>
    public struct Effect : IVisible
    {
#if JSONNET_EXISTS
        [JsonProperty("type")]
#endif
        public EffectType Type { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("visible")]
#endif
        public bool? Visible { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("color")]
#endif
        public Color Color { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("opacity")]
#endif
        public float? Opacity { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("blendMode")]
#endif
        public string BlendMode { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("offset")]
#endif
        public Vector2 Offset { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("radius")]
#endif
        public float Radius { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("showShadowBehindNode")]
#endif
        public bool? ShowShadowBehindNode { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("spread")]
#endif
        public float? Spread { get; set; }
    }

    public struct Style
    {
#if JSONNET_EXISTS
        [JsonProperty("fontFamily")]
#endif
        public string FontFamily { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("fontPostScriptName")]
#endif
        public string FontPostScriptName { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("fontWeight")]
#endif
        public int FontWeight { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("paragraphSpacing")]
#endif
        public float ParagraphSpacing { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("textAutoResize")]
#endif
        public TextAutoResize TextAutoResize { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("fontSize")]
#endif
        public float FontSize { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("textAlignHorizontal")]
#endif
        public string TextAlignHorizontal { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("textAlignVertical")]
#endif
        public string TextAlignVertical { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("letterSpacing")]
#endif
        public float LetterSpacing { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("lineHeightPx")]
#endif
        public float LineHeightPx { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("lineHeightPercentFontSize")]
#endif
        public float? LineHeightPercentFontSize { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("textCase")]
#endif
        public string TextCase { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("textDecoration")]
#endif
        public string TextDecoration { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("italic")]
#endif
        public bool? Italic { get; set; }
    }

    public struct Filters
    {
#if JSONNET_EXISTS
        [JsonProperty("exposure")]
#endif
        public float? Exposure { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("contrast")]
#endif
        public float? Contrast { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("saturation")]
#endif
        public float? Saturation { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("temperature")]
#endif
        public float? Temperature { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("tint")]
#endif
        public float? Tint { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("highlights")]
#endif
        public float? Highlights { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("shadows")]
#endif
        public float? Shadows { get; set; }
    }

    public struct GradientStop
    {
#if JSONNET_EXISTS
        [JsonProperty("color")]
#endif
        public Color Color { get; set; }

#if JSONNET_EXISTS
        [JsonProperty("position")]
#endif
        public float Position { get; set; }
    }

    public struct FillGeometry
    {
#if JSONNET_EXISTS
        [JsonProperty("path")]
#endif
        public string Path { get; set; }
    }

    /// <summary>
    /// Information about the arc properties of an ellipse. 0° is the x axis and increasing angles rotate clockwise.
    /// </summary>
    public struct ArcData
    {
#if JSONNET_EXISTS
        [JsonProperty("startingAngle")]
#endif
        public float StartingAngle { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("endingAngle")]
#endif
        public float EndingAngle { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("innerRadius")]
#endif
        public float InnerRadius { get; set; }
    }

    public struct BoundingBox
    {
#if JSONNET_EXISTS
        [JsonProperty("x")]
#endif
        public float? X { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("y")]
#endif
        public float? Y { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("width")]
#endif
        public float? Width { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("height")]
#endif
        public float? Height { get; set; }
    }

    public enum HyperlinkType
    {
        NONE,
        URL,
        NODE
    }

    public struct Hyperlink
    {
#if JSONNET_EXISTS
        [JsonProperty("type")]
#endif
        public HyperlinkType Type { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("url")]
#endif
        public string Url { get; set; }
    }

    public struct Styles
    {
#if JSONNET_EXISTS
        [JsonProperty("stroke")]
#endif
        public string Stroke { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("fill")]
#endif
        public string Fill { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("fills")]
#endif
        public string Fills { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("text")]
#endif
        public string Text { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("effect")]
#endif
        public string Effect { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("strokes")]
#endif
        public string Strokes { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("grid")]
#endif
        public string Grid { get; set; }
    }

    public enum ConstraintType
    {
        NONE,
        SCALE,
        WIDTH,
        HEIGHT
    }

    public struct Constraint
    {
#if JSONNET_EXISTS
        [JsonProperty("type")]
#endif
        public ConstraintType Type { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("value")]
#endif
        public float Value { get; set; }
    }

    public struct ExportSetting
    {
#if JSONNET_EXISTS
        [JsonProperty("suffix")]
#endif
        public string Suffix { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("format")]
#endif
        public string Format { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("constraint")]
#endif
        public Constraint Constraint { get; set; }
    }

    public struct LayoutGrid
    {
#if JSONNET_EXISTS
        [JsonProperty("pattern")]
#endif
        public string Pattern { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("sectionSize")]
#endif
        public float SectionSize { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("visible")]
#endif
        public bool Visible { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("color")]
#endif
        public Color Color { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("alignment")]
#endif
        public string Alignment { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("gutterSize")]
#endif
        public float GutterSize { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("offset")]
#endif
        public float Offset { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("count")]
#endif
        public int Count { get; set; }
    }

    public struct OpentypeFlags
    {
#if JSONNET_EXISTS
        [JsonProperty("CASE")]
#endif
        public int Case { get; set; }
    }

    public struct IndividualStrokeWeights
    {
#if JSONNET_EXISTS
        [JsonProperty("top")]
#endif
        public float Top { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("right")]
#endif
        public float Right { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("bottom")]
#endif
        public float Bottom { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("left")]
#endif
        public float Left { get; set; }
    }

    public struct FlowStartingPoint
    {
#if JSONNET_EXISTS
        [JsonProperty("nodeId")]
#endif
        public string NodeId { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("name")]
#endif
        public string Name { get; set; }
    }

    public enum PrototypeDeviceType
    {
        NONE,
        PRESET,
        CUSTOM,
        PRESENTATION
    }

    public struct PrototypeDevice
    {
#if JSONNET_EXISTS
        [JsonProperty("type")]
#endif
        public PrototypeDeviceType Type { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("rotation")]
#endif
        public string Rotation { get; set; }
    }

    public enum LayoutPositioning
    {
        AUTO,
        ABSOLUTE
    }

    public enum NodeType
    {
        NONE,
        DOCUMENT,
        CANVAS,
        FRAME,
        GROUP,
        SECTION,
        VECTOR,
        BOOLEAN_OPERATION,
        STAR,
        LINE,
        ELLIPSE,
        REGULAR_POLYGON,
        RECTANGLE,
        TABLE,
        TABLE_CELL,
        TEXT,
        SLICE,
        COMPONENT,
        COMPONENT_SET,
        INSTANCE,
        STICKY,
        SHAPE_WITH_TEXT,
        CONNECTOR,
        WASHI_TAPE
    }

    public enum PrimaryAxisAlignItem
    {
        NONE,
        MIN,
        CENTER,
        MAX,
        SPACE_BETWEEN
    }

    public enum CounterAxisAlignItem
    {
        NONE,
        MIN,
        CENTER,
        MAX,
        BASELINE,
        SPACE_BETWEEN//?
    }

    public enum StrokeAlign
    {
        NONE,
        INSIDE,
        OUTSIDE,
        CENTER
    }

    public enum OverflowDirection
    {
        NONE,
        HORIZONTAL_SCROLLING,
        VERTICAL_SCROLLING,
        HORIZONTAL_AND_VERTICAL_SCROLLING
    }

    public enum StrokeCap
    {
        NONE,
        ROUND,
        SQUARE,
        LINE_ARROW,
        TRIANGLE_ARROW
    }

    public enum LayoutMode
    {
        NONE,
        HORIZONTAL,
        VERTICAL
    }

    public enum LayoutWrap
    {
        NONE,
        NO_WRAP,
        WRAP
    }

    public enum LayoutAlign
    {
        NONE,
        INHERIT,
        STRETCH
    }

    public enum TextAutoResize
    {
        NONE,
        HEIGHT,
        WIDTH_AND_HEIGHT,
        TRUNCATE
    }

    public enum PrimaryAxisSizingMode
    {
        NONE,
        FIXED,
        AUTO
    }

    public enum CounterAxisSizingMode
    {
        NONE,
        FIXED,
        AUTO
    }
}