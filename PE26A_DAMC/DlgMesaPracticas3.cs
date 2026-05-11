using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    //-------------------------------------------------------------------------
    // Programación Estructurada 2026A
    // Diálogo de mesa de practicas 1
    // CMAD. 2/18/2026
    //-------------------------------------------------------------------------
    public partial class DlgMesaPracticas3 : Form
    {
        //CONSTRUCTOR
        public DlgMesaPracticas3()
        {
            InitializeComponent();
        }
        //-------------------------------------------------------------------------
        // BOTÓN: Ejecuta el panel de practicas 1
        //-------------------------------------------------------------------------
        private void BtnPractica1_Click(object sender, EventArgs e)
        {
            if (PnlPracticas1.Visible)
                PnlPracticas1.Visible = false;
            else
            {
                PnlPracticas1.Visible = true;
                PnlPracticas4.Visible = false;
                PnlPracticas2.Visible = false;
                PnlPracticas3.Visible = false;


                //esta linea de abajo simplifica el codigo de arriba ------|
                //PnlPracticas1.Visible = !PnlPracticas1.Visible;     <-----|
            }
            

        }
        //-------------------------------------------------------------------------
        // BOTÓN: Ejecuta el panel de practicas 2
        //-------------------------------------------------------------------------
        private void BtnPractica2_Click(object sender, EventArgs e)
        {

            if (PnlPracticas2.Visible)
                PnlPracticas2.Visible = false;
            else
            {


                PnlPracticas2.Visible = true;
                PnlPracticas1.Visible = false;
                PnlPracticas4.Visible = false;
                PnlPracticas3.Visible = false;
            }

            // PnlPracticas2.Visible = !PnlPracticas2.Visible;
        }
        //-------------------------------------------------------------------------
        // BOTÓN: Ejecuta el panel de practicas 3
        //-------------------------------------------------------------------------
        private void BtnPractica3_Click(object sender, EventArgs e)
        {
            if (PnlPracticas3.Visible)
                PnlPracticas3.Visible = false;
            else
            {


                PnlPracticas3.Visible = true;
                PnlPracticas1.Visible = false;
                PnlPracticas2.Visible = false;
                PnlPracticas4.Visible = false;
            }

            //   PnlPracticas3.Visible = !PnlPracticas3.Visible;
        }
        //-------------------------------------------------------------------------
        // BOTÓN: Ejecuta el panel de practicas 4
        //-------------------------------------------------------------------------
        private void BtnPractica4_Click(object sender, EventArgs e)
        {
            if (PnlPracticas4.Visible)
                PnlPracticas4.Visible = false;
            else
            {


                PnlPracticas4.Visible = true;
                PnlPracticas1.Visible = false;
                PnlPracticas2.Visible = false;
                PnlPracticas3.Visible = false;
            }

            // PnlPracticas4.Visible = !PnlPracticas4.Visible;
        }

    }
}