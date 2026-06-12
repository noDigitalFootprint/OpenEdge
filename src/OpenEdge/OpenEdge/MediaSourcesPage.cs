using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;

namespace OpenEdge;

public partial class MediaSourcesPage : Page, IComponentConnector
{
	private sealed class SourceEditorRow
	{
		public MediaSourceDefinition Source { get; init; }

		public CheckBox EnabledBox { get; init; }

		public CheckBox ImagesBox { get; init; }

		public CheckBox VideosBox { get; init; }

		public Border Root { get; init; }

		public List<FolderEditorNode> FolderNodes { get; init; } = new List<FolderEditorNode>();
	}

	private sealed class FolderEditorNode
	{
		public MediaSourceDefinition Source { get; init; }

		public string FullPath { get; init; } = "";

		public string RelativeFolderPath { get; init; } = "";

		public CheckBox IncludeBox { get; init; }

		public List<FolderEditorNode> Children { get; } = new List<FolderEditorNode>();

		public bool ChildrenLoaded { get; set; }
	}

	private readonly MediaCatalogService mediaCatalog;

	private readonly ImageTagger imageTagger;

	private readonly MainWindow mainWindow;

	private readonly List<SourceEditorRow> sourceRows = new List<SourceEditorRow>();

	public MediaSourcesPage(ImageTagger imageTagger, MainWindow mainWindow)
	{
		this.imageTagger = imageTagger;
		this.mainWindow = mainWindow;
		mediaCatalog = imageTagger.MediaCatalog;
		InitializeComponent();
		LoadRows();
	}

	private void LoadRows()
	{
		SourcesPanel.Children.Clear();
		sourceRows.Clear();
		foreach (MediaSourceDefinition source in mediaCatalog.GetSources().OrderBy(delegate(MediaSourceDefinition item)
		{
			return item.SortOrder;
		}))
		{
			AddRow(CloneSource(source));
		}
		UpdateSummary();
	}

