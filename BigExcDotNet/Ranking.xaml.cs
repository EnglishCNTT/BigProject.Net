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
using System.Data.SQLite;
using System.Data;
using System.Configuration;

namespace BigExcDotNet
{
    /// <summary>
    /// Interaction logic for Ranking.xaml
    /// </summary>
    public partial class Ranking : Window
    {
        public const string dbcon = "Data Source = H:\\Visual Code\\Big-Excercise-.Net\\rankdata.db";
        public Ranking()
        {
            InitializeComponent();
            //LOAD data from database.
            SQLiteConnection conn = new SQLiteConnection(dbcon);
            loadGird();
            
        }
        public void loadGird()
        {
            try
            {
                //LOAD data from database.
                SQLiteConnection conn = new SQLiteConnection(dbcon);
                conn.Open();
                SQLiteDataAdapter ad = new SQLiteDataAdapter();
                SQLiteCommand cmd = new SQLiteCommand();
                String str = "SELECT TimeRecord FROM Rank ORDER By TimeRecord ASC LIMIT 10";
                cmd.CommandText = str;
                ad.SelectCommand = cmd;
                cmd.Connection = conn;
                DataSet ds = new DataSet();
                ad.Fill(ds);
                rank_table.ItemsSource = ds.Tables[0].DefaultView;
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
