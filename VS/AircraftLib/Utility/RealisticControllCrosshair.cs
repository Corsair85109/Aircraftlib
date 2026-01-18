using AircraftLib.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework.Interfaces;

namespace AircraftLib.Utility
{
    internal class RealisticControllCrosshair : MonoBehaviour
    {
        private static RealisticControllCrosshair instance = null;
        public static RealisticControllCrosshair Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Player.main.gameObject.EnsureComponent<RealisticControllCrosshair>();
                }
                return instance;
            }
        }

        private Vector2 mousePosition = Vector2.zero;
        private int maxMouseDisplacement = 200;

        private GameObject canvasInstance;
        public GameObject Canvas
        {
            get
            {
                if (canvasInstance == null)
                {
                    canvasInstance = Instantiate(RCCrosshair.rcCrosshairCanvas);
                }
                return canvasInstance;
            }
        }

        public bool realisticCrosshairActive { get; private set; } = false;

        // this is the value that engines should read for turn input (-1 to 1)
        public Vector2 crosshairValue {  get; private set; } = Vector2.zero;


        public void Update()
        {
            if (realisticCrosshairActive)
            {
                Vector2 lookDelta = GameInput.GetLookDelta() * AircraftLibPlugin.ModConfig.CrosshairSensitivity;

                mousePosition += lookDelta;

                // clamp mouse to a circle area
                if (mousePosition.sqrMagnitude > (maxMouseDisplacement * maxMouseDisplacement))
                {
                    mousePosition.Normalize();
                    mousePosition *= maxMouseDisplacement;
                }

                setCrosshairPosition();
                Canvas.SetActive(true);

                crosshairValue = mousePosition / maxMouseDisplacement;
            }
            else
            {
                Canvas.SetActive(false);
            }
        }

        private void setCrosshairPosition()
        {
            if (Canvas.FindChild("CtrlCrosshairImage") == null)
            {
                ALLogger.Error("Couldn't find ctrlcrosshairimage");
                return;
            }
            else
            {
                Canvas.FindChild("CtrlCrosshairImage").gameObject.GetComponent<RectTransform>().anchoredPosition = mousePosition;
            }
        }

        public void SetRealisticCrosshairControlsActive(bool active, bool resetCursor = false)
        {
            if (resetCursor)
            {
                mousePosition = Vector2.zero;
            }
            realisticCrosshairActive = active;
            Canvas.SetActive(active);
        }
    }
}
