using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OpenEdge;

internal static class Program
{
	private static readonly List<string> Failures = new List<string>();

	private static int Main()
	{
		ResetHarnessRuntime();
		Run("toggle settings mirror canonical and legacy state", ToggleSettingsMirrorCanonicalAndLegacyState);
		Run("legacy flag writes promote canonical setting state", LegacyFlagWritesPromoteCanonicalSettingState);
		Run("legacy adapter promotes flags values and isolates temp flags", LegacyAdapterPromotesFlagsValuesAndIsolatesTempFlags);
		Run("all canonical aliases promote and delete", AllCanonicalAliasesPromoteAndDelete);
		Run("legacy flag deletes clear canonical setting mirrors", LegacyFlagDeletesClearCanonicalSettingMirrors);
		Run("script predicates read canonical settings", ScriptPredicatesReadCanonicalSettings);
		Run("structured script predicates share evaluator", StructuredScriptPredicatesShareEvaluator);
		Run("script setting command executor writes canonical state", ScriptSettingCommandExecutorWritesCanonicalState);
		Run("legacy state audit script runs", LegacyStateAuditScriptRuns);
		Run("queued asks dedupe and ignore answered asks", QueuedAsksDedupeAndIgnoreAnsweredAsks);
		Run("edge holds are default-off and queueable", EdgeHoldsAreDefaultOffAndQueueable);
		Run("numeric/text values mirror legacy value keys", NumericAndTextValuesMirrorLegacyValueKeys);
		Run("unknown legacy vars stay legacy-only", UnknownLegacyVarsStayLegacyOnly);
		Run("structured anal state mirrors related legacy keys", StructuredAnalStateMirrorsRelatedLegacyKeys);
		Run("structured substates mirror legacy keys", StructuredSubstatesMirrorLegacyKeys);
		Run("mod settings load as canonical simple settings", ModSettingsLoadAsCanonicalSimpleSettings);
		Run("enabled mods expose line roots and disabled mods do not", EnabledModsExposeLineRootsAndDisabledModsDoNot);
		Run("mod diagnostics report invalid JSON and duplicate keys", ModDiagnosticsReportInvalidJsonAndDuplicateKeys);
		Run("media tag compatibility imports and mirrors legacy tags", MediaTagCompatibilityImportsAndMirrorsLegacyTags);
		Run("media tag compatibility reports ambiguous legacy claims", MediaTagCompatibilityReportsAmbiguousLegacyClaims);
		Run("media identities preserve tags across rename", MediaIdentitiesPreserveTagsAcrossRename);
		Run("legacy tag import uses old root fingerprint", LegacyTagImportUsesOldRootFingerprint);
		Run("legacy tag import skips duplicate fingerprints", LegacyTagImportSkipsDuplicateFingerprints);
		Run("legacy tag import can skip existing tags", LegacyTagImportCanSkipExistingTags);
		Run("legacy tag import can overwrite existing tags", LegacyTagImportCanOverwriteExistingTags);
		Run("media identities preserve tags across source move", MediaIdentitiesPreserveTagsAcrossSourceMove);
		Run("duplicate fingerprints do not steal tags", DuplicateFingerprintsDoNotStealTags);
		Run("bulk move collision rename preserves identity", BulkMoveCollisionRenamePreservesIdentity);
		Run("external delete readd does not duplicate identity", ExternalDeleteReaddDoesNotDuplicateIdentity);
		Run("tag save tolerates externally deleted media", TagSaveToleratesExternallyDeletedMedia);
		Run("image safety allows valid jpeg marker layout", ImageSafetyAllowsValidJpegMarkerLayout);
		Run("image safety rejects truncated jpeg", ImageSafetyRejectsTruncatedJpeg);
		Run("reset progression preserves media tags and sources", ResetProgressionPreservesMediaTagsAndSources);
		Run("reset progression clears settings and queue", ResetProgressionClearsSettingsAndQueue);
		Run("line selection bag avoids repeats before exhaustion", LineSelectionBagAvoidsRepeatsBeforeExhaustion);
		Run("line selection bag avoids immediate reshuffle repeat", LineSelectionBagAvoidsImmediateReshuffleRepeat);
		Run("clear session state preserves progression", ClearSessionStatePreservesProgression);

		if (Failures.Count == 0)
		{
			Console.WriteLine("Settings harness passed.");
			return 0;
		}

		Console.Error.WriteLine("Settings harness failed:");
		foreach (string failure in Failures)
		{
			Console.Error.WriteLine("- " + failure);
		}
		return 1;
	}

	private static void Run(string name, Action test)
	{
		ResetHarnessRuntime();
		try
		{
			test();
			Console.WriteLine("PASS: " + name);
		}
		catch (Exception ex)
		{
			Failures.Add(name + ": " + ex.Message);
			Console.Error.WriteLine("FAIL: " + name + " - " + ex.Message);
		}
	}

	private static void ToggleSettingsMirrorCanonicalAndLegacyState()
	{
		SettingsRegistry registry = CreateRegistry();
		registry.SetEnabled("cuck", true);
		AssertTrue(registry.IsEnabled("cuck"), "cuck should be enabled");
		AssertTrue(registry.IsAnswered("cuck"), "cuck should be answered");
		AssertFileExists(RuntimePaths.Flag("cuck"), "legacy enabled flag should exist");
		AssertPersistentValue("settings.cuck.enabled", "True");
		AssertPersistentValue("settings.cuck.answered", "True");

		registry.SetEnabled("cuck", false);
		AssertFalse(registry.IsEnabled("cuck"), "cuck should be disabled");
		AssertFalse(File.Exists(RuntimePaths.Flag("cuck")), "legacy enabled flag should be removed");
		AssertFileExists(RuntimePaths.Flag("cuckNo"), "legacy disabled flag should exist");
	}

	private static void LegacyFlagWritesPromoteCanonicalSettingState()
	{
		SettingsRegistry registry = CreateRegistry();
		AssertTrue(registry.TryApplyLegacyFlagSet("gay"), "legacy gay flag should map");
		AssertTrue(registry.IsEnabled("gay"), "gay should be enabled from legacy flag");
		AssertTrue(registry.IsAnswered("gay"), "gay should be answered from legacy flag");
		AssertPersistentValue("settings.gay.enabled", "True");

		AssertTrue(registry.TryApplyLegacyFlagSet("gayNo"), "legacy gayNo flag should map");
		AssertFalse(registry.IsEnabled("gay"), "gay should be disabled from legacy no flag");
		AssertPersistentValue("settings.gay.enabled", "False");
	}

	private static void LegacyAdapterPromotesFlagsValuesAndIsolatesTempFlags()
	{
		CompatibilityStateService compatibility = CreateCompatibilityService();
		SettingsRegistry registry = new SettingsRegistry(compatibility);
		LegacyStateAdapter legacy = new LegacyStateAdapter(compatibility, registry);
		SessionFlagStore sessionFlags = new SessionFlagStore();

		legacy.SetFlag("cuck");
		AssertTrue(legacy.HasFlag("cuck"), "legacy adapter should write persistent flag");
		AssertTrue(registry.IsEnabled("cuck"), "legacy adapter should promote enabled flag");
		AssertTrue(registry.IsAnswered("cuck"), "legacy adapter should answer promoted flag");

		legacy.SetFlag("cuckNo");
		AssertFalse(registry.IsEnabled("cuck"), "legacy adapter should promote disabled flag");
		AssertTrue(legacy.HasAnsweredFlag("cuck"), "legacy adapter should see yes/no answered state");

		legacy.SetVar("sessionLengthMod", "6");
		AssertEqual(6, registry.GetNumericValue("sessionLengthMod"), "legacy adapter should promote numeric value var");
		AssertPersistentValue("settings.sessionLengthMod.value", "6");

		legacy.SetVar("unknownLegacyCounter", "9");
		AssertEqual("9", compatibility.GetPersistentValue("unknownLegacyCounter"), "unknown legacy value should pass through");
		AssertTrue(registry.GetDefinition("unknownLegacyCounter") == null, "unknown value should not become setting");

		sessionFlags.Set("adapterTemp");
		AssertTrue(sessionFlags.Exists("adapterTemp"), "session flag store should write temp flag");
		AssertFalse(legacy.HasFlag("adapterTemp"), "temp flags should not become persistent legacy flags");
		sessionFlags.Delete("adapterTemp");
		AssertFalse(sessionFlags.Exists("adapterTemp"), "session flag store should delete temp flag");
	}

