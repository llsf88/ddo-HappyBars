using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using VoK.Sdk.Ddo;
using VoK.Sdk.Ddo.Enums;

namespace UiRuler
{
    public partial class IngameUI : Form
    {
        private sealed class RulerTarget
        {
            public string Label { get; init; }

            public UIElementID? ElementId { get; init; }

            public bool UseClientArea { get; init; }

            public bool UseScreenArea { get; init; }

            public bool IsHotbar { get; init; }

            public bool IsVerticalHotbar { get; init; }

            public override string ToString() => Label;
        }

        private sealed class TargetOffset
        {
            public int X { get; set; }

            public int Y { get; set; }
        }

        private sealed class HotbarSnapshot
        {
            public int Hotbar { get; set; }

            public string Orientation { get; set; }

            public string ElementId { get; set; }

            public TargetOffset Offset { get; set; }

            public Rectangle? RawRect { get; set; }

            public Rectangle? AdjustedRect { get; set; }
        }

        private sealed class CharacterFileParts
        {
            public string Name { get; init; }

            public string Id { get; init; }
        }

        private sealed class HotbarLayoutEntry
        {
            public int Hotbar { get; init; }

            public int Row { get; init; }

            public int Column { get; init; }

            public bool Vertical { get; init; }

            public int Left { get; set; }

            public int Top { get; set; }

            public int Width => Vertical ? VerticalHotbarWidth : HorizontalHotbarWidth;

            public int Height => Vertical ? VerticalHotbarHeight : HorizontalHotbarHeight;
        }

        private readonly IDdoGameDataProvider _provider;
        private readonly string _folder;
        private readonly System.Threading.Timer _timer;
        private readonly RulerTarget[] _availableTargets;
        private readonly string _offsetSettingsPath;
        private readonly Dictionary<string, TargetOffset> _targetOffsets = new(StringComparer.Ordinal);
        private PopupRulerOverlayForm _overlay;
        private int _busy;
        private bool _enabled;
        private bool _captureNextLeftClick;
        private bool _wasLeftMouseDown;
        private bool _loadingTargetOffset;
        private bool _snapHotbarsEnabled;
        private bool _suppressHotbarSnap;
        private bool _snapPreviewActive;
        private string _lastDetailsText = string.Empty;
        private Dictionary<int, Rectangle> _snapDragStartRects = new();
        private const int HotbarCount = 20;
        private const int HorizontalHotbarWidth = 402;
        private const int HorizontalHotbarHeight = 44;
        private const int VerticalHotbarWidth = 42;
        private const int VerticalHotbarHeight = 398;
        private const int PositionTolerancePixels = 3;
        private const int SnapDistancePixels = 40;
        private const int SnapActivationPixels = 40;
        private const int HotbarLayoutClientBottomMargin = 30;
        private const int HotbarLayoutHorizontalGap = 5;
        public IngameUI(IDdoGameDataProvider provider, string folder)
        {
            InitializeComponent();
            txtHowToUse.Text = GetHowToUseText();
            tabTools.TabPages.Remove(tabUiRuler);
            tabTools.SelectedTab = tabHappyBars;
            Text = "HappyBars";

            _provider = provider;
            _folder = folder;
            _offsetSettingsPath = Path.Combine(_folder, "uiruler-target-offsets.json");
            LoadTargetOffsets();

            var targets = new List<RulerTarget>
            {
                new RulerTarget
                {
                    Label = "Whole Screen",
                    UseScreenArea = true
                },
                new RulerTarget
                {
                    Label = "Whole Client Area",
                    UseClientArea = true
                },
                new RulerTarget
                {
                    Label = "PopupMenuField (Dialogs)",
                    ElementId = UIElementID.PopupMenuField
                },
                new RulerTarget
                {
                    Label = "PortalActivate_Field (Portal)",
                    ElementId = UIElementID.PortalActivate_Field
                },
                new RulerTarget
                {
                    Label = "Quest_Field (Quest Journal)",
                    ElementId = UIElementID.Quest_Field
                },
                new RulerTarget
                {
                    Label = "CharacterSheetField",
                    ElementId = UIElementID.CharacterSheetField
                },
                new RulerTarget
                {
                    Label = "Inventory_Field",
                    ElementId = UIElementID.Inventory_Field
                },
                new RulerTarget
                {
                    Label = "Advancement_Field",
                    ElementId = UIElementID.Advancement_Field
                },
                new RulerTarget
                {
                    Label = "BaseOptionsField",
                    ElementId = UIElementID.BaseOptionsField
                },
                new RulerTarget
                {
                    Label = "Enchancement_Menu",
                    ElementId = UIElementID.Enchancement_Menu
                },
                new RulerTarget
                {
                    Label = "NewEpicDestiny_Menu",
                    ElementId = UIElementID.NewEpicDestiny_Menu
                },
                new RulerTarget
                {
                    Label = "DialogConfirmationFieldLarge (Confirmation)",
                    ElementId = UIElementID.DialogConfirmationFieldLarge
                }
            };

            AddUndockedHotbarTargets(targets);
            _availableTargets = targets.ToArray();

            cboTargetElement.Items.AddRange(_availableTargets);
            cboTargetElement.DropDownWidth = 640;
            cboTargetElement.SelectedIndexChanged += cboTargetElement_SelectedIndexChanged;
            cboTargetElement.SelectedIndex = 0;
            ApplyOffsetForSelectedTarget();

            _overlay = CreateOverlay();
            chkEnabled.Checked = false;
            _timer = new System.Threading.Timer(_ => BackgroundTick(), null, 0, 50);
        }

        private static string GetHowToUseText()
        {
            return string.Join(Environment.NewLine, new[]
            {
                "HappyBars - How to use",
                "",
                "Set Hotbars",
                "Use the text box in the HappyBars tab to describe the layout you want.",
                "Each line becomes a row. Values separated by commas become columns.",
                "",
                "Example:",
                "1,5",
                "2,6",
                "3,7",
                "",
                "Empty spaces",
                "Use x or 0 when you want to leave an empty slot in the layout.",
                "",
                "Example:",
                "x,3",
                "4,6",
                "9,5",
                "",
                "Vertical hotbars",
                "Add V after a hotbar number to place that hotbar vertically.",
                "",
                "Example:",
                "1,5v",
                "2,6v",
                "",
                "Anchor and screen limits",
                "The first numbered hotbar in the first column is used as the anchor.",
                "If the full layout would go past the game client border, HappyBars moves the anchor first, then places the other hotbars around it.",
                "The bottom of the client is treated as 30 pixels shorter to avoid the XP bar.",
                "",
                "Save Hotbars",
                "Saves the current hotbar positions, orientation, and current character.",
                "The save file uses the character name and character ID.",
                "",
                "Load Hotbars",
                "Loads the saved layout for the current character.",
                "HappyBars looks for the character ID first, then falls back to the character name.",
                "If a hotbar needs to change orientation, it rotates first and moves afterward.",
                "",
                "Snap Hotbars",
                "When enabled, manually dragging a hotbar shows a visual snap preview.",
                "HappyBars chooses the neighboring hotbar closest to your mouse, then previews the final position.",
                "It can snap above, below, left, or right, while avoiding overlaps with other hotbars.",
                "",
                "ESC",
                "Press ESC during Set Hotbars or Load Hotbars to stop moving hotbars."
            });
        }

