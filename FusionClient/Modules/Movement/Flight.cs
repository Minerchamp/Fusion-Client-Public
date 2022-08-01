using FC.Utils;
using System;
using FusionClient;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Animation;
using VRC.SDKBase;
using FC;
using FusionClient.Core;

namespace FusionClient.Modules
{
    class Flight : FusionModule
    {
        private static VRCPlayer currentPlayer;
        private static bool isInVR;
        private static Transform transform;
        private static VRCMotionState motionState;
        public static bool SideWasePortalTp = true;
        public static bool SpeedHack;
        public static bool Fly;
        public static bool NoClip;

        public override void Update()
        {
            try
            {

                if (currentPlayer == null || transform == null)
                {
                    currentPlayer = PlayerUtils.GetCurrentUser();
                    isInVR = PlayerUtils.SelfIsInVR();
                    transform = Camera.main.transform;
                }

                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F) && Config.Main.KeyBinds)
                {
                    if (!Fly)
                    {
                        FlyOn();
                    }
                }

                if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.F) && Config.Main.KeyBinds)
                {
                    if (Fly)
                    {
                        FlyOff();
                    }
                }

                if (Flight.Fly)
                {
                    if (!GameObject.Find("UserInterface/MenuContent/Backdrop/Header").active)
                    {
                        if (Input.GetKeyDown(KeyCode.LeftShift))
                            Config.Main.FlightSpeed *= 2;

                        if (Input.GetKeyUp(KeyCode.LeftShift))
                            Config.Main.FlightSpeed /= 2;

                        if (Config.Main.Flight)
                        {
                            if (isInVR)
                            {
                                if (Math.Abs(Input.GetAxis("Vertical")) != 0f)
                                    currentPlayer.transform.position += currentPlayer.transform.forward *
                                                                        Config.Main.FlightSpeed * Time.deltaTime *
                                                                        Input.GetAxis("Vertical");

                                if (Math.Abs(Input.GetAxis("Horizontal")) != 0f)
                                    currentPlayer.transform.position += currentPlayer.transform.right *
                                                                        Config.Main.FlightSpeed * Time.deltaTime *
                                                                        Input.GetAxis("Horizontal");

                                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f)
                                    currentPlayer.transform.position += currentPlayer.transform.up *
                                                                        Config.Main.FlightSpeed * Time.deltaTime *
                                                                        Input.GetAxisRaw(
                                                                            "Oculus_CrossPlatform_SecondaryThumbstickVertical");
                                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f)
                                    currentPlayer.transform.position += currentPlayer.transform.up *
                                                                        Config.Main.FlightSpeed * Time.deltaTime *
                                                                        Input.GetAxisRaw(
                                                                            "Oculus_CrossPlatform_SecondaryThumbstickVertical");
                            }
                            else
                            {
                                if (Input.GetKey(KeyCode.E))
                                    currentPlayer.transform.position += currentPlayer.transform.up *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.Q))
                                    currentPlayer.transform.position += currentPlayer.transform.up * -1 *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.W))
                                    currentPlayer.transform.position += currentPlayer.transform.forward *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.A))
                                    currentPlayer.transform.position += currentPlayer.transform.right * -1f *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.D))
                                    currentPlayer.transform.position += currentPlayer.transform.right *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.S))
                                    currentPlayer.transform.position += currentPlayer.transform.forward * -1f *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;
                            }
                        }
                        else
                        {
                            if (isInVR)
                            {
                                if (Math.Abs(Input.GetAxis("Vertical")) != 0f)
                                    currentPlayer.transform.position += transform.transform.forward *
                                                                        Config.Main.FlightSpeed * Time.deltaTime *
                                                                        Input.GetAxis("Vertical");

                                if (Math.Abs(Input.GetAxis("Horizontal")) != 0f)
                                    currentPlayer.transform.position += transform.transform.right *
                                                                        Config.Main.FlightSpeed * Time.deltaTime *
                                                                        Input.GetAxis("Horizontal");

                                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f)
                                    currentPlayer.transform.position += transform.transform.up *
                                                                        Config.Main.FlightSpeed * Time.deltaTime *
                                                                        Input.GetAxisRaw(
                                                                            "Oculus_CrossPlatform_SecondaryThumbstickVertical");

                                if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f)
                                    currentPlayer.transform.position += transform.transform.up *
                                                                        Config.Main.FlightSpeed * Time.deltaTime *
                                                                        Input.GetAxisRaw(
                                                                            "Oculus_CrossPlatform_SecondaryThumbstickVertical");
                            }
                            else
                            {
                                if (Input.GetKey(KeyCode.E))
                                    currentPlayer.transform.position += transform.transform.up *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.Q))
                                    currentPlayer.transform.position += transform.transform.up * -1 *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.W))
                                    currentPlayer.transform.position += transform.transform.forward *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.A))
                                    currentPlayer.transform.position += transform.transform.right * -1f *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.D))
                                    currentPlayer.transform.position += transform.transform.right *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;

                                if (Input.GetKey(KeyCode.S))
                                    currentPlayer.transform.position += transform.transform.forward * -1f *
                                                                        Config.Main.FlightSpeed * Time.deltaTime;
                            }
                        }
                    }
                }
                if (motionState != null)
                {
                    motionState.Reset();
                    if (motionState.field_Private_CharacterController_0 != null)
                        motionState.field_Private_CharacterController_0.enabled = !NoClip;
                }
                if (Config.Main.InfJump)
                {
                    if (VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump").prop_Boolean_0 && !Networking.LocalPlayer.IsPlayerGrounded())
                    {
                        var GetJump = Networking.LocalPlayer.GetVelocity();
                        GetJump.y = Networking.LocalPlayer.GetJumpImpulse();
                        Networking.LocalPlayer.SetVelocity(GetJump);
                    }
                }
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T) && Config.Main.MouseTP)
                {
                    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                    RaycastHit raycastHit;
                    if (Physics.Raycast(ray, out raycastHit))
                    {
                        PlayerUtils.GetCurrentUser().transform.position = raycastHit.point;
                    }
                }
                if (Config.Main.KeyBinds && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Mouse2) && SideWasePortalTp)
                {
                    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                    RaycastHit raycastHit;
                    if (Physics.Raycast(ray, out raycastHit))
                    {
                        MiscUtils.PortalPosition(raycastHit.point, new Quaternion(4f, 4f, 0f, 1f));
                    }
                }
                if (SpeedHack)
                {
                    if (isInVR)
                    {
                        if (Math.Abs(Input.GetAxis("Vertical")) != 0f)
                            currentPlayer.transform.position += transform.transform.forward *
                                                                Config.Main.SpeedHack * Time.deltaTime *
                                                                Input.GetAxis("Vertical");

                        if (Math.Abs(Input.GetAxis("Horizontal")) != 0f)
                            currentPlayer.transform.position += transform.transform.right *
                                                                Config.Main.SpeedHack * Time.deltaTime *
                                                                Input.GetAxis("Horizontal");
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.W))
                            currentPlayer.transform.position += currentPlayer.transform.forward *
                                                                Config.Main.SpeedHack * Time.deltaTime;

                        if (Input.GetKey(KeyCode.A))
                            currentPlayer.transform.position += currentPlayer.transform.right * -1f *
                                                                Config.Main.SpeedHack * Time.deltaTime;

                        if (Input.GetKey(KeyCode.D))
                            currentPlayer.transform.position += currentPlayer.transform.right *
                                                                Config.Main.SpeedHack * Time.deltaTime;

                        if (Input.GetKey(KeyCode.S))
                            currentPlayer.transform.position += currentPlayer.transform.forward * -1f *
                                                                Config.Main.SpeedHack * Time.deltaTime;
                    }
                }

                if (Config.Main.BunnyHop && Input.GetKey(KeyCode.Space))
                {
                    VRCInput vrcinput = VRCInputManager.Method_Public_Static_VRCInput_String_0("Jump".ToLower());
                    if (Networking.LocalPlayer.IsPlayerGrounded())
                    {
                        Vector3 velocity = PlayerUtils.GetCurrentUser().GetVRCPlayerApi().GetVelocity();
                        velocity.y = PlayerUtils.GetCurrentUser().GetVRCPlayerApi().GetJumpImpulse();
                        PlayerUtils.GetCurrentUser().GetVRCPlayerApi().SetVelocity(velocity);
                    }
                    else if (vrcinput.Method_Public_Boolean_0() && Networking.LocalPlayer.IsPlayerGrounded())
                    {
                        Vector3 velocity2 = PlayerUtils.GetCurrentUser().GetVRCPlayerApi().GetVelocity();
                        velocity2.y = PlayerUtils.GetCurrentUser().GetVRCPlayerApi().GetJumpImpulse();
                        PlayerUtils.GetCurrentUser().GetVRCPlayerApi().SetVelocity(velocity2);
                    }
                }
            }
            catch
            {
            }
        }

        public static void FlyOn()
        {
            //Gravity = Physics.gravity;
            Flight.Fly = true;
            Physics.gravity = Vector3.zero;
            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
        }

        public static void FlyOff()
        {
            Flight.Fly = false;
            Physics.gravity = new Vector3(0.0f, -9.8f, 0.0f);
            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
}