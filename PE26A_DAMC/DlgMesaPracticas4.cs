// ====================================================================
// REFERENCIAS NECESARIAS EN EL PROYECTO (.csproj):
//   - PresentationCore
//   - WindowsBase
// ====================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;          // Para SystemSounds (alertas del sistema)
using System.Threading.Tasks;
using System.Windows.Forms;
// NOTA: NO se pone "using System.Windows.Media;" aquí arriba porque
// ese namespace tiene sus propios Color, Brush, etc. que entrarían en
// conflicto con System.Drawing. En su lugar, GestorSonido usa el
// nombre completamente calificado: System.Windows.Media.MediaPlayer

namespace PE26A_DAMC
{
    // ====================================================================
    // Programacion estructurada
    // Dialogo de la mesa practicas 4. DAMC.
    // ====================================================================

    public partial class DlgMesaPracticas4 : Form
    {
        // ====================================================================
        // 1. VARIABLES DE ESTADO Y LÓGICA BASE
        // ====================================================================
        private int dificultadIA = 1;

        // >>> ELIMINADA: private SoundPlayer musicaNaval;
        // >>> Se reemplaza completamente por la clase estática GestorSonido al final del archivo.

        private readonly int tamanoTablero = 10;
        private readonly int tamanoCelda = 30;
        private int[,] matrizJ1 = new int[10, 10];
        private int[,] matrizJ2 = new int[10, 10];
        private readonly int[] flota = { 5, 4, 3, 3, 2 };
        private string[] nombresBarcos = { "Portaaviones", "Acorazado", "Submarino 1", "Submarino 2", "Patrulla" };
        private bool[] barcosColocadosJ1 = new bool[5];
        private bool[] barcosColocadosJ2 = new bool[5];
        private bool faseBatalla = false;
        private bool modo1v1 = false;
        private int turnoBatalla = 1;
        private int turnoColocacion = 1;
        private bool esHorizontal = true;
        private List<Point> objetivosIA = new List<Point>();

        // ====================================================================
        // 2. VARIABLES DEL RADAR DINÁMICO
        // ====================================================================
        private int fallosJ1 = 0;
        private int fallosJ2 = 0;
        private bool senalActiva = false;
        private bool esperandoRespuestaRadar = false;
        private Panel pnlRadar;
        private System.Windows.Forms.PictureBox pbRadar;
        private Button btnAceptarSenal;
        private Button btnDenegarSenal;
        private System.Windows.Forms.Timer timerRadar;
        private int anguloRadar = 0;
        private bool parpadeoPuntoRojo = false;

        // ====================================================================
        // 3. VARIABLES: ECONOMÍA, INVENTARIO, TIENDA Y HABILIDADES
        // ====================================================================
        private int puntosJ1 = 0;
        private int puntosJ2 = 0;
        private int turnosExtraJ1 = 0;
        private int turnosExtraJ2 = 0;

        // Inventario de armas especiales
        private int minasDisponiblesJ1 = 0;
        private int minasDisponiblesJ2 = 0;
        private int misilesDisponiblesJ1 = 0;
        private int misilesDisponiblesJ2 = 0;
        private int sonaresDisponiblesJ1 = 0;
        private int sonaresDisponiblesJ2 = 0;

        // PRECIOS BALANCEADOS
        private readonly int precioMina = 30;
        private readonly int precioSonar = 40;
        private readonly int precioMisil = 80;

        // Controles de Tienda e Inventario (Generados por código)
        private Panel pnlMercado;
        private Label lblPuntosActuales;
        private Label lblClimaGlobal;
        private Button btnComprarMina;
        private Button btnComprarMisil;
        private Button btnComprarSonar;

        // Variables de Evasión (Submarino)
        private Button btnManiobraEvasion;
        private bool evasionUsadaJ1 = false;
        private bool evasionUsadaJ2 = false;

        // Variables Climáticas
        private int contadorTurnosGlobales = 0;
        private int tormentaMagneticaRestante = 0;
        private int nieblaDensaRestante = 0;

        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================
        public DlgMesaPracticas4()
        {
            InitializeComponent();
            dgvJugador1.CellMouseClick += Tablero_CellMouseClick;
            dgvJugador2.CellMouseClick += Tablero_CellMouseClick;

            ConstruirRadarDinamico();
            ConstruirMercadoTáctico();
            ConstruirBotonEvasion();

            // >>> ELIMINADO: bloque try/catch del SoundPlayer antiguo.
        }

        private void DlgMesaPracticas4_Load(object sender, EventArgs e)
        {
            ConfigurarRadarMilitar(dgvJugador1);
            ConfigurarRadarMilitar(dgvJugador2);
            CambiarPantallaJuego(pnlInicio);
        }

