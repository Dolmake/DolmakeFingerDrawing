using UnityEngine;
using System.Collections;

namespace P8Core.P8Log
{
    public class HudFPS : MonoBehaviour
    {
        public bool Restart = false;
        public bool ShowResolution = false;
        string _resolutionLabel = "xxx";
        string _fpsLabel = "None";
        int _minFpsCounter = int.MaxValue;
        int _fpsCounter = 0;
        float _accumTime = 0;
        float _slowestAccumTime = 0;
        float MAX_TIME = 1.0f;
        float RealDeltaTime = 0f;
        float m_PrevTimeSinceStartup;

        float _slowestCall = 0f;
        
        public Rect RectLabel = new Rect(0, 0, 128, 128);
        public GUIStyle Style = new GUIStyle();

        // Use this for initialization
        void Start()
        {

            //#if !UNITY_EDITOR
            //        this.enabled = false;
            //#endif

        }

        // Update is called once per frame
        void Update()
        {
            RealDeltaTime = Time.realtimeSinceStartup - m_PrevTimeSinceStartup;
            m_PrevTimeSinceStartup = Time.realtimeSinceStartup;

            _slowestCall = RealDeltaTime > _slowestCall ? RealDeltaTime : _slowestCall;

            if (Restart)
            {
                _minFpsCounter = int.MaxValue;
                _slowestCall = 0f;
                Restart = false;
            }

            _fpsCounter++;
            _accumTime += RealDeltaTime;
            if (_accumTime > MAX_TIME)
            {
                _minFpsCounter = _fpsCounter < _minFpsCounter ? _fpsCounter : _minFpsCounter;
                _fpsLabel = _fpsCounter.ToString() + "|MinFPS:"+ _minFpsCounter.ToString() + "|SlwestF:" + (int)(_slowestCall * 1000) + "ms";
                _accumTime = 0;
                _fpsCounter = 0;
            }

            _slowestAccumTime += RealDeltaTime;
            if (_slowestAccumTime > MAX_TIME * 10f)
            {
                _slowestAccumTime = 0;
                _slowestCall = 0;
            }
            _resolutionLabel = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
            

        }
        void OnGUI()
        {
            GUI.Label(RectLabel, _fpsLabel, Style);
            if (ShowResolution)
                GUI.Label(new Rect(RectLabel.xMin, RectLabel.yMin + RectLabel.height, RectLabel.width, RectLabel.height), _resolutionLabel, Style);


        }
    }

}