	private static void AllCanonicalAliasesPromoteAndDelete()
	{
		CompatibilityStateService compatibility = CreateCompatibilityService();
		SettingsRegistry registry = new SettingsRegistry(compatibility);
		foreach (SettingDefinition definition in registry.GetDefinitions())
		{
			if (definition.Kind == SettingKind.Toggle && !string.IsNullOrWhiteSpace(definition.LegacyEnabledFlag))
			{
				AssertTrue(registry.TryApplyLegacyFlagSet(definition.LegacyEnabledFlag), "enabled alias should promote: " + definition.LegacyEnabledFlag);
				AssertTrue(registry.IsEnabled(definition.Key), "enabled alias should enable canonical key: " + definition.Key);
				AssertTrue(registry.IsAnswered(definition.Key), "enabled alias should answer canonical key: " + definition.Key);
				compatibility.DeletePersistentValue(definition.LegacyEnabledFlag);
				AssertTrue(registry.TryApplyLegacyFlagDelete(definition.LegacyEnabledFlag), "enabled alias delete should be handled: " + definition.LegacyEnabledFlag);
			}

			if (definition.Kind == SettingKind.Toggle && !string.IsNullOrWhiteSpace(definition.LegacyDisabledFlag))
			{
				AssertTrue(registry.TryApplyLegacyFlagSet(definition.LegacyDisabledFlag), "disabled alias should promote: " + definition.LegacyDisabledFlag);
				AssertFalse(registry.IsEnabled(definition.Key), "disabled alias should disable canonical key: " + definition.Key);
				AssertTrue(registry.IsAnswered(definition.Key), "disabled alias should answer canonical key: " + definition.Key);
			}
		}
	}

	private static void LegacyFlagDeletesClearCanonicalSettingMirrors()
	{
		CompatibilityStateService compatibility = CreateCompatibilityService();
		SettingsRegistry registry = new SettingsRegistry(compatibility);
		registry.SetEnabled("cuck", true);
		AssertTrue(registry.IsEnabled("cuck"), "cuck should start enabled");
		compatibility.DeletePersistentValue("cuck");
		AssertTrue(registry.TryApplyLegacyFlagDelete("cuck"), "legacy cuck delete should map");
		AssertFalse(registry.IsEnabled("cuck"), "cuck enabled mirror should clear after legacy delete");
		AssertFalse(registry.IsAnswered("cuck"), "cuck answered mirror should clear after legacy delete");
		AssertFalse(File.Exists(RuntimePaths.Flag("settings.cuck.enabled")), "canonical enabled mirror file should be deleted");
		AssertFalse(File.Exists(RuntimePaths.Flag("settings.cuck.answered")), "canonical answered mirror file should be deleted");

		registry.SetRawValue("domTitle", "Mistress");
		compatibility.DeletePersistentValue("domTitle");
		AssertTrue(registry.TryApplyLegacyFlagDelete("domTitle"), "legacy value delete should map");
		AssertTrue(string.IsNullOrEmpty(registry.GetRawValue("domTitle")), "canonical value mirror should clear after legacy value delete");
		AssertFalse(registry.IsAnswered("domTitle"), "canonical value answered mirror should clear after legacy value delete");
	}

	private static void ScriptPredicatesReadCanonicalSettings()
	{
		SettingsRegistry registry = CreateRegistry();
		registry.SetEnabled("cuck", true);
		registry.SetEnabled("gay", false);
		registry.SetNumericValue("sessionLengthMod", 3);

		AssertPredicate("ISSETTING:cuck", registry, true);
		AssertPredicate("ISNOSETTING:cuck", registry, false);
		AssertPredicate("ISSETTINGASKED:gay", registry, true);
		AssertPredicate("ISNOSETTINGASKED:edgeHold", registry, true);
		AssertPredicate("ISSETTINGDECLINED:gay", registry, true);
		AssertPredicate("ISNOSETTINGDECLINED:gay", registry, false);
		AssertPredicate("SETTINGVALUEATLEAST:sessionLengthMod,2", registry, true);
		AssertPredicate("SETTINGVALUEATLEAST:sessionLengthMod,4", registry, false);
	}

	private static void StructuredScriptPredicatesShareEvaluator()
	{
		SettingsRegistry registry = CreateRegistry();
		registry.SaveAnalState(new AnalSettingState { Enabled = true, Experience = "Beginner", Preference = "Like", TrainingEnabled = true });
		registry.SavePetPlayState(new PetPlaySettingState { Enabled = true, Answered = true, AdvancedEnabled = true, AdvancedAnswered = true });
		registry.SaveOutsideSessionState(new OutsideSessionSettingState { Enabled = true, Answered = true, NoPornRemaining = 2 });
		registry.SaveLobState(new LobSettingState { Enabled = true, Answered = true, RuntimeEnabled = true });
		registry.SetRawValue("domTitle", "Mistress");

		AssertStructuredPredicate("ANALSTAGE:Beginner", registry, true);
		AssertStructuredPredicate("ANALPREFERENCE:Dislike", registry, false);
		AssertStructuredPredicate("ISANALTRAINING", registry, true);
		AssertStructuredPredicate("ISANALTRAININGDECLINED", registry, false);
		AssertStructuredPredicate("ISPETPLAYADVANCED", registry, true);
		AssertStructuredPredicate("ISNOPETPLAYADVANCED", registry, false);
		AssertStructuredPredicate("ISOUTSIDERULEACTIVE:noPorn", registry, true);
		AssertStructuredPredicate("ISNOOUTSIDERULEACTIVE:noPorn", registry, false);
		AssertStructuredPredicate("ISLOBRUNTIMEON", registry, true);
		AssertStructuredPredicate("SETTINGTEXTSET:domTitle", registry, true);
		AssertStructuredPredicate("SETTINGTEXTEMPTY:domTitle", registry, false);
	}

