using UnityEngine;
using System.Collections;

namespace DLMK
{
    public class HandDrawingCanvas : MonoBehaviour
    {

        public enum BrushMode : int { Paiting, Erasing }

        /// <summary>
        /// The camera where to render
        /// </summary>
        public Camera Source_Camera;

        /// <summary>
        /// The render camera, combined with the flag DoNotClear do the magic
        /// </summary>
        public Camera RenderTexture_Camera;

        /// <summary>
        /// A background
        /// </summary>
        public GameObject Background;

        /// <summary>
        /// The Brush object. It could be anything
        /// </summary>
        public Brush TBrush;

        /// <summary>
        /// The material used as Eraser
        /// </summary>
        public Material Mat_Eraser;

        /// <summary>
        /// The Material of the Brush. Used for controlling: Painting|Erasing
        /// </summary>
        public Material BrushMaterial;

        /// <summary>
        /// A Mask Texture.
        /// </summary>
        public Texture2D Mask_Texture;

        /// <summary>
        /// The Eraser Object
        /// </summary>
        public GameObject Eraser;

        /// <summary>
        /// Canvas color. Anytime Canvas is cleaned, it gets fill with this color
        /// </summary>
        public Color CanvasColor = Color.red;

        /// <summary>
        /// THe layer to asign to the Brush & to the CameraRenderTexture
        /// </summary>
        public int LayerForRenderTextureCamera = 1;

        /// <summary>
        /// Public events
        /// </summary>
        public event System.Action<DLMK.HandDrawingCanvas> OnStartPress, OnEndPress, OnResetCanvas;      

        /// <summary>
        /// When user press the canvas
        /// </summary>
        public bool PressingOnCanvas
        {
            get { return _PressingOnCanvas; }
        }

        /// <summary>
        /// Brush position on Render Texture Space ([0..1] , [0..1])
        /// </summary>
        public Vector3 BrushPosition_TS
        {
            get { return _BrushPosition_TS; }
        }

        /// <summary>
        /// Brush positon on world space
        /// </summary>
        public Vector3 BrushPosition_WS
        {
            get { return _BrushPosition_WS; }
        }

        /// <summary>
        /// Mode: Or Paiting or Erasing
        /// </summary>
        public BrushMode Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        //Brush size: [0..1]
        public float BrushSize
        {
            get { return _BrushSize; }
            set { _BrushSize = value; }
        }

        /// <summary>
        /// An optional Mask
        /// </summary>
        public Mask Mask
        {
            get { return _mask; }
        }

        //Reset the Canvas.
        public void ResetCanvas()
        {
            RenderTexture_Camera.clearFlags = _flags;           

            if (Background)
                Background.SetActive(true);
           
            StartCoroutine(CO_CleanCanvas());
        }

        void OnEnable()
        {
            Initialize();
            ResetCanvas();
        }

        // Update is called once per frame
        void Update()
        {
            if (PressOnCanvas())
            {
                UpdateBrush();
                if (Mask_Texture)
                {
                    ApplyBrushOnMask();
                    UpdateMaskValues();
                }
            }
            else
            {
                DisableBrush();
            }

            if (Eraser)
                DisableEraser();
        }

      

        void OnDestroy()
        {
            DestroyResources();
        }

        #region Implementation

        bool _PressingOnCanvas = false;
        Vector3 _BrushPosition_TS;
        Vector3 _BrushPosition_WS;
        float _BrushSize = 0.1f;
        BrushMode _Mode = BrushMode.Paiting;
        Mask _mask = new Mask();
        Transform _transform;        
        CameraClearFlags _flags;
        Texture2D _frame = null;

        private void Initialize()
        {
            _transform = this.transform;
            _flags = RenderTexture_Camera.clearFlags;
            NothingCullingMask();
            ShowCullingMask(this.LayerForRenderTextureCamera);
            //TBrush.gameObject.layer = 1 << this.LayerForRenderTextureCamera;
            SetLayerRecursively(TBrush.gameObject, LayerForRenderTextureCamera);
            SetLayerRecursively(Background, LayerForRenderTextureCamera);
        }



        bool PressOnCanvas()
        {
            bool pressOnCanvas = false;
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Vector3 pos_VP = Source_Camera.ScreenToViewportPoint(Input.mousePosition);
                Ray ray = Source_Camera.ViewportPointToRay(pos_VP);
                if (Physics.Raycast(ray, out hit, Source_Camera.farClipPlane))
                {
                    if (hit.transform == _transform)
                    {
                        pressOnCanvas = true;
                        _BrushPosition_WS = hit.point;
                        _BrushPosition_TS = RenderTexture_Camera.WorldToViewportPoint(hit.point);
                    }
                }
            }
            if (!_PressingOnCanvas && pressOnCanvas)
            {
                _PressingOnCanvas = pressOnCanvas;
                if (OnStartPress != null)
                    OnStartPress(this);
            }
            else if (_PressingOnCanvas && !pressOnCanvas)
            {
                _PressingOnCanvas = pressOnCanvas;
                if (OnEndPress != null)
                    OnEndPress(this);
            }


