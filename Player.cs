using System.Collections.Generic;

namespace CardGames 
{
    abstract class Player 
    {
        protected Hand hand = new Hand();

        public void AddCardToHand(Card card) 
        {
            this.hand.AddCardToHand(card);
        }

        protected void RemoveCardFromHand(int pos) 
        {
            this.hand.RemoveCardFromHand(pos);
        }

        public Hand GetHand()
        {
            return this.hand;
        }

        public int GetHandSize() 
        {
            return this.hand.GetHandSize();
        }

        public void EmptyHand() 
        {
            this.hand.EmptyHand();
        }

        abstract protected List<int> GetDiscards(int totalDiscards = 1);

        virtual public void PlayGame(Card[] onTable, Deck deck) 
        {
            //changes up to three sets of cards from current hand
            List<int> toDiscard = GetDiscards(3);

            foreach (int posToDiscard in toDiscard) 
            {
                RemoveCardFromHand(posToDiscard);
            }

            for (int numChanged = 0; numChanged < toDiscard.Count; numChanged++) 
            {
                AddCardToHand(deck.DrawCard());
            }

            //adds cards currently on the table to the hand
            foreach (Card card in onTable) 
            {
                AddCardToHand(card);
            }

            //discards a single card to make a hand of five cards
            bool gotDiscard = false;

            while (!gotDiscard) 
            {
                List<int> toDiscard2 = GetDiscards(1);

                if (toDiscard2.Count == 1) 
                {
                    gotDiscard = true;
                    RemoveCardFromHand(toDiscard2[0]);
                }
            }

        }
    }
}