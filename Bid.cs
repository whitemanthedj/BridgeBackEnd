using System;
using System.Collections.Generic;
using BridgePlayer;


namespace BridgeBid
{
    public enum biddableSuits { PASS, C, D, H, S, NT, DOUBLE, REDOUBLE}

    public class Auction
    {
        Bid[] allBids;

        /// <summary>
        /// Bid that will win the Auction if the remaining players pass out
        /// </summary>
        Bid maxBid;
        /// <summary>
        /// Person who will play the Bid
        /// </summary>
        int maxBidPlayerIndex;

        int dealerIndex;

        private delegate void doubling();

        /// <summary>
        /// running record of all the bids in the Auction
        /// </summary>
        List<Bid> record;
        //List<string> fullRecord;

        /// <summary>
        /// used to determine when the Auction ends
        /// </summary>
        int consecutivePasses;

        public Auction(int nummaOfPlayers, int dealerIndex)
        {
            this.allBids = new Bid[nummaOfPlayers];
            for(int i = 0; i < this.allBids.Length; i++)
            {
                this.allBids[i] = new Bid(true, 0, "");
            }
            this.maxBid = new Bid(true,0,"");
            this.record = new List<Bid>();
            //this.fullRecord = new List<string>();

            this.dealerIndex = dealerIndex;
        }

        /// <summary>
        /// determines if enough people have passed, indicating if the Auction hasn't ended
        /// </summary>
        /// <returns>if the Auction remains open</returns>
        public bool KeepBidding()
        {
            /** /
            int numberOfPasses = 0, numberOfLiveBids = 0;
            foreach(Bid currentBid in this.allBids)
            {
                if(currentBid.Pass())
                {
                    numberOfPasses++;
                } else {
                    numberOfLiveBids++;
                }
            } /**/

            // are the last 3 bids a pass
            return !(this.consecutivePasses >= 3);
        }

        // updates the bid for the the player and returns if the new bid was valid
        /// <summary>
        /// adds a new bid to the Auction
        /// </summary>
        /// <param name="playerIndex">Player index who made the bid</param>
        /// <param name="newBid">Bid the player made</param>
        /// <returns> returns if the bid was a valid bid</returns>
        public bool UpdateBid(int playerIndex, Bid newBid)
        {
            // if the bid is NOT a pass and if the new bid is not valid return false
            if(NotValidBid(playerIndex, newBid))
            {
                return false;
            }
            
            if(newBid.Suit() == biddableSuits.REDOUBLE)
            {
                this.HandleDouble(this.FinalContract().ReDouble);
            } else if(newBid.Suit() == biddableSuits.DOUBLE) {
                this.HandleDouble(this.FinalContract().Double);
            }
            

            this.consecutivePasses = (newBid.Pass()? this.consecutivePasses + 1 : 0);
            //Console.WriteLine(this.consecutivePasses);
            
            this.allBids[playerIndex] = newBid;
            // update max
            this.UpdateMaxBid(playerIndex, newBid);
            this.record.Add(newBid);
            //this.fullRecord.Add(newBid.ToString());
            return true;
        }

        public bool NotValidBid(int playerIndex, Bid newBid)
        {
            // new bid isn't greater than players previous and current max bid AND it isnt a pass
            if(newBid.GetBid() != (int) biddableSuits.PASS && (this.allBids[playerIndex].GreaterThan(newBid) || !newBid.GreaterThan(this.maxBid)))
            {
                return true;
            }

            //checking for double criteria
            if(newBid.Suit() == biddableSuits.DOUBLE && (this.maxBid.Suit() == biddableSuits.PASS || this.maxBid.IsDoubled() || Bidder(playerIndex)))
            {
               return true; 
            }

            if(newBid.Suit() == biddableSuits.REDOUBLE && (this.maxBid.IsReDoubled() || !this.maxBid.IsDoubled() || !Bidder(playerIndex)))
            {
               return true; 
            }
            
            return false;
        }

        
        /// <summary>
        /// updates the Contract
        /// </summary>
        /// <param name="playerIndex">player index who made the bid</param>
        /// <param name="newBid">Bid that will become the FinalContract if it surpasses the FinalContract</param>
        private void UpdateMaxBid(int playerIndex, Bid newBid)
        {
            this.maxBidPlayerIndex = (this.maxBid.GetBid() < newBid.GetBid() ? playerIndex : this.maxBidPlayerIndex);
            this.maxBid = (this.maxBid.GetBid() < newBid.GetBid() ? newBid : this.maxBid);
        }

        /// <summary>
        /// returns the player index who will play the final Contract 
        /// </summary>
        /// <returns>player index who will play the hand</returns>
        public int ContractPlayerIndex()
        {
            for(int i = 0; i < this.record.Count; i++)
            {
                Console.WriteLine(i + ":" + Bidder(i));
                if(this.record[i].Suit() == this.maxBid.Suit() && this.Bidder(i))
                {
                    return i % this.allBids.Length;
                }
            }
            return this.maxBidPlayerIndex;
        }

        private bool Bidder(int playerIndex)
        {
            return this.maxBidPlayerIndex % (this.allBids.Length/2) == playerIndex % (this.allBids.Length/2);
        }