	private static void ScriptSettingCommandExecutorWritesCanonicalState()
	{
		SettingsRegistry registry = CreateRegistry();
		ScriptSettingCommandExecutor executor = new ScriptSettingCommandExecutor(
			registry.SetEnabled,
			registry.SetNumericValue,
			registry.SetRawValue,
			delegate(string stage)
			{
				AnalSettingState state = registry.GetAnalState();
				state.Enabled = true;
				state.Answered = true;
				state.Experience = stage;
				registry.SaveAnalState(state);
			},
			delegate(string preference)
			{
				AnalSettingState state = registry.GetAnalState();
				state.Enabled = true;
				state.Answered = true;
				state.Preference = preference;
				registry.SaveAnalState(state);
			},
			delegate(string value)
			{
				AnalSettingState state = registry.GetAnalState();
				state.TrainingEnabled = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
				registry.SaveAnalState(state);
			},
			delegate(string persona)
			{
				PetPlaySettingState state = registry.GetPetPlayState();
				state.Enabled = true;
				state.Answered = true;
				state.Persona = persona;
				registry.SavePetPlayState(state);
			},
			delegate(string key, int value)
			{
				OutsideSessionSettingState state = registry.GetOutsideSessionState();
				if (key == "noPorn")
				{
					state.NoPornRemaining = value;
				}
				state.Enabled = true;
				state.Answered = true;
				registry.SaveOutsideSessionState(state);
			},
			delegate(int early, int late)
			{
				registry.SaveLobState(new LobSettingState { Enabled = true, Answered = true, RuntimeEnabled = true, EarlyHour = early, LateHour = late });
			},
			(string value) => value,
			delegate(int stage)
			{
				registry.SaveCuckState(new CuckSettingState { Enabled = true, Answered = true, Stage = stage });
			},
			delegate(bool passed)
			{
				CuckSettingState state = registry.GetCuckState();
				state.Enabled = true;
				state.Answered = true;
				state.FridayPassed = passed;
				registry.SaveCuckState(state);
			},
			delegate(string key, string value)
			{
				ChastitySettingState state = registry.GetChastityState();
				state.Enabled = true;
				state.Answered = true;
				if (key == "wearing")
				{
					state.WearingCage = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
				}
				registry.SaveChastityState(state);
			},
			delegate(int intensity)
			{
				registry.SaveCensorshipState(new CensorshipSettingState { Enabled = true, Answered = true, Intensity = intensity });
			},
			delegate(int seconds)
			{
				registry.SaveBreathPlayState(new BreathPlaySettingState { Enabled = true, Answered = true, BreathTimeSeconds = seconds });
			});

		AssertTrue(executor.TryExecute("SETSETTING:cuck,true"), "SETSETTING should be handled");
		AssertTrue(registry.IsEnabled("cuck"), "SETSETTING should enable canonical setting");
		AssertTrue(executor.TryExecute("SETSETTINGVALUE:sessionLengthMod,7"), "SETSETTINGVALUE should be handled");
		AssertEqual(7, registry.GetNumericValue("sessionLengthMod"), "SETSETTINGVALUE should set numeric value");
		AssertTrue(executor.TryExecute("SETANALSTAGE:Beginner"), "SETANALSTAGE should be handled");
		AssertEqual("Beginner", registry.GetAnalState().Experience, "SETANALSTAGE should set structured state");
		AssertTrue(executor.TryExecute("SETOUTSIDERULE:noPorn,3"), "SETOUTSIDERULE should be handled");
		AssertEqual(3, registry.GetOutsideSessionState().NoPornRemaining, "SETOUTSIDERULE should set outside rule");
		AssertTrue(executor.TryExecute("SETLOBWINDOW:8,23"), "SETLOBWINDOW should be handled");
		AssertEqual(8, registry.GetLobState().EarlyHour, "SETLOBWINDOW should set early hour");
		AssertTrue(executor.TryExecute("SETCUCKSTAGE:5"), "SETCUCKSTAGE should be handled");
		AssertEqual(5, registry.GetCuckState().Stage, "SETCUCKSTAGE should set cuck stage");
		AssertTrue(executor.TryExecute("SETCHASTITYSTATE:wearing,true"), "SETCHASTITYSTATE should be handled");
		AssertTrue(registry.GetChastityState().WearingCage, "SETCHASTITYSTATE should set chastity state");
		AssertTrue(executor.TryExecute("SETCENSORINTENSITY:4"), "SETCENSORINTENSITY should be handled");
		AssertEqual(4, registry.GetCensorshipState().Intensity, "SETCENSORINTENSITY should set intensity");
		AssertTrue(executor.TryExecute("SETBREATHTIME:31"), "SETBREATHTIME should be handled");
		AssertEqual(31, registry.GetBreathPlayState().BreathTimeSeconds, "SETBREATHTIME should set breath time");
		AssertFalse(executor.TryExecute("NOTSETTING:anything"), "unknown command should not be handled");
	}

	private static void LegacyStateAuditScriptRuns()
	{
		string repoRoot = FindRepoRoot();
		string script = Path.Combine(repoRoot, "docs", "recovery", "audit-legacy-state.ps1");
		AssertFileExists(script, "legacy audit script should exist");

		ProcessStartInfo start = new ProcessStartInfo
		{
			FileName = "powershell",
			Arguments = "-NoProfile -ExecutionPolicy Bypass -File \"" + script + "\"",
			WorkingDirectory = repoRoot,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false
		};
		using Process process = Process.Start(start) ?? throw new InvalidOperationException("failed to start audit script");
		string output = process.StandardOutput.ReadToEnd();
		string error = process.StandardError.ReadToEnd();
		process.WaitForExit(30000);
		AssertEqual(0, process.ExitCode, "audit script should exit successfully. stderr: " + error);
		AssertTrue(output.Contains("# Legacy State Audit"), "audit output should include report title");
		AssertTrue(output.Contains("## Summary"), "audit output should include summary");
	}

	private static void QueuedAsksDedupeAndIgnoreAnsweredAsks()
	{
		SettingsRegistry registry = CreateRegistry();
		registry.QueueAsk("anal");
		registry.QueueAsk("anal");
		registry.QueueAsk("notARealQueuedAsk");
		AssertSequence(registry.GetQueuedAskKeys(), new string[] { "anal" }, "queue should contain one supported ask");

		registry.SetEnabled("anal", true);
		registry.QueueAsk("anal");
		AssertSequence(registry.GetQueuedAskKeys(), new string[] { "anal" }, "answered ask should not duplicate");

		registry.DequeueAsk("anal");
		AssertEqual(0, registry.GetQueuedAskKeys().Count, "dequeue should clear ask");
	}

	private static void EdgeHoldsAreDefaultOffAndQueueable()
	{
		SettingsRegistry registry = CreateRegistry();
		AssertFalse(registry.IsEnabled("edgeHold"), "edge holds should default disabled");
		AssertFalse(registry.IsAnswered("edgeHold"), "edge holds should default unanswered");
		AssertTrue(registry.SupportsQueuedAsk("edgeHold"), "edge holds should support queued asks");
		registry.QueueAsk("edgeHold");
		AssertSequence(registry.GetQueuedAskKeys(), new string[] { "edgeHold" }, "edge hold ask should queue");
		registry.SetEnabled("edgeHold", true);
		AssertTrue(registry.IsEnabled("edgeHold"), "edge holds should enable after acceptance");
		AssertFileExists(RuntimePaths.Flag("edgeHold"), "edgeHold legacy flag should exist");
	}

	private static void NumericAndTextValuesMirrorLegacyValueKeys()
	{
		SettingsRegistry registry = CreateRegistry();
		registry.SetNumericValue("sessionLengthMod", 3);
		AssertEqual(3, registry.GetNumericValue("sessionLengthMod"), "numeric canonical read should work");
		AssertPersistentValue("sessionLengthMod", "3");
		AssertPersistentValue("settings.sessionLengthMod.value", "3");

		registry.SetRawValue("domTitle", "Mistress");
		AssertEqual("Mistress", registry.GetRawValue("domTitle"), "text canonical read should work");
		AssertPersistentValue("domTitle", "Mistress");
		AssertPersistentValue("settings.domTitle.value", "Mistress");
	}

	private static void UnknownLegacyVarsStayLegacyOnly()
	{
		CompatibilityStateService compatibility = CreateCompatibilityService();
		SettingsRegistry registry = new SettingsRegistry(compatibility);
		compatibility.SetPersistentValue("unknownLegacyCounter", "12");
		AssertEqual("12", compatibility.GetPersistentValue("unknownLegacyCounter"), "unknown legacy var should stay in compatibility state");
		AssertTrue(registry.GetDefinition("unknownLegacyCounter") == null, "unknown legacy var should not become a setting definition");
		AssertThrows(delegate { registry.GetNumericValue("unknownLegacyCounter"); }, "unknown legacy var should not read as canonical numeric setting");
	}