	private void AddRow(MediaSourceDefinition source)
	{
		CheckBox enabledBox = new CheckBox
		{
			Content = "Enabled",
			IsChecked = source.IsEnabled,
			Margin = new Thickness(0, 0, 12, 0),
			Foreground = System.Windows.Media.Brushes.White
		};
		CheckBox imagesBox = new CheckBox
		{
			Content = "Images",
			IsChecked = source.ImagesEnabled,
			Margin = new Thickness(0, 0, 12, 0),
			Foreground = System.Windows.Media.Brushes.White
		};
		CheckBox videosBox = new CheckBox
		{
			Content = "Videos",
			IsChecked = source.VideosEnabled,
			Margin = new Thickness(0, 0, 12, 0),
			Foreground = System.Windows.Media.Brushes.White
		};
		System.Windows.Controls.Button removeButton = new System.Windows.Controls.Button
		{
			Content = source.IsLegacy ? "Legacy" : "Remove",
			Width = 100,
			IsEnabled = !source.IsLegacy
		};
		StackPanel toggles = new StackPanel
		{
			Orientation = Orientation.Horizontal
		};
		toggles.Children.Add(enabledBox);
		toggles.Children.Add(imagesBox);
		toggles.Children.Add(videosBox);
		toggles.Children.Add(removeButton);
		Border border = new Border
		{
			Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(224, 20, 20, 24)),
			BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(102, 138, 106, 176)),
			BorderThickness = new Thickness(1),
			Padding = new Thickness(12),
			Margin = new Thickness(0, 0, 0, 12)
		};
		StackPanel body = new StackPanel();
		body.Children.Add(new TextBlock
		{
			Text = source.Name,
			FontSize = 18,
			FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
			Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(237, 237, 237))
		});
		body.Children.Add(toggles);
		body.Children.Add(new TextBlock
		{
			Text = source.RootPath,
			TextWrapping = TextWrapping.Wrap,
			Foreground = System.Windows.Media.Brushes.LightGray,
			Margin = new Thickness(0, 8, 0, 0)
		});
		List<FolderEditorNode> folderNodes = new List<FolderEditorNode>();
		Expander folderExpander = BuildFolderRulesEditor(source, folderNodes);
		if (folderExpander != null)
		{
			body.Children.Add(folderExpander);
		}
		border.Child = body;
		SourceEditorRow row = new SourceEditorRow
		{
			Source = source,
			EnabledBox = enabledBox,
			ImagesBox = imagesBox,
			VideosBox = videosBox,
			Root = border,
			FolderNodes = folderNodes
		};
		removeButton.Click += delegate
		{
			SourcesPanel.Children.Remove(border);
			sourceRows.Remove(row);
			UpdateSummary();
		};
		enabledBox.Click += delegate
		{
			UpdateSummary();
		};
		imagesBox.Click += delegate
		{
			UpdateSummary();
		};
		videosBox.Click += delegate
		{
			UpdateSummary();
		};
		sourceRows.Add(row);
		SourcesPanel.Children.Add(border);
	}

	private void AddFolder_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog dialog = new OpenFileDialog
		{
			CheckFileExists = false,
			CheckPathExists = true,
			ValidateNames = false,
			FileName = "Select this folder"
		};
		if (dialog.ShowDialog() == true)
		{
			string selectedPath = Path.GetDirectoryName(dialog.FileName);
			if (string.IsNullOrWhiteSpace(selectedPath))
			{
				return;
			}
			AddRow(new MediaSourceDefinition
			{
				Name = Path.GetFileName(selectedPath),
				RootPath = selectedPath,
				IsEnabled = true,
				ImagesEnabled = true,
				VideosEnabled = true,
				SortOrder = sourceRows.Count
			});
			UpdateSummary();
		}
	}

	private void Save_Click(object sender, RoutedEventArgs e)
	{
		SaveCurrentSourcesAndReload();
		System.Windows.MessageBox.Show("Media sources saved.", "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private async void ImportLegacyTags_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog tagsDialog = new OpenFileDialog
		{
			Title = "Select legacy tags.txt",
			Filter = "Legacy tags file (tags.txt)|tags.txt|Text files (*.txt)|*.txt|All files (*.*)|*.*",
			CheckFileExists = true
		};
		if (tagsDialog.ShowDialog() != true)
		{
			return;
		}
		bool overwriteExistingTags = MessageBox.Show("Overwrite current tags when the tags.txt file matches media that is already tagged?\n\nYes = replace current tags with tags.txt\nNo = only import tags onto currently untagged media", "Legacy tag import", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		List<string> oldRoots = new List<string>();
		if (MessageBox.Show("Do you want to select the old media folder that this tags.txt came from? This improves fingerprint matching if files moved.", "Legacy tag import", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
		{
			string oldRoot = PickFolder("Select old image/video folder root");
			if (!string.IsNullOrWhiteSpace(oldRoot))
			{
				oldRoots.Add(oldRoot);
			}
		}
		SetImportBusy(true, "Saving media sources before import...");
		try
		{
			SaveCurrentSourcesAndReload();
			Progress<string> progress = new Progress<string>(delegate(string message)
			{
				ImportStatusText.Text = message;
			});
			LegacyTagImportResult result = await Task.Run(delegate
			{
				return mediaCatalog.ImportLegacyTags(tagsDialog.FileName, oldRoots, progress, overwriteExistingTags);
			});
			ImportStatusText.Text = "Reloading media after import...";
			imageTagger.reloadImagesVideosTags();
			mainWindow.reloadImagesVideos();
			ImportStatusText.Text = "Import complete. Imported " + result.ImportedCount + " tags.";
			MessageBox.Show("Legacy tag import complete.\n\nMode: " + (overwriteExistingTags ? "Overwrite existing tags" : "Only fill untagged media") + "\nImported: " + result.ImportedCount + "\nExact: " + result.ExactMatches + "\nFingerprint: " + result.FingerprintMatches + "\nFilename: " + result.FilenameMatches + "\nExisting skipped: " + result.SkippedExistingCount + "\nAmbiguous skipped: " + result.AmbiguousCount + "\nUnmatched: " + result.UnmatchedCount + "\n\nReport: " + result.ReportPath, "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
		}
		catch (Exception ex)
		{
			MessageBox.Show("Legacy tag import failed:\n" + ex.Message, "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		finally
		{
			SetImportBusy(false, "");
		}
	}

	private void SetImportBusy(bool isBusy, string message)
	{
		ImportStatusPanel.Visibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
		ImportProgressBar.IsIndeterminate = isBusy;
		ImportStatusText.Text = message;
		ImportLegacyTagsButton.IsEnabled = !isBusy;
	}

	private void SaveCurrentSourcesAndReload()
	{
		List<MediaSourceDefinition> sources = new List<MediaSourceDefinition>();
		for (int i = 0; i < sourceRows.Count; i++)
		{
			SourceEditorRow row = sourceRows[i];
			row.Source.IsEnabled = row.EnabledBox.IsChecked == true;
			row.Source.ImagesEnabled = row.ImagesBox.IsChecked == true;
			row.Source.VideosEnabled = row.VideosBox.IsChecked == true;
			row.Source.SortOrder = i;
			if (row.FolderNodes.Count > 0)
			{
				row.Source.FolderRules = MergeFolderRules(row.Source.FolderRules, BuildFolderRules(row.FolderNodes), GetLoadedFolderStates(row.FolderNodes));
			}
			sources.Add(row.Source);
		}
		mediaCatalog.SaveSources(sources);
		mediaCatalog.Reload();
	}

	private static string PickFolder(string title)
	{
		OpenFileDialog dialog = new OpenFileDialog
		{
			Title = title,
			CheckFileExists = false,
			CheckPathExists = true,
			ValidateNames = false,
			FileName = "Select this folder"
		};
		if (dialog.ShowDialog() == true)
		{
			return Path.GetDirectoryName(dialog.FileName) ?? "";
		}
		return "";
	}

	private void Back_Click(object sender, RoutedEventArgs e)
	{
		NavigationService?.GoBack();
	}

	private void UpdateSummary()
	{
		int activeSources = sourceRows.Count(delegate(SourceEditorRow row)
		{
			return row.EnabledBox.IsChecked == true;
		});
		SummaryText.Text = "Choose which folders the app uses and whether each source contributes images and/or videos. Active sources: " + activeSources;
	}

	private Expander BuildFolderRulesEditor(MediaSourceDefinition source, List<FolderEditorNode> folderNodes)
	{
		if (!Directory.Exists(source.RootPath))
		{
			return null;
		}
		TreeView treeView = new TreeView
		{
			Margin = new Thickness(0, 8, 0, 0),
			Background = System.Windows.Media.Brushes.Transparent,
			BorderThickness = new Thickness(0)
		};
		foreach (string directory in GetSafeDirectories(source.RootPath).OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase).Take(500))
		{
			FolderEditorNode node = BuildFolderNode(source, directory);
			folderNodes.Add(node);
			treeView.Items.Add(BuildTreeViewItem(node));
		}
		if (treeView.Items.Count == 0)
		{
			return null;
		}
		return new Expander
		{
			Header = "Folder rules",
			Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(237, 237, 237)),
			Margin = new Thickness(0, 8, 0, 0),
			Content = treeView
		};
	}

	private FolderEditorNode BuildFolderNode(MediaSourceDefinition source, string directory)
	{
		string relativePath = NormalizeRelativeFolderPath(Path.GetRelativePath(source.RootPath, directory));
		CheckBox includeBox = new CheckBox
		{
			Content = Path.GetFileName(directory),
			IsChecked = IsFolderIncluded(source, relativePath),
			Foreground = System.Windows.Media.Brushes.White
		};
		FolderEditorNode node = new FolderEditorNode
		{
			Source = source,
			FullPath = directory,
			RelativeFolderPath = relativePath,
			IncludeBox = includeBox
		};
		includeBox.Click += delegate
		{
			SetLoadedChildFolderStates(node, includeBox.IsChecked == true);
		};
		return node;
	}

	private TreeViewItem BuildTreeViewItem(FolderEditorNode node)
	{
		TreeViewItem item = new TreeViewItem
		{
			Header = node.IncludeBox,
			IsExpanded = false,
			Foreground = System.Windows.Media.Brushes.White,
			Tag = node
		};
		if (GetSafeDirectories(node.FullPath).Any())
		{
			item.Items.Add(new TreeViewItem
			{
				Header = "Loading...",
				Foreground = System.Windows.Media.Brushes.Gray
			});
			item.Expanded += FolderTreeItem_Expanded;
		}
		return item;
	}

	private void FolderTreeItem_Expanded(object sender, RoutedEventArgs e)
	{
		if (!(sender is TreeViewItem item) || !(item.Tag is FolderEditorNode node) || node.ChildrenLoaded)
		{
			return;
		}
		node.ChildrenLoaded = true;
		item.Items.Clear();
		foreach (string childDirectory in GetSafeDirectories(node.FullPath).OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase).Take(500))
		{
			FolderEditorNode childNode = BuildFolderNode(node.Source, childDirectory);
			node.Children.Add(childNode);
			item.Items.Add(BuildTreeViewItem(childNode));
		}
	}

	private static void SetLoadedChildFolderStates(FolderEditorNode node, bool isIncluded)
	{
		foreach (FolderEditorNode child in node.Children)
		{
			child.IncludeBox.IsChecked = isIncluded;
			SetLoadedChildFolderStates(child, isIncluded);
		}
	}

	private static List<MediaFolderRule> BuildFolderRules(List<FolderEditorNode> rootNodes)
	{
		List<MediaFolderRule> rules = new List<MediaFolderRule>();
		foreach (FolderEditorNode node in rootNodes)
		{
			CollectFolderRules(node, parentIncluded: true, rules);
		}
		return rules;
	}

	private static void CollectFolderRules(FolderEditorNode node, bool parentIncluded, List<MediaFolderRule> rules)
	{
		bool currentIncluded = node.IncludeBox.IsChecked == true;
		if (currentIncluded != parentIncluded)
		{
			rules.Add(new MediaFolderRule
			{
				RelativeFolderPath = node.RelativeFolderPath,
				IsIncluded = currentIncluded
			});
		}
		foreach (FolderEditorNode child in node.Children)
		{
			CollectFolderRules(child, currentIncluded, rules);
		}
	}

	private static Dictionary<string, bool> GetLoadedFolderStates(List<FolderEditorNode> rootNodes)
	{
		Dictionary<string, bool> states = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
		foreach (FolderEditorNode node in rootNodes)
		{
			CollectLoadedFolderStates(node, states);
		}
		return states;
	}

	private static void CollectLoadedFolderStates(FolderEditorNode node, Dictionary<string, bool> states)
	{
		states[NormalizeRelativeFolderPath(node.RelativeFolderPath)] = node.IncludeBox.IsChecked == true;
		foreach (FolderEditorNode child in node.Children)
		{
			CollectLoadedFolderStates(child, states);
		}
	}

	private static List<MediaFolderRule> MergeFolderRules(List<MediaFolderRule> existingRules, List<MediaFolderRule> editedRules, Dictionary<string, bool> loadedStates)
	{
		HashSet<string> editedPaths = new HashSet<string>(editedRules.Select(delegate(MediaFolderRule rule)
		{
			return NormalizeRelativeFolderPath(rule.RelativeFolderPath);
		}), StringComparer.OrdinalIgnoreCase);
		List<MediaFolderRule> mergedRules = (existingRules ?? new List<MediaFolderRule>()).Where(delegate(MediaFolderRule rule)
		{
			string rulePath = NormalizeRelativeFolderPath(rule.RelativeFolderPath);
			if (editedPaths.Contains(rulePath))
			{
				return false;
			}
			foreach (KeyValuePair<string, bool> loadedState in loadedStates)
			{
				string loadedPath = loadedState.Key;
				bool loadedIncluded = loadedState.Value;
				bool exactLoadedPath = string.Equals(rulePath, loadedPath, StringComparison.OrdinalIgnoreCase);
				bool underIncludedLoadedPath = loadedIncluded && !string.IsNullOrEmpty(loadedPath) && rulePath.StartsWith(loadedPath + "/", StringComparison.OrdinalIgnoreCase);
				if (exactLoadedPath || underIncludedLoadedPath)
				{
					return false;
				}
			}
			return true;
		}).ToList();
		mergedRules.AddRange(editedRules);
		return mergedRules;
	}

	private static bool IsFolderIncluded(MediaSourceDefinition source, string relativeFolderPath)
	{
		if (source.FolderRules == null || source.FolderRules.Count == 0)
		{
			return true;
		}
		string normalizedFolderPath = NormalizeRelativeFolderPath(relativeFolderPath);
		bool isIncluded = true;
		int bestMatchLength = -1;
		foreach (MediaFolderRule rule in source.FolderRules)
		{
			if (!rule.IsIncluded.HasValue)
			{
				continue;
			}
			string rulePath = NormalizeRelativeFolderPath(rule.RelativeFolderPath);
			bool isMatch = string.IsNullOrEmpty(rulePath) || string.Equals(normalizedFolderPath, rulePath, StringComparison.OrdinalIgnoreCase) || normalizedFolderPath.StartsWith(rulePath + "/", StringComparison.OrdinalIgnoreCase);
			if (isMatch && rulePath.Length > bestMatchLength)
			{
				bestMatchLength = rulePath.Length;
				isIncluded = rule.IsIncluded.Value;
			}
		}
		return isIncluded;
	}

	private static string NormalizeRelativeFolderPath(string relativeFolderPath)
	{
		if (string.IsNullOrWhiteSpace(relativeFolderPath) || relativeFolderPath == ".")
		{
			return "";
		}
		return relativeFolderPath.Replace('\\', '/').Trim('/');
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

	private static MediaSourceDefinition CloneSource(MediaSourceDefinition source)
	{
		return new MediaSourceDefinition
		{
			Id = source.Id,
			Name = source.Name,
			RootPath = source.RootPath,
			IsEnabled = source.IsEnabled,
			ImagesEnabled = source.ImagesEnabled,
			VideosEnabled = source.VideosEnabled,
			IsLegacy = source.IsLegacy,
			SortOrder = source.SortOrder,
			FolderRules = source.FolderRules?.Select(delegate(MediaFolderRule rule)
			{
				return new MediaFolderRule
				{
					RelativeFolderPath = rule.RelativeFolderPath,
					IsIncluded = rule.IsIncluded
				};
			}).ToList() ?? new List<MediaFolderRule>()
		};
	}
}
