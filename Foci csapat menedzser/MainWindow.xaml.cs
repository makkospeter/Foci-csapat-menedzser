using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;

namespace Foci_csapat_menedzser
{
    public partial class MainWindow : Window
    {
        private List<Player> players = new List<Player>();
        private const string JsonFile = "players.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadPlayers();
        }

        private void LoadPlayers()
        {
            try
            {
                if (File.Exists(JsonFile))
                {
                    string jsonString = File.ReadAllText(JsonFile);
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var playerArray = JsonSerializer.Deserialize<Player[]>(jsonString, options);
                        if (playerArray != null)
                        {
                            players = new List<Player>(playerArray);
                            PlayerList.ItemsSource = players;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("A players.json fájl nem található!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a játékosok betöltésekor: {ex.Message}");
            }
        }

        private void PlayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayerList.SelectedItem is Player selectedPlayer)
            {
                NameBox.Text = selectedPlayer.Name;
                AgeBox.Text = selectedPlayer.Age.ToString();
                BirthBox.Text = selectedPlayer.BirthDate;
                NationBox.Text = selectedPlayer.Nationality;
                ValueBox.Text = selectedPlayer.MarketValue.ToString();
                AvailableBox.IsChecked = selectedPlayer.IsAvailable;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Visszaállítja az eredeti értékeket
            PlayerList_SelectionChanged(null, null);
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerList.SelectedItem is Player selectedPlayer)
            {
                try
                {
                    // Adatok frissítése
                    selectedPlayer.Name = NameBox.Text;
                    selectedPlayer.Age = int.Parse(AgeBox.Text);
                    selectedPlayer.BirthDate = BirthBox.Text;
                    selectedPlayer.Nationality = NationBox.Text;
                    selectedPlayer.MarketValue = int.Parse(ValueBox.Text);
                    selectedPlayer.IsAvailable = AvailableBox.IsChecked ?? false;

                    // Lista frissítése
                    PlayerList.Items.Refresh();
                    MessageBox.Show("Játékos adatai frissítve!");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Kérlek, érvényes számokat adj meg az Életkor és Piaci érték mezőkben!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Kérlek, válassz ki egy játékost!");
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SavePlayers();
                MessageBox.Show("Változások mentve!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a mentéskor: {ex.Message}");
            }
        }

        private void SavePlayers()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string jsonString = JsonSerializer.Serialize(players, options);
            File.WriteAllText(JsonFile, jsonString);
        }
    }
}