	private static void StructuredAnalStateMirrorsRelatedLegacyKeys()
	{
		SettingsRegistry registry = CreateRegistry();
		registry.SaveAnalState(new AnalSettingState
		{
			Enabled = true,
			FirstSessionCompleted = true,
			Experience = "Beginner",
			Preference = "Like",
			TrainingEnabled = true,
			TrainingDeclined = false,
			WaterLubeEnabled = true,
			DildoEnabled = true,
			PlugEnabled = false
		});

		AnalSettingState state = registry.GetAnalState();
		AssertTrue(state.Enabled, "anal should be enabled");
		AssertTrue(state.FirstSessionCompleted, "anal first session should be set");
		AssertEqual("Beginner", state.Experience, "anal experience should round-trip");
		AssertEqual("Like", state.Preference, "anal preference should round-trip");
		AssertTrue(state.TrainingEnabled, "anal training should be enabled");
		AssertTrue(state.WaterLubeEnabled, "water lube should be enabled");
		AssertTrue(state.DildoEnabled, "dildo should be enabled");
		AssertFalse(state.PlugEnabled, "plug should be disabled");
		AssertFileExists(RuntimePaths.Flag("anal"), "anal legacy flag should exist");
		AssertFileExists(RuntimePaths.Flag("analBeginner"), "anal beginner legacy flag should exist");
		AssertFileExists(RuntimePaths.Flag("analLike"), "anal like legacy flag should exist");
		AssertFileExists(RuntimePaths.Flag("analTraining"), "anal training legacy flag should exist");
	}

	private static void StructuredSubstatesMirrorLegacyKeys()
	{
		SettingsRegistry registry = CreateRegistry();

		registry.SavePetPlayState(new PetPlaySettingState
		{
			Enabled = true,
			Answered = true,
			AdvancedEnabled = true,
			AdvancedAnswered = true,
			CollarEnabled = true,
			TreatsEnabled = true,
			Persona = "Pup",
			PetName = "Buddy",
			SubName = "Good boy"
		});
		PetPlaySettingState pet = registry.GetPetPlayState();
		AssertTrue(pet.Enabled, "pet play should enable");
		AssertTrue(pet.AdvancedEnabled, "pet advanced should enable");
		AssertEqual("Pup", pet.Persona, "pet persona should round-trip");
		AssertFileExists(RuntimePaths.Flag("petPlay"), "pet play legacy flag should exist");
		AssertFileExists(RuntimePaths.Flag("petPlayAdvanced"), "pet advanced legacy flag should exist");
		AssertFileExists(RuntimePaths.Flag("pup"), "pup legacy flag should exist");
		AssertPersistentValue("petName", "Buddy");

		registry.SaveChastityState(new ChastitySettingState
		{
			Enabled = true,
			Answered = true,
			CageOwned = true,
			WearingCage = true,
			CageType = "metal",
			VibratorOwned = true,
			LostKey = true,
			ToldAboutNecklace = true,
			DurationDays = 5,
			StartDateText = "2026-06-02"
		});
		ChastitySettingState chastity = registry.GetChastityState();
		AssertTrue(chastity.WearingCage, "wearing chastity should round-trip");
		AssertEqual("metal", chastity.CageType, "cage type should round-trip");
		AssertEqual(5, chastity.DurationDays, "chastity duration should round-trip");
		AssertFileExists(RuntimePaths.Flag("chastity"), "chastity legacy flag should exist");
		AssertFileExists(RuntimePaths.Flag("cage"), "cage legacy flag should exist");
		AssertPersistentValue("chastityTime", "5");
		AssertPersistentValue("chastityDate", "2026-06-02");

		registry.SaveOutsideSessionState(new OutsideSessionSettingState
		{
			Enabled = true,
			Answered = true,
			NoPornRemaining = 2,
			ConstantCeiRemaining = 3,
			PlugHourRemaining = 4,
			WatchPornRemaining = 5,
			HypnoFilesRemaining = 6
		});
		OutsideSessionSettingState outside = registry.GetOutsideSessionState();
		AssertTrue(outside.HasActiveCommandments, "outside-session commandments should be active");
		AssertEqual(2, outside.NoPornRemaining, "outside no-porn should round-trip");
		AssertFileExists(RuntimePaths.Flag("outsideSession"), "outside session legacy flag should exist");
		AssertPersistentValue("noPorn", "2");
		AssertPersistentValue("hypnoFiles", "6");

		registry.SaveLobState(new LobSettingState { Enabled = true, Answered = true, RuntimeEnabled = true, EarlyHour = 7, LateHour = 22 });
		LobSettingState lob = registry.GetLobState();
		AssertTrue(lob.RuntimeEnabled, "LOB runtime should round-trip");
		AssertEqual(7, lob.EarlyHour, "early LOB hour should round-trip");
		AssertFileExists(RuntimePaths.Flag("LOB"), "LOB legacy flag should exist");
		AssertPersistentValue("earlyLOB", "7");
		AssertPersistentValue("lateLOB", "22");

		registry.SaveCuckState(new CuckSettingState { Enabled = true, Answered = true, Stage = 4, FridayPassed = true });
		AssertEqual(4, registry.GetCuckState().Stage, "cuck stage should round-trip");
		AssertPersistentValue("cuckNum", "4");
		AssertFileExists(RuntimePaths.Flag("fridayPassed"), "friday passed legacy flag should exist");

		registry.SaveCensorshipState(new CensorshipSettingState { Enabled = true, Answered = true, Intensity = 8 });
		AssertEqual(8, registry.GetCensorshipState().Intensity, "censor intensity should round-trip");
		AssertPersistentValue("censorIncrease", "8");

		registry.SaveBreathPlayState(new BreathPlaySettingState { Enabled = true, Answered = true, BreathTimeSeconds = 45 });
		AssertEqual(45, registry.GetBreathPlayState().BreathTimeSeconds, "breath time should round-trip");
		AssertPersistentValue("breathTime", "45");
	}

	private static void ModSettingsLoadAsCanonicalSimpleSettings()
	{
		string modRoot = Path.Combine(RuntimePaths.ModsDir, "example-mod");
		Directory.CreateDirectory(Path.Combine(modRoot, "settings"));
		File.WriteAllText(Path.Combine(modRoot, "mod.json"), "{\"id\":\"example-mod\",\"name\":\"Example Mod\",\"enabled\":true}");
		File.WriteAllText(Path.Combine(modRoot, "settings", "settings.json"), "{\"settings\":[{\"key\":\"latex\",\"label\":\"Latex\",\"kind\":\"Toggle\",\"group\":\"Clothing\",\"description\":\"Shiny mod content.\",\"warning\":\"Affects mod progression.\",\"legacyEnabledFlag\":\"latex\",\"legacyDisabledFlag\":\"latexNo\",\"queueableAsk\":true,\"mediaDiscoveryTags\":[\"Latex\"],\"mediaDiscoveryMinimum\":3}]}");

		SettingsRegistry registry = new SettingsRegistry(CreateCompatibilityService(), ModService.GetEnabledSettingDefinitions());
		SettingDefinition latexDefinition = registry.GetDefinitions().FirstOrDefault((SettingDefinition definition) => string.Equals(definition.Key, "latex", StringComparison.OrdinalIgnoreCase));
		AssertTrue(latexDefinition != null, "mod setting should be registered");
		AssertSequence(latexDefinition.MediaDiscoveryTags, new string[] { "Latex" }, "mod setting should retain media discovery tags");
		AssertEqual(3, latexDefinition.MediaDiscoveryMinimum, "mod setting should retain media discovery minimum");
		AssertEqual("Clothing", latexDefinition.Group, "mod setting should retain group metadata");
		AssertEqual("Shiny mod content.", latexDefinition.Description, "mod setting should retain description metadata");
		AssertEqual("Affects mod progression.", latexDefinition.ProgressionNote, "mod setting should retain warning metadata");
		AssertTrue(registry.SupportsQueuedAsk("latex"), "queueable mod setting should support asks");
		registry.QueueAsk("latex");
		AssertSequence(registry.GetQueuedAskKeys(), new string[] { "latex" }, "mod ask should queue");
		registry.SetEnabled("latex", true);
		AssertTrue(registry.IsEnabled("latex"), "mod setting should enable");
		AssertFileExists(RuntimePaths.Flag("latex"), "mod legacy enabled flag should exist");
	}

