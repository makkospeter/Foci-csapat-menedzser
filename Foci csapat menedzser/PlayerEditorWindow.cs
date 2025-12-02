using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Foci_csapat_menedzser
{
    public partial class PlayerEditorWindow : Window
    {
        public Player EditedPlayer { get; private set; }
        private bool isEditMode;
        private Player originalPlayer;

        public PlayerEditorWindow(Player player = null)
        {
            InitializeComponent();

            if (player != null)
            {
                isEditMode = true;
                originalPlayer = player;

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
                Title = "Játékos Szerkesztése";
            }
            else
            {
                isEditMode = false;
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

            LoadNationalities();
            InitializeForm();
        }

        private void LoadNationalities()
        {
            NationalityComboBox.Items.Clear();

            if (Player.countryCodes != null)
            {
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
            }

            if (NationalityComboBox.Items.Count > 0)
            {
                NationalityComboBox.SelectedIndex = 0;
            }
        }

        private void InitializeForm()
        {
            NameTextBox.Text = EditedPlayer.Name;
            JerseyNumberTextBox.Text = EditedPlayer.JerseyNumber.ToString();
            BirthDatePicker.SelectedDate = EditedPlayer.BirthDate;
            HeightTextBox.Text = EditedPlayer.Height.ToString();

            if (!string.IsNullOrEmpty(EditedPlayer.Position))
            {
                foreach (ComboBoxItem item in PositionComboBox.Items)
                {
                    if (item.Content.ToString() == EditedPlayer.Position)
                    {
                        PositionComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (EditedPlayer.PreferredFoot == "Left")
            {
                LeftFootRadio.IsChecked = true;
            }
            else
            {
                RightFootRadio.IsChecked = true;
            }

            NationalitiesListBox.Items.Clear();
            if (EditedPlayer.Nationalities != null)
            {
                foreach (var nationality in EditedPlayer.Nationalities)
                {
                    NationalitiesListBox.Items.Add(nationality);
                }
            }

            MarketValueTextBox.Text = EditedPlayer.MarketValue.ToString();

            IsAvailableCheckBox.IsChecked = EditedPlayer.IsAvailable;
            UnavailableReasonTextBox.Text = EditedPlayer.UnavailableReason;
            ReturnDatePicker.SelectedDate = EditedPlayer.ReturnDate;

            UpdateAvailabilityPanel();
        }

        private void UpdateAvailabilityPanel()
        {
            bool isAvailable = false;
            if (IsAvailableCheckBox.IsChecked.HasValue)
            {
                isAvailable = IsAvailableCheckBox.IsChecked.Value;
            }

            if (isAvailable)
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
            if (NationalityComboBox.SelectedItem != null)
            {
                string nationality = NationalityComboBox.SelectedItem.ToString();

                bool contains = false;
                foreach (var item in NationalitiesListBox.Items)
                {
                    if (item.ToString() == nationality)
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    NationalitiesListBox.Items.Add(nationality);
                }
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

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("A név megadása kötelező!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return false;
            }

            string jerseyText = JerseyNumberTextBox.Text;
            if (!IsValidNumber(jerseyText, 1, 99))
            {
                MessageBox.Show("A mezszám 1 és 99 közötti szám kell legyen!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                JerseyNumberTextBox.Focus();
                return false;
            }

            if (BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("A születési dátum megadása kötelező!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                BirthDatePicker.Focus();
                return false;
            }

            string heightText = HeightTextBox.Text;
            if (!IsValidNumber(heightText, 150, 220))
            {
                MessageBox.Show("A magasság 150 és 220 cm között kell legyen!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                HeightTextBox.Focus();
                return false;
            }

            if (PositionComboBox.SelectedItem == null)
            {
                MessageBox.Show("A pozíció kiválasztása kötelező!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                PositionComboBox.Focus();
                return false;
            }

            bool leftChecked = false;
            bool rightChecked = false;
            if (LeftFootRadio.IsChecked.HasValue)
            {
                leftChecked = LeftFootRadio.IsChecked.Value;
            }
            if (RightFootRadio.IsChecked.HasValue)
            {
                rightChecked = RightFootRadio.IsChecked.Value;
            }

            if (!leftChecked && !rightChecked)
            {
                MessageBox.Show("Az erősebb láb kiválasztása kötelező!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (NationalitiesListBox.Items.Count == 0)
            {
                MessageBox.Show("Legalább egy nemzetiség megadása kötelező!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string marketValueText = MarketValueTextBox.Text;
            if (!IsValidPositiveNumber(marketValueText))
            {
                MessageBox.Show("A piaci érték pozitív szám kell legyen!", "Érvénytelen adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                MarketValueTextBox.Focus();
                return false;
            }

            bool isAvailable = false;
            if (IsAvailableCheckBox.IsChecked.HasValue)
            {
                isAvailable = IsAvailableCheckBox.IsChecked.Value;
            }

            if (!isAvailable)
            {
                if (string.IsNullOrWhiteSpace(UnavailableReasonTextBox.Text))
                {
                    MessageBox.Show("Az elérhetetlenség okának megadása kötelező!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    UnavailableReasonTextBox.Focus();
                    return false;
                }

                if (ReturnDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("A visszatérés dátumának megadása kötelező!", "Hiányzó adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ReturnDatePicker.Focus();
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

                if (BirthDatePicker.SelectedDate.HasValue)
                {
                    EditedPlayer.BirthDate = BirthDatePicker.SelectedDate.Value;
                }

                EditedPlayer.Height = int.Parse(HeightTextBox.Text);

                if (PositionComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    EditedPlayer.Position = selectedItem.Content.ToString();
                }

                if (LeftFootRadio.IsChecked == true)
                {
                    EditedPlayer.PreferredFoot = "Left";
                }
                else
                {
                    EditedPlayer.PreferredFoot = "Right";
                }

                EditedPlayer.Nationalities.Clear();
                foreach (string nationality in NationalitiesListBox.Items)
                {
                    EditedPlayer.Nationalities.Add(nationality);
                }

                EditedPlayer.MarketValue = int.Parse(MarketValueTextBox.Text);

                bool isAvailable = false;
                if (IsAvailableCheckBox.IsChecked.HasValue)
                {
                    isAvailable = IsAvailableCheckBox.IsChecked.Value;
                }
                EditedPlayer.IsAvailable = isAvailable;

                if (isAvailable)
                {
                    EditedPlayer.UnavailableReason = null;
                    EditedPlayer.ReturnDate = null;
                }
                else
                {
                    EditedPlayer.UnavailableReason = UnavailableReasonTextBox.Text.Trim();
                    if (ReturnDatePicker.SelectedDate.HasValue)
                    {
                        EditedPlayer.ReturnDate = ReturnDatePicker.SelectedDate.Value;
                    }
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt az adatok mentésekor: " + ex.Message, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}