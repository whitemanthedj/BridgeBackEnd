using System;
using System.Collections.Generic;
using DeckOfCards;
using BridgeBid;


namespace BridgePlayer
{
    public class Player
    {
        
        //List<Card> hand;

        List<Card> spadeCards;
        List<Card> heartCards;
        List<Card> clubCards;
        List<Card> diamondCards;

        Bid playerBid;

        Card lastCard;

        int highCardPoints;

        int indexOfPartner;

        bool removingCards;


        // only for after a rubber, otherwise the score is held in the Scorepad
        int score;


        // additional features for later
        //bool dealer;

        /// Must insert all cards before playing them
        public Player(int playerNumber, int partner)
        {
            //this.hand = new List<Card>();
            this.indexOfPartner = partner;
            this.highCardPoints = 0;
            this.playerBid = new Bid();

            this.lastCard = null;

            this.clubCards    = new List<Card>();
            this.diamondCards = new List<Card>();
            this.heartCards   = new List<Card>();
            this.spadeCards   = new List<Card>();


            this.removingCards = false;
            
        }

        public List<Card> FullHand()
        {
            List<Card> localHand = new List<Card>();
            localHand.AddRange(this.diamondCards);
            localHand.AddRange(this.clubCards);
            localHand.AddRange(this.heartCards);
            localHand.AddRange(this.spadeCards);
            return localHand;
        }

        public void AddCardToHand(Card newCard)
        {
            //this.hand.Add(newCard);
            //int sortedValueOfCard = newCard.SortedValue();
            this.removingCards = false;
            AddCardToRightSuit(FigureOutHandFrom(newCard.Suit()), newCard);

            /** /
            switch(newCard.Suit())
            {
                case cardSuit.C:
                    AddCardToRightSuit(this.clubCards, newCard);
                    break;
                case cardSuit.D:
                    AddCardToRightSuit(this.diamondCards, newCard);
                    break;
                case cardSuit.H:
                    AddCardToRightSuit(this.heartCards, newCard);
                    break;
                default:
                    AddCardToRightSuit(this.spadeCards, newCard);
                    break;
            }
            /**/

            this.highCardPoints += (newCard.FaceValue() - 10 > 0 ? newCard.FaceValue() - 10 : 0);
            //this.PrintHand();
        }

        private static void AddCardToRightSuit(List<Card> hand, Card newCard)
        {
            bool cardInserted = false;
            
            /**/
            //sorting in theta n^2, adds the bigger cards at the end
            for(int i = 0; !cardInserted && i < hand.Count; i++)
            {
                if(hand[i].SortedValue() > newCard.SortedValue()) {
                    hand.Insert(i, newCard);
                    cardInserted = true;
                }
            } /**/

            if(!cardInserted)
            {
                hand.Add(newCard);
            }

        }

        /** /
        // int theta n^2
        private void SortHand()
        {
            List<Card> sortedHand = new List<Card>();
            //int 
        } /**/


        // playing cards
        public Card PlayCard(Card firstCardPlayed)
        {
            this.removingCards = true;
            
            List<Card> theList;
            
            // this is the first card played in the trick
            if(firstCardPlayed == null)
            {
                theList = this.FullHand();
            } else {
                // GETTING HERE MEANS THAT THERE IS A CARD PLAYED
                // AND A SUIT THAT MUST BE FOLLOWED IF POSSIBLE
                theList = this.FigureOutHandFrom(firstCardPlayed.Suit());
            }

            //Print(theList);           
            Card playingCard = this.PlayCardFrom(theList);

            //so we now need to figure out what list to remove the card from :)
            this.FigureOutHandFrom(playingCard.Suit()).Remove(playingCard);

            this.lastCard = playingCard;

            return this.lastCard;
        }

        private List<Card> FigureOutHandFrom(cardSuit suit)
        {
            List<Card> localHand;

            switch(suit)
            {
                case cardSuit.C:
                    localHand = this.clubCards;
                    break;
                case cardSuit.D:
                    localHand = this.diamondCards;
                    break;
                case cardSuit.H:
                    localHand = this.heartCards;
                    break;
                default:
                    localHand = this.spadeCards;
                    break;
            }

            
            return FigureOutHandFrom(localHand);
        }

        private List<Card> FigureOutHandFrom(List<Card> hand)
        {

            return (hand == null || (hand.Count <= 0 && this.removingCards) ? this.FullHand() : hand);
        }

        private Card PlayCardFrom(List<Card> hand)
        {
            Print(hand);
            Console.Write(" playable from: ");
            Print(this.FullHand());
            Console.WriteLine("\ntype the index of the card you would like to play (remember indices start at 0, please no negatives):");
            int index;
            string i;


            //index = ( (i = Console.ReadLine().Trim()).Length > 0 ? Convert.ToInt32(i) : 0);

            //while (( index = Convert.ToInt32(Console.ReadLine().Trim()) ) >= hand.Count)
            while (( index = ( (i = Console.ReadLine().Trim()).Length > 0 ? Convert.ToInt32(i) : 0) ) >= hand.Count )
            {
                Console.WriteLine("ERROR: type the index of the card you would like to play (remember indices start at 0, please no negatives):");
            }
            
            return hand[index];
        }
        
