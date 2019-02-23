using System;
using System.Collections.Generic;

namespace WWF
{

    /**
    * This program finds unique anagrams extremely quickly.
    * 
    * An anagram is unique if its letters cannot be combined to create any other words given list.
    */
    public class FastestAnagramSolver
    {
        public static HashSet<Word> Solver(List<string> Words)
        {
            WordComparer comparer = new WordComparer();

            HashSet<Word> AnagramSet = new HashSet<Word>(comparer);

            foreach (string word in Words)
            {
                Word newWord = new Word();
                UInt64 hash = 3074457345618258791ul;
                foreach (char letter in word)
                {
                    newWord.Letters[letter - 97] += 1;
                    hash = hash ^ (letter ^ 3074457345618258799ul);
                }
                newWord.Hash = hash;
                newWord.origin = word;

                AnagramSet.Add(newWord);
            }

            return AnagramSet;
        }
        public class Word
        {
            public UInt64 Hash;
            public int[] Letters;
            public string origin;

            public Word()
            {
                Letters = new int[26];
            }
        }
        public class WordComparer : IEqualityComparer<Word>
        {
            public bool Equals(Word first, Word second)
            {
                if (first.Hash != second.Hash)
                    return false;

                for (int index = 0; index < first.Letters.Length; index++)
                {
                    if (first.Letters[index] != second.Letters[index])
                        return false;
                }
                return true;
            }

            public int GetHashCode(Word toHash)
            {
                return (int) toHash.Hash;
            }

        }
    }

}
