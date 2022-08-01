using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FusionClient;
using UnhollowerBaseLib;
using UnityEngine;
using VRC.Core;
using FusionClient.Utils.ComponentManager;
using FusionClient.Core;
using FusionClient.Utils.Manager;

namespace FusionClient.Modules.AntiCrash
{
    class AntiCrash
    {
        internal static void CleanAvatar(VRCAvatarManager avatarManager, GameObject obj)
        {
            int LimitedTransforms = 0;
            int LimitedRigidbodies = 0;
            int RemovedColliders = 0;
            int RemovedAudioSources = 0;
            int LimitedJoints = 0;
            int LimitedAudioSources = 0;


            if (Config.Main.ProcessTransforms)
            {
                List<Transform> transforms = ComponentManager.FindAllComponentsInGameObject<Transform>(obj);

                for (int i = 0; i < transforms.Count; i++)
                {
                    if (transforms[i] == null) continue;
                    if (ProcessTransforms(transforms[i]))
                    {
                        LimitedTransforms++;
                    }
                }
            }

            if (Config.Main.ProcessRigidBodies)
            {
                List<Rigidbody> rigidbodies = ComponentManager.FindAllComponentsInGameObject<Rigidbody>(obj);

                for (int i = 0; i < rigidbodies.Count; i++)
                {
                    if (rigidbodies[i] == null) continue;
                    if (ProcessRigidBodies(rigidbodies[i]))
                    {
                        LimitedRigidbodies++;
                    }
                }
            }

            if (Config.Main.ProcessColliders)
            {
                List<Collider> colliders = ComponentManager.FindAllComponentsInGameObject<Collider>(obj);

                for (int i = 0; i < colliders.Count; i++)
                {
                    if (colliders[i] == null) continue;
                    if (ProcessColliders(colliders[i]))
                    {
                        RemovedColliders++;
                    }
                }
            }

            if (Config.Main.ProcessJoints)
            {
                List<Joint> joints = ComponentManager.FindAllComponentsInGameObject<Joint>(obj);

                for (int i = 0; i < joints.Count; i++)
                {
                    if (joints[i] == null) continue;
                    if (ProcessJoints(joints[i]))
                    {
                        LimitedJoints++;
                    }
                }
            }

            if (Config.Main.ProcessJoints)
            {
                List<AudioSource> audioSources = ComponentManager.FindAllComponentsInGameObject<AudioSource>(obj);
                int tmpASCount = 0;
                var tmpUSSource = avatarManager.field_Private_VRCPlayer_0.transform.Find("AnimationController/HeadAndHandIK/HandEffector/USpeak").GetComponent<AudioSource>();

                for (int i = 0; i < audioSources.Count; i++)
                {
                    if (audioSources[i] == null) continue;

                    tmpASCount++;
                    if (tmpASCount > Config.Main.MaxAudioSources)
                    {
                        UnityEngine.Object.DestroyImmediate(audioSources[i], true);
                        RemovedAudioSources++;
                    }
                    if (audioSources[i].name.ToLower().Contains("uspeak"))
                    {
                        if (audioSources[i] != tmpUSSource)
                        {
                            UnityEngine.Object.DestroyImmediate(audioSources[i], true);
                            RemovedAudioSources++;
                        }
                    }
                    if (audioSources[i].clip == null)
                    {
                        UnityEngine.Object.DestroyImmediate(audioSources[i], true);
                        RemovedAudioSources++;
                    }
                    if (ProcessAudioSource(audioSources[i], tmpUSSource))
                    {
                        LimitedAudioSources++;
                    }

                }
            }
        }