        // ====================================================================
        // SISTEMA DE ALERTAS
        // ====================================================================
        private void MostrarAlertaMilitar(string mensaje, string titulo, bool esCritico = false)
        {
            if (esCritico) SystemSounds.Hand.Play();
            else SystemSounds.Asterisk.Play();

            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK,
                esCritico ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        // ====================================================================
        // CREACIÓN DE INTERFAZ DINÁMICA (TIENDA E INVENTARIO)
        // ====================================================================
        private void ConstruirMercadoTáctico()
        {
            pnlMercado = new Panel
            {
                Name = "pnlMercado",
                Size = new Size(530, 95),
                BackColor = Color.FromArgb(0, 0, 0),
                BorderStyle = BorderStyle.None,
                Visible = false
            };

            pnlMercado.Paint += (s, e) =>
            {
                Control pnl = (Control)s;
                Pen lapizVerde = new Pen(Color.FromArgb(50, 205, 50), 3);
                e.Graphics.DrawRectangle(lapizVerde, 0, 0, pnl.Width - 1, pnl.Height - 1);
            };

            pnlMercado.Location = new Point(320, 15);

            lblPuntosActuales = new Label
            {
                Text = "FONDOS: $0",
                ForeColor = Color.Lime,
                Font = new Font("Consolas", 10, FontStyle.Italic),
                Location = new Point(10, 10),
                AutoSize = true
            };

            lblClimaGlobal = new Label
            {
                Text = "CLIMA: DESPEJADO",
                ForeColor = Color.Cyan,
                Font = new Font("Consolas", 10, FontStyle.Italic),
                Location = new Point(250, 10),
                AutoSize = true
            };

            btnComprarMina = new Button
            {
                Text = $"MINA(${precioMina})",
                BackColor = Color.DarkOliveGreen,
                ForeColor = Color.LimeGreen,
                Location = new Point(8, 35),
                Size = new Size(150, 45),
                FlatStyle = FlatStyle.Flat,
                AutoSize = true
            };
            btnComprarMina.Click += BtnComprarMina_Click;

            btnComprarMisil = new Button
            {
                Text = $"MISIL DE ÁREA(${precioMisil})",
                BackColor = Color.DarkOliveGreen,
                ForeColor = Color.LimeGreen,
                Location = new Point(170, 35),
                Size = new Size(150, 45),
                FlatStyle = FlatStyle.Flat,
                AutoSize = true
            };
            btnComprarMisil.Click += BtnComprarMisil_Click;

            btnComprarSonar = new Button
            {
                Text = $"SONAR 3x3 (${precioSonar})",
                BackColor = Color.DarkOliveGreen,
                ForeColor = Color.LimeGreen,
                Location = new Point(365, 35),
                Size = new Size(150, 45),
                FlatStyle = FlatStyle.Flat,
                AutoSize = true
            };
            btnComprarSonar.Click += BtnComprarSonar_Click;

            pnlMercado.Controls.Add(lblPuntosActuales);
            pnlMercado.Controls.Add(lblClimaGlobal);
            pnlMercado.Controls.Add(btnComprarMina);
            pnlMercado.Controls.Add(btnComprarMisil);
            pnlMercado.Controls.Add(btnComprarSonar);

            cmbSelectorArmas = new ComboBox
            {
                Location = new Point(875, 133),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false,
                BackColor = Color.DarkGreen,
                ForeColor = Color.Black
            };
            cmbSelectorArmas.Font = new Font("Consolas", 11, FontStyle.Italic);
            cmbSelectorArmas.Items.Add("Disparo Básico");
            cmbSelectorArmas.SelectedIndex = 0;

            lblInventario = new Label
            {
                Location = new Point(440, 450),
                Size = new Size(400, 40),
                ForeColor = Color.Yellow,
                Font = new Font("Consolas", 10, FontStyle.Italic),
                Visible = false
            };

            if (pnlJuego != null)
            {
                pnlJuego.Controls.Add(pnlMercado);
                pnlJuego.Controls.Add(cmbSelectorArmas);
                pnlJuego.Controls.Add(lblInventario);

                cmbSelectorArmas.BringToFront();
                lblInventario.BringToFront();
            }
        }

        private void ConstruirBotonEvasion()
        {
            btnManiobraEvasion = new Button
            {
                Text = "MANIOBRA EVASIÓN",
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Location = new Point(925, 520),
                Size = new Size(160, 30),
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            btnManiobraEvasion.Click += BtnManiobraEvasion_Click;
            if (pnlJuego != null) pnlJuego.Controls.Add(btnManiobraEvasion);
        }

        // ====================================================================
        // LÓGICA DE LA TIENDA E INVENTARIO
        // ====================================================================
        private void ActualizarTienda()
        {
            int misPuntos = (turnoBatalla == 1) ? puntosJ1 : puntosJ2;
            int misMinas = (turnoBatalla == 1) ? minasDisponiblesJ1 : minasDisponiblesJ2;
            int misMisiles = (turnoBatalla == 1) ? misilesDisponiblesJ1 : misilesDisponiblesJ2;
            int misSonares = (turnoBatalla == 1) ? sonaresDisponiblesJ1 : sonaresDisponiblesJ2;

            lblPuntosActuales.Text = $"FONDOS J{turnoBatalla}: ${misPuntos}";
            lblInventario.Text = $"Inventario: Minas({misMinas}) | Misiles({misMisiles}) | Sonar({misSonares})";

            btnComprarMina.Enabled = (misPuntos >= precioMina);
            btnComprarMisil.Enabled = (misPuntos >= precioMisil);
            btnComprarSonar.Enabled = (misPuntos >= precioSonar);

            string seleccionPrevia = cmbSelectorArmas.SelectedItem?.ToString();
            cmbSelectorArmas.Items.Clear();
            cmbSelectorArmas.Items.Add("Disparo Básico");
            if (misMinas > 0) cmbSelectorArmas.Items.Add("Colocar Mina");
            if (misMisiles > 0) cmbSelectorArmas.Items.Add("Misil de Área");
            if (misSonares > 0) cmbSelectorArmas.Items.Add("Escáner Sonar");

            if (seleccionPrevia != null && cmbSelectorArmas.Items.Contains(seleccionPrevia))
                cmbSelectorArmas.SelectedItem = seleccionPrevia;
            else
                cmbSelectorArmas.SelectedIndex = 0;
        }

        private void BtnComprarMina_Click(object sender, EventArgs e)
        {
            int misPuntos = (turnoBatalla == 1) ? puntosJ1 : puntosJ2;
            if (misPuntos >= precioMina)
            {
                if (turnoBatalla == 1) { puntosJ1 -= precioMina; minasDisponiblesJ1++; }
                else { puntosJ2 -= precioMina; minasDisponiblesJ2++; }

                // >>> SFX: Compra exitosa
                GestorSonido.ReproducirSFX("sfx_compra.mp3");

                ActualizarTienda();
                MostrarAlertaMilitar(
                    "Mina Marina comprada. Selecciónala en armas y haz clic en TU TABLERO para esconderla. Luego podrás disparar normalmente.",
                    "Mercado Negro");
            }
        }

        private void BtnComprarMisil_Click(object sender, EventArgs e)
        {
            int misPuntos = (turnoBatalla == 1) ? puntosJ1 : puntosJ2;
            if (misPuntos >= precioMisil)
            {
                if (turnoBatalla == 1) { puntosJ1 -= precioMisil; misilesDisponiblesJ1++; }
                else { puntosJ2 -= precioMisil; misilesDisponiblesJ2++; }

                // >>> SFX: Compra exitosa
                GestorSonido.ReproducirSFX("sfx_compra.mp3");

                ActualizarTienda();
                MostrarAlertaMilitar(
                    "Misil de Área listo. Al usarlo, impactarás una celda y las 4 que la rodean (en forma de cruz). Consume tu turno.",
                    "Mercado Negro");
            }
        }

        private void BtnComprarSonar_Click(object sender, EventArgs e)
        {
            int misPuntos = (turnoBatalla == 1) ? puntosJ1 : puntosJ2;
            if (misPuntos >= precioSonar)
            {
                if (turnoBatalla == 1) { puntosJ1 -= precioSonar; sonaresDisponiblesJ1++; }
                else { puntosJ2 -= precioSonar; sonaresDisponiblesJ2++; }

                // >>> SFX: Compra exitosa
                GestorSonido.ReproducirSFX("sfx_compra.mp3");

                ActualizarTienda();
                MostrarAlertaMilitar(
                    "Escáner Sonar activo. Revela si hay algún barco enemigo en un área de 3x3. Consume tu turno.",
                    "Mercado Negro");
            }
        }

        // ====================================================================
        // LÓGICA DEL CLIMA DINÁMICO
        // ====================================================================
        private void ActualizarClima()
        {
            if (tormentaMagneticaRestante > 0) tormentaMagneticaRestante--;
            if (nieblaDensaRestante > 0) nieblaDensaRestante--;

            if (tormentaMagneticaRestante > 0)
            {
                lblClimaGlobal.Text = $"CLIMA: TORMENTA ({tormentaMagneticaRestante})";
                lblClimaGlobal.ForeColor = Color.Orange;
                pnlRadar.Enabled = false;
            }
            else if (nieblaDensaRestante > 0)
            {
                lblClimaGlobal.Text = $"CLIMA: NIEBLA ({nieblaDensaRestante})";
                lblClimaGlobal.ForeColor = Color.LightGray;
                pnlRadar.Enabled = true;
            }
            else
            {
                lblClimaGlobal.Text = "CLIMA: DESPEJADO";
                lblClimaGlobal.ForeColor = Color.Cyan;
                pnlRadar.Enabled = true;
            }
        }

        private void TirarDadosClimaticos()
        {
            contadorTurnosGlobales++;
            if (contadorTurnosGlobales % 6 == 0)
            {
                Random rnd = new Random();
                int evento = rnd.Next(100);

                if (evento < 30)
                {
                    tormentaMagneticaRestante = 2;
                    MostrarAlertaMilitar(
                        "Una tormenta magnética ha entrado a la zona de guerra. Los radares estarán inoperativos 2 turnos.",
                        "Alerta Climática", true);
                }
                else if (evento < 60)
                {
                    nieblaDensaRestante = 2;
                    MostrarAlertaMilitar(
                        "Un banco de niebla densa cubre el mar. Los disparos fallidos al agua no serán visibles temporalmente.",
                        "Alerta Climática");
                }
            }
            ActualizarClima();
        }

        // ====================================================================
        // LÓGICA DE INMERSIÓN DE EMERGENCIA (SUBMARINO)
        // ====================================================================
        private void VerificarEstadoEvasion()
        {
            int[,] miMatriz = (turnoBatalla == 1) ? matrizJ1 : matrizJ2;
            bool yaUsada = (turnoBatalla == 1) ? evasionUsadaJ1 : evasionUsadaJ2;

            List<int> barcosVivos = new List<int>();
            foreach (int celda in miMatriz)
                if (celda >= 10 && celda <= 14 && !barcosVivos.Contains(celda))
                    barcosVivos.Add(celda);

            btnManiobraEvasion.Visible =
                (barcosVivos.Count == 1 && (barcosVivos[0] == 12 || barcosVivos[0] == 13) && !yaUsada);
        }

        private void BtnManiobraEvasion_Click(object sender, EventArgs e)
        {
            int[,] miMatriz = (turnoBatalla == 1) ? matrizJ1 : matrizJ2;
            int idSubmarino = -1;

            if (SigueVivo(miMatriz, 12)) idSubmarino = 12;
            else if (SigueVivo(miMatriz, 13)) idSubmarino = 13;

            if (idSubmarino != -1)
            {
                for (int f = 0; f < 10; f++)
                    for (int c = 0; c < 10; c++)
                        if (miMatriz[f, c] == idSubmarino) miMatriz[f, c] = 0;

                bool reubicado = false;
                Random rnd = new Random();
                while (!reubicado)
                {
                    int f = rnd.Next(10);
                    int c = rnd.Next(10);
                    bool hor = rnd.Next(2) == 0;
                    if (PuedeColocarBarco(miMatriz, f, c, 3, hor))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (hor) miMatriz[f, c + i] = idSubmarino;
                            else miMatriz[f + i, c] = idSubmarino;
                        }
                        reubicado = true;
                    }
                }

                MostrarAlertaMilitar(
                    "Inmersión de emergencia ejecutada. El submarino ha reubicado su posición en el mapa.",
                    "Maniobra Táctica", true);

                if (turnoBatalla == 1) evasionUsadaJ1 = true;
                else evasionUsadaJ2 = true;

                btnManiobraEvasion.Visible = false;
                DibujarTablero((turnoBatalla == 1) ? dgvJugador1 : dgvJugador2, miMatriz, false);
            }
        }

