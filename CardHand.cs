using System;
using System.Collections.Generic;

namespace CardGames
{
    class Hand 
    {
        private List<Card> hand = new List<Card>();
        

        public void AddCardToHand(Card cardToAdd) 
        {
            hand.Add(cardToAdd);
        }

        public void RemoveCardFromHand(int posToRemove) 
        {
            Exception outsideHand = new Exception("TRIED TO DISCARD A CARD OUTSIDE OF THE HAND");

            if (posToRemove >= 0 && posToRemove < hand.Count) 
            {
                hand.RemoveAt(posToRemove);
            }
            else 
            {
                throw(outsideHand);
            }

        }

        public bool ContainsCard(Card card) 
        {
            bool containsCard = false;

            foreach (Card checkCard in hand) 
            {
                if (card.name == checkCard.name) 
                {
                    containsCard = true;
                    break;
                }
            }

            return containsCard;
        }
        
        public void EmptyHand() 
        {
            hand.Clear();
        }

        public List<Card> GetHand() 
        {
            return hand;
        }

        public void DisplayHand()
        {
            Console.WriteLine();

            for (int pos = 0; pos < hand.Count; pos++)
            {
                Console.WriteLine(@"{0} : {1}", pos , hand[pos].name);
            }

            Console.WriteLine();
        }

        public int GetHandSize() 
        {
            return hand.Count;
        }
    }
}