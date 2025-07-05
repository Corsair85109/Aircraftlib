using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleFramework.Engines;
using UnityEngine;

namespace AircraftLib.Engines
{
    public abstract class BoatEngine : ThrottledEngine
    {
        public abstract float MaxTurnForce { get;}
        public abstract float TurnTiltForce { get;}
        public abstract bool TurnDependsOnSpeed { get;}



        public override void FixedUpdate()
        {
            // thrust
            base.FixedUpdate();


            // turning
            int turnVec = 0;
            if (GameInput.GetKey(AircraftLibPlugin.ModConfig.yawLeftBind) && !GameInput.GetKey(AircraftLibPlugin.ModConfig.yawRightBind))
            {
                // left
                turnVec = -1;
            }
            else if (!GameInput.GetKey(AircraftLibPlugin.ModConfig.yawLeftBind) && GameInput.GetKey(AircraftLibPlugin.ModConfig.yawRightBind))
            {
                // right
                turnVec = 1;
            }

            if (turnVec != 0)
            {
                if (TurnDependsOnSpeed)
                {
                    float torque = MaxTurnForce * Time.fixedDeltaTime * rb.velocity.z;
                    rb.AddRelativeTorque(new Vector3(0f, torque, TurnTiltForce * rb.velocity.z * Time.fixedDeltaTime), ForceMode.Force);
                }
                else
                {
                    rb.AddRelativeTorque(new Vector3(0f, MaxTurnForce * Time.fixedDeltaTime, TurnTiltForce * rb.velocity.z * Time.fixedDeltaTime), ForceMode.Force);
                }
            }

            
        }
    }
}
