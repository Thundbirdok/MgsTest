using UnityEngine;

namespace FpsLimiter
{
    public class FPSLimiter : MonoBehaviour
    {
        public int targetFPS = 60;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0; 
            Application.targetFrameRate = targetFPS;
        }
    }
}
