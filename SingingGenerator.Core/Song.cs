using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SingingGenerator.Core
{
    public class Song
    {
        //private list to hold notes
        private List<Beat> notes;

        //properties
        public int Tempo { get; set; }

        //constructor
        public Song(int tempo = 120)
        {
            notes = new List<Beat> { };
            Tempo = tempo;
        }

        //append a note to the end of the song
        public void AppendNote(Note note)
        {
            notes.Add(note);
        }

        //append a rest to the end of the song
        public void AppendRest(int length, int notesPerBeat)
        {
            notes.Add(new Rest(length, notesPerBeat));
        }

        //remove a note from the end of the song
        public void RemoveLastNote()
        {
            if (notes.Count > 0)
            {
                notes.RemoveAt(notes.Count - 1);
            }
        }

        //get a string of phonemes and set the lyrics accordingly
        public void SetLyricsFromPhonemeArray(Phoneme[][] phonemes)
        {
            for (int i = 0, j = 0; i < phonemes.Length && j < notes.Count; ++i, ++j)
            {
                //skip rests
                while (notes[j] is Rest && j < notes.Count) { ++j; }

                if (j < notes.Count)
                {
                    (notes[j] as Note).SetPhonemes(phonemes[i]);
                }
            }
        }

        //transpose whole song
        public bool TransposeSong(int octave)
        {
            //fix for halfsteps later
            try
            {
                foreach (var note in notes)
                {
                    if (note is Note)
                    {
                        var n = note as Note;
                        n.NoteValue += octave * 12;
                    }
                }
            }
            catch (ArgumentOutOfRangeException) //too high or low
            {
                return false;
            }
            return true;
        }

        //get whole song in Apple speech synthesis script
        public string GetSpeechSynthesisScript()
        {
            string output = $"[[cmnt Tempo: {Tempo}]]\n[[inpt TUNE]]\n";
            for (int i = 0; i < notes.Count; ++i)
            {
                Beat temp = null;
                if (i + 1 < notes.Count) //get next note's data
                { 
                    temp = notes[i + 1];
                }
                output += notes[i].GetSpeechSynthesisScript(Tempo, temp) + "~\n";
            }
            output += "[[inpt TEXT]]";

            return output;
        }

        //import from an existing Apple speech synthesis script
        public void ImportFromSpeechSynthesisScript(string filepath)
        {
            using (var fileStream = new FileStream(filepath, FileMode.Open))
            {
                using (var streamReader = new StreamReader(fileStream))
                {
                    string read;
                    int tempo = 0;
                    var readNotes = new List<Beat> { };
                    double length, pitch, noteLength = 0, notePitch = 0;
                    List<Phoneme> phonemes = null;

                    while (!streamReader.EndOfStream)
                    {
                        read = streamReader.ReadLine().Trim();
                        string pattern;
                        if (tempo == 0)
                        {
                            pattern = @"^\[\[cmnt Tempo: ([\d]+)\]\]$";
                            if (Regex.IsMatch(read, pattern))
                            {
                                var groups = Regex.Match(read, pattern).Groups;
                                tempo = int.Parse(groups[1].Value);
                            }
                            else
                            {
                                throw new FormatException("Missing tempo information.");
                            }
                        }
                        else
                        {
                            pattern = @"^(\w+|%) {D ([\d]+(\.*[\d]+)?)(; P ([\d]+(\.*[\d]+)?):0)?}$";
                            if (read == "~")
                            {
                                if (phonemes != null)
                                {
                                    var note = new Note(tempo, notePitch, noteLength);
                                    note.SetPhonemes(phonemes.ToArray());
                                    readNotes.Add(note);
                                    phonemes = null;
                                }
                            }
                            else if (Regex.IsMatch(read, pattern))
                            {
                                var groups = Regex.Match(read, pattern).Groups;
                                length = double.Parse(groups[2].Value);

                                try
                                {
                                    var phon = new Phoneme(groups[1].Value);
                                    if (phon.IsPause())
                                    {
                                        readNotes.Add(new Rest(tempo, length));
                                    }
                                    else
                                    {
                                        pitch = double.Parse(groups[5].Value);
                                        if (phonemes == null)
                                        {
                                            notePitch = pitch;
                                            phonemes = new List<Phoneme> { phon };
                                            noteLength = length;
                                        }
                                        else
                                        {
                                            //to add: compare pitch to notePitch
                                            phonemes.Add(phon);
                                            noteLength += length;
                                        }
                                    }

                                }
                                catch (ArgumentException ex)
                                {
                                    throw new FormatException("File is formatted incorrectly.", ex);
                                }
                            }
                            else if (!string.IsNullOrEmpty(read) && !read.StartsWith("[[inpt", StringComparison.CurrentCulture))
                            {
                                throw new FormatException("File is formatted incorrectly.");
                            }
                        }
                    }
                    //if everything goes through, pass the loaded song to the object
                    Tempo = tempo;
                    notes = readNotes;
                }
            }
        }

        //import from a midi track
        public void ImportFromMidiTrack(Midi midi, int track)
        {
            Tempo = midi.Tempo;
            notes = midi.GetNotesFromTrack(track);
        }
    }
}
