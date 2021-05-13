using System;
using System.Collections.Generic;

namespace CardGames
{
    class ComputerOpponent : Player
    {        
        Hand currentHand = new Hand();
        private Dictionary<List<int>, (float, int)> rankDiscards = new Dictionary<List<int>, (float, int)>();

        public override void PlayGame(Card[] onTable, Deck deck)
        {
            currentHand.EmptyHand();
            rankDiscards.Clear();

            foreach (Card card in onTable) 
            {
                currentHand.AddCardToHand(card);
            }

            foreach (Card card in this.hand.GetHand()) 
            {
                currentHand.AddCardToHand(card);
            }


            int currentHandRank = CalcuateHandRank(currentHand);

            rankDiscards.Add(new List<int>(), (currentHandRank, currentHandRank));

            FindScoringHands(currentHand, 3);

            base.PlayGame(onTable, deck);
        }

        override protected List<int> GetDiscards(int totalDiscards = 1) 
        {
            //returns the discards chosen by the computer
            List<int> toDiscard = new List<int>();            

            if (this.hand.GetHandSize() < 5 && totalDiscards > 1) 
            {
                toDiscard = GetBestDiscard();
            }
            else 
            {
                toDiscard.Add(FindHighestScoringHand());
            }

            return toDiscard;
        }

        private List<int> GetBestDiscard() 
        {
            List<int> currentBestDiscard = new List<int>();
            (double avg, int max) currentBestRank = (0, 0);

            foreach (KeyValuePair<List<int>, (float avg, int max)> discards in rankDiscards) 
            {
                //favours discards with a higher average hand rank
                if (discards.Value.avg > currentBestRank.avg) 
                {
                    currentBestDiscard = discards.Key;
                    currentBestRank = discards.Value;
                }
                else if (discards.Value.avg == currentBestRank.avg) 
                {
                    //favours discards with a higher max rank if the averages are the same
                    if (discards.Value.max > currentBestRank.max) 
                    {
                        currentBestDiscard = discards.Key;
                        currentBestRank = discards.Value;
                    }
                    else if (discards.Value.max == currentBestRank.max) 
                    {
                        //favours less discards if the average and max rank are the same
                        if (discards.Key.Count > currentBestDiscard.Count) 
                        {
                            currentBestDiscard = discards.Key;
                            currentBestRank = discards.Value;
                        }
                    }
                }
            }

            return currentBestDiscard;
        }

        private void FindScoringHands(Hand currentHand, int totalDiscards = 1) 
        {
            var suiteType = typeof(Suites);
            var nameType = typeof(Names);

            //creates a list of cards that are unknown to the computer
            List<Card> cards = new List<Card>();

            for (int suite = (int)Suites.Spade; suite < (int)Suites.Last; suite++) 
            {
                for (int val = (int)Names.Ace; val < (int)Names.Last; val++) 
                {
                    Card card = new Card(Enum.GetName(nameType, val), Enum.GetName(suiteType, suite));

                    if (!currentHand.ContainsCard(card)) 
                    {
                        cards.Add(card);
                    }
                }
            }

            //finds all hands for each combination of discards
            foreach (List<int> discards in GetAllDiscards(totalDiscards)) 
            {
                //creates a new hand with all discarded cards removed
                Hand posHand = new Hand();

                for (int pos = 0 ; pos < currentHand.GetHandSize(); pos++) 
                {
                    if (!discards.Contains(pos)) 
                    {
                        posHand.AddCardToHand(currentHand.GetHand()[pos]);
                    }
                }

                List<Hand> combinations = FindAllCombinations(posHand, cards);

                (float avg, int max) handPosRank = CalculateAvgAndMaxRank(combinations);

                rankDiscards.Add(discards, handPosRank);
            }
        }

