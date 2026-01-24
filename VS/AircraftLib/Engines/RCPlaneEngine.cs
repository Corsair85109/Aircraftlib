using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework.Engines;
using VehicleFramework;
using VehicleFramework.VehicleTypes;
using AircraftLib.Managers;
using AircraftLib.VehicleTypes;
using VehicleFramework.Interfaces;
using VehicleFramework.VehicleRootComponents;
using AircraftLib.Utility;

namespace AircraftLib.Engines
{
    public abstract class RCPlaneEngine : ThrottledEngine, IPlayerListener
    {
        public override bool CanMoveAboveWater { get { return true; } }
        public override bool CanRotateAboveWater { get { return false; } }

        public virtual bool HasThrustVectoring { get { return false; } }


        void IPlayerListener.OnPlayerEntry()
        {
            RealisticControllCrosshair.Instance.SetRealisticCrosshairControlsActive(false, true);
        }
        void IPlayerListener.OnPlayerExit()
        {
            RealisticControllCrosshair.Instance.SetRealisticCrosshairControlsActive(false, true);
        }
        void IPlayerListener.OnPilotBegin()
        {
            RealisticControllCrosshair.Instance.SetRealisticCrosshairControlsActive(false);
        }
        void IPlayerListener.OnPilotEnd()
        {
            RealisticControllCrosshair.Instance.SetRealisticCrosshairControlsActive(false);
        }

        protected virtual float rollSensitivity { get; set; } = -200f;
        protected virtual float pitchSensitivity { get; set; } = -100f;
        protected virtual float yawSensitivity { get; set; } = 5f;

        protected virtual float speedDiv { get; set; } = 100f;

        protected float rollAngle = 0f;
        protected float rollDirection;
        protected float yawValue = 0f;
        protected float inputYawValue = 0f;

        protected bool wasUnderWater;

        public override void Start()
        {
            base.Start();

            wasUnderWater = MV.GetIsUnderwater();
        }

        protected void Rotation()
        {
            if (!MV.IsPlayerControlling()) return;

            if (MV.GetIsUnderwater() == true)
            {
                // underwater

                if (wasUnderWater == false)
                {
                    RealisticControllCrosshair.Instance.SetRealisticCrosshairControlsActive(false, true);
                }
                wasUnderWater = true;

                SeamothRotation(AircraftLibPlugin.ModConfig.invertPitchSubmerged);
            }
            else
            {
                // above water

                if (wasUnderWater == true) 
                {
                    RealisticControllCrosshair.Instance.SetRealisticCrosshairControlsActive(true, true);
                }
                wasUnderWater = false;
                
                RealisticRotation(AircraftLibPlugin.ModConfig.invertPitch);
            }
            RealisticControllCrosshair.Instance.SetRealisticCrosshairControlsActive(!wasUnderWater);
        }

        protected void SeamothRotation(bool inverted = false)
        {
            // stabilize roll
            FlightManager.StabilizeRoll(MV, true);

            // invert pitch setting
            int invert = 1;
            if (inverted)
            {
                invert = -1;
            }

            // normal seamoth kinda rotation (from vehicle framework)
            float pitchFactor = 5f;
            float yawFactor = 5f;
            Vector2 mouseDir = GameInput.GetLookDelta();
            float xRot = mouseDir.x;
            float yRot = mouseDir.y;
            RB.AddTorque(MV.transform.up * xRot * yawFactor * Time.deltaTime, ForceMode.VelocityChange);
            RB.AddTorque(MV.transform.right * yRot * -pitchFactor * invert * Time.deltaTime, ForceMode.VelocityChange);
        }

        protected void RealisticRotation(bool inverted = false)
        {
            // input
            inputYawValue = 0f;
            if (GameInput.GetButtonHeld(AircraftLibPlugin.YawLeftKey))
            {
                inputYawValue += 1f;
            }
            if (GameInput.GetButtonHeld(AircraftLibPlugin.YawRightKey))
            {
                inputYawValue -= 1f;
            }

            // invert pitch setting
            int invert = 1;
            if (inverted)
            {
                invert = -1;
            }

            // base on speed
            float speedMult = RB.velocity.magnitude / speedDiv;

            if (HasThrustVectoring) speedMult = 1f;


            // pitch
            RB.AddTorque(MV.transform.right * Time.deltaTime * pitchSensitivity * RealisticControllCrosshair.Instance.crosshairValue.y * speedMult * invert, ForceMode.Acceleration);

            // disable roll stabilization
            FlightManager.StabilizeRoll(MV, false);

            // do roll with mouse horizontalness
            RB.AddTorque(MV.transform.forward * Time.deltaTime * rollSensitivity * RealisticControllCrosshair.Instance.crosshairValue.x * speedMult, ForceMode.Acceleration);

            // do yaw with bank angle
            rollAngle = MV.transform.localEulerAngles.z;
            rollAngle = FlightManager.GetNormalizedAngle(rollAngle);
            rollDirection = rollAngle / Mathf.Abs(rollAngle);

            // thank you to metious for this
            if (Mathf.Abs(rollAngle) > 90)
            {
                yawValue = Mathf.Lerp(1f, 0f,
                Mathf.InverseLerp(90 * rollDirection, 180 * rollDirection, rollAngle)
                );
            }
            else
            {
                yawValue = Mathf.Lerp(0f, 1f,
                Mathf.InverseLerp(0 * rollDirection, 90 * rollDirection, rollAngle)
                );
            }

            if (inputYawValue != 0f)
            {
                RB.AddRelativeTorque(Vector3.up * Time.deltaTime * yawSensitivity * speedMult * inputYawValue * -3, ForceMode.Acceleration);
            }
            else
            {
                RB.AddRelativeTorque(Vector3.up * Time.deltaTime * yawSensitivity * speedMult * yawValue * rollDirection * -3, ForceMode.Acceleration);
            }
        }



        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Rotation();
        }



    }
}
