using System;
using System.Drawing;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    public partial class DlgMesaPracticas1 : Form
    {
        // ====================================================================
        // ATRIBUTOS DE LA CLASE
        // ====================================================================
        int RA; // Almacena el renglón actual para operaciones en matrices
        int SumatoriaTemp = 0; // Variable temporal para sumar valores en la práctica 3

        // Atributo nuevo para el control del reproductor de música
        bool estaReproduciendo = false;

        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================
        public DlgMesaPracticas1()
        {
            InitializeComponent();
            RA = 0;

            // --- INICIALIZACIÓN DEL SISTEMA DE MÚSICA ---
            try
            {
                // NOTA: Cambia esta ruta por la canción que desees reproducir
                reproductorMusica.URL = @"C:\Users\Lenovo\Music\Music\God Of War (PlayStation Soundtrack).mp3";
                // Asignamos el volumen inicial basado en el valor del TrackBar (50)
                reproductorMusica.settings.volume = trkVolumen.Value;
                estaReproduciendo = true; // Indicamos que la música arrancó
            }
            catch
            {
                // Si falla al cargar la música (ruta no existe, etc.), lo ignoramos para que no colapse el programa
            }
        }

        // ====================================================================
        // GESTIÓN DE PANELES (NAVEGACIÓN ENTRE PRÁCTICAS)
        // ====================================================================

        // Muestra u oculta el Panel de la Práctica 1 y asegura que los demás estén ocultos
        private void BtnPractica1_Click(object sender, EventArgs e)
        {
            if (PnlPracticas1.Visible)
                PnlPracticas1.Visible = false;
            else
            {
                PnlPracticas1.Visible = true;
                PnlPracticas2.Visible = false;
                PnlPracticas3.Visible = false;
                PnlPracticas4.Visible = false;
            }
        }

        // Muestra u oculta el Panel de la Práctica 2 y asegura que los demás estén ocultos
        private void BtnPractica2_Click(object sender, EventArgs e)
        {
            if (PnlPracticas2.Visible)
                PnlPracticas2.Visible = false;
            else
            {
                PnlPracticas2.Visible = true;
                PnlPracticas1.Visible = false;
                PnlPracticas3.Visible = false;
                PnlPracticas4.Visible = false;
            }
        }

        // Muestra u oculta el Panel de la Práctica 3 y asegura que los demás estén ocultos
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
        }

        // Muestra u oculta el Panel de la Práctica 4 y asegura que los demás estén ocultos
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
        }

        // Alterna la visibilidad de todas las matrices en pantalla
        private void BTNmatrix_Click(object sender, EventArgs e)
        {
            DgvMatriz1.Visible = !DgvMatriz1.Visible;
            DgvMatriz2.Visible = !DgvMatriz2.Visible;
            DGVMATRIZ3.Visible = !DGVMATRIZ3.Visible;
        }

        // ====================================================================
        // FUNCIONES DE APOYO
        // ====================================================================

        // Valida si una cadena de texto es un número entero válido
        private bool Esnumero(string Valor)
        {
            int Numero;
            return int.TryParse(Valor, out Numero);
        }

        // ====================================================================
        // PRÁCTICA 1
        // ====================================================================

        // Genera la estructura de la Matriz 1 con tamaño y anchos dinámicos
        private void BtnPractica1P1_Click(object sender, EventArgs e)
        {
            int Tamano;
            int Anchoconstante;

            // Validación de captura del tamaño de la matriz
            if (!Esnumero(TbxCaptura1.Text))
            {
                MessageBox.Show("Capture el tamaño de la matriz");
                return;
            }
            Tamano = Convert.ToInt32(TbxCaptura1.Text);

            // Validación de captura del ancho de columnas
            if (!Esnumero(TbxCaptura2.Text))
            {
                MessageBox.Show("Capture el ancho de columnas");
                return;
            }
            Anchoconstante = Convert.ToInt32(TbxCaptura2.Text);

            // Reseteo de la matriz
            DgvMatriz1.Columns.Clear();
            DgvMatriz1.Rows.Clear();
            RA = 0;

            // Agrega columnas a la matriz y establece su ancho dinámico en forma de pirámide
            for (int i = 0; i < Tamano; i++)
            {
                DgvMatriz1.Columns.Add("Col" + i.ToString(), "HC" + i.ToString());

                // La primera mitad crece, la segunda mitad decrece
                if (i < (Tamano / 2))
                    DgvMatriz1.Columns[i].Width = i * Anchoconstante;
                else
                    DgvMatriz1.Columns[i].Width = (Tamano - i) * Anchoconstante;
            }

            // Agrega las filas correspondientes al tamaño
            for (int r = 0; r < Tamano; r++)
                DgvMatriz1.Rows.Add();
        }

        // Llena la Matriz 1 y aplica colores en escala de grises basados en la posición
        private void BtnPractica2P1_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < DgvMatriz1.Rows.Count; r++)
            {
                for (int c = 0; c < DgvMatriz1.Columns.Count; c++)
                {
                    // Asigna el valor del ancho de la columna a la celda
                    DgvMatriz1.Rows[r].Cells[c].Value = DgvMatriz1.Columns[c].Width;

                    // Aplica degradado de grises (más claro hacia el centro, más oscuro a los bordes)
                    if (c < (DgvMatriz1.Columns.Count / 2))
                    {
                        int color = 127 + (c * 8);
                        DgvMatriz1.Rows[r].Cells[c].Style.BackColor = Color.FromArgb(color, color, color);
                    }
                    else
                    {
                        int color = 255 - ((c - 15) * 8);
                        DgvMatriz1.Rows[r].Cells[c].Style.BackColor = Color.FromArgb(color, color, color);
                    }
                }
            }
        }

        // Llena la Matriz 1 con números aleatorios y resalta los '9' en rojo
        private void BtnPractica3P1_Click(object sender, EventArgs e)
        {
            Random Random = new Random();

            // Recorre cada celda de la matriz y asigna un número aleatorio entre 0 y 9
            for (int r = 0; r < DgvMatriz1.Rows.Count; r++)
            {
                for (int c = 0; c < DgvMatriz1.Columns.Count; c++)
                {
                    int Ultimovalor = Random.Next(0, 10);
                    DgvMatriz1.Rows[r].Cells[c].Value = Ultimovalor;

                    // Si el número es 9, pinta el fondo de rojo
                    if (Ultimovalor == 9)
                        DgvMatriz1.Rows[r].Cells[c].Style.BackColor = Color.Red;
                }
            }
        }

        // ====================================================================
        // PRÁCTICA 2
        // ====================================================================

        // Genera la estructura de la Matriz 2 basándose en dimensiones especificadas
        private void BTN1Practrica2_Click(object sender, EventArgs e)
        {
            int Columnas, Renglones, Anchoconstante, Alturaconstante;

            // Validación de inputs para Columnas, Renglones, Ancho y Altura
            if (!Esnumero(TbxCaptura1.Text))
            {
                MessageBox.Show("Capture el número de columnas");
                TbxCaptura1.Focus(); return;
            }
            Columnas = Convert.ToInt32(TbxCaptura1.Text);

            if (!Esnumero(TbxCaptura2.Text))
            {
                MessageBox.Show("Capture el número de renglones");
                TbxCaptura2.Focus(); return;
            }
            Renglones = Convert.ToInt32(TbxCaptura2.Text);

            if (!Esnumero(TbxCaptura3.Text))
            {
                MessageBox.Show("Capture el ancho");
                TbxCaptura3.Focus(); return;
            }
            Anchoconstante = Convert.ToInt32(TbxCaptura3.Text);

            if (!Esnumero(TbxCaptura4.Text))
            {
                MessageBox.Show("Capture la altura constante en el campo 4");
                TbxCaptura4.Focus(); return;
            }
            Alturaconstante = Convert.ToInt32(TbxCaptura4.Text);

            // Limpia la matriz antes de generar una nueva
            DgvMatriz2.Columns.Clear();
            DgvMatriz2.Rows.Clear();

            // Agrega columnas a la matriz y establece su ancho dependiento del CheckBox Cilindro
            for (int i = 0; i < Columnas; i++)
            {
                DgvMatriz2.Columns.Add("Col" + i.ToString(), "HC" + i.ToString());
                DgvMatriz2.Columns[i].Width = Anchoconstante;

                if (CBXcilindro.Checked)
                {
                    if (i < (Columnas / 2))
                        DgvMatriz2.Columns[i].Width = (i * 1) + Anchoconstante;
                    else
                        DgvMatriz2.Columns[i].Width = (Columnas - i) + Anchoconstante;
                }
                else
                {
                    DgvMatriz2.Columns[i].Width = Anchoconstante;
                }
            }

            // Agrega filas a la matriz y establece su altura constante
            for (int r = 0; r < Renglones; r++)
            {
                DgvMatriz2.Rows.Add();
                DgvMatriz2.Rows[r].Height = Alturaconstante;
            }
        }

        // Aplica colores dinámicos (RGB) a las celdas de la Matriz 2 basados en su posición
        private void BTN2Practrica2_Click(object sender, EventArgs e)
        {
            for (int r = 0; r < DgvMatriz2.Rows.Count; r++)
            {
                for (int c = 0; c < DgvMatriz2.Columns.Count; c++)
                {
                    // Fórmulas matemáticas para generar tonos RGB únicos por celda
                    int rojo = ((r * c) * 1000) % 256;
                    int verde = ((r + c) * 500) % 256;
                    int azul = ((r * 5 + c * 3) * 200) % 256;

                    DgvMatriz2.Rows[r].Cells[c].Style.BackColor = Color.FromArgb(rojo, verde, azul);

                    // Coloca el código RGB generado como texto dentro de la celda
                    DgvMatriz2.Rows[r].Cells[c].Value = rojo + "," + verde + "," + azul;
                }
            }
        }

        // Llena la Matriz 2 con dígitos aleatorios, cuenta y suma pares e impares, pintándolos
        private void BTN3Practrica2_Click(object sender, EventArgs e)
        {
            int ContadorImpares = 1;
            int ContadorPares = 1;
            int SumaPares = 0;
            int SumaImpares = 0;

            int ValorRandom;
            Random Random = new Random();
            RA = 0;

            for (int r = 0; r < DgvMatriz2.Rows.Count; r++)
            {
                for (int c = 0; c < DgvMatriz2.Columns.Count; c++)
                {
                    ValorRandom = Random.Next(0, 11);

                    // Si es PAR
                    if (ValorRandom % 2 == 0)
                    {
                        DgvMatriz2.Rows[r].Cells[c].Style.BackColor = Color.DarkBlue;
                        TBXCantidadPares.Text = ContadorPares++.ToString();
                        SumaPares += ValorRandom;
                    }
                    // Si es IMPAR
                    else
                    {
                        DgvMatriz2.Rows[r].Cells[c].Style.BackColor = Color.DarkMagenta;
                        TBXCantidadImpares.Text = ContadorImpares++.ToString();
                        SumaImpares += ValorRandom;
                    }

                    DgvMatriz2.Rows[r].Cells[c].Value = ValorRandom;
                }
            }
            // Actualiza los TextBoxes con los totales de las sumatorias
            SumatoriaPares.Text = SumaPares.ToString();
            SumatoriaImpares.Text = SumaImpares.ToString();
        }

        // Ordena de mayor a menor el Renglón Actual (RA) de la Matriz 2 usando el Método Burbuja
        private void BTN4PRACTICA2_Click(object sender, EventArgs e)
        {
            bool Ordenado = true; // Bandera para verificar si la fila ya está ordenada

            int totalColumnas = DgvMatriz2.Columns.Count;

            // Recorre cada par de columnas adyacentes y compara sus valores en el renglón RA
            for (int c = 0; c < DgvMatriz2.Columns.Count - 1; c++)
            {
                int Valor1 = Convert.ToInt32(DgvMatriz2.Rows[RA].Cells[c].Value);
                int Valor2 = Convert.ToInt32(DgvMatriz2.Rows[RA].Cells[c + 1].Value);

                // Si el valor siguiente es mayor que el actual, se intercambian (orden descendente)
                if (Valor2 > Valor1)
                {
                    DgvMatriz2.Rows[RA].Cells[c].Value = Valor2;
                    DgvMatriz2.Rows[RA].Cells[c + 1].Value = Valor1;
                    Ordenado = false; // Como hubo cambio, la fila aún no estaba completamente ordenada
                }
            }

            // Si la matriz ya está ordenada, cambia el color de fondo a verde y avanza al siguiente renglón
            if (Ordenado == true)
            {
                for (int c = 0; c < DgvMatriz2.Columns.Count; c++)
                {
                    DgvMatriz2.Rows[RA].Cells[c].Style.BackColor = Color.Green;
                }

                RA++; // Avanza al siguiente renglón para la próxima vez que se pulse el botón

                if (RA == DgvMatriz2.Rows.Count - 1)
                {
                    RA = 0; // Reinicia el índice si llegó al final
                }
            }
        }

        // Ordena TODA la Matriz 2 renglón por renglón y asegura que tenga datos primero
        private void BTNMESAPRACTICAS1PANEL2_Click(object sender, EventArgs e)
        {
            // 1. Verificación de existencia de cuadrícula
            if (DgvMatriz2.Rows.Count == 0 || DgvMatriz2.Columns.Count == 0)
            {
                MessageBox.Show("Primero debe generar la matriz.");
                return;
            }

            // 2. Verificación de datos válidos (números)
            var valorCelda = DgvMatriz2.Rows[0].Cells[0].Value;
            if (valorCelda == null || !Esnumero(valorCelda.ToString()))
            {
                MessageBox.Show("La matriz no contiene números. Por favor, genere los números aleatorios con el Botón 3 primero.");
                return;
            }

            bool Ordenado = true;
            int TotalColumnas = DgvMatriz2.Columns.Count;

            // Aplica el ordenamiento Burbuja a todos los renglones
            for (int i = 0; i < TotalColumnas - 1; i++)
            {
                for (int R = 0; R < DgvMatriz2.Rows.Count; R++)
                {
                    for (int c = 0; c < DgvMatriz2.Columns.Count - 1; c++)
                    {
                        int Valor1 = Convert.ToInt32(DgvMatriz2.Rows[R].Cells[c].Value);
                        int Valor2 = Convert.ToInt32(DgvMatriz2.Rows[R].Cells[c + 1].Value);

                        if (Valor2 > Valor1)
                        {
                            DgvMatriz2.Rows[R].Cells[c].Value = Valor2;
                            DgvMatriz2.Rows[R].Cells[c + 1].Value = Valor1;
                            Ordenado = false;
                        }
                    }
                }
            }

            // Pinta toda la matriz de verde si el ordenamiento finalizó exitosamente
            if (Ordenado == true)
            {
                for (int R = 0; R < DgvMatriz2.Rows.Count; R++)
                {
                    for (int c = 0; c < DgvMatriz2.Columns.Count; c++)
                    {
                        DgvMatriz2.Rows[R].Cells[c].Style.BackColor = Color.Green;
                    }

                    RA++;
                    if (RA == DgvMatriz2.Rows.Count - 1)
                    {
                        RA = 0;
                    }
                }
            }
        }

        // ====================================================================
        // PRÁCTICA 3 (LABORATORIOS)
        // ====================================================================

        // Genera la estructura de la Matriz 3 para evaluar los 5 Laboratorios
        private void BTN1practica3_Click(object sender, EventArgs e)
        {
            // Configuración visual por defecto
            DGVMATRIZ3.DefaultCellStyle.BackColor = Color.White;
            DGVMATRIZ3.DefaultCellStyle.ForeColor = Color.Black;
            DGVMATRIZ3.AllowUserToAddRows = false;

            // Limpieza
            DGVMATRIZ3.Columns.Clear();
            DGVMATRIZ3.Rows.Clear();

            // Creación de columna principal de Laboratorios
            DGVMATRIZ3.Columns.Add("Lab", "Lab");
            DGVMATRIZ3.Columns["Lab"].Width = 180;

            // Agrega las columnas de Días (D1 a D7)
            for (int j = 1; j < 8; j++)
            {
                DGVMATRIZ3.Columns.Add("Col" + j.ToString(), "D" + j.ToString());
                DGVMATRIZ3.Columns["Col" + j.ToString()].Width = 40;
            }

            // Columnas de estadísticas
            DGVMATRIZ3.Columns.Add("promedio", "promedio");
            DGVMATRIZ3.Columns.Add("#Altas", "#Altas");
            DGVMATRIZ3.Columns.Add("#Media", "#Media");
            DGVMATRIZ3.Columns.Add("#Baja", "#Baja");

            // Anchos de las columnas estadísticas
            DGVMATRIZ3.Columns["promedio"].Width = 70;
            DGVMATRIZ3.Columns["#Altas"].Width = 60;
            DGVMATRIZ3.Columns["#Media"].Width = 60;
            DGVMATRIZ3.Columns["#Baja"].Width = 60;

            // Agrega 5 filas nombradas "Laboratorio 1" a "Laboratorio 5"
            for (int i = 0; i < 5; i++)
            {
                DGVMATRIZ3.Rows.Add("Laboratorio " + (i + 1).ToString());
            }
        }

        // Llena los días de los laboratorios con lecturas aleatorias
        private void BTN2Practica3_Click(object sender, EventArgs e)
        {
            int ValorRandom;
            Random Random = new Random();

            // Recorre cada celda de datos y asigna un valor (0 a 40)
            for (int r = 0; r < DGVMATRIZ3.Rows.Count; r++)
            {
                // Omite las últimas 4 columnas de estadísticas
                for (int c = 1; c < DGVMATRIZ3.Columns.Count - 4; c++)
                {
                    ValorRandom = Random.Next(0, 41);
                    DGVMATRIZ3.Rows[r].Cells[c].Value = ValorRandom;
                    SumatoriaTemp += ValorRandom;
                }
            }
        }

        // Calcula el promedio semanal por cada laboratorio
        private void BTN3Practica3_Click(object sender, EventArgs e)
        {
            int Promedio = 0;
            int sumatoriatempo = 0;
            int Derecha = DGVMATRIZ3.ColumnCount - 4; // Índice de la columna Promedio

            for (int r = 0; r < DGVMATRIZ3.Rows.Count; r++)
            {
                for (int c = 1; c < DGVMATRIZ3.Columns.Count; c++)
                {
                    if (c != Derecha)
                    {
                        // Suma los valores de los días
                        sumatoriatempo += Convert.ToInt32(DGVMATRIZ3.Rows[r].Cells[c].Value);
                    }
                    if (c == Derecha)
                    {
                        // Calcula el promedio entre los 7 días
                        Promedio = sumatoriatempo / 7;
                    }
                }
                sumatoriatempo = 0; // Reinicia sumatoria para el siguiente renglón
                DGVMATRIZ3.Rows[r].Cells[Derecha].Value = Promedio; // Escribe el promedio
            }
        }

        // Analiza las lecturas, las clasifica (Alta, Media, Baja) y las colorea
        private void BTN4practica3_Click(object sender, EventArgs e)
        {
            int SumaAlta = 0;
            int SumaMedia = 0;
            int SumaBaja = 0;
            int ValorActual = 0;

            for (int r = 0; r < DGVMATRIZ3.Rows.Count; r++)
            {
                // Recorre solo las columnas de los 7 días
                for (int c = 1; c < DGVMATRIZ3.Columns.Count - 4; c++)
                {
                    ValorActual = Convert.ToInt32(DGVMATRIZ3.Rows[r].Cells[c].Value);

                    // Clasificación y coloreo
                    if (ValorActual > 25)
                    {
                        SumaAlta++;
                        DGVMATRIZ3.Rows[r].Cells[c].Style.BackColor = Color.Red;
                    }
                    else if (ValorActual < 18)
                    {
                        SumaBaja++;
                        DGVMATRIZ3.Rows[r].Cells[c].Style.BackColor = Color.RoyalBlue;
                    }
                    else
                    {
                        SumaMedia++;
                        DGVMATRIZ3.Rows[r].Cells[c].Style.BackColor = Color.Yellow;
                    }
                }

                // Asigna los totales en las últimas tres columnas correspondientes
                DGVMATRIZ3.Rows[r].Cells[9].Value = SumaAlta;
                DGVMATRIZ3.Rows[r].Cells[10].Value = SumaMedia;
                DGVMATRIZ3.Rows[r].Cells[11].Value = SumaBaja;

                // Reinicia los contadores para analizar el siguiente laboratorio
                SumaAlta = 0; SumaMedia = 0; SumaBaja = 0;
            }
        }

        // ====================================================================
        // EVENTOS DEL REPRODUCTOR DE MÚSICA
        // ====================================================================

        // Evento que se dispara al mover la barra de volumen (TrackBar)
        private void trkVolumen_Scroll_1(object sender, EventArgs e)
        {
            // Ajusta el volumen del reproductor al valor actual de la barra (0 a 100)
            if (reproductorMusica != null)
            {
                reproductorMusica.settings.volume = trkVolumen.Value;
            }
        }

        // Evento que se dispara al presionar el botón de Play/Pausa
        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (reproductorMusica != null)
            {
                if (estaReproduciendo)
                {
                    // Si está sonando, la pausamos y cambiamos el texto del botón
                    reproductorMusica.Ctlcontrols.pause();
                    btnPlayPause.Text = "▶ Reproducir";
                    estaReproduciendo = false;
                }
                else
                {
                    // Si está pausada, la reanudamos y cambiamos el texto
                    reproductorMusica.Ctlcontrols.play();
                    btnPlayPause.Text = "⏸ Pausar";
                    estaReproduciendo = true;
                }
            }
        }

        // Evento que se dispara al cerrar el Formulario
        private void DlgMesaPracticas1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            // Detiene la música si el reproductor existe al cerrar la ventana
            if (reproductorMusica != null)
            {
                reproductorMusica.Ctlcontrols.stop();
            }
        }
    }
}