        private void HandleDouble(doubling preformDouble)
        {
            preformDouble();
            this.consecutivePasses = 0;
            string doubleString = (this.FinalContract().IsReDoubled()  ? "REDOUBLE" : "DOUBLE");
            //this.fullRecord.Add(doubleString);
            Console.WriteLine(this.FinalContract() + " was " + doubleString +"D");
            //return truth;
        }

        public bool ContractDoubled() 
        {
            return this.FinalContract().IsDoubled();
        }

        public bool ContractReDoubled()
        {
            return this.FinalContract().IsReDoubled();
        }

        /// <summary>
        /// returns the FinalContract
        /// </summary>
        /// <returns>the greatest bid</returns>
        public Bid FinalContract()
        {
            return this.maxBid;
        }

        
        /// <summary>
        /// prints the FinalContract
        /// </summary>
        public void PrintContract()
        {
            Console.WriteLine("Player " + ((this.ContractPlayerIndex() % this.allBids.Length) +1) + ": " + this.maxBid.ToString());
        }


        /// <summary>
        /// Prints the entire Auction
        /// </summary>
        public void PrintRecord()
        {
            int player = this.dealerIndex;
            Console.WriteLine("------");
            
            
            /** /
            foreach(string currentBid in this.fullRecord)
            {
                Console.WriteLine("Player " + ((player % this.allBids.Length) + 1) + ": " + currentBid.ToString());
                player++;
            }
            /**/
            foreach(Bid currentBid in this.record)
            {
                Console.WriteLine("Player " + ((player % this.allBids.Length) + 1) + ": " + currentBid.ToString());
                player++;
            } /**/
            Console.WriteLine("------");
        }

    }

    public class Bid
    {
        
        protected bool passed;
        protected int bidValue;

        protected bool doubled;
        protected bool redoubled;

        protected const int book = 6;
       
        protected biddableSuits bidSuit;
        
        public Bid() : this(true, 0, "") //call the other constructor
        {
            
        }
        public Bid(bool pass, int value, string suit)
        {
            if(pass)
            {
                this.bidSuit = biddableSuits.PASS;
                this.passed = true;
            } 

            
            this.bidValue = value;
            //Console.WriteLine(value);
            //Console.WriteLine(this.bidValue);
            //this.bidSuit = suit;

            switch(suit)
            {
                case "Spades":
                case "SPADES":
                case "spades":
                case "S":
                case "s":
                    this.bidSuit = biddableSuits.S;
                    break;
                case "Hearts":
                case "HEARTS":
                case "hearts":
                case "H":
                case "h":
                    this.bidSuit = biddableSuits.H;
                    break;
                case "Diamonds":
                case "DIAMONDS":
                case "diamonds":
                case "D":
                case "d":
                    this.bidSuit = biddableSuits.D;
                    break;
                case "Clubs":
                case "CLUBS":
                case "clubs":
                case "C":
                case "c":
                    this.bidSuit = biddableSuits.C;
                    break;
                case "NT":
                case "nt":
                case "N":
                case "n":
                    this.bidSuit = biddableSuits.NT;
                    break;
                default:
                    this.bidSuit = biddableSuits.PASS;
                    this.passed = true;
                    break;
            }

            
            //Console.WriteLine(this.ToString());
        }

        public biddableSuits Suit()
        {
            return this.bidSuit;
        }

        public bool Pass()
        {
            return this.bidSuit == biddableSuits.PASS;
        }

        public void Double() 
        {
            this.doubled = true;
        }

        public void ReDouble()
        {
            this.redoubled = true;
        }
        
        public bool IsDoubled() 
        {
            return this.doubled;
        }

        public bool IsReDoubled()
        {
            return this.redoubled;
        }

        // returns the integer representation of a bid
        public int GetBid()
        {
            return (this.IsBidSuit() ? (this.bidValue * /* # of bids */ 5) + (int) this.bidSuit : 0);
        }

        protected bool IsBidSuit()
        {
            return this.bidSuit != biddableSuits.PASS && this.bidSuit != biddableSuits.DOUBLE && this.bidSuit != biddableSuits.REDOUBLE;
        }

        public int TricksNeeded()
        {
            return this.bidValue + book;
        }

        // ASSUMES THE BID IS NOT A PASS
        public bool GreaterThan(Bid newBid)
        {
            return this.GetBid() > newBid.GetBid();
        }

        public override string ToString()
        {
            //Console.WriteLine(this.bidValue);
            string theBid = "";
            switch(this.bidSuit)
            {
                case biddableSuits.PASS:
                case biddableSuits.DOUBLE:
                case biddableSuits.REDOUBLE:
                    theBid += "" + this.bidSuit;
                    break;
                default:
                    theBid += "" + this.bidValue + this.bidSuit;
                    break;

            }
            
            return theBid;
        }
    }

    public class DoubleBid : Bid //extends
    {
        public DoubleBid(bool redouble) : base()
        {
            this.bidSuit = (redouble ? biddableSuits.REDOUBLE : biddableSuits.DOUBLE);
        }
    }

}



