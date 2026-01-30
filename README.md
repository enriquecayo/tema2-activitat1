# Activitat 1: Encriptació Simètrica i Hashing

Aquest projecte és una aplicació client-servidor desenvolupada per demostrar conceptes fonamentals de criptografia: encriptació simètrica (AES-256) i hashing (SHA-256) amb verificació.

El projecte consta de dues parts:
1.  **Backend (Node.js):** Una API REST que processa les peticions de criptografia.
2.  **Client (Unity):** Una interfície gràfica creada amb UI Toolkit per interactuar amb el servidor.

---

## Tecnologies Utilitzades

*   **Servidor:** Node.js, Express, Crypto (nadiu), CORS.
*   **Client:** Unity (C#), UI Toolkit (UXML/USS).
*   **Persistència:** Fitxer JSON local (`hash.json`).

---

## Instal·lació i Ús

Perquè l'aplicació funcioni, cal tenir el servidor en marxa abans d'utilitzar el client Unity.

### 1. Configuració del Backend (Node.js)

Assegura't de tenir **Node.js** instal·lat.

1.  Obre una terminal a la carpeta del servidor.
2.  Instal·la les dependències necessàries:
    ```bash
    npm install
    ```
3.  Inicia el servidor:
    ```bash
    node server.js
    ```
4.  Veuràs el missatge: `Servidor HTTP escoltant a http://localhost:3000`

### 2. Configuració del Client (Unity)

1.  Obre el projecte amb **Unity Hub**.
2.  Obre l'escena principal (on hi ha el `UIDocument`).
3.  Prem el botó **Play** ▶️.
4.  Utilitza la interfície per enviar dades al servidor.

---

## 📡 Documentació de l'API

El servidor escolta al port **3000** i accepta peticions `POST` amb format JSON.

| Mètode | Endpoint | Descripció | Entrada (JSON) | Sortida (JSON) |
| :--- | :--- | :--- | :--- | :--- |
| **POST** | `/encrypt` | Encripta un text (AES-256). | `{ "text": "hola" }` | `{ "encrypted": "a8f..." }` |
| **POST** | `/decrypt` | Desencripta un text xifrat. | `{ "encrypted": "a8f..." }` | `{ "text": "hola" }` |
| **POST** | `/hash` | Genera un hash SHA-256 i el guarda. | `{ "password": "123" }` | `{ "hash": "xy9..." }` |
| **POST** | `/verify` | Compara un password amb l'últim hash. | `{ "password": "123" }` | `{ "ok": true }` |

---

## Estructura del Projecte

*   `/Server`: Conté el codi `server.js`, `package.json` i la base de dades `hash.json`.
*   `/UnityProject`: Conté els Assets, Scripts (`APIManager.cs`) i la interfície UXML.

---

## Autor

Activitat realitzada per al mòdul M09 Programació de serveis i processos, Enrique Manuel Cayo Moye. 
