using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tonk_2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //CREATE MULTIPLE PANELS THAT OPEN FOR EACH STATE SUCH AS MAIN MENU & GAMETYPE, NAME & PLAYER COUNT & TYPE, THE GAME, THEN GAME OVER.
        //For actual P2 etc, have the other player look away from the screen when the live player turn is next lol.
        //If no P1, then have a cpu vs cpu play out but make it wait after each full turn so it can be watched at a fast but not too fast speed.
        //Use buttons instead of key inputs lmao
        public GameTable gameTable { get; set; } = null;
        public Player player1 { get; set; } = new Player();
        public CPU cpu1 { get; set; } = new CPU();

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //await GameLoop();
        }

        private void mmButton_Click(object sender, RoutedEventArgs e)
        {
            gameTable = new GameTable();
            player1.name = plyInput.Text;
            player1.id = 0;
            gameTable.Players.Add(player1);
            player1.id = 1;
            gameTable.Players.Add(cpu1);

            gameTable.deck = new Deck();
            gameTable.Table = true;
            gameTable.Deal();
            Card x = gameTable.deck.deck.First();
            gameTable.deck.Remove(x);
            gameTable.grave.Add(x);
            TableWindow NGtableWindow = new TableWindow(gameTable);
            this.Close();
            NGtableWindow.Show();
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            HelpMenu.Visibility = Visibility.Hidden;
            MenuContent.Visibility = Visibility.Visible;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MenuContent.Visibility = Visibility.Hidden;
            HelpMenu.Visibility = Visibility.Visible;
        }
    }
}