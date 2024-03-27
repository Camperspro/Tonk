using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace Tonk_2._0
{
    /// <summary>
    /// Interaction logic for TableWindow.xaml
    /// </summary>
    public partial class TableWindow : Window
    {
        GameTable gameTable = null;
        Player curPly = null;
        bool noPlayers = false;
        public int curChoice = 0;
        public List<Card> cCards { get; set; } = new List<Card>();
        public List<Card> sCards { get; set; } = new List<Card>();
        public List<Card> hCards { get; set; } = new List<Card>();
        public List<Card> dCards { get; set; } = new List<Card>();
        public List<Card[]> allSpreads { get; set; } = new List<Card[]> { };
        public string newGamelog { get; set; } = "";
        public string prevGamelog { get; set; } = "";
        public string Gamelog { get; set; } = "";

        int cap = 1;

        public TableWindow()
        {
            gameTable = new GameTable();
            if (gameTable.Table)
            { gameTable.deck = new Deck(); }
            else {  gameTable.deck = new PDeck(); }
            gameTable.GameOver = false;
            foreach (Player ply in gameTable.Players) 
            { gameTable.IsSpread(ply);}
            Random startPly = new Random();
            curPly = gameTable.Players[startPly.Next(gameTable.Players.Count)];
            
            InitializeComponent();
            UpdateTable();
        }

        public TableWindow(GameTable table)
        {
            gameTable = table;
            gameTable.GameOver = false;
            foreach (Player ply in gameTable.Players)
            { gameTable.IsSpread(ply); }
            Random startPly = new Random();
            curPly = gameTable.Players[startPly.Next(gameTable.Players.Count)];

            InitializeComponent();
            UpdateTable();
        }

        private BitmapImage findImage(Card card)
        {
            BitmapImage found = null;
            switch (card.Suit)
            {
                case "♣":
                    found = new BitmapImage(new Uri("Assets/" + card.Value + "_c.png", UriKind.Relative));
                    break;

                case "♦":
                    found = new BitmapImage(new Uri("Assets/" + card.Value + "_d.png", UriKind.Relative));
                    break;
                case "♥":
                    found = new BitmapImage(new Uri("Assets/" + card.Value + "_h.png", UriKind.Relative));
                    break;
                case "♠":
                    found = new BitmapImage(new Uri("Assets/" + card.Value + "_s.png", UriKind.Relative));
                    break;
            }
            return found;
        }

        private void updateSpreads() // Updates all current spreads. Was setup for Multiple CPU Players.
        {
            allSpreads = null;
            allSpreads = new List<Card[]>();
            foreach (Player ply in gameTable.Players)
            {
                if(ply.spreads.Count > 0)
                {
                    for(int i = 0; i < ply.spreads.Count; i++)
                    {
                        if (!allSpreads.Contains(ply.spreads[i]))
                        {
                            allSpreads.Add(ply.spreads[i]);
                        }
                    }
                }
            }
        }
        private void UpdateTable()
        {
            //Task.Delay(3000);
           
            if (gameTable.deck != null && gameTable.deck.deck.Count <= 0)
            {
                Player winner = null;
                List<int> drop = new List<int>();
                for (int i = 0; i < gameTable.Players.Count; i++)
                {
                    drop.Add(gameTable.Drop(gameTable.Players[i]));
                }
                drop.Order();
                for (int i = 0; i < gameTable.Players.Count; i++)
                {
                    if (gameTable.Drop(gameTable.Players[i]) == drop.First())
                    {
                        winner = gameTable.Players[i];
                        break;
                    }
                }
                gameTable.GameOver = true;
                prevGamelog = "Everyone drops...";
                newGamelog = "\n" + winner.name + " wins with a total of: " + drop.First() + "!";
                gameTable.GameOver = true;
                UpdateTable();
            }
            else if(gameTable.Pdeck != null && gameTable.Pdeck.deck.Count <= 0)
            {
                Player winner = null;
                List<int> drop = new List<int>();
                for (int i = 0; i < gameTable.Players.Count; i++)
                {
                    drop.Add(gameTable.Drop(gameTable.Players[i]));
                }
                drop.Order();
                for (int i = 0; i < gameTable.Players.Count; i++)
                {
                    if (gameTable.Drop(gameTable.Players[i]) == drop.First())
                    {
                        winner = gameTable.Players[i];
                        break;
                    }
                }
                gameTable.GameOver = true;
                prevGamelog = "Everyone drops...";
                newGamelog = "\n" + winner.name + " wins with a total of: " + drop.First() + "!";
                gameTable.GameOver = true;
                UpdateTable();
            }

            if (curPly.hand.Count <= 0)
            {
                gameTable.GameOver = true;
                newGamelog = "\n" + curPly.name + " hand is empty and wins!";
                Gamelog = prevGamelog + " " + newGamelog;
            }

            if (gameTable.GameOver) //GameOver State
            {
                Gamelog = prevGamelog + " " + newGamelog;
                FinalText.Text = Gamelog;
                GameOverMenu.Visibility = Visibility.Visible;
                deckHint.Visibility = Visibility.Hidden;
                graveHint.Visibility = Visibility.Hidden;
                return;
            }
            else
            {
                Gamelog = prevGamelog + " " + newGamelog;
                GameTotal.Content = "Games: " + gameTable.Games;
                WinTotal.Content = gameTable.Players[0].name + "'s Wins: " + gameTable.Players[0].wins;
                deckHint.Visibility = Visibility.Visible;
                graveHint.Visibility = Visibility.Visible;
                logBlock.Text = Gamelog;
            }

            updateSpreads();
            //Rotate curPly
            for (int i = gameTable.Players.IndexOf(curPly); i < gameTable.Players.Count; i++)
            {
                if (gameTable.ReplaceSelect || gameTable.Retry)
                {
                    if(gameTable.Retry)
                    { gameTable.Retry = false; }
                    break;
                }

                if (i == gameTable.Players.Count-1)
                {
                    curPly = gameTable.Players[0];
                    if (curPly.spectate)
                    {
                        curPly.spectate = false;
                        newGamelog = "\n →" + curPly.name + " skipped.";
                        UpdateTable();
                    }
                    break;
                }
                else
                {
                    curPly = gameTable.Players[i + 1];
                    if (curPly.spectate)
                    {
                        curPly.spectate = false;
                        newGamelog = "\n →" + curPly.name + " skipped.";
                        UpdateTable();
                    }
                    break;
                }
            }

            switch (allSpreads.Count) //SPREAD UI
            {
                case 0:

                    break; 
                case 1:
                    tsc1.Source = findImage(allSpreads[0][0]);
                    tsc2.Source = findImage(allSpreads[0][1]);
                    tsc3.Source = findImage(allSpreads[0][2]);
                    if (allSpreads[0].Length >= 4)
                    {
                        ts1e.Visibility = Visibility.Visible;
                        int x = allSpreads[0].Length - 3;
                        ts1e.Text = "+" + x;
                    }
                    break; 
                case 2:
                    tsc1.Source = findImage(allSpreads[0][0]);
                    tsc2.Source = findImage(allSpreads[0][1]);
                    tsc3.Source = findImage(allSpreads[0][2]);
                    if (allSpreads[0].Length >= 4)
                    {
                        ts1e.Visibility = Visibility.Visible;
                        int x = allSpreads[0].Length - 3;
                        ts1e.Text = "+" + x;
                    }
                    tsc4.Source = findImage(allSpreads[1][0]);
                    tsc5.Source = findImage(allSpreads[1][1]);
                    tsc6.Source = findImage(allSpreads[1][2]);
                    if (allSpreads[1].Length >= 4)
                    {
                        ts2e.Visibility = Visibility.Visible;
                        int y = allSpreads[1].Length - 3;
                        ts2e.Text = "+" + y;
                    }
                    break;
                case 3:
                    tsc1.Source = findImage(allSpreads[0][0]);
                    tsc2.Source = findImage(allSpreads[0][1]);
                    tsc3.Source = findImage(allSpreads[0][2]);
                    if (allSpreads[0].Length >= 4)
                    {
                        ts1e.Visibility = Visibility.Visible;
                        int x = allSpreads[0].Length - 3;
                        ts1e.Text = "+" + x;
                    }
                    tsc4.Source = findImage(allSpreads[1][0]);
                    tsc5.Source = findImage(allSpreads[1][1]);
                    tsc6.Source = findImage(allSpreads[1][2]);
                    if (allSpreads[1].Length >= 4)
                    {
                        ts2e.Visibility = Visibility.Visible;
                        int y = allSpreads[1].Length - 3;
                        ts2e.Text = "+" + y;
                    }
                    tsc7.Source = findImage(allSpreads[2][0]);
                    tsc8.Source = findImage(allSpreads[2][1]);
                    tsc9.Source = findImage(allSpreads[2][2]);
                    if (allSpreads[2].Length >= 4)
                    {
                        ts3e.Visibility = Visibility.Visible;
                        int z = allSpreads[2].Length - 3;
                        ts3e.Text = "+" + z;
                    }
                    break;
                case >= 4:
                    extraSpreads.Visibility= Visibility.Visible;
                    extraSpreads.Content = allSpreads[3].Length;
                    break;
            }

            if (gameTable.grave.Count > 0)
            {
                GraveCard.Source = findImage(gameTable.grave.Last());
            }

            DeckTotal.Content = gameTable.deck.deck.Count;
            CPU1Cards.Content = gameTable.Players[1].hand.Count;

            //CPU TURN
            if (curPly.GetType() == typeof(CPU))
            {
                CPU cPU = (CPU)curPly;
                cPU.updateHunt();
                if(cPU.hand.Count == 1)
                {
                    if (gameTable.isHit(cPU.hand[0], cPU.id))
                    {
                        prevGamelog = "\n →" + curPly.name + " has hit.";
                        cPU.hand.Remove(cPU.hand[0]);
                    }
                    else
                    {
                        Random dropping = new Random();
                        if(dropping.Next(0,50) <= 50)
                        {
                            Task.Delay(5000);
                            gameTable.Drop(cPU);
                        }
                    }
                }
                else if (cPU.checkGrave(gameTable.grave.Last()))
                {
                    string cardV = gameTable.grave.Last().Value.ToString();
                    if (gameTable.grave.Last().Value == 11) { cardV = "J"; }
                    else if (gameTable.grave.Last().Value == 12) { cardV = "Q"; }
                    else if (gameTable.grave.Last().Value == 13) { cardV = "K"; }
                    prevGamelog = curPly.name + " picked up " + cardV + " of " + gameTable.grave.Last().Suit;
                    gameTable.PickUp(cPU);
                    Task.Delay(5000);
                    gameTable.IsSpread(cPU);
                    if (cPU.hand.Count <= 0)
                    {
                        gameTable.GameOver = true;
                        prevGamelog = "\n then " + curPly.name + " spreads and wins!";
                        curPly.wins++;
                        UpdateTable();
                    }
                    else
                    {
                        cPU.checkHunt(cPU.hand.Last());
                        for (int i = 0; i < cPU.hand.Count; i++)
                        {
                            if (gameTable.isHit(cPU.hand[i], cPU.id))
                            {
                                prevGamelog = "\n →" + curPly.name + " has hit.";
                                cPU.hand.Remove(cPU.hand[i]);
                                Task.Delay(5000);
                            }
                        }
                    }
                    gameTable.grave.Add(cPU.Replace(cPU.choice));
                    string cardV2 = gameTable.grave.Last().Value.ToString();
                    if (gameTable.grave.Last().Value == 11) { cardV2 = "J"; }
                    else if (gameTable.grave.Last().Value == 12) { cardV2 = "Q"; }
                    else if (gameTable.grave.Last().Value == 13) { cardV2 = "K"; }
                    newGamelog = "\n →" + curPly.name + " then placed " + cardV2 + " of " + gameTable.grave.Last().Suit;
                }
                else
                {
                    gameTable.Draw(cPU);
                    Task.Delay(5000);
                    gameTable.IsSpread(cPU);
                    if (cPU.hand.Count <= 0)
                    {
                        gameTable.GameOver = true;
                        prevGamelog = "\n then " + curPly.name + " spreads and wins!";
                        curPly.wins++;
                        UpdateTable();
                    }
                    else
                    {
                        cPU.checkHunt(cPU.hand.Last());
                        for(int i = 0; i < cPU.hand.Count; i++)
                        {
                            if (gameTable.isHit(cPU.hand[i], cPU.id))
                            {
                                prevGamelog = "\n →" + curPly.name + " has hit.";
                                cPU.hand.Remove(cPU.hand[i]);
                            }
                        }
                        
                    }

                    gameTable.grave.Add(cPU.Replace(cPU.choice));
                    string cardV = gameTable.grave.Last().Value.ToString();
                    if (gameTable.grave.Last().Value == 11) { cardV = "J"; }
                    else if (gameTable.grave.Last().Value == 12) { cardV = "Q"; }
                    else if (gameTable.grave.Last().Value == 13) { cardV = "K"; }
                    newGamelog = "\n →" + curPly.name + " drawed then placed a " + cardV + " of " + gameTable.grave.Last().Suit;
                }

                if (cPU.hand.Count <= 0)
                {
                    gameTable.GameOver = true;
                    prevGamelog = "\n" + curPly.name + " spreads and wins!";
                    curPly.wins++;
                    UpdateTable();
                }

                cPU.updateHunt();
                updateSpreads();
                UpdateTable();
            }

            //Hand Display
            switch (gameTable.Players[0].hand.Count)
            {
                case 0:
                    Card0.Visibility = Visibility.Hidden;
                    Card1.Visibility = Visibility.Hidden;
                    Card2.Visibility = Visibility.Hidden;
                    Card3.Visibility = Visibility.Hidden;
                    Card4.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    Card0.Visibility = Visibility.Visible;
                    Card0.Source = findImage(gameTable.Players[0].hand[0]);
                    Card1.Visibility = Visibility.Hidden;
                    Card2.Visibility = Visibility.Hidden;
                    Card3.Visibility = Visibility.Hidden;
                    Card4.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    Card0.Visibility = Visibility.Visible;
                    Card0.Source = findImage(gameTable.Players[0].hand[0]);
                    Card1.Visibility = Visibility.Visible;
                    Card1.Source = findImage(gameTable.Players[0].hand[1]);
                    Card2.Visibility = Visibility.Hidden;
                    Card3.Visibility = Visibility.Hidden;
                    Card4.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    Card0.Visibility = Visibility.Visible;
                    Card0.Source = findImage(gameTable.Players[0].hand[0]);
                    Card1.Visibility = Visibility.Visible;
                    Card1.Source = findImage(gameTable.Players[0].hand[1]);
                    Card2.Visibility = Visibility.Visible;
                    Card2.Source = findImage(gameTable.Players[0].hand[2]);
                    Card3.Visibility = Visibility.Hidden;
                    Card4.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    Card0.Visibility = Visibility.Visible;
                    Card0.Source = findImage(gameTable.Players[0].hand[0]);
                    Card1.Visibility = Visibility.Visible;
                    Card1.Source = findImage(gameTable.Players[0].hand[1]);
                    Card2.Visibility = Visibility.Visible;
                    Card2.Source = findImage(gameTable.Players[0].hand[2]);
                    Card3.Visibility = Visibility.Visible;
                    Card3.Source = findImage(gameTable.Players[0].hand[3]);
                    Card4.Visibility = Visibility.Hidden;
                    break;
                case 5:
                    Card0.Visibility = Visibility.Visible;
                    Card0.Source = findImage(gameTable.Players[0].hand[0]);
                    Card1.Visibility = Visibility.Visible;
                    Card1.Source = findImage(gameTable.Players[0].hand[1]);
                    Card2.Visibility = Visibility.Visible;
                    Card2.Source = findImage(gameTable.Players[0].hand[2]);
                    Card3.Visibility = Visibility.Visible;
                    Card3.Source = findImage(gameTable.Players[0].hand[3]);
                    Card4.Visibility = Visibility.Visible;
                    Card4.Source = findImage(gameTable.Players[0].hand[4]);
                    break;
            }
            
            if (gameTable.ReplaceSelect)
            {
                switch (gameTable.Players[0].hand.Count)
                {
                    case 1:
                        cshint.Visibility = Visibility.Visible;
                        ssButton.Visibility = Visibility.Hidden;
                        vsButton.Visibility = Visibility.Hidden;
                        cs1.Visibility = Visibility.Visible;
                        break; 
                    case 2:
                        cshint.Visibility = Visibility.Visible;
                        ssButton.Visibility = Visibility.Hidden;
                        vsButton.Visibility = Visibility.Hidden;
                        cs1.Visibility = Visibility.Visible;
                        cs2.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        cshint.Visibility = Visibility.Visible;
                        ssButton.Visibility = Visibility.Hidden;
                        vsButton.Visibility = Visibility.Hidden;
                        cs1.Visibility = Visibility.Visible;
                        cs2.Visibility = Visibility.Visible;
                        cs3.Visibility = Visibility.Visible;
                        break; 
                    case 4:
                        cshint.Visibility = Visibility.Visible;
                        ssButton.Visibility = Visibility.Hidden;
                        vsButton.Visibility = Visibility.Hidden;
                        cs1.Visibility = Visibility.Visible;
                        cs2.Visibility = Visibility.Visible;
                        cs3.Visibility = Visibility.Visible;
                        cs4.Visibility = Visibility.Visible;
                        break;
                    case 5:
                        cshint.Visibility = Visibility.Visible;
                        ssButton.Visibility = Visibility.Hidden;
                        vsButton.Visibility = Visibility.Hidden;
                        cs1.Visibility = Visibility.Visible;
                        cs2.Visibility = Visibility.Visible;
                        cs3.Visibility = Visibility.Visible;
                        cs4.Visibility = Visibility.Visible;
                        cs5.Visibility = Visibility.Visible;
                        break;

                    case 6:
                        newHint.Visibility = Visibility.Visible;
                        cshint.Visibility = Visibility.Visible;
                        ssButton.Visibility = Visibility.Hidden;
                        vsButton.Visibility = Visibility.Hidden;
                        cs1.Visibility = Visibility.Visible;
                        cs2.Visibility = Visibility.Visible;
                        cs3.Visibility = Visibility.Visible;
                        cs4.Visibility = Visibility.Visible;
                        cs5.Visibility = Visibility.Visible;
                        cs6.Visibility = Visibility.Visible;
                        GrabbedCard.Visibility = Visibility.Visible;
                        GrabbedCard.Source = findImage(gameTable.Players[0].hand[5]);
                        break;
                }

                drawButton.Visibility = Visibility.Hidden;
                pickupButton.Visibility = Visibility.Hidden;
                hitButton.Visibility = Visibility.Hidden;
                dropButton.Visibility = Visibility.Hidden;
                gameTable.ReplaceSelect = false;
            }
            else
            {
                newHint.Visibility = Visibility.Hidden;
                cshint.Visibility = Visibility.Hidden;
                ssButton.Visibility = Visibility.Visible;
                vsButton.Visibility = Visibility.Visible;
                cs1.Visibility = Visibility.Hidden;
                cs2.Visibility = Visibility.Hidden;
                cs3.Visibility = Visibility.Hidden;
                cs4.Visibility = Visibility.Hidden;
                cs5.Visibility = Visibility.Hidden;
                cs6.Visibility = Visibility.Hidden;
                GrabbedCard.Visibility = Visibility.Hidden;
                drawButton.Visibility = Visibility.Visible;
                pickupButton.Visibility = Visibility.Visible;
                hitButton.Visibility = Visibility.Visible;
                dropButton.Visibility = Visibility.Visible;
            }
            prevGamelog = string.Empty;
        }

        private void drawButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameTable.deck.deck.Count > 0 && curPly.hand.Count <= 5)
            {
                gameTable.Draw(curPly);
                newGamelog = "\n →" + " drawed " + curPly.hand.Last().Value + " of " + curPly.hand.Last().Suit;
                gameTable.IsSpread(curPly);
                if (curPly.hand.Count <= 0)
                {
                    gameTable.GameOver = true;
                    prevGamelog = "\n then "  + curPly.name + " spreads and wins!";
                    curPly.wins++;
                    UpdateTable();
                }
                else if (gameTable.isHit(curPly.hand.Last(), curPly.id))
                {
                    curPly.hand.RemoveAt(curPly.hand.IndexOf(curPly.hand.Last()));
                    prevGamelog = "\n →" + "you hit a spread.";
                }
                gameTable.ReplaceSelect = true;
            }
            UpdateTable();
        }

        private void pickupButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameTable.grave.Count > 0 && curPly.hand.Count <= 5)
            {
                newGamelog = "\n →" + "picked up " + gameTable.grave.Last().Value + " of " + gameTable.grave.Last().Suit;
                gameTable.PickUp(curPly);
                gameTable.IsSpread(curPly);
                if (curPly.hand.Count <= 0)
                {
                    gameTable.GameOver = true;
                    prevGamelog = "\n then " + curPly.name + " spreads and wins!";
                    curPly.wins++;
                    UpdateTable();
                }
                else if (gameTable.isHit(curPly.hand.Last(), curPly.id))
                {
                    curPly.hand.RemoveAt(curPly.hand.IndexOf(curPly.hand.Last()));
                    prevGamelog = "\n →" + "you hit a spread.";
                }
                gameTable.ReplaceSelect = true;
            }
            UpdateTable();
        }

        private void hitButton_Click(object sender, RoutedEventArgs e)
        {
            bool pass = false;
            for (int i = 0; i < curPly.hand.Count; i++)
            {
                if (gameTable.isHit(curPly.hand[i], curPly.id))
                {
                    curPly.hand.RemoveAt(i);
                    prevGamelog = "\n →" + "you hit a spread.";
                    pass = true;
                }
            }
            if (curPly.hand.Count <= 0)
            {
                gameTable.GameOver = true;
                newGamelog = "\n then " + curPly.name + " hits and wins!";
                curPly.wins++;
                UpdateTable();
            }

            if (pass) //have them remove a card from hand
            {
                gameTable.ReplaceSelect = true;
            }
            else
            {
                //Failed to find anything to hit, retry selection
                newGamelog = "\n →" + "no one to hit, select another option.";
                gameTable.Retry = true;
            }
            UpdateTable();
        }

        private void dropButton_Click(object sender, RoutedEventArgs e)
        {
            Player winner = null;
            List<int> drop = new List<int>();
            for(int i = 0;i < gameTable.Players.Count;i++)
            {
                drop.Add(gameTable.Drop(gameTable.Players[i]));
            }
            drop.Order();
            for (int i = 0; i < gameTable.Players.Count; i++)
            {
                if (gameTable.Drop(gameTable.Players[i]) == drop.First())
                {
                    winner = gameTable.Players[i];
                    winner.wins++;
                    break;
                }
            }
            gameTable.GameOver = true;
            prevGamelog = prevGamelog + " & everyone drops thier hand...";
            newGamelog = "\n" + winner.name + " wins with a total of: " + drop.First() + "!";
            UpdateTable();
        }

        //Replace Buttons
        private void cs1_Click(object sender, RoutedEventArgs e)
        {
            curChoice = 0;
            gameTable.grave.Add(curPly.Replace(curChoice));
            string cardV = gameTable.grave.Last().Value.ToString();
            if (gameTable.grave.Last().Value == 11) { cardV = "J"; }
            else if (gameTable.grave.Last().Value == 12) { cardV = "Q"; }
            else if (gameTable.grave.Last().Value == 13) { cardV = "K"; }
            newGamelog = "\n →" + "placed " + cardV + " of " + gameTable.grave.Last().Suit;
            gameTable.ReplaceSelect = false;
            UpdateTable();
        }
        private void cs2_Click(object sender, RoutedEventArgs e)
        {
            curChoice = 1;
            gameTable.grave.Add(curPly.Replace(curChoice));
            string cardV = gameTable.grave.Last().Value.ToString();
            if (gameTable.grave.Last().Value == 11) { cardV = "J"; }
            else if (gameTable.grave.Last().Value == 12) { cardV = "Q"; }
            else if (gameTable.grave.Last().Value == 13) { cardV = "K"; }
            newGamelog = "\n →" + "placed " + cardV + " of " + gameTable.grave.Last().Suit;
            gameTable.ReplaceSelect = false;
            UpdateTable();
        }
        private void cs3_Click(object sender, RoutedEventArgs e)
        {
            curChoice = 2;
            gameTable.grave.Add(curPly.Replace(curChoice));
            string cardV = gameTable.grave.Last().Value.ToString();
            if (gameTable.grave.Last().Value == 11) { cardV = "J"; }
            else if (gameTable.grave.Last().Value == 12) { cardV = "Q"; }
            else if (gameTable.grave.Last().Value == 13) { cardV = "K"; }
            newGamelog = "\n →" + "placed " + cardV + " of " + gameTable.grave.Last().Suit;
            gameTable.ReplaceSelect = false;
            UpdateTable();
        }
        private void cs4_Click(object sender, RoutedEventArgs e)
        {
            curChoice = 3;
            gameTable.grave.Add(curPly.Replace(curChoice));
            string cardV = gameTable.grave.Last().Value.ToString();
            if (gameTable.grave.Last().Value == 11) { cardV = "J"; }
            else if (gameTable.grave.Last().Value == 12) { cardV = "Q"; }
            else if (gameTable.grave.Last().Value == 13) { cardV = "K"; }
            newGamelog = "\n →" + "placed " + cardV + " of " + gameTable.grave.Last().Suit;
            gameTable.ReplaceSelect = false;
            UpdateTable();
        }
        private void cs5_Click(object sender, RoutedEventArgs e)
        {
            curChoice = 4;
            gameTable.grave.Add(curPly.Replace(curChoice));
            string cardV = gameTable.grave.Last().Value.ToString();
            if (gameTable.grave.Last().Value == 11) { cardV = "J"; }
            else if (gameTable.grave.Last().Value == 12) { cardV = "Q"; }
            else if (gameTable.grave.Last().Value == 13) { cardV = "K"; }
            newGamelog = "\n →" + "placed " + cardV + " of " + gameTable.grave.Last().Suit;
            gameTable.ReplaceSelect = false;
            UpdateTable();
        }
        private void cs6_Click(object sender, RoutedEventArgs e)
        {
            curChoice = 5;
            gameTable.grave.Add(curPly.Replace(curChoice));
            string cardV = gameTable.grave.Last().Value.ToString();
            if (gameTable.grave.Last().Value == 11) {cardV = "J";}
            else if (gameTable.grave.Last().Value == 12) { cardV = "Q";}
            else if (gameTable.grave.Last().Value == 13) {cardV = "K";}
            newGamelog = "\n →" + "placed " + cardV + " of " + gameTable.grave.Last().Suit;
            gameTable.ReplaceSelect = false;
            UpdateTable();
        }

        //Sort Buttons
        private void SS_Click(object sender, RoutedEventArgs e)
        {
            var nHand = curPly.hand.OrderBy(x => x.Suit).ThenBy(x => x.Value);
            curPly.hand = nHand.ToList();
            gameTable.Retry = true;
            UpdateTable();
        }
        private void VS_Click(object sender, RoutedEventArgs e)
        {
            var nHand = curPly.hand.OrderBy(x => x.Value);
            curPly.hand = nHand.ToList();
            gameTable.Retry = true;
            UpdateTable();
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            allSpreads = new List<Card[]>();
            gameTable.Games = gameTable.Games +1;
            if (gameTable.Table)
            {
                gameTable.deck = new Deck();
                gameTable.grave = new List<Card>();
                foreach (Player ply in gameTable.Players)
                {ply.hand = null; ply.hand = new List<Card>(); ply.spreads = new List<Card[]>(); }
                gameTable.Deal();
                Card x = gameTable.deck.deck.First();
                gameTable.deck.Remove(x);
                gameTable.grave.Add(x);
                foreach (Player ply in gameTable.Players)
                {  gameTable.IsSpread(ply); }
                Random startPly = new Random();
                curPly = gameTable.Players[startPly.Next(gameTable.Players.Count)];
                gameTable.ReplaceSelect = false;
                gameTable.Retry = false;
                gameTable.GameOver = false;
            }
            this.Card0.Source = null;
            this.Card1.Source = null;
            this.Card2.Source = null;
            this.Card3.Source = null;
            this.Card4.Source = null;
            this.tsc1.Source = null;
            this.tsc2.Source = null;
            this.tsc3.Source = null;
            this.tsc4.Source = null;
            this.tsc5.Source = null;
            this.tsc6.Source = null;
            this.tsc7.Source = null;
            this.tsc8.Source = null;
            this.tsc9.Source = null;
            Gamelog = string.Empty;
            TableWindow NGtableWindow = new TableWindow(gameTable);
            this.Close();
            NGtableWindow.Show();
        }
        private void Mainmenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow NMWindow = new MainWindow();
            this.Close();
            NMWindow.Show();
        }
    }
}
