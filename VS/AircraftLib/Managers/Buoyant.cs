using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AircraftLib.Managers
{
    public class Buoyant : MonoBehaviour
    {
        public Rigidbody rb;

        public float depthToFullSubmerge;
        public float buoyancyMult;

        public int buoyantPointCount;

        public float waterDrag = 0.99f;
        public float waterAngularDrag = 0.5f;
        

        public void Start()
        {
            // gravity is applied at buoyant point positions instead of centre
            rb.useGravity = false;
        }

        public void FixedUpdate()
        {
            // apply gravity
            rb.AddForceAtPosition(Physics.gravity / buoyantPointCount, transform.position, ForceMode.Acceleration);

            float posY = transform.position.y;
            float seaSurfaceY = WaveManager.main.GetWaveHeight(transform.position.x);

            if (posY < seaSurfaceY)
            {
                float displaceMult = Mathf.Clamp01((seaSurfaceY - posY) / depthToFullSubmerge) * buoyancyMult;

                rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displaceMult, 0f), transform.position, ForceMode.Acceleration);

                rb.AddForce(displaceMult * -rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
                rb.AddTorque(displaceMult * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }
}
