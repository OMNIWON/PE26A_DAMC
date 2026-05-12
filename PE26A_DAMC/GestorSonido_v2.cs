using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PE26A_DAMC
{
    // ========================================================================
    // GESTOR DE SONIDO v2 - MEJORADO
    // ✅ Arregla: BGM que se queda activo al cerrar ventana
    // ✅ Memory leak prevention
    // ✅ Mejor manejo de excepciones
    // ========================================================================
    public static class GestorSonido
    {
        // Un único reproductor para la música de fondo (BGM en bucle)
        private static System.Windows.Media.MediaPlayer reproductorBGM
            = new System.Windows.Media.MediaPlayer();

        // Bandera para no re-suscribir el evento de loop más de una vez
        private static bool loopSuscrito = false;
        
        // Bandera para rastrear si hay BGM actualmente reproduciendo
        private static bool bgmActivo = false;
        
        // Nombre del BGM actual (para evitar reproducir el mismo)
        private static string bgmActualNombre = "";

        // ─────────────────────────────────────────────────────────────────
        // Música de Fondo — se reproduce en bucle hasta que se llame DetenerBGM
        // ─────────────────────────────────────────────────────────────────
        public static void ReproducirBGM(string nombreArchivo)
        {
            try
            {
                // Si ya está tocando la misma canción, no reiniciar
                if (bgmActivo && bgmActualNombre == nombreArchivo)
                    return;

                // Parar y descargar lo que hubiera antes
                reproductorBGM.Stop();
                reproductorBGM.Close();

                string ruta = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "sonidos", nombreArchivo);

                // Verificar que el archivo existe
                if (!System.IO.File.Exists(ruta))
                {
                    System.Diagnostics.Debug.WriteLine($"[GestorSonido] Archivo no encontrado: {ruta}");
                    return;
                }

                reproductorBGM.Open(new Uri(ruta, UriKind.Absolute));

                // Suscribir el loop sólo la primera vez (evita handlers duplicados)
                if (!loopSuscrito)
                {
                    reproductorBGM.MediaEnded += BGM_Loop;
                    loopSuscrito = true;
                }

                reproductorBGM.Play();
                bgmActivo = true;
                bgmActualNombre = nombreArchivo;
                
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] ▶️  BGM iniciado: {nombreArchivo}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] ❌ BGM error: {ex.Message}");
                bgmActivo = false;
            }
        }

        // Reinicia la posición y vuelve a reproducir (loop manual)
        private static void BGM_Loop(object sender, EventArgs e)
        {
            try
            {
                if (bgmActivo)
                {
                    reproductorBGM.Position = TimeSpan.Zero;
                    reproductorBGM.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] ❌ BGM_Loop error: {ex.Message}");
            }
        }

        // Detiene y descarga la música de fondo por completo
        public static void DetenerBGM()
        {
            try
            {
                reproductorBGM.Stop();
                reproductorBGM.Close();
                bgmActivo = false;
                bgmActualNombre = "";
                System.Diagnostics.Debug.WriteLine("[GestorSonido] ⏹️  BGM detenido");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] ❌ DetenerBGM error: {ex.Message}");
            }
        }

        // Pausa sin descargar (útil para menús)
        public static void PausarBGM()
        {
            try
            {
                reproductorBGM.Pause();
                System.Diagnostics.Debug.WriteLine("[GestorSonido] ⏸️  BGM pausado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] ❌ PausarBGM error: {ex.Message}");
            }
        }

        // Reanuda desde la pausa
        public static void ReanudarBGM()
        {
            try
            {
                if (bgmActivo)
                {
                    reproductorBGM.Play();
                    System.Diagnostics.Debug.WriteLine("[GestorSonido] ▶️  BGM reanudado");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] ❌ ReanudarBGM error: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Efectos de Sonido — cada SFX crea su propio reproductor desechable
        // ─────────────────────────────────────────────────────────────────
        public static void ReproducirSFX(string nombreArchivo)
        {
            // Ejecutar en thread separado para no bloquear la UI
            Task.Run(() =>
            {
                try
                {
                    System.Windows.Media.MediaPlayer sfx = new System.Windows.Media.MediaPlayer();

                    string ruta = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory, "sonidos", nombreArchivo);

                    // Verificar que el archivo existe
                    if (!System.IO.File.Exists(ruta))
                    {
                        System.Diagnostics.Debug.WriteLine($"[GestorSonido] SFX no encontrado: {ruta}");
                        return;
                    }

                    sfx.Open(new Uri(ruta, UriKind.Absolute));
                    sfx.Play();

                    // Auto-limpieza: cuando termina el SFX se libera el reproductor
                    sfx.MediaEnded += (s, e) =>
                    {
                        try
                        {
                            sfx.Close();
                            sfx.Dispose();
                        }
                        catch { }
                    };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GestorSonido] ❌ SFX error '{nombreArchivo}': {ex.Message}");
                }
            });
        }

        // Limpieza forzada (llamar en FormClosing)
        public static void LimpiarTodo()
        {
            try
            {
                DetenerBGM();
                System.Diagnostics.Debug.WriteLine("[GestorSonido] 🧹 Todos los sonidos limpiados");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GestorSonido] ❌ LimpiarTodo error: {ex.Message}");
            }
        }
    }
}
