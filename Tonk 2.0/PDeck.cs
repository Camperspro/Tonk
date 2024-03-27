using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Tonk_2._0
{
    public class PDeck : Deck //For Party Tonk Mode, adds 12 additonal cards to the deck that have special powers/effects.
    {
        //private List<Card> ndeck = new List<Card>();
        public PDeck() 
        {
            do
            {
                Task.Delay(120);
                sCard card = new sCard();
                card.Value = 0;
                if (!this.Dupe(card))
                {
                    card.Id = this.deck.Count;
                    this.deck.Add(card);
                }

            } while (this.deck.Count != 64);

            ShuffleDeck();
        }

        public bool Dupe(Card card)
        {
            bool result = false;
            for (int i = 0; i < this.deck.Count; i++)
            {
                if (sCard.Equals(card, this.deck[i]))
                {
                    if (this.deck[i] == card)
                    {
                        result = true;
                    }
                    else if (this.deck[i].Suit == card.Suit && this.deck[i].Value == card.Value)
                    {
                        result = true;
                    }
                }
                else 
                {
                    
                }

            }
            return result;
        }
    }
}
