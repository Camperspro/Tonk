using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tonk_2._0
{
    public class Deck
    {
        //52 draw card deck
        private List<Card> ndeck = new List<Card>();

        public Deck() 
        {
            //Our random gen deck should contain no copys
            do
            {
                Task.Delay(1000);
                Card card = new Card();
                card.Id = ndeck.Count;
                if (ndeck.Count <= 0)
                { ndeck.Add(card); }
                else if (!this.Dupe(card))
                {
                    ndeck.Add(card);
                }

            } while (ndeck.Count != 52) ;

        }

        public List<Card> deck { get { return this.ndeck; } set { this.ndeck = value; } }

        public void Add(Card card) { this.ndeck.Add(card); }

        public void Remove(Card card) { this.ndeck.Remove(card); }

        public void Clear() {  this.ndeck.Clear(); }

        public bool Dupe(Card card) 
        {
            bool result = false;
            for(int i = 0; i < ndeck.Count; i++) 
            {
                //same card check
                if(ndeck[i] == card)
                {result = true;}
                else if(ndeck[i].Id == card.Id)
                { result = true; }
                else if(ndeck[i].Suit == card.Suit && ndeck[i].Value == card.Value)
                {result = true;}
                else if(ndeck.Contains(card))
                { result = true; }
            }
            return result;
        }

        public bool Contains(Card card) { return this.ndeck.Contains(card); }

        public int IndexOf(Card card) { return this.ndeck.IndexOf(card); }

        public void ShuffleDeck()
        {
            List<Card> nDeck = new List<Card>();
            int oldmax = 64;
            do
            {
                Task.Delay(120);
                Random rnd = new Random();
                int nID = rnd.Next(0, oldmax);
                if (nDeck.Count != 64 && !nDeck.Contains(deck[nID]))
                {
                    Card copy = deck[nID];
                    nDeck.Add(copy);
                    this.deck.Remove(copy);
                    oldmax--;
                }

            } while (nDeck.Count != 64);

            for(int i = 0;i < 64; i++)
            {   nDeck[i].Id = i; }
            this.deck.Clear();
            this.deck = nDeck;
        }
    }
}
