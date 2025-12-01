using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Foci_csapat_menedzser
{
    public partial class PlayerEditorWindow : Window
    {
        public Player EditedPlayer { get; private set; }

        public PlayerEditorWindow(Player player = null)
        {
            InitializeComponent();

            if (player != null)
            {
                EditedPlayer = player;
                Title = "Játékos Szerkesztése";
            }
            else
            {
                EditedPlayer = new Player
                {
                    Nationalities = new List<string>(),
                    BirthDate = DateTime.Now.AddYears(-20),
                    IsAvailable = true
                };
                Title = "Új Játékos";
            }

            InitializeForm();
        }

        private void InitializeForm()
        {
            NameTextBox.Text = EditedPlayer.Name ?? "";

            if (EditedPlayer.JerseyNumber > 0)
            {
                JerseyNumberTextBox.Text = EditedPlayer.JerseyNumber.ToString();
            }

            if (EditedPlayer.BirthDate.Year > 1900)
            {
                BirthDatePicker.SelectedDate = EditedPlayer.BirthDate;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("A név megadása kötelező!", "Hiányzó adat");
                return;
            }

            try
            {
                EditedPlayer.Name = NameTextBox.Text.Trim();

                if (int.TryParse(JerseyNumberTextBox.Text, out int jersey))
                {
                    EditedPlayer.JerseyNumber = jersey;
                }

                if (BirthDatePicker.SelectedDate.HasValue)
                {
                    EditedPlayer.BirthDate = BirthDatePicker.SelectedDate.Value;
                }

                if (PositionComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    EditedPlayer.Position = selectedItem.Content.ToString();
                }

                if (LeftFootRadio.IsChecked == true)
                {
                    EditedPlayer.PreferredFoot = "Left";
                }
                else if (RightFootRadio.IsChecked == true)
                {
                    EditedPlayer.PreferredFoot = "Right";
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba: " + ex.Message, "Hiba");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}