        private static bool ProcessTransforms(Transform transform)
        {
            bool limitedTransform = false;

            #region Rotation Safety

            Quaternion newLocalRotation = transform.localRotation;

            if (CrashManager.IsInvalid(newLocalRotation))
            {
                newLocalRotation = Quaternion.identity;
                limitedTransform = true;
            }
            else
            {
                newLocalRotation.w = CrashManager.Clamp(newLocalRotation.w, -Config.Main.MaxTransformRotation, Config.Main.MaxTransformRotation);
                newLocalRotation.x = CrashManager.Clamp(newLocalRotation.x, -Config.Main.MaxTransformRotation, Config.Main.MaxTransformRotation);
                newLocalRotation.y = CrashManager.Clamp(newLocalRotation.y, -Config.Main.MaxTransformRotation, Config.Main.MaxTransformRotation);
                newLocalRotation.z = CrashManager.Clamp(newLocalRotation.z, -Config.Main.MaxTransformRotation, Config.Main.MaxTransformRotation);

                if (newLocalRotation != transform.localRotation)
                {
                    limitedTransform = true;
                }
            }

            transform.localRotation = newLocalRotation;
            #endregion

            #region Scale Safety

            Vector3 newLocalScale = transform.localScale;

            if (CrashManager.IsInvalid(newLocalScale))
            {
                newLocalScale = Vector3.one;
                limitedTransform = true;
            }
            else
            {
                newLocalScale.x = CrashManager.Clamp(newLocalScale.x, -Config.Main.MaxTransformScale, Config.Main.MaxTransformScale);
                newLocalScale.y = CrashManager.Clamp(newLocalScale.y, -Config.Main.MaxTransformScale, Config.Main.MaxTransformScale);
                newLocalScale.z = CrashManager.Clamp(newLocalScale.z, -Config.Main.MaxTransformScale, Config.Main.MaxTransformScale);

                if (newLocalScale != transform.localScale)
                {
                    limitedTransform = true;
                }
            }

            transform.localScale = newLocalScale;

            #endregion

            return limitedTransform;
        }
        private static bool ProcessRigidBodies(Rigidbody rigidbody)
        {
            bool result = false;

            float newMass = rigidbody.mass;
            rigidbody.mass = CrashManager.Clamp(rigidbody.mass, -10000f, 10000f);
            if (rigidbody.mass != newMass) result = true;
            rigidbody.mass = newMass;

            float newMaxAngularVelocity = rigidbody.maxAngularVelocity;
            rigidbody.maxAngularVelocity = CrashManager.Clamp(rigidbody.maxAngularVelocity, -100f, 100f);
            newMaxAngularVelocity = rigidbody.maxAngularVelocity;
            if (rigidbody.maxAngularVelocity != newMaxAngularVelocity) result = true;
            rigidbody.maxAngularVelocity = newMaxAngularVelocity;

            float newMaxDepenetrationVelocity = rigidbody.maxDepenetrationVelocity;
            rigidbody.maxDepenetrationVelocity = CrashManager.Clamp(rigidbody.maxDepenetrationVelocity, -100f, 100f);
            if (rigidbody.maxDepenetrationVelocity != newMaxDepenetrationVelocity) result = true;
            rigidbody.maxDepenetrationVelocity = newMaxDepenetrationVelocity;

            return result;
        }
        private static bool ProcessColliders(Collider collider)
        {
            #region Bounds Safety

            if ((collider.bounds.center.x < 100f && collider.bounds.center.x > 100f) ||
                            (collider.bounds.center.y < 100f && collider.bounds.center.y > 100f) ||
                            (collider.bounds.center.z < 100f && collider.bounds.center.z > 100f) ||
                            (collider.bounds.extents.x < 100f && collider.bounds.extents.x > 100f) ||
                            (collider.bounds.extents.y < 100f && collider.bounds.extents.y > 100f) ||
                            (collider.bounds.extents.z < 100f && collider.bounds.extents.z > 100f))
            {
                UnityEngine.Object.DestroyImmediate(collider, true);
                return true;
            }

            #endregion

            #region Collider Safety

            if (collider is BoxCollider boxCollider)
            {
                #region Center Safety

                Vector3 newCenter = boxCollider.center;

                newCenter.x = CrashManager.Clamp(newCenter.x, -100f, 100f);
                newCenter.y = CrashManager.Clamp(newCenter.y, -100f, 100f);
                newCenter.z = CrashManager.Clamp(newCenter.z, -100f, 100f);

                boxCollider.center = newCenter;

                #endregion

                #region Extents Safety

                Vector3 newExtents = boxCollider.extents;

                newExtents.x = CrashManager.Clamp(newExtents.x, -100f, 100f);
                newExtents.y = CrashManager.Clamp(newExtents.y, -100f, 100f);
                newExtents.z = CrashManager.Clamp(newExtents.z, -100f, 100f);

                boxCollider.center = newExtents;

                #endregion

                #region Size Safety

                Vector3 newSize = boxCollider.size;

                newSize.x = CrashManager.Clamp(newSize.x, -100f, 100f);
                newSize.y = CrashManager.Clamp(newSize.y, -100f, 100f);
                newSize.z = CrashManager.Clamp(newSize.z, -100f, 100f);

                boxCollider.size = newSize;

                #endregion
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                capsuleCollider.radius = CrashManager.Clamp(capsuleCollider.radius, -25f, 25f);
                capsuleCollider.height = CrashManager.Clamp(capsuleCollider.height, -25f, 25f);

                #region Center Safety

                Vector3 newCenter = capsuleCollider.center;

                newCenter.x = CrashManager.Clamp(newCenter.x, -100f, 100f);
                newCenter.y = CrashManager.Clamp(newCenter.y, -100f, 100f);
                newCenter.z = CrashManager.Clamp(newCenter.z, -100f, 100f);

                capsuleCollider.center = newCenter;

                #endregion
            }
            else if (collider is SphereCollider sphereCollider)
            {
                sphereCollider.radius = CrashManager.Clamp(sphereCollider.radius, -25f, 25f);

                #region Center Safety

                Vector3 newCenter = sphereCollider.center;

                newCenter.x = CrashManager.Clamp(newCenter.x, -100f, 100f);
                newCenter.y = CrashManager.Clamp(newCenter.y, -100f, 100f);
                newCenter.z = CrashManager.Clamp(newCenter.z, -100f, 100f);

                sphereCollider.center = newCenter;

                #endregion
            }

            #endregion

            return false;
        }
        private static bool ProcessJoints(Joint joint)
        {
            bool limitedJoint = false;

            #region Connected Mass Scale Safety
            float newConnectedMassScale = joint.connectedMassScale;
            newConnectedMassScale = CrashManager.Clamp(newConnectedMassScale, -25f, 25f);
            if (joint.connectedMassScale != newConnectedMassScale) limitedJoint = true;
            joint.connectedMassScale = newConnectedMassScale;
            #endregion

            #region Mass Scale Safety
            float newMassScale = joint.massScale;
            newMassScale = CrashManager.Clamp(newMassScale, -25f, 25);
            if (joint.massScale != newMassScale) limitedJoint = true;
            joint.massScale = newMassScale;
            #endregion

            #region Break Torque Safety
            float newBreakTorque = joint.breakTorque;
            newBreakTorque = CrashManager.Clamp(newBreakTorque, -100f, 100f);
            if (joint.breakTorque != newBreakTorque) limitedJoint = true;
            joint.breakTorque = newBreakTorque;
            #endregion

            #region Break Force Safety
            float newBreakForce = joint.breakForce;
            newBreakForce = CrashManager.Clamp(newBreakForce, -100f, 100f);
            if (joint.breakForce != newBreakForce) limitedJoint = true;
            joint.breakForce = newBreakForce;
            #endregion

            if (joint is SpringJoint springJoint)
            {
                #region Damper Safety
                float newDamper = springJoint.damper;
                newDamper = CrashManager.Clamp(newDamper, -100f, 100f);
                if (springJoint.damper != newDamper) limitedJoint = true;
                springJoint.damper = newDamper;
                #endregion

                #region Max Distance Safety
                float newMaxDistance = springJoint.maxDistance;
                newMaxDistance = CrashManager.Clamp(newMaxDistance, -100f, 100f);
                if (springJoint.maxDistance != newMaxDistance) limitedJoint = true;
                springJoint.maxDistance = newMaxDistance;
                #endregion

                #region Min Distance Safety
                float newMinDistance = springJoint.minDistance;
                newMinDistance = CrashManager.Clamp(newMinDistance, -100f, 100f);
                if (springJoint.minDistance != newMinDistance) limitedJoint = true;
                springJoint.minDistance = newMinDistance;
                #endregion

                #region Spring Safety
                float newSpring = springJoint.spring;
                newSpring = CrashManager.Clamp(newSpring, -100f, 100f);
                if (springJoint.spring != newSpring) limitedJoint = true;
                springJoint.spring = newSpring;
                #endregion

                #region Tolerance Safety
                float newTolerance = springJoint.tolerance;
                newTolerance = CrashManager.Clamp(newTolerance, -100f, 100f);
                if (springJoint.tolerance != newTolerance) limitedJoint = true;
                springJoint.tolerance = newTolerance;
                #endregion

                #region Anchor Safety
                Vector3 newAnchor = springJoint.anchor;
                newAnchor.x = CrashManager.Clamp(newAnchor.x, -100f, 100f);
                newAnchor.y = CrashManager.Clamp(newAnchor.y, -100f, 100f);
                newAnchor.z = CrashManager.Clamp(newAnchor.z, -100f, 100f);
                if (springJoint.anchor != newAnchor) limitedJoint = true;
                springJoint.anchor = newAnchor;
                #endregion

                #region Connected Anchor Safety
                Vector3 newConnectedAnchor = springJoint.connectedAnchor;
                newConnectedAnchor.x = CrashManager.Clamp(newConnectedAnchor.x, -100f, 100f);
                newConnectedAnchor.y = CrashManager.Clamp(newConnectedAnchor.y, -100f, 100f);
                newConnectedAnchor.z = CrashManager.Clamp(newConnectedAnchor.z, -100f, 100f);
                if (springJoint.connectedAnchor != newConnectedAnchor) limitedJoint = true;
                springJoint.connectedAnchor = newConnectedAnchor;
                #endregion
            }

            return limitedJoint;
        }
        private static bool ProcessAudioSource(AudioSource audioSource, AudioSource realUspeakAudioSource)
        {
            bool result = false;

            if (audioSource.outputAudioMixerGroup != null) // vc clapping
            {
                audioSource.outputAudioMixerGroup = null;
                result = true;
            }

            if (Config.Main.AntiWorldAudio)
            {

            }

            return result;
        }
    }
}
