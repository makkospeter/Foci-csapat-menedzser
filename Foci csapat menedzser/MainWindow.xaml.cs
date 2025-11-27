using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;

namespace Foci_csapat_menedzser
{
    public partial class MainWindow : Window
    {
        private List<Player> players = new List<Player>();
        private List<Player> filteredPlayers = new List<Player>();
        private const string JsonFile = "players.json";

        public MainWindow()
        {
            InitializeComponent();
         
        }

 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
                            players = playerArray.ToList();
                            filteredPlayers = players.ToList();

                            if (PlayerList != null)
                            {
                                PlayerList.ItemsSource = filteredPlayers;
                            }
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
            if (PlayerList != null && PlayerList.SelectedItem is Player selectedPlayer)
            {
                UpdatePlayerDetails(selectedPlayer);
            }
        }

        private void UpdatePlayerDetails(Player player)
        {
           
            PlayerNameText.Text = player.Name;
            PlayerPositionText.Text = player.Position;
            JerseyNumberText.Text = $"#{player.JerseyNumber}";

      
            int age = DateTime.Now.Year - player.BirthDate.Year;
            if (DateTime.Now.DayOfYear < player.BirthDate.DayOfYear)
                age--;
            AgeText.Text = $"{age} éves";

            MarketValueText.Text = $"{player.MarketValue:N0} €";

      
            if (player.IsAvailable)
            {
                AvailabilityStatusText.Text = "✅ Elérhető";
                AvailabilityStatusText.Foreground = System.Windows.Media.Brushes.Green;
                ReturnDateText.Text = "";
            }
            else
            {
                AvailabilityStatusText.Text = "❌ Nem elérhető";
                AvailabilityStatusText.Foreground = System.Windows.Media.Brushes.Red;
                ReturnDateText.Text = player.ReturnDate?.ToString("yyyy-MM-dd") ?? "Ismeretlen";
            }

        
            NationalitiesText.Text = player.Nationalities != null ? string.Join(", ", player.Nationalities) : "Nincs";
            BirthDateText.Text = player.BirthDate.ToString("yyyy-MM-dd");
            HeightText.Text = $"{player.Height} cm";
            PreferredFootText.Text = player.PreferredFoot;

            JoinedTeamText.Text = player.JoinedTeam.ToString("yyyy-MM-dd");
            ContractEndText.Text = player.ContractEnd.ToString("yyyy-MM-dd");

         
            string contractStatus = "Aktív";
            if (player.ContractEnd < DateTime.Now)
                contractStatus = "Lejárt";
            else if (player.ContractEnd < DateTime.Now.AddMonths(6))
                contractStatus = "Hamarosan lejár";
            ContractStatusText.Text = contractStatus;

        
            TimeSpan remaining = player.ContractEnd - DateTime.Now;
            ContractRemainingText.Text = $"{(int)remaining.TotalDays} nap";

     
            AvailabilityText.Text = player.IsAvailable ? "Elérhető" : "Nem elérhető";
            UnavailableReasonText.Text = !string.IsNullOrEmpty(player.UnavailableReason) ? player.UnavailableReason : "Nincs";
            ReturnDateDetailedText.Text = player.ReturnDate?.ToString("yyyy-MM-dd") ?? "Nincs";
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox != null && FilterComboBox.SelectedItem is ComboBoxItem item)
            {
                string filter = item.Content.ToString();

                if (filter == "Csak elérhetők")
                {
                    filteredPlayers = players.Where(p => p.IsAvailable).ToList();
                }
                else if (filter == "Csak sérültek")
                {
                    filteredPlayers = players.Where(p => !p.IsAvailable).ToList();
                }
                else
                {
                    filteredPlayers = players.ToList();
                }

                if (PlayerList != null)
                {
                    PlayerList.ItemsSource = null;
                    PlayerList.ItemsSource = filteredPlayers;
                }
            }
        }

        private void ShowStats_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerList != null && PlayerList.SelectedItem is Player player)
            {
                int age = DateTime.Now.Year - player.BirthDate.Year;
                if (DateTime.Now.DayOfYear < player.BirthDate.DayOfYear)
                    age--;

                string stats = $"{player.Name} statisztikái:\n\n" +
                              $"Életkor: {age} év\n" +
                              $"Magasság: {player.Height} cm\n" +
                              $"Mezszám: #{player.JerseyNumber}\n" +
                              $"Piaci érték: {player.MarketValue:N0} €\n" +
                              $"Nemzetiségek: {(player.Nationalities != null ? string.Join(", ", player.Nationalities) : "Nincs")}";

                MessageBox.Show(stats, "Játékos Statisztikák");
            }
        }

        private void ModifyContract_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerList != null && PlayerList.SelectedItem is Player player)
            {
                MessageBox.Show($"Szerződés módosítása: {player.Name}", "Szerződés Kezelés");
            }
        }

        private void ToggleAvailability_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerList != null && PlayerList.SelectedItem is Player player)
            {
                player.IsAvailable = !player.IsAvailable;
                if (!player.IsAvailable)
                {
                    player.UnavailableReason = "Kézi módosítás";
                    player.ReturnDate = DateTime.Now.AddDays(7);
                }
                else
                {
                    player.UnavailableReason = null;
                    player.ReturnDate = null;
                }

                PlayerList.Items.Refresh();
                UpdatePlayerDetails(player);
                MessageBox.Show($"{player.Name} elérhetősége frissítve!", "Sikeres módosítás");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(players, options);
                File.WriteAllText(JsonFile, jsonString);
                MessageBox.Show("Minden változás sikeresen elmentve!", "Sikeres mentés");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a mentéskor: {ex.Message}", "Hiba");
            }
        }
    }
}