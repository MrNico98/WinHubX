using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace WinHubX.Bottoni
{
    public class BottoniSwap : CheckBox
    {
        private Color onBackColor = Color.MediumSlateBlue;
        private Color onToggleColor = Color.WhiteSmoke;
        private Color offBackColor = Color.Gray;
        private Color offToggleColor = Color.Gainsboro;
        private bool solidStyle = true;
        private int toggleSize = 20;
        private int borderSize = 2;

        public BottoniSwap()
        {
            MinimumSize = new Size(45, toggleSize + borderSize * 2);
        }

        [Category("RJ Code Advance")]
        public Color OnBackColor
        {
            get => onBackColor;
            set { onBackColor = value; Invalidate(); }
        }

        [Category("RJ Code Advance")]
        public Color OnToggleColor
        {
            get => onToggleColor;
            set { onToggleColor = value; Invalidate(); }
        }

        [Category("RJ Code Advance")]
        public Color OffBackColor
        {
            get => offBackColor;
            set { offBackColor = value; Invalidate(); }
        }

        [Category("RJ Code Advance")]
        public Color OffToggleColor
        {
            get => offToggleColor;
            set { offToggleColor = value; Invalidate(); }
        }

        [Browsable(false)]
        public override string Text
        {
            get => base.Text;
            set { }
        }

        [Category("RJ Code Advance")]
        [DefaultValue(true)]
        public bool SolidStyle
        {
            get => solidStyle;
            set { solidStyle = value; Invalidate(); }
        }

        [Category("RJ Code Advance")]
        [DefaultValue(20)]
        public int ToggleSize
        {
            get => toggleSize;
            set
            {
                toggleSize = value;
                MinimumSize = new Size(45, toggleSize + borderSize * 2);
                Invalidate();
            }
        }

        [Category("RJ Code Advance")]
        [DefaultValue(2)]
        public int BorderSize
        {
            get => borderSize;
            set
            {
                borderSize = value;
                MinimumSize = new Size(45, toggleSize + borderSize * 2);
                Invalidate();
            }
        }

        private GraphicsPath GetFigurePath()
        {
            int arcSize = Height - 1;
            Rectangle leftArc = new Rectangle(0, 0, arcSize, arcSize);
            Rectangle rightArc = new Rectangle(Width - arcSize - 2, 0, arcSize, arcSize);

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();

            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.Clear(Parent.BackColor);
            int togglePosition = Checked ? Width - Height + 1 : 2;
            using (GraphicsPath path = GetFigurePath())
            {
                if (solidStyle)
                    pevent.Graphics.FillPath(new SolidBrush(Checked ? onBackColor : offBackColor), path);
                else
                    pevent.Graphics.DrawPath(new Pen(Checked ? onBackColor : offBackColor, borderSize), path);
            }
            pevent.Graphics.FillEllipse(new SolidBrush(Checked ? onToggleColor : offToggleColor),
                new Rectangle(togglePosition, (Height - toggleSize) / 2, toggleSize, toggleSize));
        }
    }
}
