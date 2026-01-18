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
using VehicleFramework.VehicleRootComponents;

namespace AircraftLib.Engines
{
    public abstract class ThrottledEngine : ModVehicleEngine
    {
        // disable vf movement
        protected override float FORWARD_TOP_SPEED { get { return 0f; } }
        protected override float REVERSE_TOP_SPEED { get { return 0f; } }
        protected override float STRAFE_MAX_SPEED { get { return 0f; } }
        protected override float VERT_MAX_SPEED { get { return 0f; } }
        protected override float FORWARD_ACCEL { get { return 0f; } }
        protected override float REVERSE_ACCEL { get { return 0f; } }
        protected override float STRAFE_ACCEL { get { return 0f; } }
        protected override float VERT_ACCEL { get { return 0f; } }



        // impliment own movement
        // rotation for this engine should be handled by the subclass

        public abstract float MaxForwardThrust { get; }
        public abstract float MaxReverseThrust { get; }
        public abstract bool HasReverse { get; }
        public abstract Transform ThrustPosition { get; }
        public virtual bool AnyThrustPosition { get { return false; } }

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


        public float throttle = 0f;
        protected float lastThrottle = 0f;

        // throttle percent change per second
        protected virtual int ThrottleChangeStep { get { return 200; } }
        // the max value where the throttle snaps to zero
        protected virtual int throttleDeadzone { get { return 5; } }


        public override void FixedUpdate()
        {
            DoThrottle();

            DrainThrottledPower();
        }

        protected float ClampThrottle()
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

        protected void DeadzoneThrottle()
        {
            if ((-throttleDeadzone < throttle) && (throttle < throttleDeadzone))
            {
                throttle = 0f;
            }
        }

        protected virtual void DoThrottle()
        {
            if (!MV.IsPlayerControlling()) return;

            // change throttle
            if (GameInput.GetButtonHeld(AircraftLibPlugin.IncreaseThrustKey))
            {
                throttle += ThrottleChangeStep * Time.fixedDeltaTime;
                throttle = ClampThrottle();
            }
            else if (GameInput.GetButtonHeld(AircraftLibPlugin.DecreaseThrustKey))
            {
                throttle -= ThrottleChangeStep * Time.fixedDeltaTime;
                throttle = ClampThrottle();
            }
            else
            {
                DeadzoneThrottle();
            }

            ApplyForce();

            // display throttle with nautilus message
            if (throttle != lastThrottle)
            {
                ALUtils.NautilusBasicText("Throttle: " + throttle.ToString(), 1f);
            }
            lastThrottle = throttle;
        }

        protected virtual void ApplyForce()
        {
            if (throttle != 0f)
            {
                if (CanMoveAboveWater || ThrustPosition.position.y < WaveManager.main.GetWaveHeight(ThrustPosition.position))
                {
                    float thrust = (throttle < 0f) ? MaxReverseThrust : MaxForwardThrust;
                    thrust = ((throttle / 100) * thrust) * Time.fixedDeltaTime;
                    Vector3 localForce = new Vector3(0f, 0f, thrust);
                    if (AnyThrustPosition)
                    {
                        RB.AddRelativeForce(localForce, ForceMode.Force);
                    }
                    else
                    {
                        Vector3 worldForce = transform.TransformDirection(localForce);
                        RB.AddForceAtPosition(worldForce, ThrustPosition.position, ForceMode.Force);
                    }

                    if (PitchBack)
                    {
                        RB.AddRelativeTorque(new Vector3(-PitchBackMult * thrust * throttle * Time.fixedDeltaTime, 0f, 0f));
                    }
                }
            }
        }

        public void DrainThrottledPower()
        {
            float upgradeModifier = Mathf.Pow(0.85f, MV.NumEfficiencyModules);

            float throttleMult = Mathf.Abs(throttle / 100) * 3;

            MV.gameObject.EnsureComponent<PowerManager>().TrySpendEnergy(basePowerConsumptionPerSecond * upgradeModifier * throttleMult * Time.deltaTime);
        }



    }
}
