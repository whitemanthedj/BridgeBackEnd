using System;
using BridgeTricks;
using BridgeBid;
using BridgePlayer;
using DeckOfCards;

namespace BridgeRound
{
     /// <summary>
    /// refers to one hand or ROUND of bridge
    /// </summary>
    public class Hand
    {
        
        int nummaOfPlayers;
        int dealerIndex;

        Player[] players;

        Deck deck;

        Auction auction;
        int nummaOfTricks;

        int tricksTakenByBidWinners;

        Trick[] allTricks;

        /// <summary>
        /// refers to one hand or ROUND of bridge
        /// </summary>
        public Hand(int dealerIndex)
        {
            this.dealerIndex = dealerIndex;
            this.deck = new Deck();

            //this.NewHand(allPlayers);
            
        }

        /// <summary>
        /// initializes and runs a hand of bridge, returns the number of tricks taken by the winners of the bid
        /// </summary>
        /// <param name="allPlayers"></param>
        /// <returns>tricks taken by bid winners</returns>
        public int NewHand(Player[] allPlayers)
        {
            this.players = allPlayers;
            this.nummaOfPlayers = this.players.Length;
            this.tricksTakenByBidWinners = 0;

            if(this.deck.CardCount() == 0)
            {
                this.deck.Populate();
                foreach(Player p in this.players)
                {
                    p.NewHand();
                }
            }

            this.nummaOfTricks = deck.CardCount() / this.nummaOfPlayers;
            
            this.DealAllCards((this.dealerIndex + 1) % this.nummaOfPlayers);

            //this.Auction();

            if(this.PlayableContract())
            {
                // resets all of the tricks
                this.allTricks = new Trick[this.nummaOfTricks];

                this.PlayTricks();
            }

            // SETUP FOR ANOTHER HAND
            // updates the dealer for the next hand
            this.dealerIndex++;

            return this.tricksTakenByBidWinners;
            
        }

