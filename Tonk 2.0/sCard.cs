using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tonk_2._0
{
    internal class sCard : Card
    {
        //Peek - Allows player to peek at one random card in desired suit that is in the deck.
        //One Under - Allows player to grab the card under the most placed.
        //Shuffle - Reshuffle the deck & grave.
        //Clone - Clones a random card in your hand.
        //Swap - Switch one random card from your hand to anothers or to the deck.
        private string Value;
        public sCard()
        {
            Random rndV = new Random();
            Random rndS = new Random();
            string[] suits = ["♣", "♦", "♥", "♠"];
            string[] power = ["peek", "oneunder", "shuffle"];
            Value = power[rndV.Next(0, 3)];
            Suit = suits[rndS.Next(0, 4)];
            Id = 0;
        }
    }
}