        private List<Hand> FindAllCombinations(Hand posHand, List<Card> cards, int maxCardsInHand = 6) 
        {
            List<Hand> posHands = new List<Hand>();

            bool foundAllHands = false;
            int posToChange = posHand.GetHandSize();
            int numDiscards = maxCardsInHand - posHand.GetHandSize();
            int[] startPosInCards = new int[numDiscards - 1];
            int posInCards = 0;

            for (int i = 0; i < startPosInCards.Length; i++) 
            {
                startPosInCards[i] = i + 1;
            }

            //cycles through all possible hands from the provided hand
            while (!foundAllHands) 
            {
                if (posInCards >= cards.Count) 
                {
                    if (6 - posHand.GetHandSize() >= numDiscards) 
                    {
                        //all hands found if more cards discarded than specified
                        foundAllHands = true;
                    }
                    else 
                    {
                        int toChange = 6 - posHand.GetHandSize() - 1;
                        if (startPosInCards[toChange] < cards.Count) 
                        {
                            //updates position in the list of cards to start adding cards from
                            posInCards = ++startPosInCards[toChange];
                            for (int i = toChange - 1; i >= 0; i--) 
                            {
                                startPosInCards[i] = startPosInCards[toChange];
                            }
                        }
                        else 
                        {
                            //discards last card in hand if no more cards to pick
                            posHand.RemoveCardFromHand(posHand.GetHandSize() - 1);
                        }                            
                    }
                }
                else 
                {
                    Card nextCard = cards[posInCards];

                    if (!posHand.ContainsCard(nextCard)) 
                    {
                        //adds cards till the hand contains 6 cards
                        if (posHand.GetHandSize() < 6) 
                        {
                            posHand.AddCardToHand(nextCard);
                            posInCards++;
                        }

                        //once the hand is full, records and removes the last card in prepration for the next
                        if (posHand.GetHandSize() == 6) 
                        {
                            posHands.Add(CopyHand(posHand));
                            posHand.RemoveCardFromHand(posHand.GetHandSize() - 1);
                        }
                    }
                    else 
                    {
                        posInCards++;
                    }
                }                    
            }

            return posHands;
        }

        private (float avg, int max) CalculateAvgAndMaxRank(List<Hand> handList) 
        {
            //calculates the average and maximum rank for a list of different hands
            float totalRank = 0f;
            int maxRank = 0;

            foreach (Hand handToRank in handList) 
            {
                int handRank = CalcuateHandRank(handToRank);

                totalRank += (float)handRank;

                if (handRank > maxRank) 
                {
                    maxRank = handRank;
                }
            }

            return (totalRank / (float)handList.Count, maxRank);
        }

        private Hand CopyHand(Hand hand) 
        {
            //copies a hand into a new hand
            Hand newHand = new Hand();

            foreach (Card card in hand.GetHand()) 
            {
                newHand.AddCardToHand(card);
            }

            return newHand;
        }

        private List<List<int>> GetAllDiscards(int totalDiscards = 1) 
        {
            //finds all combinations of discards up to a total number of discards
            List<List<int>> allDiscards = new List<List<int>>();

            List<int> potDiscards = new List<int>();

            bool moreDiscards = true;
            int numDiscards = 1;
            int posToChange = 0;

            while (moreDiscards) 
            {
                if (potDiscards.Count < numDiscards) 
                {
                    //adds a discard to the list when increasing number discards
                    potDiscards.Insert(0, 0);
                    for (int i = 1; i < potDiscards.Count; i++) 
                    {
                        potDiscards[i] = i;
                    }
                    posToChange = potDiscards.Count - 1;

                    allDiscards.Add(CopyAndSortList(potDiscards));
                }
                else 
                {
                    if (potDiscards[posToChange] < this.hand.GetHandSize() - 1 && !potDiscards.Contains(potDiscards[posToChange] + 1)) 
                    {
                        //updates a position to a new discard position
                        potDiscards[posToChange]++;

                        if (posToChange < potDiscards.Count - 1) 
                        {
                            for (int i = posToChange + 1; i < potDiscards.Count; i++) 
                            {
                                potDiscards[i] = potDiscards[posToChange] + i - posToChange;
                            }

                            posToChange = potDiscards.Count - 1;
                        }

                        allDiscards.Add(CopyAndSortList(potDiscards));
                    }
                    else 
                    {
                        if (posToChange - 1 >= 0) 
                        {
                            //changes position to update if current position is at maximum value
                            posToChange--;
                        }
                        else 
                        {
                            if (numDiscards + 1 <= totalDiscards) 
                            {
                                //increases number of discards up to a total number of discards
                                numDiscards++;
                            }
                            else 
                            {
                                //all discards found
                                moreDiscards = false;
                            }
                        }
                        
                    }
                }
            }

            return allDiscards;
        }

        private List<int> CopyAndSortList(List<int> toCopy) 
        {
            //copies and sorts a list into a new list
            List<int> newList = new List<int>();

            foreach (int item in toCopy) 
            {
                newList.Add(item);
            }

            newList.Sort();
            newList.Reverse();

            return newList;
        }

        private int FindHighestScoringHand() 
        {
            //finds discard that produces highest ranked five card hand 
            int toDiscard = 0;
            int currentHighestRank = 0;

            for (int posToDiscard = 0; posToDiscard < this.hand.GetHandSize(); posToDiscard++) 
            {
                //fills new temporary hand with all currently held cards bar the one to discard
                Hand potHand = new Hand();

                for (int pos = 0; pos < this.hand.GetHandSize(); pos++) 
                {
                    if (pos != posToDiscard) 
                    {
                        potHand.AddCardToHand(this.hand.GetHand()[pos]);
                    }
                }

                int rank = CalcuateHandRank(potHand);

                if (currentHighestRank < rank) 
                {
                    toDiscard = posToDiscard;
                    currentHighestRank = rank;
                }
            }

            //returns the discard that gives the hand the highest rank
            return toDiscard;
        }

