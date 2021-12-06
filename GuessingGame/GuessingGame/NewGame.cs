using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessingGame
{
    public class NewGame
    {
        private int WordLength;
        private int GuessCount;
        private int MaxGuesses;

        private string PlayWord; //Secret word being guessed by player

        private List<string> WordFamilyList;
        private List<char> CorrectLetters = new List<char>();
        private List<char> WrongLetters = new List<char>();

        public string[] Dictionary;

        public Random Randomizer = new Random();

        private bool ShowWordLLength;

        public IEnumerable<string> GetWordsWithLength(int length)
        {
            foreach (var word in Dictionary)
            {
                if (word.Length == length)
                {
                    yield return word;
                }
            }

            yield break;
        }

        public bool IsUsable(string word, string wordDash, char guessedChar = ' ', bool findWithChar = false)
        {
            //Word is Usable
            bool isUsable = true;

            if (word.Length != wordDash.Length) return false;

            //Check character position matches
            for (int i = 0; i < wordDash.Length; i++)
            {
                switch (wordDash[i])
                {
                    case '-':
                        if (word[i] == guessedChar)
                            isUsable &= true && (findWithChar == false);
                        break;
                    default:
                        isUsable &= wordDash[i] == word[i];
                        break;
                }
            }

            //Check if word contains wrong letter
            foreach (var c in word) isUsable &= !WrongLetters.Contains(c);
            return isUsable;
        }

        string GetWordDash()
        {
            string wordDash = "";

            for (int i = 0; i < PlayWord.Length; i++)
            {
                if (CorrectLetters.Contains(PlayWord[i])) wordDash += PlayWord[i].ToString();
                else wordDash += "-";
            }
            return wordDash;
        }

        public void Start()//method starts the game
        {
            Console.Clear();
            Console.WriteLine("The game rules are:");
            Console.WriteLine("1. Select a word length");
            Console.WriteLine("2. Enter desired number of guesses");
            Console.WriteLine("3. Attempt to guess the letters in the word till you run out of guesses");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();

            GetWordLength();
            GetNumberOfGuesses();
            GetShowListLength();
            Console.Clear();

            for (GuessCount = 0; GuessCount < MaxGuesses && GetWordDash().Contains("-");)
                RunGameLogic();

            if (GetWordDash().Contains("-"))
            {
                Console.WriteLine("Sorry, you ran out of guesses");
                Console.WriteLine("The word was: {0}", PlayWord);
            }
            else
            {
                Console.WriteLine("You were correct : {0}! ", PlayWord);
            }

            if (AskPlayAgain())
            {
                Console.Clear();
                CorrectLetters.Clear();
                WrongLetters.Clear();
                GuessCount = 0;
                Start();
            }
        }

        private bool AskPlayAgain()//Asks the user whether they want to play the game again
        {
            bool playAgain;
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Play again? (Y) or (N):  ");

                var con = Console.ReadKey(true).Key;
                if (con == ConsoleKey.Y || con == ConsoleKey.N)
                {
                    playAgain = con == ConsoleKey.Y;
                    break;
                }
            }

            return playAgain;
        }

        private void GetWordLength()
        {
            //Asks user for the word length
            Console.Write("Enter the desired word length?  ");
            string input = Console.ReadLine();

            // Checks to see if the user entered a number or not
            while (!int.TryParse(input, out WordLength))
            {
                Console.Write("\nInput a number.Try again: ");
                input = Console.ReadLine();
            }

            try
            {
                WordFamilyList = GetWordsWithLength(WordLength).ToList();
                PlayWord = WordFamilyList[Randomizer.Next(0, WordFamilyList.Count())];
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid word Length");
                GetWordLength();
            }
        }

        private void GetNumberOfGuesses()
        {
            // Ask user for their desired number of guesses
            Console.Write("\nEnter your desired number of guesses: ");
            int.TryParse(Console.ReadLine(), out MaxGuesses);

            // Check if user input is valid
            while (MaxGuesses < 1)
            {
                Console.Write("Enter a valid number: ");
                int.TryParse(Console.ReadLine(), out MaxGuesses);
            }
        }

        private void GetShowListLength()
        {
            while (true)
            {
                Console.Write("\nDisplay word list length? (Y) or (N):  ");
                var sWList = Console.ReadKey(false).Key;
                if (sWList == ConsoleKey.Y || sWList == ConsoleKey.N)
                {
                    ShowWordLLength = sWList == ConsoleKey.Y;
                    break;
                }
            }
        }

        private void ShowGameStatus()
        {
            string guessesLeft = string.Format("You have {0} guesses left.", MaxGuesses - GuessCount);
            string wrongLetters = string.Format("Wrong letters: {0}", string.Join(" ", WrongLetters));
            string correctLetters = string.Format("Correct letters: {0}", string.Join(" ", CorrectLetters));
            string curWList = string.Format("Word List length: {0}", WordFamilyList.Count() + 1);
            string sWord = string.Format("Secret Word: {0}", GetWordDash());

            Console.Clear();

            Console.WriteLine(wrongLetters);
            Console.WriteLine(correctLetters);
            Console.WriteLine(sWord);
            Console.WriteLine(guessesLeft);
            if (ShowWordLLength) Console.WriteLine(curWList);
        }

        List<string> GetWords(bool withChar, List<string> list, char c)
        {
            //Get words either with or without char
            //withChar -> Get words with char if true and vice versa
            List<string> output = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                string currentWord = list[i];
                if (IsUsable(currentWord, GetWordDash(), c, withChar))
                    output.Add(currentWord);
            }

            return output;
        }

        private void Cheat(char input)
        {
            //Cheat using input from the user to make a decision
            if (!PlayWord.Contains(input.ToString()) || CorrectLetters.Contains(input)) return;

            List<string> wordsWithChar = GetWords(false, WordFamilyList, input);
            List<string> wordsWithoutChar = GetWords(true, WordFamilyList, input);
            List<string> nwList;

            if (wordsWithChar.Count() > wordsWithoutChar.Count()) nwList = wordsWithChar;
            else nwList = wordsWithoutChar;

            PlayWord = nwList[Randomizer.Next(0, nwList.Count())];
        }

        private void RunGameLogic()
        {
            ShowGameStatus(); //Show current game status
            Console.WriteLine("Your guess: ");
            char c = Console.ReadKey().KeyChar;
            Cheat(c);


            if (!char.IsLetter(c))
            {
                //Guess is not an alpabet
                Console.WriteLine("Guess is not an alphabet.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                RunGameLogic();//Start method again
            }
            else
            {
                if (WrongLetters.Contains(c) || CorrectLetters.Contains(c))
                {
                    //Guess already made guess
                    Console.WriteLine("Guess has already been made.");
                    Console.WriteLine("You will still loose a guess. Be careful");
                    Console.ReadKey();
                    GuessCount++;
                }
                else if (PlayWord.Contains(c.ToString()))
                {
                    //Is Correct guess
                    CorrectLetters.Add(c);
                }
                else if (!WrongLetters.Contains(c))
                {
                    //Is wrong guess
                    WrongLetters.Add(c);
                    GuessCount++;
                }
            }
        }
        public NewGame(string[] dict)
        {
            Dictionary = dict;
        }
    }
}
