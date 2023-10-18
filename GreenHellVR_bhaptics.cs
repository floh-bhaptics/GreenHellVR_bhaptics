using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;


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
        public class bhaptics_CastSpell
        {
            [HarmonyPostfix]
            public static void Postfix(Player __instance, DeathController.DeathType type)
            {
                tactsuitVr.StopThreads();
            }
        }

    }
}