	private static void EnabledModsExposeLineRootsAndDisabledModsDoNot()
	{
		string enabledRoot = CreateLineRootMod("enabled-lines", true);
		string disabledRoot = CreateLineRootMod("disabled-lines", false);

		IReadOnlyList<string> roots = ModService.GetEnabledLineRoots();
		AssertTrue(roots.Any((string root) => string.Equals(root, Path.Combine(enabledRoot, "lines"), StringComparison.OrdinalIgnoreCase)), "enabled mod line root should be exposed");
		AssertFalse(roots.Any((string root) => string.Equals(root, Path.Combine(disabledRoot, "lines"), StringComparison.OrdinalIgnoreCase)), "disabled mod line root should not be exposed");
	}

	private static void ModDiagnosticsReportInvalidJsonAndDuplicateKeys()
	{
		string badRoot = Path.Combine(RuntimePaths.ModsDir, "bad-json");
		Directory.CreateDirectory(badRoot);
		File.WriteAllText(Path.Combine(badRoot, "mod.json"), "{ invalid json");

		string duplicateOne = CreateModWithSetting("duplicate-one", true, "latex");
		string duplicateTwo = CreateModWithSetting("duplicate-two", true, "latex");

		IReadOnlyList<ModSummary> summaries = ModService.GetModSummaries();
		ModSummary badSummary = summaries.FirstOrDefault((ModSummary summary) => string.Equals(summary.Id, "bad-json", StringComparison.OrdinalIgnoreCase));
		AssertTrue(badSummary != null, "invalid JSON mod should produce a summary");
		AssertTrue((badSummary.Error ?? "").Contains("invalid JSON", StringComparison.OrdinalIgnoreCase), "invalid JSON summary should include parse details");
		AssertTrue((badSummary.Error ?? "").Contains("mod.json", StringComparison.OrdinalIgnoreCase), "invalid JSON summary should include file path");

		ModSummary first = summaries.FirstOrDefault((ModSummary summary) => string.Equals(summary.RootPath, duplicateOne, StringComparison.OrdinalIgnoreCase));
		ModSummary second = summaries.FirstOrDefault((ModSummary summary) => string.Equals(summary.RootPath, duplicateTwo, StringComparison.OrdinalIgnoreCase));
		AssertTrue((first?.Error ?? "").Contains("duplicate enabled setting key: latex", StringComparison.OrdinalIgnoreCase), "first duplicate mod should report duplicate setting key");
		AssertTrue((second?.Error ?? "").Contains("duplicate enabled setting key: latex", StringComparison.OrdinalIgnoreCase), "second duplicate mod should report duplicate setting key");
	}

	private static void MediaTagCompatibilityImportsAndMirrorsLegacyTags()
	{
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		string exact = Path.Combine(RuntimePaths.ImagesDir, "exact.jpg");
		string fallback = Path.Combine(RuntimePaths.ImagesDir, "fallback.jpg");
		File.WriteAllBytes(exact, new byte[] { 1, 2, 3, 4 });
		File.WriteAllBytes(fallback, new byte[] { 5, 6, 7, 8 });
		File.WriteAllText(RuntimePaths.TagsFile, "\\images\\exact.jpgkljnrbkrbasxalkxmbtGirl" + Environment.NewLine + "old/fallback.jpgkljnrbkrbasxalkxmbtFeet");

		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();

		AssertEqual("Girl", catalog.GetTags("\\images\\exact.jpg"), "exact legacy path should import tags");
		AssertEqual("Feet", catalog.GetTags("\\images\\fallback.jpg"), "unique filename resolver should import tags");
		MediaTagCompatibilityDiagnostics diagnostics = catalog.GetTagCompatibilityDiagnostics();
		AssertEqual(2, diagnostics.LegacyLinesRead, "diagnostics should count legacy lines");
		AssertEqual(1, diagnostics.ExactPathMatches, "diagnostics should count exact matches");
		AssertEqual(1, diagnostics.ResolverMatches, "diagnostics should count resolver matches");

		catalog.SetTags("\\images\\exact.jpg", "GirlSolo");
		string mirror = File.ReadAllText(RuntimePaths.TagsFile);
		AssertTrue(mirror.Contains("\\images\\exact.jpgkljnrbkrbasxalkxmbtGirlSolo"), "legacy mirror should write current identity tags");
	}

	private static void MediaTagCompatibilityReportsAmbiguousLegacyClaims()
	{
		Directory.CreateDirectory(Path.Combine(RuntimePaths.ImagesDir, "a"));
		Directory.CreateDirectory(Path.Combine(RuntimePaths.ImagesDir, "b"));
		File.WriteAllBytes(Path.Combine(RuntimePaths.ImagesDir, "a", "dup.jpg"), new byte[] { 1, 1, 1 });
		File.WriteAllBytes(Path.Combine(RuntimePaths.ImagesDir, "b", "dup.jpg"), new byte[] { 2, 2, 2 });
		File.WriteAllText(RuntimePaths.TagsFile, "old/dup.jpgkljnrbkrbasxalkxmbtGirl");

		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();

		MediaTagCompatibilityDiagnostics diagnostics = catalog.GetTagCompatibilityDiagnostics();
		AssertEqual(1, diagnostics.LegacyLinesRead, "ambiguous diagnostics should count legacy line");
		AssertEqual(1, diagnostics.UnmatchedClaims, "ambiguous filename claim should be reported as unmatched");
		AssertEqual("", catalog.GetTags("\\images\\a\\dup.jpg"), "ambiguous claim should not tag first duplicate");
		AssertEqual("", catalog.GetTags("\\images\\b\\dup.jpg"), "ambiguous claim should not tag second duplicate");
	}

	private static void MediaIdentitiesPreserveTagsAcrossRename()
	{
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		string original = Path.Combine(RuntimePaths.ImagesDir, "rename-original.jpg");
		string renamed = Path.Combine(RuntimePaths.ImagesDir, "rename-new.jpg");
		File.WriteAllBytes(original, CreateBytes(70_000, 11));
		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		catalog.SetTags("\\images\\rename-original.jpg", "Girl");

		File.Move(original, renamed);
		catalog.Reload();

		AssertEqual("Girl", catalog.GetTags("\\images\\rename-new.jpg"), "identity tags should survive rename in same source");
	}

	private static void LegacyTagImportUsesOldRootFingerprint()
	{
		string oldRoot = Path.Combine(RuntimePaths.RuntimeRoot, "old-import-root");
		Directory.CreateDirectory(oldRoot);
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		byte[] bytes = CreateBytes(70_000, 66);
		File.WriteAllBytes(Path.Combine(oldRoot, "legacy-name.jpg"), bytes);
		File.WriteAllBytes(Path.Combine(RuntimePaths.ImagesDir, "current-name.jpg"), bytes);
		string externalTags = Path.Combine(RuntimePaths.RuntimeRoot, "external-tags.txt");
		File.WriteAllText(externalTags, "\\images\\legacy-name.jpgkljnrbkrbasxalkxmbtImportedTag");

		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		LegacyTagImportResult result = catalog.ImportLegacyTags(externalTags, new string[] { oldRoot });

		AssertEqual(1, result.ImportedCount, "legacy tag import should import one entry");
		AssertEqual(1, result.FingerprintMatches, "legacy tag import should use old-root fingerprint match");
		AssertEqual("ImportedTag", catalog.GetTags("\\images\\current-name.jpg"), "legacy tag import should apply tags to current media identity");
		AssertFileExists(result.ReportPath, "legacy tag import should write report");
	}

