require('dotenv').config()

const host = process.env.DATABASE_URL;

const { Pool } = require('pg');

const pool = new Pool({
    connectionString: host
});

module.exports = {
    query: (text, params) => pool.query(text, params)
};