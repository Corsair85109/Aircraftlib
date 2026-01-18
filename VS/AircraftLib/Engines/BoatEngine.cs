using AircraftLib.Managers;
using AircraftLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VehicleFramework;
using VehicleFramework.Engines;
using VehicleFramework.Interfaces;
using static GameInput;

namespace AircraftLib.Engines
{
    public abstract class BoatEngine : ThrottledEngine, IPlayerListener
    {
        public abstract float MaxTurnForce { get;}
        public abstract float TurnTiltForce { get;}
        public abstract int SpeedAtMaxTilt { get;}
        public abstract int SpeedAtMaxYaw { get;}
        public abstract bool TurnDependsOnSpeed { get;}


        // stop mouse rotation
        public override void ControlRotation()
        {
            return;
        }

        // stop vf drag
        protected override void ApplyDrag(Vector3 move)
        {
            return;
        }
        protected override float DragDecay
        {
            get 
            { 
                return 0f; 
            }
        }


        public override void FixedUpdate()
        {
            // thrust
            base.FixedUpdate();


            // turning
            int turnVec = 0;
            if ( GameInput.GetButtonHeld(AircraftLibPlugin.YawLeftKey) && !GameInput.GetButtonHeld(AircraftLibPlugin.YawRightKey) )
            {
                // left
                turnVec = -1;
            }
            else if ( !GameInput.GetButtonHeld(AircraftLibPlugin.YawLeftKey) && GameInput.GetButtonHeld(AircraftLibPlugin.YawRightKey) )
            {
                // right
                turnVec = 1;
            }

            if (turnVec != 0 && ThrustPosition.position.y < WaveManager.main.GetWaveHeight(ThrustPosition.position))
            {
                // Get moving forward or backwards
                float dot = Vector3.Dot(RB.velocity, transform.forward);
                float moveVector = 1;

                if (dot < 0)
                {
                    moveVector = -1;
                }

                // calculate turn forces
                float roll = turnVec * TurnTiltForce * Time.fixedDeltaTime * Mathf.Clamp01(Mathf.Abs(RB.velocity.magnitude) / SpeedAtMaxTilt);
                float yaw = turnVec * moveVector * MaxTurnForce * Time.fixedDeltaTime;

                if (TurnDependsOnSpeed)
                {
                    yaw = yaw * Mathf.Clamp01(Mathf.Abs(RB.velocity.magnitude) / (SpeedAtMaxYaw));
                }

                RB.AddRelativeTorque(new Vector3(0f, yaw, roll), ForceMode.Force);
            }

            
        }





        // blizzard's code from the beluga

        private Player _player;

        private MainCameraControl _mcc;

        private Player player
        {
            get
            {
                bool flag = this._player == null;
                if (flag)
                {
                    this._player = base.GetComponent<Player>();
                }
                return this._player;
            }
        }
        private MainCameraControl mcc
        {
            get
            {
                this._mcc = MainCameraControl.main;

                return this._mcc;
            }
        }

        public bool isFreeLooking = false;
        private bool wasFreelyPilotingLastFrame = false;
        private Quaternion savedCameraRotation;

        public void UpdateCallFreelook()
        {
            if (isFreeLooking)
            {
                UpdateFreelook();
            }
        }

        public void Update()
        {
            UpdateCallFreelook();
        }

        public void UpdateFreelook()
        {
            if (isFreeLooking)
            {
                float deadzone = 20f / 100f;
                bool triggerState = (UnityEngine.Input.GetAxisRaw("ControllerAxis3") > deadzone) || (UnityEngine.Input.GetAxisRaw("ControllerAxis3") < -deadzone);

                ExecuteFreeLook(MV);

                if (triggerState && !wasFreelyPilotingLastFrame)
                {
                    StopFreelook();
                }

                wasFreelyPilotingLastFrame = triggerState;
            }
        }

        private void ExecuteFreeLook(Vehicle vehicle)
        {
            OxygenManager oxygenMgr = Player.main.oxygenMgr;
            oxygenMgr.AddOxygen(Time.deltaTime);
            MoveCamera();
        }

        public void BeginFreeLook()
        {
            if (!isFreeLooking)
            {

                isFreeLooking = true;
                savedCameraRotation = mcc.transform.localRotation;
                mcc.cinematicMode = true;
                mcc.rotationX = mcc.camRotationX;
                mcc.rotationY = mcc.camRotationY;
                mcc.transform.Find("camOffset/pdaCamPivot").localRotation = Quaternion.identity;
            }
        }

        public void StopFreelook()
        {
            if (isFreeLooking)
            {
                isFreeLooking = false;
                mcc.cinematicMode = false;
                mcc.transform.localRotation = savedCameraRotation;
                MainCamera.camera.transform.localEulerAngles = savedCameraRotation.eulerAngles;
            }
        }
        public Transform Camerastatesave = null;
        private void MoveCamera()
        {
            Vector2 myLookDelta = GameInput.GetLookDelta();
            /*if (myLookDelta == Vector2.zero)
            {
                myLookDelta.x -= GameInput.GetAnalogValueForButton(GameInput.Button.LookLeft);
                myLookDelta.x += GameInput.GetAnalogValueForButton(GameInput.Button.LookRight);
                myLookDelta.y += GameInput.GetAnalogValueForButton(GameInput.Button.LookUp);
                myLookDelta.y -= GameInput.GetAnalogValueForButton(GameInput.Button.LookDown);
            }*/
            mcc.rotationX += myLookDelta.x;
            mcc.rotationY += myLookDelta.y;
            mcc.rotationX = Mathf.Clamp(mcc.rotationX, -120, 120);
            mcc.rotationY = Mathf.Clamp(mcc.rotationY, -80, 80);

            MainCamera.camera.transform.localEulerAngles = new Vector3(-mcc.rotationY, mcc.rotationX, 0f);
        }

        void IPlayerListener.OnPilotBegin()
        {
            BeginFreeLook();
        }

        void IPlayerListener.OnPilotEnd()
        {
            StopFreelook();
        }

        void IPlayerListener.OnPlayerEntry()
        {
            return;
        }

        void IPlayerListener.OnPlayerExit()
        {
            return;
        }
    }
}
