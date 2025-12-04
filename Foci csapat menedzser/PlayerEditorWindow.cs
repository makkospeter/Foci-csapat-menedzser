using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Foci_csapat_menedzser
{
    public partial class PlayerEditorWindow : Window
    {
        public Player EditedPlayer { get; private set; }
        private MainWindow mainWindow;

        public PlayerEditorWindow(Player player = null, Window owner = null)
        {
            InitializeComponent();

            if (owner is MainWindow mw)
            {
                mainWindow = mw;
            }

            if (player != null)
            {
                TempEditedPlayer(player);
            }
            else
            {
                NewPlayer();
            }

            LoadNationalities();
            
            FillForm();
        }

        private void TempEditedPlayer(Player player)
        {
            List<string> nationalities;

            if (player.Nationalities != null)
            {
                nationalities = new List<string>(player.Nationalities);
            }
            else
            {
                nationalities = new List<string>();
            }

            EditedPlayer = new Player
            {
                Name = player.Name,
                BirthDate = player.BirthDate,
                Nationalities = nationalities,
                MarketValue = player.MarketValue,
                IsAvailable = player.IsAvailable,
                UnavailableReason = player.UnavailableReason,
                ReturnDate = player.ReturnDate,
                JerseyNumber = player.JerseyNumber,
                Height = player.Height,
                PreferredFoot = player.PreferredFoot,
                Position = player.Position,
                JoinedTeam = player.JoinedTeam,
                ContractEnd = player.ContractEnd
            };

            Title = $"{player.Name} Szerkesztése";
        }

        private void NewPlayer()
        {
            EditedPlayer = new Player
            {
                Nationalities = new List<string>(),
                BirthDate = DateTime.Now.AddYears(-20),
                JoinedTeam = DateTime.Now,
                ContractEnd = DateTime.Now.AddYears(3),
                IsAvailable = true
            };

            Title = "Új Játékos Hozzáadása";
        }

        private void LoadNationalities()
        {
            NationalityComboBox.Items.Clear();

            if (Player.countryCodes == null)
            {
                return;
            }

            List<string> nationalities = new List<string>();

            foreach (var codePair in Player.countryCodes)
            {
                nationalities.Add(codePair.Value);
            }

            nationalities.Sort();

            foreach (var nationality in nationalities)
            {
                NationalityComboBox.Items.Add(nationality);
            }

            if (NationalityComboBox.Items.Count > 0)
            {
                NationalityComboBox.SelectedIndex = 0;
            }
        }

        private void FillForm()
        {
            NameTextBox.Text = EditedPlayer.Name;
            JerseyNumberTextBox.Text = EditedPlayer.JerseyNumber.ToString();
            BirthDatePicker.SelectedDate = EditedPlayer.BirthDate;
            HeightTextBox.Text = EditedPlayer.Height.ToString();

            SetPosition();

            SetFoot();

            FillNationalitiesPanel();

            MarketValueTextBox.Text = EditedPlayer.MarketValue.ToString();

            IsAvailableCheckBox.IsChecked = EditedPlayer.IsAvailable;
            UnavailableReasonTextBox.Text = EditedPlayer.UnavailableReason;
            ReturnDatePicker.SelectedDate = EditedPlayer.ReturnDate;

            JoinedTeamPicker.SelectedDate = EditedPlayer.JoinedTeam;
            ContractEndPicker.SelectedDate = EditedPlayer.ContractEnd;

            UpdateAvailabilityPanel();
        }

        private void SetPosition()
        {
            if (string.IsNullOrEmpty(EditedPlayer.Position))
            {
                return;
            }

            foreach (ComboBoxItem item in PositionComboBox.Items)
            {
                if (item.Content.ToString() == EditedPlayer.Position)
                {
                    PositionComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SetFoot()
        {
            if (EditedPlayer.PreferredFoot == "Bal")
            {
                LeftFootRadio.IsChecked = true;
            }
            else if (EditedPlayer.PreferredFoot == "Jobb")
            {
                RightFootRadio.IsChecked = true;
            }
            else
            {
                RightFootRadio.IsChecked = true;
            }
        }

        private void FillNationalitiesPanel()
        {
            NationalitiesListBox.Items.Clear();

            if (EditedPlayer.Nationalities == null)
            {
                return;
            }

            foreach (var nationality in EditedPlayer.Nationalities)
            {
                NationalitiesListBox.Items.Add(nationality);
            }
        }

        private void UpdateAvailabilityPanel()
        {
            if (IsAvailableCheckBox.IsChecked == true)
            {
                UnavailablePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                UnavailablePanel.Visibility = Visibility.Visible;
            }
        }

        private void IsAvailableCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateAvailabilityPanel();
        }

        private void AddNationalityButton_Click(object sender, RoutedEventArgs e)
        {
            if (NationalityComboBox.SelectedItem == null)
            {
                return;
            }

            string nationality = NationalityComboBox.SelectedItem.ToString();
            bool alreadyAdded = false;

            foreach (var item in NationalitiesListBox.Items)
            {
                if (item.ToString() == nationality)
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (!alreadyAdded)
            {
                NationalitiesListBox.Items.Add(nationality);
            }
        }

        private void RemoveNationality_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag != null)
                {
                    NationalitiesListBox.Items.Remove(button.Tag.ToString());
                }
            }
        }

        private bool IsValidNumber(string text, int minValue, int maxValue)
        {
            try
            {
                int number = int.Parse(text);
                return number >= minValue && number <= maxValue;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPositiveNumber(string text)
        {
            try
            {
                int number = int.Parse(text);
                return number >= 0;
            }
            catch
            {
                return false;
            }
        }

        private bool ContainsDigits(string text)
        {
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("A név megadása kötelező!");
                NameTextBox.Focus();
                return false;
            }

            bool containsSpecialChar = false;
            
            string specialChars = "!@#$%^&*()_+=[]{};:<>|/?,\"";
            
            for (int i = 0; i < NameTextBox.Text.Length; i++)
            {
                char ch = NameTextBox.Text[i];
                if (specialChars.Contains(ch))
                {
                    containsSpecialChar = true;
                    break;
                }
            }

            string name = NameTextBox.Text.Trim();
            if (ContainsDigits(name) || containsSpecialChar)
            {
                MessageBox.Show("A név nem tartalmazhat számot vagy speciális karaktert!");
                NameTextBox.Focus();
                return false;
            }

            string jerseyText = JerseyNumberTextBox.Text;
            if (!IsValidNumber(jerseyText, 1, 99))
            {
                MessageBox.Show("A mezszám 1 és 99 közötti szám kell legyen!");
                JerseyNumberTextBox.Focus();
                return false;
            }

            if (EditedPlayer.JerseyNumber != int.Parse(JerseyNumberTextBox.Text))
            { 
                foreach (var player in mainWindow.players)
                {
                    if (player.JerseyNumber == int.Parse(JerseyNumberTextBox.Text))
                    {
                        MessageBox.Show($"A {player.JerseyNumber} mezszám már foglalt!");
                        JerseyNumberTextBox.Focus();
                        return false;
                    }
                }
            }

            if (BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("A születési dátum megadása kötelező!");
                BirthDatePicker.Focus();
                return false;
            }

            if (BirthDatePicker.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("A születési dátum nem lehet jövőbeli!");
                BirthDatePicker.Focus();
                return false;
            }

            DateTime minBirthDate = DateTime.Now.AddYears(-15);
            if (BirthDatePicker.SelectedDate > minBirthDate)
            {
                MessageBox.Show("A játékosnak legalább 15 évesnek kell lennie!");
                BirthDatePicker.Focus();
                return false;
            }

            string heightText = HeightTextBox.Text;
            if (!IsValidNumber(heightText, 150, 220))
            {
                MessageBox.Show("A magasság 150 és 220 cm között kell legyen!");
                HeightTextBox.Focus();
                return false;
            }

            if (PositionComboBox.SelectedItem == null)
            {
                MessageBox.Show("A pozíció kiválasztása kötelező!");
                PositionComboBox.Focus();
                return false;
            }

            if (LeftFootRadio.IsChecked == false && RightFootRadio.IsChecked == false)
            {
                MessageBox.Show("Az erősebb láb kiválasztása kötelező!");
                return false;
            }

            if (NationalitiesListBox.Items.Count == 0)
            {
                MessageBox.Show("Legalább egy nemzetiség megadása kötelező!");
                return false;
            }

            string marketValueText = MarketValueTextBox.Text;
            if (!IsValidPositiveNumber(marketValueText))
            {
                MessageBox.Show("A piaci érték pozitív szám kell legyen!");
                MarketValueTextBox.Focus();
                return false;
            }

            if (JoinedTeamPicker.SelectedDate == null || ContractEndPicker.SelectedDate == null)
            {
                MessageBox.Show("Mindkét szerződési dátumot meg kell adni!");
                return false;
            }

            if (ContractEndPicker.SelectedDate <= JoinedTeamPicker.SelectedDate)
            {
                MessageBox.Show("A szerződés vége nem lehet korábbi, mint a csatlakozás dátuma!");
                return false;
            }

            if (JoinedTeamPicker.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("A csatlakozás dátuma nem lehet jövőbeli!");
                JoinedTeamPicker.Focus();
                return false;
            }

            if (IsAvailableCheckBox.IsChecked == false)
            {
                if (string.IsNullOrWhiteSpace(UnavailableReasonTextBox.Text))
                {
                    MessageBox.Show("Az el nem érhetőség okának megadása kötelező!");
                    UnavailableReasonTextBox.Focus();
                    return false;
                }

                if (ReturnDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("A visszatérés dátumának megadása kötelező!");
                    ReturnDatePicker.Focus();
                    return false;
                }

                if (ReturnDatePicker.SelectedDate < DateTime.Now)
                {
                    MessageBox.Show("A játékos már elérhető!");
                    IsAvailableCheckBox.IsChecked = true;
                    return false;
                }

            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            try
            {
                EditedPlayer.Name = NameTextBox.Text.Trim();
                EditedPlayer.JerseyNumber = int.Parse(JerseyNumberTextBox.Text);
                EditedPlayer.BirthDate = BirthDatePicker.SelectedDate.Value;
                EditedPlayer.Height = int.Parse(HeightTextBox.Text);

                EditedPlayer.Position = (PositionComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                if (LeftFootRadio.IsChecked == true)
                {
                    EditedPlayer.PreferredFoot = "Bal";
                }
                else
                {
                    EditedPlayer.PreferredFoot = "Jobb";
                }

                EditedPlayer.Nationalities.Clear();
                foreach (string nationality in NationalitiesListBox.Items)
                {
                    EditedPlayer.Nationalities.Add(nationality);
                }

                EditedPlayer.MarketValue = int.Parse(MarketValueTextBox.Text);

                EditedPlayer.JoinedTeam = JoinedTeamPicker.SelectedDate.Value;
                EditedPlayer.ContractEnd = ContractEndPicker.SelectedDate.Value;

                UpdatePlayerAvailability();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt az adatok mentésekor: " + ex.Message);
            }
        }

        private void UpdatePlayerAvailability()
        {
            if (EditedPlayer.ContractEnd < DateTime.Now)
            {
                EditedPlayer.IsAvailable = false;

                if (string.IsNullOrEmpty(EditedPlayer.UnavailableReason))
                {
                    EditedPlayer.UnavailableReason = "Lejárt szerződés";
                }
            }
            else
            {
                EditedPlayer.IsAvailable = IsAvailableCheckBox.IsChecked == true;
            }

            if (EditedPlayer.IsAvailable)
            {
                EditedPlayer.UnavailableReason = null;
                EditedPlayer.ReturnDate = null;
            }
            else
            {
                EditedPlayer.UnavailableReason = UnavailableReasonTextBox.Text.Trim();
                EditedPlayer.ReturnDate = ReturnDatePicker.SelectedDate.Value;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}