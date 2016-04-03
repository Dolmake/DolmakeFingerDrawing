using UnityEngine;
using System.Collections;

namespace DLMK
{
    public class Mask
    {
        public int Total_PX_with_color;
        public int Height_PX;
        public int Width_PX;

        float[,] _mask;
        float[,] _color;

        private float _Inside_Color, _Total_Color, _Outside_Color = 0f;

        public float Outside_Color
        {
            get { return _Outside_Color; }           
        }

        public float Total_Color
        {
            get { return _Total_Color; }           
        }

        public float Inside_Color
        {
            get { return _Inside_Color; }
            
        }

        public void ResetBrush(Texture2D mask)
        {
            Height_PX = mask.width;
            Width_PX = mask.height;
            Debug.Log(string.Format("{0} Mask: {1} {2}",mask.name, Width_PX,Height_PX));

            _mask = new float[Width_PX, Height_PX];
            _color = new float[Width_PX, Height_PX];
            int pixeles = Height_PX * Width_PX;

            int counter = 0;
            for (int x = 0; x < Width_PX; ++x)
            {
                for (int y = 0; y < Height_PX; ++y)
                {
                    float alpha = mask.GetPixel(x, y).a;
                    _mask[x, y] = alpha;
                    _color[x, y] = 0f;
                    counter = alpha > 0f ? counter + 1 : counter;
                }
            }
            Total_PX_with_color = counter;
            _Inside_Color = _Outside_Color = _Total_Color = 0f;
            Debug.Log("Alpha in Image is " + (1f - System.Math.Round((float)Total_PX_with_color / (float)pixeles, 2)).ToString() + "%");
        }

        public void UpdateColorPercent()
        {
            CalculateColorPercent(out _Inside_Color, out _Total_Color, out _Outside_Color);
        }

        public void CalculateColorPercent(out float inside_color, out float total_color, out float outside_percent)
        {
            int colored_outside = 0;
            int colored_inside = 0;
            int total_colored = 0;
            for (int x = 0; x < Width_PX; ++x)
            {
                for (int y = 0; y < Height_PX; ++y)
                {
                    float colored = _color[x, y];
                    float masked = _mask[x, y];
                    if (colored * masked > 0)
                        colored_inside++;
                    else if (colored > 0)
                        colored_outside++;

                    total_colored = colored > 0 ? total_colored + 1 : total_colored;
                }
            }
            inside_color = (float)colored_inside / (float)Total_PX_with_color;
            total_color = (float)total_colored / (float)(Width_PX * Height_PX);
            outside_percent = (float)colored_outside / ((float)(Width_PX * Height_PX) - (float)Total_PX_with_color);
        }

        public void ApplyBrushOnMask(float colorValue, Vector3 pos_VP, float brushSize)
        {
            int X = (int)(pos_VP.x * (float)Width_PX);
            int Y = (int)(pos_VP.y * (float)Height_PX);
            int minX = X - (int)(brushSize * (float)Width_PX * 0.5f);
            int maxX = X + (int)(brushSize * (float)Width_PX * 0.5f);
            minX = minX < 0 ? 0 : minX;
            maxX = maxX > _mask.GetLength(0) - 1 ? _mask.GetLength(0) : maxX;

            int minY = Y - (int)(brushSize * (float)Height_PX * 0.5f);
            int maxY = Y + (int)(brushSize * (float)Height_PX * 0.5f);
            minY = minY < 0 ? 0 : minY;
            maxY = maxY > _mask.GetLength(1) - 1 ? _mask.GetLength(1) : maxY;


            for (int x = minX; x < maxX; ++x)
            {
                for (int y = minY; y < maxY; ++y)
                {
                    _color[x, y] = colorValue;// *_mask[x, y];
                }
            }
        }
    }
}