	private static void LegacyTagImportSkipsDuplicateFingerprints()
	{
		string oldRoot = Path.Combine(RuntimePaths.RuntimeRoot, "old-duplicate-import-root");
		Directory.CreateDirectory(oldRoot);
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		byte[] bytes = CreateBytes(70_000, 88);
		File.WriteAllBytes(Path.Combine(oldRoot, "legacy-name.jpg"), bytes);
		File.WriteAllBytes(Path.Combine(RuntimePaths.ImagesDir, "duplicate-a.jpg"), bytes);
		File.WriteAllBytes(Path.Combine(RuntimePaths.ImagesDir, "duplicate-b.jpg"), bytes);
		string externalTags = Path.Combine(RuntimePaths.RuntimeRoot, "external-duplicate-tags.txt");
		File.WriteAllText(externalTags, "\\images\\legacy-name.jpgkljnrbkrbasxalkxmbtImportedTag");

		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		LegacyTagImportResult result = catalog.ImportLegacyTags(externalTags, new string[] { oldRoot });

		AssertEqual(0, result.ImportedCount, "legacy tag import should not apply one old fingerprint to duplicate current files");
		AssertEqual("", catalog.GetTags("\\images\\duplicate-a.jpg"), "first duplicate should remain untagged");
		AssertEqual("", catalog.GetTags("\\images\\duplicate-b.jpg"), "second duplicate should remain untagged");
	}

	private static void LegacyTagImportCanSkipExistingTags()
	{
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		string path = Path.Combine(RuntimePaths.ImagesDir, "existing-skip.jpg");
		File.WriteAllBytes(path, CreateBytes(70_000, 89));
		string externalTags = Path.Combine(RuntimePaths.RuntimeRoot, "external-skip-existing-tags.txt");
		File.WriteAllText(externalTags, "\\images\\existing-skip.jpgkljnrbkrbasxalkxmbtImportedTag");

		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		catalog.SetTags("\\images\\existing-skip.jpg", "CurrentTag");
		LegacyTagImportResult result = catalog.ImportLegacyTags(externalTags, Array.Empty<string>(), overwriteExistingTags: false);

		AssertEqual(0, result.ImportedCount, "non-overwrite import should skip existing tags");
		AssertEqual(1, result.SkippedExistingCount, "non-overwrite import should report skipped existing tags");
		AssertEqual("CurrentTag", catalog.GetTags("\\images\\existing-skip.jpg"), "current tags should be preserved when overwrite is false");
	}

	private static void LegacyTagImportCanOverwriteExistingTags()
	{
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		string path = Path.Combine(RuntimePaths.ImagesDir, "existing-overwrite.jpg");
		File.WriteAllBytes(path, CreateBytes(70_000, 90));
		string externalTags = Path.Combine(RuntimePaths.RuntimeRoot, "external-overwrite-existing-tags.txt");
		File.WriteAllText(externalTags, "\\images\\existing-overwrite.jpgkljnrbkrbasxalkxmbtImportedTag");

		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		catalog.SetTags("\\images\\existing-overwrite.jpg", "CurrentTag");
		LegacyTagImportResult result = catalog.ImportLegacyTags(externalTags, Array.Empty<string>(), overwriteExistingTags: true);

		AssertEqual(1, result.ImportedCount, "overwrite import should import over existing tags");
		AssertEqual("ImportedTag", catalog.GetTags("\\images\\existing-overwrite.jpg"), "current tags should be replaced when overwrite is true");
	}

	private static void MediaIdentitiesPreserveTagsAcrossSourceMove()
	{
		string sourceA = Path.Combine(RuntimePaths.RuntimeRoot, "source-a");
		string sourceB = Path.Combine(RuntimePaths.RuntimeRoot, "source-b");
		Directory.CreateDirectory(sourceA);
		Directory.CreateDirectory(sourceB);
		string original = Path.Combine(sourceA, "move-source.jpg");
		string moved = Path.Combine(sourceB, "move-source.jpg");
		File.WriteAllBytes(original, CreateBytes(70_000, 22));
		MediaCatalogService catalog = new MediaCatalogService();
		catalog.SaveSources(new MediaSourceDefinition[]
		{
			new MediaSourceDefinition { Id = "source-a", Name = "A", RootPath = sourceA, IsEnabled = true, ImagesEnabled = true, VideosEnabled = false },
			new MediaSourceDefinition { Id = "source-b", Name = "B", RootPath = sourceB, IsEnabled = true, ImagesEnabled = true, VideosEnabled = false }
		});
		catalog.Reload();
		catalog.SetTags("\\source-a\\move-source.jpg", "Feet");

		File.Move(original, moved);
		catalog.Reload();

		AssertEqual("Feet", catalog.GetTags("\\source-b\\move-source.jpg"), "identity tags should survive unique fingerprint move to another source");
	}

	private static void DuplicateFingerprintsDoNotStealTags()
	{
		Directory.CreateDirectory(Path.Combine(RuntimePaths.ImagesDir, "tagged-a"));
		Directory.CreateDirectory(Path.Combine(RuntimePaths.ImagesDir, "tagged-b"));
		Directory.CreateDirectory(Path.Combine(RuntimePaths.ImagesDir, "new"));
		byte[] bytes = CreateBytes(70_000, 33);
		string taggedA = Path.Combine(RuntimePaths.ImagesDir, "tagged-a", "dup-a.jpg");
		string taggedB = Path.Combine(RuntimePaths.ImagesDir, "tagged-b", "dup-b.jpg");
		string replacement = Path.Combine(RuntimePaths.ImagesDir, "new", "dup-new.jpg");
		File.WriteAllBytes(taggedA, bytes);
		File.WriteAllBytes(taggedB, bytes);
		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		catalog.SetTags("\\images\\tagged-a\\dup-a.jpg", "Girl");
		catalog.SetTags("\\images\\tagged-b\\dup-b.jpg", "Feet");

		File.Delete(taggedA);
		File.Delete(taggedB);
		File.WriteAllBytes(replacement, bytes);
		catalog.Reload();

		AssertEqual("", catalog.GetTags("\\images\\new\\dup-new.jpg"), "ambiguous duplicate fingerprints should not silently steal tags");
	}

	private static void BulkMoveCollisionRenamePreservesIdentity()
	{
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		string destination = Path.Combine(RuntimePaths.RuntimeRoot, "bulk-destination");
		Directory.CreateDirectory(destination);
		File.WriteAllBytes(Path.Combine(RuntimePaths.ImagesDir, "collision.jpg"), CreateBytes(70_000, 44));
		File.WriteAllBytes(Path.Combine(destination, "collision.jpg"), CreateBytes(70_000, 45));
		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		catalog.SetTags("\\images\\collision.jpg", "Solo");

		BulkMoveResult result = catalog.MoveMedia(new string[] { "\\images\\collision.jpg" }, destination);

		AssertEqual(1, result.MovedCount, "bulk move should move one file");
		AssertEqual(1, result.RenamedCount, "bulk move should rename on collision");
		string movedPath = catalog.Snapshot.IdentitiesById.Values.First((MediaIdentityRecord identity) => string.Equals(identity.Tags, "Solo", StringComparison.Ordinal)).CurrentRelativePath;
		AssertTrue(movedPath.StartsWith("\\bulk-destination\\collision", StringComparison.OrdinalIgnoreCase), "moved identity should point at collision-renamed destination");
		AssertEqual("Solo", catalog.GetTags(movedPath), "bulk move collision rename should preserve identity tags");
	}

