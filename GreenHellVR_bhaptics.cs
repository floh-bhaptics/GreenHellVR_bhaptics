using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Enums;
using JetBrains.Annotations;

[assembly: MelonInfo(typeof(GreenHellVR_bhaptics.GreenHellVR_bhaptics), "GreenHellVR_bhaptics", "1.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("Incuvo", "GHVR")]

namespace GreenHellVR_bhaptics
{
    public class GreenHellVR_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        [HarmonyPatch(typeof(Player), "Die", new Type[] { typeof(DeathController.DeathType) })]
        public class bhaptics_PlayerDie
        {
            [HarmonyPostfix]
            public static void Postfix(Player __instance, DeathController.DeathType type)
            {
                tactsuitVr.StopThreads();
            }
        }

        #region Damage

        private static (float, float) getAngleAndShift(Transform player, Vector3 hitPoint)
        {
            Vector3 patternOrigin = new Vector3(0f, 0f, 1f);
            // y is "up", z is "forward" in local coordinates
            Vector3 hitPosition = hitPoint - player.position;
            Quaternion PlayerRotation = player.rotation;
            Vector3 playerDir = PlayerRotation.eulerAngles;
            // We only want rotation correction in y direction (left-right), top-bottom and yaw we can leave
            Vector3 flattenedHit = new Vector3(hitPosition.x, 0f, hitPosition.z);
            float earlyhitAngle = Vector3.Angle(flattenedHit, patternOrigin);
            Vector3 earlycrossProduct = Vector3.Cross(flattenedHit, patternOrigin);
            if (earlycrossProduct.y > 0f) { earlyhitAngle *= -1f; }
            float myRotation = earlyhitAngle - playerDir.y;
            myRotation *= -1f;
            if (myRotation < 0f) { myRotation = 360f + myRotation; }

            float hitShift = hitPosition.y;
            //tactsuitVr.LOG("Hitshift: " + hitShift.ToString());
            float upperBound = 1.7f;
            float lowerBound = 1.2f;
            if (hitShift > upperBound) { hitShift = 0.5f; }
            else if (hitShift < lowerBound) { hitShift = -0.5f; }
            // ...and then spread/shift it to [-0.5, 0.5], which is how bhaptics expects it
            else { hitShift = (hitShift - lowerBound) / (upperBound - lowerBound) - 0.5f; }

            return (myRotation, hitShift);
        }


        [HarmonyPatch(typeof(Player), "TakeDamage", new Type[] { typeof(DamageInfo) })]
        public class bhaptics_PlayerDamage
        {
            [HarmonyPostfix]
            public static void Postfix(Player __instance, DamageInfo info)
            {
                string pattern = "";
                if (info.m_FromInjury)
                {
                    pattern = "Injury";
                    switch (info.m_InjuryPlace)
                    {
                        case InjuryPlace.LHand:
                            pattern += "Hand_L";
                            break;
                        case InjuryPlace.RHand:
                            pattern += "Hand_R";
                            break;
                        case InjuryPlace.LLeg:
                            pattern += "Leg_L";
                            break;
                        case InjuryPlace.RLeg:
                            pattern += "Leg_R";
                            break;
                        default:
                            pattern += "General";
                            break;
                    }
                    tactsuitVr.PlaybackHaptics(pattern);
                    return;
                }

                switch (info.m_DamageType)
                {
                    case DamageType.Fall:
                        pattern = "FallDamage";
                        tactsuitVr.PlaybackHaptics(pattern);
                        return;
                    case DamageType.VenomPoison:
                        pattern = "Poison";
                        tactsuitVr.PlaybackHaptics(pattern);
                        return;
                    case DamageType.Insects:
                        pattern = "Insects";
                        tactsuitVr.PlaybackHaptics(pattern);
                        return;
                    case DamageType.SnakePoison:
                        pattern = "Poison";
                        tactsuitVr.PlaybackHaptics(pattern);
                        return;
                    case DamageType.Infection:
                        pattern = "Poison";
                        tactsuitVr.PlaybackHaptics(pattern);
                        return;
                    case DamageType.Fire:
                        pattern = "Fire";
                        tactsuitVr.PlaybackHaptics(pattern);
                        return;
                    case DamageType.Thrust:
                        pattern = "Spear";
                        break;
                    case DamageType.Melee:
                        pattern = "Slash";
                        break;
                    case DamageType.Claws:
                        pattern = "Slash";
                        break;
                    case DamageType.Critical:
                        pattern = "Impact";
                        break;
                    case DamageType.StoneCollision:
                        pattern = "Impact";
                        break;
                    default:
                        pattern = "Impact";
                        break;
                }
                float hitAngle;
                float hitShift;
                (hitAngle, hitShift) = getAngleAndShift(__instance.transform, info.m_HitDir);
                if (hitShift >= 0.5f) { tactsuitVr.HeadShot(hitAngle); return; }
                tactsuitVr.PlayBackHit(pattern, hitAngle, hitShift);
            }
        }
        #endregion



    }
}
