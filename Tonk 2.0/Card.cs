using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tonk_2._0
{
    public class Card
    {
        private int value = 0; //Jack == 11, Queen == 12, King == 13. They're true values 
        private string suit = "";
        private int id = 0;

        public Card()
        {
            Random rndV = new Random();
            Random rndS = new Random();
            string[] suits = ["♣", "♦", "♥", "♠"];
            Value = rndV.Next(1, 14);
            Suit = suits[rndS.Next(0, 4)];
            Id = 0;
        }


        //Some Methods for the powerup card Gamemode
        public void sShuffle()
        {
            Random rndS = new Random();
            string[] suits = ["♣", "♦", "♥", "♠"];
            Suit = suits[rndS.Next(0, 4)];
        }

        public string Suit { get { return this.suit; } set { this.suit = value; } }
        public int Value { get { return this.value; } set { this.value = value; } }
        public int Id { get { return this.id; } set { this.id = value; } }


    }
}
