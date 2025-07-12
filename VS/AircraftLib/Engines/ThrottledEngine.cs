using AircraftLib.Managers;
using AircraftLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework;
using VehicleFramework.Engines;

namespace AircraftLib.Engines
{
    public abstract class ThrottledEngine : ModVehicleEngine
    {
        // disable vf movement
        protected override float FORWARD_TOP_SPEED
        {
            get
            {
                return 0f;
            }
        }
        protected override float REVERSE_TOP_SPEED
        {
            get
            {
                return 0f;
            }
        }
        protected override float STRAFE_MAX_SPEED
        {
            get
            {
                return 0f;
            }
        }
        protected override float VERT_MAX_SPEED
        {
            get
            {
                return 0f;
            }
        }
        protected override float FORWARD_ACCEL
        {
            get
            {
                return 0f;
            }
        }
        protected override float REVERSE_ACCEL
        {
            get
            {
                return 0f;
            }
        }
        protected override float STRAFE_ACCEL
        {
            get
            {
                return 0f;
            }
        }
        protected override float VERT_ACCEL
        {
            get
            {
                return 0f;
            }
        }


        // impliment own movement
        // turning in this engine should be handled by derived subclass

        public abstract float MaxForwardThrust { get; }
        public abstract float MaxReverseThrust { get; }
        public abstract bool HasReverse { get; }
        public abstract Transform ThrustPosition { get; }

        public abstract bool PitchBack { get; }
        public virtual float PitchBackMult 
        { 
            get
            {
                return 0.4f;
            }
        }

        public virtual float basePowerConsumptionPerSecond
        {
            get
            {
                return 0.2f;
            }
        }


        internal float throttle = 0f;
        private float lastThrottle = 0f;

        // throttle percent change per second
        internal virtual int ThrottleChangeStep { get { return 200; } }

        private int throttleDeadzone = 5;


        public override void FixedUpdate()
        {
            DoThrottle();

            DrainThrottledPower();
        }

        private float ClampThrottle()
        {
            if (HasReverse)
            {
                return Mathf.Clamp(throttle, -100, 100);
            }
            else
            {
                return Mathf.Clamp(throttle, 0, 100);
            }
        }

        private void DoThrottle()
        {
            if (!mv.IsPlayerControlling())
            {
                return;
            }

            // change throttle
            if (GameInput.GetKey(AircraftLibPlugin.ModConfig.thrustIncreaseBind))
            {
                throttle += ThrottleChangeStep * Time.fixedDeltaTime;
                throttle = ClampThrottle();
            }
            if (GameInput.GetKey(AircraftLibPlugin.ModConfig.thrustDecreaseBind))
            {
                throttle -= ThrottleChangeStep * Time.fixedDeltaTime;
                throttle = ClampThrottle();
            }

            // deadzone
            if ((-throttleDeadzone < throttle) && (throttle < throttleDeadzone))
            {
                throttle = 0f;
            }

            // add force
            if (throttle != 0f)
            {
                if (CanMoveAboveWater || ThrustPosition.position.y < WaveManager.main.GetWaveHeight(ThrustPosition.position))
                {
                    float thrust = (throttle < 0f) ? MaxReverseThrust : MaxForwardThrust;
                    thrust = ((throttle / 100) * thrust) * Time.fixedDeltaTime;
                    Vector3 localForce = new Vector3(0f, 0f, thrust);
                    Vector3 worldForce = transform.TransformDirection(localForce);
                    rb.AddForceAtPosition(worldForce, ThrustPosition.position, ForceMode.Force);

                    if (PitchBack)
                    {
                        rb.AddRelativeTorque(new Vector3(-PitchBackMult * thrust * throttle * Time.fixedDeltaTime, 0f, 0f));
                    }
                }
            }

            // display throttle with nautilus message
            if (throttle != lastThrottle)
            {
                ALUtils.NautilusBasicText("Throttle: " + throttle.ToString(), 1f);
            }
            lastThrottle = throttle;
        }

        public void DrainThrottledPower()
        {
            float upgradeModifier = Mathf.Pow(0.85f, mv.numEfficiencyModules);

            float throttleMult = Mathf.Abs(throttle / 100) * 3;

            mv.GetComponent<PowerManager>().TrySpendEnergy(basePowerConsumptionPerSecond * upgradeModifier * throttleMult * Time.deltaTime);
        }


        


    }
}
