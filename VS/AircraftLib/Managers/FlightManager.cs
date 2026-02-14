using AircraftLib.Utility;
using AircraftLib.VehicleTypes;
using Nautilus.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework;
using VehicleFramework.Engines;
using static GameInput;

namespace AircraftLib.Managers
{
    public class FlightManager
    {
        public static void ShowHintMessage(string messageToHint)
        {
            Nautilus.Utility.BasicText message = new Nautilus.Utility.BasicText(500, 0);
            message.ShowMessage(messageToHint, 2 * Time.deltaTime);
        }

        public static void StabilizeRoll(ModVehicle mv, bool enabled)
        {
            if (mv == null)
            {
                return;
            }
            
            mv.stabilizeRoll = enabled;
        }

        public static void CheckLandingGear(PlaneVehicle mv)
        {
            if (mv == null)
            {
                return;
            }

            if (mv.gearOut)
            {
                // when a wheel collider is enabled the rigidbody velocity resets to 0, 0, 0
                // so save previous velocity and restore it after the gear is enabled
                Rigidbody rb = mv.GetComponent<Rigidbody>();
                Vector3 velocity = rb.velocity;
                mv.gameObject.FindChild("LandingGear").gameObject.SetActive(true);
                rb.velocity = velocity;
            }
            else
            {
                mv.gameObject.FindChild("LandingGear").gameObject.SetActive(false);
            }
        }

        // thank you to metious
        public static float GetNormalizedAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
            {
                return angle - 360;
            }

            return angle;
        }



        public static void DoPlaneFlight(PlaneVehicle vehicle)
        {
            if (vehicle == null)
            {
                return;
            }
            vehicle.moveOnLand = true;
            Rigidbody rb = vehicle.useRigidbody;

            float airDensity = 1.225f * Mathf.Pow(1f - (vehicle.transform.position.y / 44300f), 4.23f);

            float liftForce = vehicle.GetLiftCoefficient() * vehicle.WingArea * (0.5f * airDensity * rb.velocity.magnitude * rb.velocity.magnitude);

            rb.AddRelativeForce(new Vector3(0f, liftForce, 0f), ForceMode.Force);

            int rudderAngle = 0;
            if (GameInput.GetButtonHeld(AircraftLibPlugin.YawLeftKey))
            {
                rudderAngle -= vehicle.maxYawFromRudder;
            }
            if (GameInput.GetButtonHeld(AircraftLibPlugin.YawRightKey))
            {
                rudderAngle += vehicle.maxYawFromRudder;
            }

            TorqueTowardsMovementVector(rb, vehicle.vertStabilizerFactor, rudderAngle);

            /*ALLogger.Log("AOA: " + vehicle.AngleOfAttack.ToString());
            ALLogger.Log("Lift Coefficient: " + vehicle.GetLiftCoefficient.ToString());
            ALLogger.Log("Air density: " + airDensity.ToString());
            ALLogger.Log("Velocity: " + rb.velocity.magnitude.ToString());
            ALLogger.Log("Lift force: " + liftForce.ToString());*/
        }

        public static void TorqueTowardsMovementVector(Rigidbody rb, float factor, int rudderAngle)
        {
            if (rb.velocity.sqrMagnitude < 1)
                return;

            Quaternion rudderRotation = Quaternion.AngleAxis(rudderAngle, rb.transform.up);

            Vector3 forwardVector = rudderRotation * rb.transform.forward;

            Vector3 xzVelocity = Vector3.ProjectOnPlane(rb.velocity, rb.transform.up).normalized;
            Vector3 xzForward = Vector3.ProjectOnPlane(rb.transform.forward, rb.transform.up).normalized;

            float yawAngleDifference = Vector3.SignedAngle(xzForward, xzVelocity, rb.transform.up);

            float torque = yawAngleDifference * factor * (rb.velocity.magnitude / 40);

            rb.AddRelativeTorque(new Vector3(0, (rudderAngle / 8) * (rb.velocity.magnitude / 40), 0), ForceMode.Acceleration);

            rb.AddTorque(rb.transform.up * torque, ForceMode.Acceleration);
        }

        public static void DoHoverFlight(ModVehicle mv)
        {
            if (mv == null)
            {
                return;
            }
            mv.moveOnLand = true;

            mv.worldForces.aboveWaterDrag = 0.5f;

            if (mv.gameObject.transform.position.y > GetMaxAltitude(mv))
            {
                mv.useRigidbody.AddForce(Physics.gravity * 2);
            }
            else
            {
                mv.worldForces.aboveWaterGravity = 0f;
            }

            mv.useRigidbody.velocity = Vector3.ClampMagnitude(mv.useRigidbody.velocity, (mv as AirshipVehicle).maxSpeed);
        }



        public static bool checkUnderwaterActual(ModVehicle mv)
        {
            if (mv.gameObject.transform.position.y < WaveManager.main.GetWaveHeight(mv.gameObject.transform.position))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static float GetMaxAltitude(ModVehicle mv)
        {
            float currentMVMaxAlt = 1000f;

            return currentMVMaxAlt;
        }
    }
}