        private static void AddUndockedHotbarTargets(List<RulerTarget> targets)
        {
            for (var i = 0; i < HotbarCount; i++)
            {
                var horizontalName = $"UndockedShortcut{i}_Horizontal_MarkerList";
                if (Enum.TryParse(horizontalName, out UIElementID horizontalElementId))
                {
                    targets.Add(new RulerTarget
                    {
                        Label = horizontalName,
                        ElementId = horizontalElementId,
                        IsHotbar = true
                    });
                }

                var verticalName = $"UndockedShortcut{i}_Vertical_MarkerList";
                if (Enum.TryParse(verticalName, out UIElementID verticalElementId))
                {
                    targets.Add(new RulerTarget
                    {
                        Label = verticalName,
                        ElementId = verticalElementId,
                        IsHotbar = true,
                        IsVerticalHotbar = true
                    });
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                _timer?.Dispose();
            }
            catch
            {
            }

            try
            {
                if (_overlay != null && !_overlay.IsDisposed)
                    _overlay.Close();
            }
            catch
            {
            }

            base.OnFormClosed(e);
        }

        private PopupRulerOverlayForm CreateOverlay()
        {
            var overlay = new PopupRulerOverlayForm
            {
                GridSpacing = (int)nudGridSpacing.Value,
                ShowGuideLabels = chkShowLabels.Checked,
                ShowGrid = chkShowGrid.Checked,
                ShowCrosshair = chkShowCrosshair.Checked
            };

            return overlay;
        }

        private void BackgroundTick()
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (Interlocked.Exchange(ref _busy, 1) == 1)
                return;

            try
            {
                BeginInvoke(new Action(UpdateOverlay));
            }
            catch
            {
            }
            finally
            {
                Interlocked.Exchange(ref _busy, 0);
            }
        }

        private void UpdateOverlay()
        {
            if (IsDisposed)
                return;

            HandlePendingClickCapture();

            if (_snapPreviewActive)
                return;

            if (_overlay == null || _overlay.IsDisposed)
                _overlay = CreateOverlay();

            _overlay.GridSpacing = (int)nudGridSpacing.Value;
            _overlay.ShowGuideLabels = chkShowLabels.Checked;
            _overlay.ShowGrid = chkShowGrid.Checked;
            _overlay.ShowCrosshair = chkShowCrosshair.Checked;

            if (!_enabled)
            {
                _overlay.HideOverlay();
                lblStatus.Text = "Overlay disabled.";
                UpdateDetailsTextIfChanged("Overlay disabled.");
                return;
            }

            var target = GetSelectedTarget();
            var rawRect = TryGetTargetRectangle(target);
            if (!rawRect.HasValue)
            {
                _overlay.HideOverlay();
                lblStatus.Text = $"{target.Label} not found.";
                UpdateDetailsTextIfChanged(BuildTargetDetailsText(target, null, null, null, null));
                return;
            }

            if (rawRect.Value.Width <= 0 || rawRect.Value.Height <= 0)
            {
                _overlay.HideOverlay();
                lblStatus.Text = $"{target.Label} has invalid size.";
                UpdateDetailsTextIfChanged(BuildTargetDetailsText(target, rawRect, null, null, null));
                return;
            }

            var adjustedRect = ApplyOffsets(rawRect.Value);
            _overlay.UpdatePopup(adjustedRect, Cursor.Position);

            var relX = Cursor.Position.X - adjustedRect.Left;
            var relY = Cursor.Position.Y - adjustedRect.Top;
            var captureSuffix = _captureNextLeftClick ? "  Capture armed" : string.Empty;
            lblStatus.Text = $"{GetTargetLogName(target)} {adjustedRect.Width}x{adjustedRect.Height}  Mouse {relX},{relY}  Offset {(int)nudOffsetX.Value},{(int)nudOffsetY.Value}{captureSuffix}";
            UpdateDetailsTextIfChanged(BuildTargetDetailsText(target, rawRect, adjustedRect, relX, relY));
        }

        private void chkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _enabled = chkEnabled.Checked;
            if (!_enabled && _overlay != null && !_overlay.IsDisposed)
                _overlay.HideOverlay();
        }

        private void chkSnapHotbars_CheckedChanged(object sender, EventArgs e)
        {
            _snapHotbarsEnabled = chkSnapHotbars.Checked;
            _snapDragStartRects.Clear();
            lblStatus.Text = _snapHotbarsEnabled ? "Hotbar snap enabled." : "Hotbar snap disabled.";
        }

        private void nudGridSpacing_ValueChanged(object sender, EventArgs e)
        {
            if (_overlay != null && !_overlay.IsDisposed)
                _overlay.GridSpacing = (int)nudGridSpacing.Value;
        }

        private void nudOffset_ValueChanged(object sender, EventArgs e)
        {
            if (_loadingTargetOffset)
                return;

            var target = GetSelectedTarget();
            _targetOffsets[GetTargetSettingsKey(target)] = new TargetOffset
            {
                X = (int)nudOffsetX.Value,
                Y = (int)nudOffsetY.Value
            };
            SaveTargetOffsets();
        }