        // ====================================================================
        // CREACIÓN DEL RADAR DINÁMICO
        // ====================================================================
        private void ConstruirRadarDinamico()
        {
            pnlRadar = new Panel
            {
                Size = new Size(160, 220),
                BackColor = Color.FromArgb(20, 20, 20),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            pbRadar = new System.Windows.Forms.PictureBox
            {
                Size = new Size(140, 140),
                Location = new Point(10, 10),
                BackColor = Color.Black
            };
            pbRadar.Paint += PbRadar_Paint;

            btnAceptarSenal = new Button
            {
                Text = "ACEPTAR SEÑAL",
                Size = new Size(140, 25),
                Location = new Point(10, 160),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnAceptarSenal.Click += BtnAceptarSenal_Click;

            btnDenegarSenal = new Button
            {
                Text = "DENEGAR",
                Size = new Size(140, 25),
                Location = new Point(10, 190),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            btnDenegarSenal.Click += BtnDenegarSenal_Click;

            pnlRadar.Controls.Add(pbRadar);
            pnlRadar.Controls.Add(btnAceptarSenal);
            pnlRadar.Controls.Add(btnDenegarSenal);

            pnlRadar.Location = new Point(910, 240);
            if (pnlJuego != null) pnlJuego.Controls.Add(pnlRadar);

            timerRadar = new System.Windows.Forms.Timer { Interval = 50 };
            timerRadar.Tick += (s, ev) =>
            {
                anguloRadar = (anguloRadar + 5) % 360;
                parpadeoPuntoRojo = !parpadeoPuntoRojo;
                pbRadar.Invalidate();
            };
        }

        private void PbRadar_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int size = pbRadar.Width;
            int center = size / 2;

            Color colorRadar = Color.LimeGreen;
            if (turnoBatalla == 2) colorRadar = modo1v1 ? Color.Cyan : Color.Red;

            g.DrawEllipse(new Pen(colorRadar, 2), 2, 2, size - 5, size - 5);
            g.DrawEllipse(new Pen(colorRadar, 1), center / 2, center / 2, center, center);
            g.DrawLine(new Pen(Color.FromArgb(100, colorRadar), 1), center, 0, center, size);
            g.DrawLine(new Pen(Color.FromArgb(100, colorRadar), 1), 0, center, size, center);

            double rad = anguloRadar * Math.PI / 180;
            int x2 = center + (int)(center * Math.Cos(rad));
            int y2 = center + (int)(center * Math.Sin(rad));
            g.DrawLine(new Pen(colorRadar, 2), center, center, x2, y2);

            if (senalActiva && parpadeoPuntoRojo)
                g.FillEllipse(Brushes.Red, center + 20, center - 30, 10, 10);
        }

        private void BtnAceptarSenal_Click(object sender, EventArgs e)
        {
            // >>> SFX: Activación del radar / intercomunicador
            GestorSonido.ReproducirSFX("sfx_radar.mp3");

            DesactivarSenalRadar();
            int[,] matrizEnemiga = (turnoBatalla == 1) ? matrizJ2 : matrizJ1;
            RevelarPistaIntercomunicador(matrizEnemiga);
            esperandoRespuestaRadar = false;
            FinalizarTurno();
        }

        private void BtnDenegarSenal_Click(object sender, EventArgs e)
        {
            SystemSounds.Hand.Play();
            DesactivarSenalRadar();
            esperandoRespuestaRadar = false;
            FinalizarTurno();
        }

        private void DesactivarSenalRadar()
        {
            senalActiva = false;
            btnAceptarSenal.Enabled = false;
            btnDenegarSenal.Enabled = false;
            if (turnoBatalla == 1) fallosJ1 = 0; else fallosJ2 = 0;
            pbRadar.Invalidate();
        }

        private void RevelarPistaIntercomunicador(int[,] matrizEnemiga)
        {
            List<Point> celdasVivas = new List<Point>();
            for (int f = 0; f < 10; f++)
                for (int c = 0; c < 10; c++)
                    if (matrizEnemiga[f, c] >= 10 && matrizEnemiga[f, c] <= 14)
                        celdasVivas.Add(new Point(f, c));

            if (celdasVivas.Count > 0)
            {
                Random rnd = new Random();
                Point objetivo = celdasVivas[rnd.Next(celdasVivas.Count)];
                char letraColumna = (char)('A' + objetivo.Y);
                int numeroFila = objetivo.X + 1;
                bool pistaFila = rnd.Next(2) == 0;
                string pista = pistaFila ? $"en la Fila {numeroFila}" : $"en la Columna {letraColumna}";

                MostrarAlertaMilitar(
                    $"📡 TRANSMISIÓN DESENCRIPTADA: Comandante, inteligencia detecta emisiones térmicas enemigas {pista}.",
                    "Radar Intercomunicador");
            }
        }

        // ====================================================================
        // GESTOR DE PANTALLAS (NAVEGACIÓN)
        // ====================================================================
        private void CambiarPantallaJuego(Panel pantallaDestino)
        {
            if (pnlInicio != null) pnlInicio.Visible = false;
            if (pnlMenuModos != null) pnlMenuModos.Visible = false;
            if (pnlDificultad != null) pnlDificultad.Visible = false;
            if (pnlJuego != null) pnlJuego.Visible = false;

            if (pantallaDestino != null)
            {
                pantallaDestino.Visible = true;
                pantallaDestino.BringToFront();
            }
        }

        // ====================================================================
        // BOTONES DE NAVEGACIÓN PRINCIPAL
        // ====================================================================

        private void btnIniciarJuego_Click(object sender, EventArgs e)
        {
            // >>> BGM: Música del menú al entrar al modo de juego
            GestorSonido.ReproducirBGM("musica_menu.mp3");
            CambiarPantallaJuego(pnlMenuModos);
        }

        private void btnSalirJuego_Click(object sender, EventArgs e)
        {
            // >>> BGM: Silencio al volver a la pantalla de inicio
            GestorSonido.DetenerBGM();
            CambiarPantallaJuego(pnlInicio);
        }

        private void btnModo1v1_Click(object sender, EventArgs e)
        {
            modo1v1 = true;
            IniciarFaseColocacion();
        }

        private void btnModoPC_Click(object sender, EventArgs e)
        {
            modo1v1 = false;
            CambiarPantallaJuego(pnlDificultad);
        }

        private void btnVolverMenu_Click(object sender, EventArgs e)
        {
            CambiarPantallaJuego(pnlMenuModos);
        }

        private void btnFacil_Click(object sender, EventArgs e)
        {
            dificultadIA = 0;
            IniciarFaseColocacion();
        }

        private void btnExperto_Click(object sender, EventArgs e)
        {
            dificultadIA = 1;
            IniciarFaseColocacion();
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            faseBatalla = false;
            timerRadar.Stop();
            pnlRadar.Visible = false;
            pnlMercado.Visible = false;
            cmbSelectorArmas.Visible = false;
            lblInventario.Visible = false;

            // >>> BGM: Silencio al volver a inicio (la música de menú arrancará
            //     de nuevo cuando el jugador pulse btnIniciarJuego)
            GestorSonido.DetenerBGM();
            CambiarPantallaJuego(pnlInicio);
        }

        // ====================================================================
        // MOTOR DE JUEGO (ARRANQUE Y DISPAROS)
        // ====================================================================
        private void IniciarFaseColocacion()
        {
            // >>> BGM: Música de colocación/acomodar barcos
            GestorSonido.ReproducirBGM("musica_acomodar.mp3");

            CambiarPantallaJuego(pnlJuego);
            faseBatalla = false; turnoColocacion = 1; objetivosIA.Clear();
            puntosJ1 = 0; puntosJ2 = 0; turnosExtraJ1 = 0; turnosExtraJ2 = 0;
            minasDisponiblesJ1 = 0; minasDisponiblesJ2 = 0;
            misilesDisponiblesJ1 = 0; misilesDisponiblesJ2 = 0;
            sonaresDisponiblesJ1 = 0; sonaresDisponiblesJ2 = 0;
            evasionUsadaJ1 = false; evasionUsadaJ2 = false;
            contadorTurnosGlobales = 0;

            fallosJ1 = 0; fallosJ2 = 0; esperandoRespuestaRadar = false;
            DesactivarSenalRadar();
            pnlRadar.Visible = false;
            pnlMercado.Visible = false;
            btnManiobraEvasion.Visible = false;
            cmbSelectorArmas.Visible = false;
            lblInventario.Visible = false;
            timerRadar.Stop();

            Array.Clear(matrizJ1, 0, matrizJ1.Length);
            Array.Clear(matrizJ2, 0, matrizJ2.Length);
            Array.Clear(barcosColocadosJ1, 0, barcosColocadosJ1.Length);
            Array.Clear(barcosColocadosJ2, 0, barcosColocadosJ2.Length);

            DibujarTablero(dgvJugador1, matrizJ1, false);
            DibujarTablero(dgvJugador2, matrizJ2, false);

            btnAutoJ1.Visible = btnRotar.Visible = btnConfirmar.Visible = btnReacomodar.Visible = true;
            if (cmbSeleccionBarco != null) cmbSeleccionBarco.Visible = true;
            btnAutoJ2.Visible = false;

            CargarMenuBarcos();
            ActualizarLabels(matrizJ1, barcosColocadosJ1);
            lblEstado.Text = "JUGADOR 1: Despliega tu flota.";

            if (!modo1v1)
            {
                GenerarFlotaAleatoria(matrizJ2);
                for (int i = 0; i < 5; i++) barcosColocadosJ2[i] = true;
                DibujarTablero(dgvJugador2, matrizJ2, true);
            }
        }

        private void ProcesarDisparo(int[,] matriz, int f, int c, DataGridView dgv)
        {
            int estadoCelda = matriz[f, c];
            if (estadoCelda == 2 || estadoCelda == 3 || estadoCelda == 21 || estadoCelda == 4) return;

            bool huboAcierto = false;
            string mensaje = "";

            // ── Mina enemiga ──────────────────────────────────────────────
            if (estadoCelda == 20)
            {
                // >>> SFX: Explosión de mina
                GestorSonido.ReproducirSFX("sfx_mina.mp3");

                matriz[f, c] = 21;
                MostrarAlertaMilitar(
                    "¡BOOOM! ¡Has impactado una mina enemiga! Pierdes tu turno y el enemigo gana una acción.",
                    "¡TRAMPA!", true);

                if (turnoBatalla == 1) turnosExtraJ2++;
                else turnosExtraJ1++;

                DibujarTablero(dgv, matriz, true);
                FinalizarTurno();
                return;
            }

            // ── Impacto en barco ──────────────────────────────────────────
            if (estadoCelda >= 10 && estadoCelda <= 14)
            {
                matriz[f, c] = 2;
                huboAcierto = true;

                if (turnoBatalla == 1) { fallosJ1 = 0; puntosJ1 += 10; }
                else { fallosJ2 = 0; puntosJ2 += 10; }

                DesactivarSenalRadar();

                if (!SigueVivo(matriz, estadoCelda))
                {
                    // >>> SFX: Barco hundido
                    GestorSonido.ReproducirSFX("sfx_hundido.mp3");

                    if (turnoBatalla == 1) puntosJ1 += 40;
                    else puntosJ2 += 40;

                    mensaje = $"¡HUNDIDO! Destruiste el {nombresBarcos[estadoCelda - 10]}. ¡Tienes un tiro extra!";
                }
                else
                {
                    // >>> SFX: Impacto en barco (no hundido)
                    GestorSonido.ReproducirSFX("sfx_impacto.mp3");

                    mensaje = "¡IMPACTO CONFIRMADO! ¡Tienes un tiro extra!";
                }
            }
            else
            {
                // ── Disparo al agua ───────────────────────────────────────
                // >>> SFX: Agua
                GestorSonido.ReproducirSFX("sfx_agua.mp3");

                matriz[f, c] = 3;
                if (turnoBatalla == 1) fallosJ1++;
                else fallosJ2++;

                mensaje = "¡DISPARO AL AGUA! 🌊";
            }

            DibujarTablero(dgv, matriz, true);
            ActualizarLabels(matriz, null);
            ActualizarTienda();

            int misTurnosExtra = (turnoBatalla == 1) ? turnosExtraJ1 : turnosExtraJ2;

            if (!huboAcierto)
            {
                if (misTurnosExtra > 0)
                {
                    if (turnoBatalla == 1) turnosExtraJ1--;
                    else turnosExtraJ2--;

                    MostrarAlertaMilitar(
                        mensaje + " Fallo, PERO el enemigo detonó una mina tuya. ¡Tienes un turno extra!",
                        "Estrategia");
                    return;
                }

                int misFallosActuales = (turnoBatalla == 1) ? fallosJ1 : fallosJ2;
                if (misFallosActuales >= 3 && !senalActiva && tormentaMagneticaRestante == 0)
                {
                    MostrarAlertaMilitar(
                        mensaje + " ⚠️ ¡ALERTA! SEÑAL DE RADAR DETECTADA.",
                        "Reporte de Artillería", true);
                    senalActiva = true;
                    esperandoRespuestaRadar = true;
                    btnAceptarSenal.Enabled = true;
                    btnDenegarSenal.Enabled = true;
                    return;
                }
                else
                {
                    MostrarAlertaMilitar(mensaje + " Cambio de turno.", "Reporte de Artillería");
                }

                if (VerificarVictoria(matriz)) { TerminarGuerra(); return; }

                FinalizarTurno();
            }
            else
            {
                MostrarAlertaMilitar(mensaje, "Reporte de Artillería");
                if (VerificarVictoria(matriz)) { TerminarGuerra(); return; }

                if (!modo1v1 && turnoBatalla == 2) TurnoPC();
            }
        }

        private async void FinalizarTurno()
        {
            TirarDadosClimaticos();

            if (modo1v1)
            {
                await Task.Delay(500);

                turnoBatalla = (turnoBatalla == 1) ? 2 : 1;
                pbRadar.Invalidate();

                MostrarAlertaMilitar(
                    $"Cambio de turno. Jugador {turnoBatalla}, siéntate al mando y presiona 'Aceptar' cuando estés listo.",
                    "Cambio de Comandante");

                PrepararTablerosParaTurno();
            }
            else
            {
                turnoBatalla = 2;
                lblEstado.Text = "Turno de la PC...";
                Application.DoEvents();
                TurnoPC();
            }
        }

        // ====================================================================
        // IA DE COMPUTADORA
        // ====================================================================
        private void TurnoPC()
        {
            Application.DoEvents();
            System.Threading.Thread.Sleep(600);

            Random rnd = new Random();
            int f = 0, c = 0;
            bool disparoValido = false;

            if (dificultadIA == 1 && objetivosIA.Count > 0)
            {
                Point p = objetivosIA[0]; objetivosIA.RemoveAt(0);
                f = p.X; c = p.Y;
                if (f >= 0 && f < 10 && c >= 0 && c < 10 &&
                    matrizJ1[f, c] != 2 && matrizJ1[f, c] != 3 &&
                    matrizJ1[f, c] != 21 && matrizJ1[f, c] != 4)
                    disparoValido = true;
            }

            if (!disparoValido)
            {
                while (!disparoValido)
                {
                    f = rnd.Next(10); c = rnd.Next(10);
                    if (matrizJ1[f, c] != 2 && matrizJ1[f, c] != 3 &&
                        matrizJ1[f, c] != 21 && matrizJ1[f, c] != 4)
                        disparoValido = true;
                }
            }

            int celda = matrizJ1[f, c];
            bool acierto = false;

            if (celda == 20)
            {
                // >>> SFX: PC detona mina
                GestorSonido.ReproducirSFX("sfx_mina.mp3");

                matrizJ1[f, c] = 21;
                MostrarAlertaMilitar("¡La PC ha detonado tu mina! Pierde su turno y ganas una acción.", "¡BOOM!", true);
                turnosExtraJ1++;
                DibujarTablero(dgvJugador1, matrizJ1, false);
                turnoBatalla = 1;
                PrepararTablerosParaTurno();
                return;
            }

            if (celda >= 10 && celda <= 14)
            {
                matrizJ1[f, c] = 2;
                acierto = true;
                if (dificultadIA == 1) AgregarObjetivosA_MemoriaIA(f, c);

                if (!SigueVivo(matrizJ1, celda))
                {
                    // >>> SFX: PC hunde un barco tuyo
                    GestorSonido.ReproducirSFX("sfx_hundido.mp3");
                    MostrarAlertaMilitar(
                        $"¡ALERTA! La PC ha hundido tu {nombresBarcos[celda - 10]}. Y va a volver a disparar.",
                        "Ataque Enemigo", true);
                }
                else
                {
                    // >>> SFX: PC impacta un barco tuyo
                    GestorSonido.ReproducirSFX("sfx_impacto.mp3");
                    MostrarAlertaMilitar("La PC ha impactado uno de tus barcos. Y va a volver a disparar.", "Ataque Enemigo");
                }
            }
            else
            {
                // >>> SFX: PC dispara al agua
                GestorSonido.ReproducirSFX("sfx_agua.mp3");

                matrizJ1[f, c] = 3;
                MostrarAlertaMilitar("La PC disparó al agua. Ahora es tu turno.", "Ataque Enemigo");
            }

            DibujarTablero(dgvJugador1, matrizJ1, false);
            ActualizarLabels(matrizJ1, null);

            if (VerificarVictoria(matrizJ1)) { TerminarGuerra(); return; }

            if (!acierto)
            {
                turnoBatalla = 1;
                pbRadar.Invalidate();
                PrepararTablerosParaTurno();
            }
            else TurnoPC();
        }

        private void AgregarObjetivosA_MemoriaIA(int filaHit, int colHit)
        {
            int[] dirFila = { -1, 1, 0, 0 };
            int[] dirCol = { 0, 0, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int nFila = filaHit + dirFila[i];
                int nCol = colHit + dirCol[i];
                if (nFila >= 0 && nFila < 10 && nCol >= 0 && nCol < 10 &&
                    matrizJ1[nFila, nCol] != 2 && matrizJ1[nFila, nCol] != 3)
                    objetivosIA.Insert(0, new Point(nFila, nCol));
            }
        }

        private void TerminarGuerra()
        {
            faseBatalla = false;
            string ganador = (turnoBatalla == 1)
                ? "¡JUGADOR 1 GANA LA GUERRA!"
                : (modo1v1 ? "¡JUGADOR 2 GANA LA GUERRA!" : "¡LA PC GANA LA GUERRA!");

            lblEstado.Text = ganador;

            pnlRadar.Visible = false;
            pnlMercado.Visible = false;
            btnManiobraEvasion.Visible = false;
            timerRadar.Stop();
            cmbSelectorArmas.Visible = false;
            lblInventario.Visible = false;

            // >>> BGM: Silencio al terminar la partida
            GestorSonido.DetenerBGM();

            MostrarAlertaMilitar(ganador, "FIN DEL JUEGO", true);

            btnAutoJ1.Visible = btnAutoJ2.Visible = btnRotar.Visible =
            btnReacomodar.Visible = btnConfirmar.Visible = false;
            if (cmbSeleccionBarco != null) cmbSeleccionBarco.Visible = false;
        }

        // ====================================================================
        // EVENTOS CLIC EN TABLERO E INVENTARIO DE ARMAS
        // ====================================================================
        private void Tablero_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            DataGridView dgv = (DataGridView)sender;

            if (faseBatalla && esperandoRespuestaRadar)
            {
                MostrarAlertaMilitar("¡Comandante, atienda la señal del radar antes de continuar!", "Radar Activo", true);
                return;
            }

            if (!faseBatalla)
            {
                if (cmbSeleccionBarco.SelectedIndex == -1) return;
                int[,] matrizAfectada = (turnoColocacion == 1) ? matrizJ1 : matrizJ2;
                bool[] colocadosActual = (turnoColocacion == 1) ? barcosColocadosJ1 : barcosColocadosJ2;

                if (turnoColocacion == 1 && dgv != dgvJugador1) return;
                if (turnoColocacion == 2 && dgv != dgvJugador2) return;

                int indiceBarco = cmbSeleccionBarco.SelectedIndex;
                if (colocadosActual[indiceBarco])
                {
                    MostrarAlertaMilitar("¡Ya desplegaste este barco! Elige otro o dale a Reacomodar.", "Atención");
                    return;
                }

                int tamano = flota[indiceBarco];
                int idBarco = 10 + indiceBarco;

                if (PuedeColocarBarco(matrizAfectada, e.RowIndex, e.ColumnIndex, tamano, esHorizontal))
                {
                    for (int i = 0; i < tamano; i++)
                    {
                        if (esHorizontal) matrizAfectada[e.RowIndex, e.ColumnIndex + i] = idBarco;
                        else matrizAfectada[e.RowIndex + i, e.ColumnIndex] = idBarco;
                    }
                    colocadosActual[indiceBarco] = true;
                    DibujarTablero(dgv, matrizAfectada, false);
                    ActualizarLabels(matrizAfectada, colocadosActual);
                }
                else MostrarAlertaMilitar("¡Posición inválida! El barco choca o sale del mapa.", "Error Táctico");
            }
            else
            {
                string armaSeleccionada = cmbSelectorArmas.SelectedItem?.ToString() ?? "Disparo Básico";

                // ── Colocar Mina ──────────────────────────────────────────
                if (armaSeleccionada == "Colocar Mina")
                {
                    if (turnoBatalla == 1 && dgv != dgvJugador1)
                    { MostrarAlertaMilitar("Debes colocar la mina en TU propio tablero.", "Error"); return; }
                    if (turnoBatalla == 2 && dgv != dgvJugador2)
                    { MostrarAlertaMilitar("Debes colocar la mina en TU propio tablero.", "Error"); return; }

                    int[,] miMatriz = (turnoBatalla == 1) ? matrizJ1 : matrizJ2;
                    if (miMatriz[e.RowIndex, e.ColumnIndex] == 0)
                    {
                        miMatriz[e.RowIndex, e.ColumnIndex] = 20;
                        if (turnoBatalla == 1) minasDisponiblesJ1--;
                        else minasDisponiblesJ2--;

                        MostrarAlertaMilitar(
                            "Mina colocada en el agua con éxito. Ahora realiza tu disparo a la flota enemiga.",
                            "Estrategia");
                        DibujarTablero(dgv, miMatriz, false);
                        cmbSelectorArmas.SelectedIndex = 0;
                        ActualizarTienda();
                        return;
                    }
                    else { MostrarAlertaMilitar("Celda ocupada. Busca agua libre.", "Error"); return; }
                }

                // ── Escáner Sonar ─────────────────────────────────────────
                if (armaSeleccionada == "Escáner Sonar")
                {
                    if (turnoBatalla == 1 && dgv != dgvJugador2)
                    { MostrarAlertaMilitar("Apunta al tablero enemigo.", "Error"); return; }
                    if (turnoBatalla == 2 && dgv != dgvJugador1)
                    { MostrarAlertaMilitar("Apunta al tablero enemigo.", "Error"); return; }

                    int[,] matrizObjetivo = (turnoBatalla == 1) ? matrizJ2 : matrizJ1;
                    bool detectado = false;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int nf = e.RowIndex + i;
                            int nc = e.ColumnIndex + j;
                            if (nf >= 0 && nf < 10 && nc >= 0 && nc < 10)
                            {
                                int celda = matrizObjetivo[nf, nc];
                                if (celda >= 10 && celda <= 14) detectado = true;
                                else if (celda == 0) matrizObjetivo[nf, nc] = 4;
                            }
                        }
                    }

                    if (turnoBatalla == 1) sonaresDisponiblesJ1--;
                    else sonaresDisponiblesJ2--;

                    // >>> SFX: Uso del sonar/radar
                    GestorSonido.ReproducirSFX("sfx_radar.mp3");

                    DibujarTablero(dgv, matrizObjetivo, true);

                    if (detectado)
                        MostrarAlertaMilitar(
                            "¡SEÑALES TÉRMICAS CONFIRMADAS! Hay fuerte presencia naval enemiga en el área escaneada. Nota: El agua vacía ha sido marcada de verde.",
                            "Sonar Satelital", true);
                    else
                        MostrarAlertaMilitar(
                            "El sonar no detecta actividad en este sector. Zona despejada marcada en verde.",
                            "Sonar Satelital");

                    ActualizarTienda();
                    FinalizarTurno();
                    return;
                }

                // ── Misil de Área ─────────────────────────────────────────
                if (armaSeleccionada == "Misil de Área")
                {
                    if (turnoBatalla == 1 && dgv != dgvJugador2)
                    { MostrarAlertaMilitar("Apunta al tablero enemigo.", "Error"); return; }
                    if (turnoBatalla == 2 && dgv != dgvJugador1)
                    { MostrarAlertaMilitar("Apunta al tablero enemigo.", "Error"); return; }

                    int[,] matrizObjetivo = (turnoBatalla == 1) ? matrizJ2 : matrizJ1;
                    int[] dirF = { 0, -1, 1, 0, 0 };
                    int[] dirC = { 0, 0, 0, -1, 1 };
                    int aciertos = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        int nf = e.RowIndex + dirF[i];
                        int nc = e.ColumnIndex + dirC[i];
                        if (nf >= 0 && nf < 10 && nc >= 0 && nc < 10)
                        {
                            int celda = matrizObjetivo[nf, nc];
                            if (celda >= 10 && celda <= 14)
                            {
                                matrizObjetivo[nf, nc] = 2;
                                aciertos++;
                                if (turnoBatalla == 1) puntosJ1 += 10; else puntosJ2 += 10;
                                if (!SigueVivo(matrizObjetivo, celda))
                                {
                                    if (turnoBatalla == 1) puntosJ1 += 40; else puntosJ2 += 40;
                                }
                            }
                            else if (celda == 0 || celda == 4)
                            {
                                matrizObjetivo[nf, nc] = 3;
                            }
                            else if (celda == 20)
                            {
                                matrizObjetivo[nf, nc] = 21;
                                if (turnoBatalla == 1) turnosExtraJ2++; else turnosExtraJ1++;
                                GestorSonido.ReproducirSFX("sfx_mina.mp3");
                                MostrarAlertaMilitar(
                                    "¡El misil detonó una mina enemiga! El rival gana un turno extra.",
                                    "Daño Colateral", true);
                            }
                        }
                    }

                    if (turnoBatalla == 1) misilesDisponiblesJ1--;
                    else misilesDisponiblesJ2--;

                    // >>> SFX: Impactos del misil
                    if (aciertos > 0) GestorSonido.ReproducirSFX("sfx_impacto.mp3");
                    else GestorSonido.ReproducirSFX("sfx_agua.mp3");

                    MostrarAlertaMilitar($"¡MISIL DETONADO! Impactos a barcos enemigos: {aciertos}", "Reporte de Artillería");

                    DibujarTablero(dgv, matrizObjetivo, true);
                    ActualizarLabels(matrizObjetivo, null);
                    ActualizarTienda();

                    if (VerificarVictoria(matrizObjetivo)) { TerminarGuerra(); return; }

                    FinalizarTurno();
                    return;
                }

