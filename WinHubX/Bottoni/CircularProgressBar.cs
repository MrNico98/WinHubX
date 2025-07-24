using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace WinHubX.Bottoni
{
    public class CircularProgressBar : Control
    {
        private int _minimum = 0;
        private int _maximum = 100;
        private int _value = 30;

        [Category("Appearance")]
        public int Minimum
        {
            get { return _minimum; }
            set
            {
                if (value >= _maximum)
                    throw new ArgumentException("Minimum must be less than Maximum");
                _minimum = value;
                if (_value < _minimum)
                    _value = _minimum;
                Refresh();
            }
        }

        [Category("Appearance")]
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                if (value <= _minimum)
                    throw new ArgumentException("Maximum must be greater than Minimum");
                _maximum = value;
                if (_value > _maximum)
                    _value = _maximum;
                Refresh();
            }
        }

        [Category("Appearance")]
        public int Value
        {
            get { return _value; }
            set
            {
                if (value < _minimum || value > _maximum)
                    throw new ArgumentException("Value must be between Minimum and Maximum");
                _value = value;
                Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            int margin = 20;
            int circleSize = Math.Min(Width, Height) - margin * 2;
            Rectangle rect = new Rectangle((Width - circleSize) / 2, (Height - circleSize) / 2, circleSize, circleSize);
            float progressAngle = 360.0f * (_value - _minimum) / (_maximum - _minimum);
            using (LinearGradientBrush brushBackground = new LinearGradientBrush(rect, Color.FromArgb(230, 230, 230), Color.FromArgb(200, 200, 200), 45f))
            {
                g.FillEllipse(brushBackground, rect);
            }
            using (LinearGradientBrush brushProgress = new LinearGradientBrush(rect, Color.FromArgb(0, 150, 136), Color.FromArgb(0, 188, 212), 45f))
            using (Pen penProgress = new Pen(brushProgress, 12))
            {
                g.DrawArc(penProgress, rect, -90, progressAngle);
            }
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (PathGradientBrush brushShadow = new PathGradientBrush(path))
                {
                    brushShadow.CenterColor = Color.FromArgb(50, Color.Black);
                    brushShadow.SurroundColors = new Color[] { Color.Transparent };
                    g.FillEllipse(brushShadow, rect);
                }
            }
            string text = $"{Value}%";
            using (Font font = new Font("Segoe UI", 20f, FontStyle.Bold))
            using (Brush brushText = new SolidBrush(Color.FromArgb(0, 150, 136)))
            {
                SizeF textSize = g.MeasureString(text, font);
                PointF textLocation = new PointF(Width / 2 - textSize.Width / 2,
                                                 Height / 2 - textSize.Height / 2);
                g.DrawString(text, font, brushText, textLocation);
            }
        }

        public CircularProgressBar()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Size = new Size(150, 150);
        }
    }
}