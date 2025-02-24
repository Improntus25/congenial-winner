using System;
using System.Collections.Generic;
using System.Windows;
using ModernNesting.Models;

namespace ModernNesting.UI.Dialogs
{
    public partial class SheetSettingsDialog : Window
    {
        private List<Sheet> sheets;

        public List<Sheet> Sheets => sheets;

        public SheetSettingsDialog(List<Sheet> currentSheets)
        {
            InitializeComponent();
            sheets = new List<Sheet>();

            // Load current sheets
            if (currentSheets != null && currentSheets.Count > 0)
            {
                if (currentSheets.Count >= 1)
                {
                    Sheet1Width.Text = currentSheets[0].Width.ToString();
                    Sheet1Height.Text = currentSheets[0].Height.ToString();
                    Sheet1Material.Text = currentSheets[0].Material;
                    Sheet1Thickness.Text = currentSheets[0].Thickness.ToString();
                }
                if (currentSheets.Count >= 2)
                {
                    Sheet2Width.Text = currentSheets[1].Width.ToString();
                    Sheet2Height.Text = currentSheets[1].Height.ToString();
                    Sheet2Material.Text = currentSheets[1].Material;
                    Sheet2Thickness.Text = currentSheets[1].Thickness.ToString();
                }
                if (currentSheets.Count >= 3)
                {
                    Sheet3Width.Text = currentSheets[2].Width.ToString();
                    Sheet3Height.Text = currentSheets[2].Height.ToString();
                    Sheet3Material.Text = currentSheets[2].Material;
                    Sheet3Thickness.Text = currentSheets[2].Thickness.ToString();
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate and add Sheet 1 (required)
                if (!ValidateSheet(Sheet1Width.Text, Sheet1Height.Text, "Sheet 1"))
                {
                    return;
                }

                sheets.Add(new Sheet(
                    double.Parse(Sheet1Width.Text),
                    double.Parse(Sheet1Height.Text),
                    Sheet1Material.Text,
                    ParseThickness(Sheet1Thickness.Text)
                ));

                // Try to add Sheet 2 if values are provided
                if (!string.IsNullOrWhiteSpace(Sheet2Width.Text) && !string.IsNullOrWhiteSpace(Sheet2Height.Text))
                {
                    if (!ValidateSheet(Sheet2Width.Text, Sheet2Height.Text, "Sheet 2"))
                    {
                        return;
                    }

                    sheets.Add(new Sheet(
                        double.Parse(Sheet2Width.Text),
                        double.Parse(Sheet2Height.Text),
                        Sheet2Material.Text,
                        ParseThickness(Sheet2Thickness.Text)
                    ));
                }

                // Try to add Sheet 3 if values are provided
                if (!string.IsNullOrWhiteSpace(Sheet3Width.Text) && !string.IsNullOrWhiteSpace(Sheet3Height.Text))
                {
                    if (!ValidateSheet(Sheet3Width.Text, Sheet3Height.Text, "Sheet 3"))
                    {
                        return;
                    }

                    sheets.Add(new Sheet(
                        double.Parse(Sheet3Width.Text),
                        double.Parse(Sheet3Height.Text),
                        Sheet3Material.Text,
                        ParseThickness(Sheet3Thickness.Text)
                    ));
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating sheet settings: {ex.Message}",
                              "Validation Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateSheet(string width, string height, string sheetName)
        {
            if (!double.TryParse(width, out double w) || w <= 0)
            {
                MessageBox.Show($"Invalid width for {sheetName}", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!double.TryParse(height, out double h) || h <= 0)
            {
                MessageBox.Show($"Invalid height for {sheetName}", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private double ParseThickness(string thickness)
        {
            if (double.TryParse(thickness, out double t) && t > 0)
            {
                return t;
            }
            return 1.0; // default thickness
        }
    }
}