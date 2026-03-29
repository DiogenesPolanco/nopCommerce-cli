import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const distPath = path.join(__dirname, '..', 'dist', 'nopcli.js');
const shebang = '#!/usr/bin/env node\n';

if (fs.existsSync(distPath)) {
    let content = fs.readFileSync(distPath, 'utf8');
    if (!content.startsWith(shebang)) {
        fs.writeFileSync(distPath, shebang + content);
        console.log('Shebang added successfully');
    }
}
