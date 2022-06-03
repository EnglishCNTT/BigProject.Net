using System;
using System.Collections.Generic;
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
using System.Windows.Threading;
using System.Data.SQLite;

namespace BigExcDotNet
{
    /// <summary>
    /// Interaction logic for Minesweeper.xaml
    /// </summary>
    public partial class Minesweeper : Window
    {
        public const string dbcon = "Data Source = H:\\Visual Code\\Big-Excercise-.Net\\rankdata.db";
        game currGame = new game();
        System.Windows.Threading.DispatcherTimer timer;
        

        public Minesweeper()
        {
            InitializeComponent();
            buildField();
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            time.Content = "0";

        }
        public void timerTick(object sender, EventArgs e)
        {

            int t = Convert.ToInt32(time.Content);
            t += 1;
            time.Content = t.ToString();
        }
        private void buildField()
        {
            RowDefinition row = new RowDefinition();
            ColumnDefinition col = new ColumnDefinition();
            row.Height = new GridLength(0, GridUnitType.Auto);
            mainGrid.RowDefinitions.Add(row); // make grid of needed dimension

            Image img = new Image();
            img.Source = new BitmapImage(new Uri("H:/Visual Code/Big-Excercise-.Net/BigExcDotNet/Icons/reset.png")); //place reset button
            StackPanel stackPnl = new StackPanel();
            stackPnl.Margin = new Thickness(2);
            stackPnl.Children.Add(img);
            Button resetButton = new Button();
            resetButton.Content = stackPnl;
            resetButton.Width = resetButton.Height = 23;
            resetButton.Tag = "Null";
            resetButton.Click += restartClick;
            resetButton.Name = "reset";
            Grid.SetRow(resetButton, 0);
            Grid.SetColumn(resetButton, currGame.colls / 2);
            mainGrid.Children.Add(resetButton);

            for (int i = 1; i < currGame.rows + 1; i++)
            {
                row = new RowDefinition();
                row.Height = new GridLength(0, GridUnitType.Auto);
                mainGrid.RowDefinitions.Add(row);
                for (int j = 0; j < currGame.colls; j++)
                {
                    if (i == 1)
                    {
                        col = new ColumnDefinition();
                        col.Width = new GridLength(0, GridUnitType.Auto);
                        mainGrid.ColumnDefinitions.Add(col);
                    }
                    Button button = new Button();
                    button.Width = button.Height = 23;
                    button.Click += leftClick;
                    button.PreviewMouseLeftButtonUp += leftClick;
                    button.PreviewMouseRightButtonUp += rightClick;
                    String name = "modsX" + (i - 1).ToString() + "X" + j.ToString(); // put button coordinates in its name
                    button.Name = name;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    mainGrid.Children.Add(button);
                }
            }
        }
        private void restartClick(object sender, RoutedEventArgs e)
        {
            string pressed = ((sender as Button) == null) ? (sender as MenuItem).Header.ToString() : (sender as Button).Name.ToString(); // read both button and menu item
            switch (pressed)
            {
                case "Easy":
                    currGame = new game(9, 9, 10);
                    break;
                case "Medium":
                    currGame = new game(15, 15, 40);
                    break;
                case "Hard":
                    currGame = new game(31, 16, 99);
                    break;
                default:
                    currGame = new game(currGame.colls, currGame.rows, currGame.mines);
                    break;
            }
            mainGrid.Children.Clear();
            mainGrid.RowDefinitions.Clear();
            mainGrid.ColumnDefinitions.Clear();
            buildField();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            time.Content = "0";
        }
        private void closeClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void leftClick(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            String[] strlist = clicked.Name.Split('X');
            if ((!currGame.mat[int.Parse(strlist[1])][int.Parse(strlist[2])].marked) && (!currGame.mat[int.Parse(strlist[1])][int.Parse(strlist[2])].opened)) // check if button should be pressable
            {
                clicked.IsEnabled = false;
                currGame.openCell(int.Parse(strlist[1]), int.Parse(strlist[2]));
                switch (currGame.mat[int.Parse(strlist[1])][int.Parse(strlist[2])].value)
                {
                    case -1: // end game if cell had bomb
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri("H:/Visual Code/Big-Excercise-.Net/BigExcDotNet/Icons/bomb.png"));
                        StackPanel stackPnl = new StackPanel();
                        stackPnl.Children.Add(img);
                        stackPnl.Background = Brushes.Red;
                        clicked.Content = stackPnl;
                        blockField();
                        timer.Stop();
                        break;
                    case 0: // clean nearest fields if cell was empty
                        for (int i = -1; i < 2; i++)
                            for (int j = -1; j < 2; j++)
                                if ((int.Parse(strlist[1]) + i >= 0) && (int.Parse(strlist[2]) + j >= 0) && (int.Parse(strlist[1]) + i < currGame.rows) && (int.Parse(strlist[2]) + j < currGame.colls))
                                {
                                    var child = System.Windows.LogicalTreeHelper.FindLogicalNode(clicked.Parent, "modsX" + (int.Parse(strlist[1]) + i).ToString() + "X" + (int.Parse(strlist[2]) + j).ToString());
                                    if ((child != null) && (!currGame.mat[int.Parse(strlist[1]) + i][int.Parse(strlist[2]) + j].opened))
                                        (child as Button).RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                                }
                        if (currGame.checkWin())
                            initiateWin(clicked);
                        break;
                    default: // reveal number if cell is safe
                        clicked.Content = currGame.mat[int.Parse(strlist[1])][int.Parse(strlist[2])].value;
                        if (currGame.checkWin())
                            initiateWin(clicked);
                        break;
                }
            }
        }
        private void rightClick(object sender, RoutedEventArgs e)
        {
            Button clicked = (Button)sender;
            String[] strlist = clicked.Name.Split('X');
            if (currGame.markCell(int.Parse(strlist[1]), int.Parse(strlist[2]))) // true if cell wasnt marked
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("H:/Visual Code/Big-Excercise-.Net/BigExcDotNet/Icons/redflag.png"));
                StackPanel stackPnl = new StackPanel();
                stackPnl.Children.Add(img);
                clicked.Content = stackPnl;
            }
            else
            {
                clicked.Content = null;
            }
            if (currGame.checkWin())
                initiateWin(clicked);
        }
        private void initiateWin(Button clicked) // sending button for easier grid finding
        {
            blockField();
            timer.Stop();
            var child = System.Windows.LogicalTreeHelper.FindLogicalNode(clicked.Parent, "reset");
            Image resetImg = new Image();
            resetImg.Source = new BitmapImage(new Uri("H:/Visual Code/Big-Excercise-.Net/BigExcDotNet/Icons/tada.png"));
            StackPanel resetStackPnl = new StackPanel();
            resetStackPnl.Margin = new Thickness(2);
            resetStackPnl.Children.Add(resetImg);
            (child as Button).Content = resetStackPnl;
            MessageBox.Show("Congratulations !!! With Time : " + time.Content);
            //INSERT a student to DB.
            SQLiteConnection conn = new SQLiteConnection(dbcon);
            conn.Open();
            SQLiteDataAdapter ad = new SQLiteDataAdapter();
            SQLiteCommand cmd = new SQLiteCommand();
            String str = "INSERT INTO Rank (TimeRecord) VALUES ('" + int.Parse((string)time.Content) + "')";
            cmd.CommandText = str;
            ad.SelectCommand = cmd;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
            conn.Close();
        }
        private void blockField() // ending game with blocking or revealing cells
        {
            for (int i = 0; i < currGame.rows; i++)
                for (int j = 0; j < currGame.colls; j++)
                    if (!currGame.mat[i][j].opened)
                    {
                        var button = System.Windows.LogicalTreeHelper.FindLogicalNode(this, "modsX" + i.ToString() + "X" + j.ToString());
                        switch (currGame.mat[i][j].value)
                        {
                            case -1:
                                if (currGame.mat[i][j].marked)
                                {
                                    Image img = new Image();
                                    img.Source = new BitmapImage(new Uri("H:/Visual Code/Big-Excercise-.Net/BigExcDotNet/Icons/bomb.png"));
                                    StackPanel stackPnl = new StackPanel();
                                    stackPnl.Children.Add(img);
                                    stackPnl.Background = Brushes.Green;
                                    (button as Button).Content = stackPnl;
                                    (button as Button).IsEnabled = false;
                                }
                                else
                                {
                                    Image img = new Image();
                                    img.Source = new BitmapImage(new Uri("H:/Visual Code/Big-Excercise-.Net/BigExcDotNet/Icons/bomb.png"));
                                    StackPanel stackPnl = new StackPanel();
                                    stackPnl.Children.Add(img);
                                    (button as Button).Content = stackPnl;
                                    (button as Button).IsEnabled = false;
                                };
                                break;
                            case 0:
                                if (currGame.mat[i][j].marked)
                                {
                                    (button as Button).Click -= leftClick;
                                    (button as Button).PreviewMouseLeftButtonUp -= leftClick;
                                    (button as Button).PreviewMouseRightButtonUp -= rightClick;
                                }
                                else
                                {
                                    (button as Button).RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                                };
                                break;
                            default:
                                (button as Button).Click -= leftClick;
                                (button as Button).PreviewMouseLeftButtonUp -= leftClick;
                                (button as Button).PreviewMouseRightButtonUp -= rightClick;
                                break;
                        }
                    }
        }
    }
}
