using HartUI.Controls;
namespace WinHubX.Impostazioni
{
    public static class ThemeManager
    {
        public static bool IsDarkTheme { get; private set; } = false;

        private static ThemeConfig Config => ThemeConfig.Load();

        public static void SetTheme(bool darkTheme, bool? manual = null)
        {
            IsDarkTheme = darkTheme;

            var config = Config;
            config.DarkTheme = darkTheme;

            // ✅ Solo se manual è specificato esplicitamente, modifica il flag
            if (manual.HasValue)
            {
                config.ThemeManuallySet = manual.Value;
            }
            // ❌ Altrimenti mantieni il valore esistente

            config.Save();

            // Applica a tutti i form aperti
            foreach (Form form in Application.OpenForms)
            {
                ApplyThemeToControl(form, darkTheme);
            }
        }

        /// <summary>
        /// Inverte il tema (Dark/Light) e lo salva come impostazione manuale.
        /// </summary>
        public static void ToggleTheme()
        {
            SetTheme(!IsDarkTheme, manual: true);
        }

        /// <summary>
        /// Reimposta per seguire automaticamente il tema di Windows.
        /// </summary>
        public static void UseSystemTheme()
        {
            var config = Config;
            config.ThemeManuallySet = false;
            config.DarkTheme = Program.IsSystemInDarkMode();
            config.Save();

            SetTheme(config.DarkTheme);
        }

