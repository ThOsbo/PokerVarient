using System;
using System.Collections.Generic;

namespace CardGames
{
    class HumanOpponent : Player 
    {
        public override void PlayGame(Card[] onTable, Deck deck)
        {
            base.PlayGame(onTable, deck);
        }

        override protected List<int> GetDiscards(int totalDiscards = 1) 
        {
            List<int> toDiscard = new List<int>();
            
            int numDiscards = 0;

            Console.WriteLine("Player's Hand : ");
            this.hand.DisplayHand();

            Console.Write("Choose cards to change or press enter : ");

            //gets input from the player until enter is pressed
            while (numDiscards < totalDiscards) 
            {
                char input = Console.ReadKey().KeyChar;

                if (input == '\r') 
                {
                    break;
                }
                else 
                {
                    if (Char.IsDigit(input)) 
                    {
                        int posToDiscard = Convert.ToInt16(input.ToString());

                        if (posToDiscard >= 0 && posToDiscard < this.hand.GetHandSize() && !toDiscard.Contains(posToDiscard)) 
                        {
                            toDiscard.Add(posToDiscard);
                            numDiscards++;
                        }
                    }
                }
            }

            Console.WriteLine("\n");

            toDiscard.Sort();
            toDiscard.Reverse();

            return toDiscard;
        }
    }
}