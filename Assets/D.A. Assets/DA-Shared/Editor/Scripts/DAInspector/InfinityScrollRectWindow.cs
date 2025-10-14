using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.DAI
{
    public delegate void DrawItem<T>(T item);

    public class InfinityScrollRectWindow<T> 
    {
        [SerializeField] DAInspector gui;

        private T[] _items;
        private Vector2 _scrollPosition;
        protected int _visibleItemCount;
        protected float _itemHeight;
        private float _totalScrollHeight;
        private float _visibleAreaHeight;
        private DrawItem<T> _drawItem;

        public InfinityScrollRectWindow(int visibleItemCount, float itemHeight, DAInspector gui)
        {
            this.gui = gui;
            _visibleItemCount = visibleItemCount;
            _itemHeight = itemHeight;
        }

        public void SetData(IEnumerable<T> items, DrawItem<T> drawItem)
        {
            _drawItem = drawItem;
            _items = items.ToArray();

            if (_items.Length < _visibleItemCount)
            {
                _visibleItemCount = _items.Length;
            }

            _visibleAreaHeight = _visibleItemCount * _itemHeight;
            _totalScrollHeight = _items.Length * _itemHeight;
        }

        public void OnGUI()
        {
            if (_items == null || _items.Length < 1)
            {
                GUILayout.Label("No data.");
                return;
            }

            if (_drawItem == null)
            {
                GUILayout.Label("DrawItem is missing.");
                return;
            }

            gui.Colorize(() =>
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(_visibleAreaHeight));
            });

            float currentScrollPos = _scrollPosition.y;
            int startIndex = Mathf.Max(0, (int)(currentScrollPos / _itemHeight));
            int endIndex = Mathf.Min(_items.Length, startIndex + _visibleItemCount + 3);

            GUILayout.BeginVertical();
            GUILayout.Space(startIndex * _itemHeight);

            for (int i = startIndex; i < endIndex; i++)
            {
                _drawItem(_items[i]);
            }

            GUILayout.Space(_totalScrollHeight - endIndex * _itemHeight);
            GUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }
    }
}