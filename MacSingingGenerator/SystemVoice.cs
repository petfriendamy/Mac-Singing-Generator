using System;

using Foundation;
using AppKit;

namespace MacSingingGenerator
{
    public class SystemVoice
    {
        public string Identifier { get; }

        private NSDictionary Attributes
        {
            get { return NSSpeechSynthesizer.AttributesForVoice(Identifier); }
        }

        public string Name
        {
            get
            {
                return Attributes.ValueForKey(new NSString("VoiceName")).ToString();
            }
        }

        public string LocaleIdentifier
        {
            get
            {
                return Attributes.ValueForKey(new NSString("VoiceLocaleIdentifier")).ToString();
            }
        }

        public bool ConstantRateOnly
        {
            get
            {
                NSObject temp;
                return Attributes.TryGetValue(new NSString("VoiceConstantRateOnly"), out temp);
            }
        }

        public SystemVoice(string identifier)
        {
            Identifier = identifier;
        }
    }
}
