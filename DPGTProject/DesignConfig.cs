using DPGTProject.Properties;
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
        public static Color PrimaryColor { get; set; } = SystemColors.HotTrack;
        public static Color SecondaryColor { get; set; } = SystemColors.Control;
        public static Color AccentColor { get; set; } = SystemColors.Highlight;
        public static Color TextColor { get; set; } = SystemColors.ControlText;
        public static Color ErrorColor { get; set; } = SystemColors.ControlDark;

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
                    button.BackColor = PrimaryColor;
                    button.ForeColor = Color.White;
                    break;

                case TextBox textBox:
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                    break;

                case Label label:
                    label.BackColor = Color.Transparent;
                    break;

                case Form form:
                    form.BackColor = SecondaryColor;
                    break;

                case CheckBox checkBox:
                case RadioButton radioButton:
                    control.BackColor = Color.Transparent;
                    break;

                case ComboBox comboBox:
                case ListBox listBox:
                    control.BackColor = SecondaryColor;
                    control.ForeColor = TextColor;
                    break;

                case DataGridView grid:
                    grid.BackgroundColor = SecondaryColor;
                    grid.ForeColor = TextColor;
                    grid.DefaultCellStyle.BackColor = SecondaryColor;
                    grid.DefaultCellStyle.ForeColor = TextColor;
                    grid.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
                    grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
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
        internal static void ApplyTheme(ApplicationTheme theme, Control.ControlCollection controls)
        {
            switch (theme)
            {
                case ApplicationTheme.SystemDefault:
                    SetSystemColors();
                    break;
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
                ApplyControlsSettings(controls, usePalette: true);
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

        public static void ApplyTheme(ApplicationTheme theme, IEnumerable<Control> controls)
        {
            if (controls == null)
                return;

            switch (theme)
            {
                case ApplicationTheme.SystemDefault:
                    SetSystemColors();
                    break;
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

            ApplyControlsSettings(controls, usePalette: true);
        }

        private static void SetSystemColors()
        {
            PrimaryColor = SystemColors.HotTrack;
            SecondaryColor = SystemColors.Control;
            AccentColor = SystemColors.Highlight;
            TextColor = SystemColors.ControlText;
            ErrorColor = SystemColors.ControlDark;
        }

        private static void SetLightTheme()
        {
            PrimaryColor = Color.FromArgb(0, 120, 215);
            SecondaryColor = Color.WhiteSmoke;
            AccentColor = Color.FromArgb(0, 153, 255);
            TextColor = Color.Black;
            ErrorColor = Color.FromArgb(255, 50, 50);
        }

        private static void SetDarkTheme()
        {
            PrimaryColor = Color.FromArgb(0, 120, 215);
            SecondaryColor = Color.FromArgb(45, 45, 48);
            AccentColor = Color.FromArgb(0, 153, 255);
            TextColor = Color.White;
            ErrorColor = Color.FromArgb(255, 50, 50);
        }

        private static void SetBlueTheme()
        {
            PrimaryColor = Color.FromArgb(0, 90, 158);
            SecondaryColor = Color.FromArgb(240, 240, 240);
            AccentColor = Color.FromArgb(0, 120, 215);
            TextColor = Color.Black;
            ErrorColor = Color.FromArgb(200, 0, 0);
        }

        private static void SetHighContrastTheme()
        {
            PrimaryColor = Color.Black;
            SecondaryColor = Color.White;
            AccentColor = Color.Yellow;
            TextColor = Color.Black;
            ErrorColor = Color.Red;
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
