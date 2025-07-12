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

        public float waterForwardDrag;
        public float waterVerticalDrag;
        

        public void Start()
        {
            // gravity is applied at buoyant point positions instead of centre
            rb.useGravity = false;

            
        }

        public void FixedUpdate()
        {
            // add own drag
            rb.drag = 0.01f;


            // apply gravity
            rb.AddForceAtPosition(Physics.gravity / buoyantPointCount, transform.position, ForceMode.Acceleration);

            float posY = transform.position.y;
            float seaSurfaceY = WaveManager.main.GetWaveHeight(transform.position);

            if (posY < seaSurfaceY)
            {
                float displaceMult = Mathf.Clamp01((seaSurfaceY - posY) / depthToFullSubmerge);

                // float force
                rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displaceMult * buoyancyMult, 0f), transform.position, ForceMode.Acceleration);

                // water drag force
                Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
                Vector3 dragForce = new Vector3(-localVelocity.x * waterVerticalDrag, -localVelocity.y * waterVerticalDrag, -localVelocity.z * waterForwardDrag);
                rb.AddRelativeForce(displaceMult * dragForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }
}
