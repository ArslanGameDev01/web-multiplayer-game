// https://github.com/unitycoder/webgl-js-TTS

using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

namespace UnityCoder.Samples
{
    public class TTS : MonoBehaviour
    {
        // this doesnt run in editor, but commented out just to see the code easier
#if UNITY_WEBGL //&& !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern void readTextAloud(string text, float rate, float pitch, string lang, string voiceName);
        
        [DllImport("__Internal")]
        private static extern string GetVoices();

        float pitch = 1;
        float rate = 1;

        public void Speak(string text, float rate = 1.0f, float pitch = 1.0f, string lang = "en-US", string voiceName = "")
        {
            readTextAloud(text, rate, pitch, lang, voiceName);
        }

        public void SetPitch(float pitch)
        {
            this.pitch = pitch;
        }

        public void SetRate(float rate)
        {
            this.rate = rate;
        }

        void Start()
        {
            //Speak("Hello, how are you today?", rate, pitch, "en-US");

            // doesnt work yet?
#if UNITY_WEBGL && !UNITY_EDITOR
            var voices = GetVoices();
            //Debug.Log(voices + " voice Artist");
#else
            Debug.Log("TTS is only supported in WebGL builds.");
#endif
        }

        public void ReadInputField(TMP_InputField source)
        {
            var msg = source.text;
            Debug.Log(msg);
            Speak(msg, rate, pitch,"en-US","Google UK English Female");
        }

#endif
    }
}