            return _PressingOnCanvas;
        }


        private void ApplyBrushOnMask()
        {
            _mask.ApplyBrushOnMask(_Mode == BrushMode.Paiting ? 1f : 0f, _BrushPosition_TS, _BrushSize);
        }

        private void UpdateMaskValues()
        {
            _mask.UpdateColorPercent();
        }

        void SaveCanvasTexture(Texture2D frame)
        {
            RenderTexture.active = RenderTexture_Camera.targetTexture;
            frame.ReadPixels(new Rect(0, 0, RenderTexture_Camera.targetTexture.width, RenderTexture_Camera.targetTexture.height), 0, 0, false);
            frame.Apply();
        }

        IEnumerator CO_CleanCanvas()
        {
            this.RenderTexture_Camera.backgroundColor = this.CanvasColor;

            yield return new WaitForEndOfFrame();

            CleanCanvas();

            yield return null;
        }

        void CleanCanvas()
        {
            Debug.Log("Clean Canvas");

            //Disable theBackground
            if (Background)
                Background.SetActive(false);

            //Create Backframe
            if (_frame != null)
                Destroy(_frame);
            _frame = new Texture2D(RenderTexture_Camera.targetTexture.width, RenderTexture_Camera.targetTexture.height, TextureFormat.ARGB32, false, false);
            _frame.filterMode = FilterMode.Point;
            _frame.wrapMode = TextureWrapMode.Clamp;

            //Copy the canvas clean
            SaveCanvasTexture(_frame);
            Mat_Eraser.mainTexture = _frame;

            //EraseAll();
            if (Eraser)
            {
                _eraserCounter = 10;
                Eraser.SetActive(true);
            }

            //Prepared for drawing
            RenderTexture_Camera.clearFlags = CameraClearFlags.Nothing;

            if (Mask_Texture)
                _mask.ResetBrush(Mask_Texture);

            if (OnResetCanvas != null)
                OnResetCanvas(this);
        }

        private void DestroyResources()
        {
            if (_frame)
            {
                Destroy(_frame);
                _frame = null;
            }
        }

        private void UpdateBrush()
        {
            TBrush.gameObject.SetActive(true);
            TBrush.transform.position = _BrushPosition_WS;
            TBrush.BrushMaterial = _Mode == BrushMode.Paiting ? BrushMaterial : Mat_Eraser;
            TBrush.transform.localScale = Vector3.one * _BrushSize;
        }

        private void DisableBrush()
        {
            TBrush.gameObject.SetActive(false);
        }

        int _eraserCounter = 0;
        private void DisableEraser()
        {
            if (_eraserCounter <= 0)
            {
                if (Eraser)
                    Eraser.SetActive(false);
            }
            else
            {
                _eraserCounter--;
                //Debug.Log("Eraser : " + _eraserCounter);
            }
        }


        // Set layer for an object
        public static void SetLayerRecursively(GameObject go, int layerNumber)
        {
            if (go == null) return;
            foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layerNumber;
            }
        }

        // Turn on the bit using an OR operation:
        private void ShowCullingMask(int layer)
        {
            RenderTexture_Camera.cullingMask |= 1 << layer;
        }
        // Turn on the bit using an OR operation:
        private void ShowCullingMask(string layer)
        {
            RenderTexture_Camera.cullingMask |= 1 << LayerMask.NameToLayer(layer);
        }


        // Turn off the bit using an AND operation with the complement of the shifted int:
        private void HideCullingMask(int layer)
        {
            RenderTexture_Camera.cullingMask &= ~(1 << layer);
        }

        // Turn off the bit using an AND operation with the complement of the shifted int:
        private void HideCullingMask(string layer)
        {
            RenderTexture_Camera.cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
        }

        // Turn off the bit using an AND operation with the complement of the shifted int:
        private void AllCullingMask()
        {
            RenderTexture_Camera.cullingMask = int.MaxValue;
        }

        // Turn off the bit using an AND operation with the complement of the shifted int:
        private void NothingCullingMask()
        {
            RenderTexture_Camera.cullingMask = 0;
        }

        #endregion

    }
}
