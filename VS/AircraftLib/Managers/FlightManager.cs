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

            if (mv.IsGearOut())
            {
                mv.gameObject.FindChild("LandingGear").gameObject.SetActive(true);
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

            //rb.useGravity = !vehicle.GetIsUnderwater();

            float airDensity = 1.225f * Mathf.Pow(1f - (vehicle.transform.position.y / 44300f), 4.23f);

            float liftForce = vehicle.GetCalculatedLiftCoefficient() * vehicle.WingArea * (0.5f * airDensity * rb.velocity.magnitude * rb.velocity.magnitude);

            rb.AddRelativeForce(new Vector3(0f, liftForce, 0f), ForceMode.Force);

            ALLogger.Log("AOA: " + vehicle.AngleOfAttack.ToString());
            ALLogger.Log("Lift Coefficient: " + vehicle.GetCalculatedLiftCoefficient().ToString());
            ALLogger.Log("Air density: " + airDensity.ToString());
            ALLogger.Log("Velocity: " + rb.velocity.magnitude.ToString());
            ALLogger.Log("Lift force: " + liftForce.ToString());
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