                // ── Disparo Básico ────────────────────────────────────────
                if (turnoBatalla == 1 && dgv != dgvJugador2) return;
                if (turnoBatalla == 2 && dgv != dgvJugador1) return;

                int[,] objetivo = (turnoBatalla == 1) ? matrizJ2 : matrizJ1;
                ProcesarDisparo(objetivo, e.RowIndex, e.ColumnIndex, dgv);
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            bool[] colocadosActual = (turnoColocacion == 1) ? barcosColocadosJ1 : barcosColocadosJ2;
            if (!colocadosActual.All(c => c == true))
            {
                MostrarAlertaMilitar("¡Aún no has desplegado toda tu flota!", "Atención");
                return;
            }

            if (modo1v1 && turnoColocacion == 1)
            {
                lblEstado.Text = "¡Jugador 2, es tu turno de colocar barcos!";
                turnoColocacion = 2;
                DibujarTablero(dgvJugador1, matrizJ1, true);
                ActualizarLabels(matrizJ2, barcosColocadosJ2);
                btnAutoJ1.Visible = false;
                btnAutoJ2.Visible = true;
                if (cmbSeleccionBarco != null) cmbSeleccionBarco.SelectedIndex = 0;
            }
            else
            {
                // >>> BGM: Música de batalla al confirmar flotas
                GestorSonido.ReproducirBGM("musica_batalla.mp3");

                faseBatalla = true; turnoBatalla = 1;
                btnAutoJ1.Visible = btnAutoJ2.Visible = btnRotar.Visible = false;
                btnReacomodar.Visible = btnConfirmar.Visible = false;
                if (cmbSeleccionBarco != null) cmbSeleccionBarco.Visible = false;

                pnlRadar.Visible = true;
                pnlMercado.Visible = true;
                cmbSelectorArmas.Visible = true;
                lblInventario.Visible = true;
                timerRadar.Start();

                MostrarAlertaMilitar(
                    "¡Todas las flotas han sido desplegadas!\n\nJugador 1, te toca iniciar el ataque.",
                    "¡A LA BATALLA!");

                PrepararTablerosParaTurno();
                ActualizarLabels(matrizJ1, null);
                ActualizarTienda();
            }
        }

