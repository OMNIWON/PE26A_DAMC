using System;
using System.Drawing;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    // ========================================================================
    // TEMA: MILITAR OSCURO — Batalla Naval
    // Paleta: Verde militar + Ámbar + Negro profundo
    // ========================================================================

    public static class ColorScheme
    {
        // ── Colores primarios ────────────────────────────────────────────────
        public static readonly Color Verde          = Color.FromArgb(0,   210,  90);   // Verde radar
        public static readonly Color VerdeOscuro    = Color.FromArgb(0,   120,  50);   // Verde botón
        public static readonly Color VerdePanel     = Color.FromArgb(10,  40,   20);   // Fondo panel
        public static readonly Color Ambar          = Color.FromArgb(255, 176,  0);    // Acento / alerta
        public static readonly Color Rojo           = Color.FromArgb(200,  40,  40);   // Peligro
        public static readonly Color Cyan           = Color.FromArgb(0,   210, 210);   // Info / radar J2

        // ── Fondos ───────────────────────────────────────────────────────────
        public static readonly Color FondoPrincipal = Color.FromArgb(8,   12,   8);    // Negro verdoso
        public static readonly Color FondoPanel     = Color.FromArgb(12,  22,  14);   // Panel base
        public static readonly Color FondoControl   = Color.FromArgb(18,  35,  20);   // Controles
        public static readonly Color FondoBorde     = Color.FromArgb(0,   160,  60);  // Borde activo

        // ── Textos ───────────────────────────────────────────────────────────
        public static readonly Color TextoPrincipal   = Color.FromArgb(0,   255, 100); // Verde brillante
        public static readonly Color TextoSecundario  = Color.FromArgb(120, 200, 140); // Verde suave
        public static readonly Color TextoDesact      = Color.FromArgb(60,  100,  70); // Deshabilitado
        public static readonly Color TextoTitulo      = Color.FromArgb(255, 176,   0); // Ámbar títulos
    }

    public static class FontScheme
    {
        // Fuente principal — Consolas da feeling terminal/militar
        public static readonly Font Titulo      = new Font("Consolas", 18, FontStyle.Bold);
        public static readonly Font Subtitulo   = new Font("Consolas", 13, FontStyle.Bold);
        public static readonly Font Boton       = new Font("Consolas", 10, FontStyle.Bold);
        public static readonly Font BotonSmall  = new Font("Consolas",  9, FontStyle.Bold);
        public static readonly Font Label       = new Font("Consolas",  9, FontStyle.Regular);
        public static readonly Font LabelBold   = new Font("Consolas",  9, FontStyle.Bold);
        public static readonly Font Estado      = new Font("Consolas", 10, FontStyle.Bold | FontStyle.Italic);
        public static readonly Font Mercado     = new Font("Consolas",  9, FontStyle.Italic);
    }

    public static class StyleUtils
    {
        // ── Botón primario (acción principal) ────────────────────────────────
        public static void BtnPrimario(Button b)
        {
            b.BackColor = ColorScheme.VerdeOscuro;
            b.ForeColor = ColorScheme.TextoPrincipal;
            b.Font      = FontScheme.Boton;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor        = ColorScheme.FondoBorde;
            b.FlatAppearance.BorderSize         = 1;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 160, 60);
            b.FlatAppearance.MouseDownBackColor = Color.FromArgb(0,  80, 30);
            b.Cursor    = Cursors.Hand;
        }

        // ── Botón peligro (rojo) ─────────────────────────────────────────────
        public static void BtnPeligro(Button b)
        {
            b.BackColor = Color.FromArgb(100, 10, 10);
            b.ForeColor = Color.FromArgb(255, 100, 100);
            b.Font      = FontScheme.Boton;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor        = ColorScheme.Rojo;
            b.FlatAppearance.BorderSize         = 1;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(140, 20, 20);
            b.Cursor    = Cursors.Hand;
        }

        // ── Botón acento (ámbar) ─────────────────────────────────────────────
        public static void BtnAccento(Button b)
        {
            b.BackColor = Color.FromArgb(80, 55, 0);
            b.ForeColor = ColorScheme.Ambar;
            b.Font      = FontScheme.Boton;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor        = ColorScheme.Ambar;
            b.FlatAppearance.BorderSize         = 1;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(110, 75, 0);
            b.Cursor    = Cursors.Hand;
        }

        // ── Panel base ───────────────────────────────────────────────────────
        public static void Panel(Panel p)
        {
            p.BackColor   = ColorScheme.FondoPanel;
            p.BorderStyle = BorderStyle.FixedSingle;
        }

        // ── Label título ─────────────────────────────────────────────────────
        public static void LabelTitulo(Label l)
        {
            l.ForeColor = ColorScheme.TextoTitulo;
            l.Font      = FontScheme.Subtitulo;
            l.BackColor = Color.Transparent;
        }

        // ── Label normal ─────────────────────────────────────────────────────
        public static void LabelNormal(Label l)
        {
            l.ForeColor = ColorScheme.TextoPrincipal;
            l.Font      = FontScheme.Label;
            l.BackColor = Color.Transparent;
        }

        // ── Label estado (barra inferior) ────────────────────────────────────
        public static void LabelEstado(Label l)
        {
            l.ForeColor = ColorScheme.Ambar;
            l.Font      = FontScheme.Estado;
            l.BackColor = Color.FromArgb(20, 20, 0);
        }

        // ── ComboBox ─────────────────────────────────────────────────────────
        public static void Combo(ComboBox c)
        {
            c.BackColor   = ColorScheme.FondoControl;
            c.ForeColor   = ColorScheme.TextoPrincipal;
            c.Font        = FontScheme.BotonSmall;
            c.FlatStyle   = FlatStyle.Flat;
        }

        // ── DataGridView (tablero) ───────────────────────────────────────────
        public static void Tablero(DataGridView dgv)
        {
            dgv.BackgroundColor = ColorScheme.FondoPrincipal;
            dgv.GridColor       = Color.FromArgb(0, 80, 30);

            dgv.DefaultCellStyle.BackColor          = Color.FromArgb(5, 15, 8);
            dgv.DefaultCellStyle.ForeColor          = ColorScheme.TextoPrincipal;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 60, 20);
            dgv.DefaultCellStyle.SelectionForeColor = ColorScheme.TextoPrincipal;
            dgv.DefaultCellStyle.Font               = FontScheme.BotonSmall;

            dgv.ColumnHeadersDefaultCellStyle.BackColor  = Color.FromArgb(0, 60, 20);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor  = ColorScheme.Ambar;
            dgv.ColumnHeadersDefaultCellStyle.Font       = FontScheme.LabelBold;

            dgv.RowHeadersDefaultCellStyle.BackColor     = Color.FromArgb(0, 60, 20);
            dgv.RowHeadersDefaultCellStyle.ForeColor     = ColorScheme.Ambar;
            dgv.RowHeadersDefaultCellStyle.Font          = FontScheme.LabelBold;

            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersBorderStyle     = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersBorderStyle  = DataGridViewHeaderBorderStyle.Single;
        }
    }
}
