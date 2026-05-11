using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AxWMPLib;

namespace PE26A_DAMC
{
    /// <summary>
    /// Formulario de prácticas 2 - Programación Estructurada
    /// Implementa:
    /// - Reproductor de música vintage con Windows Media Player
    /// - Generación y manipulación de matrices dinámicas (DataGridView)
    /// - Búsqueda de palabras en matrices
    /// - Dibujo de gráficos en tiempo real
    /// 
    /// Autor: DAMC
    /// Fecha: 18/02/2026
    /// </summary>
    public partial class DlgMesaPracticas2 : Form
    {
        // ====================================================================
        // ATRIBUTOS DE LA CLASE
        // ====================================================================

        /// <summary>Variable auxiliar para operaciones en matrices</summary>
        private int renglonActual = 0;

        /// <summary>Lista de coordenadas para dibujo de líneas entre celdas</summary>
        private List<Point> coordenadas = new List<Point>();

        /// <summary>Generador de números aleatorios a nivel de clase (evita repetición de semillas)</summary>
        private Random generadorAleatorio = new Random();

        // --- SISTEMA DEL REPRODUCTOR VINTAGE ---
        /// <summary>Rutas completas de los archivos de audio</summary>
        private List<string> rutasCanciones = new List<string>();

        /// <summary>Nombres de canciones para mostrar en interfaz</summary>
        private List<string> nombresCanciones = new List<string>();

        /// <summary>Índice de la canción actualmente cargada</summary>
        private int indiceCancionActual = 0;

        /// <summary>Flag que indica si la música está en reproducción</summary>
        private bool estaReproduciendo = false;

        /// <summary>Flag para detectar si el usuario está arrastrando el slider de progreso</summary>
        private bool arrastrandoProgreso = false;

        /// <summary>Flag para detectar si el usuario está arrastrando el slider de volumen</summary>
        private bool arrastrandoVolumen = false;

        // ====================================================================
        // CONSTRUCTOR DEL FORMULARIO
        // ====================================================================

        /// <summary>
        /// Inicializa el formulario y configura todos los eventos y controles
        /// </summary>
        public DlgMesaPracticas2()
        {
            InitializeComponent();

            coordenadas = new List<Point>();

            // Suscripción a eventos de pintado en matrices
            DGVPRACTICA2P1.CellPainting += DgvMatrizCellPaint;
            DGVmesa2panel2.CellPainting += DgvMatrizCellPaint2;

            renglonActual = 0;

            try
            {
                // Configurar Windows Media Player
                RM2.uiMode = "none"; // Oculta el reproductor nativo

                // Evento que detecta cambios de estado (play, pausa, fin)
                RM2.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(RM2_PlayStateChange);

                CargarPlaylistPorDefecto();
                ActualizarPosicionSliderVolumen(70); // Volumen inicial al 70%
                CargarCancion(0, false); // Carga primera canción sin reproducir automáticamente
            }
            catch
            {
                lblCancion.Text = "ERROR INICIAL";
            }
        }

        // ====================================================================
        // LÓGICA DEL REPRODUCTOR VINTAGE
        // ====================================================================

        /// <summary>
        /// Carga la playlist por defecto con canciones predefinidas
        /// </summary>
        private void CargarPlaylistPorDefecto()
        {
            rutasCanciones.Clear();
            nombresCanciones.Clear();

            // Rutas físicas de las canciones (adaptadas según el sistema del usuario)
            rutasCanciones.Add(@"C:\Users\Lenovo\Music\Music\Doja Cat - So High (Official Video).mp3");
            rutasCanciones.Add(@"C:\Users\Lenovo\Music\Music\Conocí la Paz (Bolero).mp3");
            rutasCanciones.Add(@"C:\Users\Lenovo\Music\Music\Balada de la trompeta (1).mp3");

            // Nombres a mostrar en la interfaz
            nombresCanciones.Add("1. Doja Cat - So High");
            nombresCanciones.Add("2. CONOCI LA PAZ - Bolero");
            nombresCanciones.Add("3. BALADA DI TROMPETA - Vibes");
        }

        /// <summary>
        /// Carga una canción en el reproductor por su índice
        /// </summary>
        /// <param name="indice">Índice de la canción en la lista</param>
        /// <param name="autplay">Si es verdadero, inicia reproducción automáticamente</param>
        private void CargarCancion(int indice, bool autplay = true)
        {
            if (rutasCanciones.Count == 0) return;

            indiceCancionActual = indice;
            RM2.URL = rutasCanciones[indice];
            lblCancion.Text = nombresCanciones[indice];

            if (autplay || estaReproduciendo)
            {
                RM2.Ctlcontrols.play();
                estaReproduciendo = true;
            }
        }

