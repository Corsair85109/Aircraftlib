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
        public virtual float UnderwaterDrag { get; set; } = 0.5f;
        public virtual float AbovewaterDrag { get; set; } = 0.05f;

        public virtual float GearExtraDrag { get; set; } = 0.01f;

        public abstract float WingArea { get; set; }
        public virtual float LiftSlope { get { return 0.1f; } }

        public virtual float MaxLiftCoefficient { get { return 1.5f; } }
        public virtual float CriticalAOA { get { return 20f; } }
        public virtual int ZeroLiftAOA { get { return -2; } }

        public bool gearOut = true;


        public float AngleOfAttack;
        public float GLoad;
        private Vector3 lastVelocity = Vector3.zero;

        public override void FixedUpdate()
        {
            float underwaterDrag = gearOut ? UnderwaterDrag + GearExtraDrag : UnderwaterDrag;
            float abovewaterDrag = gearOut ? AbovewaterDrag + GearExtraDrag : AbovewaterDrag;

            worldForces.underwaterDrag = underwaterDrag;
            worldForces.aboveWaterDrag = abovewaterDrag;

            base.FixedUpdate();

            Rigidbody rb = GetComponent<Rigidbody>();

            rb.drag = GetIsUnderwater() ? underwaterDrag : abovewaterDrag;

            AngleOfAttack = GetAngleOfAttack(rb);

            GLoad = GetGLoad(rb, lastVelocity);
            lastVelocity = rb.velocity;

            FlightManager.DoPlaneFlight(this);
        }

        public float GetAngleOfAttack()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            return -Mathf.Atan2(localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;
        }
        public float GetAngleOfAttack(Rigidbody rb)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
            return -Mathf.Atan2(localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;
        }

        public float GetGLoad(Rigidbody rb, Vector3 prevVelocity)
        {
            Vector3 acceleration = (rb.velocity - prevVelocity) / Time.fixedDeltaTime;
            float vertAcceleration = Vector3.Dot(acceleration, transform.up);
            return (vertAcceleration + Physics.gravity.magnitude) / Physics.gravity.magnitude;
        }

        public override void Start()
        {
            base.Start();

            FlightManager.CheckLandingGear(this);
        }

        public override void Update()
        {
            base.Update();

            if (GameInput.GetButtonDown(AircraftLibPlugin.GearKey))
            {
                gearOut = !gearOut;
                FlightManager.CheckLandingGear(this);
            }
        }

        public float GetCalculatedLiftCoefficient()
        {
            float liftCoeff = LiftSlope * (AngleOfAttack - ZeroLiftAOA);

            if (AngleOfAttack > CriticalAOA)
            {
                float stall = Mathf.InverseLerp(CriticalAOA, CriticalAOA + 10f, AngleOfAttack);
                liftCoeff = Mathf.Lerp(MaxLiftCoefficient, 0.5f * MaxLiftCoefficient, stall);
            }

            return liftCoeff;
        }
    }
}
