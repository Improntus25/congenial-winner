using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ModernNesting.Core;
using ModernNesting.IO;
using ModernNesting.Models;
using ModernNesting.UI.Dialogs;
using Microsoft.Win32;
using System.Linq;

namespace ModernNesting.UI
{
    public partial class MainWindow : Window
    {
        private NestingEngine nestingEngine;
        private FileImporter fileImporter;
        private FileExporter fileExporter;
        private List<Part> loadedParts;
        private List<Sheet> availableSheets;
        private NestingConfig nestingConfig;

        public MainWindow()
        {
            InitializeComponent();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            fileImporter = new FileImporter();
            fileExporter = new FileExporter();
            loadedParts = new List<Part>();
            availableSheets = new List<Sheet>();
            nestingConfig = new NestingConfig();

            // Add default sheet
            availableSheets.Add(new Sheet(1000, 1000));

            // Initialize nesting engine
            nestingEngine = new NestingEngine(availableSheets, loadedParts, nestingConfig);

            // Update preview
            PreviewCanvas.SetSheets(availableSheets);
        }

        private void ImportParts_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = fileImporter.GetSupportedFileTypes(),
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    foreach (var fileName in openFileDialog.FileNames)
                    {
                        var newParts = fileImporter.ImportFile(fileName);
                        loadedParts.AddRange(newParts);
                    }

                    UpdatePartsListDisplay();
                    PreviewCanvas.SetParts(loadedParts);
                    StatusText.Text = $"Imported {loadedParts.Count} parts";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing file: {ex.Message}", "Import Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportResult_Click(object sender, RoutedEventArgs e)
        {
            if (loadedParts.Count == 0)
            {
                MessageBox.Show("No parts to export. Please import some parts first.",
                              "Export Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "DXF File|*.dxf|SVG File|*.svg|PDF File|*.pdf|Report|*.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var format = saveFileDialog.FilterIndex switch
                    {
                        1 => FileExporter.ExportFormat.DXF,
                        2 => FileExporter.ExportFormat.SVG,
                        3 => FileExporter.ExportFormat.PDF,
                        4 => FileExporter.ExportFormat.Report,
                        _ => FileExporter.ExportFormat.DXF
                    };

                    var result = nestingEngine.ProcessNesting();
                    fileExporter.ExportResult(result, saveFileDialog.FileName, format);
                    StatusText.Text = "Export completed";
                    EfficiencyText.Text = $"Efficiency: {result.Efficiency:F1}%";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting file: {ex.Message}", "Export Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdatePartsListDisplay()
        {
            PartsList.ItemsSource = null;
            PartsList.ItemsSource = loadedParts;
            TotalPartsText.Text = loadedParts.Count.ToString();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SheetSettings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SheetSettingsDialog(availableSheets);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                availableSheets = dialog.Sheets;
                nestingEngine = new NestingEngine(availableSheets, loadedParts, nestingConfig);
                PreviewCanvas.SetSheets(availableSheets);
                StatusText.Text = $"Updated sheet settings: {availableSheets.Count} sheet(s) configured";
            }
        }

        private void NestingOptions_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement nesting options dialog
            MessageBox.Show("Nesting options dialog will be implemented soon.", "Coming Soon",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            // Delegate to PartCanvas zoom functionality
            PreviewCanvas.ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            // Delegate to PartCanvas zoom functionality
            PreviewCanvas.ZoomOut();
        }

        private void FitToScreen_Click(object sender, RoutedEventArgs e)
        {
            PreviewCanvas.FitToScreen();
        }

        private void UserManual_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Show user manual
            MessageBox.Show("User manual will be implemented soon.", "Coming Soon",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "ModernNesting\nVersion 1.0\n\n" +
                "A modern nesting application for laser cutting and CNC machines.\n\n" +
                "Supports DXF, SVG, AI, and CDR files.",
                "About ModernNesting",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void PartsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartsList.SelectedItem is Part selectedPart)
            {
                // Highlight selected part in preview
                PreviewCanvas.HighlightPart(selectedPart);
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            loadedParts.Clear();
            UpdatePartsListDisplay();
            PreviewCanvas.SetParts(loadedParts);
            StatusText.Text = "All parts cleared";
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            if (PartsList.SelectedItem is Part selectedPart)
            {
                loadedParts.Remove(selectedPart);
                UpdatePartsListDisplay();
                PreviewCanvas.SetParts(loadedParts);
                StatusText.Text = "Selected part removed";
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (loadedParts.Any())
            {
                var result = MessageBox.Show(
                    "Do you want to save your work before closing?",
                    "Save Changes",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ExportResult_Click(this, new RoutedEventArgs());
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
            base.OnClosing(e);
        }
    }
}