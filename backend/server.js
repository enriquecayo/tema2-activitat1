import express from 'express';
import cors from 'cors';
import fs from 'fs';
import crypto from 'crypto';

const app = express();
const PORT = 3000;
const DB_FILE = './hash.json';

app.use(cors());
app.use(express.json());

// en un entorn real, guardar en (.env)
const ALGORITHM = 'aes-256-cbc';
const KEY = crypto.scryptSync('123', 'salt', 32);
const IV = Buffer.alloc(16, 0);

// funcions

function guardarHash(passwordHash) {
    let dades = [];
    try {
        if (fs.existsSync(DB_FILE)) {
            dades = JSON.parse(fs.readFileSync(DB_FILE, 'utf-8'));
        }
        dades.push({ hash: passwordHash, date: new Date().toISOString() });
        fs.writeFileSync(DB_FILE, JSON.stringify(dades, null, 2));
    } catch (e) {
        console.error("Error guardant fitxer:", e);
    }
}

function obtenirUltimHash() {
    try {
        if (fs.existsSync(DB_FILE)) {
            const dades = JSON.parse(fs.readFileSync(DB_FILE, 'utf-8'));
            if (dades.length > 0) {
                return dades[dades.length - 1].hash;
            }
        }
    } catch (e) { console.error(e); }
    return null;
}


// 1. POST /encrypt
app.post('/encrypt', (req, res) => {
    const { text } = req.body;
    if (!text) return res.status(400).json({ error: "Falta el text" });

    const cipher = crypto.createCipheriv(ALGORITHM, KEY, IV);
    let encrypted = cipher.update(text, 'utf8', 'hex');
    encrypted += cipher.final('hex');

    console.log(`Text encriptat: ${text} -> ${encrypted}`);
    res.json({ encrypted: encrypted });
});

// 2. POST /decrypt
app.post('/decrypt', (req, res) => {
    const { encrypted } = req.body;
    if (!encrypted) return res.status(400).json({ error: "Falta el text encriptat" });

    try {
        const decipher = crypto.createDecipheriv(ALGORITHM, KEY, IV);
        let decrypted = decipher.update(encrypted, 'hex', 'utf8');
        decrypted += decipher.final('utf8');

        console.log(`Text desencriptat: ${encrypted} -> ${decrypted}`);
        res.json({ text: decrypted });
    } catch (e) {
        res.status(500).json({ error: "Error al desencriptar. Potser el format és incorrecte." });
    }
});

// 3. POST /hash (Generar hash i guardar a disc)
app.post('/hash', (req, res) => {
    const { password } = req.body;
    if (!password) return res.status(400).json({ error: "Falta el password" });

    const hash = crypto.createHash('sha256').update(password).digest('hex');
    
    guardarHash(hash);

    console.log(`Hash generat i guardat: ${hash}`);
    res.json({ hash: hash });
});

// 4. POST /verify (Comparar amb el hash guardat)
app.post('/verify', (req, res) => {
    const { password } = req.body;
    if (!password) return res.status(400).json({ error: "Falta el password" });

    const ultimHashGuardat = obtenirUltimHash();
    
    if (!ultimHashGuardat) {
        return res.json({ ok: false, message: "No hi ha cap hash guardat al servidor" });
    }

    const hashIntent = crypto.createHash('sha256').update(password).digest('hex');

    const coincideix = (hashIntent === ultimHashGuardat);
    
    console.log(`Verificació: ${coincideix ? "CORRECTE" : "INCORRECTE"}`);
    res.json({ ok: coincideix });
});

// server
app.listen(PORT, () => {
    console.log(`Servidor HTTP escoltant a http://localhost:${PORT}`);
});