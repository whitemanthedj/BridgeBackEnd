using System;
using System.Collections.Generic;
using DeckOfCards;
using BridgePlayer;
using BridgeBid;


namespace BridgeScoring
{
    
    public class ScorePad
    {

        PartnerScore we;
        PartnerScore they;

        const int book = 6;


        public ScorePad(int dealerIndex, Player[] allPlayers)
        {
            int nummaOfPlayers = allPlayers.Length;
            int acrossTable = nummaOfPlayers/2 + dealerIndex;
            this.we = new PartnerScore(allPlayers[dealerIndex], allPlayers[(acrossTable) % nummaOfPlayers]);
            this.they = new PartnerScore( allPlayers[(dealerIndex + 1 ) % nummaOfPlayers], allPlayers[(acrossTable + 1) % nummaOfPlayers]);
            
        }

        public void UpdateScores(int tricksTaken, Bid finalContract, Player person)
        {
            if(finalContract.Suit() != biddableSuits.PASS)
            {
                Console.WriteLine("The bidders got " + tricksTaken + " tricks and needed " + finalContract.TricksNeeded());
                int netTricks = tricksTaken - finalContract.TricksNeeded();
                if(this.we.Contains(person))
                {
                    // we made the bid
                    HandleScores(we, they, finalContract, netTricks);
                } else {
                    // they made the bid
                    HandleScores(they, we, finalContract, netTricks);
                }
            }

        }

        private static void HandleScores(PartnerScore bidders, PartnerScore opponent, Bid finalContract, int netTricks)
        {
            if(ContractWasMade(netTricks))
            {
                // bidders got enough triks
                bidders.addScore(netTricks, finalContract.TricksNeeded() - book, finalContract, bidders.Vulnerable());
            } else {
                // bidders went down tricks
                opponent.addScore(Math.Abs(netTricks), bidders.Vulnerable());
            }
            if(bidders.GotGame())
            {
                bidders.GetAGame();
            }
        }

        public static bool ContractWasMade(int netTricks)
        {
            return netTricks >= 0;
        }

        public bool RubberOver()
        {
            return this.we.GotRubber() || this.they.GotRubber();
        }

    }
    
    public class PartnerScore
    {
        //RECORDS FOR PRINTING
        List<int> aboveLine;
        List<int> belowLine;

        //VALUES FOR ADDING
        int below;
        //int aboveNotAdded;


        int nummaOfGames;

        Player player1;
        Player player2;

        const int book = 6;


        public PartnerScore(Player p1, Player p2)
        {
            this.player1 = p1;
            this.player2 = p2;

            this.aboveLine = new List<int>();
            this.belowLine = new List<int>();
            this.below = 0;

            this.nummaOfGames = 0;

        }

        public bool GotGame()
        {
            return below >= 100;
        }

        public void GetAGame()
        {
            this.nummaOfGames++;
        }

        public void UpdateAboveLine()
        {
            this.belowLine.AddRange(this.aboveLine);
            this.aboveLine = this.belowLine;
            this.belowLine = new List<int>();
        }

        public void addScore(int tricksAbove, int tricksBelow, Bid finalBid, bool biddersVulnerable)
        {
            int aPoints = 0, bPoints = 0;

            aPoints += IsSlam(tricksAbove+tricksBelow, biddersVulnerable);

            switch(finalBid.Suit())
            {
                // NT
                case biddableSuits.NT:
                    bPoints = 10;
                    goto case biddableSuits.H;
                // Meant to fall through
                case biddableSuits.H:
                case biddableSuits.S:
                    aPoints += 30 * tricksAbove;
                    bPoints += 30 * tricksBelow;
                    break;
                default:
                    aPoints += 20 * tricksAbove;
                    bPoints += 20 * tricksBelow;
                    break;
            }

            bPoints *= (finalBid.IsReDoubled() ? 4 : (finalBid.IsDoubled() ? 2 : 1));

            //for above teh line: OVERTRICKS
            int vulnerable = (biddersVulnerable ? 2 : 1);
            aPoints = vulnerable * (finalBid.IsReDoubled() ? 200 : (finalBid.IsDoubled() ? 100 : aPoints));

            this.aboveLine.Add(aPoints);
            this.below += bPoints;
            this.belowLine.Add(bPoints);

            player1.UpdateScore(aPoints+bPoints);
            player2.UpdateScore(aPoints+bPoints);
        }

        private int IsSlam(int totalTricks, bool vulnerable)
        {
            int SLAM = 6, GRANDSLAM = 7;
            if(totalTricks >= GRANDSLAM)
            {
                // GRAND SLAM
                return (vulnerable ? 1500 : 750);
            } else if(totalTricks == SLAM) {
                // SLAM
                return (vulnerable ? 1000 : 500);
            }
            return 0;
        }



        public void addScore(int underTricks, bool isOppenentVulnerable, Bid finalBid)
        {
            //for first trick: underTrick is >= 1
            int points =  (isOppenentVulnerable ? 2 : 1) * (finalBid.IsReDoubled() ? 4 : (finalBid.IsDoubled() ? 2 : 1)) * 50;
             
            if(underTricks > 1)
            {
                int crazyBids = (isOppenentVulnerable ? 3 : 4); /* different ratios then: 6 for vuln and 4 for not */
                points += (isOppenentVulnerable ? 2 : 1) * (finalBid.IsReDoubled() ? 2 * crazyBids : (finalBid.IsDoubled() ? crazyBids : 1)) * 50 * (underTricks - 1);
            }

            this.aboveLine.Add( points );

            player1.UpdateScore(points);
            player2.UpdateScore(points);
        }


        public bool Vulnerable()
        {
            return this.nummaOfGames >= 1;
        }

        public bool GotRubber()
        {
            return this.nummaOfGames >= 2;
        }

        public bool Contains(Player player)
        {
            
            return this.player1 == player || this.player2 == player;
        }
    }
}