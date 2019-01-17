using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**
 * This program gives helpful 'suggestions' for possible moves in Words With Friends.
 */

namespace WWFHelper
{
    class Node
    {

        private readonly Node[] _children;
        private bool _isWord;
        private const int _alphabetOffet = 97;
        private const char _usedChar = '_';

        public Node()
        {
            this._isWord = false;
            this._children = new Node[26];
        }

        public void Insert(string word)
        {
            // Get the first char of the string, and see if it exists in the array, if not, make a new node and add it to the array
            char firstLetter = word.ElementAt(0);
            int index = firstLetter - _alphabetOffet;

            if (_children[index] == null)
                _children[index] = new Node(); // The child Node does not exist, make a new one and place it there

            // If there is more to be done, pass on the rest of the word, otherwise, mark it as a complete word
            if (word.Length > 1)
                _children[index].Insert(word.Substring(1));
            else
                _children[index]._isWord = true;
        }

        public List<string> GetPlayableWordsFromChildren(char[] letters)
        {
            List<string> toReturn = new List<string>();

            for (int index = 0; index < letters.Length; index++)
            {
                if (letters[index] == _usedChar)
                    continue;

                int startIndex, searchLength, count = 0;

                // If we are on a free tile, we need to search each of the children for possible letters
                if (letters[index] == '-')
                {
                    startIndex = 0;
                    searchLength = 26;
                }
                else
                {
                    startIndex = startIndex = letters[index] - _alphabetOffet;
                    searchLength = 1;
                }

                while (count < searchLength)
                {
                    if (_children[startIndex + count] != null)
                    {
                        if (_children[startIndex + count]._isWord)
                        {
                            if (letters[index] == '-')
                                toReturn.Add(":" + Convert.ToChar((startIndex + count) + _alphabetOffet) + ":");
                            else
                                toReturn.Add(letters[index].ToString());
                        }
                        // Remove this letter from the remaining letters, restore after call
                        char temp = letters[index];
                        letters[index] = _usedChar;
                        foreach (string partialWord in _children[startIndex + count].GetPlayableWordsFromChildren(letters))
                        {
                            if (temp == '-') // If the tile is free, add markers to show what the free tile should be
                                toReturn.Add(":" + Convert.ToChar((startIndex + count) + _alphabetOffet) + ":" + partialWord);
                            else
                                toReturn.Add(temp + partialWord);
                        }
                        letters[index] = temp;
                    }
                    count++;
                }
            }
            return toReturn;
        }
    }

    class WordsWithFriendsHelper
    {
        static void Main(string[] args) {

            Node root = new Node();
            System.IO.StreamReader file = new System.IO.StreamReader(@"../../wwf_dict.txt");
            string nextLine;
            while ((nextLine = file.ReadLine()) != null)
                root.Insert(nextLine);
            file.Close();

            Console.Write("Enter your tiles: ");
            string letters = Console.ReadLine();
            string possiblePlayoffTiles = "";

            while (true)
            {
                Console.Write("Enter Possible Playoff Tiles: ");
                possiblePlayoffTiles = Console.ReadLine();

                if(possiblePlayoffTiles == "exit")
                    Environment.Exit(0);

                char[] tiles = (letters.ToLower() + possiblePlayoffTiles.ToLower()).ToCharArray();
                List<string> wordsWithoutPlayoffTiles = new List<string>();
                List<string> wordsWithMultiplePlayoffTiles = new List<string>();

                // Get all of the possible words, sort them longest to smallest, remove all duplicates
                List<string> words = root.GetPlayableWordsFromChildren(tiles).OrderBy(x => x.Length * -1).Distinct().ToList();
                for (int count = 0; count < words.Count; count++)
                {
                    string possibleWord = words.ElementAt(count);
                    if (count < words.Count - 1 && possibleWord.Equals(words.ElementAt(count + 1)))
                        continue; // Don't bother with repeats

                    bool containPlayoffTile = false, containsMultiplePlayoffTiles = false;
                    foreach (char element in possiblePlayoffTiles)
                    {
                        if (possibleWord.IndexOf(element) != -1 && containPlayoffTile)
                        {
                            containsMultiplePlayoffTiles = true; // It is unlikely both playoff tiles are aligned correctly
                            break;
                        }

                        if (possibleWord.IndexOf(element) != -1) // Words that do not contain playoff tiles
                            containPlayoffTile = true;
                    }
                    if (!containPlayoffTile)
                        wordsWithoutPlayoffTiles.Add(possibleWord);
                    else if (containsMultiplePlayoffTiles)
                        wordsWithMultiplePlayoffTiles.Add(possibleWord);
                    else
                        Console.WriteLine(possibleWord);
                }

                Console.WriteLine("Words Without Playoff Tiles:");
                Console.WriteLine(String.Join("\n", wordsWithoutPlayoffTiles));

                Console.WriteLine("Words with Multiple Playoff Tiles:");
                Console.WriteLine(String.Join("\n", wordsWithMultiplePlayoffTiles));
            }
        }
    }
}