        private int CalcuateHandRank(Hand checkHand) 
        {
            //calculates rank of hand and returns
            var handType = typeof(ScoringHands);

            int check1 = (int)Enum.Parse(handType, CheckCollections(checkHand));
            int check2 = (int)Enum.Parse(handType, CheckStraightsAndFlushes(checkHand));

            return (check1 > check2) ? check1 : check2;
        }

        private string CheckCollections(Hand checkHand) 
        {
            int numPairs = 0;
            
            //records card values found in hand and how many each shows up
            Dictionary<int, int> CountVals = new Dictionary<int, int>();

            foreach (Card card in checkHand.GetHand()) 
            {
                if (CountVals.ContainsKey(card.value)) 
                {
                   if (++CountVals[card.value] == 2) 
                   {
                       //increases if a card with the same value is found twice
                       //also includes 3 and 4 of a kinds
                       numPairs++;
                   }
                }
                else 
                {
                    CountVals.Add(card.value, 1);
                }
            }

            //determines hand type from contents of dictionary
            string handType;

            if (CountVals.ContainsValue(4)) 
            {
                handType = "FourOfAKind";
            }
            else if (CountVals.ContainsValue(3) && numPairs >= 2) 
            {
                //the three of a kind will also be counted by numPairs
                handType = "FullHouse";
            }
            else if (CountVals.ContainsValue(3)) 
            {
                handType = "ThreeOfAKind";
            }
            else if (numPairs == 2) 
            {
                handType = "TwoPairs";
            }
            else if (numPairs == 1) 
            {
                handType = "Pair";
            }
            else 
            {
                handType = "Empty";
            }

            return handType;
        }

        private string CheckStraightsAndFlushes(Hand checkHand) 
        {
            const int maxHandSize = 5;

            //takes into account cards can be discarded to make a 5 card staight or flush
            int numDiscards = checkHand.GetHandSize() - maxHandSize;
            int numDiscardsNeeded = 0;

            //records suites found in hand and number of recurences 
            Dictionary<string, int> suiteCount = new Dictionary<string, int>();

            List<int> cardValues = new List<int>();

            for (int i = 0; i < checkHand.GetHandSize(); i++) 
            {
                //adds values to list to check for staights
                cardValues.Add(checkHand.GetHand()[i].value);

                string nextSuite = checkHand.GetHand()[i].suite;

                if (suiteCount.ContainsKey(nextSuite)) 
                {
                    suiteCount[nextSuite]++;
                }
                else 
                {
                    suiteCount.Add(nextSuite, 1);
                }
            }

            //determines if hand is a flush if a set number of cards have the same suite
            bool isFlush = true;

            if (!suiteCount.ContainsValue(maxHandSize)) 
            {
                isFlush = false;
            }

            cardValues.Sort();

            //if hand contains both a king and an ace then ace comes after king in a straight
            if (cardValues[0] == (int)Names.Ace && cardValues[cardValues.Count - 1] == (int)Names.King) 
            {
                cardValues.RemoveAt(0);
                cardValues.Add((int)Names.King + 1);
            }

            bool isStraight = true;

            //resets to check straights
            numDiscardsNeeded = 0;

            //sets to first value in sorted list of values in hand
            int lastValue = cardValues[0];

            for (int i = 1; i < cardValues.Count; i++) 
            {
                int nextValue = cardValues[i];

                //checks if the next value directly follows the last one
                if (lastValue != nextValue - 1) 
                {
                    if (numDiscardsNeeded >= numDiscards) 
                    {
                        isStraight = false;
                    }
                    else 
                    {
                        numDiscardsNeeded++;
                    }
                }
                else 
                {
                    lastValue = cardValues[i];
                }
            }

            //determines hand type from combinations of the isFlush and isStraight bools
            string handType;

            if (isStraight && isFlush && cardValues[0] == (int)Names.Ten) 
            {
                handType = "RoyalFlush";
            }
            else if (isStraight && isFlush) 
            {
                handType = "StraightFlush";
            }
            else if (isFlush) 
            {
                handType = "Flush";
            }
            else if (isStraight) 
            {
                handType = "Straight";
            }
            else 
            {
                handType = "Empty";
            }

            return handType;
        }

    }
}