	private static void ExternalDeleteReaddDoesNotDuplicateIdentity()
	{
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		string path = Path.Combine(RuntimePaths.ImagesDir, "readd.jpg");
		File.WriteAllBytes(path, CreateBytes(70_000, 55));
		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		catalog.SetTags("\\images\\readd.jpg", "Girl");
		File.Delete(path);
		catalog.Reload();
		File.WriteAllBytes(path, CreateBytes(70_000, 55));
		catalog.Reload();

		int matchingIdentities = catalog.Snapshot.IdentitiesById.Values.Count((MediaIdentityRecord identity) => identity.KnownRelativePaths.Contains("\\images\\readd.jpg", StringComparer.OrdinalIgnoreCase));
		AssertEqual(1, matchingIdentities, "delete/re-add of same file should not duplicate identity records for the path");
		AssertEqual("Girl", catalog.GetTags("\\images\\readd.jpg"), "delete/re-add of same file should preserve identity tags");
	}

	private static void TagSaveToleratesExternallyDeletedMedia()
	{
		Directory.CreateDirectory(RuntimePaths.ImagesDir);
		string path = Path.Combine(RuntimePaths.ImagesDir, "deleted-before-save.jpg");
		File.WriteAllBytes(path, CreateBytes(70_000, 77));
		MediaCatalogService catalog = new MediaCatalogService();
		catalog.Reload();
		File.Delete(path);

		catalog.SetTags("\\images\\deleted-before-save.jpg", "Girl");

		AssertEqual("Girl", catalog.GetTags("\\images\\deleted-before-save.jpg"), "tags should save even when media was externally deleted before fingerprinting");
	}

	private static void ImageSafetyAllowsValidJpegMarkerLayout()
	{
		string path = Path.Combine(RuntimePaths.RuntimeRoot, "valid-marker-layout.jpg");
		File.WriteAllBytes(path, CreateMinimalJpegBytes(includeEndMarker: true));

		AssertTrue(ImageFileSafety.IsSafeForWpfDecode(path, out string reason), "valid jpeg marker layout should be allowed: " + reason);
	}

	private static void ImageSafetyRejectsTruncatedJpeg()
	{
		string path = Path.Combine(RuntimePaths.RuntimeRoot, "truncated.jpg");
		File.WriteAllBytes(path, CreateMinimalJpegBytes(includeEndMarker: false));

		AssertFalse(ImageFileSafety.IsSafeForWpfDecode(path, out _), "truncated jpeg should be skipped before WPF decode");
	}

	private static void ResetProgressionPreservesMediaTagsAndSources()
	{
		SettingsRegistry registry = CreateRegistry();
		string mediaSourcesFile = Path.Combine(RuntimePaths.RuntimeRoot, "media-sources.json");
		string mediaTagIndexFile = Path.Combine(RuntimePaths.RuntimeRoot, "media-tag-index.json");
		File.WriteAllText(mediaSourcesFile, "{\"sources\":[]}");
		File.WriteAllText(RuntimePaths.TagsFile, "images/example.jpgkljnrbkrbasxalkxmbtGirl(s)Solo");
		File.WriteAllText(mediaTagIndexFile, "{\"identities\":[]}");

		registry.SetEnabled("cuck", true);
		registry.QueueAsk("anal");
		registry.ResetAllSettingsState();

		AssertFileExists(mediaSourcesFile, "media sources should survive reset progression");
		AssertFileExists(RuntimePaths.TagsFile, "legacy media tags should survive reset progression");
		AssertFileExists(mediaTagIndexFile, "media tag identity index should survive reset progression");
		AssertFalse(registry.IsEnabled("cuck"), "progression setting should still reset");
		AssertEqual(0, registry.GetQueuedAskKeys().Count, "queued asks should still reset");
	}

	private static void ResetProgressionClearsSettingsAndQueue()
	{
		SettingsRegistry registry = CreateRegistry();
		registry.SetEnabled("cuck", true);
		registry.SetNumericValue("sessionLengthMod", 4);
		registry.QueueAsk("anal");
		registry.SaveAnalState(new AnalSettingState
		{
			Enabled = true,
			Experience = "Experienced",
			Preference = "Dislike",
			TrainingDeclined = true
		});

		registry.ResetAllSettingsState();
		AssertFalse(registry.IsEnabled("cuck"), "cuck should reset disabled");
		AssertFalse(registry.IsAnswered("cuck"), "cuck should reset unanswered");
		AssertEqual(0, registry.GetQueuedAskKeys().Count, "queue should reset empty");
		AssertEqual(0, registry.GetNumericValue("sessionLengthMod"), "numeric setting should reset");
		AssertFalse(File.Exists(RuntimePaths.Flag("analDislike")), "related anal legacy flag should be removed");
		AssertFalse(File.Exists(RuntimePaths.Flag("analTrainingNo")), "anal training decline flag should be removed");
	}

	private static void LineSelectionBagAvoidsRepeatsBeforeExhaustion()
	{
		LineSelectionBag bag = new LineSelectionBag(new Random(123));
		string[] options = new string[] { "a", "b", "c", "d" };
		HashSet<int> drawn = new HashSet<int>();
		for (int i = 0; i < options.Length; i++)
		{
			drawn.Add(bag.NextIndex("test", options));
		}
		AssertEqual(options.Length, drawn.Count, "selection bag should draw every option once before repeating");
	}

	private static void LineSelectionBagAvoidsImmediateReshuffleRepeat()
	{
		LineSelectionBag bag = new LineSelectionBag(new Random(456));
		string[] options = new string[] { "a", "b", "c" };
		int last = -1;
		for (int cycle = 0; cycle < 20; cycle++)
		{
			for (int i = 0; i < options.Length; i++)
			{
				int next = bag.NextIndex("test", options);
				if (last >= 0)
				{
					AssertFalse(next == last && i == 0, "first draw after reshuffle should not repeat previous draw when alternatives exist");
				}
				last = next;
			}
		}
	}

	private static void ClearSessionStatePreservesProgression()
	{
		CompatibilityStateService compatibility = CreateCompatibilityService();
		SettingsRegistry registry = new SettingsRegistry(compatibility);
		registry.SetEnabled("cuck", true);
		registry.SaveAnalState(new AnalSettingState { Enabled = true, Experience = "Beginner", Preference = "Like" });
		compatibility.SetPersistentValue("failedSessionEnd", DateTime.Now.AddDays(-2).ToString());
		compatibility.SetPersistentValue("leftEarly", "True");
		Directory.CreateDirectory(Path.GetDirectoryName(RuntimePaths.TempFlag("leftEarly")) ?? RuntimePaths.FlagsDir);
		File.WriteAllText(RuntimePaths.TempFlag("leftEarly"), "True");

		int cleared = registry.ResetSessionRecoveryState();

		AssertTrue(cleared >= 3, "session recovery reset should clear recovery markers and reset daily timing");
		AssertTrue(compatibility.PersistentEntryExists("failedSessionEnd"), "daily session timing marker should be reset, not deleted");
		AssertTrue(new LegacyStateAdapter(compatibility, registry).GetFlagAgeSeconds("failedSessionEnd") >= 86400, "daily session timing should be old enough to allow a new session");
		AssertFalse(compatibility.PersistentEntryExists("leftEarly"), "left early marker should clear");
		AssertFalse(File.Exists(RuntimePaths.TempFlag("leftEarly")), "temp left early marker should clear");
		AssertTrue(registry.IsEnabled("cuck"), "clear session state should preserve progression setting");
		AssertEqual("Beginner", registry.GetAnalState().Experience, "clear session state should preserve structured progression");
	}