        public Card LastCardPlayed()
        {
            return this.lastCard;
        }

        //
        public void PrintHand()
        {
            /**/
            
            /** /
            List<Card> localHand = new List<Card>();
            localHand.AddRange(this.diamondCards);
            localHand.AddRange(this.clubCards);
            localHand.AddRange(this.heartCards);
            localHand.AddRange(this.spadeCards);
            /**/
            

            Print(this.FullHand());
            
            Console.WriteLine(": ~" + this.highCardPoints + " points");
            //this.PrintHCP();
        }

        /// <summary>
        /// displays the cards a player can play based off a suit
        /// </summary>
        /// <param name="suit">suit that the player must play if the player has a card with the same suit</param>
        public void PrintPlayableHand(cardSuit suit)
        {
            Print(FigureOutHandFrom(suit));
        }

        private static void Print(List<Card> hand)
        {
            Console.Write("[");
            for(int i = 0; i < hand.Count; i++) //Card thisCard in this.hand)
            {
                if(hand[i].FaceValue() != 10)
                {
                    Console.Write(" ");
                }
                
                Console.Write(hand[i].ToString());
                
                if(i != hand.Count - 1)
                {
                    Console.Write(", ");
                }

            }
            Console.Write(" ]");
            
        }

        private void PrintHCP()
        {
            //foreach(Card current in this.hand)

        }


        public Bid MakeBid()
        {
            this.PrintHand();
            
            string thisBid = "";
            Console.WriteLine("What is your bid?");
            thisBid += Console.ReadLine().Trim();

            if(thisBid.Length > 0)
            {
                
                switch(thisBid.Substring(0,1))
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                        this.playerBid = new Bid(false, Convert.ToInt32(thisBid.Substring(0,1)), thisBid.Substring(1).Trim());
                        return this.playerBid;
                        //break;
                    case "D":
                    case "d":
                        this.playerBid = new Bid(false, Convert.ToInt32(thisBid.Substring(0,1)), thisBid.Substring(1).Trim());
                        return this.playerBid;
                    default:
                        // pass
                        break;
                }
            }
            
            this.playerBid = new Bid();
            return this.playerBid;

            /** /
            Console.WriteLine("Would you like to PASS? (Y/N)");
            string passing = Console.ReadLine();

            // determines if the player is passing
            switch(passing)
            {
                case "Yes":
                case "YES":
                case "yes":
                case "Y":
                case "y":
                case "true":
                    this.playerBid = new Bid(true, 0, "");
                    return this.playerBid;
                    //break;
                default:
                    break;

            }
            
            Console.WriteLine("What would you like to bid? (# SUIT)");
            string bid = Console.ReadLine();
            int bidVal = Convert.ToInt32(bid.Substring(0,1));
            string suit = bid.Substring(1).Trim();


            this.playerBid = new Bid(false, bidVal, suit);
            Console.WriteLine(this.playerBid.ToString());
            return this.playerBid;
            /**/
        }
    

        public int UpdateScore(int morePoints)
        {
            this.score += morePoints;
            return this.score;
        }

        public int Score()
        {
            return this.score;
        }
    }




    /* NOT SURE IF THIS IS NECESSARY * /
    public class Partner
    {
        Player p1;

        Player p2;

        List<FirstToBidSuit> firstToBidSuit;

        

        public Partner(Player player1, Player player2)
        {
            this.p1 = player1;
            this.p2 = player2;
        }

        public bool Contains(Player player)
        {
            return this.p1 == player || this.p2 == player;
        }

        // Assumes that the player is part of the partner
        public void UpdateFirstBidderOfSuit(Player player, cardSuit suit)
        {
            this.firstToBidSuit.Add(new FirstToBidSuit(player, suit));
        }

        // Assumes that the partner got the bid
        // determines who will play the hand
        public Player FirstBidderOf(cardSuit suit)
        {
            foreach(FirstToBidSuit bid in this.firstToBidSuit) //list cannot be greater than 5 entries due to only having 5 biddable suits
            {
                if(bid.suit == suit)
                {
                    return bid.person;
                }
            }
            // will NEVER get here
            return null;
        }

        public void UpdateScore(int score)
        {
            this.p1.UpdateScore(score);
            this.p2.UpdateScore(score);
        }
    }

    struct FirstToBidSuit
    {
        public Player person {get;}

        public cardSuit suit {get;}

        public FirstToBidSuit(Player player, cardSuit suit)
        {
            this.person = player;
            this.suit = suit;
        }
    }

    /**/


}