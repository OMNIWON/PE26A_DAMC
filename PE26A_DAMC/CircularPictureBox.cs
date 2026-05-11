using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    // DEFINITIVA SOLUCIÓN PARA BOTONES REDONDOS:
    // Creamos una nueva clase que hereda de PictureBox para que sea redondo.
    public class CircularPictureBox : PictureBox
    {
        // Propiedades para personalizar el borde
        public Color BorderColor { get; set; } = Color.Black; // nego
        public int BorderSize { get; set; } = 0;

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            // Usamos un GraphicsPath para definir un círculo perfecto
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, ClientSize.Width - 1, ClientSize.Height - 1);

                // Aplicamos este círculo como la REGION del control, lo que
                // "recorta" todo lo que esté afuera.
                this.Region = new Region(path);

                // Opcional: Dibujamos un borde suave (anti-alias)
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(BorderColor, BorderSize))
                {
                    pen.Alignment = PenAlignment.Inset;
                    pe.Graphics.DrawPath(pen, path);
                }
            }
        }
    }
}