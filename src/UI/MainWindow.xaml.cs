using System;
using System.Windows;
using System.Windows.Controls;
using ModernNesting.Core;
using ModernNesting.IO;
using ModernNesting.Models;
using Microsoft.Win32;
using System.Collections.Generic;

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
        }

        private void ImportParts_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All Supported Files|*.dxf;*.svg;*.ai;*.cdr|DXF Files|*.dxf|SVG Files|*.svg|AI Files|*.ai|CDR Files|*.cdr"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var newParts = fileImporter.ImportFile(openFileDialog.FileName);
                    loadedParts.AddRange(newParts);
                    UpdatePartsListDisplay();
                    StatusText.Text = $"Imported {newParts.Count} parts";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing file: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportResult_Click(object sender, RoutedEventArgs e)
        {
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

                    fileExporter.ExportResult(nestingEngine.ProcessNesting(), saveFileDialog.FileName, format);
                    StatusText.Text = "Export completed";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting file: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdatePartsListDisplay()
        {
            PartsList.Items.Clear();
            foreach (var part in loadedParts)
            {
                PartsList.Items.Add($"{part.Name} ({part.Width}x{part.Height})");
            }
        }

        // TODO: Implement remaining event handlers
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SheetSettings_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement sheet settings dialog
        }

        private void NestingOptions_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement nesting options dialog
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement zoom in
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement zoom out
        }

        private void FitToScreen_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement fit to screen
        }

        private void UserManual_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Show user manual
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ModernNesting\nVersion 1.0\n\nA modern nesting application for laser cutting and CNC machines.", 
                          "About ModernNesting", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Information);
        }
    }
}
