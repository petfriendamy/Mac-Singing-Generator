using System;
using System.Collections.Generic;
namespace SingingGenerator.Core
{
    public class Note : Beat
    {
        //private members
        private char noteLetter;
        private List<Phoneme> phonemes = new List<Phoneme> { };
        int octave;
        private int noteValue;

        //basing frequency of note on A4 at 440 Hz
        //http://pages.mtu.edu/~suits/NoteFreqCalcs.html
        static readonly double BASE_FREQUENCY = 440.0, OFFSET = 1.059463;

        //the note's value from its offset from A4
        public int NoteValue
        {
            get { return noteValue; }
            set
            {
                if (value < -48)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Note is too low.");
                }
                if (value > 39)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Note is too high.");
                }
                noteValue = value;
            }
        }

        //constructor
        public Note(int offset, int length, int notesPerBeat) :base(length, notesPerBeat)
        {
            NoteValue = offset;
        }

        //constructor from existing script
        public Note(int tempo, double pitch, double length) :base (tempo, length)
        {
            NoteValue = GetNoteFromFrequency(pitch);
        }

        //returns an array of the associated phonemes (or "la" if none exist)
        public Phoneme[] GetPhonemes()
        {
            if (phonemes.Count == 0)
            {
                return new Phoneme[2] { new Phoneme("l"), new Phoneme("AA") };
            }
            return phonemes.ToArray();
        }

        //set the phonemes for the note
        public void SetPhonemes(Phoneme[] phonemes)
        {
            this.phonemes = new List<Phoneme> { };
            foreach (var phoneme in phonemes)
            {
                this.phonemes.Add(phoneme);
            }
        }

        //gets the note from a letter value
        public static int GetNoteFromLetter(char letter, bool sharp, int octave)
        {
            if (octave < 0 || octave > 8)
            {
                throw new ArgumentException("Invalid octave.");
            }
            letter = letter.ToString().ToUpper()[0]; //correct case
            int offset;
            switch (letter)
            {
                case 'C':
                    offset = -9;
                    break;
                case 'D':
                    offset = -7;
                    break;
                case 'E':
                    offset = -5;
                    break;
                case 'F':
                    offset = -4;
                    break;
                case 'G':
                    offset = -2;
                    break;
                case 'A':
                    offset = 0;
                    break;
                case 'B':
                    offset = 2;
                    break;
                default:
                    throw new ArgumentException("Invalid note.");
            }
            if (sharp) { offset++; }
            offset += (octave - 4) * 12;
            return offset;
        }

        //gets the frequency of a note
        public static double GetFrequencyFromNote(int note)
        {
            return BASE_FREQUENCY * Math.Pow(OFFSET, note);
        }

        //gets own note's frequency
        public double GetFrequency()
        {
            return GetFrequencyFromNote(NoteValue);
        }

        //gets note value based on the frequency
        public static int GetNoteFromFrequency(double frequency)
        {
            return (int)Math.Round(Math.Log(frequency / BASE_FREQUENCY) / Math.Log(OFFSET));
        }

        //sets note from a frequency value
        public void SetNoteFromFrequency(double frequency)
        {
            NoteValue = GetNoteFromFrequency(frequency);
        }

        //get note in Apple speech synthesis script
        public override string GetSpeechSynthesisScript(int tempo, Beat next = null)
        {
            var phons = new List<Phoneme>(GetPhonemes());
            if (phons[phons.Count - 1].IsVowel())
            {
                if (next != null && next is Note)
                {
                    phons.Add((next as Note).GetPhonemes()[0]);
                }
                else
                {
                    phons.Add(new Phoneme("h"));
                }
            }

            string frequency = $"{GetFrequency():F5}",
                length = (GetLengthInMilliseconds(tempo) - ((phons.Count - 1) * 15)).ToString();

            string output = "";
            foreach (var phon in phons)
            {
                output += phon.Value + " {D ";
                if (phon.IsVowel()) { output += length; }
                else { output += "15"; }
                output += "; P " + frequency + ":0}\n";
            }
            return output;
        }
    }
}
