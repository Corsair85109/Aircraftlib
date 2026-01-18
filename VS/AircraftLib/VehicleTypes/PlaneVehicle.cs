using AircraftLib.Managers;
using Nautilus.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework;
using VehicleFramework.VehicleTypes;

namespace AircraftLib.VehicleTypes
{
    public abstract class PlaneVehicle : Submersible
    {
        public abstract float UnderwaterDrag {  get; }
        public abstract float AbovewaterDrag { get; }

        public abstract float WingArea { get; set; }
        public virtual float LiftSlope { get { return 0.1f; } }

        public virtual float MaxLiftCoefficient { get { return 1.5f; } }
        public virtual float CriticalAOA { get { return 20f; } }
        public virtual int ZeroLiftAOA { get { return -2; } }


        public float AngleOfAttack { get; private set; }

        private Vector3 lastPosition;

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            worldForces.aboveWaterDrag = AbovewaterDrag;
            worldForces.underwaterDrag = UnderwaterDrag;

            Rigidbody rb = GetComponent<Rigidbody>();

            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            AngleOfAttack = -Mathf.Atan2(localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

            FlightManager.DoPlaneFlight(this);
        }

        public float GetCalculatedLiftCoefficient()
        {
            float liftCoeff = LiftSlope * (AngleOfAttack - ZeroLiftAOA);

            if (AngleOfAttack > CriticalAOA)
            {
                float stall = Mathf.InverseLerp(CriticalAOA, CriticalAOA + 10f, AngleOfAttack);
                liftCoeff = Mathf.Lerp(MaxLiftCoefficient, 0.5f * MaxLiftCoefficient, stall);
            }
            else if (AngleOfAttack < -CriticalAOA)
            {
                float stall = Mathf.InverseLerp(-CriticalAOA, -CriticalAOA - 10f, AngleOfAttack);
            }

            return liftCoeff;
        }

        public bool IsGearOut()
        {
            return true;
        }
    }
}
