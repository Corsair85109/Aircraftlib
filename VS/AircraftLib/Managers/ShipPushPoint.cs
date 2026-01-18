using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Base;

namespace AircraftLib.Managers
{
    public class ShipPushPoint : HandTarget, IHandTarget
    {
        public Transform vehicle;
        private Rigidbody rb;

        public Side side;

        public float pushForce;


        public float rollForce = 40f;


        public override void Awake()
        {
            base.Awake();

            rb = vehicle.GetComponent<Rigidbody>();
        }

        public void OnHandHover(GUIHand hand)
        {
            string displayString = "Push";
            HandReticle.main.SetText(HandReticle.TextType.Hand, displayString, true, GameInput.Button.LeftHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnHandClick(GUIHand hand)
        {
            if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
            {
                // push back
                if (side == Side.Centre)
                {
                    rb.AddRelativeForce(new Vector3(0f, pushForce / 2, -pushForce), ForceMode.Impulse);
                }


                // roll
                if (side == Side.Left)
                {
                    rb.AddRelativeTorque(new Vector3(0, 0, -rollForce), ForceMode.VelocityChange);
                }
                else if (side == Side.Right)
                {
                    rb.AddRelativeTorque(new Vector3(0, 0, rollForce), ForceMode.VelocityChange);
                }
            }
        }




        public enum Side { Left, Right, Centre };
    }
}
