using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Tonk_2._0
{
    public class CPU : Player
    {
        public int difficulty = 0; //0 = Easy, 1 = Med, 2 = Hard
        public string spreadHunt = "Same"; //Same = hunts for same # diff suit spread, Suit = Chronological values but same suit
        public string suitSpread = string.Empty;
        public int valueSpread = 0;
        public int choice { get; set; } = 0;
        public int cC, sC, hC, dC = 0;

        public CPU() 
        {
            hand = new List<Card>();
            hand.Capacity = 5;
            spreads = new List<Card[]>();
            wins = 0;
            losses = 0;
            name = "CPU";
            title = "Diff: Easy";
        }

        public void checkHunt(Card card) //Add check for spreads and add them to safe if hitable
        {
            List<int> safe = new List<int>();
            switch (spreadHunt)
            {
                case "Same":
                    foreach(Card x in hand)
                    {
                        if(x.Value == card.Value && x.Suit != card.Suit)
                        {
                            safe.Add(hand.IndexOf(x));
                        }
                    }
                    break;
                case "Suit":
                    foreach (Card x in hand)
                    {
                        if (x.Suit == card.Suit)
                        {
                            if(x.Value == card.Value+1 || x.Value == card.Value - 1)
                            {
                                safe.Add(hand.IndexOf(x));
                            }
                        }
                    }
                    break;
            }
            Random remove = new Random();
            choice = remove.Next(0, 5);
            bool _break = true;
            do
            {
                Task.Delay(600);
                if (safe.Contains(choice))
                {
                    //re roll
                    choice = remove.Next(0, 5);
                }
                else if(choice > hand.Count())
                {
                    //re roll
                    choice = remove.Next(0, 5);
                }
                else
                {
                    _break = false;
                }

            } while (_break);
            
        }

        public bool checkGrave(Card card)
        {
            bool yes = false;
            switch (spreadHunt)
            {
                case "Same":
                    foreach (Card x in hand)
                    {
                        if (x.Value == card.Value)
                        {
                            yes = true;
                        }
                    }
                    break;
                case "Suit":
                    foreach (Card x in hand)
                    {
                        if (x.Suit == card.Suit)
                        {
                            if (x.Value == card.Value + 1 || x.Value == card.Value - 1)
                            {
                                yes = true;
                            }
                        }
                    }
                    break;
            }

            return yes;
        }

        public void updateHunt()
        {
            if (hand.Count >= 3)
            {
                List<Card> Vcontenders = new List<Card>();
                List<Card> Scontenders = new List<Card>();
                Scontenders.Add(hand[0]);
                Vcontenders.Add(hand[0]);
                for (int i = 1; i < hand.Count; i++)
                {
                   
                    foreach (Card check in hand) //Same Value Check
                    {
                        if (check.Value == hand[i].Value && check.Id != hand[i].Id)
                        {
                            if (!Vcontenders.Contains(check))
                            { Vcontenders.Add(check); }
                        }
                    }
               
                    foreach (Card check in hand) //Chronological Value Check
                    {
                        if (check.Suit == hand[i].Suit && check.Id != hand[i].Id)
                        {
                            if (check.Value == hand[i].Value + 1 && check.Id != hand[i].Id || check.Value == hand[i].Value - 1 && check.Id != hand[i].Id)
                            {
                                if (!Scontenders.Contains(check))
                                { Scontenders.Add(check);}
                            }
                        }
                    }

                }

                if (Vcontenders.Count > Scontenders.Count)
                {
                    spreadHunt = "Same";
                }
                else if(Vcontenders.Count < Scontenders.Count)
                {
                    spreadHunt = "Suit";
                }
                else
                {
                    spreadHunt = "Same";
                }

            }
        }
    }
}
