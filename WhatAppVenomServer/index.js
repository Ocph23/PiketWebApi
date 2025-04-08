
const express = require('express');
const qrcode = require('qrcode-terminal');
const { Client, LocalAuth } = require('whatsapp-web.js');
const db = require('./db.js');

const app = express();
app.use(express.json());

const client = new Client({
    authStrategy: new LocalAuth()
});

client.on('qr', (qr) => {
    // Generate and scan this code with your phone
    console.log('QR RECEIVED', qr);
    qrcode.generate(qr, { small: true });
});

client.on('ready', () => {
    console.log('Client is ready!');
});

client.on('message', async (msg) => {
    var message = msg.body.toLowerCase();
    var messageSplit = message.split('#');
    if (messageSplit[0] == 'register') {
        console.log(messageSplit[1]);
        if (messageSplit[1] != '') {
            await registerNomorTelepon(msg.from, messageSplit[1]);
        } else {
            client.sendMessage(msg.from, `Silahkan kirim pesan dengan format register#<NIS/NISN>`);
        }
    }
});

async function registerNomorTelepon(nomorTelepon, nis) {
    const telp = nomorTelepon.split('@')[0];
    ///cek di database jika nomor telepon sudah terdaftar
    try {
        const result = await db.query('select * from "Students" WHERE "ParentPhoneNumber" = $1', [telp]);
        if (result.rows.length > 0) {
            if (result.rows[0].NIS == nis || result.rows[0].NISN == nis) {
                client.sendMessage(nomorTelepon, `Nomor telepon  ${telp} sudah terdaftar untuk siswa ${result.rows[0].NIS} - ${result.rows[0].Name}`);
                return;
            } else {
                client.sendMessage(nomorTelepon, `Nomor telepon  ${telp} sudah terdaftar untuk siswa lain, hubungi wali kelas untuk mengubah data`);
                return;
            }
        }
        const result1 = await db.query('select * from "Students" WHERE "NIS" = $1 or "NISN" = $1', [nis]);
        if (result1.rows.length <= 0) {
            client.sendMessage(nomorTelepon, `NIS/NISN ${nis} tidak ditemukan `);
            return;
        }

        if (result1.rows[0].ParentPhoneNumber) {
            client.sendMessage(nomorTelepon, `${nis} sudah menggunakan nomor telepon yang lain, hubungi wali kelas untuk mengubah data`);
            return;
        }
        const student = result1.rows[0];
        await db.query('UPDATE "Students" SET "ParentPhoneNumber" = $1 WHERE "Id" = $2', [telp, student.Id]);
        client.sendMessage(nomorTelepon, `Nomor telepon ${telp} berhasil didaftarkan untuk siswa ${nis} - ${student.Name}`);
        console.log('result:', student);
    } catch (err) {
        console.error(err);
    }
}

app.post('/absen', async (req, res) => {
    const { to, message } = req.body;
    const address = `${to}@c.us`
    console.log(address, message);
    client.sendMessage(address, message).then((result) => {
        console.log(result);
    })
});

app.listen(3000, () => {
    client.initialize();
    console.log("🚀 API WhatsApp aktif di http://localhost:3000");
});