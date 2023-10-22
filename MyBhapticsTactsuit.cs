using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Bhaptics.SDK2;
using MelonLoader;

namespace MyBhapticsTactsuit
{
    public class TactsuitVR
    {
        private static ManualResetEvent HeartBeat_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Water_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Necktingle_mrse = new ManualResetEvent(false);
        private static ManualResetEvent Rain_mrse = new ManualResetEvent(false);


        public void HeartBeatFunc()
        {
            while (true)
            {
                HeartBeat_mrse.WaitOne();
                Thread.Sleep(1000);
                BhapticsSDK2.Play("heartbeat");
            }
        }
        public void WaterFunc()
        {
            while (true)
            {
                Water_mrse.WaitOne();
                Thread.Sleep(5050);
                BhapticsSDK2.Play("waterslushing");
            }
        }

        public void NecktingleFunc()
        {
            while (true)
            {
                Water_mrse.WaitOne();
                Thread.Sleep(2050);
                BhapticsSDK2.Play("necktingleshort");
            }
        }

        public void RainFunc()
        {
            while (true)
            {
                Water_mrse.WaitOne();
                Thread.Sleep(3050);
                BhapticsSDK2.Play("raining");
            }
        }

        public TactsuitVR()
        {
            LOG("Starting HeartBeat, Water, Rain, and NeckTingle thread...");
            var res = BhapticsSDK2.Initialize("0fV9Kade5nuBbn40uHhr", "SYu6gMVrlO6gcr988Wnz", "");

            if (res > 0)
            {
                LOG("Failed to do bhaptics initialization...");
            }
            
            Thread HeartBeatThread = new Thread(HeartBeatFunc);
            HeartBeatThread.Start();
            Thread WaterThread = new Thread(WaterFunc);
            WaterThread.Start();
            Thread NecktingleThread = new Thread(NecktingleFunc);
            NecktingleThread.Start();
            Thread RainThread = new Thread(RainFunc);
            RainThread.Start();
        }

        public void LOG(string logStr)
        {
            MelonLogger.Msg(logStr);
        }

        public void PlaybackHaptics(String key, float intensity = 1.0f, float duration = 1.0f, float xzAngle = 0f, float yShift = 0f)
        {
            BhapticsSDK2.Play(key.ToLower(), intensity, duration, xzAngle, yShift);
            // LOG("Playing back: " + key);
        }

        public void PlayBackHit(String key, float xzAngle, float yShift)
        {
            // two parameters can be given to the pattern to move it on the vest:
            // 1. An angle in degrees [0, 360] to turn the pattern to the left
            // 2. A shift [-0.5, 0.5] in y-direction (up and down) to move it up or down
            PlaybackHaptics(key.ToLower(), 1f, 1f, xzAngle, yShift);
        }

        public void Recoil(bool isRightHand, float intensity = 1.0f)
        {
            float duration = 1.0f;
            
            string postfix = "_L";
            if (isRightHand) postfix = "_R";
            string keyArm = "RecoilBlade" + postfix;
            string keyVest = "RecoilBladeVest" + postfix;
            
            PlaybackHaptics(keyArm, intensity, duration);
            PlaybackHaptics(keyVest, intensity, duration);
        }

        public void StartHeartBeat()
        {
            HeartBeat_mrse.Set();
        }

        public void StopHeartBeat()
        {
            HeartBeat_mrse.Reset();
        }

        public void StartWater()
        {
            Water_mrse.Set();
        }

        public void StopWater()
        {
            Water_mrse.Reset();
            BhapticsSDK2.Stop("waterslushing");
        }

        public void StartNecktingle()
        {
            Necktingle_mrse.Set();
        }

        public void StopNecktingle()
        {
            Necktingle_mrse.Reset();
            BhapticsSDK2.Stop("necktingleshort");
        }

        public void StartRain()
        {
            Rain_mrse.Set();
        }

        public void StopRain()
        {
            Rain_mrse.Reset();
            BhapticsSDK2.Stop("raining");
        }

        public bool IsPlaying(String effect)
        {
            return BhapticsSDK2.IsPlaying(effect.ToLower());
        }

        public void StopHapticFeedback(String effect)
        {
            BhapticsSDK2.Stop(effect.ToLower());
        }

        public void StopAllHapticFeedback()
        {
            StopThreads();
            BhapticsSDK2.StopAll();
        }

        public void StopThreads()
        {
            StopHeartBeat();
            StopWater();
            StopNecktingle();
            StopRain();
        }


    }
}
