using System.Collections.Generic;
using UnityEngine;

namespace FusionClient
{
    public class RainbowColor
    {
        private static Color color;

        public static float Speed;

        #region Variables
        /// <summary>
        /// Elements of the Window.
        /// </summary>
        public readonly List<RainbowColor> Elements;

        /// <summary>
        /// ID of the Window.
        /// </summary>
        public readonly int WindowId;

        /// <summary>
        /// Defines if the Window is draggable.
        /// </summary>
        public bool draggable = true;

        /// <summary>
        /// Position of the Window.
        /// </summary>
        public Rect windowRect;

        /// <summary>
        /// Style of the Window.
        /// </summary>
        /// 
        public GUIStyle windowStyle;

        /// <summary>
        /// Defines if the Window can be dragged of Screen.
        /// </summary>
        public bool allowOffscreen;

        public float
            x, y,
            width, height,
            margin,
            controlHeight,
            controlDist;
        private float nextControlY;

        /// <summary>
        /// Title of the Window
        /// </summary>
        public string text;
        #endregion

        public RainbowColor(int windowId, string text, float _x, float _y, float _width, float _height, float _margin, float _controlHeight, float _controlDist)
        {
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            margin = _margin;
            controlHeight = _controlHeight;
            controlDist = _controlDist;
            this.text = text;
            Elements = new List<RainbowColor>();
            windowRect = new Rect(x, y, width, height);
            WindowId = windowId;
            x = 0;
            y = 0;
            nextControlY = y + 20;
        }
        public Rect NextControlRect(int index)
        {

            //Rect r = new Rect(x, nextControlY, width - margin * 2, controlHeight);
            //Rect r = new Rect(x + margin, nextControlY, width - margin * 2, controlHeight);
            Rect r = new Rect(0 + margin, (index + 1) * (controlDist + controlHeight), width - margin * 2, controlHeight);
            return r;
        }

        public RainbowColor(float speed = 0.25f)
        {
            Speed = speed;
            color = Color.HSVToRGB(.34f, .84f, .67f);
        }

        public static Color GetColor()
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            return color = Color.HSVToRGB(h + Time.deltaTime * Speed, s, v);
        }

        private readonly SortedDictionary<int, RainbowColor> elementMap;

        public void AddWindow(int id, string text, float x, float y, float width, float height)
        {
            elementMap.Add(id, new RainbowColor(id, text, x, y, width, height, margin, controlHeight, controlDist));
        }
    }
}