        /// <summary>
        /// Evento que se dispara cuando cambia el estado de reproducción del media player
        /// </summary>
        private void RM2_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            // Estado 8 = Media Ended (La canción terminó)
            if (e.newState == 8)
            {
                // BeginInvoke evita conflictos entre el hilo del reproductor y la UI
                this.BeginInvoke(new Action(() => { TerminoCancion(); }));
            }
        }

        /// <summary>
        /// Maneja el final de la canción: reproduce la siguiente o solicita más canciones
        /// </summary>
        private void TerminoCancion()
        {
            int siguiente = indiceCancionActual + 1;

            if (siguiente < rutasCanciones.Count)
            {
                CargarCancion(siguiente);
            }
            else
            {
                RM2.Ctlcontrols.stop();
                estaReproduciendo = false;

                DialogResult respuesta = MessageBox.Show(
                    "¡Se acabó la música! ¿Quieres seguir escuchando unas rolitas más?",
                    "Playlist Terminada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    CargarCancionesDesdeArchivo();
                }
                else
                {
                    lblCancion.Text = "REPRODUCTOR DETENIDO";
                }
            }
        }

        /// <summary>
        /// Abre un diálogo para cargar nuevas canciones desde el sistema de archivos
        /// </summary>
        private void CargarCancionesDesdeArchivo()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Archivos de Audio|*.mp3;*.wav";
            ofd.Title = "Selecciona tus nuevas canciones retro";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                rutasCanciones.Clear();
                nombresCanciones.Clear();

