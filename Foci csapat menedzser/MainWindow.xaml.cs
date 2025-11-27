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
        private Player currentPlayer;

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

                            foreach (var player in players)
                            {
                                if (player.ContractEnd < DateTime.Now)
                                {
                                    player.IsAvailable = false;
                                    if (string.IsNullOrEmpty(player.UnavailableReason))
                                    {
                                        player.UnavailableReason = "Lejárt szerződés";
                                    }
                                }
                            }

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
                currentPlayer = selectedPlayer;
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

            JoinedTeamPicker.SelectedDate = player.JoinedTeam;
            ContractEndPicker.SelectedDate = player.ContractEnd;

            string contractStatus = "Aktív";
            if (player.ContractEnd < DateTime.Now)
                contractStatus = "Lejárt";
            else if (player.ContractEnd < DateTime.Now.AddMonths(6))
                contractStatus = "Hamarosan lejár";
            ContractStatusText.Text = contractStatus;

            TimeSpan remaining = player.ContractEnd - DateTime.Now;
            ContractRemainingText.Text = remaining.TotalDays > 0 ? $"{(int)remaining.TotalDays} nap" : "Lejárt";

            AvailableCheckBox.IsChecked = player.IsAvailable;
            ReasonTextBox.Text = player.UnavailableReason ?? "";
            ReturnDatePicker.SelectedDate = player.ReturnDate;
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
                else if (filter == "Lejárt szerződések")
                {
                    filteredPlayers = players.Where(p => p.ContractEnd < DateTime.Now).ToList();
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

        private void SaveContract_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayer == null) return;

            if (JoinedTeamPicker.SelectedDate == null || ContractEndPicker.SelectedDate == null)
            {
                MessageBox.Show("Kérlek, add meg mindkét dátumot!");
                return;
            }

            if (ContractEndPicker.SelectedDate <= JoinedTeamPicker.SelectedDate)
            {
                MessageBox.Show("A szerződés vége nem lehet korábbi, mint a csatlakozás dátuma!");
                return;
            }

            currentPlayer.JoinedTeam = JoinedTeamPicker.SelectedDate.Value;
            currentPlayer.ContractEnd = ContractEndPicker.SelectedDate.Value;

            if (currentPlayer.ContractEnd < DateTime.Now)
            {
                currentPlayer.IsAvailable = false;
                if (string.IsNullOrEmpty(currentPlayer.UnavailableReason))
                {
                    currentPlayer.UnavailableReason = "Lejárt szerződés";
                }
            }

            PlayerList.Items.Refresh();
            UpdatePlayerDetails(currentPlayer);
            MessageBox.Show("Szerződés adatai frissítve!");
        }

        private void SaveAvailability_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayer == null) return;

            bool newAvailable = AvailableCheckBox.IsChecked == true;
            string newReason = ReasonTextBox.Text;
            DateTime? newReturnDate = ReturnDatePicker.SelectedDate;

            if (!newAvailable && string.IsNullOrWhiteSpace(newReason))
            {
                MessageBox.Show("Kérlek, add meg az okot, ha a játékos nem elérhető!");
                return;
            }

            if (!newAvailable && newReturnDate == null)
            {
                MessageBox.Show("Kérlek, add meg a várható visszatérés dátumát!");
                return;
            }

            currentPlayer.IsAvailable = newAvailable;
            currentPlayer.UnavailableReason = newReason;
            currentPlayer.ReturnDate = newReturnDate;

            PlayerList.Items.Refresh();
            UpdatePlayerDetails(currentPlayer);
            MessageBox.Show("Elérhetőség frissítve!");
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(players, options);
                File.WriteAllText(JsonFile, jsonString);
                MessageBox.Show("Minden változás sikeresen elmentve!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a mentéskor: {ex.Message}");
            }
        }
    }
}