        private void cboTargetElement_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyOffsetForSelectedTarget();
        }

        private void ApplyOffsetForSelectedTarget()
        {
            var target = GetSelectedTarget();
            var offset = GetTargetOffset(target);

            _loadingTargetOffset = true;
            try
            {
                nudOffsetX.Value = offset.X;
                nudOffsetY.Value = offset.Y;
            }
            finally
            {
                _loadingTargetOffset = false;
            }
        }

        private TargetOffset GetTargetOffset(RulerTarget target)
        {
            if (_targetOffsets.TryGetValue(GetTargetSettingsKey(target), out var savedOffset))
                return savedOffset;

            if (target.UseScreenArea || target.UseClientArea)
                return new TargetOffset { X = 0, Y = 0 };

            if (target.IsHotbar)
                return target.IsVerticalHotbar
                    ? new TargetOffset { X = 10, Y = 32 }
                    : new TargetOffset { X = 9, Y = 32 };

            return new TargetOffset { X = 0, Y = 0 };
        }

        private string GetTargetSettingsKey(RulerTarget target)
        {
            if (target.UseScreenArea)
                return "WholeScreen";

            if (target.UseClientArea)
                return "WholeClientArea";

            return target.ElementId?.ToString() ?? "WholeClientArea";
        }

        private void LoadTargetOffsets()
        {
            try
            {
                if (!File.Exists(_offsetSettingsPath))
                    return;

                var offsets = JsonSerializer.Deserialize<Dictionary<string, TargetOffset>>(File.ReadAllText(_offsetSettingsPath));
                if (offsets == null)
                    return;

                foreach (var offset in offsets)
                    _targetOffsets[offset.Key] = offset.Value;
            }
            catch
            {
            }
        }

        private void SaveTargetOffsets()
        {
            try
            {
                Directory.CreateDirectory(_folder);
                var json = JsonSerializer.Serialize(_targetOffsets, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_offsetSettingsPath, json);
            }
            catch
            {
            }
        }

        private void chkShowLabels_CheckedChanged(object sender, EventArgs e)
        {
            if (_overlay != null && !_overlay.IsDisposed)
            {
                _overlay.ShowGuideLabels = chkShowLabels.Checked;
                _overlay.Invalidate();
            }
        }

        private void chkShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            if (_overlay != null && !_overlay.IsDisposed)
            {
                _overlay.ShowGrid = chkShowGrid.Checked;
                _overlay.Invalidate();
            }
        }

        private void chkShowCrosshair_CheckedChanged(object sender, EventArgs e)
        {
            if (_overlay != null && !_overlay.IsDisposed)
            {
                _overlay.ShowCrosshair = chkShowCrosshair.Checked;
                _overlay.Invalidate();
            }
        }

        private void btnLogMeasurement_Click(object sender, EventArgs e)
        {
            _captureNextLeftClick = true;
            lblStatus.Text = "Capture armed. Click in DDO to log the next left click.";
        }

        private void btnCaptureSnapshot_Click(object sender, EventArgs e)
        {
            try
            {
                CaptureSnapshot();
            }
            catch (Exception ex)
            {
                txtLog.AppendText($"Snapshot failed: {ex.Message}{Environment.NewLine}");
                lblStatus.Text = $"Snapshot failed: {ex.Message}";
            }
        }

        private void btnCopyDetails_Click(object sender, EventArgs e)
        {
            CopyTextToClipboard(txtConversation.Text, "Details copied.");
        }

        private void btnCopyLog_Click(object sender, EventArgs e)
        {
            CopyTextToClipboard(txtLog.Text, "Log copied.");
        }

        private void btnSelectDetails_Click(object sender, EventArgs e)
        {
            txtConversation.Focus();
            txtConversation.SelectAll();
        }

        private void btnMoveMouseToTargetOrigin_Click(object sender, EventArgs e)
        {
            var target = GetSelectedTarget();
            var rawRect = TryGetTargetRectangle(target);
            if (!rawRect.HasValue)
            {
                lblStatus.Text = $"{target.Label} not found.";
                return;
            }

            var adjustedRect = ApplyOffsets(rawRect.Value);
            SetCursorPos(adjustedRect.Left, adjustedRect.Top);
            lblStatus.Text = $"Mouse moved to {GetTargetLogName(target)} rel 0,0.";
        }

        private void btnDragTargetToRawPosition_Click(object sender, EventArgs e)
        {
        }

        private void btnSetHotbars_Click(object sender, EventArgs e)
        {
            if (!TryParseHotbarLayout(txtHotbarLayout.Text, out var entries, out var error))
            {
                lblStatus.Text = error;
                return;
            }

            var anchor = entries
                .Where(e => e.Column == 0)
                .OrderBy(e => e.Row)
                .FirstOrDefault()
                ?? entries.OrderBy(e => e.Row).ThenBy(e => e.Column).First();

            var anchorTarget = ChooseCurrentHotbarTarget(anchor.Hotbar - 1);
            var anchorRect = anchorTarget == null ? null : TryGetTargetRectangle(anchorTarget);
            if (!anchorRect.HasValue)
            {
                lblStatus.Text = $"Hotbar {anchor.Hotbar} not found.";
                return;
            }

            PositionLayout(entries, anchor, anchorRect.Value.Location);
            if (!ConstrainLayoutToGameClient(entries, out var layoutWasShifted))
            {
                lblStatus.Text = "Layout is too large for the game client area.";
                return;
            }

            var moved = 0;
            var moveOrder = GetHotbarLayoutMoveOrder(entries, anchor, layoutWasShifted);
            foreach (var entry in moveOrder)
            {
                if (IsEscapePressed())
                {
                    lblStatus.Text = $"Set hotbars stopped by ESC after {moved}/{entries.Count}.";
                    return;
                }

                if (ApplyHotbarLayoutEntry(entry))
                {
                    moved++;
                    Thread.Sleep(200);
                }
            }

            lblStatus.Text = $"Set hotbars applied {moved}/{entries.Count}.";
        }

        private void btnSaveHotbars_Click(object sender, EventArgs e)
        {
            var snapshots = new List<HotbarSnapshot>();

            for (var i = 0; i < HotbarCount; i++)
            {
                snapshots.Add(BuildHotbarSnapshot(i));
            }

            var path = GetCharacterHotbarsFilePath();
            var json = JsonSerializer.Serialize(snapshots, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            txtLog.AppendText($"Hotbar positions saved: {path}{Environment.NewLine}");
            lblStatus.Text = $"Saved {snapshots.Count} hotbars.";
        }

        private void btnLoadHotbars_Click(object sender, EventArgs e)
        {
            var path = FindCharacterHotbarsFilePath();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                lblStatus.Text = "No hotbar save found for this character.";
                return;
            }

            List<HotbarSnapshot> snapshots;
            try
            {
                snapshots = JsonSerializer.Deserialize<List<HotbarSnapshot>>(File.ReadAllText(path)) ?? [];
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Load failed: {ex.Message}";
                return;
            }

            var moved = 0;
            foreach (var snapshot in snapshots)
            {
                if (IsEscapePressed())
                {
                    lblStatus.Text = $"Load hotbars stopped by ESC after {moved}/{snapshots.Count}.";
                    return;
                }

                if (!snapshot.RawRect.HasValue || snapshot.Hotbar <= 0)
                    continue;

                if (LoadHotbarSnapshot(snapshot))
                {
                    moved++;
                    Thread.Sleep(200);
                }
            }

            txtLog.AppendText($"Hotbar positions loaded: {path}{Environment.NewLine}");
            lblStatus.Text = $"Loaded {moved}/{snapshots.Count} hotbars.";
        }

        private string GetCharacterHotbarsFilePath()
        {
            var parts = GetCharacterFileParts();
            var fileName = $"{parts.Name}_{parts.Id}_uiruler-hotbars.json";
            return Path.Combine(_folder, fileName);
        }

        private string FindCharacterHotbarsFilePath()
        {
            var exactPath = GetCharacterHotbarsFilePath();
            if (File.Exists(exactPath))
                return exactPath;

            var parts = GetCharacterFileParts();
            var idMatch = Directory
                .EnumerateFiles(_folder, $"*_{parts.Id}_uiruler-hotbars.json")
                .OrderByDescending(File.GetLastWriteTime)
                .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(idMatch))
                return idMatch;

            return Directory
                .EnumerateFiles(_folder, $"{parts.Name}_*_uiruler-hotbars.json")
                .OrderByDescending(File.GetLastWriteTime)
                .FirstOrDefault();
        }

        private CharacterFileParts GetCharacterFileParts()
        {
            var characterName = "Unknown";
            var characterId = "UnknownId";

            try
            {
                var character = _provider.GetCurrentCharacter();
                if (!string.IsNullOrWhiteSpace(character?.Name))
                    characterName = character.Name;
            }
            catch
            {
            }

            try
            {
                characterId = _provider.GetCurrentCharacterId().ToString();
            }
            catch
            {
            }

            return new CharacterFileParts
            {
                Name = SanitizeFileNamePart(characterName),
                Id = SanitizeFileNamePart(characterId)
            };
        }

        private static string SanitizeFileNamePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "Unknown";

            var invalidChars = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(value.Length);
            foreach (var ch in value.Trim())
                builder.Append(Array.IndexOf(invalidChars, ch) >= 0 ? '_' : ch);

            return builder.ToString();
        }

        private void CopyTextToClipboard(string text, string successStatus)
        {
            if (string.IsNullOrEmpty(text))
            {
                lblStatus.Text = "Nothing to copy.";
                return;
            }

            try
            {
                Clipboard.SetText(text);
                lblStatus.Text = successStatus;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Copy failed: {ex.Message}";
            }
        }

        private void UpdateDetailsTextIfChanged(string text)
        {
            text ??= string.Empty;
            if (string.Equals(_lastDetailsText, text, StringComparison.Ordinal))
                return;

            if (txtConversation.Focused && txtConversation.SelectionLength > 0)
                return;

            _lastDetailsText = text;
            var selectionStart = txtConversation.SelectionStart;
            var selectionLength = txtConversation.SelectionLength;
            txtConversation.Text = text;
            if (selectionStart <= txtConversation.TextLength)
                txtConversation.Select(selectionStart, Math.Min(selectionLength, txtConversation.TextLength - selectionStart));
        }

        private string BuildTargetDetailsText(RulerTarget target, Rectangle? rawRect, Rectangle? adjustedRect, int? relX, int? relY)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Target: {target.Label}");
            sb.AppendLine($"ElementId: {GetTargetLogName(target)}");
            sb.AppendLine();

            if (rawRect.HasValue)
                sb.AppendLine($"Raw rect: L={rawRect.Value.Left} T={rawRect.Value.Top} R={rawRect.Value.Right} B={rawRect.Value.Bottom} W={rawRect.Value.Width} H={rawRect.Value.Height}");
            else
                sb.AppendLine("Raw rect: <not available>");

            if (adjustedRect.HasValue)
            sb.AppendLine($"Adjusted rect: L={adjustedRect.Value.Left} T={adjustedRect.Value.Top} R={adjustedRect.Value.Right} B={adjustedRect.Value.Bottom} W={adjustedRect.Value.Width} H={adjustedRect.Value.Height}");
            else
                sb.AppendLine("Adjusted rect: <not available>");

            sb.AppendLine($"Offsets: X={(int)nudOffsetX.Value} Y={(int)nudOffsetY.Value}");
            sb.AppendLine($"Grid spacing: {(int)nudGridSpacing.Value}");
            sb.AppendLine($"Show labels: {chkShowLabels.Checked}");
            sb.AppendLine($"Show grid: {chkShowGrid.Checked}");
            sb.AppendLine($"Show crosshair: {chkShowCrosshair.Checked}");

            if (relX.HasValue && relY.HasValue)
                sb.AppendLine($"Mouse rel: X={relX.Value} Y={relY.Value}");
            else
                sb.AppendLine("Mouse rel: <not available>");

            return sb.ToString();
        }

        private static bool TryParseHotbarLayout(string text, out List<HotbarLayoutEntry> entries, out string error)
        {
            entries = [];
            error = null;

            var lines = (text ?? string.Empty)
                .Split(["\r\n", "\n"], StringSplitOptions.None)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToArray();

            if (lines.Length == 0)
            {
                error = "Write a hotbar layout first.";
                return false;
            }

            var used = new HashSet<int>();
            for (var row = 0; row < lines.Length; row++)
            {
                var columns = lines[row].Split(',');
                for (var column = 0; column < columns.Length; column++)
                {
                    var token = columns[column].Trim();
                    if (token.Length == 0)
                        continue;

                    if (string.Equals(token, "x", StringComparison.OrdinalIgnoreCase) || token == "0")
                        continue;

                    var vertical = token.EndsWith("v", StringComparison.OrdinalIgnoreCase);
                    if (vertical)
                        token = token[..^1].Trim();

                    if (!int.TryParse(token, out var hotbar) || hotbar < 1 || hotbar > HotbarCount)
                    {
                        error = $"Invalid hotbar: {columns[column].Trim()}";
                        return false;
                    }

                    if (!used.Add(hotbar))
                    {
                        error = $"Hotbar {hotbar} appears twice.";
                        return false;
                    }

                    entries.Add(new HotbarLayoutEntry
                    {
                        Hotbar = hotbar,
                        Row = row,
                        Column = column,
                        Vertical = vertical
                    });
                }
            }

            return true;
        }

        private static void PositionLayout(List<HotbarLayoutEntry> entries, HotbarLayoutEntry anchor, Point anchorLocation)
        {
            var rowHeights = entries
                .GroupBy(e => e.Row)
                .ToDictionary(group => group.Key, group => group.Max(e => e.Height));
            var columnWidths = entries
                .GroupBy(e => e.Column)
                .ToDictionary(group => group.Key, group => group.Max(e => e.Width));

            var anchorX = SumBefore(columnWidths, anchor.Column) + (anchor.Column * HotbarLayoutHorizontalGap);
            var anchorY = SumBefore(rowHeights, anchor.Row);

            foreach (var entry in entries)
            {
                entry.Left = anchorLocation.X + SumBefore(columnWidths, entry.Column) + (entry.Column * HotbarLayoutHorizontalGap) - anchorX;
                entry.Top = anchorLocation.Y + SumBefore(rowHeights, entry.Row) - anchorY;
            }
        }

        private static int SumBefore(Dictionary<int, int> sizes, int index)
        {
            var total = 0;
            for (var i = 0; i < index; i++)
            {
                if (sizes.TryGetValue(i, out var size))
                    total += size;
            }

            return total;
        }

        private static IEnumerable<HotbarLayoutEntry> GetHotbarLayoutMoveOrder(List<HotbarLayoutEntry> entries, HotbarLayoutEntry anchor, bool layoutWasShifted)
        {
            var ordered = entries.OrderBy(e => e.Row).ThenBy(e => e.Column).ToList();
            if (!layoutWasShifted)
                return ordered;

            return ordered
                .OrderBy(e => e.Hotbar == anchor.Hotbar ? 0 : 1)
                .ThenBy(e => e.Row)
                .ThenBy(e => e.Column)
                .ToList();
        }

        private bool ConstrainLayoutToGameClient(List<HotbarLayoutEntry> entries, out bool layoutWasShifted)
        {
            layoutWasShifted = false;
            var bounds = TryGetClientAreaLayoutRectangle();
            if (!bounds.HasValue || entries.Count == 0)
                return true;

            bounds = new Rectangle(
                bounds.Value.Left,
                bounds.Value.Top,
                bounds.Value.Width,
                Math.Max(0, bounds.Value.Height - HotbarLayoutClientBottomMargin));

            var minLeft = entries.Min(e => e.Left);
            var minTop = entries.Min(e => e.Top);
            var maxRight = entries.Max(e => e.Left + e.Width);
            var maxBottom = entries.Max(e => e.Top + e.Height);
            var layoutWidth = maxRight - minLeft;
            var layoutHeight = maxBottom - minTop;

            if (layoutWidth > bounds.Value.Width || layoutHeight > bounds.Value.Height)
                return false;

            var dx = 0;
            var dy = 0;

            if (minLeft < bounds.Value.Left)
                dx = bounds.Value.Left - minLeft;
            else if (maxRight > bounds.Value.Right)
                dx = bounds.Value.Right - maxRight;

            if (minTop < bounds.Value.Top)
                dy = bounds.Value.Top - minTop;
            else if (maxBottom > bounds.Value.Bottom)
                dy = bounds.Value.Bottom - maxBottom;

            layoutWasShifted = dx != 0 || dy != 0;

            foreach (var entry in entries)
            {
                entry.Left += dx;
                entry.Top += dy;
            }

            minLeft += dx;
            minTop += dy;
            maxRight += dx;
            maxBottom += dy;
            if (minLeft < bounds.Value.Left
                || minTop < bounds.Value.Top
                || maxRight > bounds.Value.Right
                || maxBottom > bounds.Value.Bottom)
            {
                return false;
            }

            txtLog.AppendText($"Hotbar layout bounds L={minLeft} T={minTop} R={maxRight} B={maxBottom} inside layout client L={bounds.Value.Left} T={bounds.Value.Top} R={bounds.Value.Right} B={bounds.Value.Bottom}{Environment.NewLine}");
            return true;
        }

        private bool ApplyHotbarLayoutEntry(HotbarLayoutEntry entry)
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                if (IsEscapePressed())
                    return false;

                var target = EnsureHotbarOrientation(entry.Hotbar - 1, entry.Vertical);
                if (target == null)
                    return false;

                var rawRect = TryGetTargetRectangle(target);
                if (!rawRect.HasValue)
                    return false;

                if (IsAtSavedPosition(rawRect.Value, entry.Left, entry.Top))
                    return true;

                DragTargetToRawPosition(target, entry.Left, entry.Top);
                Thread.Sleep(250);

                rawRect = TryGetTargetRectangle(target);
                if (rawRect.HasValue && IsAtSavedPosition(rawRect.Value, entry.Left, entry.Top))
                    return true;
            }

            return false;
        }

        private bool DragTargetToRawPosition(RulerTarget target, int targetLeft, int targetTop)
        {
            var rawRect = TryGetTargetRectangle(target);
            if (!rawRect.HasValue)
            {
                lblStatus.Text = $"{target.Label} not found.";
                return false;
            }

            var offset = GetTargetOffset(target);
            var dragStart = new Point(rawRect.Value.Left + offset.X, rawRect.Value.Top + offset.Y);
            var dragEnd = new Point(targetLeft + offset.X, targetTop + offset.Y);

            _overlay?.HideOverlay();
            BringGameWindowToFront();
            _suppressHotbarSnap = true;
            try
            {
                DragMouse(dragStart, dragEnd);
            }
            finally
            {
                _suppressHotbarSnap = false;
            }

            return true;
        }

        private bool LoadHotbarSnapshot(HotbarSnapshot snapshot)
        {
            var desiredVertical = string.Equals(snapshot.Orientation, "Vertical", StringComparison.OrdinalIgnoreCase);
            var desiredLeft = snapshot.RawRect.Value.Left;
            var desiredTop = snapshot.RawRect.Value.Top;

            for (var attempt = 0; attempt < 2; attempt++)
            {
                if (IsEscapePressed())
                    return false;

                var target = EnsureHotbarOrientation(snapshot.Hotbar - 1, desiredVertical);
                if (target == null)
                    return false;

                var rawRect = TryGetTargetRectangle(target);
                if (!rawRect.HasValue)
                    return false;

                if (IsAtSavedPosition(rawRect.Value, desiredLeft, desiredTop))
                    return true;

                DragTargetToRawPosition(target, desiredLeft, desiredTop);
                Thread.Sleep(250);

                rawRect = TryGetTargetRectangle(target);
                if (rawRect.HasValue && IsAtSavedPosition(rawRect.Value, desiredLeft, desiredTop))
                    return true;
            }

            return false;
        }

        private static bool IsAtSavedPosition(Rectangle rawRect, int desiredLeft, int desiredTop)
        {
            return Math.Abs(rawRect.Left - desiredLeft) <= PositionTolerancePixels
                && Math.Abs(rawRect.Top - desiredTop) <= PositionTolerancePixels;
        }

        private RulerTarget EnsureHotbarOrientation(int index, bool desiredVertical)
        {
            var horizontal = CreateHotbarTarget(index, vertical: false);
            var vertical = CreateHotbarTarget(index, vertical: true);
            var horizontalRect = horizontal == null ? null : TryGetTargetRectangle(horizontal);
            var verticalRect = vertical == null ? null : TryGetTargetRectangle(vertical);
            var currentTarget = ChooseHotbarTarget(index, horizontal, horizontalRect, vertical, verticalRect);

            if (currentTarget == null)
                return desiredVertical ? vertical : horizontal;

            if (currentTarget.IsVerticalHotbar == desiredVertical)
                return currentTarget;

            if (RotateHotbar(currentTarget, desiredVertical))
            {
                var desiredTarget = desiredVertical ? vertical : horizontal;
                if (desiredTarget != null && TryGetTargetRectangle(desiredTarget).HasValue)
                    return desiredTarget;
            }

            return desiredVertical ? vertical : horizontal;
        }

        private bool RotateHotbar(RulerTarget target, bool desiredVertical)
        {
            for (var attempt = 0; attempt < 2; attempt++)
            {
                if (IsEscapePressed())
                    return false;

                var rawRect = TryGetTargetRectangle(target);
                if (!rawRect.HasValue)
                    return false;

                var offset = GetTargetOffset(target);
                var clickX = rawRect.Value.Left + offset.X + (target.IsVerticalHotbar ? 10 : 12);
                var clickY = rawRect.Value.Top + offset.Y + (target.IsVerticalHotbar ? 10 : 32);

                _overlay?.HideOverlay();
                BringGameWindowToFront();
                txtLog.AppendText($"Rotating {GetTargetLogName(target)} at {clickX},{clickY} attempt {attempt + 1}{Environment.NewLine}");
                ClickMouse(clickX, clickY);
                Thread.Sleep(350);

                var desiredTarget = CreateHotbarTarget(GetHotbarIndex(target), desiredVertical);
                if (desiredTarget != null && TryGetTargetRectangle(desiredTarget).HasValue)
                    return true;
            }

            return false;
        }

        private HotbarSnapshot BuildHotbarSnapshot(int index)
        {
            var horizontal = CreateHotbarTarget(index, vertical: false);
            var vertical = CreateHotbarTarget(index, vertical: true);
            var horizontalRect = horizontal == null ? null : TryGetTargetRectangle(horizontal);
            var verticalRect = vertical == null ? null : TryGetTargetRectangle(vertical);

            var target = ChooseHotbarTarget(index, horizontal, horizontalRect, vertical, verticalRect);
            var rawRect = target == vertical ? verticalRect : horizontalRect;
            var offset = GetTargetOffset(target);

            return new HotbarSnapshot
            {
                Hotbar = index + 1,
                Orientation = target.IsVerticalHotbar ? "Vertical" : "Horizontal",
                ElementId = target.ElementId?.ToString(),
                Offset = offset,
                RawRect = rawRect,
                AdjustedRect = rawRect.HasValue
                    ? new Rectangle(rawRect.Value.Left + offset.X, rawRect.Value.Top + offset.Y, rawRect.Value.Width, rawRect.Value.Height)
                    : null
            };
        }

        private static RulerTarget ChooseHotbarTarget(
            int index,
            RulerTarget horizontal,
            Rectangle? horizontalRect,
            RulerTarget vertical,
            Rectangle? verticalRect)
        {
            if (IsLikelyVerticalHotbar(verticalRect))
                return vertical;

            if (IsLikelyHorizontalHotbar(horizontalRect))
                return horizontal;

            if (verticalRect.HasValue && !horizontalRect.HasValue)
                return vertical;

            if (horizontalRect.HasValue && !verticalRect.HasValue)
                return horizontal;

            // Fallback for the user's current layout: bars 10-14 vertical; 15 and 1-9 horizontal.
            return index >= 9 && index <= 13 && vertical != null ? vertical : horizontal ?? vertical;
        }

        private static bool IsLikelyHorizontalHotbar(Rectangle? rect)
        {
            return rect.HasValue && rect.Value.Width >= 350 && rect.Value.Height <= 80;
        }

        private static bool IsLikelyVerticalHotbar(Rectangle? rect)
        {
            return rect.HasValue && rect.Value.Width <= 80 && rect.Value.Height >= 350;
        }

        private RulerTarget ChooseCurrentHotbarTarget(int index)
        {
            var horizontal = CreateHotbarTarget(index, vertical: false);
            var vertical = CreateHotbarTarget(index, vertical: true);
            var horizontalRect = horizontal == null ? null : TryGetTargetRectangle(horizontal);
            var verticalRect = vertical == null ? null : TryGetTargetRectangle(vertical);
            return ChooseHotbarTarget(index, horizontal, horizontalRect, vertical, verticalRect);
        }

        private static RulerTarget CreateHotbarTarget(int index, bool vertical)
        {
            var name = vertical
                ? $"UndockedShortcut{index}_Vertical_MarkerList"
                : $"UndockedShortcut{index}_Horizontal_MarkerList";

            if (!Enum.TryParse(name, out UIElementID elementId))
                return null;

            return new RulerTarget
            {
                Label = name,
                ElementId = elementId,
                IsHotbar = true,
                IsVerticalHotbar = vertical
            };
        }

        private static int GetHotbarIndex(RulerTarget target)
        {
            if (target?.ElementId == null)
                return -1;

            var name = target.ElementId.Value.ToString();
            const string prefix = "UndockedShortcut";
            if (!name.StartsWith(prefix, StringComparison.Ordinal))
                return -1;

            var suffixStart = prefix.Length;
            var suffixEnd = name.IndexOf('_', suffixStart);
            if (suffixEnd <= suffixStart)
                return -1;

            return int.TryParse(name.Substring(suffixStart, suffixEnd - suffixStart), out var index) ? index : -1;
        }

        private RulerTarget GetSelectedTarget()
        {
            return cboTargetElement.SelectedItem as RulerTarget ?? _availableTargets[0];
        }

        private void HandlePendingClickCapture()
        {
            var isLeftDown = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;

            try
            {
                HandleHotbarSnapTransition(isLeftDown);

                if (_captureNextLeftClick && isLeftDown && !_wasLeftMouseDown && !IsCursorOverUiRuler())
                    LogMeasurementAtCursor();
            }
            finally
            {
                _wasLeftMouseDown = isLeftDown;
            }
        }

        private void HandleHotbarSnapTransition(bool isLeftDown)
        {
            if (!_snapHotbarsEnabled || _suppressHotbarSnap)
            {
                _snapPreviewActive = false;
                return;
            }

            if (isLeftDown && !_wasLeftMouseDown && !IsCursorOverUiRuler())
            {
                _snapDragStartRects = CaptureCurrentHotbarRects();
            }

            if (isLeftDown && _snapDragStartRects.Count > 0)
            {
                UpdateHotbarSnapPreview(_snapDragStartRects);
                return;
            }

            _snapPreviewActive = false;

            if (!_wasLeftMouseDown || _snapDragStartRects.Count == 0)
                return;

            var startRects = _snapDragStartRects;
            _snapDragStartRects = new Dictionary<int, Rectangle>();
            ApplyHotbarSnap(startRects);
        }

        private Dictionary<int, Rectangle> CaptureCurrentHotbarRects()
        {
            var rects = new Dictionary<int, Rectangle>();
            for (var i = 0; i < HotbarCount; i++)
            {
                var target = ChooseCurrentHotbarTarget(i);
                var rect = target == null ? null : TryGetTargetRectangle(target);
                if (rect.HasValue)
                    rects[i] = NormalizeHotbarRect(target, rect.Value);
            }

            return rects;
        }

        private void ApplyHotbarSnap(Dictionary<int, Rectangle> startRects)
        {
            var currentRects = CaptureCurrentHotbarRects();
            if (currentRects.Count < 2)
                return;

            var movedIndex = -1;
            var movedDistance = 0;
            foreach (var current in currentRects)
            {
                if (!startRects.TryGetValue(current.Key, out var start))
                    continue;

                var distance = Math.Abs(current.Value.Left - start.Left) + Math.Abs(current.Value.Top - start.Top);
                if (distance > movedDistance)
                {
                    movedDistance = distance;
                    movedIndex = current.Key;
                }
            }

            if (movedIndex < 0 || movedDistance <= PositionTolerancePixels)
                return;

            var movedRect = currentRects[movedIndex];
            var otherRects = currentRects
                .Where(pair => pair.Key != movedIndex)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            if (!TryFindSnappedHotbarPosition(movedRect, otherRects, Cursor.Position, out var snap))
                return;

            if (IsAtSavedPosition(movedRect, snap.TargetRect.Left, snap.TargetRect.Top))
                return;

            var target = ChooseCurrentHotbarTarget(movedIndex);
            if (target == null)
                return;

            txtLog.AppendText($"Snapping hotbar {movedIndex + 1} to L={snap.TargetRect.Left} T={snap.TargetRect.Top}{Environment.NewLine}");
            DragTargetToRawPosition(target, snap.TargetRect.Left, snap.TargetRect.Top);
            lblStatus.Text = $"Snapped hotbar {movedIndex + 1}.";
        }

        private void UpdateHotbarSnapPreview(Dictionary<int, Rectangle> startRects)
        {
            var currentRects = CaptureCurrentHotbarRects();
            if (currentRects.Count < 2)
            {
                _snapPreviewActive = false;
                return;
            }

            var movedIndex = FindMovedHotbarIndex(startRects, currentRects);
            if (movedIndex < 0)
            {
                _snapPreviewActive = false;
                return;
            }

            var movedRect = currentRects[movedIndex];
            var otherRects = currentRects
                .Where(pair => pair.Key != movedIndex)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            if (!TryFindSnappedHotbarPosition(movedRect, otherRects, Cursor.Position, out var snap))
            {
                _snapPreviewActive = false;
                return;
            }

            if (_overlay == null || _overlay.IsDisposed)
                _overlay = CreateOverlay();

            if (!TryConvertLayoutRectToScreen(movedRect, out var movingScreenRect)
                || !TryConvertLayoutRectToScreen(snap.NeighborRect, out var neighborScreenRect)
                || !TryConvertLayoutRectToScreen(snap.TargetRect, out var targetScreenRect))
            {
                _snapPreviewActive = false;
                return;
            }

            _overlay.UpdateSnapPreview(movingScreenRect, neighborScreenRect, targetScreenRect, snap.Side);
            _snapPreviewActive = true;
        }

        private bool TryConvertLayoutRectToScreen(Rectangle rect, out Rectangle screenRect)
        {
            var clientScreenRect = TryGetClientAreaRectangle();
            var clientLayoutRect = TryGetClientAreaLayoutRectangle();
            if (!clientScreenRect.HasValue || !clientLayoutRect.HasValue)
            {
                screenRect = rect;
                return false;
            }

            screenRect = new Rectangle(
                clientScreenRect.Value.Left + rect.Left - clientLayoutRect.Value.Left,
                clientScreenRect.Value.Top + rect.Top - clientLayoutRect.Value.Top,
                rect.Width,
                rect.Height);
            return true;
        }

        private static int FindMovedHotbarIndex(Dictionary<int, Rectangle> startRects, Dictionary<int, Rectangle> currentRects)
        {
            var movedIndex = -1;
            var movedDistance = 0;
            foreach (var current in currentRects)
            {
                if (!startRects.TryGetValue(current.Key, out var start))
                    continue;

                var distance = Math.Abs(current.Value.Left - start.Left) + Math.Abs(current.Value.Top - start.Top);
                if (distance > movedDistance)
                {
                    movedDistance = distance;
                    movedIndex = current.Key;
                }
            }

            return movedDistance <= PositionTolerancePixels ? -1 : movedIndex;
        }

        private static Rectangle NormalizeHotbarRect(RulerTarget target, Rectangle rect)
        {
            return new Rectangle(
                rect.Left,
                rect.Top,
                target.IsVerticalHotbar ? VerticalHotbarWidth : HorizontalHotbarWidth,
                target.IsVerticalHotbar ? VerticalHotbarHeight : HorizontalHotbarHeight);
        }

        private static bool TryFindSnappedHotbarPosition(Rectangle movedRect, Dictionary<int, Rectangle> otherRects, Point mouseScreen, out HotbarSnapCandidate snap)
        {
            snap = default;
            var candidates = new List<HotbarSnapCandidate>();

            foreach (var other in otherRects.OrderBy(pair => DistanceFromPointToRect(mouseScreen, pair.Value)))
            {
                var anchor = other.Value;
                var gap = DistanceBetweenRects(movedRect, anchor);
                var overlaps = RectsOverlap(movedRect, anchor);
                if (!overlaps && gap > SnapActivationPixels)
                    continue;

                AddSnapCandidate(movedRect, anchor.Left, anchor.Top - movedRect.Height, anchor, SnapSide.Above, mouseScreen, candidates);
                AddSnapCandidate(movedRect, anchor.Left, anchor.Bottom, anchor, SnapSide.Below, mouseScreen, candidates);
                AddSnapCandidate(movedRect, anchor.Left - movedRect.Width - HotbarLayoutHorizontalGap, anchor.Top, anchor, SnapSide.Left, mouseScreen, candidates);
                AddSnapCandidate(movedRect, anchor.Right + HotbarLayoutHorizontalGap, anchor.Top, anchor, SnapSide.Right, mouseScreen, candidates);

                foreach (var candidate in candidates
                    .Where(c => c.NeighborRect == anchor)
                    .OrderBy(c => c.MouseDistance)
                    .ThenBy(c => c.MoveDistance))
                {
                    if (otherRects.Values.Any(rect => rect != anchor && RectsOverlap(candidate.TargetRect, rect)))
                        continue;

                    snap = candidate;
                    return true;
                }

                candidates.Clear();
            }

            return false;
        }

        private static void AddSnapCandidate(
            Rectangle movedRect,
            int left,
            int top,
            Rectangle anchor,
            SnapSide side,
            Point mouseScreen,
            List<HotbarSnapCandidate> candidates)
        {
            var candidateRect = new Rectangle(left, top, movedRect.Width, movedRect.Height);
            var distance = Math.Abs(candidateRect.Left - movedRect.Left) + Math.Abs(candidateRect.Top - movedRect.Top);
            if (distance > SnapDistancePixels && !RectsOverlap(movedRect, anchor))
                return;

            candidates.Add(new HotbarSnapCandidate
            {
                TargetRect = candidateRect,
                NeighborRect = anchor,
                Side = side,
                MoveDistance = distance,
                MouseDistance = DistanceFromPointToRect(mouseScreen, anchor)
            });
        }

        private struct HotbarSnapCandidate
        {
            public Rectangle TargetRect { get; init; }

            public Rectangle NeighborRect { get; init; }

            public SnapSide Side { get; init; }

            public int MoveDistance { get; init; }

            public int MouseDistance { get; init; }
        }

        private static int DistanceBetweenRects(Rectangle a, Rectangle b)
        {
            var dx = a.Right < b.Left ? b.Left - a.Right : b.Right < a.Left ? a.Left - b.Right : 0;
            var dy = a.Bottom < b.Top ? b.Top - a.Bottom : b.Bottom < a.Top ? a.Top - b.Bottom : 0;
            return dx + dy;
        }

        private static bool RectsOverlap(Rectangle a, Rectangle b)
        {
            return a.Left < b.Right
                && a.Right > b.Left
                && a.Top < b.Bottom
                && a.Bottom > b.Top;
        }

        private static int DistanceFromPointToRect(Point point, Rectangle rect)
        {
            var dx = point.X < rect.Left ? rect.Left - point.X : point.X > rect.Right ? point.X - rect.Right : 0;
            var dy = point.Y < rect.Top ? rect.Top - point.Y : point.Y > rect.Bottom ? point.Y - rect.Bottom : 0;
            return dx + dy;
        }

        private bool IsCursorOverUiRuler()
        {
            if (!Visible || !IsHandleCreated)
                return false;

            var screenBounds = RectangleToScreen(ClientRectangle);
            return screenBounds.Contains(Cursor.Position);
        }

        private void LogMeasurementAtCursor()
        {
            var target = GetSelectedTarget();
            var rawRect = TryGetTargetRectangle(target);
            if (!rawRect.HasValue)
            {
                txtLog.AppendText($"{GetTargetLogName(target)} not found. Mouse={Cursor.Position.X},{Cursor.Position.Y}{Environment.NewLine}");
                lblStatus.Text = $"{target.Label} not found while capturing click.";
                _captureNextLeftClick = false;
                return;
            }

            var adjustedRect = ApplyOffsets(rawRect.Value);
            var relX = Cursor.Position.X - adjustedRect.Left;
            var relY = Cursor.Position.Y - adjustedRect.Top;
            var line = $"{GetTargetLogName(target)} raw L={rawRect.Value.Left} T={rawRect.Value.Top} R={rawRect.Value.Right} B={rawRect.Value.Bottom} W={rawRect.Value.Width} H={rawRect.Value.Height} Adjusted L={adjustedRect.Left} T={adjustedRect.Top} Offset={(int)nudOffsetX.Value},{(int)nudOffsetY.Value} Mouse={Cursor.Position.X},{Cursor.Position.Y} Rel={relX},{relY}";
            txtLog.AppendText(line + Environment.NewLine);
            lblStatus.Text = $"Captured {GetTargetLogName(target)} {relX},{relY}";
            _captureNextLeftClick = false;
        }

        private void CaptureSnapshot()
        {
            CaptureSnapshotInternal("captures");
        }

        private void CaptureSnapshotInternal(string subfolderName)
        {
            var target = GetSelectedTarget();
            var rawRect = TryGetTargetRectangle(target);
            var adjustedRect = rawRect.HasValue ? ApplyOffsets(rawRect.Value) : (Rectangle?)null;
            if (adjustedRect == null || adjustedRect.Value.Width <= 0 || adjustedRect.Value.Height <= 0)
                throw new InvalidOperationException($"Could not read valid bounds for {GetTargetLogName(target)}.");

            var timestamp = DateTime.Now;
            var capturesDir = Path.Combine(_folder, subfolderName);
            Directory.CreateDirectory(capturesDir);

            var baseName = $"{timestamp:yyyyMMdd_HHmmss_fff}_{GetTargetLogName(target)}";
            var imagePath = Path.Combine(capturesDir, baseName + ".png");
            var textPath = Path.Combine(capturesDir, baseName + ".txt");

            _overlay?.BringToFront();
            _overlay?.Refresh();
            Refresh();
            Thread.Sleep(50);

            using (var bitmap = new Bitmap(adjustedRect.Value.Width, adjustedRect.Value.Height))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(adjustedRect.Value.Location, Point.Empty, adjustedRect.Value.Size);
                bitmap.Save(imagePath, ImageFormat.Png);
            }

            File.WriteAllText(textPath, BuildSnapshotMetadata(target, rawRect, adjustedRect, timestamp, imagePath));

            txtLog.AppendText($"Snapshot saved: {imagePath}{Environment.NewLine}");
            lblStatus.Text = $"Snapshot saved: {Path.GetFileName(imagePath)}";
        }

        private Rectangle? TryGetTargetRectangle(RulerTarget target)
        {
            if (target.UseScreenArea)
                return TryGetScreenAreaRectangle();

            if (target.UseClientArea)
                return TryGetClientAreaRectangle();

            if (!target.ElementId.HasValue)
                return null;

            var elementLocation = _provider.GetUiElementLocation((uint)target.ElementId.Value);
            if (!elementLocation.HasValue)
                return null;

            return Rectangle.FromLTRB(elementLocation.Value.Left, elementLocation.Value.Top, elementLocation.Value.Right, elementLocation.Value.Bottom);
        }

        private Rectangle ApplyOffsets(Rectangle rect)
        {
            return new Rectangle(rect.Left + (int)nudOffsetX.Value, rect.Top + (int)nudOffsetY.Value, rect.Width, rect.Height);
        }

        private string GetTargetLogName(RulerTarget target)
        {
            if (target.UseScreenArea)
                return "WholeScreen";

            return target.ElementId?.ToString() ?? "ClientArea";
        }

        private Rectangle? TryGetScreenAreaRectangle()
        {
            var windowHandle = _provider.GameProcess?.MainWindowHandle ?? IntPtr.Zero;
            if (windowHandle == IntPtr.Zero)
                return null;

            var screen = Screen.FromHandle(windowHandle);
            return screen?.Bounds;
        }

        private Rectangle? TryGetClientAreaLayoutRectangle()
        {
            var windowHandle = _provider.GameProcess?.MainWindowHandle ?? IntPtr.Zero;
            if (windowHandle == IntPtr.Zero)
                return null;

            if (!GetClientRect(windowHandle, out var clientRect))
                return null;

            return new Rectangle(
                clientRect.Left,
                clientRect.Top,
                clientRect.Right - clientRect.Left,
                clientRect.Bottom - clientRect.Top);
        }

        private Rectangle? TryGetClientAreaRectangle()
        {
            var windowHandle = _provider.GameProcess?.MainWindowHandle ?? IntPtr.Zero;
            if (windowHandle == IntPtr.Zero)
                return null;

            if (!GetClientRect(windowHandle, out var clientRect))
                return null;

            var origin = new POINT { X = clientRect.Left, Y = clientRect.Top };
            if (!ClientToScreen(windowHandle, ref origin))
                return null;

            return new Rectangle(
                origin.X,
                origin.Y,
                clientRect.Right - clientRect.Left,
                clientRect.Bottom - clientRect.Top);
        }

        private string BuildSnapshotMetadata(RulerTarget target, Rectangle? rawRect, Rectangle? adjustedRect, DateTime timestamp, string imagePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Timestamp: {timestamp:yyyy-MM-dd HH:mm:ss.fff}");
            sb.AppendLine($"Target: {target.Label}");
            sb.AppendLine($"ElementId: {GetTargetLogName(target)}");
            sb.AppendLine($"Image: {imagePath}");
            sb.AppendLine($"GridSpacing: {(int)nudGridSpacing.Value}");
            sb.AppendLine($"Offsets: X={(int)nudOffsetX.Value} Y={(int)nudOffsetY.Value}");
            sb.AppendLine($"ShowLabels: {chkShowLabels.Checked}");
            sb.AppendLine($"ShowGrid: {chkShowGrid.Checked}");
            sb.AppendLine($"ShowCrosshair: {chkShowCrosshair.Checked}");

            if (rawRect.HasValue)
                sb.AppendLine($"RawRect: L={rawRect.Value.Left} T={rawRect.Value.Top} R={rawRect.Value.Right} B={rawRect.Value.Bottom} W={rawRect.Value.Width} H={rawRect.Value.Height}");
            else
                sb.AppendLine("RawRect: <not available>");

            if (adjustedRect.HasValue)
                sb.AppendLine($"AdjustedRect: L={adjustedRect.Value.Left} T={adjustedRect.Value.Top} R={adjustedRect.Value.Right} B={adjustedRect.Value.Bottom} W={adjustedRect.Value.Width} H={adjustedRect.Value.Height}");
            else
                sb.AppendLine("AdjustedRect: <not available>");

            sb.AppendLine();
            sb.AppendLine("Details:");
            sb.AppendLine(txtConversation.Text);

            return sb.ToString();
        }

        private static void DragMouse(Point start, Point end)
        {
            SetCursorPos(start.X, start.Y);
            Thread.Sleep(200);
            mouse_event(MouseEventLeftDown, 0, 0, 0, UIntPtr.Zero);

            Thread.Sleep(200);
            const int steps = 36;
            for (var i = 1; i <= steps; i++)
            {
                if (IsEscapePressed())
                    break;

                var x = start.X + ((end.X - start.X) * i / steps);
                var y = start.Y + ((end.Y - start.Y) * i / steps);
                SetCursorPos(x, y);
                Thread.Sleep(18);
            }

            Thread.Sleep(250);
            mouse_event(MouseEventLeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        private static void ClickMouse(int x, int y)
        {
            SetCursorPos(x, y);
            Thread.Sleep(120);
            mouse_event(MouseEventLeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(80);
            mouse_event(MouseEventLeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        private void BringGameWindowToFront()
        {
            var windowHandle = _provider.GameProcess?.MainWindowHandle ?? IntPtr.Zero;
            if (windowHandle == IntPtr.Zero)
                return;

            ShowWindow(windowHandle, ShowWindowRestore);
            SetForegroundWindow(windowHandle);
            Thread.Sleep(200);
        }

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static bool IsEscapePressed()
        {
            return (GetAsyncKeyState(VirtualKeyEscape) & 0x8000) != 0;
        }

        private const uint MouseEventLeftDown = 0x0002;

        private const uint MouseEventLeftUp = 0x0004;

        private const int ShowWindowRestore = 9;

        private const int VirtualKeyEscape = 0x1B;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }
    }
}
