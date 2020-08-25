using System;
using System.Collections.Generic;

using Foundation;
using AppKit;

using SingingGenerator.Core;

namespace MacSingingGenerator
{
    public static class SpeechSynthesizer
    {
        private static NSSpeechSynthesizer speech = new NSSpeechSynthesizer();
        private static SystemVoice currentVoice;

        public static SystemVoice[] SystemVoices { get; } = GetSystemVoices();
        public static SystemVoice CurrentVoice
        {
            get { return currentVoice; }
            private set
            {
                currentVoice = value;
                speech.Voice = value.Identifier;
            }
        }
        public static bool IsSpeaking
        {
            get { return speech.IsSpeaking; }
        }

        private static SystemVoice[] GetSystemVoices()
        {
            var temp = new List<SystemVoice> { };
            foreach (var voice in NSSpeechSynthesizer.AvailableVoices)
            {
                var v = new SystemVoice(voice);
                if (v.LocaleIdentifier == "en_US" && !v.ConstantRateOnly)
                {
                    if (v.Name != "Deranged" && v.Name != "Whisper" && v.Name != "Samantha" && v.Name != "Tom") //other voices that don't work
                    {
                        temp.Add(v);
                    }
                }
            }
            return temp.ToArray();
        }

        public static void StartSpeaking(string text, SystemVoice voice)
        {
            speech.Voice = voice.Identifier;
            speech.StartSpeakingString(text);
        }

        public static bool SaveToFile(string text, SystemVoice voice, NSUrl url)
        {
            speech.Voice = voice.Identifier;
            return speech.StartSpeakingStringtoURL(text, url);
        }

        public static void StopSpeaking()
        {
            speech.StopSpeaking();
        }

        public static Phoneme[][] GetPhonemesFromText(string text)
        {
            try
            {
                speech.Voice = SystemVoices[0].Identifier;
                return Phoneme.GetPhonemesFromString(speech.PhonemesFromText(text));
            }
            catch (FormatException ex)
            {
                throw ex;
            }
        }
    }
}
