using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DPGTProject.Configs
{
    public class DesignConfig
    {
        public enum ApplicationTheme
        {
            SystemDefault,
            Light,
            Dark,
            Blue,
            HighContrast
        }

        // Цветовая палитра
        private static Color _formBackgroundColor = SystemColors.Control;
        public static Color FormBackgroundColor
        {
            get => _formBackgroundColor;
            set
            {
                _formBackgroundColor = value;
                // Автоматически обновить все открытые формы
                foreach (Form form in Application.OpenForms)
                {
                    form.BackColor = value;
                }
            }
        }
        public static Color PrimaryColor { get; set; } = SystemColors.HotTrack;
        public static Color SecondaryColor { get; set; } = SystemColors.Control;
        public static Color AccentColor { get; set; } = SystemColors.Highlight;
        public static Color TextColor { get; set; } = SystemColors.ControlText;
        public static Color ErrorColor { get; set; } = SystemColors.ControlDark;
        public static Color DisabledControlColor { get; set; } = SystemColors.ControlDark;
        public static Color DisabledTextColor { get; set; } = SystemColors.GrayText;
        public static Color GridLineColor { get; set; } = SystemColors.ControlDark;
        public static Color LinesColor { get; set; } = SystemColors.ControlDark;
        public static int BorderSize { get; set; } = 1;

        // Основной метод для настройки контролов
        public static void ApplyControlsSettings(Control[] controls, Dictionary<string, object> settings = null, bool usePalette = false)
        {
            ApplyControlsSettings((IEnumerable<Control>)controls, settings, usePalette);
        }

        // Перегрузка для IEnumerable
        public static void ApplyControlsSettings(IEnumerable<Control> controls, Dictionary<string, object> settings = null, bool usePalette = false)
        {
            if (controls == null || !controls.Any())
                return;

            foreach (var control in controls)
            {
                try
                {
                    if (settings != null && !usePalette)
                    {
                        ApplySettings(control, settings);
                    }
                    else if (usePalette)
                    {
                        ApplyPaletteSettings(control);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error applying settings to {control.Name}: {ex.Message}");
                }
            }
        }
        private static void ApplyControlsSettings(Control.ControlCollection controls, Dictionary<string, object> settings = null, bool usePalette = false)
        {
            if (controls == null || controls.Count == 0)
                return;

            ApplyControlsSettings(controls.Cast<Control>(), settings, usePalette);
        }

        private static void ApplySettings(Control control, Dictionary<string, object> settings)
        {
            foreach (var setting in settings)
            {
                var property = control.GetType().GetProperty(setting.Key);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(control, setting.Value);
                }
            }
        }

        private static void ApplyPaletteSettings(Control control)
        {
            // Базовые настройки для всех контролов
            control.BackColor = SecondaryColor;
            control.ForeColor = TextColor;

            // Специальные настройки для разных типов контролов
            switch (control)
            {
                case Button button:
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = BorderSize;
                    button.FlatAppearance.BorderColor = LinesColor;
                    if (button.Enabled)
                    {
                        button.BackColor = PrimaryColor;
                        button.ForeColor = Color.White;
                    }
                    else
                    {
                        button.BackColor = DisabledControlColor;
                        button.ForeColor = DisabledTextColor;
                    }
                    break;

                case TextBox textBox:
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;

                case Label label:
                    label.BackColor = Color.Transparent;
                    break;

                case Form form:
                    form.BackColor = FormBackgroundColor;
                    break;

                case CheckBox checkBox:
                case RadioButton radioButton:
                    control.BackColor = Color.Transparent;
                    break;

                case ComboBox comboBox:
                    comboBox.FlatStyle = FlatStyle.Flat;
                    comboBox.BackColor = SecondaryColor;
                    comboBox.ForeColor = TextColor;
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    if (!comboBox.Enabled)
                    {
                        comboBox.BackColor = DisabledControlColor;
                    }
                    break;
                case ListBox listBox:
                    listBox.BackColor = SecondaryColor;
                    listBox.ForeColor = TextColor;
                    break;

                case DataGridView grid:
                    grid.BackgroundColor = SecondaryColor;
                    grid.ForeColor = TextColor;
                    grid.BorderStyle = BorderStyle.None;
                    grid.EnableHeadersVisualStyles = false;
                    grid.DefaultCellStyle.BackColor = SecondaryColor;
                    grid.DefaultCellStyle.ForeColor = TextColor;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    grid.RowHeadersDefaultCellStyle.BackColor = SecondaryColor;
                    grid.RowHeadersDefaultCellStyle.ForeColor = TextColor;
                    grid.GridColor = GridLineColor;
                    break;

                case Panel panel:
                case GroupBox groupBox:
                    control.BackColor = SecondaryColor;
                    break;

                case MenuStrip menu:
                case ToolStrip tool:
                    control.BackColor = SecondaryColor;
                    control.ForeColor = TextColor;
                    break;
            }

            // Рекурсивная обработка дочерних контролов
            foreach (Control childControl in control.Controls)
            {
                ApplyPaletteSettings(childControl);
            }
        }


        // Методы для работы с темами
        public static void ApplyTheme(ApplicationTheme theme, Control[] controls = null)
        {
            ApplyTheme(theme, (IEnumerable<Control>)controls);
        }

        // Перегрузка для IEnumerable
        public static void ApplyTheme(ApplicationTheme theme, Control control)
        {
            ApplyTheme(theme, new[] { control });
        }

        public static void ApplyTheme(ApplicationTheme theme, IEnumerable<Control> controls = null)
        {
            if (!SystemConfig.applyCustomThemes) { Console.WriteLine("Невозможно применить тему. Это отключено в настройках."); return; }
            switch (theme)
            {
                case ApplicationTheme.SystemDefault:
                    return;
                case ApplicationTheme.Light:
                    SetLightTheme();
                    break;
                case ApplicationTheme.Dark:
                    SetDarkTheme();
                    break;
                case ApplicationTheme.Blue:
                    SetBlueTheme();
                    break;
                case ApplicationTheme.HighContrast:
                    SetHighContrastTheme();
                    break;
            }

            if (controls != null)
            {
                ApplyControlsSettings(controls, usePalette: true);
            }
            // Обновить все открытые формы
            foreach (Form form in Application.OpenForms)
            {
                form.BackColor = FormBackgroundColor;
                ApplyControlsSettings(form.Controls, usePalette: true);
            }
        }

        private static void SetSystemColors()
        {
            PrimaryColor = SystemColors.HotTrack;
            SecondaryColor = SystemColors.Control;
            FormBackgroundColor = SystemColors.Control;
            AccentColor = SystemColors.Highlight;
            TextColor = SystemColors.ControlText;
            ErrorColor = SystemColors.ControlDark;
            DisabledControlColor = SystemColors.ControlDark;
            DisabledTextColor = SystemColors.GrayText;
            GridLineColor = SystemColors.ControlDark;
            BorderSize = 1;
        }

        private static void SetLightTheme()
        {
            PrimaryColor = Color.FromArgb(0, 120, 215);
            SecondaryColor = Color.WhiteSmoke;
            FormBackgroundColor = Color.WhiteSmoke;
            AccentColor = Color.FromArgb(0, 153, 255);
            TextColor = Color.Black;
            ErrorColor = Color.FromArgb(255, 50, 50);
            DisabledControlColor = Color.FromArgb(220, 220, 220);
            DisabledTextColor = Color.Gray;
            GridLineColor = Color.FromArgb(200, 200, 200);
            BorderSize = 0;
        }

        private static void SetDarkTheme()
        {
            PrimaryColor = Color.FromArgb(0, 120, 215);
            SecondaryColor = Color.FromArgb(45, 45, 48);
            FormBackgroundColor = Color.FromArgb(45, 45, 48);
            AccentColor = Color.FromArgb(0, 153, 255);
            TextColor = Color.White;
            ErrorColor = Color.FromArgb(255, 50, 50);
            DisabledControlColor = Color.FromArgb(80, 80, 80);
            DisabledTextColor = Color.Gray;
            GridLineColor = Color.FromArgb(80, 80, 80);
            BorderSize = 0;
        }

        private static void SetBlueTheme()
        {
            PrimaryColor = Color.FromArgb(0, 90, 158);
            SecondaryColor = Color.FromArgb(240, 240, 240);
            FormBackgroundColor = Color.FromArgb(240, 240, 240);
            AccentColor = Color.FromArgb(0, 120, 215);
            TextColor = Color.Black;
            ErrorColor = Color.FromArgb(200, 0, 0);
            DisabledControlColor = Color.FromArgb(200, 200, 200);
            DisabledTextColor = Color.Gray;
            GridLineColor = Color.FromArgb(180, 180, 180);
            BorderSize = 0;
        }

        private static void SetHighContrastTheme()
        {
            PrimaryColor = Color.Black;
            SecondaryColor = Color.White;
            FormBackgroundColor = Color.White;
            AccentColor = Color.Yellow;
            TextColor = Color.Black;
            ErrorColor = Color.Red;
            DisabledControlColor = Color.Gray;
            DisabledTextColor = Color.DarkGray;
            GridLineColor = Color.Black;
            BorderSize = 1;
        }


        //   # ЭТО ВСЁ НЕОБХОДИМО ПРОПИСЫВАТЬ В НЕОБХОДИМОЙ ДЛЯ ВАС ФОРМЕ, !УЧТИТЕ!, ЧТО ПРИ ИНИЦИАЛИЗАЦИИ КОНСТРУКТОРА, ВАША ТЕМА МОЖЕТ ЗАМЕНИТЬСЯ!
        //   # Если необходимо применять различные параметры:
        //   # Применение настроек к контролам
        //       DesignConfig.ApplyControlsSettings(myControls, new Dictionary<string, object>
        //       {
        //           {"Font", new Font("Arial", 10)},
        //           {"BackColor", Color.White}
        //       });

        //   # Или применение темы
        //       DesignConfig.ApplyTheme(DesignConfig.ApplicationTheme.Dark, allControls);
        //   # Можно на вход подать свой массив контролов, либо абсолютно все. Если объект не может сменить цвет - он просто выдаст ошибку в консоль и продолжит работу.
        //   # ЭТО ВСЁ НЕОБХОДИМО ПРОПИСЫВАТЬ В НЕОБХОДИМОЙ ДЛЯ ВАС ФОРМЕ, !УЧТИТЕ!, ЧТО ПРИ ИНИЦИАЛИЗАЦИИ КОНСТРУКТОРА, ВАША ТЕМА МОЖЕТ ЗАМЕНИТЬСЯ!
    }
}
