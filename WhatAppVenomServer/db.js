require('dotenv').config()

const user = process.env.user;
const password = process.env.password;
const host = process.env.host;
const database = process.env.database;
const port = process.env.port;


const { Pool } = require('pg');

const pool = new Pool({
    user: user,
    password: password,
    host: host,
    port: port, // default Postgres port
    database: database
});

module.exports = {
    query: (text, params) => pool.query(text, params)
};