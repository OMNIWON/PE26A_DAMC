using System;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    /// <summary>
    /// Formulario de la Mesa de Prácticas 3 - Navegación entre Paneles
    /// Proporciona interfaz modular para acceder a diferentes prácticas
    /// 
    /// Autor: DAMC
    /// Fecha: 2026
    /// </summary>
    public partial class DlgMesaPracticas3 : Form
    {
        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        /// <summary>
        /// Inicializa el formulario de navegación
        /// </summary>
        public DlgMesaPracticas3()
        {
            InitializeComponent();
        }

        // ====================================================================
        // GESTIÓN DE PANELES (NAVEGACIÓN ENTRE PRÁCTICAS)
        // ====================================================================

        /// <summary>
        /// Alterna visibilidad del Panel 1 y oculta los demás
        /// </summary>
        private void BtnPractica1_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas1);

        /// <summary>
        /// Alterna visibilidad del Panel 2 y oculta los demás
        /// </summary>
        private void BtnPractica2_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas2);

        /// <summary>
        /// Alterna visibilidad del Panel 3 y oculta los demás
        /// </summary>
        private void BtnPractica3_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas3);

        /// <summary>
        /// Alterna visibilidad del Panel 4 y oculta los demás
        /// </summary>
        private void BtnPractica4_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas4);

        /// <summary>
        /// Muestra un panel específico y oculta todos los demás
        /// Implementa patrón de navegación modular
        /// </summary>
        /// <param name="panelActivo">Panel a mostrar</param>
        private void MostrarPanel(Panel panelActivo)
        {
            // Ocultar todos los paneles
            PnlPracticas1.Visible = false;
            PnlPracticas2.Visible = false;
            PnlPracticas3.Visible = false;
            PnlPracticas4.Visible = false;

            // Mostrar solo el panel seleccionado
            if (panelActivo != null)
                panelActivo.Visible = true;
        }
    }
}