        // ====================================================================
        // FUNCIONES GRÁFICAS Y UTILIDADES
        // ====================================================================
        private void ConfigurarRadarMilitar(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ReadOnly = true;
            dgv.ScrollBars = ScrollBars.None;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgv.MultiSelect = false;

            dgv.BackgroundColor = Color.Black;
            dgv.GridColor = Color.LimeGreen;
            dgv.DefaultCellStyle.BackColor = Color.Black;
            dgv.DefaultCellStyle.ForeColor = Color.LimeGreen;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkGreen;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkOliveGreen;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.RowHeadersDefaultCellStyle.BackColor = Color.DarkOliveGreen;
            dgv.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Clear(); dgv.Rows.Clear();
            char letra = 'A';
            for (int i = 0; i < tamanoTablero; i++)
            {
                dgv.Columns.Add($"col{i}", letra.ToString());
                dgv.Columns[i].Width = tamanoCelda;
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                letra++;
            }
            for (int i = 0; i < tamanoTablero; i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].Height = tamanoCelda;
                dgv.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
            dgv.RowHeadersWidth = 50;
            dgv.Width = (tamanoCelda * tamanoTablero) + dgv.RowHeadersWidth + 3;
            dgv.Height = (tamanoCelda * tamanoTablero) + dgv.ColumnHeadersHeight + 3;
        }

