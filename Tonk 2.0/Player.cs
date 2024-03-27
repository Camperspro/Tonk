using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tonk_2._0
{
    public class Player
    {
        public int wins { get;  set; }
        public int losses { get;  set; }
        public int id { get; set; }
        public string name { get; set; } = "Player";
        public string title { get; set; } = "Noob";
        public bool spectate { get; set; } = false;
        public List<Card> hand { get; set; } = new List<Card>();
        public List<Card[]> spreads { get; set; } = new List<Card[]>();

        public Player() 
        {
            hand = new List<Card>();
            hand.Capacity = 5;
            spreads = new List<Card[]>();
            wins = 0;
            losses = 0;
        }

        public Card Replace(int choice)
        {
            Card drop = null;
            if (choice > hand.Count || choice > 5 || choice < 0)
            {
                if(hand.Count > 0)
                {
                    choice = 0;
                    drop = hand[choice];
                    hand.Remove(hand[choice]);
                }
                else
                {
                    choice = 0;
                }
            }
            else if(choice == hand.Count)
            {
                if(hand.Count > 0)
                {
                    drop = hand[choice - 1];
                    hand.Remove(hand[choice - 1]);
                }
                else
                {
                    drop = hand[0];
                    hand.Remove(hand[0]);
                }
            }
            else
            {
                if (hand.Count > 0)
                {
                    drop = hand[choice];
                    hand.Remove(hand[choice]);
                }
                else
                {
                    drop = hand[0];
                    hand.Remove(hand[0]);
                }
            }
           
            return drop;
        }

        public string Emote(int index, string[] pack)
        {
            string emote = string.Empty;
            emote = pack[index];
            return emote;
        }
    }
}
