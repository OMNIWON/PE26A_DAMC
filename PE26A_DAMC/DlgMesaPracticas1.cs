using System;
using System.Drawing;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    /// <summary>
    /// Formulario de la Mesa de Prácticas 1 - Operaciones con Matrices
    /// Implementa: Generación de matrices, coloreo, ordenamiento y análisis de datos
    /// Incluye reproductor de música integrado
    /// 
    /// Autor: DAMC
    /// Fecha: 2026
    /// </summary>
    public partial class DlgMesaPracticas1 : Form
    {
        // ====================================================================
        // ATRIBUTOS DE LA CLASE
        // ====================================================================

        /// <summary>Índice del renglón actual para operaciones en matrices</summary>
        private int renglonActual = 0;

        /// <summary>Variable temporal para acumular sumatoria en prácticas</summary>
        private int sumatoria = 0;

        /// <summary>Flag que indica si la música está reproduciéndose</summary>
        private bool estaReproduciendo = false;

        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        /// <summary>
        /// Inicializa el formulario y configura el reproductor de música
        /// </summary>
        public DlgMesaPracticas1()
        {
            InitializeComponent();
            renglonActual = 0;

            try
            {
                // NOTA: Cambiar esta ruta según tu sistema de archivos
                reproductorMusica.URL = @"C:\Users\Lenovo\Music\Music\God Of War (PlayStation Soundtrack).mp3";
                reproductorMusica.settings.volume = trkVolumen.Value;
                estaReproduciendo = true;
            }
            catch
            {
                // Si la canción no existe, continuar sin música
            }
        }

        // ====================================================================
        // GESTIÓN DE PANELES (NAVEGACIÓN)
        // ====================================================================

        /// <summary>Alterna visibilidad del Panel 1 y oculta los demás</summary>
        private void BtnPractica1_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas1);

        /// <summary>Alterna visibilidad del Panel 2 y oculta los demás</summary>
        private void BtnPractica2_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas2);

        /// <summary>Alterna visibilidad del Panel 3 y oculta los demás</summary>
        private void BtnPractica3_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas3);

        /// <summary>Alterna visibilidad del Panel 4 y oculta los demás</summary>
        private void BtnPractica4_Click(object sender, EventArgs e)
            => MostrarPanel(PnlPracticas4);

        /// <summary>
        /// Muestra un panel específico y oculta todos los demás
        /// </summary>
        private void MostrarPanel(Panel panelActivo)
        {
            PnlPracticas1.Visible = (panelActivo == PnlPracticas1);
            PnlPracticas2.Visible = (panelActivo == PnlPracticas2);
            PnlPracticas3.Visible = (panelActivo == PnlPracticas3);
            PnlPracticas4.Visible = (panelActivo == PnlPracticas4);
        }

        /// <summary>Alterna la visibilidad de todas las matrices</summary>
        private void BTNmatrix_Click(object sender, EventArgs e)
        {
            DgvMatriz1.Visible = !DgvMatriz1.Visible;
            DgvMatriz2.Visible = !DgvMatriz2.Visible;
            DGVMATRIZ3.Visible = !DGVMATRIZ3.Visible;
        }

        // ====================================================================
        // FUNCIONES DE APOYO
        // ====================================================================

        /// <summary>
        /// Valida si una cadena es un número entero válido
        /// </summary>
        /// <param name="valor">Cadena a validar</param>
        /// <returns>True si es número válido, False en otro caso</returns>
        private bool EsNumero(string valor) 
            => int.TryParse(valor, out _);

        // ====================================================================
        // PRÁCTICA 1: MATRICES CON DEGRADADO Y ORDENAMIENTO
        // ====================================================================

        /// <summary>
        /// Genera estructura de matriz con columnas en forma de pirámide
        /// </summary>
        private void BtnPractica1P1_Click(object sender, EventArgs e)
        {
            if (!EsNumero(TbxCaptura1.Text))
            {
                MessageBox.Show("Capture el tamaño de la matriz");
                TbxCaptura1.Focus();
                return;
            }
            int tamanio = Convert.ToInt32(TbxCaptura1.Text);

            if (!EsNumero(TbxCaptura2.Text))
            {
                MessageBox.Show("Capture el ancho de columnas");
                TbxCaptura2.Focus();
                return;
            }
            int anchoConstante = Convert.ToInt32(TbxCaptura2.Text);

            DgvMatriz1.Columns.Clear();
            DgvMatriz1.Rows.Clear();
            renglonActual = 0;

            // Crear columnas con ancho en forma de pirámide
            for (int i = 0; i < tamanio; i++)
            {
                DgvMatriz1.Columns.Add("Col" + i, "HC" + i);
                
                // Primera mitad crece, segunda mitad decrece
                if (i < (tamanio / 2))
                    DgvMatriz1.Columns[i].Width = i * anchoConstante;
                else
                    DgvMatriz1.Columns[i].Width = (tamanio - i) * anchoConstante;
            }

            for (int r = 0; r < tamanio; r++)
                DgvMatriz1.Rows.Add();
        }

        /// <summary>
        /// Llena matriz con degradado de grises basado en posición
        /// </summary>
        private void BtnPractica2P1_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < DgvMatriz1.Rows.Count; r++)
            {
                for (int c = 0; c < DgvMatriz1.Columns.Count; c++)
                {
                    DgvMatriz1.Rows[r].Cells[c].Value = DgvMatriz1.Columns[c].Width;

                    int colorGris = (c < (DgvMatriz1.Columns.Count / 2))
                        ? 127 + (c * 8)
                        : 255 - ((c - 15) * 8);

                    DgvMatriz1.Rows[r].Cells[c].Style.BackColor = 
                        Color.FromArgb(colorGris, colorGris, colorGris);
                }
            }
        }

        /// <summary>
        /// Llena matriz con números aleatorios (0-9) y colorea los 9 en rojo
        /// </summary>
        private void BtnPractica3P1_Click(object sender, EventArgs e)
        {
            Random generador = new Random();

            for (int r = 0; r < DgvMatriz1.Rows.Count; r++)
            {
                for (int c = 0; c < DgvMatriz1.Columns.Count; c++)
                {
                    int numero = generador.Next(0, 10);
                    DgvMatriz1.Rows[r].Cells[c].Value = numero;
                    DgvMatriz1.Rows[r].Cells[c].Style.BackColor = 
                        (numero == 9) ? Color.Red : Color.Empty;
                }
            }
        }

        // ====================================================================
        // PRÁCTICA 2: COLOREO RGB Y ANÁLISIS DE PARES/IMPARES
        // ====================================================================

        /// <summary>
        /// Construye matriz 2 con dimensiones y opciones de cilindro
        /// </summary>
        private void BTN1Practrica2_Click(object sender, EventArgs e)
        {
            if (!ValidarParametrosMatriz(out int columnas, out int renglones, 
                out int ancho, out int altura))
                return;

            DgvMatriz2.Columns.Clear();
            DgvMatriz2.Rows.Clear();

            for (int i = 0; i < columnas; i++)
            {
                DgvMatriz2.Columns.Add("Col" + i, "HC" + i);

                if (CBXcilindro.Checked)
                {
                    if (i < (columnas / 2))
                        DgvMatriz2.Columns[i].Width = (i * 1) + ancho;
                    else
                        DgvMatriz2.Columns[i].Width = (columnas - i) + ancho;
                }
                else
                    DgvMatriz2.Columns[i].Width = ancho;
            }

            for (int r = 0; r < renglones; r++)
            {
                DgvMatriz2.Rows.Add();
                DgvMatriz2.Rows[r].Height = altura;
            }
        }

        /// <summary>
        /// Valida parámetros de entrada para crear matriz
        /// </summary>
        private bool ValidarParametrosMatriz(out int columnas, out int renglones,
            out int ancho, out int altura)
        {
            columnas = renglones = ancho = altura = 0;

            if (!EsNumero(TbxCaptura1.Text))
            {
                MessageBox.Show("Capture columnas");
                TbxCaptura1.Focus();
                return false;
            }
            columnas = Convert.ToInt32(TbxCaptura1.Text);

            if (!EsNumero(TbxCaptura2.Text))
            {
                MessageBox.Show("Capture renglones");
                TbxCaptura2.Focus();
                return false;
            }
            renglones = Convert.ToInt32(TbxCaptura2.Text);

            if (!EsNumero(TbxCaptura3.Text))
            {
                MessageBox.Show("Capture ancho");
                TbxCaptura3.Focus();
                return false;
            }
            ancho = Convert.ToInt32(TbxCaptura3.Text);

            if (!EsNumero(TbxCaptur4.Text))
            {
                MessageBox.Show("Capture altura");
                TbxCaptur4.Focus();
                return false;
            }
            altura = Convert.ToInt32(TbxCaptur4.Text);

            return true;
        }

        /// <summary>
        /// Aplica colores RGB dinámicos basados en posición de celda
        /// </summary>
        private void BTN2Practrica2_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < DgvMatriz2.Rows.Count; r++)
            {
                for (int c = 0; c < DgvMatriz2.Columns.Count; c++)
                {
                    int rojo = ((r * c) * 1000) % 256;
                    int verde = ((r + c) * 500) % 256;
                    int azul = ((r * 5 + c * 3) * 200) % 256;

                    DgvMatriz2.Rows[r].Cells[c].Style.BackColor = 
                        Color.FromArgb(rojo, verde, azul);
                    DgvMatriz2.Rows[r].Cells[c].Value = $"{rojo},{verde},{azul}";
                }
            }
        }

        /// <summary>
        /// Llena con aleatorios, cuenta pares/impares y los colorea
        /// </summary>
        private void BTN3Practrica2_Click(object sender, EventArgs e)
        {
            int contadorPares = 1;
            int contadorImpares = 1;
            int sumaPares = 0;
            int sumaImpares = 0;
            Random generador = new Random();

            for (int r = 0; r < DgvMatriz2.Rows.Count; r++)
            {
                for (int c = 0; c < DgvMatriz2.Columns.Count; c++)
                {
                    int numero = generador.Next(0, 11);

                    if (numero % 2 == 0)
                    {
                        DgvMatriz2.Rows[r].Cells[c].Style.BackColor = Color.DarkBlue;
                        TBXCantidadPares.Text = contadorPares++.ToString();
                        sumaPares += numero;
                    }
                    else
                    {
                        DgvMatriz2.Rows[r].Cells[c].Style.BackColor = Color.DarkMagenta;
                        TBXCantidadImpares.Text = contadorImpares++.ToString();
                        sumaImpares += numero;
                    }

                    DgvMatriz2.Rows[r].Cells[c].Value = numero;
                }
            }

            SumatoriaPares.Text = sumaPares.ToString();
            SumatoriaImpares.Text = sumaImpares.ToString();
        }

        /// <summary>
        /// Ordena renglón actual usando algoritmo Burbuja (descendente)
        /// </summary>
        private void BTN4PRACTICA2_Click(object sender, EventArgs e)
        {
            bool ordenado = true;

            for (int c = 0; c < DgvMatriz2.Columns.Count - 1; c++)
            {
                int valor1 = Convert.ToInt32(DgvMatriz2.Rows[renglonActual].Cells[c].Value);
                int valor2 = Convert.ToInt32(DgvMatriz2.Rows[renglonActual].Cells[c + 1].Value);

                if (valor2 > valor1)
                {
                    DgvMatriz2.Rows[renglonActual].Cells[c].Value = valor2;
                    DgvMatriz2.Rows[renglonActual].Cells[c + 1].Value = valor1;
                    ordenado = false;
                }
            }

            if (ordenado)
            {
                for (int c = 0; c < DgvMatriz2.Columns.Count; c++)
                    DgvMatriz2.Rows[renglonActual].Cells[c].Style.BackColor = Color.Green;

                renglonActual++;
                if (renglonActual == DgvMatriz2.Rows.Count)
                    renglonActual = 0;
            }
        }

        /// <summary>
        /// Ordena TODA la matriz renglón por renglón
        /// </summary>
        private void BTNMESAPRACTICAS1PANEL2_Click(object sender, EventArgs e)
        {
            if (DgvMatriz2.Rows.Count == 0 || DgvMatriz2.Columns.Count == 0)
            {
                MessageBox.Show("Primero genere la matriz");
                return;
            }

            var valor = DgvMatriz2.Rows[0].Cells[0].Value;
            if (valor == null || !EsNumero(valor.ToString()))
            {
                MessageBox.Show("La matriz no contiene números. Presione Botón 3 primero");
                return;
            }

            bool ordenado = true;

            for (int i = 0; i < DgvMatriz2.Columns.Count - 1; i++)
            {
                for (int r = 0; r < DgvMatriz2.Rows.Count; r++)
                {
                    for (int c = 0; c < DgvMatriz2.Columns.Count - 1; c++)
                    {
                        int v1 = Convert.ToInt32(DgvMatriz2.Rows[r].Cells[c].Value);
                        int v2 = Convert.ToInt32(DgvMatriz2.Rows[r].Cells[c + 1].Value);

                        if (v2 > v1)
                        {
                            DgvMatriz2.Rows[r].Cells[c].Value = v2;
                            DgvMatriz2.Rows[r].Cells[c + 1].Value = v1;
                            ordenado = false;
                        }
                    }
                }
            }

            if (ordenado)
            {
                for (int r = 0; r < DgvMatriz2.Rows.Count; r++)
                    for (int c = 0; c < DgvMatriz2.Columns.Count; c++)
                        DgvMatriz2.Rows[r].Cells[c].Style.BackColor = Color.Green;
            }
        }

        // ====================================================================
        // PRÁCTICA 3: ANÁLISIS DE LABORATORIOS
        // ====================================================================

        /// <summary>
        /// Construye tabla para evaluar 5 laboratorios en 7 días
        /// </summary>
        private void BTN1practica3_Click(object sender, EventArgs e)
        {
            DGVMATRIZ3.DefaultCellStyle.BackColor = Color.White;
            DGVMATRIZ3.DefaultCellStyle.ForeColor = Color.Black;
            DGVMATRIZ3.AllowUserToAddRows = false;
            DGVMATRIZ3.Columns.Clear();
            DGVMATRIZ3.Rows.Clear();

            DGVMATRIZ3.Columns.Add("Lab", "Lab");
            DGVMATRIZ3.Columns["Lab"].Width = 180;

            for (int j = 1; j < 8; j++)
            {
                DGVMATRIZ3.Columns.Add("Col" + j, "D" + j);
                DGVMATRIZ3.Columns["Col" + j].Width = 40;
            }

            DGVMATRIZ3.Columns.Add("promedio", "promedio");
            DGVMATRIZ3.Columns.Add("#Altas", "#Altas");
            DGVMATRIZ3.Columns.Add("#Media", "#Media");
            DGVMATRIZ3.Columns.Add("#Baja", "#Baja");

            for (int i = 0; i < 5; i++)
                DGVMATRIZ3.Rows.Add("Laboratorio " + (i + 1));
        }

        /// <summary>
        /// Llena días con lecturas aleatorias (0-40)
        /// </summary>
        private void BTN2Practica3_Click(object sender, EventArgs e)
        {
            Random generador = new Random();

            for (int r = 0; r < DGVMATRIZ3.Rows.Count; r++)
            {
                for (int c = 1; c < DGVMATRIZ3.Columns.Count - 4; c++)
                {
                    int numero = generador.Next(0, 41);
                    DGVMATRIZ3.Rows[r].Cells[c].Value = numero;
                    sumatoria += numero;
                }
            }
        }

        /// <summary>
        /// Calcula promedio semanal por laboratorio
        /// </summary>
        private void BTN3Practica3_Click(object sender, EventArgs e)
        {
            int columnaPromedio = DGVMATRIZ3.ColumnCount - 4;

            for (int r = 0; r < DGVMATRIZ3.Rows.Count; r++)
            {
                int suma = 0;
                for (int c = 1; c < columnaPromedio; c++)
                    suma += Convert.ToInt32(DGVMATRIZ3.Rows[r].Cells[c].Value);

                int promedio = suma / 7;
                DGVMATRIZ3.Rows[r].Cells[columnaPromedio].Value = promedio;
            }
        }

        /// <summary>
        /// Analiza y colorea lecturas: Rojo (>25), Azul (<18), Amarillo (18-25)
        /// </summary>
        private void BTN4practica3_Click(object sender, EventArgs e)
        {
            int sumaAlta = 0, sumaMedia = 0, sumaBaja = 0;

            for (int r = 0; r < DGVMATRIZ3.Rows.Count; r++)
            {
                for (int c = 1; c < DGVMATRIZ3.Columns.Count - 4; c++)
                {
                    int valor = Convert.ToInt32(DGVMATRIZ3.Rows[r].Cells[c].Value);

                    if (valor > 25)
                    {
                        sumaAlta++;
                        DGVMATRIZ3.Rows[r].Cells[c].Style.BackColor = Color.Red;
                    }
                    else if (valor < 18)
                    {
                        sumaBaja++;
                        DGVMATRIZ3.Rows[r].Cells[c].Style.BackColor = Color.RoyalBlue;
                    }
                    else
                    {
                        sumaMedia++;
                        DGVMATRIZ3.Rows[r].Cells[c].Style.BackColor = Color.Yellow;
                    }
                }

                DGVMATRIZ3.Rows[r].Cells[9].Value = sumaAlta;
                DGVMATRIZ3.Rows[r].Cells[10].Value = sumaMedia;
                DGVMATRIZ3.Rows[r].Cells[11].Value = sumaBaja;

                sumaAlta = sumaMedia = sumaBaja = 0;
            }
        }

        // ====================================================================
        // EVENTOS DEL REPRODUCTOR DE MÚSICA
        // ====================================================================

        /// <summary>
        /// Ajusta volumen al mover el TrackBar
        /// </summary>
        private void trkVolumen_Scroll_1(object sender, EventArgs e)
        {
            if (reproductorMusica != null)
                reproductorMusica.settings.volume = trkVolumen.Value;
        }

        /// <summary>
        /// Alterna entre Play/Pausa
        /// </summary>
        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (reproductorMusica == null) return;

            if (estaReproduciendo)
            {
                reproductorMusica.Ctlcontrols.pause();
                btnPlayPause.Text = "▶ Reproducir";
                estaReproduciendo = false;
            }
            else
            {
                reproductorMusica.Ctlcontrols.play();
                btnPlayPause.Text = "⏸ Pausar";
                estaReproduciendo = true;
            }
        }

        /// <summary>
        /// Detiene música al cerrar formulario
        /// </summary>
        private void DlgMesaPracticas1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (reproductorMusica != null)
                reproductorMusica.Ctlcontrols.stop();
        }
    }
}
