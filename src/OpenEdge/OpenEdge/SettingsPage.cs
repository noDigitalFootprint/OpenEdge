using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;

namespace OpenEdge;

public partial class SettingsPage : Page, IComponentConnector
{
	private sealed class SettingEditorRow
	{
		public SettingDefinition Definition { get; init; }

		public CheckBox ToggleBox { get; init; }

		public TextBox ValueBox { get; init; }

		public TextBlock AnsweredText { get; init; }

		public TextBlock RelatedStateText { get; init; }
	}

	private static readonly IReadOnlySet<string> StructuredSettingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"cuck", "petPlay", "chastity", "anal", "noVideo", "outsideSession", "LOB", "censorship", "breathPlay", "gay"
	};

	private readonly SettingsRegistry settingsRegistry;

	private readonly Page1 page1;

	private readonly List<SettingEditorRow> rows = new List<SettingEditorRow>();

	private CheckBox cuckEnabledBox;

	private TextBlock cuckAnsweredText;

	private TextBox cuckStageBox;

	private CheckBox cuckFridayPassedBox;

	private CheckBox petPlayEnabledBox;

	private TextBlock petPlayAnsweredText;

	private CheckBox petPlayAdvancedBox;

	private TextBlock petPlayAdvancedAnsweredText;

	private CheckBox petPlayCollarBox;

	private CheckBox petPlayTreatsBox;

	private ComboBox petPlayPersonaBox;

	private TextBox petPlayPetNameBox;

	private TextBox petPlaySubNameBox;

	private CheckBox chastityEnabledBox;

	private CheckBox chastityCageBox;

	private CheckBox chastityWearingBox;

	private CheckBox chastityVibratorBox;

	private CheckBox chastityLostKeyBox;

	private CheckBox chastityToldAboutNecklaceBox;

	private TextBlock chastityAnsweredText;

	private TextBox chastityCageTypeBox;

	private TextBox chastityDurationBox;

	private TextBox chastityDateBox;

	private CheckBox analEnabledBox;

	private TextBlock analAnsweredText;

	private ComboBox analExperienceBox;

	private ComboBox analPreferenceBox;

	private CheckBox analFirstSessionBox;

	private CheckBox analTrainingBox;

	private CheckBox analTrainingDeclinedBox;

	private CheckBox analWaterLubeBox;

	private CheckBox analDildoBox;

	private CheckBox analPlugBox;

	private CheckBox analProstateOrgasmBox;

	private CheckBox lobEnabledBox;

	private TextBlock lobAnsweredText;

	private CheckBox lobRuntimeBox;

	private TextBox lobEarlyBox;

	private TextBox lobLateBox;

	private CheckBox censorshipEnabledBox;

	private TextBlock censorshipAnsweredText;

	private TextBox censorshipIntensityBox;

	private CheckBox breathPlayEnabledBox;

	private TextBlock breathPlayAnsweredText;

	private TextBox breathPlayTimeBox;

	private CheckBox outsideSessionEnabledBox;

	private TextBlock outsideSessionAnsweredText;

	private TextBox outsideSessionNoPornBox;

	private TextBox outsideSessionConstantCeiBox;

	private TextBox outsideSessionPlugHourBox;

	private TextBox outsideSessionWatchPornBox;

	private TextBox outsideSessionHypnoFilesBox;

	private CheckBox noVideoEnabledBox;

	private TextBlock noVideoAnsweredText;

	private TextBox noVideoValueBox;

	private CheckBox gayEnabledBox;

	private TextBlock gayAnsweredText;

	private CheckBox gayHumiliationBox;

	private TextBlock gayHumiliationAnsweredText;

	public SettingsPage(SettingsRegistry settingsRegistry, Page1 page1)
	{
		this.settingsRegistry = settingsRegistry;
		this.page1 = page1;
		InitializeComponent();
		LoadRows();
	}

	private void LoadRows()
	{
		rows.Clear();
		SettingsPanel.Children.Clear();
		AddStructuredSection();
		AddSection("Preferences & Capabilities", GetGenericDefinitions(SettingKind.Toggle));
		AddSection("Progression Modifiers", GetGenericDefinitions(SettingKind.Numeric));
		AddSection("Identity & Text", GetGenericDefinitions(SettingKind.Text));
		AddModSettingsSection(GetModDefinitions());
		AddCompatibilityAdvancedSection();
		UpdateSummary();
	}

	private IEnumerable<SettingDefinition> GetGenericDefinitions(SettingKind kind)
	{
		return settingsRegistry.GetDefinitions().Where(delegate(SettingDefinition item)
		{
			return string.IsNullOrWhiteSpace(item.SourceName) && item.Kind == kind && !settingsRegistry.IsStructuredChildDefinition(item.Key) && (kind != SettingKind.Toggle || !StructuredSettingKeys.Contains(item.Key));
		});
	}

	private IEnumerable<SettingDefinition> GetModDefinitions()
	{
		return settingsRegistry.GetDefinitions().Where((SettingDefinition item) => !string.IsNullOrWhiteSpace(item.SourceName));
	}

	private string GetAnsweredStatusText(string key, bool answered)
	{
		if (answered)
		{
			return "Answered";
		}
		if (settingsRegistry.IsAskQueued(key))
		{
			return "Unanswered · queued";
		}
		return "Unanswered";
	}

	private Brush GetAnsweredStatusBrush(string key, bool answered)
	{
		if (answered)
		{
			return Brushes.LightGreen;
		}
		if (settingsRegistry.IsAskQueued(key))
		{
			return Brushes.Gold;
		}
		return Brushes.Orange;
	}

	private void QueueSettingAsk(string key, ICollection<string> queuedAskKeys)
	{
		settingsRegistry.QueueAsk(key);
		if (!queuedAskKeys.Any((string existingKey) => string.Equals(existingKey, key, StringComparison.OrdinalIgnoreCase)))
		{
			queuedAskKeys.Add(key);
		}
	}

	private void AddStructuredSection()
	{
		Border border = new Border
		{
			Background = new SolidColorBrush(Color.FromArgb(224, 20, 20, 24)),
			BorderBrush = new SolidColorBrush(Color.FromArgb(80, 138, 106, 176)),
			BorderThickness = new Thickness(1.0),
			CornerRadius = new CornerRadius(10.0),
			Padding = new Thickness(16.0),
			Margin = new Thickness(0.0, 0.0, 0.0, 16.0)
		};
		StackPanel stackPanel = new StackPanel();
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Structured Progression Bundles",
			Foreground = new SolidColorBrush(Color.FromRgb(237, 237, 237)),
			FontSize = 21.0,
			FontFamily = new FontFamily("Segoe UI"),
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		});
		stackPanel.Children.Add(new TextBlock
		{
			Text = "These settings carry additional progression, identity, or schedule state and should not be treated as simple toggles.",
			Foreground = new SolidColorBrush(Color.FromRgb(191, 175, 224)),
			FontSize = 13.0,
			TextWrapping = TextWrapping.Wrap,
			Margin = new Thickness(0.0, 0.0, 0.0, 12.0)
		});
		stackPanel.Children.Add(CreateCuckEditor());
		stackPanel.Children.Add(CreatePetPlayEditor());
		stackPanel.Children.Add(CreateChastityEditor());
		stackPanel.Children.Add(CreateAnalEditor());
		stackPanel.Children.Add(CreateNoVideoEditor());
		stackPanel.Children.Add(CreateLobEditor());
		stackPanel.Children.Add(CreateCensorshipEditor());
		stackPanel.Children.Add(CreateBreathPlayEditor());
		stackPanel.Children.Add(CreateOutsideSessionEditor());
		stackPanel.Children.Add(CreateGayEditor());
		border.Child = stackPanel;
		SettingsPanel.Children.Add(border);
	}

	private FrameworkElement CreateCuckEditor()
	{
		CuckSettingState cuckState = settingsRegistry.GetCuckState();
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Cuck",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		cuckAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("cuck", cuckState.Answered),
			Foreground = GetAnsweredStatusBrush("cuck", cuckState.Answered),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(cuckAnsweredText);
		WrapPanel wrapPanel = new WrapPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		cuckEnabledBox = new CheckBox
		{
			Content = "Enabled",
			IsChecked = cuckState.Enabled,
			Foreground = Brushes.White,
			Margin = new Thickness(0.0, 0.0, 18.0, 0.0)
		};
		wrapPanel.Children.Add(cuckEnabledBox);
		cuckFridayPassedBox = new CheckBox
		{
			Content = "Friday passed",
			IsChecked = cuckState.FridayPassed,
			Foreground = Brushes.White,
			Margin = new Thickness(0.0, 0.0, 18.0, 0.0)
		};
		wrapPanel.Children.Add(cuckFridayPassedBox);
		wrapPanel.Children.Add(new TextBlock
		{
			Text = "Stage",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		cuckStageBox = CreateEditorTextBox(cuckState.Stage.ToString(CultureInfo.InvariantCulture), 80.0);
		wrapPanel.Children.Add(cuckStageBox);
		stackPanel.Children.Add(wrapPanel);
		return stackPanel;
	}

	private FrameworkElement CreatePetPlayEditor()
	{
		PetPlaySettingState petPlayState = settingsRegistry.GetPetPlayState();
		StackPanel stackPanel = new StackPanel();
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Pet Play",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		petPlayAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("petPlay", petPlayState.Answered),
			Foreground = GetAnsweredStatusBrush("petPlay", petPlayState.Answered),
			FontSize = 13.0
		};
		petPlayAdvancedAnsweredText = new TextBlock
		{
			Text = (petPlayState.AdvancedAnswered ? "Advanced: Answered" : (settingsRegistry.IsAskQueued("petPlayAdvanced") ? "Advanced: Unanswered · queued" : "Advanced: Unanswered")),
			Foreground = (petPlayState.AdvancedAnswered ? Brushes.LightGreen : (settingsRegistry.IsAskQueued("petPlayAdvanced") ? Brushes.Gold : Brushes.Orange)),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(petPlayAnsweredText);
		stackPanel.Children.Add(petPlayAdvancedAnsweredText);
		WrapPanel wrapPanel = new WrapPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		petPlayEnabledBox = new CheckBox
		{
			Content = "Enabled",
			IsChecked = petPlayState.Enabled,
			Foreground = Brushes.White,
			Margin = new Thickness(0.0, 0.0, 18.0, 0.0)
		};
		wrapPanel.Children.Add(petPlayEnabledBox);
		petPlayAdvancedBox = new CheckBox
		{
			Content = "Advanced enabled",
			IsChecked = petPlayState.AdvancedEnabled,
			Foreground = Brushes.White,
			Margin = new Thickness(0.0, 0.0, 18.0, 0.0)
		};
		wrapPanel.Children.Add(petPlayAdvancedBox);
		petPlayCollarBox = CreateDarkCheckBox("Collar", petPlayState.CollarEnabled);
		petPlayTreatsBox = CreateDarkCheckBox("Treats", petPlayState.TreatsEnabled);
		wrapPanel.Children.Add(petPlayCollarBox);
		wrapPanel.Children.Add(petPlayTreatsBox);
		wrapPanel.Children.Add(new TextBlock
		{
			Text = "Persona",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		petPlayPersonaBox = CreateDarkComboBox(120.0);
		petPlayPersonaBox.Items.Add("None");
		petPlayPersonaBox.Items.Add("Pup");
		petPlayPersonaBox.Items.Add("Cat");
		petPlayPersonaBox.SelectedItem = petPlayState.Persona;
		wrapPanel.Children.Add(petPlayPersonaBox);
		stackPanel.Children.Add(wrapPanel);
		Grid grid = new Grid
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 4.0)
		};
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition());
		grid.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		grid.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		TextBlock textBlock = new TextBlock
		{
			Text = "Pet name",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 6.0)
		};
		grid.Children.Add(textBlock);
		petPlayPetNameBox = CreateEditorTextBox(petPlayState.PetName, 220.0);
		Grid.SetColumn(petPlayPetNameBox, 1);
		grid.Children.Add(petPlayPetNameBox);
		TextBlock textBlock2 = new TextBlock
		{
			Text = "Human name",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 0.0)
		};
		Grid.SetRow(textBlock2, 1);
		grid.Children.Add(textBlock2);
		petPlaySubNameBox = CreateEditorTextBox(petPlayState.SubName, 220.0);
		Grid.SetColumn(petPlaySubNameBox, 1);
		Grid.SetRow(petPlaySubNameBox, 1);
		grid.Children.Add(petPlaySubNameBox);
		stackPanel.Children.Add(grid);
		return stackPanel;
	}

	private FrameworkElement CreateChastityEditor()
	{
		ChastitySettingState chastityState = settingsRegistry.GetChastityState();
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Chastity",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		chastityAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("chastity", chastityState.Answered),
			Foreground = GetAnsweredStatusBrush("chastity", chastityState.Answered),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(chastityAnsweredText);
		WrapPanel wrapPanel = new WrapPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		chastityEnabledBox = CreateDarkCheckBox("Enabled", chastityState.Enabled);
		chastityCageBox = CreateDarkCheckBox("Owns cage", chastityState.CageOwned);
		chastityWearingBox = CreateDarkCheckBox("Currently wearing cage", chastityState.WearingCage);
		chastityVibratorBox = CreateDarkCheckBox("Owns vibrator", chastityState.VibratorOwned);
		wrapPanel.Children.Add(chastityEnabledBox);
		wrapPanel.Children.Add(chastityCageBox);
		wrapPanel.Children.Add(chastityWearingBox);
		wrapPanel.Children.Add(chastityVibratorBox);
		stackPanel.Children.Add(wrapPanel);
		WrapPanel wrapPanel2 = new WrapPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		chastityLostKeyBox = CreateDarkCheckBox("Lost key", chastityState.LostKey);
		chastityToldAboutNecklaceBox = CreateDarkCheckBox("Told about necklace", chastityState.ToldAboutNecklace);
		wrapPanel2.Children.Add(chastityLostKeyBox);
		wrapPanel2.Children.Add(chastityToldAboutNecklaceBox);
		wrapPanel2.Children.Add(new TextBlock
		{
			Text = "Duration (days)",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		chastityDurationBox = CreateEditorTextBox(chastityState.DurationDays.ToString(CultureInfo.InvariantCulture), 80.0);
		wrapPanel2.Children.Add(chastityDurationBox);
		stackPanel.Children.Add(wrapPanel2);
		Grid grid = new Grid
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 4.0)
		};
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition());
		grid.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		grid.RowDefinitions.Add(new RowDefinition
		{
			Height = GridLength.Auto
		});
		TextBlock textBlock = new TextBlock
		{
			Text = "Cage type",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 6.0)
		};
		grid.Children.Add(textBlock);
		chastityCageTypeBox = CreateEditorTextBox(chastityState.CageType, 220.0);
		Grid.SetColumn(chastityCageTypeBox, 1);
		grid.Children.Add(chastityCageTypeBox);
		TextBlock textBlock2 = new TextBlock
		{
			Text = "Start date",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 0.0)
		};
		Grid.SetRow(textBlock2, 1);
		grid.Children.Add(textBlock2);
		chastityDateBox = CreateEditorTextBox(chastityState.StartDateText, 220.0);
		Grid.SetColumn(chastityDateBox, 1);
		Grid.SetRow(chastityDateBox, 1);
		grid.Children.Add(chastityDateBox);
		stackPanel.Children.Add(grid);
		return stackPanel;
	}

	private FrameworkElement CreateNoVideoEditor()
	{
		NoVideoSettingState noVideoState = settingsRegistry.GetNoVideoState();
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};
		stackPanel.Children.Add(new TextBlock
		{
			Text = "No Video",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		noVideoAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("noVideo", noVideoState.Answered),
			Foreground = GetAnsweredStatusBrush("noVideo", noVideoState.Answered),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(noVideoAnsweredText);
		WrapPanel wrapPanel = new WrapPanel();
		noVideoEnabledBox = CreateDarkCheckBox("Enabled", noVideoState.Enabled);
		wrapPanel.Children.Add(noVideoEnabledBox);
		wrapPanel.Children.Add(new TextBlock
		{
			Text = "Duration (days)",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		noVideoValueBox = CreateEditorTextBox(noVideoState.DurationDays.ToString(CultureInfo.InvariantCulture), 80.0);
		wrapPanel.Children.Add(noVideoValueBox);
		stackPanel.Children.Add(wrapPanel);
		return stackPanel;
	}

	private FrameworkElement CreateAnalEditor()
	{
		AnalSettingState analState = settingsRegistry.GetAnalState();
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Anal",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		analAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("anal", analState.Answered),
			Foreground = GetAnsweredStatusBrush("anal", analState.Answered),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(analAnsweredText);
		WrapPanel wrapPanel = new WrapPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		analEnabledBox = new CheckBox
		{
			Content = "Enabled",
			IsChecked = analState.Enabled,
			Foreground = Brushes.White,
			Margin = new Thickness(0.0, 0.0, 18.0, 0.0)
		};
		wrapPanel.Children.Add(analEnabledBox);
		wrapPanel.Children.Add(new TextBlock
		{
			Text = "Experience",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		analExperienceBox = CreateDarkComboBox(140.0);
		analExperienceBox.Items.Add("Unknown");
		analExperienceBox.Items.Add("Beginner");
		analExperienceBox.Items.Add("Experienced");
		analExperienceBox.SelectedItem = analState.Experience;
		wrapPanel.Children.Add(analExperienceBox);
		stackPanel.Children.Add(wrapPanel);
		WrapPanel wrapPanel2 = new WrapPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		analFirstSessionBox = CreateDarkCheckBox("First session done", analState.FirstSessionCompleted);
		analTrainingBox = CreateDarkCheckBox("Training active", analState.TrainingEnabled);
		analTrainingDeclinedBox = CreateDarkCheckBox("Training declined", analState.TrainingDeclined);
		wrapPanel2.Children.Add(analFirstSessionBox);
		wrapPanel2.Children.Add(analTrainingBox);
		wrapPanel2.Children.Add(analTrainingDeclinedBox);
		wrapPanel2.Children.Add(new TextBlock
		{
			Text = "Preference",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		analPreferenceBox = CreateDarkComboBox(120.0);
		analPreferenceBox.Items.Add("Unknown");
		analPreferenceBox.Items.Add("Like");
		analPreferenceBox.Items.Add("Neutral");
		analPreferenceBox.Items.Add("Dislike");
		analPreferenceBox.SelectedItem = analState.Preference;
		wrapPanel2.Children.Add(analPreferenceBox);
		stackPanel.Children.Add(wrapPanel2);
		WrapPanel wrapPanel3 = new WrapPanel();
		analWaterLubeBox = CreateDarkCheckBox("Water lube", analState.WaterLubeEnabled);
		analDildoBox = CreateDarkCheckBox("Dildo", analState.DildoEnabled);
		analPlugBox = CreateDarkCheckBox("Plug", analState.PlugEnabled);
		analProstateOrgasmBox = CreateDarkCheckBox("Prostate orgasm", analState.ProstateOrgasmEnabled);
		wrapPanel3.Children.Add(analWaterLubeBox);
		wrapPanel3.Children.Add(analDildoBox);
		wrapPanel3.Children.Add(analPlugBox);
		wrapPanel3.Children.Add(analProstateOrgasmBox);
		stackPanel.Children.Add(wrapPanel3);
		return stackPanel;
	}

	private FrameworkElement CreateLobEditor()
	{
		LobSettingState lobState = settingsRegistry.GetLobState();
		StackPanel stackPanel = new StackPanel();
		stackPanel.Children.Add(new TextBlock
		{
			Text = "LOB",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		lobAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("LOB", lobState.Answered),
			Foreground = GetAnsweredStatusBrush("LOB", lobState.Answered),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(lobAnsweredText);
		WrapPanel wrapPanel = new WrapPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		lobEnabledBox = CreateDarkCheckBox("Enabled", lobState.Enabled);
		lobRuntimeBox = CreateDarkCheckBox("Runtime enabled", lobState.RuntimeEnabled);
		wrapPanel.Children.Add(lobEnabledBox);
		wrapPanel.Children.Add(lobRuntimeBox);
		stackPanel.Children.Add(wrapPanel);
		WrapPanel wrapPanel2 = new WrapPanel();
		wrapPanel2.Children.Add(new TextBlock
		{
			Text = "Earliest hour",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		lobEarlyBox = CreateEditorTextBox(lobState.EarlyHour.ToString(CultureInfo.InvariantCulture), 80.0);
		wrapPanel2.Children.Add(lobEarlyBox);
		wrapPanel2.Children.Add(new TextBlock
		{
			Text = "Latest hour",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(18.0, 0.0, 8.0, 0.0)
		});
		lobLateBox = CreateEditorTextBox(lobState.LateHour.ToString(CultureInfo.InvariantCulture), 80.0);
		wrapPanel2.Children.Add(lobLateBox);
		stackPanel.Children.Add(wrapPanel2);
		return stackPanel;
	}

	private FrameworkElement CreateCensorshipEditor()
	{
		CensorshipSettingState censorshipState = settingsRegistry.GetCensorshipState();
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Censorship",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		censorshipAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("censorship", censorshipState.Answered),
			Foreground = GetAnsweredStatusBrush("censorship", censorshipState.Answered),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(censorshipAnsweredText);
		WrapPanel wrapPanel = new WrapPanel();
		censorshipEnabledBox = CreateDarkCheckBox("Enabled", censorshipState.Enabled);
		wrapPanel.Children.Add(censorshipEnabledBox);
		wrapPanel.Children.Add(new TextBlock
		{
			Text = "Intensity",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		censorshipIntensityBox = CreateEditorTextBox(censorshipState.Intensity.ToString(CultureInfo.InvariantCulture), 80.0);
		wrapPanel.Children.Add(censorshipIntensityBox);
		stackPanel.Children.Add(wrapPanel);
		return stackPanel;
	}

	private FrameworkElement CreateBreathPlayEditor()
	{
		BreathPlaySettingState breathPlayState = settingsRegistry.GetBreathPlayState();
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Breath Play",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		breathPlayAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("breathPlay", breathPlayState.Answered),
			Foreground = GetAnsweredStatusBrush("breathPlay", breathPlayState.Answered),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(breathPlayAnsweredText);
		WrapPanel wrapPanel = new WrapPanel();
		breathPlayEnabledBox = CreateDarkCheckBox("Enabled", breathPlayState.Enabled);
		wrapPanel.Children.Add(breathPlayEnabledBox);
		wrapPanel.Children.Add(new TextBlock
		{
			Text = "Breath time (s)",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 8.0, 0.0)
		});
		breathPlayTimeBox = CreateEditorTextBox(breathPlayState.BreathTimeSeconds.ToString(CultureInfo.InvariantCulture), 80.0);
		wrapPanel.Children.Add(breathPlayTimeBox);
		stackPanel.Children.Add(wrapPanel);
		return stackPanel;
	}

	private FrameworkElement CreateOutsideSessionEditor()
	{
		OutsideSessionSettingState outsideSessionState = settingsRegistry.GetOutsideSessionState();
		StackPanel stackPanel = new StackPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 14.0)
		};
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Outside Session",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		outsideSessionAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("outsideSession", outsideSessionState.Answered),
			Foreground = GetAnsweredStatusBrush("outsideSession", outsideSessionState.Answered),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(outsideSessionAnsweredText);
		WrapPanel wrapPanel = new WrapPanel
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		};
		outsideSessionEnabledBox = CreateDarkCheckBox("Enabled", outsideSessionState.Enabled);
		wrapPanel.Children.Add(outsideSessionEnabledBox);
		stackPanel.Children.Add(wrapPanel);
		Grid grid = new Grid();
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition());
		for (int i = 0; i < 5; i++)
		{
			grid.RowDefinitions.Add(new RowDefinition
			{
				Height = GridLength.Auto
			});
		}
		TextBlock textBlock = new TextBlock
		{
			Text = "No porn remaining",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 6.0)
		};
		grid.Children.Add(textBlock);
		outsideSessionNoPornBox = CreateEditorTextBox(outsideSessionState.NoPornRemaining.ToString(CultureInfo.InvariantCulture), 120.0);
		Grid.SetColumn(outsideSessionNoPornBox, 1);
		grid.Children.Add(outsideSessionNoPornBox);
		TextBlock textBlock2 = new TextBlock
		{
			Text = "Constant CEI remaining",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 6.0)
		};
		Grid.SetRow(textBlock2, 1);
		grid.Children.Add(textBlock2);
		outsideSessionConstantCeiBox = CreateEditorTextBox(outsideSessionState.ConstantCeiRemaining.ToString(CultureInfo.InvariantCulture), 120.0);
		Grid.SetColumn(outsideSessionConstantCeiBox, 1);
		Grid.SetRow(outsideSessionConstantCeiBox, 1);
		grid.Children.Add(outsideSessionConstantCeiBox);
		TextBlock textBlock3 = new TextBlock
		{
			Text = "Plug hour remaining",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 6.0)
		};
		Grid.SetRow(textBlock3, 2);
		grid.Children.Add(textBlock3);
		outsideSessionPlugHourBox = CreateEditorTextBox(outsideSessionState.PlugHourRemaining.ToString(CultureInfo.InvariantCulture), 120.0);
		Grid.SetColumn(outsideSessionPlugHourBox, 1);
		Grid.SetRow(outsideSessionPlugHourBox, 2);
		grid.Children.Add(outsideSessionPlugHourBox);
		TextBlock textBlock4 = new TextBlock
		{
			Text = "Watch porn remaining",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 6.0)
		};
		Grid.SetRow(textBlock4, 3);
		grid.Children.Add(textBlock4);
		outsideSessionWatchPornBox = CreateEditorTextBox(outsideSessionState.WatchPornRemaining.ToString(CultureInfo.InvariantCulture), 120.0);
		Grid.SetColumn(outsideSessionWatchPornBox, 1);
		Grid.SetRow(outsideSessionWatchPornBox, 3);
		grid.Children.Add(outsideSessionWatchPornBox);
		TextBlock textBlock5 = new TextBlock
		{
			Text = "Hypno files remaining",
			Foreground = Brushes.White,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(0.0, 0.0, 10.0, 0.0)
		};
		Grid.SetRow(textBlock5, 4);
		grid.Children.Add(textBlock5);
		outsideSessionHypnoFilesBox = CreateEditorTextBox(outsideSessionState.HypnoFilesRemaining.ToString(CultureInfo.InvariantCulture), 120.0);
		Grid.SetColumn(outsideSessionHypnoFilesBox, 1);
		Grid.SetRow(outsideSessionHypnoFilesBox, 4);
		grid.Children.Add(outsideSessionHypnoFilesBox);
		stackPanel.Children.Add(grid);
		return stackPanel;
	}

	private FrameworkElement CreateGayEditor()
	{
		GaySettingState gayState = settingsRegistry.GetGayState();
		StackPanel stackPanel = new StackPanel();
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Gay",
			Foreground = Brushes.White,
			FontSize = 20.0,
			FontFamily = new FontFamily("Times New Roman")
		});
		gayAnsweredText = new TextBlock
		{
			Text = GetAnsweredStatusText("gay", gayState.Answered),
			Foreground = GetAnsweredStatusBrush("gay", gayState.Answered),
			FontSize = 13.0
		};
		gayHumiliationAnsweredText = new TextBlock
		{
			Text = "Humiliation: " + (gayState.HumiliationAnswered ? "Answered" : "Unanswered"),
			Foreground = (gayState.HumiliationAnswered ? Brushes.LightGreen : Brushes.Orange),
			FontSize = 13.0,
			Margin = new Thickness(0.0, 0.0, 0.0, 8.0)
		};
		stackPanel.Children.Add(gayAnsweredText);
		stackPanel.Children.Add(gayHumiliationAnsweredText);
		WrapPanel wrapPanel = new WrapPanel();
		gayEnabledBox = CreateDarkCheckBox("Enabled", gayState.Enabled);
		gayHumiliationBox = CreateDarkCheckBox("Humiliation enabled", gayState.HumiliationEnabled);
		wrapPanel.Children.Add(gayEnabledBox);
		wrapPanel.Children.Add(gayHumiliationBox);
		stackPanel.Children.Add(wrapPanel);
		return stackPanel;
	}

	private ComboBox CreateDarkComboBox(double width)
	{
		ComboBox comboBox = new ComboBox
		{
			Width = width,
			Background = new SolidColorBrush(Color.FromRgb(28, 28, 34)),
			Foreground = Brushes.White,
			BorderBrush = new SolidColorBrush(Color.FromArgb(102, 138, 106, 176))
		};
		comboBox.Style = (Style)FindResource("DarkComboBoxStyle");
		comboBox.ItemContainerStyle = (Style)FindResource("DarkComboBoxItemStyle");
		return comboBox;
	}

	private static CheckBox CreateDarkCheckBox(string label, bool isChecked)
	{
		return new CheckBox
		{
			Content = label,
			IsChecked = isChecked,
			Foreground = new SolidColorBrush(Color.FromRgb(237, 237, 237)),
			Margin = new Thickness(0.0, 0.0, 18.0, 0.0),
			FontFamily = new FontFamily("Segoe UI"),
			FontSize = 13.0
		};
	}

	private static TextBox CreateEditorTextBox(string text, double width)
	{
		return new TextBox
		{
			Text = text ?? "",
			Background = new SolidColorBrush(Color.FromRgb(28, 28, 34)),
			Foreground = Brushes.White,
			BorderBrush = new SolidColorBrush(Color.FromArgb(102, 138, 106, 176)),
			CaretBrush = Brushes.White,
			Width = width,
			Padding = new Thickness(8.0, 4.0, 8.0, 4.0),
			FontSize = 13.0,
			FontFamily = new FontFamily("Segoe UI")
		};
	}

	private void SaveHiddenToggleSetting(string key, bool enabled, ICollection<string> queuedAskKeys)
	{
		if (!settingsRegistry.IsAnswered(key) && settingsRegistry.SupportsQueuedAsk(key))
		{
			if (enabled)
			{
				QueueSettingAsk(key, queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk(key);
			}
			return;
		}
		if (!settingsRegistry.IsAnswered(key) && enabled == settingsRegistry.IsEnabled(key))
		{
			return;
		}
		settingsRegistry.SetEnabled(key, enabled);
	}

	private static int ParseEditorNumericValue(string value)
	{
		return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;
	}

	private void SaveHiddenNumericSetting(string key, string value)
	{
		if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
		{
			settingsRegistry.SetNumericValue(key, result);
		}
	}

	private void SaveHiddenTextSetting(string key, string value)
	{
		settingsRegistry.SetRawValue(key, value);
	}

	private Border CreateSectionBorder()
	{
		return new Border
		{
			Background = new SolidColorBrush(Color.FromArgb(224, 20, 20, 24)),
			BorderBrush = new SolidColorBrush(Color.FromArgb(70, 138, 106, 176)),
			BorderThickness = new Thickness(1.0),
			CornerRadius = new CornerRadius(10.0),
			Padding = new Thickness(16.0),
			Margin = new Thickness(0.0, 0.0, 0.0, 16.0)
		};
	}

	private void AddSection(string title, IEnumerable<SettingDefinition> definitions)
	{
		Border border = CreateSectionBorder();
		StackPanel stackPanel = new StackPanel();
		stackPanel.Children.Add(new TextBlock
		{
			Text = title,
			Foreground = new SolidColorBrush(Color.FromRgb(237, 237, 237)),
			FontSize = 21.0,
			FontFamily = new FontFamily("Segoe UI"),
			Margin = new Thickness(0.0, 0.0, 0.0, 12.0)
		});
		foreach (SettingDefinition item in definitions.OrderBy((SettingDefinition item) => item.Label, StringComparer.OrdinalIgnoreCase))
		{
			stackPanel.Children.Add(CreateRow(item));
		}
		border.Child = stackPanel;
		SettingsPanel.Children.Add(border);
	}

	private void AddModSettingsSection(IEnumerable<SettingDefinition> definitions)
	{
		List<SettingDefinition> modDefinitions = definitions.ToList();
		if (modDefinitions.Count == 0)
		{
			return;
		}
		Border border = CreateSectionBorder();
		StackPanel stackPanel = new StackPanel();
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Mod Settings",
			Foreground = new SolidColorBrush(Color.FromRgb(237, 237, 237)),
			FontSize = 21.0,
			FontFamily = new FontFamily("Segoe UI"),
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		});
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Settings added by enabled mods are grouped here to keep core progression readable.",
			Foreground = Brushes.LightGray,
			FontSize = 13.0,
			TextWrapping = TextWrapping.Wrap,
			Margin = new Thickness(0.0, 0.0, 0.0, 12.0)
		});
		foreach (IGrouping<string, SettingDefinition> group in modDefinitions.GroupBy(GetModGroupKey, StringComparer.OrdinalIgnoreCase).OrderBy((IGrouping<string, SettingDefinition> group) => group.Key, StringComparer.OrdinalIgnoreCase))
		{
			string headerText = BuildModSettingsHeader(group);
			StackPanel modPanel = new StackPanel
			{
				Margin = new Thickness(8.0, 8.0, 0.0, 0.0)
			};
			foreach (SettingDefinition item in group.OrderBy((SettingDefinition item) => item.Kind).ThenBy((SettingDefinition item) => item.Label, StringComparer.OrdinalIgnoreCase))
			{
				modPanel.Children.Add(CreateRow(item));
			}
			stackPanel.Children.Add(new Expander
			{
				Header = headerText,
				Foreground = Brushes.White,
				Background = new SolidColorBrush(Color.FromArgb(80, 36, 30, 44)),
				BorderBrush = new SolidColorBrush(Color.FromArgb(70, 138, 106, 176)),
				BorderThickness = new Thickness(1.0),
				Padding = new Thickness(8.0),
				Margin = new Thickness(0.0, 0.0, 0.0, 8.0),
				IsExpanded = false,
				Content = modPanel
			});
		}
		border.Child = stackPanel;
		SettingsPanel.Children.Add(border);
	}

	private static string GetModGroupKey(SettingDefinition definition)
	{
		string source = string.IsNullOrWhiteSpace(definition.SourceName) ? "Mod" : definition.SourceName.Trim();
		string group = string.IsNullOrWhiteSpace(definition.Group) ? "General" : definition.Group.Trim();
		return source + " / " + group;
	}

	private static string BuildModSettingsHeader(IEnumerable<SettingDefinition> definitions)
	{
		List<SettingDefinition> list = definitions.ToList();
		string sourceName = GetModGroupKey(list.FirstOrDefault() ?? new SettingDefinition { SourceName = "Mod" });
		int toggles = list.Count((SettingDefinition item) => item.Kind == SettingKind.Toggle);
		int numbers = list.Count((SettingDefinition item) => item.Kind == SettingKind.Numeric);
		int text = list.Count((SettingDefinition item) => item.Kind == SettingKind.Text);
		List<string> parts = new List<string>();
		if (toggles > 0)
		{
			parts.Add(toggles + " toggle" + (toggles == 1 ? "" : "s"));
		}
		if (numbers > 0)
		{
			parts.Add(numbers + " number" + (numbers == 1 ? "" : "s"));
		}
		if (text > 0)
		{
			parts.Add(text + " text" + (text == 1 ? "" : " values"));
		}
		return sourceName + " (" + string.Join(", ", parts) + ")";
	}

	private void AddCompatibilityAdvancedSection()
	{
		Border border = CreateSectionBorder();
		StackPanel stackPanel = new StackPanel();
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Compatibility / Advanced",
			Foreground = new SolidColorBrush(Color.FromRgb(237, 237, 237)),
			FontSize = 21.0,
			FontFamily = new FontFamily("Segoe UI"),
			Margin = new Thickness(0.0, 0.0, 0.0, 6.0)
		});
		stackPanel.Children.Add(new TextBlock
		{
			Text = "Read-only view of canonical setting keys and their legacy aliases. Old scripts and old installs may still use these names, but new content should use canonical SET*/ISSETTING commands.",
			Foreground = Brushes.LightGray,
			FontSize = 13.0,
			TextWrapping = TextWrapping.Wrap,
			Margin = new Thickness(0.0, 0.0, 0.0, 12.0)
		});
		StackPanel aliasPanel = new StackPanel
		{
			Margin = new Thickness(8.0)
		};
		foreach (SettingDefinition definition in settingsRegistry.GetDefinitions().Where(HasCompatibilityAlias).OrderBy((SettingDefinition item) => item.Key, StringComparer.OrdinalIgnoreCase))
		{
			aliasPanel.Children.Add(new TextBlock
			{
				Text = BuildCompatibilityLine(definition),
				Foreground = Brushes.LightBlue,
				FontSize = 12.0,
				FontFamily = new FontFamily("Consolas"),
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(0.0, 0.0, 0.0, 4.0)
			});
		}
		stackPanel.Children.Add(new Expander
		{
			Header = "Show legacy aliases",
			Foreground = Brushes.White,
			Background = new SolidColorBrush(Color.FromArgb(80, 36, 30, 44)),
			BorderBrush = new SolidColorBrush(Color.FromArgb(70, 138, 106, 176)),
			BorderThickness = new Thickness(1.0),
			Padding = new Thickness(8.0),
			IsExpanded = false,
			Content = aliasPanel
		});
		border.Child = stackPanel;
		SettingsPanel.Children.Add(border);
	}

	private static bool HasCompatibilityAlias(SettingDefinition definition)
	{
		return !string.IsNullOrWhiteSpace(definition.LegacyEnabledFlag) || !string.IsNullOrWhiteSpace(definition.LegacyDisabledFlag) || !string.IsNullOrWhiteSpace(definition.LegacyValueKey) || definition.RelatedLegacyKeys.Count > 0;
	}

	private static string BuildCompatibilityLine(SettingDefinition definition)
	{
		List<string> aliases = new List<string>();
		if (!string.IsNullOrWhiteSpace(definition.LegacyEnabledFlag))
		{
			aliases.Add("enabled=" + definition.LegacyEnabledFlag);
		}
		if (!string.IsNullOrWhiteSpace(definition.LegacyDisabledFlag))
		{
			aliases.Add("disabled=" + definition.LegacyDisabledFlag);
		}
		if (!string.IsNullOrWhiteSpace(definition.LegacyValueKey))
		{
			aliases.Add("value=" + definition.LegacyValueKey);
		}
		foreach (string relatedLegacyKey in definition.RelatedLegacyKeys)
		{
			aliases.Add("related=" + relatedLegacyKey);
		}
		return definition.Key + " -> " + string.Join(", ", aliases);
	}

	private FrameworkElement CreateRow(SettingDefinition definition)
	{
		Grid grid = new Grid
		{
			Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
		};
		grid.ColumnDefinitions.Add(new ColumnDefinition());
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		grid.ColumnDefinitions.Add(new ColumnDefinition
		{
			Width = GridLength.Auto
		});
		StackPanel stackPanel = new StackPanel();
		stackPanel.Children.Add(new TextBlock
		{
			Text = definition.Label,
			Foreground = new SolidColorBrush(Color.FromRgb(237, 237, 237)),
			FontSize = 16.0,
			FontFamily = new FontFamily("Segoe UI")
		});
		stackPanel.Children.Add(new TextBlock
		{
			Text = definition.Key,
			Foreground = Brushes.LightGray,
			FontSize = 13.0,
			FontFamily = new FontFamily("Consolas")
		});
		if (!string.IsNullOrWhiteSpace(definition.Description))
		{
			stackPanel.Children.Add(new TextBlock
			{
				Text = definition.Description,
				Foreground = Brushes.LightGray,
				FontSize = 12.0,
				TextWrapping = TextWrapping.Wrap,
				MaxWidth = 520.0
			});
		}
		if (!string.IsNullOrWhiteSpace(definition.ProgressionNote))
		{
			stackPanel.Children.Add(new TextBlock
			{
				Text = definition.ProgressionNote,
				Foreground = Brushes.Gold,
				FontSize = 12.0,
				TextWrapping = TextWrapping.Wrap,
				MaxWidth = 520.0
			});
		}
		grid.Children.Add(stackPanel);
		TextBlock textBlock = new TextBlock
		{
			Text = GetAnsweredStatusText(definition.Key, settingsRegistry.IsAnswered(definition.Key)),
			Foreground = GetAnsweredStatusBrush(definition.Key, settingsRegistry.IsAnswered(definition.Key)),
			FontSize = 14.0,
			VerticalAlignment = VerticalAlignment.Center,
			Margin = new Thickness(16.0, 0.0, 16.0, 0.0)
		};
		Grid.SetColumn(textBlock, 1);
		grid.Children.Add(textBlock);
		TextBlock textBlock2 = new TextBlock
		{
			Text = settingsRegistry.GetRelatedStateSummary(definition.Key),
			Foreground = Brushes.LightBlue,
			FontSize = 12.0,
			Margin = new Thickness(0.0, 4.0, 0.0, 0.0),
			TextWrapping = TextWrapping.Wrap,
			MaxWidth = 520.0
		};
		stackPanel.Children.Add(textBlock2);
		SettingEditorRow settingEditorRow = new SettingEditorRow
		{
			Definition = definition,
			AnsweredText = textBlock,
			RelatedStateText = textBlock2
		};
		if (definition.Kind == SettingKind.Toggle)
		{
			CheckBox checkBox = new CheckBox
			{
				IsChecked = settingsRegistry.IsEnabled(definition.Key),
				Foreground = Brushes.White,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(0.0, 0.0, 4.0, 0.0)
			};
			Grid.SetColumn(checkBox, 2);
			grid.Children.Add(checkBox);
			settingEditorRow = new SettingEditorRow
			{
				Definition = definition,
				ToggleBox = checkBox,
				AnsweredText = textBlock,
				RelatedStateText = textBlock2
			};
		}
		else
		{
			TextBox textBox = new TextBox
			{
				Text = settingsRegistry.GetRawValue(definition.Key) ?? "",
				Background = new SolidColorBrush(Color.FromRgb(28, 28, 34)),
				Foreground = Brushes.White,
				BorderBrush = new SolidColorBrush(Color.FromArgb(102, 138, 106, 176)),
				CaretBrush = Brushes.White,
				Width = 220.0,
				Padding = new Thickness(8.0, 4.0, 8.0, 4.0),
				FontSize = 13.0,
				FontFamily = new FontFamily("Segoe UI")
			};
			Grid.SetColumn(textBox, 2);
			grid.Children.Add(textBox);
			settingEditorRow = new SettingEditorRow
			{
				Definition = definition,
				ValueBox = textBox,
				AnsweredText = textBlock,
				RelatedStateText = textBlock2
			};
		}
		rows.Add(settingEditorRow);
		return grid;
	}

	private void Save_Click(object sender, RoutedEventArgs e)
	{
		CuckSettingState cuckState = settingsRegistry.GetCuckState();
		PetPlaySettingState petPlayState = settingsRegistry.GetPetPlayState();
		ChastitySettingState chastityState = settingsRegistry.GetChastityState();
		List<string> queuedAskKeys = new List<string>();
		List<SettingEditorRow> list = rows.Where(delegate(SettingEditorRow row)
		{
			return row.Definition.Kind != SettingKind.Toggle;
		}).ToList();
		bool flag = list.Any(delegate(SettingEditorRow row)
		{
			string text = settingsRegistry.GetRawValue(row.Definition.Key) ?? "";
			return !string.Equals(text, row.ValueBox.Text, StringComparison.Ordinal);
		});
		AnalSettingState analState = settingsRegistry.GetAnalState();
		LobSettingState lobState = settingsRegistry.GetLobState();
		CensorshipSettingState censorshipState = settingsRegistry.GetCensorshipState();
		BreathPlaySettingState breathPlayState = settingsRegistry.GetBreathPlayState();
		OutsideSessionSettingState outsideSessionState = settingsRegistry.GetOutsideSessionState();
		NoVideoSettingState noVideoState = settingsRegistry.GetNoVideoState();
		GaySettingState gayState = settingsRegistry.GetGayState();
		bool flag2 = cuckState.Enabled != (cuckEnabledBox.IsChecked == true)
			|| cuckState.FridayPassed != (cuckFridayPassedBox.IsChecked == true)
			|| cuckState.Stage.ToString(CultureInfo.InvariantCulture) != cuckStageBox.Text
			|| petPlayState.Enabled != (petPlayEnabledBox.IsChecked == true)
			|| petPlayState.AdvancedEnabled != (petPlayAdvancedBox.IsChecked == true)
			|| petPlayState.CollarEnabled != (petPlayCollarBox.IsChecked == true)
			|| petPlayState.TreatsEnabled != (petPlayTreatsBox.IsChecked == true)
			|| !string.Equals(petPlayState.Persona, petPlayPersonaBox.SelectedItem as string ?? "None", StringComparison.Ordinal)
			|| !string.Equals(petPlayState.PetName, petPlayPetNameBox.Text, StringComparison.Ordinal)
			|| !string.Equals(petPlayState.SubName, petPlaySubNameBox.Text, StringComparison.Ordinal)
			|| chastityState.Enabled != (chastityEnabledBox.IsChecked == true)
			|| chastityState.CageOwned != (chastityCageBox.IsChecked == true)
			|| chastityState.WearingCage != (chastityWearingBox.IsChecked == true)
			|| !string.Equals(chastityState.CageType, chastityCageTypeBox.Text, StringComparison.Ordinal)
			|| chastityState.VibratorOwned != (chastityVibratorBox.IsChecked == true)
			|| chastityState.LostKey != (chastityLostKeyBox.IsChecked == true)
			|| chastityState.ToldAboutNecklace != (chastityToldAboutNecklaceBox.IsChecked == true)
			|| chastityState.DurationDays.ToString(CultureInfo.InvariantCulture) != chastityDurationBox.Text
			|| !string.Equals(chastityState.StartDateText, chastityDateBox.Text, StringComparison.Ordinal)
			|| analState.Enabled != (analEnabledBox.IsChecked == true)
			|| analState.FirstSessionCompleted != (analFirstSessionBox.IsChecked == true)
			|| !string.Equals(analState.Experience, analExperienceBox.SelectedItem as string ?? "Unknown", StringComparison.Ordinal)
			|| !string.Equals(analState.Preference, analPreferenceBox.SelectedItem as string ?? "Unknown", StringComparison.Ordinal)
			|| analState.TrainingEnabled != (analTrainingBox.IsChecked == true)
			|| analState.TrainingDeclined != (analTrainingDeclinedBox.IsChecked == true)
			|| analState.WaterLubeEnabled != (analWaterLubeBox.IsChecked == true)
			|| analState.DildoEnabled != (analDildoBox.IsChecked == true)
			|| analState.PlugEnabled != (analPlugBox.IsChecked == true)
			|| analState.ProstateOrgasmEnabled != (analProstateOrgasmBox.IsChecked == true)
			|| lobState.Enabled != (lobEnabledBox.IsChecked == true)
			|| lobState.RuntimeEnabled != (lobRuntimeBox.IsChecked == true)
			|| lobState.EarlyHour.ToString(CultureInfo.InvariantCulture) != lobEarlyBox.Text
			|| lobState.LateHour.ToString(CultureInfo.InvariantCulture) != lobLateBox.Text
			|| censorshipState.Enabled != (censorshipEnabledBox.IsChecked == true)
			|| censorshipState.Intensity.ToString(CultureInfo.InvariantCulture) != censorshipIntensityBox.Text
			|| breathPlayState.Enabled != (breathPlayEnabledBox.IsChecked == true)
			|| breathPlayState.BreathTimeSeconds.ToString(CultureInfo.InvariantCulture) != breathPlayTimeBox.Text
			|| outsideSessionState.Enabled != (outsideSessionEnabledBox.IsChecked == true)
			|| outsideSessionState.NoPornRemaining.ToString(CultureInfo.InvariantCulture) != outsideSessionNoPornBox.Text
			|| outsideSessionState.ConstantCeiRemaining.ToString(CultureInfo.InvariantCulture) != outsideSessionConstantCeiBox.Text
			|| outsideSessionState.PlugHourRemaining.ToString(CultureInfo.InvariantCulture) != outsideSessionPlugHourBox.Text
			|| outsideSessionState.WatchPornRemaining.ToString(CultureInfo.InvariantCulture) != outsideSessionWatchPornBox.Text
			|| outsideSessionState.HypnoFilesRemaining.ToString(CultureInfo.InvariantCulture) != outsideSessionHypnoFilesBox.Text
			|| noVideoState.Enabled != (noVideoEnabledBox.IsChecked == true)
			|| noVideoState.DurationDays.ToString(CultureInfo.InvariantCulture) != noVideoValueBox.Text
			|| gayState.Enabled != (gayEnabledBox.IsChecked == true)
			|| gayState.HumiliationEnabled != (gayHumiliationBox.IsChecked == true);
		if ((flag || flag2) && MessageBox.Show("You are manually editing progression or identity values. These fields are normally changed by scripts during play. Saving can skip asks, unlock or disable content, and change future session behavior. Media tags and sources are not affected. Save anyway?", "OpenEdge", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
		{
			return;
		}
		if (!cuckState.Answered)
		{
			if (cuckEnabledBox.IsChecked == true)
			{
				QueueSettingAsk("cuck", queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk("cuck");
			}
		}
		else
		{
			settingsRegistry.SaveCuckState(new CuckSettingState
			{
				Enabled = cuckEnabledBox.IsChecked == true,
				Answered = true,
				FridayPassed = cuckFridayPassedBox.IsChecked == true,
				Stage = (int.TryParse(cuckStageBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int cuckStageValue) ? cuckStageValue : 0)
			});
		}
		if (!petPlayState.Answered)
		{
			if (petPlayEnabledBox.IsChecked == true)
			{
				QueueSettingAsk("petPlay", queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk("petPlay");
			}
			settingsRegistry.DequeueAsk("petPlayAdvanced");
			SaveHiddenToggleSetting("collar", petPlayCollarBox.IsChecked == true, queuedAskKeys);
			SaveHiddenToggleSetting("treats", petPlayTreatsBox.IsChecked == true, queuedAskKeys);
			SaveHiddenToggleSetting("pup", string.Equals(petPlayPersonaBox.SelectedItem as string ?? "None", "Pup", StringComparison.Ordinal), queuedAskKeys);
			SaveHiddenToggleSetting("cat", string.Equals(petPlayPersonaBox.SelectedItem as string ?? "None", "Cat", StringComparison.Ordinal), queuedAskKeys);
			SaveHiddenTextSetting("petName", petPlayPetNameBox.Text);
			SaveHiddenTextSetting("subName", petPlaySubNameBox.Text);
		}
		else
		{
			bool flag3 = !petPlayState.AdvancedAnswered && petPlayAdvancedBox.IsChecked == true;
			if (flag3)
			{
				QueueSettingAsk("petPlayAdvanced", queuedAskKeys);
			}
			else if (!petPlayState.AdvancedAnswered)
			{
				settingsRegistry.DequeueAsk("petPlayAdvanced");
			}
			settingsRegistry.SavePetPlayState(new PetPlaySettingState
			{
				Enabled = petPlayEnabledBox.IsChecked == true,
				Answered = true,
				AdvancedEnabled = (flag3 ? petPlayState.AdvancedEnabled : (petPlayAdvancedBox.IsChecked == true)),
				AdvancedAnswered = (flag3 ? petPlayState.AdvancedAnswered : (petPlayState.AdvancedAnswered || petPlayAdvancedBox.IsChecked == true)),
				CollarEnabled = petPlayCollarBox.IsChecked == true,
				TreatsEnabled = petPlayTreatsBox.IsChecked == true,
				Persona = petPlayPersonaBox.SelectedItem as string ?? "None",
				PetName = petPlayPetNameBox.Text,
				SubName = petPlaySubNameBox.Text
			});
		}
		settingsRegistry.SaveChastityState(new ChastitySettingState
		{
			Enabled = chastityEnabledBox.IsChecked == true,
			Answered = true,
			CageOwned = chastityCageBox.IsChecked == true,
			WearingCage = chastityWearingBox.IsChecked == true,
			CageType = chastityCageTypeBox.Text,
			VibratorOwned = chastityVibratorBox.IsChecked == true,
			LostKey = chastityLostKeyBox.IsChecked == true,
			ToldAboutNecklace = chastityToldAboutNecklaceBox.IsChecked == true,
			DurationDays = (int.TryParse(chastityDurationBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int chastityDurationValue) ? chastityDurationValue : 0),
			StartDateText = chastityDateBox.Text
		});
		if (!analState.Answered)
		{
			if (analEnabledBox.IsChecked == true)
			{
				QueueSettingAsk("anal", queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk("anal");
			}
			SaveHiddenToggleSetting("waterLube", analWaterLubeBox.IsChecked == true, queuedAskKeys);
			SaveHiddenToggleSetting("dildo", analDildoBox.IsChecked == true, queuedAskKeys);
			SaveHiddenToggleSetting("plug", analPlugBox.IsChecked == true, queuedAskKeys);
			SaveHiddenToggleSetting("prostateOrgasm", analProstateOrgasmBox.IsChecked == true, queuedAskKeys);
		}
		else
		{
			settingsRegistry.SaveAnalState(new AnalSettingState
			{
				Enabled = analEnabledBox.IsChecked == true,
				Answered = true,
				FirstSessionCompleted = analFirstSessionBox.IsChecked == true,
				Experience = analExperienceBox.SelectedItem as string ?? "Unknown",
				Preference = analPreferenceBox.SelectedItem as string ?? "Unknown",
				TrainingEnabled = analTrainingBox.IsChecked == true,
				TrainingDeclined = analTrainingDeclinedBox.IsChecked == true,
				WaterLubeEnabled = analWaterLubeBox.IsChecked == true,
				DildoEnabled = analDildoBox.IsChecked == true,
				PlugEnabled = analPlugBox.IsChecked == true,
				ProstateOrgasmEnabled = analProstateOrgasmBox.IsChecked == true
			});
		}
		if (!lobState.Answered)
		{
			if (lobEnabledBox.IsChecked == true)
			{
				QueueSettingAsk("LOB", queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk("LOB");
			}
			SaveHiddenNumericSetting("earlyLOB", lobEarlyBox.Text);
			SaveHiddenNumericSetting("lateLOB", lobLateBox.Text);
		}
		else
		{
			settingsRegistry.SaveLobState(new LobSettingState
			{
				Enabled = lobEnabledBox.IsChecked == true,
				Answered = true,
				RuntimeEnabled = lobRuntimeBox.IsChecked == true,
				EarlyHour = (int.TryParse(lobEarlyBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int earlyHour) ? earlyHour : 0),
				LateHour = (int.TryParse(lobLateBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int lateHour) ? lateHour : 0)
			});
		}
		if (!censorshipState.Answered)
		{
			if (censorshipEnabledBox.IsChecked == true)
			{
				QueueSettingAsk("censorship", queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk("censorship");
			}
			SaveHiddenNumericSetting("censorIncrease", censorshipIntensityBox.Text);
		}
		else
		{
			settingsRegistry.SaveCensorshipState(new CensorshipSettingState
			{
				Enabled = censorshipEnabledBox.IsChecked == true,
				Answered = true,
				Intensity = (int.TryParse(censorshipIntensityBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int censorValue) ? censorValue : 0)
			});
		}
		if (!breathPlayState.Answered)
		{
			if (breathPlayEnabledBox.IsChecked == true)
			{
				QueueSettingAsk("breathPlay", queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk("breathPlay");
			}
			SaveHiddenNumericSetting("breathTime", breathPlayTimeBox.Text);
		}
		else
		{
			settingsRegistry.SaveBreathPlayState(new BreathPlaySettingState
			{
				Enabled = breathPlayEnabledBox.IsChecked == true,
				Answered = true,
				BreathTimeSeconds = (int.TryParse(breathPlayTimeBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int breathValue) ? breathValue : 60)
			});
		}
		if (!outsideSessionState.Answered)
		{
			if (outsideSessionEnabledBox.IsChecked == true)
			{
				QueueSettingAsk("outsideSession", queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk("outsideSession");
			}
			settingsRegistry.SaveOutsideSessionDraftState(new OutsideSessionSettingState
			{
				NoPornRemaining = ParseEditorNumericValue(outsideSessionNoPornBox.Text),
				ConstantCeiRemaining = ParseEditorNumericValue(outsideSessionConstantCeiBox.Text),
				PlugHourRemaining = ParseEditorNumericValue(outsideSessionPlugHourBox.Text),
				WatchPornRemaining = ParseEditorNumericValue(outsideSessionWatchPornBox.Text),
				HypnoFilesRemaining = ParseEditorNumericValue(outsideSessionHypnoFilesBox.Text)
			});
		}
		else
		{
			settingsRegistry.SaveOutsideSessionState(new OutsideSessionSettingState
			{
				Enabled = outsideSessionEnabledBox.IsChecked == true,
				Answered = true,
				NoPornRemaining = (int.TryParse(outsideSessionNoPornBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int noPornValue) ? noPornValue : 0),
				ConstantCeiRemaining = (int.TryParse(outsideSessionConstantCeiBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int constantCeiValue) ? constantCeiValue : 0),
				PlugHourRemaining = (int.TryParse(outsideSessionPlugHourBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int plugHourValue) ? plugHourValue : 0),
				WatchPornRemaining = (int.TryParse(outsideSessionWatchPornBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int watchPornValue) ? watchPornValue : 0),
				HypnoFilesRemaining = (int.TryParse(outsideSessionHypnoFilesBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int hypnoFilesValue) ? hypnoFilesValue : 0)
			});
		}
		settingsRegistry.SaveNoVideoState(new NoVideoSettingState
		{
			Enabled = noVideoEnabledBox.IsChecked == true,
			Answered = true,
			DurationDays = (int.TryParse(noVideoValueBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int noVideoDurationValue) ? noVideoDurationValue : 0)
		});
		if (!gayState.Answered)
		{
			if (gayEnabledBox.IsChecked == true)
			{
				QueueSettingAsk("gay", queuedAskKeys);
			}
			else
			{
				settingsRegistry.DequeueAsk("gay");
			}
			SaveHiddenToggleSetting("gayHumiliation", gayHumiliationBox.IsChecked == true, queuedAskKeys);
		}
		else
		{
			settingsRegistry.SaveGayState(new GaySettingState
			{
				Enabled = gayEnabledBox.IsChecked == true,
				Answered = true,
				HumiliationEnabled = gayHumiliationBox.IsChecked == true,
				HumiliationAnswered = true
			});
		}
		foreach (SettingEditorRow row in rows)
		{
			if (row.Definition.Kind == SettingKind.Toggle)
			{
				bool flag4 = row.ToggleBox.IsChecked == true;
				if (!settingsRegistry.IsAnswered(row.Definition.Key) && settingsRegistry.SupportsQueuedAsk(row.Definition.Key))
				{
					if (flag4)
					{
						QueueSettingAsk(row.Definition.Key, queuedAskKeys);
					}
					else
					{
						settingsRegistry.DequeueAsk(row.Definition.Key);
					}
					continue;
				}
				if (!settingsRegistry.IsAnswered(row.Definition.Key) && flag4 == settingsRegistry.IsEnabled(row.Definition.Key))
				{
					continue;
				}
				settingsRegistry.SetEnabled(row.Definition.Key, flag4);
			}
			else if (row.Definition.Kind == SettingKind.Numeric)
			{
				if (int.TryParse(row.ValueBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
				{
					settingsRegistry.SetNumericValue(row.Definition.Key, result);
				}
			}
			else
			{
				if (string.Equals(row.Definition.Key, "guessName", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(row.ValueBox.Text))
				{
					settingsRegistry.ClearSetting(row.Definition.Key);
				}
				else
				{
					settingsRegistry.SetRawValue(row.Definition.Key, row.ValueBox.Text);
				}
			}
		}
		page1.SyncPronounSelectionFromSettings();
		page1.RefreshMenuButtons();
		LoadRows();
		string text2 = "Settings saved.";
		if (queuedAskKeys.Count > 0)
		{
			text2 = text2 + " Queued asks: " + string.Join(", ", queuedAskKeys) + ". They will run in a future session once their normal requirements are met.";
		}
		MessageBox.Show(text2, "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void Refresh_Click(object sender, RoutedEventArgs e)
	{
		LoadRows();
	}

	private void ClearSessionState_Click(object sender, RoutedEventArgs e)
	{
		if (MessageBox.Show("This clears interruption/recovery state from the previous session, such as leave-early and reconnect markers, and makes the app treat today as eligible for a new session. It does not change preferences, media tags, orgasm history, or normal progression. Continue?", "Clear session state", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
		{
			return;
		}
		int cleared = settingsRegistry.ResetSessionRecoveryState();
		page1.strokePage?.ClearSessionRecoveryRuntimeState();
		page1.RefreshMenuButtons();
		LoadRows();
		MessageBox.Show(cleared == 0 ? "No session recovery state was set." : "Session recovery state has been cleared.", "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void ResetProgression_Click(object sender, RoutedEventArgs e)
	{
		if (MessageBox.Show("This will clear all canonical settings, queued asks, and mirrored legacy progression values. Media tags and media sources are not affected. Continue?", "Reset settings progression", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
		{
			return;
		}
		settingsRegistry.ResetAllSettingsState();
		page1.SyncPronounSelectionFromSettings();
		page1.RefreshMenuButtons();
		LoadRows();
		MessageBox.Show("Settings progression has been reset.", "OpenEdge", MessageBoxButton.OK, MessageBoxImage.Information);
	}

	private void MediaSources_Click(object sender, RoutedEventArgs e)
	{
		page1.OpenMediaSources(NavigationService);
	}

	private void MigrationTools_Click(object sender, RoutedEventArgs e)
	{
		page1.OpenMigrationTools(NavigationService);
	}

	private void ConnectToys_Click(object sender, RoutedEventArgs e)
	{
		page1.OpenBluetooth(NavigationService);
	}

	private void Back_Click(object sender, RoutedEventArgs e)
	{
		NavigationService?.GoBack();
	}

	private void UpdateSummary()
	{
		IReadOnlyList<SettingDefinition> definitions = settingsRegistry.GetDefinitions();
		int num = definitions.Count(delegate(SettingDefinition definition)
		{
			return definition.Kind == SettingKind.Toggle && settingsRegistry.IsEnabled(definition.Key);
		});
		int num2 = definitions.Count(delegate(SettingDefinition definition)
		{
			return settingsRegistry.IsAnswered(definition.Key);
		});
		SummaryText.Text = "Settings are now backed by the canonical SettingsRegistry and mirrored through the compatibility layer. Enabled toggles: " + num + ". Answered settings: " + num2 + "/" + definitions.Count + ". Queued asks: " + settingsRegistry.GetQueuedAskKeys().Count + ".";
	}
}
