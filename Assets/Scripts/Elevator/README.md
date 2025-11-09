# 🛗 Sistema de Elevador - Documentación Actualizada

## ⚠️ CAMBIOS IMPORTANTES - REFACTORIZACIÓN COMPLETADA

**Fecha**: Noviembre 2025  
**Estado**: ✅ Refactorización Completada  
**Impacto**: Arquitectura mejorada, eliminada redundancia

---

## 🎯 Lo Que Cambió

### Archivos Eliminados (Reemplazados)
- ❌ **ElevatorInteractionSystem.cs** → Reemplazado por RaycastDetector
- ❌ **ElevatorInteractionSystemEnhanced.cs** → Reemplazado por RaycastDetector

### Archivos Actualizados (Mejorados)
- ✅ **ElevatorButton.cs** - Limpio, documentado, interfaz IInteractable
- ✅ **ElevatorDoor.cs** - Limpio, documentado, interfaz IInteractable

### Nuevo Sistema Central
- 🎯 **RaycastDetector.cs** (ubicado en `Assets/Scripts/`)
  - Centraliza toda la detección de raycast
  - Sistema único para interacciones
  - Notifica vía IInteractable

---

## 📊 Arquitectura Nueva

```
┌─────────────────────────────────┐
│   Input del Jugador (E)         │
└────────────────┬────────────────┘
                 │
┌────────────────▼─────────────────────┐
│   RaycastDetector (CENTRAL)         │
│   • Detecta objetos IInteractable   │
│   • Llamadas: OnLookAt / OnLookAway │
│   • Llamadas: OnInteract            │
└────────────────┬─────────────────────┘
         ┌───────┴───────┐
    ┌────▼────┐     ┌────▼────┐
    │ Botón   │     │ Puerta  │
    │ ✅      │     │ ✅      │
    │IInteract│     │IInteract│
    │  able   │     │  able   │
    └────┬────┘     └────┬────┘
         │               │
    ┌────▼───────────────▼────┐
    │  Acciones Específicas    │
    │  • Presionar botón       │
    │  • Abrir/Cerrar puerta   │
    └──────────────────────────┘
```

---

## 🔄 Flujo de Interacción

```
1. Jugador apunta a botón
   ↓
2. RaycastDetector detecta
   ↓
3. RaycastDetector.OnLookAt()
   └─ Botón se resalta (visual)
   ↓
4. Jugador presiona E
   ↓
5. RaycastDetector.Interact()
   └─ RaycastDetector.OnInteract()
   └─ Botón presiona elevador
   ↓
6. Jugador mira otro lado
   ↓
7. RaycastDetector.OnLookAway()
   └─ Botón vuelve a normal
```

---

## 🛠️ Componentes Principales

### RaycastDetector (Assets/Scripts/)
**Responsabilidad**: Detectar objetos interactuables  
**Métodos Públicos**:
- `Interact()` - Ejecutar interacción
- `IsLookingAtInteractable()` - Verificar si hay target
- `GetCurrentTargetName()` - Nombre del objeto actual
- `SetInteractRange(float)` - Cambiar rango dinámicamente

### ElevatorButton (Assets/Scripts/Elevator/)
**Responsabilidad**: Presionar botón y llamar elevador  
**Interfaz**: IInteractable  
**Métodos**:
- `OnLookAt()` - Resaltar botón
- `OnLookAway()` - Restaurar visual
- `OnInteract()` - Presionar botón

### ElevatorDoor (Assets/Scripts/Elevator/)
**Responsabilidad**: Abrir/Cerrar puertas  
**Interfaz**: IInteractable, IElevatorDoor  
**Métodos**:
- `OnLookAt()` - Resaltar puerta
- `OnLookAway()` - Restaurar visual
- `OnInteract()` - Alternar puerta
- `Open()` - Abrir
- `Close()` - Cerrar
- `Toggle()` - Alternar
- `SetLocked(bool)` - Bloquear/Desbloquear

---

## 📋 Configuración en Escena

