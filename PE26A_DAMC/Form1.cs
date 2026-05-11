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
    // Programacion Estrucuturada 2026A
    //-------------------------------------------------------------------------
    //Dialogo del menu Principal del proyecto final
    //CMAD. 04/02/2026
    //-------------------------------------------------------------------------
    public partial class Form1 : Form
    {
        //-------------------------------------------------------------------------
        //CONSTRUCTOR (FUNCION ESPECIAL)
        //-------------------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
        }
        //-------------------------------------------------------------------------
        //Pregunta al usuario si desea salir del programa
        //-------------------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult repo = MessageBox.Show(
             "te gusta la michoacan?",
            "ENCUESTA MICHONA",
                    MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question
 );

            if (repo == DialogResult.Yes)
            {
                MessageBox.Show("ERES UN CHABACON", "CHAVALO CHAVACON", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("te amo", "EL HOMBRE GAI");
        }

        //-------------------------------------------------------------------------
        // convierte los grados centigrados a grados farenheit
        //-------------------------------------------------------------------------

        private double ConvertirGradosCAF(double centigrados)

        {
            double farenheit = (centigrados * 1.8) + 32;
        return farenheit;
        }
            


        //-------------------------------------------------------------------------
        // convierte los grados farenheit a grados centigrados
        //-------------------------------------------------------------------------
        private double ConvertirGradosFAC(double farenheit)
            {
            double centigrados = (farenheit - 32) * 5 / 9;
        return centigrados;


        }


        
    }
}
