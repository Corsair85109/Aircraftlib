using AircraftLib.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework.VehicleTypes;

namespace AircraftLib.VehicleTypes
{
    public abstract class ShipVehicle : Submarine
    {
        public abstract List<GameObject> BuoyancyPoints { get; }

        public abstract float BuoyancyDepthToFullSubmerge { get; }
        public abstract float BuoyancyMult { get; }

        public override void Awake()
        {
            base.Awake();

            int count = BuoyancyPoints.Count;

            foreach (GameObject point in BuoyancyPoints)
            {
                Buoyant buoyant = point.EnsureComponent<Buoyant>();

                buoyant.rb = gameObject.GetComponent<Rigidbody>();
                buoyant.depthToFullSubmerge = BuoyancyDepthToFullSubmerge;
                buoyant.buoyancyMult = BuoyancyMult;
                buoyant.buoyantPointCount = count;
            }
        }
    }
}