        private void DibujarTablero(DataGridView dgv, int[,] matriz, bool ocultarBarcos)
        {
            for (int f = 0; f < tamanoTablero; f++)
            {
                for (int c = 0; c < tamanoTablero; c++)
                {
                    int estado = matriz[f, c];
                    DataGridViewCell celda = dgv.Rows[f].Cells[c];

                    if (estado == 0)
                    { celda.Style.BackColor = Color.Black; celda.Value = ""; }
                    else if (estado >= 10 && estado <= 14)
                    { celda.Style.BackColor = ocultarBarcos ? Color.Black : Color.Gray; celda.Value = ocultarBarcos ? "" : "🚢"; }
                    else if (estado == 20)
                    { celda.Style.BackColor = ocultarBarcos ? Color.Black : Color.DarkOrange; celda.Value = ocultarBarcos ? "" : "💣"; }
                    else if (estado == 2)
                    { celda.Style.BackColor = Color.DarkRed; celda.Value = "💥"; }
                    else if (estado == 21)
                    { celda.Style.BackColor = Color.DarkMagenta; celda.Value = "🎇"; }
                    else if (estado == 4)
                    { celda.Style.BackColor = Color.DarkGreen; celda.Value = "🔍"; }
                    else if (estado == 3)
                    {
                        if (nieblaDensaRestante > 0)
                        { celda.Style.BackColor = Color.Black; celda.Value = ""; }
                        else
                        { celda.Style.BackColor = Color.DarkBlue; celda.Value = "🌊"; }
                    }
                }
            }
            dgv.ClearSelection();
        }

