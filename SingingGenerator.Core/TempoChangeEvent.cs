using System;
namespace SingingGenerator.Core
{
    public class TempoChangeEvent : SongEvent
    {
        public int Tempo { get; set; }

        public TempoChangeEvent(int tempo)
        {
            Tempo = tempo;
        }

        public string GetSpeechSynthesisScript()
        {
            return $"[[cmnt Tempo: {Tempo}]]\n";
        }
    }
}
