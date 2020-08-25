using System;
namespace SingingGenerator.Core
{
    public class Rest : Beat
    {
        public Rest(int length, int notePerBeat) : base(length, notePerBeat) { }
        public Rest(int tempo, double length) : base(tempo, length) { }

        //get rest in Apple speech synthesis script
        public override string GetSpeechSynthesisScript(int tempo, Beat next = null)
        {
            return "% {D " + GetLengthInMilliseconds(tempo).ToString() + "}\n";
        }
    }
}