### Paso 1: RaycastDetector
```
Jerarquía:
└─ Main Camera
   └─ RaycastDetector.cs ✅

Inspector:
- Player Camera: Auto (o asignar)
- Interact Range: 5m
- Detection Layer: All (-1) o específica
- Show Debug Logs: true (testing)
- Show Gizmos: true (debugging)
```

### Paso 2: Botones
```
Jerarquía:
└─ ElevatorButton_Floor1
   ├─ Collider (BoxCollider, SphereCollider, etc.)
   ├─ Renderer (para resalte visual)
   ├─ ElevatorButton.cs
   ├─ Material para resalte
   └─ AudioSource (opcional)

Inspector (ElevatorButton):
- Elevator Controller: ElevatorControllerEnhanced
- Elevator Height: altura del piso
- Use Fixed Height: true
- Highlight Material: Material de resalte
```

### Paso 3: Puertas
```
Jerarquía:
└─ ElevatorDoor_Left
   ├─ Collider (BoxCollider, MeshCollider)
   ├─ Renderer
   ├─ ElevatorDoor.cs
   ├─ ItemHighlight.cs (auto-agregado)
   ├─ Opening Sounds/ (carpeta)
   │  ├─ AudioSource (sonido 1)
   │  └─ AudioSource (sonido 2)
   └─ Closing Sounds/ (carpeta)
      ├─ AudioSource (sonido 3)
      └─ AudioSource (sonido 4)

Inspector (ElevatorDoor):
- Door Type: Hinged o Accordion
- Open Angle: 90-120 grados
- Animation Speed: 0.1
```

---

## 🎮 Integración con Input

El sistema espera que se llame a `RaycastDetector.Interact()` cuando se presiona E:

```csharp
public class PlayerInputManager : MonoBehaviour
{
    private RaycastDetector raycastDetector;

    private void Start()
    {
        raycastDetector = Camera.main.GetComponent<RaycastDetector>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact")) // E
        {
            raycastDetector.Interact();
        }
    }
}
```

---

## 🔍 Interfaces

### IInteractable
```csharp
public interface IInteractable
{
    void OnLookAt();      // Cuando comienza a mirar
    void OnLookAway();    // Cuando deja de mirar
    void OnInteract();    // Cuando presiona E
}
```

**Implementado por**:
- ✅ ElevatorButton
- ✅ ElevatorDoor

### IElevatorDoor
```csharp
public interface IElevatorDoor
{
    void Open();
    void Close();
    void Toggle();
    void SetLocked(bool isLocked);
    DoorState CurrentState { get; }
    bool IsLocked { get; }
    bool IsFullyClosed { get; }
    bool IsFullyOpen { get; }
}
```

**Implementado por**:
- ✅ ElevatorDoor

---

## 🧪 Validación

### Usar Herramienta de Validación
```
Window → Lineaf Horror → Elevator System Validator
```

Verifica automáticamente:
- ✅ RaycastDetector presente en escena
- ✅ Botones tienen Collider, Renderer, IInteractable
- ✅ Puertas tienen Collider, IInteractable
- ✅ No hay archivos deprecados
- ✅ Todas las interfaces implementadas

### Testing Manual
1. Entra en Play Mode
2. Apunta a un botón → Debe resaltarse ✅
3. Presiona E → Debe presionar botón ✅
4. Elevador se mueve → Verifica puertas se abren ✅
5. Apunta a puerta → Debe resaltarse ✅
6. Presiona E → Debe abrirse/cerrarse ✅

---

## 🐛 Debugging

### Activar Logs
En RaycastDetector, activar:
- ✅ **Show Debug Logs** - Ver detección en consola
- ✅ **Show Gizmos** - Ver raycast visual en editor

### Logs Esperados
```
[RaycastDetector] Detectado: ElevatorButton_Floor1 (IInteractable)
[RaycastDetector] Interactuando con: ElevatorButton_Floor1
[ElevatorButton] Botón ElevatorButton_Floor1 presionado - Elevador a altura 10
[ElevatorDoor] ElevatorDoor_Left abierta completamente
```

### Gizmos
- 🟢 Verde: Raycast tocó algo (target)
- 🔵 Azul: Raycast (objeto seleccionado)
- 🟡 Amarillo: Raycast (normal)
- 🔴 Rojo: Raycast no tocó nada

