# PE26A_DAMC - Prácticas de Programación Estructurada

## 📚 Descripción del Proyecto

Proyecto educativo de prácticas realizadas en **Visual Studio 2022** con **C#** y **Windows Forms**. Implementa múltiples mesas de prácticas enfocadas en programación estructurada, manipulación de matrices dinámicas, gráficos en tiempo real y un juego de batalla naval avanzado.

### Desarrollador
**DAMC** - Prácticas Educativas 2026

---

## 🎯 Mesas de Prácticas

### **Mesa 1: Matrices y Operaciones Básicas**
- ✅ Generación de matrices dinámicas con dimensiones personalizables
- ✅ Degradados de color en escala de grises
- ✅ Generación de números aleatorios y coloreo por valor
- ✅ Operaciones: contar pares/impares, sumar valores
- ✅ Algoritmo de ordenamiento Burbuja
- ✅ Análisis de datos: clasificación por rangos (Alta/Media/Baja)
- ✅ Reproductor de música integrado con controles de volumen

### **Mesa 2: Búsqueda de Palabras y Gráficos Avanzados**
- ✅ Generación de matrices con letras aleatorias
- ✅ Búsqueda lineal de palabras completas en matrices
- ✅ Dibujo dinámico de líneas entre celdas
- ✅ Reproductor de música vintage con Windows Media Player
- ✅ Sliders personalizados para progreso y volumen
- ✅ Sistema de eventos para pintura en tiempo real

### **Mesa 3: Interfaz de Navegación**
- ✅ Sistema modular de paneles
- ✅ Navegación fluida entre diferentes prácticas
- ✅ Gestión de visibilidad de componentes

### **Mesa 4: Juego de Batalla Naval Avanzado** 🎮
- ✅ Modo 1v1 (Jugador vs Jugador)
- ✅ Modo vs IA con dificultades (Fácil/Experto)
- ✅ Sistema de economía y tienda dentro del juego
- ✅ Armas especiales: Minas, Misiles de área, Sonar 3x3
- ✅ Radar dinámico animado con intercomunicador
- ✅ Sistema climático: Tormentas magnéticas y niebla
- ✅ Maniobra de evasión para submarinos
- ✅ Sistema de puntos y turnos extra
- ✅ Música de fondo (BGM) y efectos de sonido (SFX)
- ✅ IA inteligente que recuerda objetivos
- ✅ Interfaz táctica militar retro con colores verde lima

---

## 🛠️ Tecnologías Utilizadas

| Tecnología | Descripción |
|-----------|-------------|
| **Lenguaje** | C# (.NET Framework) |
| **Framework UI** | Windows Forms |
| **Multimedia** | Windows Media Player (WMP) |
| **Gráficos** | System.Drawing |
| **Control de eventos** | Delegados y eventos .NET |
| **Sonido** | GestorSonido (clase personalizada) |

---

## 📂 Estructura del Proyecto

```
PE26A_DAMC/
├── Form1.cs                          # Formulario principal
├── DlgMesaPracticas1.cs             # Prácticas 1: Matrices básicas
├── DlgMesaPracticas2.cs             # Prácticas 2: Búsqueda y gráficos
├── DlgMesaPracticas3.cs             # Prácticas 3: Navegación
├── DlgMesaPracticas4.cs             # Prácticas 4: Batalla naval
├── GestorSonido.cs                  # Clase para manejar audio
├── CircularPictureBox.cs            # Control personalizado
├── OvalBocaControl.cs               # Control personalizado
└── Properties/                       # Recursos del proyecto
```

---

## 🎮 Cómo Jugar - Batalla Naval

### **Fase 1: Colocación de Barcos**
1. Selecciona un barco del menú desplegable
2. Haz clic en tu tablero para colocarlo
3. Usa **"Girar"** para cambiar orientación (Horizontal/Vertical)
4. Usa **"Auto"** para generar flota aleatoria
5. Usa **"Reacomodar"** para limpiar y empezar de nuevo
6. **"Confirmar"** cuando toda tu flota esté lista

### **Fase 2: Batalla**
1. **Turno básico**: Haz clic en una celda del tablero enemigo
2. **Compra armas**: Usa los puntos ganados por impactos
   - **Mina ($30)**: Coloca en tu tablero para trampa
   - **Misil ($80)**: Explota en área de cruz (5 celdas)
   - **Sonar ($40)**: Escanea área 3x3
3. **Maniobra Evasión**: Si solo te queda 1 submarino, puedes reubicarlo
4. **Radar**: Después de 3 fallos, recibirás una pista de ubicación enemiga
5. **Clima**: Eventos aleatorios que afectan el juego

### **Sistema de Puntos**
- Impacto en barco: **+10 puntos**
- Barco hundido: **+40 puntos adicionales**
- Ganador final: **Primero en hundir toda la flota enemiga**

---

## 📋 Requisitos del Sistema

- **SO**: Windows 7 o superior
- **Framework**: .NET Framework 4.7.2+
- **Memoria**: Mínimo 512 MB RAM
- **Sonido**: Tarjeta de audio (opcional, para efectos de sonido)

---

## 🚀 Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/OMNIWON/PE26A_DAMC.git
   ```

2. **Abrir en Visual Studio**
   ```bash
   cd PE26A_DAMC
   # Doble clic en PE26A_DAMC.sln
   ```

3. **Compilar y ejecutar**
   - Visual Studio → Build → Build Solution (Ctrl+Shift+B)
   - Debug → Start Debugging (F5)

---

## 📝 Documentación del Código

Todo el código incluye **documentación XML** para:
- ✅ Descripción de métodos
- ✅ Parámetros y valores de retorno
- ✅ Ejemplos de uso
- ✅ Notas importantes

La documentación aparece automáticamente en Visual Studio al pasar el ratón sobre un método.

---

## 🎓 Conceptos Educativos Cubiertos

- ✅ Arrays multidimensionales
- ✅ Lógica de búsqueda y ordenamiento
- ✅ Programación orientada a eventos
- ✅ Gráficos y renderizado en tiempo real
- ✅ Interfaz gráfica (Windows Forms)
- ✅ Manejo de archivos multimedia
- ✅ Algoritmos de IA básicos
- ✅ Gestión de estado y turnos
- ✅ Sistemas de puntuación y economía

---

## 🐛 Conocidos

- La ruta de canciones en DlgMesaPracticas1 está hardcodeada (cambiar según tu sistema)
- En sistemas con múltiples monitores, el radar puede aparecer fuera de pantalla (ajustar posición)

---

## 📞 Contacto & Soporte

Para preguntas o reportar errores, abre un **Issue** en GitHub.

---

## 📄 Licencia

Proyecto educativo - Uso libre para fines académicos.

---

**Última actualización**: Mayo 2026  
**Estado**: ✅ En desarrollo activo
