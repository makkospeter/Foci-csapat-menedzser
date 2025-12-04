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
        public List<Player> players = new List<Player>();
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

        public void LoadPlayers()
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

            JoinedTeamText.Text = player.JoinedTeam.ToString("yyyy-MM-dd");
            ContractEndText.Text = player.ContractEnd.ToString("yyyy-MM-dd");

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

            if (player.IsAvailable)
            {
                AvailabilityText.Text = "Elérhető";
            }
            else
            {
                AvailabilityText.Text = "Nem elérhető";
            }

            if (!string.IsNullOrEmpty(player.UnavailableReason))
            {
                UnavailableReasonText.Text = player.UnavailableReason;
            }
            else
            {
                UnavailableReasonText.Text = "Nincs";
            }

            if (player.ReturnDate.HasValue)
            {
                ReturnDateDetailedText.Text = player.ReturnDate.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                ReturnDateDetailedText.Text = "Nincs";
            }
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

        private void AddPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            var editorWindow = new PlayerEditorWindow(null, this);
            editorWindow.Owner = this;

            if (editorWindow.ShowDialog() == true)
            {
                var newPlayer = editorWindow.EditedPlayer;
                players.Add(newPlayer);
                filteredPlayers.Add(newPlayer);

                RefreshPlayerList();
                SaveAllChanges();

                MessageBox.Show("Új játékos sikeresen hozzáadva!");
            }
        }

        private void EditPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayer == null)
            {
                MessageBox.Show("Válasszon ki egy játékost a szerkesztéshez!");
                return;
            }

            var editorWindow = new PlayerEditorWindow(currentPlayer, this);
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
                    originalPlayer.JoinedTeam = editedPlayer.JoinedTeam;
                    originalPlayer.ContractEnd = editedPlayer.ContractEnd;

                    RefreshPlayerList();
                    UpdatePlayerDetails(originalPlayer);
                    SaveAllChanges();

                    MessageBox.Show("Játékos adatai sikeresen frissítve!");
                }
            }
        }

        private void DeletePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPlayer == null)
            {
                MessageBox.Show("Válasszon ki egy játékost a törléshez!");
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

                MessageBox.Show("Játékos sikeresen törölve!");
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
            JoinedTeamText.Text = "";
            ContractEndText.Text = "";
            ContractStatusText.Text = "";
            ContractRemainingText.Text = "";
            AvailabilityText.Text = "";
            UnavailableReasonText.Text = "";
            ReturnDateDetailedText.Text = "";
        }

        private void SaveAllChanges()
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();

                string projectPath = Path.GetFullPath(Path.Combine(currentPath, "..", "..", ".."));

                string jsonPath = Path.Combine(projectPath, JsonFile);


                MessageBox.Show($"most: {currentPath}\nprojekt: {projectPath}\njson: {jsonPath}");



                using (StreamWriter sw = new StreamWriter(jsonPath))
                {
                    sw.WriteLine("[");

                    for (int i = 0; i < players.Count; i++)
                    {
                        Player player = players[i];

                        sw.WriteLine("  {");
                        sw.WriteLine($"    \"Name\": \"{player.Name}\",");
                        sw.WriteLine($"    \"BirthDate\": \"{player.BirthDate.Year:D4}-{player.BirthDate.Month:D2}-{player.BirthDate.Day:D2}\",");

                        sw.Write("    \"Nationalities\": [");
                        if (player.Nationalities != null && player.Nationalities.Count > 0)
                        {
                            for (int n = 0; n < player.Nationalities.Count; n++)
                            {
                                sw.Write($" \"{player.Nationalities[n]}\"");
                                if (n < player.Nationalities.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                        }
                        sw.WriteLine(" ],");

                        sw.WriteLine($"    \"MarketValue\": {player.MarketValue},");

                        if (player.IsAvailable)
                        {
                            sw.WriteLine("    \"IsAvailable\": true,");
                        }
                        else
                        {
                            sw.WriteLine("    \"IsAvailable\": false,");
                        }

                        if (player.UnavailableReason == null)
                        {
                            sw.WriteLine("    \"UnavailableReason\": null,");
                        }
                        else
                        {
                            sw.WriteLine($"    \"UnavailableReason\": \"{player.UnavailableReason}\",");
                        }

                        if (player.ReturnDate == null)
                        {
                            sw.WriteLine("    \"ReturnDate\": null,");

                        }
                        else
                        {
                            DateTime returnDate = player.ReturnDate.Value;
                            sw.WriteLine($"    \"ReturnDate\": \"{returnDate.Year:D4}-{returnDate.Month:D2}-{returnDate.Day:D2}\",");
                        }

                        sw.WriteLine($"    \"JerseyNumber\": {player.JerseyNumber},");
                        sw.WriteLine($"    \"Height\": {player.Height},");
                        sw.WriteLine($"    \"PreferredFoot\": \"{player.PreferredFoot}\",");
                        sw.WriteLine($"    \"Position\": \"{player.Position}\",");
                        sw.WriteLine($"    \"JoinedTeam\": \"{player.JoinedTeam.Year:D4}-{player.JoinedTeam.Month:D2}-{player.JoinedTeam.Day:D2}\",");
                        sw.WriteLine($"    \"ContractEnd\": \"{player.ContractEnd.Year:D4}-{player.ContractEnd.Month:D2}-{player.ContractEnd.Day:D2}\"");

                        sw.Write("  }");
                        if (i < players.Count - 1)
                        {
                            sw.WriteLine(",");
                        }
                        else
                        {
                            sw.WriteLine();
                        }
                    }

                    sw.WriteLine("]");
                }

                MessageBox.Show("Mentés");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a mentéskor: " + ex.Message);
            }
        }


    }
}