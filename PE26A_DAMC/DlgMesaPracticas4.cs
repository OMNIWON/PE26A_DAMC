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
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    public partial class DlgMesaPracticas4 : Form
    {
        // ====================================================================
        // ENUM: ESTADO DEL JUEGO
        // ====================================================================
        private enum EstadoJuego
        {
            MenuPrincipal,
            SeleccionModo,
            SeleccionDificultad,
            ColocacionBarcos,
            EnBatalla,
            FinalizadoVictoria,
            FinalizadoDerrota
        }

        #region ══════════════════════════════════════════════════════════════
        // CONSTANTES GLOBALES
        #endregion

        // Tablero
        private const int TABLERO_TAMANIO = 10;
        private const int TAMANIO_CELDA = 30;
        private const int CANTIDAD_BARCOS = 5;

        // Economía
        private const int PRECIO_MINA = 30;
        private const int PRECIO_SONAR = 40;
        private const int PRECIO_MISIL = 80;
        private const int PUNTOS_IMPACTO = 10;
        private const int PUNTOS_HUNDIMIENTO = 40;

        // IDs de celdas
        private const int CELDA_VACIA = 0;
        private const int CELDA_BARCO_MIN = 10;
        private const int CELDA_BARCO_MAX = 14;
        private const int CELDA_IMPACTO = 2;
        private const int CELDA_AGUA = 3;
        private const int CELDA_SONAR = 4;
        private const int CELDA_MINA = 20;
        private const int CELDA_MINA_EXPLOTADA = 21;

        #region ══════════════════════════════════════════════════════════════
        // VARIABLES: ESTADO Y LÓGICA BASE
        #endregion

        private EstadoJuego estadoActual = EstadoJuego.MenuPrincipal;
        private int dificultadIA = 1;
        private int[,] matrizJ1 = new int[TABLERO_TAMANIO, TABLERO_TAMANIO];
        private int[,] matrizJ2 = new int[TABLERO_TAMANIO, TABLERO_TAMANIO];
        private readonly int[] flota = { 5, 4, 3, 3, 2 };
        private string[] nombresBarcos = { "Portaaviones", "Acorazado", "Submarino 1", "Submarino 2", "Patrulla" };
        private bool[] barcosColocadosJ1 = new bool[CANTIDAD_BARCOS];
        private bool[] barcosColocadosJ2 = new bool[CANTIDAD_BARCOS];
        private bool faseBatalla = false;
        private bool juegoActivo = false;
        private bool modo1v1 = false;
        private int turnoBatalla = 1;
        private int turnoColocacion = 1;
        private bool esHorizontal = true;
        private List<Point> objetivosIA = new List<Point>();

        #region ══════════════════════════════════════════════════════════════
        // VARIABLES: RADAR DINÁMICO
        // pnlRadar viene del Designer — NO se declara aquí
        #endregion

        private int fallosJ1 = 0;
        private int fallosJ2 = 0;
        private bool senalActiva = false;
        private bool esperandoRespuestaRadar = false;
        // ── pnlRadar → declarado en Designer.cs ──────────────────────────
        private System.Windows.Forms.PictureBox pbRadar;
        private Button btnAceptarSenal;
        private Button btnDenegarSenal;
        private System.Windows.Forms.Timer timerRadar;
        private int anguloRadar = 0;
        private bool parpadeoPuntoRojo = false;

        #region ══════════════════════════════════════════════════════════════
        // VARIABLES: ECONOMÍA, INVENTARIO Y HABILIDADES
        // pnlMercado, cmbSelectorArmas, lblInventario vienen del Designer
        #endregion

        private int puntosJ1 = 0;
        private int puntosJ2 = 0;
        private int turnosExtraJ1 = 0;
        private int turnosExtraJ2 = 0;

        private int minasDisponiblesJ1 = 0;
        private int minasDisponiblesJ2 = 0;
        private int misilesDisponiblesJ1 = 0;
        private int misilesDisponiblesJ2 = 0;
        private int sonaresDisponiblesJ1 = 0;
        private int sonaresDisponiblesJ2 = 0;

        // ── pnlMercado → declarado en Designer.cs ────────────────────────
        private Label lblPuntosActuales;
        private Label lblClimaGlobal;
        private Button btnComprarMina;
        private Button btnComprarMisil;
        private Button btnComprarSonar;
        // ── cmbSelectorArmas → declarado en Designer.cs ──────────────────
        // ── lblInventario    → declarado en Designer.cs ──────────────────

        #region ══════════════════════════════════════════════════════════════
        // VARIABLES: EVASIÓN Y CLIMA
        // btnManiobraEvasion viene del Designer
        #endregion

        // ── btnManiobraEvasion → declarado en Designer.cs ─────────────────
        private bool evasionUsadaJ1 = false;
        private bool evasionUsadaJ2 = false;

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

            this.FormClosing += DlgMesaPracticas4_FormClosing;
        }

        // ====================================================================
        // EVENTOS DE FORMULARIO
        // ====================================================================
        private void DlgMesaPracticas4_Load(object sender, EventArgs e)
        {
            // ── Solo configuración visual ── el juego NO arranca aquí ──────
            // Los DataGridView necesitan estar visibles para que el layout
            // calcule tamaños correctamente, por eso esto va en Load y no
            // en el constructor.
            ConfigurarRadarMilitar(dgvJugador1);
            ConfigurarRadarMilitar(dgvJugador2);
            AplicarDisenoVisual();

            // Dejar TODO apagado. El juego despierta solo cuando el usuario
            // presiona "Iniciar Misión" (btnIniciarJuego_Click → IniciarJuegoCompleto).
            ApagadoInicial();
        }

        private void DlgMesaPracticas4_FormClosing(object sender, FormClosingEventArgs e)
        {
            GestorSonido.DetenerBGM();
            GestorSonido.LimpiarTodo();
            if (timerRadar != null) { timerRadar.Stop(); timerRadar.Dispose(); }
            System.Diagnostics.Debug.WriteLine("[Mesa4] ✅ Limpieza completada al cerrar.");
        }

        // ====================================================================
        // MÉTODOS DE CONTROL DE JUEGO
        // ====================================================================

        private void DetenerJuego()
        {
            juegoActivo = false;

            faseBatalla = false;

            estadoActual = EstadoJuego.MenuPrincipal;

            esperandoRespuestaRadar = false;

            senalActiva = false;

            if (timerRadar != null)
            {
                timerRadar.Stop();
                timerRadar.Enabled = false;
            }

            pnlRadar.Visible = false;
            pnlMercado.Visible = false;
            cmbSelectorArmas.Visible = false;
            lblInventario.Visible = false;
            btnManiobraEvasion.Visible = false;

            objetivosIA.Clear();

            GestorSonido.DetenerBGM();

            Array.Clear(matrizJ1, 0, matrizJ1.Length);
            Array.Clear(matrizJ2, 0, matrizJ2.Length);

            DibujarTablero(dgvJugador1, matrizJ1, false);
            DibujarTablero(dgvJugador2, matrizJ2, false);

            lblEstado.Text = "Juego detenido.";
        }

        /// <summary>
        /// Estado inicial del formulario: todo apagado, sin juego activo.
        /// Solo se llama desde Load. El juego arranca desde IniciarJuegoCompleto.
        /// </summary>
        private void ApagadoInicial()
        {
            juegoActivo = false;
            faseBatalla = false;
            senalActiva = false;
            esperandoRespuestaRadar = false;
            estadoActual = EstadoJuego.MenuPrincipal;

            // Apagar timer si ya existe (por si Load se llama más de una vez)
            if (timerRadar != null)
            {
                timerRadar.Stop();
                timerRadar.Enabled = false;
            }

            // Ocultar controles de juego
            pnlRadar.Visible = false;
            pnlMercado.Visible = false;
            cmbSelectorArmas.Visible = false;
            lblInventario.Visible = false;
            btnManiobraEvasion.Visible = false;

            // Ocultar paneles de juego, mostrar pantalla de inicio
            if (pnlMenuModos != null) pnlMenuModos.Visible = false;
            if (pnlDificultad != null) pnlDificultad.Visible = false;
            if (pnlJuego != null) pnlJuego.Visible = false;
            if (pnlInicio != null)
            {
                pnlInicio.Visible = true;
                pnlInicio.BringToFront();
            }
        }

        private void LimpiarEstadoJuego()
        {
            puntosJ1 = puntosJ2 = 0;
            turnosExtraJ1 = turnosExtraJ2 = 0;
            minasDisponiblesJ1 = minasDisponiblesJ2 = 0;
            misilesDisponiblesJ1 = misilesDisponiblesJ2 = 0;
            sonaresDisponiblesJ1 = sonaresDisponiblesJ2 = 0;
            evasionUsadaJ1 = evasionUsadaJ2 = false;
            fallosJ1 = fallosJ2 = 0;
            contadorTurnosGlobales = 0;
            esperandoRespuestaRadar = false;
        }

        /// <summary>
        /// Apaga el motor del juego sin intentar redibujar tableros.
        /// Úsalo cuando vas a cambiar de panel (prácticas) para no provocar
        /// renders sobre controles que quedarán ocultos.
        /// </summary>
        private void ApagarMotorJuego()
        {
            juegoActivo = false;
            faseBatalla = false;
            senalActiva = false;
            esperandoRespuestaRadar = false;
            estadoActual = EstadoJuego.MenuPrincipal;

            if (timerRadar != null)
            {
                timerRadar.Stop();
                timerRadar.Enabled = false;
            }

            objetivosIA.Clear();

            pnlRadar.Visible = false;
            pnlMercado.Visible = false;
            cmbSelectorArmas.Visible = false;
            lblInventario.Visible = false;
            btnManiobraEvasion.Visible = false;

            GestorSonido.DetenerBGM();

            // Limpiar matrices en memoria (sin redibujar)
            Array.Clear(matrizJ1, 0, matrizJ1.Length);
            Array.Clear(matrizJ2, 0, matrizJ2.Length);
        }

        private void IniciarJuegoCompleto()
        {
            // ── Activar el motor ─────────────────────────────────────────────
            juegoActivo = true;
            estadoActual = EstadoJuego.MenuPrincipal;

            // Limpiar estado por si se inicia después de una partida anterior
            LimpiarEstadoJuego();
            objetivosIA.Clear();
            Array.Clear(matrizJ1, 0, matrizJ1.Length);
            Array.Clear(matrizJ2, 0, matrizJ2.Length);
            Array.Clear(barcosColocadosJ1, 0, barcosColocadosJ1.Length);
            Array.Clear(barcosColocadosJ2, 0, barcosColocadosJ2.Length);

            // Controles de juego apagados hasta que empiece la batalla
            pnlRadar.Visible = false;
            pnlMercado.Visible = false;
            cmbSelectorArmas.Visible = false;
            lblInventario.Visible = false;
            btnManiobraEvasion.Visible = false;

            if (timerRadar != null)
            {
                timerRadar.Stop();
                timerRadar.Enabled = false;
            }

            // Ocultar paneles de práctica para que no compitan con el juego
            if (PnlPracticas1 != null) PnlPracticas1.Visible = false;
            if (PnlPracticas2 != null) PnlPracticas2.Visible = false;

            GestorSonido.ReproducirBGM("musica_menu.mp3");

            // Ir al menú de selección de modo
            CambiarPantallaJuego(pnlMenuModos);
        }

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
        // CREACIÓN DE INTERFAZ DINÁMICA
        // ====================================================================

        /// <summary>
        /// Construye los controles internos del pnlMercado (que ya existe en el Designer).
        /// Solo añade los Labels y Buttons hijos — NO crea el panel ni lo añade a pnlJuego.
        /// </summary>
        private void ConstruirMercadoTáctico()
        {
            // pnlMercado ya existe (viene del Designer) — solo configuramos su apariencia

            pnlMercado.BackColor = Color.FromArgb(0, 0, 0);
            pnlMercado.BorderStyle = BorderStyle.None;
            pnlMercado.Visible = false;

            pnlMercado.Paint += (s, e) =>
            {
                Control pnl = (Control)s;
                Pen lapizVerde = new Pen(Color.FromArgb(50, 205, 50), 3);
                e.Graphics.DrawRectangle(lapizVerde, 0, 0, pnl.Width - 1, pnl.Height - 1);
            };

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
                Text = $"MINA(${PRECIO_MINA})",
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
                Text = $"MISIL DE ÁREA(${PRECIO_MISIL})",
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
                Text = $"SONAR 3x3 (${PRECIO_SONAR})",
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

            // cmbSelectorArmas ya existe en Designer — solo configuramos
            cmbSelectorArmas.BackColor = Color.DarkGreen;
            cmbSelectorArmas.ForeColor = Color.Black;
            cmbSelectorArmas.Font = new Font("Consolas", 11, FontStyle.Italic);
            cmbSelectorArmas.Visible = false;
            cmbSelectorArmas.Items.Add("Disparo Básico");
            cmbSelectorArmas.SelectedIndex = 0;

            // lblInventario ya existe en Designer — solo configuramos
            lblInventario.ForeColor = Color.Yellow;
            lblInventario.Font = new Font("Consolas", 10, FontStyle.Italic);
            lblInventario.Visible = false;
        }

        /// <summary>
        /// Configura btnManiobraEvasion (que ya existe en el Designer).
        /// Solo asigna el evento Click y deja Visible = false.
        /// </summary>
        private void ConstruirBotonEvasion()
        {
            // btnManiobraEvasion ya existe (viene del Designer) — solo conectamos el evento
            btnManiobraEvasion.BackColor = Color.DarkRed;
            btnManiobraEvasion.ForeColor = Color.White;
            btnManiobraEvasion.Visible = false;
            btnManiobraEvasion.Click += BtnManiobraEvasion_Click;
        }

        /// <summary>
        /// Construye los controles internos del pnlRadar (que ya existe en el Designer).
        /// Solo añade PictureBox y botones hijos — NO crea el panel ni lo añade a pnlJuego.
        /// </summary>
        private void ConstruirRadarDinamico()
        {
            // pnlRadar ya existe (viene del Designer) — solo configuramos
            pnlRadar.BackColor = Color.FromArgb(20, 20, 20);
            pnlRadar.BorderStyle = BorderStyle.FixedSingle;
            pnlRadar.Visible = false;

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
            GestorSonido.ReproducirSFX("sfx_radar.mp3");
            DesactivarSenalRadar();
            RevelarPistaIntercomunicador((turnoBatalla == 1) ? matrizJ2 : matrizJ1);
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
            for (int f = 0; f < TABLERO_TAMANIO; f++)
                for (int c = 0; c < TABLERO_TAMANIO; c++)
                    if (matrizEnemiga[f, c] >= CELDA_BARCO_MIN && matrizEnemiga[f, c] <= CELDA_BARCO_MAX)
                        celdasVivas.Add(new Point(f, c));

            if (celdasVivas.Count == 0) return;

            Random rnd = new Random();
            Point objetivo = celdasVivas[rnd.Next(celdasVivas.Count)];
            char letra = (char)('A' + objetivo.Y);
            int fila = objetivo.X + 1;
            string pista = (rnd.Next(2) == 0) ? $"en la Fila {fila}" : $"en la Columna {letra}";

            MostrarAlertaMilitar(
                $"📡 TRANSMISIÓN DESENCRIPTADA: Inteligencia detecta emisiones térmicas enemigas {pista}.",
                "Radar Intercomunicador");
        }

        // ====================================================================
        // LÓGICA DE LA TIENDA
        // ====================================================================
        private void ActualizarTienda()
        {
            int misPuntos = (turnoBatalla == 1) ? puntosJ1 : puntosJ2;
            int misMinas = (turnoBatalla == 1) ? minasDisponiblesJ1 : minasDisponiblesJ2;
            int misMisiles = (turnoBatalla == 1) ? misilesDisponiblesJ1 : misilesDisponiblesJ2;
            int misSonares = (turnoBatalla == 1) ? sonaresDisponiblesJ1 : sonaresDisponiblesJ2;

            lblPuntosActuales.Text = $"FONDOS J{turnoBatalla}: ${misPuntos}";
            lblInventario.Text = $"Inventario: Minas({misMinas}) | Misiles({misMisiles}) | Sonar({misSonares})";

            btnComprarMina.Enabled = (misPuntos >= PRECIO_MINA);
            btnComprarMisil.Enabled = (misPuntos >= PRECIO_MISIL);
            btnComprarSonar.Enabled = (misPuntos >= PRECIO_SONAR);

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

        private void ComprarItem(int precio, ref int puntos, ref int inventario, string nombreItem, string descripcion)
        {
            if (puntos >= precio)
            {
                puntos -= precio;
                inventario++;
                GestorSonido.ReproducirSFX("sfx_compra.mp3");
                ActualizarTienda();
                MostrarAlertaMilitar(descripcion, "Mercado Negro");
            }
        }

        private void BtnComprarMina_Click(object sender, EventArgs e)
        {
            if (turnoBatalla == 1)
                ComprarItem(PRECIO_MINA, ref puntosJ1, ref minasDisponiblesJ1, "Mina",
                    "Mina Marina comprada. Selecciónala en armas y haz clic en TU TABLERO para esconderla.");
            else
                ComprarItem(PRECIO_MINA, ref puntosJ2, ref minasDisponiblesJ2, "Mina",
                    "Mina Marina comprada. Selecciónala en armas y haz clic en TU TABLERO para esconderla.");
        }

        private void BtnComprarMisil_Click(object sender, EventArgs e)
        {
            if (turnoBatalla == 1)
                ComprarItem(PRECIO_MISIL, ref puntosJ1, ref misilesDisponiblesJ1, "Misil",
                    "Misil de Área listo. Impacta una celda y las 4 que la rodean.");
            else
                ComprarItem(PRECIO_MISIL, ref puntosJ2, ref misilesDisponiblesJ2, "Misil",
                    "Misil de Área listo. Impacta una celda y las 4 que la rodean.");
        }

        private void BtnComprarSonar_Click(object sender, EventArgs e)
        {
            if (turnoBatalla == 1)
                ComprarItem(PRECIO_SONAR, ref puntosJ1, ref sonaresDisponiblesJ1, "Sonar",
                    "Escáner Sonar activo. Revela si hay barco enemigo en área 3x3.");
            else
                ComprarItem(PRECIO_SONAR, ref puntosJ2, ref sonaresDisponiblesJ2, "Sonar",
                    "Escáner Sonar activo. Revela si hay barco enemigo en área 3x3.");
        }

        // ====================================================================
        // CLIMA DINÁMICO
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
            if (contadorTurnosGlobales % 6 != 0) { ActualizarClima(); return; }

            Random rnd = new Random();
            int evento = rnd.Next(100);

            if (evento < 30)
            {
                tormentaMagneticaRestante = 2;
                MostrarAlertaMilitar(
                    "Una tormenta magnética ha entrado a la zona. Los radares estarán inoperativos 2 turnos.",
                    "Alerta Climática", true);
            }
            else if (evento < 60)
            {
                nieblaDensaRestante = 2;
                MostrarAlertaMilitar(
                    "Niebla densa cubre el mar. Los fallos al agua no serán visibles temporalmente.",
                    "Alerta Climática");
            }
            ActualizarClima();
        }

        // ====================================================================
        // EVASIÓN DE EMERGENCIA (SUBMARINO)
        // ====================================================================
        private void VerificarEstadoEvasion()
        {
            int[,] miMatriz = (turnoBatalla == 1) ? matrizJ1 : matrizJ2;
            bool yaUsada = (turnoBatalla == 1) ? evasionUsadaJ1 : evasionUsadaJ2;

            List<int> barcosVivos = new List<int>();
            foreach (int celda in miMatriz)
                if (celda >= CELDA_BARCO_MIN && celda <= CELDA_BARCO_MAX && !barcosVivos.Contains(celda))
                    barcosVivos.Add(celda);

            btnManiobraEvasion.Visible =
                (barcosVivos.Count == 1 && (barcosVivos[0] == 12 || barcosVivos[0] == 13) && !yaUsada);
        }

        private void BtnManiobraEvasion_Click(object sender, EventArgs e)
        {
            int[,] miMatriz = (turnoBatalla == 1) ? matrizJ1 : matrizJ2;
            int idSubmarino = SigueVivo(miMatriz, 12) ? 12 : (SigueVivo(miMatriz, 13) ? 13 : -1);

            if (idSubmarino == -1) return;

            for (int f = 0; f < TABLERO_TAMANIO; f++)
                for (int c = 0; c < TABLERO_TAMANIO; c++)
                    if (miMatriz[f, c] == idSubmarino) miMatriz[f, c] = CELDA_VACIA;

            Random rnd = new Random();
            bool reubicado = false;
            while (!reubicado)
            {
                int f = rnd.Next(TABLERO_TAMANIO);
                int c = rnd.Next(TABLERO_TAMANIO);
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

            MostrarAlertaMilitar("Inmersión ejecutada. El submarino ha reubicado su posición.", "Maniobra Táctica", true);
            if (turnoBatalla == 1) evasionUsadaJ1 = true; else evasionUsadaJ2 = true;
            btnManiobraEvasion.Visible = false;
            DibujarTablero((turnoBatalla == 1) ? dgvJugador1 : dgvJugador2, miMatriz, false);
        }

        // ====================================================================
        // NAVEGACIÓN PRINCIPAL
        // ====================================================================
        private void btnIniciarJuego_Click(object sender, EventArgs e)
        {
            IniciarJuegoCompleto();
        }

        private void btnSalirJuego_Click(object sender, EventArgs e)
        {
            GestorSonido.DetenerBGM();
            estadoActual = EstadoJuego.MenuPrincipal;
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
            estadoActual = EstadoJuego.SeleccionDificultad;
            CambiarPantallaJuego(pnlDificultad);
        }

        private void btnVolverMenu_Click(object sender, EventArgs e) =>
            CambiarPantallaJuego(pnlMenuModos);

        private void btnFacil_Click(object sender, EventArgs e) { dificultadIA = 0; IniciarFaseColocacion(); }
        private void btnExperto_Click(object sender, EventArgs e) { dificultadIA = 1; IniciarFaseColocacion(); }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            DetenerJuego();
            CambiarPantallaJuego(pnlInicio);
        }

        // ====================================================================
        // MOTOR DE JUEGO — ARRANQUE
        // ====================================================================
        private void IniciarFaseColocacion()
        {
            GestorSonido.ReproducirBGM("musica_acomodar.mp3");
            estadoActual = EstadoJuego.ColocacionBarcos;
            CambiarPantallaJuego(pnlJuego);

            faseBatalla = false;
            turnoColocacion = 1;
            objetivosIA.Clear();

            LimpiarEstadoJuego();
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
                for (int i = 0; i < CANTIDAD_BARCOS; i++) barcosColocadosJ2[i] = true;
                DibujarTablero(dgvJugador2, matrizJ2, true);
            }
            // Forzar posición DESPUÉS del layout
            this.BeginInvoke(new Action(() =>
            {
                pnlMercado.Location = new System.Drawing.Point(260, 0); // tu Y deseada
                lblInventario.Location = new System.Drawing.Point(446, 480); // tu Y deseada
                this.pnlRadar.Location = new System.Drawing.Point(905, 213);
                cmbSelectorArmas.Location = new System.Drawing.Point(910, 120);

            }));
        }

        // ====================================================================
        // MOTOR DE JUEGO — DISPAROS
        // ====================================================================
        private void ProcesarDisparo(int[,] matriz, int f, int c, DataGridView dgv)
        {
            int estadoCelda = matriz[f, c];
            if (estadoCelda == CELDA_IMPACTO || estadoCelda == CELDA_AGUA ||
                estadoCelda == CELDA_MINA_EXPLOTADA || estadoCelda == CELDA_SONAR) return;

            bool huboAcierto = false;
            string mensaje = "";

            if (estadoCelda == CELDA_MINA)
            {
                GestorSonido.ReproducirSFX("sfx_mina.mp3");
                matriz[f, c] = CELDA_MINA_EXPLOTADA;
                MostrarAlertaMilitar("¡BOOOM! ¡Has impactado una mina enemiga! Pierdes tu turno.", "¡TRAMPA!", true);
                if (turnoBatalla == 1) turnosExtraJ2++; else turnosExtraJ1++;
                DibujarTablero(dgv, matriz, true);
                FinalizarTurno();
                return;
            }

            if (estadoCelda >= CELDA_BARCO_MIN && estadoCelda <= CELDA_BARCO_MAX)
            {
                matriz[f, c] = CELDA_IMPACTO;
                huboAcierto = true;

                if (turnoBatalla == 1) { fallosJ1 = 0; puntosJ1 += PUNTOS_IMPACTO; }
                else { fallosJ2 = 0; puntosJ2 += PUNTOS_IMPACTO; }

                DesactivarSenalRadar();

                if (!SigueVivo(matriz, estadoCelda))
                {
                    GestorSonido.ReproducirSFX("sfx_hundido.mp3");
                    if (turnoBatalla == 1) puntosJ1 += PUNTOS_HUNDIMIENTO;
                    else puntosJ2 += PUNTOS_HUNDIMIENTO;
                    mensaje = $"¡HUNDIDO! Destruiste el {nombresBarcos[estadoCelda - CELDA_BARCO_MIN]}. ¡Tienes un tiro extra!";
                }
                else
                {
                    GestorSonido.ReproducirSFX("sfx_impacto.mp3");
                    mensaje = "¡IMPACTO CONFIRMADO! ¡Tienes un tiro extra!";
                }
            }
            else
            {
                GestorSonido.ReproducirSFX("sfx_agua.mp3");
                matriz[f, c] = CELDA_AGUA;
                if (turnoBatalla == 1) fallosJ1++; else fallosJ2++;
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
                    if (turnoBatalla == 1) turnosExtraJ1--; else turnosExtraJ2--;
                    MostrarAlertaMilitar(mensaje + " Fallo, PERO tienes turno extra por mina enemiga.", "Estrategia");
                    return;
                }

                int misFallos = (turnoBatalla == 1) ? fallosJ1 : fallosJ2;
                if (misFallos >= 3 && !senalActiva && tormentaMagneticaRestante == 0)
                {
                    MostrarAlertaMilitar(mensaje + " ⚠️ ¡SEÑAL DE RADAR DETECTADA.", "Reporte de Artillería", true);
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
            if (!juegoActivo) return;

            TirarDadosClimaticos();

            if (modo1v1)
            {
                await Task.Delay(500);
                turnoBatalla = (turnoBatalla == 1) ? 2 : 1;
                pbRadar.Invalidate();
                MostrarAlertaMilitar(
                    $"Cambio de turno. Jugador {turnoBatalla}, siéntate al mando.",
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
            if (!juegoActivo) return;

            Application.DoEvents();
            System.Threading.Thread.Sleep(600);

            Random rnd = new Random();
            bool disparoValido = false;
            int f = 0, c = 0;

            if (dificultadIA == 1 && objetivosIA.Count > 0)
            {
                Point p = objetivosIA[0]; objetivosIA.RemoveAt(0);
                f = p.X; c = p.Y;
                if (f >= 0 && f < TABLERO_TAMANIO && c >= 0 && c < TABLERO_TAMANIO &&
                    matrizJ1[f, c] != CELDA_IMPACTO && matrizJ1[f, c] != CELDA_AGUA &&
                    matrizJ1[f, c] != CELDA_MINA_EXPLOTADA && matrizJ1[f, c] != CELDA_SONAR)
                    disparoValido = true;
            }

            if (!disparoValido)
            {
                while (!disparoValido)
                {
                    f = rnd.Next(TABLERO_TAMANIO); c = rnd.Next(TABLERO_TAMANIO);
                    if (matrizJ1[f, c] != CELDA_IMPACTO && matrizJ1[f, c] != CELDA_AGUA &&
                        matrizJ1[f, c] != CELDA_MINA_EXPLOTADA && matrizJ1[f, c] != CELDA_SONAR)
                        disparoValido = true;
                }
            }

            int celda = matrizJ1[f, c];
            bool acierto = false;

            if (celda == CELDA_MINA)
            {
                GestorSonido.ReproducirSFX("sfx_mina.mp3");
                matrizJ1[f, c] = CELDA_MINA_EXPLOTADA;
                MostrarAlertaMilitar("¡La PC ha detonado tu mina! Pierde su turno.", "¡BOOM!", true);
                turnosExtraJ1++;
                DibujarTablero(dgvJugador1, matrizJ1, false);
                turnoBatalla = 1;
                PrepararTablerosParaTurno();
                return;
            }

            if (celda >= CELDA_BARCO_MIN && celda <= CELDA_BARCO_MAX)
            {
                matrizJ1[f, c] = CELDA_IMPACTO;
                acierto = true;
                if (dificultadIA == 1) AgregarObjetivosA_MemoriaIA(f, c);

                if (!SigueVivo(matrizJ1, celda))
                {
                    GestorSonido.ReproducirSFX("sfx_hundido.mp3");
                    MostrarAlertaMilitar($"¡La PC ha hundido tu {nombresBarcos[celda - CELDA_BARCO_MIN]}!", "Ataque Enemigo", true);
                }
                else
                {
                    GestorSonido.ReproducirSFX("sfx_impacto.mp3");
                    MostrarAlertaMilitar("La PC ha impactado uno de tus barcos.", "Ataque Enemigo");
                }
            }
            else
            {
                GestorSonido.ReproducirSFX("sfx_agua.mp3");
                matrizJ1[f, c] = CELDA_AGUA;
                MostrarAlertaMilitar("La PC disparó al agua. Ahora es tu turno.", "Ataque Enemigo");
            }

            DibujarTablero(dgvJugador1, matrizJ1, false);
            ActualizarLabels(matrizJ1, null);

            if (VerificarVictoria(matrizJ1)) { TerminarGuerra(); return; }

            if (!acierto) { turnoBatalla = 1; pbRadar.Invalidate(); PrepararTablerosParaTurno(); }
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
                if (nFila >= 0 && nFila < TABLERO_TAMANIO && nCol >= 0 && nCol < TABLERO_TAMANIO &&
                    matrizJ1[nFila, nCol] != CELDA_IMPACTO && matrizJ1[nFila, nCol] != CELDA_AGUA)
                    objetivosIA.Insert(0, new Point(nFila, nCol));
            }
        }

        private void TerminarGuerra()
        {
            DetenerJuego();
            estadoActual = (turnoBatalla == 1) ? EstadoJuego.FinalizadoVictoria : EstadoJuego.FinalizadoDerrota;

            string ganador = (turnoBatalla == 1)
                ? "¡JUGADOR 1 GANA LA GUERRA!"
                : (modo1v1 ? "¡JUGADOR 2 GANA LA GUERRA!" : "¡LA PC GANA LA GUERRA!");

            lblEstado.Text = ganador;

            btnAutoJ1.Visible = btnAutoJ2.Visible = btnRotar.Visible =
            btnReacomodar.Visible = btnConfirmar.Visible = false;
            if (cmbSeleccionBarco != null) cmbSeleccionBarco.Visible = false;

            MostrarAlertaMilitar(ganador, "FIN DEL JUEGO", true);
        }

        // ====================================================================
        // EVENTOS: CLIC EN TABLERO
        // ====================================================================
        private void Tablero_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!juegoActivo) return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            DataGridView dgv = (DataGridView)sender;

            if (faseBatalla && esperandoRespuestaRadar)
            {
                MostrarAlertaMilitar("¡Atienda la señal del radar antes de continuar!", "Radar Activo", true);
                return;
            }

            if (!faseBatalla) { ManejarColocacion(dgv, e); }
            else { ManejarDisparo(dgv, e); }
        }

        private void ManejarColocacion(DataGridView dgv, DataGridViewCellMouseEventArgs e)
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
            int idBarco = CELDA_BARCO_MIN + indiceBarco;

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

        private void ManejarDisparo(DataGridView dgv, DataGridViewCellMouseEventArgs e)
        {
            string arma = cmbSelectorArmas.SelectedItem?.ToString() ?? "Disparo Básico";

            if (arma == "Colocar Mina") { ManejarColocacionMina(dgv, e); }
            else if (arma == "Escáner Sonar") { ManejarSonar(dgv, e); }
            else if (arma == "Misil de Área") { ManejarMisil(dgv, e); }
            else { ManejarDisparoBasico(dgv, e); }
        }

        private void ManejarColocacionMina(DataGridView dgv, DataGridViewCellMouseEventArgs e)
        {
            if (turnoBatalla == 1 && dgv != dgvJugador1) { MostrarAlertaMilitar("Coloca la mina en TU tablero.", "Error"); return; }
            if (turnoBatalla == 2 && dgv != dgvJugador2) { MostrarAlertaMilitar("Coloca la mina en TU tablero.", "Error"); return; }

            int[,] miMatriz = (turnoBatalla == 1) ? matrizJ1 : matrizJ2;
            if (miMatriz[e.RowIndex, e.ColumnIndex] != CELDA_VACIA)
            { MostrarAlertaMilitar("Celda ocupada. Busca agua libre.", "Error"); return; }

            miMatriz[e.RowIndex, e.ColumnIndex] = CELDA_MINA;
            if (turnoBatalla == 1) minasDisponiblesJ1--; else minasDisponiblesJ2--;

            MostrarAlertaMilitar("Mina colocada. Ahora realiza tu disparo a la flota enemiga.", "Estrategia");
            DibujarTablero(dgv, miMatriz, false);
            cmbSelectorArmas.SelectedIndex = 0;
            ActualizarTienda();
        }

        private void ManejarSonar(DataGridView dgv, DataGridViewCellMouseEventArgs e)
        {
            if (turnoBatalla == 1 && dgv != dgvJugador2) { MostrarAlertaMilitar("Apunta al tablero enemigo.", "Error"); return; }
            if (turnoBatalla == 2 && dgv != dgvJugador1) { MostrarAlertaMilitar("Apunta al tablero enemigo.", "Error"); return; }

            int[,] matrizObjetivo = (turnoBatalla == 1) ? matrizJ2 : matrizJ1;
            bool detectado = false;

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    int nf = e.RowIndex + i, nc = e.ColumnIndex + j;
                    if (nf < 0 || nf >= TABLERO_TAMANIO || nc < 0 || nc >= TABLERO_TAMANIO) continue;
                    int celda = matrizObjetivo[nf, nc];
                    if (celda >= CELDA_BARCO_MIN && celda <= CELDA_BARCO_MAX) detectado = true;
                    else if (celda == CELDA_VACIA) matrizObjetivo[nf, nc] = CELDA_SONAR;
                }

            if (turnoBatalla == 1) sonaresDisponiblesJ1--; else sonaresDisponiblesJ2--;

            GestorSonido.ReproducirSFX("sfx_radar.mp3");
            DibujarTablero(dgv, matrizObjetivo, true);
            MostrarAlertaMilitar(
                detectado ? "¡SEÑALES TÉRMICAS! Hay presencia naval enemiga en el área escaneada."
                          : "El sonar no detecta actividad en este sector.",
                "Sonar Satelital", detectado);
            ActualizarTienda();
            FinalizarTurno();
        }

        private void ManejarMisil(DataGridView dgv, DataGridViewCellMouseEventArgs e)
        {
            if (turnoBatalla == 1 && dgv != dgvJugador2) { MostrarAlertaMilitar("Apunta al tablero enemigo.", "Error"); return; }
            if (turnoBatalla == 2 && dgv != dgvJugador1) { MostrarAlertaMilitar("Apunta al tablero enemigo.", "Error"); return; }

            int[,] matrizObjetivo = (turnoBatalla == 1) ? matrizJ2 : matrizJ1;
            int[] dirF = { 0, -1, 1, 0, 0 };
            int[] dirC = { 0, 0, 0, -1, 1 };
            int aciertos = 0;

            for (int i = 0; i < 5; i++)
            {
                int nf = e.RowIndex + dirF[i], nc = e.ColumnIndex + dirC[i];
                if (nf < 0 || nf >= TABLERO_TAMANIO || nc < 0 || nc >= TABLERO_TAMANIO) continue;
                int celda = matrizObjetivo[nf, nc];

                if (celda >= CELDA_BARCO_MIN && celda <= CELDA_BARCO_MAX)
                {
                    matrizObjetivo[nf, nc] = CELDA_IMPACTO;
                    aciertos++;
                    if (turnoBatalla == 1) puntosJ1 += PUNTOS_IMPACTO; else puntosJ2 += PUNTOS_IMPACTO;
                    if (!SigueVivo(matrizObjetivo, celda))
                        if (turnoBatalla == 1) puntosJ1 += PUNTOS_HUNDIMIENTO; else puntosJ2 += PUNTOS_HUNDIMIENTO;
                }
                else if (celda == CELDA_VACIA || celda == CELDA_SONAR)
                    matrizObjetivo[nf, nc] = CELDA_AGUA;
                else if (celda == CELDA_MINA)
                {
                    matrizObjetivo[nf, nc] = CELDA_MINA_EXPLOTADA;
                    if (turnoBatalla == 1) turnosExtraJ2++; else turnosExtraJ1++;
                    GestorSonido.ReproducirSFX("sfx_mina.mp3");
                    MostrarAlertaMilitar("¡El misil detonó una mina! El rival gana un turno extra.", "Daño Colateral", true);
                }
            }

            if (turnoBatalla == 1) misilesDisponiblesJ1--; else misilesDisponiblesJ2--;

            GestorSonido.ReproducirSFX(aciertos > 0 ? "sfx_impacto.mp3" : "sfx_agua.mp3");
            MostrarAlertaMilitar($"¡MISIL DETONADO! Impactos: {aciertos}", "Reporte de Artillería");

            DibujarTablero(dgv, matrizObjetivo, true);
            ActualizarLabels(matrizObjetivo, null);
            ActualizarTienda();

            if (VerificarVictoria(matrizObjetivo)) { TerminarGuerra(); return; }
            FinalizarTurno();
        }

        private void ManejarDisparoBasico(DataGridView dgv, DataGridViewCellMouseEventArgs e)
        {
            if (turnoBatalla == 1 && dgv != dgvJugador2) return;
            if (turnoBatalla == 2 && dgv != dgvJugador1) return;

            int[,] objetivo = (turnoBatalla == 1) ? matrizJ2 : matrizJ1;
            ProcesarDisparo(objetivo, e.RowIndex, e.ColumnIndex, dgv);
        }

        // ====================================================================
        // BOTONES DE COLOCACIÓN
        // ====================================================================
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
                GestorSonido.ReproducirBGM("musica_batalla.mp3");
                estadoActual = EstadoJuego.EnBatalla;
                faseBatalla = true;
                turnoBatalla = 1;

                btnAutoJ1.Visible = btnAutoJ2.Visible = btnRotar.Visible =
                btnReacomodar.Visible = btnConfirmar.Visible = false;
                if (cmbSeleccionBarco != null) cmbSeleccionBarco.Visible = false;

                pnlRadar.Visible = true;
                pnlMercado.Visible = true;
                cmbSelectorArmas.Visible = true;
                lblInventario.Visible = true;
                timerRadar.Start();

                MostrarAlertaMilitar("¡Todas las flotas desplegadas!\n\nJugador 1, inicia el ataque.", "¡A LA BATALLA!");
                PrepararTablerosParaTurno();
                ActualizarLabels(matrizJ1, null);
                ActualizarTienda();
            }
            // Forzar posición DESPUÉS del layout
            this.BeginInvoke(new Action(() =>
            {
                pnlMercado.Location = new System.Drawing.Point(260, 0); // tu Y deseada
                lblInventario.Location = new System.Drawing.Point(446, 480); // tu Y deseada
                this.pnlRadar.Location = new System.Drawing.Point(905, 213);
                cmbSelectorArmas.Location = new System.Drawing.Point(910, 120);


            }));
        }

        private void btnRotar_Click(object sender, EventArgs e)
        {
            esHorizontal = !esHorizontal;
            btnRotar.Text = esHorizontal ? "Girar: Horizontal" : "Girar: Vertical";
        }

        private void btnAutoJ1_Click(object sender, EventArgs e)
        {
            GenerarFlotaAleatoria(matrizJ1);
            for (int i = 0; i < CANTIDAD_BARCOS; i++) barcosColocadosJ1[i] = true;
            DibujarTablero(dgvJugador1, matrizJ1, false);
            ActualizarLabels(matrizJ1, barcosColocadosJ1);
        }

        private void btnAutoJ2_Click(object sender, EventArgs e)
        {
            GenerarFlotaAleatoria(matrizJ2);
            for (int i = 0; i < CANTIDAD_BARCOS; i++) barcosColocadosJ2[i] = true;
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

        // ====================================================================
        // GRÁFICOS Y UTILIDADES
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
            for (int i = 0; i < TABLERO_TAMANIO; i++)
            {
                dgv.Columns.Add($"col{i}", letra.ToString());
                dgv.Columns[i].Width = TAMANIO_CELDA;
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                letra++;
            }
            for (int i = 0; i < TABLERO_TAMANIO; i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].Height = TAMANIO_CELDA;
                dgv.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
            dgv.RowHeadersWidth = 50;
            dgv.Width = (TAMANIO_CELDA * TABLERO_TAMANIO) + dgv.RowHeadersWidth + 3;
            dgv.Height = (TAMANIO_CELDA * TABLERO_TAMANIO) + dgv.ColumnHeadersHeight + 3;
        }

        private void DibujarTablero(DataGridView dgv, int[,] matriz, bool ocultarBarcos)
        {
            for (int f = 0; f < TABLERO_TAMANIO; f++)
                for (int c = 0; c < TABLERO_TAMANIO; c++)
                {
                    int estado = matriz[f, c];
                    DataGridViewCell celda = dgv.Rows[f].Cells[c];

                    if (estado == CELDA_VACIA)
                    { celda.Style.BackColor = Color.Black; celda.Value = ""; }
                    else if (estado >= CELDA_BARCO_MIN && estado <= CELDA_BARCO_MAX)
                    {
                        celda.Style.BackColor = ocultarBarcos ? Color.Black : Color.Gray;
                        celda.Value = ocultarBarcos ? "" : "🚢";
                    }
                    else if (estado == CELDA_MINA)
                    {
                        celda.Style.BackColor = ocultarBarcos ? Color.Black : Color.DarkOrange;
                        celda.Value = ocultarBarcos ? "" : "💣";
                    }
                    else if (estado == CELDA_IMPACTO)
                    { celda.Style.BackColor = Color.DarkRed; celda.Value = "💥"; }
                    else if (estado == CELDA_MINA_EXPLOTADA)
                    { celda.Style.BackColor = Color.DarkMagenta; celda.Value = "🎇"; }
                    else if (estado == CELDA_SONAR)
                    { celda.Style.BackColor = Color.DarkGreen; celda.Value = "🔍"; }
                    else if (estado == CELDA_AGUA)
                    {
                        celda.Style.BackColor = nieblaDensaRestante > 0 ? Color.Black : Color.DarkBlue;
                        celda.Value = nieblaDensaRestante > 0 ? "" : "🌊";
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
            if (cmbSeleccionBarco == null) return;
            cmbSeleccionBarco.Items.Clear();
            for (int i = 0; i < flota.Length; i++)
                cmbSeleccionBarco.Items.Add($"{nombresBarcos[i]} ({flota[i]} celdas)");
            cmbSeleccionBarco.SelectedIndex = 0;
        }

        private void ActualizarLabels(int[,] matriz, bool[] colocados)
        {
            if (!faseBatalla)
            {
                lblPortaaviones.Text = $"Portaaviones [5]: {(colocados[0] ? "Desplegado" : "Falta 1")}";
                lblAcorazado.Text = $"Acorazado [4]: {(colocados[1] ? "Desplegado" : "Falta 1")}";
                int subs = (colocados[2] ? 1 : 0) + (colocados[3] ? 1 : 0);
                lblSubmarinos.Text = subs == 0 ? "Submarinos [3]: Faltan 2"
                                     : subs == 1 ? "Submarinos [3]: Falta 1"
                                                 : "Submarinos [3]: Desplegados";
                lblPatrulla.Text = $"Patrulla [2]: {(colocados[4] ? "Desplegado" : "Falta 1")}";
            }
            else
            {
                lblPortaaviones.Text = $"Portaaviones: {VerificarEstadoBarco(matriz, 10)}";
                lblAcorazado.Text = $"Acorazado: {VerificarEstadoBarco(matriz, 11)}";
                int subsVivos = (SigueVivo(matriz, 12) ? 1 : 0) + (SigueVivo(matriz, 13) ? 1 : 0);
                lblSubmarinos.Text = $"Submarinos: {subsVivos}/2 Vivos";
                lblPatrulla.Text = $"Patrulla: {VerificarEstadoBarco(matriz, 14)}";
            }
        }

        // ====================================================================
        // UTILIDADES PURAS
        // ====================================================================
        private string VerificarEstadoBarco(int[,] matriz, int idBarco) =>
            SigueVivo(matriz, idBarco) ? "VIVO" : "HUNDIDO 💥";

        private bool SigueVivo(int[,] matriz, int idBarco)
        {
            foreach (int celda in matriz) if (celda == idBarco) return true;
            return false;
        }

        private bool VerificarVictoria(int[,] matriz)
        {
            foreach (int celda in matriz)
                if (celda >= CELDA_BARCO_MIN && celda <= CELDA_BARCO_MAX) return false;
            return true;
        }

        private void GenerarFlotaAleatoria(int[,] matriz)
        {
            Array.Clear(matriz, 0, matriz.Length);
            Random rnd = new Random();
            for (int x = 0; x < flota.Length; x++)
            {
                bool colocado = false;
                int idBarco = CELDA_BARCO_MIN + x;
                while (!colocado)
                {
                    int f = rnd.Next(TABLERO_TAMANIO);
                    int c = rnd.Next(TABLERO_TAMANIO);
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
                if (c + t > TABLERO_TAMANIO) return false;
                for (int i = 0; i < t; i++)
                    if (matriz[f, c + i] != CELDA_VACIA && matriz[f, c + i] != CELDA_MINA) return false;
            }
            else
            {
                if (f + t > TABLERO_TAMANIO) return false;
                for (int i = 0; i < t; i++)
                    if (matriz[f + i, c] != CELDA_VACIA && matriz[f + i, c] != CELDA_MINA) return false;
            }
            return true;
        }

        // ====================================================================
        // NAVEGACIÓN ENTRE PRÁCTICAS
        // ====================================================================
        private void BtnPractica1_Click(object sender, EventArgs e)
        {
            // Apagar el motor sin redibujar tableros
            ApagarMotorJuego();
            LimpiarEstadoJuego();

            // Ocultar los paneles del juego explícitamente
            if (pnlInicio != null) pnlInicio.Visible = false;
            if (pnlMenuModos != null) pnlMenuModos.Visible = false;
            if (pnlDificultad != null) pnlDificultad.Visible = false;
            if (pnlJuego != null) pnlJuego.Visible = false;

            // Alternar panel de práctica 1, cerrar práctica 2
            PnlPracticas2.Visible = false;
            PnlPracticas1.Visible = !PnlPracticas1.Visible;

            if (PnlPracticas1.Visible)
                PnlPracticas1.BringToFront();
        }

        private void BtnPractica2_Click(object sender, EventArgs e)
        {
            // Apagar el motor sin redibujar tableros
            ApagarMotorJuego();
            LimpiarEstadoJuego();

            // Ocultar los paneles del juego explícitamente
            if (pnlInicio != null) pnlInicio.Visible = false;
            if (pnlMenuModos != null) pnlMenuModos.Visible = false;
            if (pnlDificultad != null) pnlDificultad.Visible = false;
            if (pnlJuego != null) pnlJuego.Visible = false;

            // Alternar panel de práctica 2, cerrar práctica 1
            PnlPracticas1.Visible = false;
            PnlPracticas2.Visible = !PnlPracticas2.Visible;

            if (PnlPracticas2.Visible)
                PnlPracticas2.BringToFront();
        }

    }
}