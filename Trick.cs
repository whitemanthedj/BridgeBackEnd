using System;
using System.Collections.Generic;
using DeckOfCards;

namespace BridgeTricks
{
    public class Trick
    {
        
        int nummaOfPlayers;

        Card firstCard;

        Card highestCard;

        //player winning the trick
        int winnerIndex;


        /* JOINT LISTS, both change at the same time and have same length */
        List<Card> cardsPlayed;
        List<int> playerOrder;

        public Trick(int nummaOfPlayers)
        {
            this.nummaOfPlayers = nummaOfPlayers;
            this.cardsPlayed = new List<Card>();
            this.playerOrder = new List<int>();

            this.firstCard = null;
            this.highestCard = null;
        }

        /// <summary>
        /// Adds a card to the trick. 
        /// ASSUMES THE CARD CAN BE PLAYED
        /// </summary>
        /// <param name="playingCard">card to add</param>
        /// <param name="trump">suit of trump</param>
        /// <param name="playerIndex">player who played the card</param>
        /// <returns>returns if the trick is OVER</returns>
        public bool AddCard(Card playingCard, cardSuit trump, int playerIndex)
        {
            //initialize the first card in the trick
            return this.AddingTheCard(playingCard, CardIsFirstTrump(playingCard, trump), playerIndex);
        }

        // No trump
        /// <summary>
        /// adds a card to the trick when there is explicitly no trump suit: NT.
        /// ASSUMES THE CARD CAN BE PLAYED
        /// </summary>
        /// <param name="playingCard">card to add</param>
        /// <param name="playerIndex">player who played the card</param>
        /// <returns>returns if the trick is OVER</returns>
        public bool AddCard(Card playingCard, int playerIndex)
        {
            //initialize the first card in the trick
            return this.AddingTheCard(playingCard, false, playerIndex);
        }

        private bool AddingTheCard(Card playingCard, bool isThisFirstTrump, int playerIndex)
        {
            //initialize the first card in the trick
            if(this.firstCard == null)
            {
                this.firstCard = playingCard;
                this.highestCard = playingCard;
                this.winnerIndex = playerIndex;
            }

            this.cardsPlayed.Add(playingCard);
            this.playerOrder.Add(playerIndex);

            // update the winning trick
            if(GreaterInSameSuit(playingCard, this.highestCard) || isThisFirstTrump)
            {
                this.highestCard = playingCard;
                this.winnerIndex = playerIndex;
            }

            // if all players have played, the trick is over
            return this.cardsPlayed.Count < this.nummaOfPlayers;
        }


        public Card WinningTrick()
        {
            return this.highestCard;
        }

        public int TrickWinner()
        {
            return this.winnerIndex;
        }

        public bool hasTrickStarted()
        {
            return (this.firstCard != null);
        }
        
        // ASSUMES THAT THERE HAS BEEN A CARD PLAYED
        public cardSuit FollowSuit()
        {

            return this.firstCard.Suit();
        }

        public Card FirstCardPlayed()
        {
            return (this.firstCard);
        }

        public static bool GreaterInSameSuit(Card playingCard, Card highestCard)
        {
            return ( (playingCard.Suit() == highestCard.Suit()) && (playingCard.FaceValue() > highestCard.FaceValue()) );
        }

        /**/
        public bool CardIsFirstTrump(Card playingCard, cardSuit trump)
        {
            return (playingCard.Suit() == trump && (this.highestCard != null ? this.highestCard.Suit() != trump : true));
        }
        /**/


        public void PrintRecord()
        {
            int index = 0;
            Console.WriteLine("------");
            foreach(Card currentCard in this.cardsPlayed)
            {
                Console.WriteLine("Player " + (this.playerOrder[index] + 1) + ": " + currentCard.ToString());
                index++;
            }
            Console.WriteLine("------");
        }
    
    }



   
}