using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE26A_DAMC
{
    // ========================================================================
    // GESTOR DE SONIDO
    // Usa System.Windows.Media.MediaPlayer (nombre completo para evitar
    // conflicto con System.Drawing.Color / Brush en el resto del archivo).
    // Requiere referencias: PresentationCore y WindowsBase en el proyecto.
    // Carpeta de archivos: <ejecutable>\sonidos\  (ej. sfx_agua.mp3)
    // ========================================================================
    public static class GestorSonido
    {
        // Un único reproductor para la música de fondo (BGM en bucle)
        private static System.Windows.Media.MediaPlayer reproductorBGM
            = new System.Windows.Media.MediaPlayer();

        // Bandera para no re-suscribir el evento de loop más de una vez
        private static bool loopSuscrito = false;

        // ────────────────────────────────────────────────────────────────────
        // Música de Fondo — se reproduce en bucle hasta que se llame DetenerBGM
        // ────────────────────────────────────────────────────────────────────
        public static void ReproducirBGM(string nombreArchivo)
        {
            try
            {
                // Parar y descargar lo que hubiera antes
                reproductorBGM.Stop();
                reproductorBGM.Close();

                string ruta = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "sonidos", nombreArchivo);

                reproductorBGM.Open(new Uri(ruta, UriKind.Absolute));

                // Suscribir el loop sólo la primera vez (evita handlers duplicados)
                if (!loopSuscrito)
                {
                    reproductorBGM.MediaEnded += BGM_Loop;
                    loopSuscrito = true;
                }

                reproductorBGM.Play();
            }
            catch (Exception ex)
            {
                // El juego no debe romperse si falta un archivo de sonido
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] BGM error: {ex.Message}");
            }
        }

        // Reinicia la posición y vuelve a reproducir (loop manual)
        private static void BGM_Loop(object sender, EventArgs e)
        {
            reproductorBGM.Position = TimeSpan.Zero;
            reproductorBGM.Play();
        }

        // Detiene y descarga la música de fondo por completo
        public static void DetenerBGM()
        {
            try
            {
                reproductorBGM.Stop();
                reproductorBGM.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] DetenerBGM error: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // Efectos de Sonido — cada SFX crea su propio reproductor desechable,
        // por lo que varios efectos pueden sonar simultáneamente sin cortar
        // la música de fondo.
        // ────────────────────────────────────────────────────────────────────
        public static void ReproducirSFX(string nombreArchivo)
        {
            try
            {
                System.Windows.Media.MediaPlayer sfx = new System.Windows.Media.MediaPlayer();

                string ruta = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "sonidos", nombreArchivo);

                sfx.Open(new Uri(ruta, UriKind.Absolute));
                sfx.Play();

                // Auto-limpieza: cuando termina el SFX se libera el reproductor
                sfx.MediaEnded += (s, e) =>
                {
                    sfx.Close();
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] SFX error '{nombreArchivo}': {ex.Message}");
            }
        }

        // Limpieza forzada de todos los recursos (llamar al cerrar el formulario)
        public static void LimpiarTodo()
        {
            try
            {
                reproductorBGM.Stop();
                reproductorBGM.Close();

                if (loopSuscrito)
                {
                    reproductorBGM.MediaEnded -= BGM_Loop;
                    loopSuscrito = false;
                }

                System.Diagnostics.Debug.WriteLine("[GestorSonido] ✅ Limpieza completada.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] LimpiarTodo error: {ex.Message}");
            }
        }
    }
}
