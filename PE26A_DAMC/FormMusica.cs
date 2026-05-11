using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace PE26A_DAMC
{
    public class FormMusica : Form
    {
        private AxWMPLib.AxWindowsMediaPlayer reproductorPrincipal;

        private Timer timerRotacion;
        private float anguloVinilo = 0f;

        public FormMusica(AxWMPLib.AxWindowsMediaPlayer reproductor)
        {
            reproductorPrincipal = reproductor;

            // --- 1. CONFIGURACIÓN DE LA VENTANA GIGANTE ---
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(500, 500);
            this.BackColor = Color.FromArgb(10, 10, 20);
            this.StartPosition = FormStartPosition.CenterParent;
            this.DoubleBuffered = true;

            // --- 2. MOTOR DEL DISCO GIRATORIO ---
            timerRotacion = new Timer();
            timerRotacion.Interval = 30;
            timerRotacion.Tick += (s, e) =>
            {
                anguloVinilo += 2f;
                if (anguloVinilo >= 360) anguloVinilo -= 360;
                this.Invalidate();
            };
            timerRotacion.Start();

            // --- 3. BOTONES PERSONALIZADOS CON TUS RUTAS EXACTAS ---

            // Lado Izquierdo
            CrearBotonMood(" CHILL", new Point(40, 140), (s, e) => ReproducirArchivoExacto(@"C:\Users\Lenovo\Music\Music\Aria Math.mp3"));
            CrearBotonMood("🎲 RANDOM", new Point(40, 200), (s, e) => ReproducirAleatorioDesdeCarpeta(@"C:\Users\Lenovo\Music\Music"));

            // Lado Derecho
            CrearBotonMood("🤑 CACHIN", new Point(350, 140), (s, e) => ReproducirArchivoExacto(@"C:\Users\Lenovo\Music\Music\Chico Bestia - Atrapo a la Villana.ft.Raven_Video Oficial.mp3"));
            CrearBotonMood("✨ SHINYY", new Point(350, 200), (s, e) => ReproducirArchivoExacto(@"C:\Users\Lenovo\Music\Music\The Night Begins to Shine.mp3"));


            // --- 4. BOTONES INFERIORES ORIGINALES ---
            Button btnCargar = new Button();
            btnCargar.Text = "🎵 ELEGIR CANCIÓN";
            btnCargar.ForeColor = Color.Cyan;
            btnCargar.FlatStyle = FlatStyle.Flat;
            btnCargar.Size = new Size(160, 40);
            btnCargar.Location = new Point(170, 340);
            btnCargar.Cursor = Cursors.Hand;
            btnCargar.Click += BtnCargar_Click;
            this.Controls.Add(btnCargar);

            Button btnCerrar = new Button();
            btnCerrar.Text = "✖ VOLVER";
            btnCerrar.ForeColor = Color.Fuchsia;
            btnCerrar.FlatStyle = FlatStyle.Flat;
            btnCerrar.Size = new Size(100, 30);
            btnCerrar.Location = new Point(200, 400);
            btnCerrar.Cursor = Cursors.Hand;
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);
        }

        // --- Clic original para buscar archivo manual en el explorador ---
        private void BtnCargar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Audio|*.mp3;*.wav|Todos|*.*";
            ofd.Title = "Busca tus rolas favoritas";

            // --- ¡ESTO ES LO QUE TE DEJA SOMBREAR VARIAS CANCIONES! ---
            ofd.Multiselect = true;

            // Abre directo en la carpeta Música
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // 1. Creamos una lista de reproducción temporal
                var miPlaylist = reproductorPrincipal.playlistCollection.newPlaylist("PlaylistTemporal");

                // 2. Recorremos todas las canciones que sombreaste
                foreach (string archivo in ofd.FileNames)
                {
                    // Las convertimos al formato que entiende el reproductor y las agregamos a la lista
                    var cancion = reproductorPrincipal.newMedia(archivo);
                    miPlaylist.appendItem(cancion);
                }

                // 3. Le decimos al reproductor que cargue esa lista y le damos Play
                reproductorPrincipal.currentPlaylist = miPlaylist;
                reproductorPrincipal.Ctlcontrols.play();
            }
        }

        // --- Estilo visual de los botones morados ---
        private void CrearBotonMood(string texto, Point ubicacion, EventHandler eventoClick)
        {
            Button btn = new Button();
            btn.Text = texto;
            btn.ForeColor = Color.FromArgb(192, 0, 192);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.Size = new Size(110, 35);
            btn.Location = ubicacion;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.BackColor = Color.FromArgb(15, 10, 30);
            btn.Click += eventoClick;
            this.Controls.Add(btn);
        }

        // --- ¡NUEVO! Función para reproducir un archivo exacto ---
        private void ReproducirArchivoExacto(string rutaArchivo)
        {
            if (File.Exists(rutaArchivo))
            {
                reproductorPrincipal.URL = rutaArchivo;
                reproductorPrincipal.Ctlcontrols.play();
            }
            else
            {
                MessageBox.Show($"¡Ups! No encuentro la canción:\n{rutaArchivo}\n\nRevisa que el nombre y la carpeta estén bien escritos.", "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Función para el botón Random (Elige una al azar de la carpeta principal) ---
        private void ReproducirAleatorioDesdeCarpeta(string rutaCarpeta)
        {
            if (!Directory.Exists(rutaCarpeta))
            {
                MessageBox.Show($"¡Error! No encuentro la carpeta: {rutaCarpeta}", "Carpeta no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] archivos = Directory.GetFiles(rutaCarpeta, "*.mp3");
            if (archivos.Length == 0) archivos = Directory.GetFiles(rutaCarpeta, "*.wav");

            if (archivos.Length > 0)
            {
                Random rnd = new Random();
                reproductorPrincipal.URL = archivos[rnd.Next(archivos.Length)];
                reproductorPrincipal.Ctlcontrols.play();
            }
            else
            {
                MessageBox.Show($"La carpeta '{rutaCarpeta}' no tiene música mp3 ni wav.", "Carpeta Vacía", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Detenemos el giro del vinilo al cerrar para no consumir recursos
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timerRotacion.Stop();
            base.OnFormClosed(e);
        }

        // --- DIBUJOS: EL DISCO GIRATORIO Y LOS NEONES ---
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Círculo principal
            GraphicsPath pathForma = new GraphicsPath();
            pathForma.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new Region(pathForma);

            // Borde neón
            using (Pen penNeon = new Pen(Color.FromArgb(192, 0, 192), 6))
            {
                g.DrawEllipse(penNeon, 3, 3, this.Width - 6, this.Height - 6);
            }

            // --- EL VINILO ---
            GraphicsState estadoOriginal = g.Save();
            g.TranslateTransform(250, 180);
            g.RotateTransform(anguloVinilo);

            Rectangle rectDisco = new Rectangle(-100, -100, 200, 200);

            using (SolidBrush brushNegro = new SolidBrush(Color.FromArgb(15, 15, 20)))
            {
                g.FillEllipse(brushNegro, rectDisco);
            }

            using (Pen penSurcos = new Pen(Color.FromArgb(30, 30, 40), 1))
            {
                for (int i = 0; i < 6; i++)
                {
                    int r = 30 + (i * 12);
                    g.DrawEllipse(penSurcos, -r, -r, r * 2, r * 2);
                }
            }

            Rectangle rectSello = new Rectangle(-40, -40, 80, 80);
            using (SolidBrush brushSello = new SolidBrush(Color.FromArgb(128, 0, 128)))
            {
                g.FillEllipse(brushSello, rectSello);
            }

            g.FillEllipse(Brushes.Black, -8, -8, 16, 16);

            using (Pen penDetalle = new Pen(Color.White, 2))
            {
                g.DrawLine(penDetalle, 15, 15, 30, 30);
                g.DrawLine(penDetalle, -15, -15, -30, -30);
            }

            g.Restore(estadoOriginal);
        }
    }
}