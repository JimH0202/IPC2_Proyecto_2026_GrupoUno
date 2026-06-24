# OrbitNet - Simulador de Red de Satélites

## 📡 Descripción

OrbitNet es un simulador de constelaciones de satélites de órbita baja y media (LEO/MEO) que modelan la interacción entre satélites y estaciones terrestres receptoras. El sistema representa el tránsito de paquetes de datos críticos entre satélites utilizando enlaces lógicos para buscar rutas de descarga óptimas.

## 🏗️ Arquitectura

El proyecto utiliza una **arquitectura distribuida con dos instancias simultáneas**:

- **Instancia 1 - Hemisferio Norte**: Puerto 5000
  - Constelación de satélites polares y ecuatoriales del norte
  - Acceso: http://localhost:5000

- **Instancia 2 - Hemisferio Sur**: Puerto 5001
  - Constelación de satélites polares y ecuatoriales del sur
  - Acceso: http://localhost:5001

Cuando un paquete ingresa a una constelación pero su destino está en la otra, el sistema realiza un **salto HTTP síncrono** mediante POST seguro hacia la instancia hermana.

## 🚀 Ejecución

### Requisitos
- .NET 10.0 SDK instalado
- Dos terminales (PowerShell, Bash, o similar)
- Navegador web

### Paso 1: Compilar el proyecto

```bash
cd Orbinet.Web
dotnet build
