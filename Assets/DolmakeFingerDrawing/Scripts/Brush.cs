using UnityEngine;
using System.Collections;

public class Brush : MonoBehaviour {


    public Renderer BrushRenderer;
    public Material BrushMaterial
    {
        get
        {
            return BrushRenderer.material;
        }
        set
        {
            BrushRenderer.material = value;
        }
    }
}
