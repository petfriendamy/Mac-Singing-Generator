using System;
namespace SingingGenerator.Core
{
    public abstract class Beat
    {
        public int NoteLength { get; protected set; }
        public int NotesPerBeat { get; protected set; }

        //constructor using notes per beat
        protected Beat(int length, int notesPerBeat)
        {
            NoteLength = length;
            NotesPerBeat = notesPerBeat;
        }

        //constructor using milliseconds
        protected Beat(int tempo, double milliseconds)
        {
            SetLengthFromMilliseconds(tempo, milliseconds);
        }

        //sets length
        public void SetLength(int length, int notesPerBeat)
        {
            NoteLength = length;
            NotesPerBeat = notesPerBeat;

            while (NoteLength % 2 == 0 && NotesPerBeat % 2 == 0) //simplify
            {
                NoteLength /= 2;
                NotesPerBeat /= 2;
            }
        }

        //returns the length of the note relative to the tempo
        public double GetLengthInMilliseconds(int tempo)
        {
            //https://hubpages.com/entertainment/Tempo-to-Millisecond-Delay-Interval-Calculator
            return (60000.0 * NoteLength) / (NotesPerBeat * tempo);
        }

        //sets the length from a millisecond value
        public void SetLengthFromMilliseconds(int tempo, double milliseconds)
        {
            //assume beat is 1/16 until proven otherwise
            int notesPerBeat = 4;
            double sixteenth = 15000.0 / tempo,
                length = Math.Round(milliseconds / sixteenth);

            while (length < 1) //if beat is shorter than 1/16
            {
                notesPerBeat *= 2;
                length *= 2;
            }
            SetLength((int)length, notesPerBeat);
        }

        //get note in Apple speech synthesis script
        public abstract string GetSpeechSynthesisScript(int tempo, Beat next = null);
    }
}
