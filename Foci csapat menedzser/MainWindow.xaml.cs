using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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

            try
            {
                string flagPath = $"Assets/Flags/{player.FirstNationalityCode}.png";
                var converter = new ImageSourceConverter();
                PlayerFlagImage.Source = (ImageSource)converter.ConvertFromString(flagPath);
            }
            catch
            {
                PlayerFlagImage.Source = null;
            }

            JerseyNumberText.Text = $"#{player.JerseyNumber}";
            AgeText.Text = $"{player.Age} éves";
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

                if (player.ReturnDate.HasValue)
                {
                    ReturnDateText.Text = player.ReturnDate.Value.ToString("yyyy-MM-dd");
                }
                else
                {
                    ReturnDateText.Text = "Ismeretlen";
                }
            }

            if (player.Nationalities != null && player.Nationalities.Count > 0)
            {
                NationalitiesText.Text = string.Join(", ", player.Nationalities);
            }
            else
            {
                NationalitiesText.Text = "Nincs";
            }

            BirthDateText.Text = player.BirthDate.ToString("yyyy-MM-dd");
            HeightText.Text = $"{player.Height} cm";
            PreferredFootText.Text = player.PreferredFoot;

            JoinedTeamPicker.SelectedDate = player.JoinedTeam;
            ContractEndPicker.SelectedDate = player.ContractEnd;

            string contractStatus = "Aktív";

            if (player.ContractEnd < DateTime.Now)
            {
                contractStatus = "Lejárt";
            }
            else if (player.ContractEnd < DateTime.Now.AddMonths(6))
            {
                contractStatus = "Hamarosan lejár";
            }
            ContractStatusText.Text = contractStatus;

            TimeSpan remaining = player.ContractEnd - DateTime.Now;

            if (remaining.TotalDays > 0)
            {
                ContractRemainingText.Text = $"{(int)remaining.TotalDays} nap";
            }
            else
            {
                ContractRemainingText.Text = "Lejárt";
            }

            AvailableCheckBox.IsChecked = player.IsAvailable;

            if (!string.IsNullOrEmpty(player.UnavailableReason))
            {
                ReasonTextBox.Text = player.UnavailableReason;
            }
            else
            {
                ReasonTextBox.Text = "";
            }

            ReturnDatePicker.SelectedDate = player.ReturnDate;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox != null && FilterComboBox.SelectedItem is ComboBoxItem item)
            {
                string filter = item.Content.ToString();
                filteredPlayers = new List<Player>();

                if (filter == "Csak elérhetők")
                {
                    foreach (var player in players)
                    {
                        if (player.IsAvailable)
                        {
                            filteredPlayers.Add(player);
                        }
                    }
                }
                else if (filter == "Csak sérültek")
                {
                    foreach (var player in players)
                    {
                        if (!player.IsAvailable)
                        {
                            filteredPlayers.Add(player);
                        }
                    }
                }
                else if (filter == "Lejárt szerződések")
                {
                    foreach (var player in players)
                    {
                        if (player.ContractEnd < DateTime.Now)
                        {
                            filteredPlayers.Add(player);
                        }
                    }
                }
                else
                {
                    foreach (var player in players)
                    {
                        filteredPlayers.Add(player);
                    }
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

        private void AddPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            var editorWindow = new PlayerEditorWindow();
            editorWindow.Owner = this;

            if (editorWindow.ShowDialog() == true)
            {
                var newPlayer = editorWindow.EditedPlayer;
                newPlayer.JoinedTeam = DateTime.Now;
                newPlayer.ContractEnd = DateTime.Now.AddYears(3);

                players.Add(newPlayer);
                filteredPlayers.Add(newPlayer);

                RefreshPlayerList();
                SaveAllChanges();

                MessageBox.Show("Új játékos sikeresen hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayer == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy játékost a szerkesztéshez!", "Nincs kiválasztva", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editorWindow = new PlayerEditorWindow(currentPlayer);
            editorWindow.Owner = this;

            if (editorWindow.ShowDialog() == true)
            {
                var editedPlayer = editorWindow.EditedPlayer;

                var originalPlayer = players.FirstOrDefault(p => p == currentPlayer);
                if (originalPlayer != null)
                {
                    originalPlayer.Name = editedPlayer.Name;
                    originalPlayer.BirthDate = editedPlayer.BirthDate;
                    originalPlayer.Nationalities = editedPlayer.Nationalities;
                    originalPlayer.MarketValue = editedPlayer.MarketValue;
                    originalPlayer.IsAvailable = editedPlayer.IsAvailable;
                    originalPlayer.UnavailableReason = editedPlayer.UnavailableReason;
                    originalPlayer.ReturnDate = editedPlayer.ReturnDate;
                    originalPlayer.JerseyNumber = editedPlayer.JerseyNumber;
                    originalPlayer.Height = editedPlayer.Height;
                    originalPlayer.PreferredFoot = editedPlayer.PreferredFoot;
                    originalPlayer.Position = editedPlayer.Position;

                    RefreshPlayerList();
                    UpdatePlayerDetails(originalPlayer);
                    SaveAllChanges();

                    MessageBox.Show("Játékos adatai sikeresen frissítve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeletePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayer == null)
            {
                MessageBox.Show("Kérjük, válasszon ki egy játékost a törléshez!", "Nincs kiválasztva", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Biztosan törölni szeretné a(z) '{currentPlayer.Name}' játékost?",
                                       "Játékos törlése", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                players.Remove(currentPlayer);
                filteredPlayers.Remove(currentPlayer);
                currentPlayer = null;

                RefreshPlayerList();
                ClearPlayerDetails();
                SaveAllChanges();

                MessageBox.Show("Játékos sikeresen törölve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshPlayerList()
        {
            PlayerList.ItemsSource = null;
            PlayerList.ItemsSource = filteredPlayers;
        }

        private void ClearPlayerDetails()
        {
            PlayerNameText.Text = "";
            PlayerPositionText.Text = "";
            PlayerFlagImage.Source = null;
            JerseyNumberText.Text = "";
            AgeText.Text = "";
            MarketValueText.Text = "";
            AvailabilityStatusText.Text = "";
            ReturnDateText.Text = "";
            NationalitiesText.Text = "";
            BirthDateText.Text = "";
            HeightText.Text = "";
            PreferredFootText.Text = "";
            JoinedTeamPicker.SelectedDate = null;
            ContractEndPicker.SelectedDate = null;
            ContractStatusText.Text = "";
            ContractRemainingText.Text = "";
            AvailableCheckBox.IsChecked = true;
            ReasonTextBox.Text = "";
            ReturnDatePicker.SelectedDate = null;
        }

        private void SaveAllChanges()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(players, options);
                File.WriteAllText(JsonFile, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a mentéskor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}