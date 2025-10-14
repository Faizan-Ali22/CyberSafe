/*This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <https://unlicense.org>*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class SpriteOutline : MaskableGraphic
    {
        [SerializeField, Range(0f, 500f)] float _outlineWidth = 100f;
        public float OutlineWidth { get => _outlineWidth; set => _outlineWidth = value; }

        [SerializeField, Range(0f, 500f)] float _cornerRadius = 50f;
        public float CornerRadius { get => _cornerRadius; set => _cornerRadius = value; }

        [SerializeField, Range(1, 20)] int _cornerSegments = 10;
        public int CornerSegments { get => _cornerSegments; set => _cornerSegments = value; }

        [SerializeField, Range(0f, 1f)] float _mappingBias = 0.5f;
        public float MappingBias { get => _mappingBias; set => _mappingBias = value; }

        [SerializeField] bool _fillCenter;
        public bool FillCenter { get => _fillCenter; set => _fillCenter = value; }

        private Vector3[] _corners = new Vector3[4];
        private List<UIVertex> _verts = new List<UIVertex>();

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
            SetMaterialDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            // Clamp corner radius
            var rect = rectTransform.rect;
            var clampedCornerRadius = Mathf.Min((Mathf.Min(rect.width, rect.height) / 2f), _cornerRadius);

            // Offset corner based on clamped corner radius
            rectTransform.GetLocalCorners(_corners);
            _corners[0] += new Vector3(clampedCornerRadius, clampedCornerRadius, 0f);
            _corners[1] += new Vector3(clampedCornerRadius, -clampedCornerRadius, 0f);
            _corners[2] += new Vector3(-clampedCornerRadius, -clampedCornerRadius, 0f);
            _corners[3] += new Vector3(-clampedCornerRadius, clampedCornerRadius, 0f);

            // Calculate dimensions
            var height = _corners[1].y - _corners[0].y;
            var width = _corners[2].x - _corners[1].x;
            var edgeLengths = new[] { height, width, height, width };
            var circumference = 2f * Mathf.PI * Mathf.Lerp(clampedCornerRadius, clampedCornerRadius + _outlineWidth, _mappingBias);
            var around = height * 2f + width * 2f + circumference;
            var cornerLength = circumference / 4f;
            var segmentLength = cornerLength / _cornerSegments;

            var vert = new UIVertex { color = color };
            _verts.Clear();

            // Create corners
            var u = 0f;
            for (var c = 0; c < 4; c++)
            {
                // Create verts
                var origin = _corners[c];
                for (var i = 0; i < _cornerSegments + 1; i++)
                {
                    var angle = (float)i / _cornerSegments * Mathf.PI / 2f + Mathf.PI * 0.5f - Mathf.PI * c * 1.5f;
                    var direction = new Vector3(Mathf.Cos(-angle), Mathf.Sin(-angle), 0f);

                    vert.position = origin + direction * clampedCornerRadius;
                    vert.uv0 = new Vector2(u, 0f);
                    _verts.Add(vert);

                    vert.position = origin + direction * (clampedCornerRadius + _outlineWidth);
                    vert.uv0 = new Vector2(u, 1f);
                    _verts.Add(vert);

                    if (_fillCenter)
                    {
                        vert.position = rect.center;
                        vert.uv0 = new Vector2(u, 0f);
                        _verts.Add(vert);
                    }

                    if (i < _cornerSegments)
                        u += segmentLength / around;
                    else
                        u += edgeLengths[c] / around;
                }
            }

            // Add end verts
            vert = _verts[0];
            vert.uv0 = new Vector2(1f, 0f);
            _verts.Add(vert);

            vert = _verts[1];
            vert.uv0 = new Vector2(1f, 1f);
            _verts.Add(vert);

            if (_fillCenter)
            {
                vert = _verts[2];
                vert.uv0 = new Vector2(1f, 1f);
                _verts.Add(vert);
            }

            // Add verts to VertexHelper
            foreach (var vertex in _verts)
                vh.AddVert(vertex);

            // Add triangles to VertexHelper 
            if (_fillCenter)
            {
                for (var v = 0; v < vh.currentVertCount - 3; v += 3)
                {
                    vh.AddTriangle(v, v + 1, v + 4);
                    vh.AddTriangle(v, v + 4, v + 3);

                    vh.AddTriangle(v + 2, v, v + 3);
                    vh.AddTriangle(v + 2, v + 3, v + 5);
                }
            }
            else
            {
                for (var v = 0; v < vh.currentVertCount - 2; v += 2)
                {
                    vh.AddTriangle(v, v + 1, v + 3);
                    vh.AddTriangle(v, v + 3, v + 2);
                }
            }
        }
    }
}