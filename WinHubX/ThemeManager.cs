namespace WinHubX
{
    public static class ThemeManager
    {
        public static bool IsDarkTheme { get; private set; } = false;
        public static void SetTheme(bool darkTheme)
        {
            IsDarkTheme = darkTheme;
            foreach (Form form in Application.OpenForms)
            {
                ApplyThemeToControl(form, darkTheme);
            }
        }

        public static void ApplyThemeToControl(Control control, bool darkTheme)
        {
            Color backColor = darkTheme ? Color.FromArgb(32, 32, 32) : Color.White;
            Color foreColor = darkTheme ? Color.White : Color.Black;
            string parentPanelName = GetSpecialParentPanelName(control);
            if (parentPanelName == "panel1" || parentPanelName == "panel2" || parentPanelName == "panel3" || parentPanelName == "tableLayoutPanel1")
            {
                backColor = darkTheme ? Color.FromArgb(64, 60, 59) : Color.FromArgb(245, 245, 245);
            }
            else if (parentPanelName == "PnlFormLoader")
            {
                backColor = darkTheme ? Color.FromArgb(37, 38, 39) : Color.White;
            }
            Color arancione = Color.Coral;
            Color sostitutoTemaChiaro = Color.DarkOrange;
            if (control is Form form && form.Name != "Form1")
            {
                if (AreColorsSimilar(form.BackColor, arancione) || AreColorsSimilar(form.BackColor, sostitutoTemaChiaro))
                {
                    form.BackColor = darkTheme ? arancione : sostitutoTemaChiaro;
                }
                else
                {
                    form.BackColor = darkTheme ? Color.FromArgb(37, 38, 39) : Color.White;
                }
                if (AreColorsSimilar(form.ForeColor, arancione) || AreColorsSimilar(form.ForeColor, sostitutoTemaChiaro))
                {
                    form.ForeColor = darkTheme ? arancione : sostitutoTemaChiaro;
                }
                else
                {
                    form.ForeColor = foreColor;
                }
            }
            else
            {
                if (AreColorsSimilar(control.BackColor, arancione) || AreColorsSimilar(control.BackColor, sostitutoTemaChiaro))
                {
                    control.BackColor = darkTheme ? arancione : sostitutoTemaChiaro;
                }
                else
                {
                    control.BackColor = backColor;
                }
                if (!(control is PictureBox))
                {
                    if (AreColorsSimilar(control.ForeColor, arancione) || AreColorsSimilar(control.ForeColor, sostitutoTemaChiaro))
                    {
                        control.ForeColor = darkTheme ? arancione : sostitutoTemaChiaro;
                    }
                    else
                    {
                        control.ForeColor = foreColor;
                    }
                }
            }
            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child, darkTheme);
            }
        }

        private static string GetSpecialParentPanelName(Control control)
        {
            Control current = control;
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
            return darkTheme ? Color.FromArgb(37, 38, 39) : Color.White;
        }

        public static Color GetForeColor(bool darkTheme)
        {
            return darkTheme ? Color.White : Color.Black;
        }
    }
}

