import { sql } from "@vercel/postgres";
import { readFile } from 'fs/promises';
import path from 'path';
import { fileURLToPath } from 'url';
import dotenv from "dotenv";
dotenv.config();

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const sqlFilePath = path.join(__dirname, '../database/db.sql');

const createDatabase = async () => {
    try {
        const createTablesSQL = await readFile(sqlFilePath, 'utf8');
        await sql.query(createTablesSQL);
        console.log('Tables created (if not already existing)');
    } catch (err) {
        console.error('Error creating the database', err);
    }
};

createDatabase();