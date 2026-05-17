using System;
using System.Drawing;
using System.Windows.Forms;

namespace UiRuler
{
    internal enum SnapSide
    {
        None,
        Above,
        Below,
        Left,
        Right
    }

    internal class PopupRulerOverlayForm : Form
    {
        private static readonly Color KeyColor = Color.Magenta;
        private Rectangle _popupRect;
        private Point _mouseScreen;
        private bool _visible;
        private bool _snapPreviewVisible;
        private Rectangle _snapMovingRect;
        private Rectangle _snapNeighborRect;
        private Rectangle _snapTargetRect;
        private SnapSide _snapSide;

        public int GridSpacing { get; set; } = 10;
        public bool ShowGuideLabels { get; set; }
        public bool ShowGrid { get; set; } = true;
        public bool ShowCrosshair { get; set; } = true;

        public PopupRulerOverlayForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = KeyColor;
            TransparencyKey = KeyColor;
            DoubleBuffered = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                const int WS_EX_TOOLWINDOW = 0x80;
                const int WS_EX_NOACTIVATE = 0x08000000;

                var cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                cp.ExStyle |= WS_EX_TOOLWINDOW;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        protected override bool ShowWithoutActivation => true;

        public void UpdatePopup(Rectangle popupRect, Point mouseScreen)
        {
            _popupRect = popupRect;
            _mouseScreen = mouseScreen;
            _visible = true;
            _snapPreviewVisible = false;

            Bounds = popupRect;
            if (!Visible)
                Show();

            Invalidate();
        }

        public void UpdateSnapPreview(Rectangle movingRect, Rectangle neighborRect, Rectangle targetRect, SnapSide side)
        {
            if (!IsReasonableSnapRect(movingRect) || !IsReasonableSnapRect(neighborRect) || !IsReasonableSnapRect(targetRect))
            {
                HideOverlay();
                return;
            }

            var bounds = Rectangle.Union(Rectangle.Union(movingRect, neighborRect), targetRect);
            bounds.Inflate(8, 8);
            if (bounds.Width <= 0 || bounds.Height <= 0 || bounds.Width > 2200 || bounds.Height > 1400)
            {
                HideOverlay();
                return;
            }

            _snapMovingRect = movingRect;
            _snapNeighborRect = neighborRect;
            _snapTargetRect = targetRect;
            _snapSide = side;
            _visible = false;
            _snapPreviewVisible = true;

            Bounds = bounds;
            if (!Visible)
                Show();

            Invalidate();
        }

        public void HideOverlay()
        {
            _visible = false;
            _snapPreviewVisible = false;
            Hide();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if ((!_visible && !_snapPreviewVisible) || Width <= 0 || Height <= 0)
                return;

            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            if (_snapPreviewVisible)
            {
                DrawSnapPreview(g);
                return;
            }

            using var borderPen = new Pen(Color.Lime, 2);
            using var gridPen = new Pen(Color.FromArgb(180, Color.Cyan), 1);
            using var textBrush = new SolidBrush(Color.White);
            using var textBack = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
            using var crossPen = new Pen(Color.Yellow, 1);
            using var font = new Font("Consolas", 10f, FontStyle.Bold);

            g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

            if (ShowGrid)
            {
                int spacing = Math.Max(5, GridSpacing);
                for (int y = spacing; y < Height; y += spacing)
                {
                    g.DrawLine(gridPen, 0, y, Width, y);
                    g.DrawString(y.ToString(), font, textBrush, 4, y - font.Height);
                }

                for (int x = 50; x < Width; x += 50)
                {
                    g.DrawLine(gridPen, x, 0, x, Height);
                }
            }

            var relX = _mouseScreen.X - _popupRect.Left;
            var relY = _mouseScreen.Y - _popupRect.Top;

            if (ShowCrosshair && relX >= 0 && relX < Width && relY >= 0 && relY < Height)
            {
                g.DrawLine(crossPen, relX, 0, relX, Height);
                g.DrawLine(crossPen, 0, relY, Width, relY);
                g.FillEllipse(Brushes.Yellow, relX - 3, relY - 3, 6, 6);
            }

            if (ShowGuideLabels)
            {
                var info = $"Popup {Width}x{Height}   Mouse {relX},{relY}";
                var size = g.MeasureString(info, font);
                g.FillRectangle(textBack, 6, 6, size.Width + 10, size.Height + 6);
                g.DrawString(info, font, textBrush, 11, 9);
            }
        }

        private void DrawSnapPreview(Graphics g)
        {
            g.Clear(KeyColor);

            var moving = ToLocal(_snapMovingRect);
            var neighbor = ToLocal(_snapNeighborRect);
            var target = ToLocal(_snapTargetRect);

            using var movingPen = new Pen(Color.White, 2);
            using var neighborPen = new Pen(Color.Lime, 3);
            using var targetPen = new Pen(Color.Lime, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            using var directionPen = new Pen(Color.Red, 4);

            g.DrawRectangle(targetPen, target);
            g.DrawRectangle(movingPen, moving);
            g.DrawRectangle(neighborPen, neighbor);
            DrawSnapDirectionLine(g, directionPen, target, _snapSide);
        }

        private static bool IsReasonableSnapRect(Rectangle rect)
        {
            return rect.Width > 0
                && rect.Height > 0
                && rect.Width <= 500
                && rect.Height <= 500
                && Math.Abs(rect.Left) < 20000
                && Math.Abs(rect.Top) < 20000;
        }

        private Rectangle ToLocal(Rectangle rect)
        {
            return new Rectangle(rect.Left - Left, rect.Top - Top, rect.Width, rect.Height);
        }

        private static void DrawSnapDirectionLine(Graphics g, Pen pen, Rectangle rect, SnapSide side)
        {
            switch (side)
            {
                case SnapSide.Above:
                    g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                    break;
                case SnapSide.Below:
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                    break;
                case SnapSide.Left:
                    g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                    break;
                case SnapSide.Right:
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                    break;
            }
        }
    }
}
