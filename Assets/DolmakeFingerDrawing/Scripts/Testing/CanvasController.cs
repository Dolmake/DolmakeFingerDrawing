using UnityEngine;
using System.Collections;
using DLMK;

public class CanvasController : MonoBehaviour {

    public DLMK.HandDrawingCanvas Canvas;   
    public Material Red, Green, Custom;
    Material[] mats;

    void Start()
    {
        mats = new Material[] {Red, Green, Custom};
    }

    public Rect r = new Rect(0, 0, 64, 64); 
    void OnGUI()
    {
        r = new Rect(0, 64, 64, 64);
        if (GUI.Button(r, "Reset"))
        {
            Canvas.ResetCanvas();
        }

        string right = System.Math.Round(Canvas.Mask.Inside_Color, 2).ToString() + "%";
        GUI.Label(new Rect(r.width, r.y, 200, 32), "Inside " + right);

        string total = System.Math.Round(Canvas.Mask.Total_Color, 2).ToString() + "%";
        GUI.Label(new Rect(r.width, r.y + 32, 200, 32), "Total " + total);

        string xPercent = System.Math.Round(Canvas.Mask.Outside_Color, 2).ToString() + "%";
        GUI.Label(new Rect(r.width, r.y + 64, 200, 32), "OutSide " + xPercent);

        r.y += r.height;
        if (GUI.Button(r, "Erase"))
        {
            Canvas.Mode = DLMK.HandDrawingCanvas.BrushMode.Erasing;
        }
        foreach (Material m in mats)
        {
            r.y += r.height;
            if (GUI.Button(r, m.name))
            {
                Canvas.Mode = DLMK.HandDrawingCanvas.BrushMode.Paiting;
                Canvas.BrushMaterial = m;
            }
        }      
        r.y += r.height;
        r.width = 128;
        r.height = 32;
        GUI.Label(r, "Brush size:" + Canvas.BrushSize);
        r.y += r.height;
        r.height = 16;
        Canvas.BrushSize = GUI.HorizontalScrollbar(r, Canvas.BrushSize * 10f, 1, 0.1f, 10) / 10f;
        
    }
}
