using System;
using System.Collections.Generic;
namespace SingingGenerator.Core
{
    public class Phoneme
    {
        //validators
        static private readonly string[] VOWELS =
        {
            "AE", "EY", "AO", "AX", "IY", "EH", "IH", "AY", "IX", "AA",
            "UW", "UH", "UX", "OW", "AW", "OY"
        };
        static private readonly char[] CONSONANTS =
        {
            'b', 'C', 'D', 'd', 'f', 'g', 'h', 'J', 'k', 'l', 'm', 'N',
            'n', 'p', 'r', 's', 'S', 'T', 't', 'v', 'w', 'y', 'Z', 'z'
        };

        public string Value { get; private set; }

        public Phoneme(string value)
        {
            if (IsVowel(value))
            {
                Value = value.ToUpper();
            }
            else if (IsConsonant(value) || IsPause(value))
            {
                Value = value;
            }
            else
            {
                throw new ArgumentException($"Invalid phoneme: {value}");
            }
        }

        //check for valid vowels
        public static bool IsVowel(string value)
        {
            foreach (var vowel in VOWELS) //check if string is a valid vowel
            {
                if (value.ToUpper() == vowel) { return true; }
            }
            return false;
        }

        public bool IsVowel()
        {
            return IsVowel(Value);
        }

        //check for valid consonants
        public static bool IsConsonant(string value)
        {
            if (value.Length > 0)
            {
                foreach (var consonant in CONSONANTS)
                {
                    if (value[0] == consonant) //check if valid as it is
                    {
                        return true;
                    }
                    else if (value.ToLower()[0] == consonant) //check if valid in lowercase
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsConsonant()
        {
            return IsConsonant(Value);
        }

        //check for a pause
        public static bool IsPause(string value)
        {
            return value == "%";
        }

        public bool IsPause()
        {
            return IsPause(Value);
        }

        //check if any of the above are true
        public static bool IsValidPhoneme(string value)
        {
            return IsVowel(value) || IsConsonant(value) || IsPause(value);
        }

        //get 2D phoneme array from a phoneme string
        public static Phoneme[][] GetPhonemesFromString(string value)
        {
            var words = value.Split(' ');

            var phonList = new List<Phoneme[]> { };
            foreach (var word in words)
            {
                var phons = new List<Phoneme> { };
                for (int i = 0; i < word.Length; ++i)
                {
                    var c = word[i].ToString();
                    if (IsConsonant(c))
                    {
                        phons.Add(new Phoneme(c));
                    }
                    if (i < word.Length - 1) //if the string is long enough, check for vowels
                    {
                        var v = word.Substring(i, 2).ToUpper();
                        if (IsVowel(v))
                        {
                            phons.Add(new Phoneme(v));
                            i++;
                        }
                    }
                }
                phonList.Add(phons.ToArray());
            }
            return phonList.ToArray();
        }
    }
}