        private void PrepararTablerosParaTurno()
        {
            VerificarEstadoEvasion();
            ActualizarTienda();

            if (turnoBatalla == 1)
            {
                DibujarTablero(dgvJugador1, matrizJ1, false);
                DibujarTablero(dgvJugador2, matrizJ2, true);
                lblEstado.Text = "JUGADOR 1: Tu turno de atacar.";
            }
            else
            {
                DibujarTablero(dgvJugador1, matrizJ1, true);
                DibujarTablero(dgvJugador2, matrizJ2, false);
                lblEstado.Text = "JUGADOR 2: Tu turno de atacar.";
            }
        }

        private void CargarMenuBarcos()
        {
            if (cmbSeleccionBarco != null)
            {
                cmbSeleccionBarco.Items.Clear();
                for (int i = 0; i < flota.Length; i++)
                    cmbSeleccionBarco.Items.Add($"{nombresBarcos[i]} ({flota[i]} celdas)");
                cmbSeleccionBarco.SelectedIndex = 0;
            }
        }

        private void btnRotar_Click(object sender, EventArgs e)
        {
            esHorizontal = !esHorizontal;
            btnRotar.Text = esHorizontal ? "Girar: Horizontal" : "Girar: Vertical";
        }

        private void btnAutoJ1_Click(object sender, EventArgs e)
        {
            GenerarFlotaAleatoria(matrizJ1);
            for (int i = 0; i < 5; i++) barcosColocadosJ1[i] = true;
            DibujarTablero(dgvJugador1, matrizJ1, false);
            ActualizarLabels(matrizJ1, barcosColocadosJ1);
        }