                foreach (string archivo in ofd.FileNames)
                {
                    rutasCanciones.Add(archivo);
                    nombresCanciones.Add(System.IO.Path.GetFileNameWithoutExtension(archivo));
                }
                CargarCancion(0);
            }
        }

        // --- EVENTOS DE BOTONES DEL REPRODUCTOR ---

        /// <summary>
        /// Play/Pausa: alterna entre reproducción y pausa
        /// </summary>
        private void picPlay_Click(object sender, EventArgs e)
        {
            if (estaReproduciendo)
            {
                RM2.Ctlcontrols.pause();
                estaReproduciendo = false;
            }
            else
            {
                RM2.Ctlcontrols.play();
                estaReproduciendo = true;
            }
        }

        /// <summary>
        /// Detiene la reproducción y reinicia la posición
        /// </summary>
        private void picStop_Click(object sender, EventArgs e)
        {
            RM2.Ctlcontrols.stop();
            estaReproduciendo = false;
            picSliderProgreso.Left = 0; // Regresa el slider al inicio
        }

        /// <summary>
        /// Reproduce la siguiente canción en la playlist
        /// </summary>
        private void picSiguiente_Click(object sender, EventArgs e)
        {
            int siguiente = indiceCancionActual + 1;
            if (siguiente >= rutasCanciones.Count) siguiente = 0;
            CargarCancion(siguiente);
        }

        /// <summary>
        /// Reproduce la canción anterior en la playlist
        /// </summary>
        private void picAnterior_Click(object sender, EventArgs e)
        {
            int anterior = indiceCancionActual - 1;
            if (anterior < 0) anterior = rutasCanciones.Count - 1;
            CargarCancion(anterior);
        }

        // ====================================================================
        // SLIDERS PERSONALIZADOS (VOLUMEN Y PROGRESO)
        // ====================================================================

        private void picSliderVolumen_MouseDown(object sender, MouseEventArgs e)
            => arrastrandoVolumen = true;

        private void picSliderVolumen_MouseUp(object sender, MouseEventArgs e)
            => arrastrandoVolumen = false;

        /// <summary>
        /// Controla el movimiento del slider de volumen
        /// </summary>
        private void picSliderVolumen_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastrandoVolumen)
            {
                int nuevaX = picSliderVolumen.Left + e.X;
                nuevaX = Math.Max(0, Math.Min(nuevaX, pnlFondoVolumen.Width - picSliderVolumen.Width));

                picSliderVolumen.Left = nuevaX;
                double porcentaje = (double)nuevaX / (pnlFondoVolumen.Width - picSliderVolumen.Width);
                RM2.settings.volume = (int)(porcentaje * 100);
            }
        }

        /// <summary>
        /// Actualiza la posición visual del slider de volumen según el porcentaje
        /// </summary>
        /// <param name="vol">Volumen en porcentaje (0-100)</param>
        private void ActualizarPosicionSliderVolumen(int vol)
        {
            if (pnlFondoVolumen.Width > 0 && picSliderVolumen.Width > 0)
            {
                double ratio = vol / 100.0;
                picSliderVolumen.Left = (int)(ratio * (pnlFondoVolumen.Width - picSliderVolumen.Width));
            }
            RM2.settings.volume = vol;
        }

        private void picSliderProgreso_MouseDown(object sender, MouseEventArgs e)
            => arrastrandoProgreso = true;

        /// <summary>
        /// Al soltar el slider de progreso, salta a la posición indicada en la canción
        /// </summary>
        private void picSliderProgreso_MouseUp(object sender, MouseEventArgs e)
        {
            if (arrastrandoProgreso && RM2.currentMedia != null)
            {
                double porcentaje = (double)picSliderProgreso.Left / (pnlFondoProgreso.Width - picSliderProgreso.Width);
                RM2.Ctlcontrols.currentPosition = porcentaje * RM2.currentMedia.duration;
            }
            arrastrandoProgreso = false;
        }

        /// <summary>
        /// Controla el movimiento del slider de progreso durante reproducción
        /// </summary>
        private void picSliderProgreso_MouseMove(object sender, MouseEventArgs e)
        {
            if (arrastrandoProgreso)
            {
                int nuevaX = picSliderProgreso.Left + e.X;
                nuevaX = Math.Max(0, Math.Min(nuevaX, pnlFondoProgreso.Width - picSliderProgreso.Width));
                picSliderProgreso.Left = nuevaX;
            }
        }

        /// <summary>
        /// Timer que actualiza la posición del slider de progreso en tiempo real
        /// </summary>
        private void timerProgreso_Tick(object sender, EventArgs e)
        {
            if (estaReproduciendo && RM2.currentMedia != null && !arrastrandoProgreso)
            {
                if (RM2.currentMedia.duration > 0)
                {
                    double progreso = RM2.Ctlcontrols.currentPosition / RM2.currentMedia.duration;
                    picSliderProgreso.Left = (int)(progreso * (pnlFondoProgreso.Width - picSliderProgreso.Width));
                }
            }
        }

        /// <summary>
        /// Detiene la música al cerrar el formulario
        /// </summary>
        private void DlgMesaPracticas2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RM2 != null) RM2.Ctlcontrols.stop();
        }

        // ====================================================================
        // GESTIÓN DE PANELES
        // ====================================================================

        /// <summary>
        /// Alterna la visibilidad del panel de Práctica 1
        /// </summary>
        private void BtnPractica1_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas1);

        /// <summary>
        /// Alterna la visibilidad del panel de Práctica 2
        /// </summary>
        private void BtnPractica2_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas2);

        /// <summary>
        /// Alterna la visibilidad del panel de Práctica 3
        /// </summary>
        private void BtnPractica3_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas4);

        /// <summary>
        /// Alterna la visibilidad del panel de Práctica 4
        /// </summary>
        private void BtnPractica4_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas3);

        /// <summary>
        /// Muestra un panel y oculta todos los demás
        /// </summary>
        private void MostrarPanel(Panel panelActivo)
        {
            panelActivo.Visible = !panelActivo.Visible;
            PnlPracticas1.Visible = (panelActivo == PnlPracticas1) ? panelActivo.Visible : false;
            PnlPracticas2.Visible = (panelActivo == PnlPracticas2) ? panelActivo.Visible : false;
            PnlPracticas3.Visible = (panelActivo == PnlPracticas3) ? panelActivo.Visible : false;
            PnlPracticas4.Visible = (panelActivo == PnlPracticas4) ? panelActivo.Visible : false;
        }

        /// <summary>
        /// Valida si una cadena es un número entero válido
        /// </summary>
        /// <param name="valor">Cadena a validar</param>
        /// <returns>True si es número, False en otro caso</returns>
        private bool EsNumero(string valor)
            => int.TryParse(valor, out _);

        // ====================================================================
        // PRÁCTICA 2 PANEL 1: MATRIZ CON LÍNEAS ALEATORIAS
        // ====================================================================

        /// <summary>
        /// Construye la estructura de la tabla (columnas y filas con dimensiones constantes)
        /// </summary>
        private void Btn1Practica2Panel1_Click(object sender, EventArgs e)
        {
            // Validar y obtener parámetros
            if (!ValidarParametrosMatriz(out int columnas, out int renglones,
                out int ancho, out int altura))
                return;

            // Limpiar y crear estructura
            DGVPRACTICA2P1.Columns.Clear();
            DGVPRACTICA2P1.Rows.Clear();

            // Crear columnas
            for (int i = 0; i < columnas; i++)
            {
                DGVPRACTICA2P1.Columns.Add("Col" + i, "HC" + i);
                DGVPRACTICA2P1.Columns[i].Width = ancho;
            }

            // Crear filas
            for (int r = 0; r < renglones; r++)
            {
                DGVPRACTICA2P1.Rows.Add();
                DGVPRACTICA2P1.Rows[r].Height = altura;
            }
        }

        /// <summary>
        /// Valida los parámetros de entrada para crear una matriz
        /// </summary>
        private bool ValidarParametrosMatriz(out int columnas, out int renglones,
            out int ancho, out int altura)
        {
            columnas = renglones = ancho = altura = 0;

            if (!EsNumero(TbxCaptura1.Text))
            {
                MessageBox.Show("Capture el número de columnas");
                TbxCaptura1.Focus();
                return false;
            }
            columnas = Convert.ToInt32(TbxCaptura1.Text);

            if (!EsNumero(TbxCaptura2.Text))
            {
                MessageBox.Show("Capture el número de renglones");
                TbxCaptura2.Focus();
                return false;
            }
            renglones = Convert.ToInt32(TbxCaptura2.Text);

            if (!EsNumero(TbxCaptura3.Text))
            {
                MessageBox.Show("Capture el ancho");
                TbxCaptura3.Focus();
                return false;
            }
            ancho = Convert.ToInt32(TbxCaptura3.Text);

            if (!EsNumero(TbxCaptur4.Text))
            {
                MessageBox.Show("Capture la altura constante en el campo 4");
                TbxCaptur4.Focus();
                return false;
            }
            altura = Convert.ToInt32(TbxCaptur4.Text);

            return true;
        }

        /// <summary>
        /// Llena la matriz con números aleatorios (0-9) y colorea los 9 de rojo
        /// </summary>
        private void Btn2Practica2Panel1_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < DGVPRACTICA2P1.Rows.Count; r++)
            {
                for (int c = 0; c < DGVPRACTICA2P1.Columns.Count; c++)
                {
                    int numero = generadorAleatorio.Next(0, 10);
                    DGVPRACTICA2P1.Rows[r].Cells[c].Value = numero;

                    // Colorear celdas con valor 9
                    DGVPRACTICA2P1.Rows[r].Cells[c].Style.BackColor =
                        (numero == 9) ? Color.Red : Color.Empty;
                }
            }

            DGVPRACTICA2P1.Invalidate();
        }

        /// <summary>
        /// El dibujo se realiza automáticamente en el evento CellPainting
        /// </summary>
        private void Btn3Practica2Panel1_Click(object sender, EventArgs e)
        {
            // Método vacío: el dibujo se realiza automáticamente en dgvMatrizCellPaint
        }

        /// <summary>
        /// Evento que dibuja líneas desde cada celda a una celda aleatoria
        /// Las líneas tienen colores basados en el valor numérico (0-9)
        /// </summary>
        private void DgvMatrizCellPaint(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Descartar encabezados
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            object valor = e.Value;
            if (valor != null && int.TryParse(valor.ToString(), out int numero))
            {
                // Centro de la celda origen
                Point centroCelda = new Point(
                    e.CellBounds.Left + (e.CellBounds.Width / 2),
                    e.CellBounds.Top + (e.CellBounds.Height / 2)
                );

                // Celda destino aleatoria
                int colAleatoria = generadorAleatorio.Next(0, DGVPRACTICA2P1.Columns.Count);
                int filaAleatoria = generadorAleatorio.Next(0, DGVPRACTICA2P1.Rows.Count);
                Rectangle rectDestino = DGVPRACTICA2P1.GetCellDisplayRectangle(colAleatoria, filaAleatoria, false);

                if (rectDestino.IsEmpty)
                {
                    e.Handled = true;
                    return;
                }

                Point destino = new Point(
                    rectDestino.Left + (rectDestino.Width / 2),
                    rectDestino.Top + (rectDestino.Height / 2)
                );

                // Seleccionar color de pluma según el número
                Pen pluma = ObtenerPlumaParaNumero(numero, destino);

                if (pluma != null)
                {
                    // Proteger área de dibujo
                    Rectangle areaProtegida = new Rectangle(
                        DGVPRACTICA2P1.RowHeadersWidth,
                        DGVPRACTICA2P1.ColumnHeadersHeight,
                        DGVPRACTICA2P1.Width - DGVPRACTICA2P1.RowHeadersWidth,
                        DGVPRACTICA2P1.Height - DGVPRACTICA2P1.ColumnHeadersHeight
                    );
                    e.Graphics.SetClip(areaProtegida);
                    e.Graphics.DrawLine(pluma, centroCelda, destino);
                    e.Graphics.ResetClip();
                    pluma.Dispose();
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Retorna una pluma (color y grosor) según el valor numérico
        /// </summary>
        private Pen ObtenerPlumaParaNumero(int numero, Point puntoDestino)
        {
            switch (numero)
            {
                case 0: return new Pen(Color.Black, 2);
                case 1: return new Pen(Color.Blue, 2);
                case 2: return new Pen(Color.Red, 2);
                case 3: return new Pen(Color.Chocolate, 2);
                case 4: return new Pen(Color.DarkBlue, 2);
                case 5: return new Pen(Color.OrangeRed, 9);
                case 6: return new Pen(Color.Beige, 2);
                case 7: return new Pen(Color.Yellow, 3);
                case 8: return new Pen(Color.Green, 5);
                case 9: return new Pen(Color.Crimson, 7);
                default: return null;
            }
        }

        // ====================================================================
        // PRÁCTICA 2 PANEL 2: BÚSQUEDA DE PALABRAS EN MATRIZ
        // ====================================================================

        /// <summary>
        /// Construye la segunda matriz (Panel 2)
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!ValidarParametrosMatriz(out int columnas, out int renglones,
                out int ancho, out int altura))
                return;

            DGVmesa2panel2.Columns.Clear();
            DGVmesa2panel2.Rows.Clear();

            for (int i = 0; i < columnas; i++)
            {
                DGVmesa2panel2.Columns.Add("Col" + i, "HC" + i);
                DGVmesa2panel2.Columns[i].Width = ancho;
            }

            for (int r = 0; r < renglones; r++)
            {
                DGVmesa2panel2.Rows.Add();
                DGVmesa2panel2.Rows[r].Height = altura;
            }
        }

        /// <summary>
        /// Llena la matriz 2 con letras aleatorias (A-Z) con fondo negro
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < DGVmesa2panel2.Rows.Count; r++)
            {
                for (int c = 0; c < DGVmesa2panel2.Columns.Count; c++)
                {
                    char letra = (char)generadorAleatorio.Next('A', 'Z' + 1);
                    DGVmesa2panel2.Rows[r].Cells[c].Value = letra;
                    DGVmesa2panel2.Rows[r].Cells[c].Style.BackColor = Color.Black;
                }
            }
        }

        /// <summary>
        /// Busca una palabra en la matriz y destaca las letras encontradas
        /// </summary>
        private void BTN3PANEL2_Click(object sender, EventArgs e)
        {
            string texto = TBXcaptur5.Text.ToUpper().Replace(" ", "").Replace(".", "").Replace(",", "");
            coordenadas.Clear();

            int indiceTexto = 0;
            bool palabraEncontrada = false;

            for (int r = 0; r < DGVmesa2panel2.Rows.Count && !palabraEncontrada; r++)
            {
                for (int c = 0; c < DGVmesa2panel2.Columns.Count && !palabraEncontrada; c++)
                {
                    char letra = (char)DGVmesa2panel2.Rows[r].Cells[c].Value;
                    DGVmesa2panel2.Rows[r].Cells[c].Style.BackColor = Color.Black;

                    if (letra == texto[indiceTexto])
                    {
                        DGVmesa2panel2.Rows[r].Cells[c].Style.BackColor = Color.Yellow;
                        indiceTexto++;

                        if (indiceTexto == texto.Length)
                        {
                            palabraEncontrada = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evento que dibuja en la segunda matriz
        /// Almacena coordenadas de celdas amarillas para dibujo posterior
        /// </summary>
        private void DgvMatrizCellPaint2(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            object valor = e.Value;
            if (valor != null && e.CellStyle.BackColor == Color.Yellow)
            {
                Point centroCelda = new Point(
                    e.CellBounds.Left + (e.CellBounds.Width / 2),
                    e.CellBounds.Top + (e.CellBounds.Height / 2)
                );

                coordenadas.Add(centroCelda);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Dibuja líneas conectando todas las coordenadas almacenadas
        /// </summary>
        private void BTN4PANEL2_Click(object sender, EventArgs e)
        {
            if (coordenadas.Count < 2) return;

            Graphics grafica = DGVmesa2panel2.CreateGraphics();
            Pen pluma = new Pen(Color.Red, 2);
            grafica.DrawLines(pluma, coordenadas.ToArray());
            pluma.Dispose();
        }

        /// <summary>
        /// Alterna la visibilidad de ambas matrices
        /// </summary>
        private void BTNmatrizPractica2_Click(object sender, EventArgs e)
        {
            DGVmesa2panel2.Visible = !DGVmesa2panel2.Visible;
            DGVPRACTICA2P1.Visible = !DGVPRACTICA2P1.Visible;
        }

        /// <summary>
        /// Abre explorador de archivos para cargar canciones nuevas
        /// </summary>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (estaReproduciendo)
            {
                RM2.Ctlcontrols.pause();
                estaReproduciendo = false;
            }

            OpenFileDialog explorador = new OpenFileDialog();
            explorador.Multiselect = true;
            explorador.Filter = "Archivos de Audio|*.mp3;*.wav";
            explorador.Title = "Elige tus canciones para el reproductor vintage";
            explorador.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            if (explorador.ShowDialog() == DialogResult.OK)
            {
                rutasCanciones.Clear();
                nombresCanciones.Clear();

                foreach (string archivo in explorador.FileNames)
                {
                    rutasCanciones.Add(archivo);
                    nombresCanciones.Add(System.IO.Path.GetFileNameWithoutExtension(archivo));
                }

                CargarCancion(0, true);
            }
        }

        // ====================================================================
        // PRÁCTICA 3: DIBUJO DE GRÁFICOS BÁSICOS
        // ====================================================================

        /// <summary>
        /// Dibuja elipses concéntricas y líneas diagonales
        /// </summary>
        private void Btn1Practic3mesa2_Click(object sender, EventArgs e)
        {
            Graphics lienzo = panelP3.CreateGraphics();
            Pen pluma1 = new Pen(Color.Magenta, 3);
            Pen pluma2 = new Pen(Color.Blue, 2);

            // Elipse central
            lienzo.DrawEllipse(pluma1,
                (panelP3.Width / 2) - (500 / 2),
                (panelP3.Height / 2) - (150 / 2), 500, 150);

            // Líneas diagonales
            lienzo.DrawLine(pluma2, 0, 0, panelP3.Width, panelP3.Height);
            lienzo.DrawLine(pluma2, panelP3.Width, 0, 0, panelP3.Height);

            // Elipses concéntricas
            int centroX = (panelP3.Width / 2) - (500 / 2);
            int centroY = (panelP3.Height / 2) - (150 / 2);

            for (int i = 0; i < 50; i++)
            {
                int crecimiento = i * 10;
                int crecimientoTotal = i * 20;

                lienzo.DrawEllipse(pluma1,
                    centroX - crecimiento,
                    centroY - crecimiento,
                    500 + crecimientoTotal,
                    150 + crecimientoTotal);
            }

            pluma1.Dispose();
            pluma2.Dispose();
        }

        // ====================================================================
        // PRÁCTICA 4: DIBUJO INTERACTIVO CON MOVIMIENTO DEL RATÓN
        // ====================================================================

        /// <summary>
        /// Dibuja líneas aleatorias según el movimiento del ratón
        /// </summary>
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics lienzo = PnlM2P4.CreateGraphics();

            Color colorAleatorio = Color.FromArgb(
                generadorAleatorio.Next(0, 256),
                generadorAleatorio.Next(0, 256),
                generadorAleatorio.Next(0, 256));

            Pen pluma = new Pen(colorAleatorio, 3);

            lienzo.DrawLine(pluma,
                generadorAleatorio.Next(0, PnlM2P4.Width),
                generadorAleatorio.Next(0, PnlM2P4.Height),
                e.Location.X,
                e.Location.Y);

            pluma.Dispose();
        }
    }
}
