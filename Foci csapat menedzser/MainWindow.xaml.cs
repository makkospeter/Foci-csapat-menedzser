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

    }
}