        public static void ApplyThemeToControl(Control control, bool darkTheme)
        {
            if (control.Name == "pnlNav")
                return;
            if (control is Form formMain && formMain.Name == "Form1")
            {
                formMain.BackColor = darkTheme ? UiPalette.DarkWindow : UiPalette.LightWindow;
                foreach (Control child in formMain.Controls)
                    ApplyThemeToControl(child, darkTheme);

                return; 
            }
            Color backColor = darkTheme ? UiPalette.DarkSurface : UiPalette.LightSurface;
            Color foreColor = darkTheme ? UiPalette.DarkText : UiPalette.LightText;

            string parentPanelName = GetSpecialParentPanelName(control);
            if (parentPanelName == "panel1" || parentPanelName == "panel2" || parentPanelName == "panel3" || parentPanelName == "tableLayoutPanel1")
            {
                backColor = darkTheme ? UiPalette.DarkWindow : UiPalette.LightWindow;
            }
            else if (parentPanelName == "PnlFormLoader")
            {
                backColor = darkTheme ? UiPalette.DarkSurface : UiPalette.LightSurface;
            }

            Color arancione = Color.Coral;
            Color sostitutoTemaChiaro = Color.DarkOrange;
            Color bluSpeciale = Color.FromArgb(0, 126, 249);
            Color verdeSpeciale = Color.FromArgb(46, 125, 60);
            Color rossoSpeciale = Color.FromArgb(192, 0, 0);

            // 🔸 Helper per verificare se un colore è tra quelli "speciali" che non cambiano
            bool IsSpecialColor(Color c) =>
                AreColorsSimilar(c, arancione) ||
                AreColorsSimilar(c, sostitutoTemaChiaro) ||
                AreColorsSimilar(c, bluSpeciale) ||
                AreColorsSimilar(c, rossoSpeciale) ||
                AreColorsSimilar(c, verdeSpeciale);

            if (control is Form form && form.Name != "Form1")
            {
                if (!IsSpecialColor(form.BackColor))
                    form.BackColor = darkTheme ? UiPalette.DarkSurface : UiPalette.LightSurface;

                if (!IsSpecialColor(form.ForeColor))
                    form.ForeColor = foreColor;
            }
            else
            {
                if (!IsSpecialColor(control.BackColor))
                    control.BackColor = backColor;

                if (control is not PictureBox && !IsSpecialColor(control.ForeColor))
                    control.ForeColor = foreColor;
                if (control is cuiButton cuoreBtn)
                {
                    bool isVerdi = cuoreBtn.Name.Contains("Verdi");
                    bool isBianco = cuoreBtn.Name.Contains("Bianco");
                    Color themeBack = darkTheme ? UiPalette.DarkSurface : UiPalette.LightSurface;
                    Color themeFore = darkTheme ? UiPalette.DarkText : UiPalette.LightText;

                    // BackColor e NormalBackground seguono sempre il tema
                    cuoreBtn.BackColor = themeBack;
                    cuoreBtn.NormalBackground = themeBack;
                    if (isVerdi)
                    {
                        // Regole speciali per bottoni "Verdi"
                        if (darkTheme)
                        {
                            cuoreBtn.NormalForeColor = Color.White;
                            cuoreBtn.NormalImageTint = Color.White;
                            cuoreBtn.HoverForeColor = Color.White;
                            cuoreBtn.HoverImageTint = Color.White;
                            cuoreBtn.PressedForeColor = Color.Black;
                            cuoreBtn.PressedImageTint = Color.Black;
                        }
                        else
                        {
                            cuoreBtn.NormalForeColor = Color.Black;
                            cuoreBtn.NormalImageTint = Color.Black;
                            cuoreBtn.HoverForeColor = Color.Black;
                            cuoreBtn.HoverImageTint = Color.Black;
                            cuoreBtn.PressedForeColor = Color.White;
                            cuoreBtn.PressedImageTint = Color.White;
                        }
                    }
                    else if (isBianco)
                    {
                        // Regole speciali per bottoni "Bianco"
                        if (darkTheme)
                        {
                            cuoreBtn.NormalBackground = UiPalette.DarkSurface;
                            cuoreBtn.NormalForeColor = UiPalette.DarkText;
                            cuoreBtn.NormalImageTint = UiPalette.DarkText;
                            cuoreBtn.NormalOutline = UiPalette.DarkBorder;

                            cuoreBtn.HoverBackground = Color.White;
                            cuoreBtn.HoverForeColor = Color.Black;
                            cuoreBtn.HoverImageTint = Color.Black;
                            cuoreBtn.HoverOutline = Color.White;

                            cuoreBtn.PressedBackground = UiPalette.DarkSurface;
                            cuoreBtn.PressedForeColor = UiPalette.DarkText;
                            cuoreBtn.PressedImageTint = UiPalette.DarkText;
                            cuoreBtn.PressedOutline = UiPalette.DarkBorder;

                            cuoreBtn.CheckedBackground = UiPalette.DarkSurface;
                            cuoreBtn.CheckedForeColor = UiPalette.DarkText;
                            cuoreBtn.CheckedImageTint = UiPalette.DarkText;
                            cuoreBtn.CheckedOutline = UiPalette.DarkBorder;
                        }
                        else
                        {
                            cuoreBtn.NormalBackground = Color.White;
                            cuoreBtn.NormalForeColor = Color.Black;
                            cuoreBtn.NormalImageTint = Color.Black;
                            cuoreBtn.NormalOutline = Color.Black;

                            cuoreBtn.HoverBackground = Color.Black;
                            cuoreBtn.HoverForeColor = Color.White;
                            cuoreBtn.HoverImageTint = Color.White;
                            cuoreBtn.HoverOutline = Color.Black;

                            cuoreBtn.PressedBackground = Color.White;
                            cuoreBtn.PressedForeColor = Color.Black;
                            cuoreBtn.PressedImageTint = Color.Black;
                            cuoreBtn.PressedOutline = Color.Black;

                            cuoreBtn.CheckedBackground = Color.White;
                            cuoreBtn.CheckedForeColor = Color.Black;
                            cuoreBtn.CheckedImageTint = Color.Black;
                            cuoreBtn.CheckedOutline = Color.Black;
                        }
                    }
                    else
                    {
                        // Comportamento standard
                        if (!cuoreBtn.Name.Contains("Princi"))
                        {
                            cuoreBtn.NormalImageTint = themeFore;
                            cuoreBtn.CheckedImageTint = themeFore;
                            cuoreBtn.HoverImageTint = themeFore;
                            cuoreBtn.PressedImageTint = themeFore;
                        }
                        else
                        {
                            // Comportamento specifico per Princi
                            cuoreBtn.HoverBackground = darkTheme ? Color.White : Color.Gray; // Bianco in dark, grigio in light
                        }

                        if (AreColorsSimilar(cuoreBtn.NormalOutline, Color.White))
                            cuoreBtn.NormalOutline = themeFore;
                        if (AreColorsSimilar(cuoreBtn.HoverOutline, Color.White))
                            cuoreBtn.HoverOutline = themeFore;
                        if (AreColorsSimilar(cuoreBtn.CheckedOutline, Color.White))
                            cuoreBtn.CheckedOutline = themeFore;
                    }
                    if (cuoreBtn.Name.EndsWith("Disattivo", StringComparison.OrdinalIgnoreCase))
                    {
                        Color grigio = darkTheme ? UiPalette.DarkMutedText : UiPalette.LightMutedText;

                        cuoreBtn.ForeColor = grigio;
                        cuoreBtn.NormalForeColor = grigio;
                        cuoreBtn.NormalImageTint = grigio;
                        cuoreBtn.NormalOutline = grigio;

                        // Mantieni anche gli stati Hover/Pressed/Checked nel grigio,
                        // così non cambiano col tema o durante l’interazione
                        cuoreBtn.HoverForeColor = grigio;
                        cuoreBtn.HoverImageTint = grigio;
                        cuoreBtn.HoverOutline = grigio;

                        cuoreBtn.PressedForeColor = grigio;
                        cuoreBtn.PressedImageTint = grigio;
                        cuoreBtn.PressedOutline = grigio;

                        cuoreBtn.CheckedForeColor = grigio;
                        cuoreBtn.CheckedImageTint = grigio;
                        cuoreBtn.CheckedOutline = grigio;

                        return; // 🔥 Blocca ulteriori modifiche di stile
                    }
                }

                if (control is cuiPanel panel)
                {
                    panel.PanelColor = darkTheme ? UiPalette.DarkSurface : UiPalette.LightSurface;
                    panel.BackColor = darkTheme ? UiPalette.DarkSurface : UiPalette.LightSurface;
                    panel.PanelOutlineColor = darkTheme ? UiPalette.DarkBorder : UiPalette.LightBorder;
                }
                if (control is cuiPictureBox pic)
                {
                    pic.ImageTint = darkTheme ? Color.White : Color.Black;
                }
                if (control is cuiFileDropper dropper)
                {
                    bool isWhiteVersion = dropper.Name.Contains("White", StringComparison.OrdinalIgnoreCase);

                    if (isWhiteVersion)
                    {
                        dropper.NormalUploadForeColor = darkTheme ? Color.White : Color.Black;
                    }
                }
                if (control is RadioButton rb)
                {
                    rb.BackColor = backColor;
                    rb.ForeColor = foreColor;
                }
            }
            foreach (Control child in control.Controls)
            {
                if (child.BackColor == Color.Transparent)
                    child.BackColor = backColor;

                ApplyThemeToControl(child, darkTheme);
            }
        }

        private static string GetSpecialParentPanelName(Control control)
        {
            Control? current = control;
            while (current != null)
            {
                if (current.Name == "panel1" || current.Name == "panel2" || current.Name == "panel3" || current.Name == "PnlFormLoader" || current.Name == "tableLayoutPanel1")
                    return current.Name;
                current = current.Parent;
            }
            return string.Empty;
        }

        private static bool AreColorsSimilar(Color c1, Color c2, int tolerance = 5)
        {
            return Math.Abs(c1.R - c2.R) <= tolerance &&
                   Math.Abs(c1.G - c2.G) <= tolerance &&
                   Math.Abs(c1.B - c2.B) <= tolerance;
        }

        public static Color GetBackColor(bool darkTheme)
        {
            return darkTheme ? UiPalette.DarkSurface : UiPalette.LightSurface;
        }

        public static Color GetForeColor(bool darkTheme)
        {
            return darkTheme ? UiPalette.DarkText : UiPalette.LightText;
        }
    }
}
