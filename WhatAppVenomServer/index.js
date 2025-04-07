
const express = require('express');
const app = express();
app.use(express.json());
const { Client, LocalAuth } = require('whatsapp-web.js');
const qrcode = require('qrcode-terminal');

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

client.on('message', msg => {
    if (msg.body == '!ping') {
        msg.reply('pong');
    } else {
        msg.reply(msg.body);
    }
});

app.post('/send-message', async (req, res) => {
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