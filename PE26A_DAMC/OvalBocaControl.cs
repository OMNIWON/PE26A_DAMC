using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    public partial class OvalBocaControl : UserControl
    {
        private Timer animationTimer;
        private bool growing = true;

        // ¡NUEVO!: Candado de seguridad para evitar que crezca al infinito
        private bool isAnimating = false;

        private Color _neonColor = Color.Fuchsia;
        public Color NeonColor { get => _neonColor; set { _neonColor = value; Invalidate(); } }

        public string TextBoca { get => txtBoca.Text; set => txtBoca.Text = value; }

        public TextBox txtBoca;

        public OvalBocaControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            txtBoca = new TextBox();
            txtBoca.BorderStyle = BorderStyle.None;
            txtBoca.BackColor = this.BackColor;
            txtBoca.ForeColor = Color.Cyan;
            txtBoca.Font = new Font("Consolas", 12, FontStyle.Bold);
            txtBoca.TextAlign = HorizontalAlignment.Center;

            this.Controls.Add(txtBoca);

            animationTimer = new Timer();
            animationTimer.Interval = 20;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            if (txtBoca != null) txtBoca.BackColor = this.BackColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle drawRect = new Rectangle(5, 5, this.Width - 10, this.Height - 10);

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(drawRect);
                this.Region = new Region(path);

                using (Pen glowPen = new Pen(Color.FromArgb(80, NeonColor), 10))
                {
                    g.DrawEllipse(glowPen, drawRect);
                }
                using (Pen mainPen = new Pen(NeonColor, 4))
                {
                    g.DrawEllipse(mainPen, drawRect);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (txtBoca != null)
            {
                txtBoca.Width = this.Width - 30;
                int centroX = (this.Width - txtBoca.Width) / 2;
                int centroY = (this.Height - txtBoca.Height) / 2;
                txtBoca.Location = new Point(centroX, centroY);
            }
        }

        // ==========================================
        // LA MAGIA DEL BOSTEZO BLINDADA
        // ==========================================
        private int originalWidth, originalHeight;
        private Point originalLocation;
        private double maxScale = 1.8;
        private double scaleStep = 0.03;
        private double currentScale = 1.0;

        public void StartBostezo(string message)
        {
            // ¡EL CANDADO! Si ya se está moviendo, ignoramos la orden.
            if (isAnimating) return;

            isAnimating = true; // Ponemos el candado
            this.TextBoca = message;

            originalWidth = this.Width;
            originalHeight = this.Height;
            originalLocation = this.Location;
            currentScale = 1.0;
            growing = true;

            txtBoca.Enabled = false;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (growing)
            {
                currentScale += scaleStep;
                if (currentScale >= maxScale) growing = false;
            }
            else
            {
                currentScale -= scaleStep;

                if (currentScale <= 1.0)
                {
                    // FIN DE LA ANIMACIÓN: Restauramos todo y quitamos el candado
                    currentScale = 1.0;
                    animationTimer.Stop();
                    this.TextBoca = "";

                    this.Width = originalWidth;
                    this.Height = originalHeight;
                    this.Location = originalLocation;

                    txtBoca.Enabled = true;
                    txtBoca.Focus();
                    isAnimating = false; // Quitamos el candado
                    return;
                }
            }

            int newWidth = (int)(originalWidth * currentScale);
            int newHeight = (int)(originalHeight * currentScale);

            int diffX = newWidth - originalWidth;
            int diffY = newHeight - originalHeight;

            this.Location = new Point(originalLocation.X - (diffX / 2), originalLocation.Y - (diffY / 2));
            this.Width = newWidth;
            this.Height = newHeight;

            float newFontSize = 12 * (float)currentScale;
            this.txtBoca.Font = new Font(this.txtBoca.Font.FontFamily, newFontSize, FontStyle.Bold);

            this.Invalidate();
        }

        // ==========================================
        // BOTÓN DE PÁNICO 
        // ==========================================
        public void ResetBoca()
        {
            if (isAnimating)
            {
                animationTimer.Stop();
                currentScale = 1.0;
                this.Width = originalWidth;
                this.Height = originalHeight;
                this.Location = originalLocation;
                this.TextBoca = "";
                txtBoca.Enabled = true;
                isAnimating = false;
                this.Invalidate();
            }
        }
    }
}