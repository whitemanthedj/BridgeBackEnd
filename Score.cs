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
                opponent.addScore(Math.Abs(netTricks), bidders.Vulnerable(), finalContract);
            }
            if(bidders.GotGame())
            {
                int increase = (bidders.BelowRecord().Count > opponent.BelowRecord().Count ? bidders.BelowRecord().Count: opponent.BelowRecord().Count);
                // Use that to increase the the count of both scores by ^
                bidders.GetAGame(increase);
                opponent.UpdateAboveLine(increase);
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

        
        public void PrintScoreCard()
        {
            int start = (we.AboveRecord().Count > they.AboveRecord().Count ? we.AboveRecord().Count : they.AboveRecord().Count);

            Console.WriteLine("WE   | THEY");
            Console.WriteLine("     |     ");

            for(int i = start - 1; i >= 0; i--)
            {
                string weString = (i < we.AboveRecord().Count ? we.AboveRecord()[i] : "");
                string theyString = (i < they.AboveRecord().Count ? they.AboveRecord()[i] : "");
                
                Console.WriteLine("{0, -5}|{1, -5}", weString, theyString);
            }


            int below = (we.BelowRecord().Count > they.BelowRecord().Count ? we.BelowRecord().Count : they.BelowRecord().Count);


            for(int i = 0; i < below; i++)
            {
                string weString = (i < we.BelowRecord().Count ? we.BelowRecord()[i] : "    ");
                string theyString = (i < they.BelowRecord().Count ? they.BelowRecord()[i] : "");
                
                Console.WriteLine("{0, -5}|{1, -5}", weString, theyString);
            }
        }
    }
    
    public class PartnerScore
    {
        //RECORDS FOR PRINTING
        List<string> aboveLine;
        List<string> belowLine;

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

            this.aboveLine = new List<string>();
            this.belowLine = new List<string>();
            this.aboveLine.Add("=====");
            this.below = 0;

            this.nummaOfGames = 0;

        }

        public bool GotGame()
        {
            return below >= 100;
        }

        public void GetAGame(int increaseAboveLine)
        {
            this.nummaOfGames++;
            this.UpdateAboveLine(increaseAboveLine);
        }
        

        public void UpdateAboveLine(int increaseAboveLine)
        {
            while(this.belowLine.Count < increaseAboveLine)
            {
                this.belowLine.Add("");
            }
            
            this.belowLine.AddRange(this.aboveLine);
            this.aboveLine = this.belowLine;
            this.belowLine = new List<string>();
            this.belowLine.Add("-----");
            this.below = 0;
        }

        public void GetARubber(bool opponentVulnerable)
        {
            int rubber = (opponentVulnerable ? 500 : 700);

            this.aboveLine.Add(rubber+"");
            player1.UpdateScore(rubber);
            player2.UpdateScore(rubber);
        }

        public void addScore(int overTricks, int tricksBelow, Bid finalBid, bool biddersVulnerable)
        {
            int aPoints = 0, bPoints = 0;

            aPoints += IsSlam(finalBid.TricksNeeded(), overTricks+tricksBelow, biddersVulnerable);

            switch(finalBid.Suit())
            {
                // NT
                case biddableSuits.NT:
                    bPoints = 10;
                    goto case biddableSuits.H;
                // Meant to fall through
                case biddableSuits.H:
                case biddableSuits.S:
                    aPoints += 30 * overTricks;
                    bPoints += 30 * tricksBelow;
                    break;
                default:
                    aPoints += 20 * overTricks;
                    bPoints += 20 * tricksBelow;
                    break;
            }

            bPoints *= (finalBid.IsReDoubled() ? 4 : (finalBid.IsDoubled() ? 2 : 1));

            //for above teh line: OVERTRICKS
            int vulnerable = (biddersVulnerable ? 2 : 1);
            aPoints = vulnerable * (finalBid.IsReDoubled() ? 200 * overTricks: (finalBid.IsDoubled() ? 100 * overTricks : aPoints));

            if(aPoints > 0)
            {
                this.aboveLine.Add(aPoints+"");
            }
            this.below += bPoints;
            this.belowLine.Add(bPoints+"");

            player1.UpdateScore(aPoints+bPoints);
            player2.UpdateScore(aPoints+bPoints);
        }

        private int IsSlam(int bidVal, int totalTricks, bool vulnerable)
        {
            int SLAM = 6, GRANDSLAM = 7;
            if(bidVal >= GRANDSLAM && totalTricks >= GRANDSLAM)
            {
                // GRAND SLAM
                return (vulnerable ? 1500 : 750);
            } else if(bidVal == SLAM && totalTricks == SLAM) {
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
                int crazyBids = (isOppenentVulnerable ? 3 : 4); /* different ratios then: 6 for vuln and 4 for not only for db or redb */
                points += (isOppenentVulnerable ? 2 : 1) * (finalBid.IsReDoubled() ? 2 * crazyBids : (finalBid.IsDoubled() ? crazyBids : 1)) * 50 * (underTricks - 1);
            }

            this.aboveLine.Add( points+"" );

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
    
        public List<string> AboveRecord()
        {
            return this.aboveLine;
        }

        public List<string> BelowRecord()
        {
            return this.belowLine;
        }
    }
}