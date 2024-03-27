using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Tonk_2._0
{
    public class GameTable
    {
        public bool GameOver { get;  set; } = false;
        public bool Table { get; set; } = false;
        public bool Retry { get; set; } = false;
        public bool PTable { get; set; } = false;
        public bool Tutorial { get; private set; }
        public bool ReplaceSelect { get;  set; } //Our index for hands
        public bool Pause { get; private set; }
        public int Games { get; set; }
        public Deck deck { get; set; } = null;
        public PDeck Pdeck { get; set; } = null;
        public List<Card> grave { get; set; } = new List<Card>();
        public List<Player> Players { get; set; } = new List<Player>();

        public GameTable()
        { 
            deck = new Deck();
            Pdeck = new PDeck();
        }

        public void Deal() // Remake to take player array so it can take 1-4 players no trouble
        {
            //Player order is decided by coin toss.
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].hand.Count <= 0)
                {
                    for (int x = 0; i < 5; x++)
                    {
                        if (Players[i].hand.Count != 5)
                        {
                            Players[i].hand.Add(deck.deck.First());
                            deck.Remove(deck.deck.First());
                        }
                        else
                        {break;}
                    }
                }
            }
        }
        public void sDeal()
        {
            //Player order is decided by coin toss.
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].hand.Count <= 0)
                {
                    for (int x = 0; i < 5; x++)
                    {
                        if (Players[i].hand.Count != 5)
                        {
                            Players[i].hand.Add(Pdeck.deck.First());
                            Pdeck.Remove(Pdeck.deck.First());
                        }
                        else
                        { break; }
                    }
                }
            }
        }
        public void Draw(Player p)
        {
            Card temp = deck.deck.First();
            p.hand.Add(temp);
            deck.deck.Remove(temp);
        }
        public void PickUp(Player p)
        {
            Card temp = grave.Last();
            p.hand.Add(temp);
            grave.Remove(temp);
        }
        public void IsSpread(Player p)
        {
            bool ans = false;
            if (p.hand.Count >= 3)
            {
                for(int i = 0; i < p.hand.Count; i++)
                {
                    List<Card> Vcontenders = new List<Card>();
                    List<Card> Scontenders = new List<Card>();
                    Scontenders.Add(p.hand[i]);
                    Vcontenders.Add(p.hand[i]);

                    foreach (Card check in p.hand) //Same Value Check
                    {
                        if(check.Value == p.hand[i].Value && check.Id != p.hand[i].Id)
                        {
                            Vcontenders.Add(check);
                        }
                    }
                    if(Vcontenders.Count >= 3)
                    {
                        Card[] spread = new Card[Vcontenders.Count];
                        for (int y = 0; y < Vcontenders.Count; y++)
                        {
                            spread[y] = Vcontenders[y];
                            p.hand.Remove(Vcontenders[y]);
                        }
                        p.spreads.Add(spread);
                        break;
                    }

                    foreach (Card check in p.hand) //Chronological Value Check
                    {
                        if (check.Suit == p.hand[i].Suit && check.Id != p.hand[i].Id)
                        {
                            if(check.Value == p.hand[i].Value+1 && check.Id != p.hand[i].Id || check.Value == p.hand[i].Value - 1 && check.Id != p.hand[i].Id)
                            {
                                Scontenders.Add(check);
                            }
                        }
                    }

                    if (Scontenders.Count >= 3)
                    {
                        Card[] spread = new Card[Scontenders.Count];
                        for (int y = 0; y < Scontenders.Count; y++)
                        {
                            spread[y] = Scontenders[y];
                            p.hand.Remove(spread[y]);
                        }
                        var nHand = spread.OrderBy(x => x.Suit).ThenBy(x => x.Value);
                        spread = nHand.ToArray();
                        p.spreads.Add(spread);
                        break;
                    }
                }
                
            }
            
        }

        public bool isHit(Card card, int id)
        {
            bool ans = false;
            //check each player for spreads, if found confirm it is hitable.
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].spreads.Any() && Players[i].spreads.Count != 0)
                {
                   for(int j = 0; j < Players[i].spreads.Count; j++)
                   {
                        if(Players[i].spreads[j].Length > 0)
                        {
                            foreach (Card[] tmp in Players[i].spreads)
                            {
                                if (tmp.Contains(card))
                                {
                                    break;
                                }
                                else if (vCheck(tmp, card) && !tmp.Contains(card))
                                {
                                    int index = Players[i].spreads.IndexOf(tmp);
                                    ans = true;
                                    if (id != Players[i].id)
                                    {
                                        Players[i].spectate = true;
                                    }
                                    Card[] nSpread = new Card[tmp.Count() + 1];
                                    for (int x = 0; x < tmp.Length; x++)
                                    {
                                        nSpread[x] = Players[i].spreads[j][x];
                                    }

                                    nSpread[nSpread.Length - 1] = card;
                                    Players[i].spreads.Add(nSpread);
                                    Players[i].spreads.RemoveAt(index);
                                    break;
                                }
                                else if(sCheck(tmp, card) && !tmp.Contains(card))
                                {
                                    int index = Players[i].spreads.IndexOf(tmp);
                                    ans = true;
                                    if (id != Players[i].id)
                                    {
                                        Players[i].spectate = true;
                                    }
                                    Card[] nSpread = new Card[tmp.Count() + 1];
                                    var nHand = Players[i].spreads[j].OrderBy(x => x.Suit).ThenBy(x => x.Value);
                                    Card[] sortedH = nHand.ToArray();
                                    if (card.Value == sortedH[0].Value - 1) //front check
                                    {
                                        nSpread[0] = card;
                                        for (int x = 0; x < sortedH.Length; x++)
                                        {
                                            nSpread[x+1] = sortedH[x];
                                        }
                                        
                                    }
                                    else if (card.Value == sortedH[sortedH.Length - 1].Value + 1)
                                    {
                                        for (int x = 0; x < sortedH.Length; x++)
                                        {
                                            nSpread[x] = sortedH[x];
                                        }
                                        nSpread[nSpread.Length-1] = card;
                                    }
                                    Players[i].spreads.Add(nSpread.OrderBy(x => x.Suit).ThenBy(x => x.Value).ToArray());
                                    Players[i].spreads.RemoveAt(index);
                                    break;
                                }
                            }
                        }
                   }
                }
            }
            return ans;
        }

        public int Drop(Player p)
        {
            int total = 0;
            for (int i = 0; i < p.hand.Count; i++)
            {
                if (p.hand[i].Value > 10) { total = total + 10; }
                else { total = total + p.hand[i].Value; }
            
            }
            return total;
        }

        private bool sCheck(Card[] spread, Card card)
        {
            bool result = false;
            bool vspread = false;
            if (spread[0].Value == spread[1].Value)  //is value spread so dont bother checking
            { vspread = true; }

            if (!vspread)
            {
                if (!vCheck(spread, card))
                {
                    if (card.Value == spread[0].Value - 1 && card.Suit == spread[1].Suit && card.Id != spread[0].Id || card.Value == spread[spread.Length - 1].Value + 1 && card.Suit == spread[spread.Length - 2].Suit && card.Id != spread[spread.Length - 1].Id)
                    {
                        result = true;
                    }
                }
            }
            
            return result;
        }
        private bool vCheck(Card[] spread, Card card)
        {
            bool result = true;
            foreach(Card compare in spread)
            {
                if(card.Value != compare.Value || card.Suit == compare.Suit)
                {
                    result = false;
                }
            }
           
            return result;
        }

    }
}
