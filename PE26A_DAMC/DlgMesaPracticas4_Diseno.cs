// ============================================================================
// DlgMesaPracticas4_Diseno.cs  —  v2 FIXED
//
// INSTRUCCIONES:
//   1. Reemplaza el archivo DlgMesaPracticas4_Diseno.cs anterior con este.
//   2. En DlgMesaPracticas4_Load agrega AL FINAL:
//         AplicarDisenoVisual();
//   3. Asegurate de que ColorScheme.cs este en el proyecto.
// ============================================================================

using System.Drawing;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    public partial class DlgMesaPracticas4 : Form
    {
        private void AplicarDisenoVisual()
        {
            // ── 1. FORM BASE ─────────────────────────────────────────────────
            this.BackColor = ColorScheme.FondoPrincipal;
            this.ForeColor = ColorScheme.TextoPrincipal;
            this.Font = FontScheme.Label;
            this.BackgroundImage = null;   // elimina imagen de fondo del designer

            // ── 2. PANELES (fondo + borde verde en todos) ─────────────────────
            AplicarFondoPanel(pnlInicio);
            AplicarFondoPanel(pnlMenuModos);
            AplicarFondoPanel(pnlDificultad);
            AplicarFondoPanel(pnlJuego);

            // ── 3. TABLEROS ───────────────────────────────────────────────────
            StyleUtils.Tablero(dgvJugador1);
            StyleUtils.Tablero(dgvJugador2);

            // ── 4. pnlInicio ──────────────────────────────────────────────────
            StyleUtils.BtnPrimario(btnIniciarJuego);
            btnIniciarJuego.Text = "INICIAR MISION";
            btnIniciarJuego.Size = new Size(180, 40);

            StyleUtils.BtnPeligro(btnSalirJuego);
            btnSalirJuego.Text = "ABANDONAR";
            btnSalirJuego.Size = new Size(130, 35);

            // ── 5. pnlMenuModos ───────────────────────────────────────────────
            StyleUtils.BtnPrimario(btnModo1v1);
            btnModo1v1.Text = "JUGADOR VS JUGADOR";
            btnModo1v1.Size = new Size(240, 55);

            StyleUtils.BtnAccento(btnModoPC);
            btnModoPC.Text = "JUGADOR VS PC";
            btnModoPC.Size = new Size(240, 55);

            // ── 6. pnlDificultad ──────────────────────────────────────────────
            StyleUtils.BtnPrimario(btnFacil);
            btnFacil.Text = "RECLUTA  (FACIL)";
            btnFacil.Size = new Size(220, 50);

            StyleUtils.BtnPeligro(btnExperto);
            btnExperto.Text = "ALMIRANTE (EXPERTO)";
            btnExperto.Size = new Size(220, 50);

            StyleUtils.BtnPeligro(btnVolverMenu);
            btnVolverMenu.Text = "< VOLVER";
            btnVolverMenu.Size = new Size(110, 35);

            // ── 7. pnlJuego — botones con tamanio fijo ────────────────────────
            StyleUtils.BtnPrimario(btnAutoJ1);
            btnAutoJ1.Text = "AUTO J1";
            btnAutoJ1.Size = new Size(140, 32);

            StyleUtils.BtnPrimario(btnAutoJ2);
            btnAutoJ2.Text = "AUTO J2";
            btnAutoJ2.Size = new Size(140, 32);

            StyleUtils.BtnAccento(btnRotar);
            btnRotar.Text = "GIRAR";
            btnRotar.Size = new Size(140, 32);

            StyleUtils.BtnPeligro(btnReacomodar);
            btnReacomodar.Text = "REACOMODAR";
            btnReacomodar.Size = new Size(140, 32);

            StyleUtils.BtnPrimario(btnConfirmar);
            btnConfirmar.Text = "CONFIRMAR";
            btnConfirmar.Size = new Size(140, 32);

            StyleUtils.BtnPeligro(btnReiniciar);
            btnReiniciar.Text = "REINICIAR";
            btnReiniciar.Size = new Size(140, 32);

            // ── 8. REPOSICIONAR controles columna derecha ─────────────────────
            //   Columna derecha: empieza en X=960
            //   Si tu form es mas angosto, baja este numero (ej. 880)
            int xD = 960;

            // Labels de estado de barcos — esquina superior derecha
            lblPortaaviones.Location = new Point(xD, 10);
            lblPortaaviones.Size = new Size(185, 20);
            StyleUtils.LabelNormal(lblPortaaviones);

            lblAcorazado.Location = new Point(xD, 32);
            lblAcorazado.Size = new Size(185, 20);
            StyleUtils.LabelNormal(lblAcorazado);

            lblSubmarinos.Location = new Point(xD, 54);
            lblSubmarinos.Size = new Size(185, 20);
            StyleUtils.LabelNormal(lblSubmarinos);

            lblPatrulla.Location = new Point(xD, 76);
            lblPatrulla.Size = new Size(185, 20);
            StyleUtils.LabelNormal(lblPatrulla);

            // ComboBox de seleccion de barcos
            if (cmbSeleccionBarco != null)
            {
                cmbSeleccionBarco.Location = new Point(xD, 105);
                cmbSeleccionBarco.Size = new Size(180, 28);
                StyleUtils.Combo(cmbSeleccionBarco);
            }

            // Botones de colocacion apilados verticalmente
            int yBtn = 142;
            btnAutoJ1.Location = new Point(xD, yBtn); yBtn += 40;
            btnAutoJ2.Location = new Point(xD, yBtn); yBtn += 40;
            btnRotar.Location = new Point(xD, yBtn); yBtn += 40;
            btnReacomodar.Location = new Point(xD, yBtn); yBtn += 40;
            btnConfirmar.Location = new Point(xD, yBtn);

            // REINICIAR — esquina inferior izquierda
            btnReiniciar.Location = new Point(15, 520);
            btnReiniciar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            // ── 9. LABEL ESTADO ───────────────────────────────────────────────
            StyleUtils.LabelEstado(lblEstado);
            lblEstado.Location = new Point(10, 10);
            lblEstado.Size = new Size(430, 24);
            lblEstado.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            // ── 10. CONTROLES DINAMICOS (creados por codigo) ──────────────────

            // pnlMercado — franja horizontal arriba de los tableros
            if (pnlMercado != null)
            {
                pnlMercado.Location = new Point(20, 115);
                pnlMercado.Size = new Size(920, 95);
                pnlMercado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }

            // cmbSelectorArmas — bajo el pnlMercado, columna derecha
            if (cmbSelectorArmas != null)
            {
                cmbSelectorArmas.Location = new Point(xD, 215);
                cmbSelectorArmas.Size = new Size(180, 28);
                StyleUtils.Combo(cmbSelectorArmas);
            }

            // lblInventario — bajo los tableros
            if (lblInventario != null)
            {
                lblInventario.Location = new Point(20, 480);
                lblInventario.Size = new Size(880, 22);
                lblInventario.ForeColor = ColorScheme.Ambar;
                lblInventario.Font = FontScheme.Mercado;
                lblInventario.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }

            // pnlRadar — columna derecha bajo cmbSelectorArmas
            if (pnlRadar != null)
            {
                pnlRadar.Location = new Point(xD, 250);
                pnlRadar.BackColor = Color.FromArgb(10, 20, 10);
            }

            // btnManiobraEvasion — bajo el radar
            if (btnManiobraEvasion != null)
            {
                StyleUtils.BtnPeligro(btnManiobraEvasion);
                btnManiobraEvasion.Location = new Point(xD, 480);
                btnManiobraEvasion.Size = new Size(165, 30);
                btnManiobraEvasion.Text = "MANIOBRA EVASION";
            }
        }

        // =====================================================================
        // HELPER — fondo oscuro + borde verde en panel y todos sus hijos
        // =====================================================================
        private void AplicarFondoPanel(Panel p)
        {
            if (p == null) return;

            p.BackColor = ColorScheme.FondoPanel;
            p.BorderStyle = BorderStyle.None;
            p.BackgroundImage = null;

            // Borde verde dibujado manualmente (no depende de BorderStyle)
            p.Paint += (s, e) =>
            {
                var ctrl = (Control)s;
                using (var pen = new Pen(ColorScheme.FondoBorde, 2))
                    e.Graphics.DrawRectangle(pen, 1, 1, ctrl.Width - 3, ctrl.Height - 3);
            };

            // Propaga color a controles hijo
            foreach (Control hijo in p.Controls)
            {
                hijo.BackColor = hijo is Label ? Color.Transparent
                               : hijo is Panel ? ColorScheme.FondoPanel
                               : ColorScheme.FondoControl;

                if (hijo.ForeColor == SystemColors.ControlText)
                    hijo.ForeColor = ColorScheme.TextoPrincipal;
            }
        }
    }
}