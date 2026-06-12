using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;

namespace OpenEdge;

public partial class BulkMoveDestinationDialog : Window, IComponentConnector
{
	private sealed class DestinationOption
	{
		public string Label { get; init; } = "";

		public string Path { get; init; } = "";
	}

	public string SelectedDestinationPath { get; private set; } = "";

	public BulkMoveDestinationDialog(IEnumerable<MediaSourceDefinition> sources)
	{
		InitializeComponent();
		LoadDestinations(sources);
	}

	private void LoadDestinations(IEnumerable<MediaSourceDefinition> sources)
	{
		foreach (MediaSourceDefinition source in sources.Where(delegate(MediaSourceDefinition item)
		{
			return item.IsEnabled && !string.IsNullOrWhiteSpace(item.RootPath) && Directory.Exists(item.RootPath);
		}).OrderBy(delegate(MediaSourceDefinition item)
		{
			return item.SortOrder;
		}).ThenBy(delegate(MediaSourceDefinition item)
		{
			return item.Name;
		}, StringComparer.OrdinalIgnoreCase))
		{
			DestinationList.Items.Add(new DestinationOption
			{
				Label = source.Name + " (source root)",
				Path = source.RootPath
			});
			foreach (string directory in GetSafeDirectories(source.RootPath).OrderBy(System.IO.Path.GetFileName, StringComparer.OrdinalIgnoreCase).Take(500))
			{
				DestinationList.Items.Add(new DestinationOption
				{
					Label = source.Name + " / " + System.IO.Path.GetFileName(directory),
					Path = directory
				});
			}
		}
	}

	private void DestinationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (DestinationList.SelectedItem is DestinationOption option)
		{
			SelectedDestinationPath = option.Path;
			SelectedPathText.Text = option.Path;
			MoveButton.IsEnabled = true;
			return;
		}
		SelectedDestinationPath = "";
		SelectedPathText.Text = "";
		MoveButton.IsEnabled = false;
	}

	private void NewFolder_Click(object sender, RoutedEventArgs e)
	{
		string parentPath = SelectedDestinationPath;
		if (string.IsNullOrWhiteSpace(parentPath))
		{
			MessageBox.Show("Select a source or folder first, then create a new subfolder inside it.", "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}
		string folderName = Microsoft.VisualBasic.Interaction.InputBox("New folder name:", "Create Move Destination", "New Folder");
		if (string.IsNullOrWhiteSpace(folderName))
		{
			return;
		}
		foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
		{
			folderName = folderName.Replace(invalidChar.ToString(), "");
		}
		if (string.IsNullOrWhiteSpace(folderName))
		{
			return;
		}
		string newPath = System.IO.Path.Combine(parentPath, folderName.Trim());
		Directory.CreateDirectory(newPath);
		SelectedDestinationPath = newPath;
		SelectedPathText.Text = newPath;
		DestinationList.Items.Add(new DestinationOption
		{
			Label = "New folder / " + folderName.Trim(),
			Path = newPath
		});
		MoveButton.IsEnabled = true;
	}

	private void CustomFolder_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog dialog = new OpenFileDialog
		{
			CheckFileExists = false,
			CheckPathExists = true,
			ValidateNames = false,
			FileName = "Select this folder"
		};
		if (dialog.ShowDialog(this) == true)
		{
			SelectedDestinationPath = System.IO.Path.GetDirectoryName(dialog.FileName) ?? "";
			SelectedPathText.Text = string.IsNullOrWhiteSpace(SelectedDestinationPath) ? "" : "Custom: " + SelectedDestinationPath;
			MoveButton.IsEnabled = !string.IsNullOrWhiteSpace(SelectedDestinationPath);
		}
	}

	private void Cancel_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = false;
	}

	private void MoveButton_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = !string.IsNullOrWhiteSpace(SelectedDestinationPath);
	}

	private static IEnumerable<string> GetSafeDirectories(string directory)
	{
		try
		{
			return Directory.GetDirectories(directory);
		}
		catch (UnauthorizedAccessException)
		{
			return Array.Empty<string>();
		}
		catch (IOException)
		{
			return Array.Empty<string>();
		}
	}
}