	private static byte[] CreateBytes(int length, byte seed)
	{
		byte[] bytes = new byte[length];
		for (int i = 0; i < bytes.Length; i++)
		{
			bytes[i] = (byte)(seed + i % 251);
		}
		return bytes;
	}

	private static byte[] CreateMinimalJpegBytes(bool includeEndMarker)
	{
		List<byte> bytes = new List<byte>
		{
			0xFF, 0xD8,
			0xFF, 0xC0,
			0x00, 0x11,
			0x08,
			0x00, 0x01,
			0x00, 0x01,
			0x03,
			0x01, 0x11, 0x00,
			0x02, 0x11, 0x00,
			0x03, 0x11, 0x00
		};
		if (includeEndMarker)
		{
			bytes.Add(0xFF);
			bytes.Add(0xD9);
		}
		return bytes.ToArray();
	}

	private static string CreateLineRootMod(string id, bool enabled)
	{
		string modRoot = Path.Combine(RuntimePaths.ModsDir, id);
		Directory.CreateDirectory(Path.Combine(modRoot, "lines", "Scripts"));
		Directory.CreateDirectory(Path.Combine(modRoot, "lines", "Vocab"));
		File.WriteAllText(Path.Combine(modRoot, "mod.json"), "{\"id\":\"" + id + "\",\"name\":\"" + id + "\",\"enabled\":" + (enabled ? "true" : "false") + "}");
		return modRoot;
	}

	private static string CreateModWithSetting(string id, bool enabled, string settingKey)
	{
		string modRoot = Path.Combine(RuntimePaths.ModsDir, id);
		Directory.CreateDirectory(Path.Combine(modRoot, "settings"));
		File.WriteAllText(Path.Combine(modRoot, "mod.json"), "{\"id\":\"" + id + "\",\"name\":\"" + id + "\",\"enabled\":" + (enabled ? "true" : "false") + "}");
		File.WriteAllText(Path.Combine(modRoot, "settings", "settings.json"), "{\"settings\":[{\"key\":\"" + settingKey + "\",\"label\":\"" + settingKey + "\",\"kind\":\"Toggle\"}]}");
		return modRoot;
	}

	private static SettingsRegistry CreateRegistry()
	{
		return new SettingsRegistry(CreateCompatibilityService());
	}

	private static CompatibilityStateService CreateCompatibilityService()
	{
		CompatibilityStateService compatibility = new CompatibilityStateService();
		compatibility.EnsureInitialized();
		return compatibility;
	}

	private static void ResetHarnessRuntime()
	{
		DeleteDirectoryIfExists(RuntimePaths.FlagsDir);
		DeleteDirectoryIfExists(RuntimePaths.ModsDir);
		DeleteDirectoryIfExists(RuntimePaths.ImagesDir);
		DeleteDirectoryIfExists(RuntimePaths.VideosDir);
		DeleteDirectoryIfExists(RuntimePaths.CompatibilityBackupsDir);
		DeleteDirectoryIfExists(RuntimePaths.CompatibilityTransfersDir);
		DeleteFileIfExists(RuntimePaths.CompatibilityStateFile);
		DeleteFileIfExists(RuntimePaths.OptionsFile);
		DeleteFileIfExists(RuntimePaths.TasksFile);
		DeleteFileIfExists(RuntimePaths.TagsFile);
		DeleteFileIfExists(Path.Combine(RuntimePaths.RuntimeRoot, "media-sources.json"));
		DeleteFileIfExists(Path.Combine(RuntimePaths.RuntimeRoot, "media-sources.json.backup"));
		DeleteFileIfExists(Path.Combine(RuntimePaths.RuntimeRoot, "media-tag-index.json"));
		DeleteDirectoryIfExists(Path.Combine(RuntimePaths.RuntimeRoot, "source-a"));
		DeleteDirectoryIfExists(Path.Combine(RuntimePaths.RuntimeRoot, "source-b"));
		DeleteDirectoryIfExists(Path.Combine(RuntimePaths.RuntimeRoot, "old-import-root"));
		DeleteFileIfExists(Path.Combine(RuntimePaths.RuntimeRoot, "external-tags.txt"));
		DeleteDirectoryIfExists(Path.Combine(RuntimePaths.RuntimeRoot, "bulk-destination"));
	}

	private static string FindRepoRoot()
	{
		DirectoryInfo current = new DirectoryInfo(Directory.GetCurrentDirectory());
		while (current != null)
		{
			if (File.Exists(Path.Combine(current.FullName, "OpenEdge.sln")) && Directory.Exists(Path.Combine(current.FullName, "docs", "recovery")))
			{
				return current.FullName;
			}
			current = current.Parent;
		}
		throw new InvalidOperationException("could not locate repository root from " + Directory.GetCurrentDirectory());
	}

	private static void AssertPersistentValue(string key, string expected)
	{
		string path = RuntimePaths.Flag(key);
		AssertFileExists(path, "persistent value missing for " + key);
		AssertEqual(expected, File.ReadAllText(path), "persistent value mismatch for " + key);
	}

	private static void AssertStructuredPredicate(string token, SettingsRegistry registry, bool expected)
	{
		bool handled = ScriptStructuredPredicateEvaluator.TryEvaluate(token, registry.IsAnalStage, registry.IsAnalPreference, registry.HasCompletedFirstAnalSession, registry.HasDeclinedAnalTraining, registry.HasActiveAnalTraining, registry.IsPetPlayAdvancedDeclined, registry.IsPetPlayAdvancedEnabled, registry.IsOutsideSessionRuleActive, registry.IsLobRuntimeEnabled, registry.HasTextValue, out bool actual);
		AssertTrue(handled, "structured predicate should be handled: " + token);
		AssertEqual(expected, actual, "structured predicate result mismatch for " + token);
	}

	private static void AssertPredicate(string token, SettingsRegistry registry, bool expected)
	{
		bool handled = ScriptSettingPredicateEvaluator.TryEvaluate(token, registry.IsEnabled, registry.IsAnswered, (string key) => registry.IsAnswered(key) && !registry.IsEnabled(key), registry.GetNumericValue, int.Parse, out bool actual);
		AssertTrue(handled, "predicate should be handled: " + token);
		AssertEqual(expected, actual, "predicate result mismatch for " + token);
	}

	private static void AssertThrows(Action action, string message)
	{
		try
		{
			action();
		}
		catch
		{
			return;
		}
		throw new InvalidOperationException(message);
	}

	private static void AssertFileExists(string path, string message)
	{
		if (!File.Exists(path))
		{
			throw new InvalidOperationException(message + " (" + path + ")");
		}
	}

	private static void AssertTrue(bool condition, string message)
	{
		if (!condition)
		{
			throw new InvalidOperationException(message);
		}
	}

	private static void AssertFalse(bool condition, string message)
	{
		if (condition)
		{
			throw new InvalidOperationException(message);
		}
	}

	private static void AssertEqual<T>(T expected, T actual, string message)
	{
		if (!EqualityComparer<T>.Default.Equals(expected, actual))
		{
			throw new InvalidOperationException(message + " Expected [" + expected + "], got [" + actual + "].");
		}
	}

	private static void AssertSequence(IReadOnlyList<string> actual, IReadOnlyList<string> expected, string message)
	{
		if (!actual.SequenceEqual(expected, StringComparer.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException(message + " Expected [" + string.Join(",", expected) + "], got [" + string.Join(",", actual) + "].");
		}
	}

	private static void DeleteDirectoryIfExists(string path)
	{
		if (Directory.Exists(path))
		{
			Directory.Delete(path, recursive: true);
		}
	}

	private static void DeleteFileIfExists(string path)
	{
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}
}
