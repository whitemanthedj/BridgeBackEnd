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
                int netTricks = tricksTaken - finalContract.TricksNeeded();
                if(this.we.Contains(person))
                {
                    // we made their bid
                    if(netTricks >= 0)
                    {
                        we.addScore(netTricks, finalContract.TricksNeeded() - book, finalContract);
                    } else {
                        // we went down tricks
                        they.addScore(Math.Abs(netTricks), we.Vulnerable());
                    }
                } else {
                    // they made their bid
                    if(netTricks >= 0)
                    {
                        they.addScore(netTricks, finalContract.TricksNeeded() - book, finalContract);
                    } else {
                        // they went down tricks
                        we.addScore(Math.Abs(netTricks), we.Vulnerable());
                    }
                }
            }

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

        }

        public bool GotGame()
        {
            return below >= 100;
        }

        public void UpdateAboveLine()
        {
            this.belowLine.AddRange(this.aboveLine);
            this.aboveLine = this.belowLine;
            this.belowLine = new List<int>();
        }

        public void addScore(int tricksAbove, int tricksBelow, Bid bid)
        {
            this.addScore(tricksAbove, tricksBelow, bid, false, false);
        }

        public void addScore(int tricksAbove, int tricksBelow, Bid bid, bool doubled, bool redoubled)
        {
            int aPoints = 0, bPoints = 0;

            switch(bid.Suit())
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

            this.aboveLine.Add(aPoints);
            this.below += bPoints;
            this.belowLine.Add(bPoints);

            player1.UpdateScore(aPoints+bPoints);
            player2.UpdateScore(aPoints+bPoints);
        }

        public void addScore(int tricksWentDown, bool isOppenentVulnerable)
        {
            this.addScore(tricksWentDown, isOppenentVulnerable, false, false);

            
        }

        public void addScore(int tricksWentDown, bool isOppenentVulnerable, bool doubled, bool redoubled)
        {
            int points = (isOppenentVulnerable ? 50 : 100) * tricksWentDown ;
            
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