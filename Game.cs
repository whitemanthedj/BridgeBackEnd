using System;
using System.Collections.Generic;
using DeckOfCards;
using BridgePlayer;
using BridgeBid;
using BridgeTricks;
using BridgeScoring;
using BridgeRound;


namespace BridgeGame
{
    public class BGame
    {
        int nummaOfPlayers;
        int dealerIndex;
        Player[] players;
        Deck deck;

        Hand currentHand;

        ScorePad scorepad;


        // CONSIDER A HAND/ROUND CLASS

        public BGame()
        {
            // DEFAULT NUMBER OF PLAYERS
            this.nummaOfPlayers = 4;
            this.dealerIndex = 0;
            this.deck = new Deck();

            this.currentHand = new Hand(this.dealerIndex);

            

            this.players = new Player[nummaOfPlayers];

            for(int i = 0; i < nummaOfPlayers; i++)
            {
                this.players[i] = new Player( (i), this.partnerAcrossTable(i) );
            }

            this.scorepad = new ScorePad(this.dealerIndex, this.players);


            this.scorepad.UpdateScores(this.currentHand.NewHand(this.players), this.currentHand.FinalContract(), this.players[this.currentHand.FinalContractPlayer()]);

            this.printAllPlayersScores();

            /** /
            this.nummaOfTricks = this.deck.CardCount() / this.nummaOfPlayers;

            /** /this.deck.PrintDeck();/** /
            this.DealAllCards(this.dealerIndex);

            //this.Auction();

            if(this.PlayableContract())
            {
                this.allTricks = new Trick[this.nummaOfTricks];

                this.PlayTricks();
            }
            /**/


            
        }


       
       

      
        

        public void printAllPlayers()
        {
            for(int i = 0; i < nummaOfPlayers; i++)
            {
                Console.WriteLine("Player " + (i+1) + ":");
                this.players[i].PrintHand();
            }
        }

        public void printAllPlayersScores()
        {
            for(int i = 0; i < nummaOfPlayers; i++)
            {
                Console.WriteLine("Player " + (i+1) + ": " + this.players[i].Score() + " points");
                
            }
        }

      
        //only works with even number of players
        private int partnerAcrossTable(int indexOfMe)
        {
            return (indexOfMe + nummaOfPlayers/2) % nummaOfPlayers;
        }
        
    }
}