        private void btnAutoJ2_Click(object sender, EventArgs e)
        {
            GenerarFlotaAleatoria(matrizJ2);
            for (int i = 0; i < 5; i++) barcosColocadosJ2[i] = true;
            DibujarTablero(dgvJugador2, matrizJ2, false);
            ActualizarLabels(matrizJ2, barcosColocadosJ2);
        }

        private void btnReacomodar_Click(object sender, EventArgs e)
        {
            int[,] matrizAfectada = (turnoColocacion == 1) ? matrizJ1 : matrizJ2;
            bool[] colocadosActual = (turnoColocacion == 1) ? barcosColocadosJ1 : barcosColocadosJ2;
            DataGridView dgv = (turnoColocacion == 1) ? dgvJugador1 : dgvJugador2;

            Array.Clear(matrizAfectada, 0, matrizAfectada.Length);
            Array.Clear(colocadosActual, 0, colocadosActual.Length);
            if (cmbSeleccionBarco.Items.Count > 0) cmbSeleccionBarco.SelectedIndex = 0;
            DibujarTablero(dgv, matrizAfectada, false);
            ActualizarLabels(matrizAfectada, colocadosActual);
        }

        private void ActualizarLabels(int[,] matriz, bool[] colocados)
        {
            if (!faseBatalla)
            {
                lblPortaaviones.Text = $"Portaaviones [5]: {(colocados[0] ? "Desplegado" : "Falta 1")}";
                lblAcorazado.Text = $"Acorazado [4]: {(colocados[1] ? "Desplegado" : "Falta 1")}";
                int subsColocados = (colocados[2] ? 1 : 0) + (colocados[3] ? 1 : 0);
                if (subsColocados == 0) lblSubmarinos.Text = "Submarinos [3]: Faltan 2";
                else if (subsColocados == 1) lblSubmarinos.Text = "Submarinos [3]: Falta 1";
                else lblSubmarinos.Text = "Submarinos [3]: Desplegados";
                lblPatrulla.Text = $"Patrulla [2]: {(colocados[4] ? "Desplegado" : "Falta 1")}";
            }
            else
            {
                lblPortaaviones.Text = $"Portaaviones: {VerificarEstadoBarco(matriz, 10)}";
                lblAcorazado.Text = $"Acorazado: {VerificarEstadoBarco(matriz, 11)}";
                int subsVivos = (VerificarEstadoBarco(matriz, 12) == "VIVO" ? 1 : 0)
                              + (VerificarEstadoBarco(matriz, 13) == "VIVO" ? 1 : 0);
                lblSubmarinos.Text = $"Submarinos: {subsVivos}/2 Vivos";
                lblPatrulla.Text = $"Patrulla: {VerificarEstadoBarco(matriz, 14)}";
            }
        }

        private string VerificarEstadoBarco(int[,] matriz, int idBarco)
        {
            foreach (int celda in matriz) if (celda == idBarco) return "VIVO";
            return "HUNDIDO 💥";
        }

        private bool SigueVivo(int[,] matriz, int idBarco)
        {
            foreach (int celda in matriz) if (celda == idBarco) return true;
            return false;
        }

        private bool VerificarVictoria(int[,] matriz)
        {
            foreach (int celda in matriz) if (celda >= 10 && celda <= 14) return false;
            return true;
        }

        private void GenerarFlotaAleatoria(int[,] matriz)
        {
            Array.Clear(matriz, 0, matriz.Length);
            Random rnd = new Random();
            for (int x = 0; x < flota.Length; x++)
            {
                bool colocado = false;
                int idBarco = 10 + x;
                while (!colocado)
                {
                    int f = rnd.Next(10);
                    int c = rnd.Next(10);
                    bool hor = rnd.Next(2) == 0;
                    if (PuedeColocarBarco(matriz, f, c, flota[x], hor))
                    {
                        for (int i = 0; i < flota[x]; i++)
                        {
                            if (hor) matriz[f, c + i] = idBarco;
                            else matriz[f + i, c] = idBarco;
                        }
                        colocado = true;
                    }
                }
            }
        }

        private bool PuedeColocarBarco(int[,] matriz, int f, int c, int t, bool h)
        {
            if (h)
            {
                if (c + t > 10) return false;
                for (int i = 0; i < t; i++)
                    if (matriz[f, c + i] != 0 && matriz[f, c + i] != 20) return false;
            }
            else
            {
                if (f + t > 10) return false;
                for (int i = 0; i < t; i++)
                    if (matriz[f + i, c] != 0 && matriz[f + i, c] != 20) return false;
            }
            return true;
        }

        // ====================================================================
        // NAVEGACIÓN ENTRE PRÁCTICAS
        // ====================================================================
        private void BtnPractica1_Click(object sender, EventArgs e)
        {
            PnlPracticas1.Visible = !PnlPracticas1.Visible;
            PnlPracticas2.Visible = false;
            PnlPracticas4.Visible = false;
        }

        private void BtnPractica2_Click(object sender, EventArgs e)
        {
            PnlPracticas2.Visible = !PnlPracticas2.Visible;
            PnlPracticas1.Visible = false;
            PnlPracticas4.Visible = false;
        }
    }

    
}

