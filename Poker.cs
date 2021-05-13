using System;
using System.Collections.Generic;

namespace CardGames 
{
    enum ScoringHands { Empty, Pair, TwoPairs, ThreeOfAKind, Straight, Flush, FullHouse, FourOfAKind, StraightFlush, RoyalFlush, Last}

    class Poker 
    {
        public void PlayPoker() 
        {
            bool playGame = true;

            Deck deck = new Deck();

            Card[] onTable = new Card[3];

            HumanOpponent player = new HumanOpponent();
            ComputerOpponent dealer = new ComputerOpponent();

            var handType = typeof(ScoringHands);

            while (playGame) 
            {
                deck.CreateNewDeck();
                deck.ShuffuleDeck();

                player.EmptyHand();
                dealer.EmptyHand();

                Array.Clear(onTable, 0, onTable.Length);

                Console.WriteLine("Cards on the table :");

                for (int i = 0; i < 3; i++) 
                {
                    onTable[i] = deck.DrawCard();
                    Console.WriteLine(onTable[i].name);

                    player.AddCardToHand(deck.DrawCard());
                    dealer.AddCardToHand(deck.DrawCard());
                    
                }

                Console.WriteLine();

                player.PlayGame(onTable, deck);
                dealer.PlayGame(onTable, deck);

                int playerRank = RankHand(player.GetHand());
                int dealerRank = RankHand(dealer.GetHand());

                Console.WriteLine("Player's Hand :");
                player.GetHand().DisplayHand();

                Console.WriteLine("Dealer's Hand : ");
                dealer.GetHand().DisplayHand();

                Console.WriteLine(@"Player has {0}", Enum.GetName(handType, playerRank));
                Console.WriteLine(@"Dealer has {0}", Enum.GetName(handType, dealerRank));

                Console.WriteLine();

                if (playerRank > dealerRank) 
                {
                    Console.WriteLine("Player Wins");
                }
                else if (dealerRank > playerRank) 
                {
                    Console.WriteLine("Dealer Wins");
                }
                else 
                {
                    Console.WriteLine("Draw");
                }

                Console.Write("Next Round (y/n)? : ");

                if (Console.ReadKey().KeyChar =='n') 
                {
                    playGame = false;
                }
                else 
                {
                    Console.WriteLine("\n");
                }
            }
        }

        private int RankHand(Hand hand) 
        {
            int rank = 0;

            var handType = typeof(ScoringHands);

            int checkHand = (int)Enum.Parse(handType, CheckCollections(hand));
            int checkHand2 = (int)Enum.Parse(handType, CheckStraightsAndFlushes(hand));

            return rank = (checkHand > checkHand2) ? checkHand : checkHand2;
        }

        private string CheckCollections(Hand hand) 
        {
            int numPairs = 0;
            
            Dictionary<int, int> CountVals = new Dictionary<int, int>();

            foreach (Card card in hand.GetHand()) 
            {
                if (CountVals.ContainsKey(card.value)) 
                {
                   if (++CountVals[card.value] == 2) 
                   {
                       numPairs++;
                   }
                }
                else 
                {
                    CountVals.Add(card.value, 1);
                }
            }

            string handType;

            if (CountVals.ContainsValue(4)) 
            {
                handType = "FourOfAKind";
            }
            else if (CountVals.ContainsValue(3) && CountVals.ContainsValue(2)) 
            {
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

        private string CheckStraightsAndFlushes(Hand hand) 
        {
            bool isFlush = true;

            List<int> cardValues = new List<int>();

            for (int i = 0; i < hand.GetHandSize(); i++) 
            {
                cardValues.Add(hand.GetHand()[i].value);

                if (hand.GetHand()[0].suite != hand.GetHand()[i].suite) 
                {
                    isFlush = false;
                }
            }

            cardValues.Sort();

            //if hand contains both a king and an ace then ace comes after king in a straight
            if (cardValues[0] == (int)Names.Ace && cardValues[cardValues.Count - 1] == (int)Names.King) 
            {
                cardValues.RemoveAt(0);
                cardValues.Add((int)Names.King + 1);
            }

            bool isStraight = true;

            for (int i = 0; i < cardValues.Count - 1; i++) 
            {
                if (cardValues[i] != cardValues[i + 1] + 1) 
                {
                    isStraight = false;
                }
            }

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