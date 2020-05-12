using System;
using System.Collections.Generic;



namespace DeckOfCards
{

    public enum cardSuit { D, C, H, S}

    public class Card
    {
        
        int value;
        cardSuit suit;
        string cardString;

        // tryin these out; not sure if needed
        //int inPlayersHand;

        public Card(int value, cardSuit suit)
        {
            this.value = value;
            this.suit = suit;
        }

        public int FaceValue()
        {
            return this.value + 2;
        }

        public cardSuit Suit()
        {
            return this.suit;
        }
        public int SortedValue()
        {
            return this.value + ((int) this.suit * Deck.cardsPerSuit);
        }

        
        public override string ToString()
        {
            if(cardString == null)
            {
                string cardVal = "";
                string coolSuit = "";

                switch(this.suit)
                {
                    case cardSuit.D:
                        coolSuit = "♦";
                        break;
                    case cardSuit.C:
                        coolSuit = "♣";
                        break;
                    case cardSuit.H:
                        coolSuit = "♥";
                        break;
                    default:
                        coolSuit = "♠";
                        break;
                }

                switch(this.FaceValue())
                {
                    case 14:
                        cardVal += "A";
                        break;
                    case 13:
                        cardVal += "K";
                        break;
                    case 12:
                        cardVal += "Q";
                        break;
                    case 11:
                        cardVal += "J";
                        break;
                    case 10:
                        cardVal = "10";
                        break;
                    default:
                        cardVal += "" + this.FaceValue();
                        break;
                } 
                
                cardString = cardVal + "" + coolSuit;
            }

            return cardString;
        }

    }



    class Deck
    {
        List<Card> theDeck;
        public const int nummaOfSuits = 4;
        public const int cardsPerSuit = 13;

        public Deck()
        {
            //nummaOfSuits = 4;
            //cardsPerSuit = 13;
            this.theDeck = new List<Card>();
            this.Populate();
        }

        public void Populate()
        {
            for(int i = 0; i < nummaOfSuits; i++)
            {
                for(int j = 0; j < cardsPerSuit; j++)
                {
                    this.theDeck.Add(new Card(j, (cardSuit) i));
                }
            }
        }

        public void PrintDeck()
        {
            for(int i = 0; i < nummaOfSuits; i++)
            {
                for(int j = 0; j < cardsPerSuit; j++)
                {
                    Console.Write(this.theDeck[i*cardsPerSuit+j].ToString()+",");
                }
                Console.WriteLine();
            }
        }

        public Card DealCard()
        {
            return this.DealRandomCard();
        }

        public Card DealRandomCard()
        {
            Random RNG = new Random();
            
            Card randoCard = this.theDeck[RNG.Next(this.theDeck.Count)];
            this.theDeck.Remove(randoCard);
            return randoCard;
        }

        public int CardCount()
        {
            return this.theDeck.Count;
        }
    }


}