---

## ⚠️ Problemas Comunes

| Problema | Solución |
|----------|----------|
| No se detecta botón | Verificar Collider (no-trigger), Layer correcto |
| No se resalta | Verificar Renderer, Material de resalte asignado |
| No interactúa | Verificar que RaycastDetector.Interact() se llama |
| No hay sonido | Verificar AudioSource, clips asignados |
| Puertas no se abren | Verificar que no estén locked |
| Raycast no ve objetos | Verificar Layer, distancia, obstáculos |

---

## 📚 Documentación Adicional

1. **INTEGRATION_GUIDE.md** - Guía detallada (este archivo)
2. **Guía de Integración (Artifact)** - Documentación visual completa
3. **ElevatorSystemValidator.cs** - Herramienta de validación
4. **Análisis de Limpieza (Artifact)** - Detalles técnicos
5. **Resumen Ejecutivo (Artifact)** - Overview general

---

## ✅ Buenas Prácticas Aplicadas

✅ **DRY** - Sin código duplicado (un solo RaycastDetector)  
✅ **SOLID** - Cada clase tiene una responsabilidad  
✅ **Interfaz Consistente** - Todos usan IInteractable  
✅ **Bajo Acoplamiento** - Componentes independientes  
✅ **Bien Documentado** - Código comentado y guías  
✅ **Fácil de Extender** - Patrón reutilizable  
✅ **Testeable** - Cada componente testeable  

---

## 🚀 Beneficios

### Antes (Anterior)
```
❌ Dos sistemas de raycast (redundancia)
❌ Mantenimiento difícil
❌ Difícil de extender
❌ Acoplamiento alto
❌ ~400 líneas de código duplicado
```

### Después (Actual)
```
✅ Un solo sistema central (RaycastDetector)
✅ Fácil de mantener
✅ Fácil de extender a nuevos objetos
✅ Bajo acoplamiento
✅ Código limpio y reutilizable
```

---

## 🔮 Extensiones Futuras

Gracias a esta arquitectura, es fácil agregar:

### Nuevos Objetos Interactuables
```csharp
public class Lever : MonoBehaviour, IInteractable
{
    public void OnLookAt() { /* resaltar */ }
    public void OnLookAway() { /* normal */ }
    public void OnInteract() { /* acción */ }
}
```

### UI de Interacción
```csharp
public class InteractionUI : MonoBehaviour
{
    private RaycastDetector raycastDetector;
    
    private void Update()
    {
        if (raycastDetector.HasTarget)
            ShowPrompt();
        else
            HidePrompt();
    }
}
```

### Diálogos y Cutscenes
```csharp
public class NPC : MonoBehaviour, IInteractable
{
    public void OnInteract() => dialogueSystem.StartConversation();
}
```

---

## 📞 Soporte

### Si algo no funciona:
1. Ejecuta **ElevatorSystemValidator** (Window > Lineaf Horror)
2. Revisa la consola (Ctrl+Shift+C) - Logs detallados
3. Verifica que RaycastDetector tiene "Show Debug Logs" activado
4. Revisa esta documentación
5. Revisa archivos .meta (a veces causa problemas)

### Ubicación de Archivos
```
Assets/
├─ Scripts/
│  ├─ RaycastDetector.cs ⭐ CENTRAL
│  └─ Elevator/
│     ├─ ElevatorButton.cs ✅
│     ├─ ElevatorDoor.cs ✅
│     ├─ ElevatorController.cs
│     ├─ ElevatorControllerEnhanced.cs
│     ├─ IInteractable.cs
│     ├─ IElevatorDoor.cs
│     ├─ INTEGRATION_GUIDE.md
│     ├─ README.md (este archivo)
│     └─ .DEPRECATED_* (archivos antiguos)
```

---

## ✨ Conclusión

**El sistema de elevador ha sido refactorizado a un diseño limpio y profesional.**

- ✅ Arquitectura mejorada
- ✅ Código centralizado y reutilizable
- ✅ Documentación completa
- ✅ Herramientas de validación
- ✅ Listo para producción
- ✅ Fácil de extender

**Estado**: 🎉 Refactorización completa y verificada