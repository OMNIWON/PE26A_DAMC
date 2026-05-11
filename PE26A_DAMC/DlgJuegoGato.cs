using System;
using System.Drawing; // Necesario para usar Color.Blue y Color.Red
using System.Reflection.Emit;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    /// <summary>
    /// Formulario que maneja la lógica y la interfaz del juego de Gato (Tic-Tac-Toe).
    /// </summary>
    public partial class DlgJuegoGato : Form
    {
        // ====================================================================
        // VARIABLES GLOBALES DEL JUEGO
        // ====================================================================

        /// <summary>
        /// Indica de quién es el turno actual. 
        /// true = Turno del jugador X | false = Turno del jugador O.
        /// </summary>
        private bool turnoX = true;

        /// <summary>
        /// Lleva el registro de cuántas casillas se han jugado para detectar empates.
        /// </summary>
        private int contadorTurnos = 0;

        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================
        public DlgJuegoGato()
        {
            InitializeComponent();

            // Vinculamos automáticamente el evento Click a las 9 casillas del tablero.
            foreach (Control c in this.Controls)
            {
                // Verificamos que sea un botón y que no sea el botón de reiniciar.
                // OJO: Asegúrate de que tu botón de Reiniciar se llame "btnReiniciar" en las Propiedades,
                // de lo contrario (por ejemplo, si se llama button10), se le pondrá una X o una O al hacerle clic.
                if (c is Button && c.Name != "btnReiniciar")
                {
                    c.Click += new EventHandler(ClickEnCasilla);
                }
            }
        }

        // ====================================================================
        // EVENTOS
        // ====================================================================

        /// <summary>
        /// Evento principal que se dispara cuando el usuario hace clic en cualquier casilla.
        /// </summary>
        private void ClickEnCasilla(object sender, EventArgs e)
        {
            Button botonPresionado = (Button)sender;

            if (botonPresionado.Text != "")
            {
                return;
            }

            if (turnoX == true)
            {
                botonPresionado.Text = "X";
                botonPresionado.ForeColor = Color.Blue;
            }
            else
            {
                botonPresionado.Text = "O";
                botonPresionado.ForeColor = Color.Red;
            }

            turnoX = !turnoX;
            contadorTurnos++;

            // --- ¡NUEVO CÓDIGO PARA EL LABEL! ---
            // Actualizamos el texto del label indicando de quién es el turno ahora
            
            if (turnoX == true)
            {
                lblMensaje.Text = "Turno de X";
            }
            else
            {
                lblMensaje.Text = "Turno de O";
            }
            // -------------------------------------

            VerificarGanador();
        }

        /// <summary>
        /// Evento para el botón de reiniciar.
        /// </summary>
       

        // ====================================================================
        // MÉTODOS DE LÓGICA DEL JUEGO
        // ====================================================================

        /// <summary>
        /// Evalúa todas las combinaciones posibles (filas, columnas, diagonales) 
        /// para determinar si hay un ganador o si el juego terminó en empate.
        /// </summary>
        private void VerificarGanador()
        {
            bool hayGanador = false;

            // --- REVISIÓN DE FILAS ---
            if (button1.Text != "" && button1.Text == button2.Text && button2.Text == button3.Text) hayGanador = true;
            else if (button4.Text != "" && button4.Text == button5.Text && button5.Text == button6.Text) hayGanador = true;
            else if (button7.Text != "" && button7.Text == button8.Text && button8.Text == button9.Text) hayGanador = true;

            // --- REVISIÓN DE COLUMNAS ---
            else if (button1.Text != "" && button1.Text == button4.Text && button4.Text == button7.Text) hayGanador = true;
            else if (button2.Text != "" && button2.Text == button5.Text && button5.Text == button8.Text) hayGanador = true;
            else if (button3.Text != "" && button3.Text == button6.Text && button6.Text == button9.Text) hayGanador = true;

            // --- REVISIÓN DE DIAGONALES ---
            else if (button1.Text != "" && button1.Text == button5.Text && button5.Text == button9.Text) hayGanador = true;
            else if (button3.Text != "" && button3.Text == button5.Text && button5.Text == button7.Text) hayGanador = true;

            // --- EVALUAR RESULTADO ---
            if (hayGanador)
            {
                // Como el turno se invirtió en 'ClickEnCasilla', el que ganó fue el jugador del turno anterior
                string ganador = !turnoX ? "X" : "O";

                MessageBox.Show("¡Felicidades, el jugador " + ganador + " ha ganado!", "¡Tenemos un ganador!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReiniciarTablero();
            }
            else if (contadorTurnos == 9)
            {
                // Si se llenaron las 9 casillas y no se activó 'hayGanador', es un empate matemático
                MessageBox.Show("El tablero está lleno. ¡Es un empate!", "Fin del juego", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ReiniciarTablero();
            }
        }

        /// <summary>
        /// Restablece todas las variables y limpia el texto de los botones para iniciar un juego nuevo.
        /// </summary>
        private void ReiniciarTablero()
        {
            // Regresamos los valores a su estado inicial
            turnoX = true;
            contadorTurnos = 0;

            // Limpiamos el texto de las 9 casillas
            foreach (Control c in this.Controls)
            {
                if (c is Button && c.Name != "btnReiniciar")
                {
                    c.Text = "";
                }
            }
        }

        private void btnReiniciar_Click_1(object sender, EventArgs e)
        {
            ReiniciarTablero();
        }
    }
}