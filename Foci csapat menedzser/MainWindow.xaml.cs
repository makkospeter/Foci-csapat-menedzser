using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Foci_csapat_menedzser
{
    public partial class MainWindow : Window
    {
        ArrayList players = new ArrayList();
        string JsonFile = "players.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadPlayers();
        }

        void LoadPlayers()
        {
            players.Clear();

            string text = File.ReadAllText(JsonFile);
            text = text.Replace("[", "").Replace("]", "");

            string[] blocks = text.Split(new string[] { "}," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string b in blocks)
            {
                string cleaned = b.Replace("{", "").Replace("}", "").Trim();
                string[] lines = cleaned.Split(',');

                Player p = new Player();

                foreach (string line in lines)
                {
                    string[] kv = line.Split(':');
                    string key = kv[0].Trim().Replace("\"", "");
                    string val = kv[1].Trim().Replace("\"", "");

                    if (key == "Name") p.Name = val;
                    if (key == "Age") p.Age = int.Parse(val);
                    if (key == "BirthDate") p.BirthDate = val;
                    if (key == "Nationality") p.Nationality = val;
                    if (key == "MarketValue") p.MarketValue = int.Parse(val);
                    if (key == "IsAvailable") p.IsAvailable = (val == "true");
                }

                players.Add(p);
            }

            PlayerList.ItemsSource = null;
            PlayerList.ItemsSource = players;
        }

        void PlayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Player p = (Player)PlayerList.SelectedItem;
            if (p == null) return;

            NameBox.Text = p.Name;
            AgeBox.Text = p.Age.ToString();
            BirthBox.Text = p.BirthDate;
            NationBox.Text = p.Nationality;
            ValueBox.Text = p.MarketValue.ToString();
            AvailableBox.IsChecked = p.IsAvailable;
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PlayerList_SelectionChanged(null, null);
        }

        void Modify_Click(object sender, RoutedEventArgs e)
        {
            Player p = (Player)PlayerList.SelectedItem;
            if (p == null) return;

            p.Name = NameBox.Text;
            p.Age = int.Parse(AgeBox.Text);
            p.BirthDate = BirthBox.Text;
            p.Nationality = NationBox.Text;
            p.MarketValue = int.Parse(ValueBox.Text);
            p.IsAvailable = AvailableBox.IsChecked == true;

            PlayerList.Items.Refresh();
        }

        void Ok_Click(object sender, RoutedEventArgs e)
        {
            SaveJson();
            MessageBox.Show("Változások mentve.");
        }

        void SaveJson()
        {
            string outText = "[\n";

            for (int i = 0; i < players.Count; i++)
            {
                Player p = (Player)players[i];
                outText += "  {\n";
                outText += $"    \"Name\": \"{p.Name}\",\n";
                outText += $"    \"Age\": {p.Age},\n";
                outText += $"    \"BirthDate\": \"{p.BirthDate}\",\n";
                outText += $"    \"Nationality\": \"{p.Nationality}\",\n";
                outText += $"    \"MarketValue\": {p.MarketValue},\n";
                outText += $"    \"IsAvailable\": {(p.IsAvailable ? "true" : "false")}\n";
                outText += (i == players.Count - 1) ? "  }\n" : "  },\n";
            }

            outText += "]";

            File.WriteAllText(JsonFile, outText);
        }
    }
}
