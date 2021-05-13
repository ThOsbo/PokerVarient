using System;
using System.Collections.Generic;

namespace CardGames
{
    enum Suites { Spade, Club, Diamond, Heart, Last };
    enum Names { Joker, Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Last };

    class Deck 
    {
        private Stack<Card> deck = new Stack<Card>();
        public int size = 0;

        public Deck(int numDecks = 1) 
        {
            CreateNewDeck(numDecks);
            size = deck.Count;
        }

        public void CreateNewDeck(int numDecks = 1) 
        {
            //creates a new ordered deck
            var suiteType = typeof(Suites);
            var nameType = typeof(Names);

            deck.Clear();
            for (int i = 0; i < numDecks; i++) 
            {
                for (int suiteIndex = 0; suiteIndex < (int)Suites.Last; suiteIndex++) 
                {
                    string suite = Enum.GetName(suiteType, suiteIndex);

                    for (int nameIndex = 1; nameIndex < (int)Names.Last; nameIndex++) 
                    {
                        string name = Enum.GetName(nameType, nameIndex);

                        deck.Push(new Card(name, suite));
                    }
                }
            }
            
        }

        public Card DrawCard() 
        {
            //draws the top card from the deck
            Exception noCards = new Exception("NO CARDS IN DECK");

            if (deck.Count > 0) 
            {
                size--;
                return deck.Pop();
            }
            else 
            {
                throw(noCards);
            }
        }

        public void ShuffuleDeck() 
        {
            //shuffles current deck
            List<Card> toShuffle = new List<Card>(deck);
            deck.Clear();

            Random random = new Random();

            while (toShuffle.Count > 0) 
            {
                //takes card from random position in list and adds to deck stack
                int nextPos = random.Next(0, toShuffle.Count);
                deck.Push(toShuffle[nextPos]);
                toShuffle.RemoveAt(nextPos);
            }
        }
    }

    class Card 
    {
        public int value;
        public string suite;
        public string name;

        public Card(string _name, string _suite) 
        {
            var suiteType = typeof(Suites);
            var nameType = typeof(Names);

            Func<string, string> Capitalize = (input) => 
            {
                string firstLetter = input.Remove(1);
                string tailingLetters = input.Remove(0, 1);

                return firstLetter.ToUpper() + tailingLetters.ToLower();
            };

            Exception invalidSuite = new Exception("INVALID SUITE");
            Exception invalidName = new Exception("INVALID NAME");

            _suite = Capitalize(_suite);

            if (Enum.IsDefined(suiteType, _suite)) 
            {
                suite = _suite;
            }
            else 
            {
                throw(invalidSuite);
            }

            _name = Capitalize(_name);

            if (Enum.IsDefined(nameType, _name)) 
            {
                value = (int)Enum.Parse(nameType, _name);
            }
            else 
            {
                throw(invalidName);
            }

            name = string.Format("{0} of {1}s", _name, _suite);
            
        }
    }
}