        private static bool CanConvertBidSuitToCardSuit(biddableSuits bidSuit)
        {
            switch(bidSuit)
            {
                case biddableSuits.C:
                case biddableSuits.D:
                case biddableSuits.H:
                case biddableSuits.S:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts the suit of the bid to the card suit. Assumes that the bid suit can be converted
        /// </summary>
        /// <param name="bidSuit">bid suit to change</param>
        /// <returns>the suit of the card</returns>
        private static cardSuit ConvertBidSuitToCardSuit(biddableSuits bidSuit)
        {
            cardSuit suit;
            switch(bidSuit)
            {
                case biddableSuits.C:
                    suit = cardSuit.C;
                    break;
                case biddableSuits.D:
                    suit = cardSuit.D;
                    break;
                case biddableSuits.H:
                    suit = cardSuit.H;
                    break;
                default:
                    suit = cardSuit.S;
                    break;
            }

            return suit;
        }

        public void PlayTricks()
        {
            // person to the left of the bidder starts
            int currentPlayerIndex = (this.auction.ContractPlayerIndex() + (this.nummaOfPlayers + 1)) % this.nummaOfPlayers;
            for(int i = 0; i < this.nummaOfTricks; i++)
            {
                Trick currentTrick = new Trick(this.nummaOfPlayers);

                //currentPlayerIndex %= this.nummaOfPlayers;
                for(int j = currentPlayerIndex; j < currentPlayerIndex + this.nummaOfPlayers; j++)
                {
                    currentTrick = this.NextCardInTrick(j % this.nummaOfPlayers, currentTrick);
                }

                currentPlayerIndex = currentTrick.TrickWinner();
                // increment the number of tricks taken if it was taken by the right team
                //this.tricksTakenByBidWinners = ( this.AuctionWinners(currentPlayerIndex) ? this.tricksTakenByBidWinners + 1 : this.tricksTakenByBidWinners );

                if(this.AuctionWinners(currentPlayerIndex))
                {
                    this.tricksTakenByBidWinners++;
                    Console.WriteLine("the winners of the bid have now gotten " + this.tricksTakenByBidWinners + " tricks.");
                }

                this.allTricks[i] = currentTrick;
                Console.WriteLine("End of trick " + (i+1) + ": Player " + (currentPlayerIndex+1) + " took the trick with: " + currentTrick.WinningTrick().ToString());
                currentTrick.PrintRecord();
            }
        }

        private bool AuctionWinners(int currentPlayerIndex)
        {
            return currentPlayerIndex % (this.nummaOfPlayers/2) == (this.auction.ContractPlayerIndex() % (this.nummaOfPlayers/2));
        }

        private Trick NextCardInTrick(int pIndex, Trick currentTrick)
        {
            Console.WriteLine("Player " + (pIndex+1) + ":");
            if(CanConvertBidSuitToCardSuit(this.auction.FinalContract().Suit()))
            {
                currentTrick.AddCard(this.players[pIndex].PlayCard(currentTrick.FirstCardPlayed()), ConvertBidSuitToCardSuit(this.auction.FinalContract().Suit()), pIndex);
            } else {
                // NO TRUMP
                currentTrick.AddCard(this.players[pIndex].PlayCard(currentTrick.FirstCardPlayed()), pIndex);
            }

            Console.WriteLine("Player " + (pIndex+1) + " played: " + this.players[pIndex].LastCardPlayed().ToString());
            Console.WriteLine("Player " + (currentTrick.TrickWinner()+1) + " will get the trick at the end with: " + currentTrick.WinningTrick().ToString());

            return currentTrick;
        }



        private bool PlayableContract()
        {
            this.auction = new Auction(nummaOfPlayers, this.dealerIndex);
            this.AuctionPhase(this.dealerIndex);
            this.auction.PrintRecord();
            this.auction.PrintContract();

            // returns if there is a playable contract
            return this.auction.FinalContract().Suit() != biddableSuits.PASS;
        }

        public Bid FinalContract()
        {
            return this.auction.FinalContract();
        }

        public int FinalContractPlayer()
        {
            return this.auction.ContractPlayerIndex();
        }

        

        public void AuctionPhase(int dealerIndex)
        {
            int playerIndex = dealerIndex, everyonesBid = 0;
            // short circuit
            while(everyonesBid < 4 || this.auction.KeepBidding())
            {
                int player = playerIndex % this.players.Length;
                Console.WriteLine("Player "+ (player+1) +":");
                
                while(!this.auction.UpdateBid( player, this.MakeBid(this.players[player]) ))
                {
                    Console.WriteLine("ERROR: Incorrect bid entered, please try again");
                    Console.WriteLine("Player "+ (player+1) +":");

                }
                playerIndex++;
                everyonesBid++;
            }
        }

        private Bid MakeBid(Player cardShark)
        {
            cardShark.PrintHand();
            
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
                        return new Bid(false, Convert.ToInt32(thisBid.Substring(0,1)), thisBid.Substring(1).Trim());
                        //break;
                    case "D":
                    case "d":
                        //this.auction.Double();
                        return new DoubleBid(false);
                    case "R":
                    case "r":
                        //this.auction.ReDouble();
                        return new DoubleBid(true);
                    default:
                        // pass
                        break;
                }
            }
            return new Bid();
        }

        
        private void DealAllCards(int indexOfDealer)
        {
            int playaNumma = indexOfDealer + 1; //start dealing with the left of the dealer ie, dealer gets the last card
            while(this.deck.CardCount() > 0)
            {
                this.players[playaNumma % nummaOfPlayers].AddCardToHand(this.deck.DealCard());
                playaNumma++;
            }
        }


        public void PrintNumberOfBiddersTricks()
        {
            Console.WriteLine("The bidders got " + this.tricksTakenByBidWinners + " tricks and needed " + this.auction.FinalContract().TricksNeeded());
        }

        public void PrintHand()
        {
            Console.WriteLine("PrintHand");
            for(int i=0; i < this.allTricks.Length; i++)
            {
                this.allTricks[i].PrintRecord();
                Console.WriteLine("At the end of trick " + (i+1) + ": Player " + (this.allTricks[i].TrickWinner()+1) + " took the trick with: " + this.allTricks[i].WinningTrick().ToString());
            }
        }
    }
}