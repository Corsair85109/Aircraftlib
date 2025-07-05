using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
        public abstract Vector3 ForwardThrustPosition { get; }
        public abstract bool HasReverse { get; }


        internal float throttle = 0f;

        // throttle percent change per second
        private int throttleChangeStep = 50;

        private int throttleDeadzone = 5;


        public override void FixedUpdate()
        {
            return;
            // change throttle
            if (GameInput.GetKey(AircraftLibPlugin.ModConfig.thrustIncreaseBind))
            {
                throttle += throttleChangeStep * Time.fixedDeltaTime;
                ClampThrottle();
            }
            if (GameInput.GetKey(AircraftLibPlugin.ModConfig.thrustDecreaseBind))
            {
                throttle -= throttleChangeStep * Time.fixedDeltaTime;
                ClampThrottle();
            }

            // deadzone
            if ((-throttleDeadzone < throttle) && (throttle < throttleDeadzone))
            {
                throttle = 0f;
            }

            // add force
            if (throttle != 0f)
            {
                float thrust = (throttle < 0f) ? MaxReverseThrust : MaxForwardThrust;
                rb.AddForceAtPosition(Vector3.forward * (throttle / 100) * thrust * Time.fixedDeltaTime, ForwardThrustPosition, ForceMode.Force);
            }
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
    }
}
