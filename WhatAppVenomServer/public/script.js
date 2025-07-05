let qrcode = new QRCode(
    document.querySelector(".qrcode")
);
// Initial QR code generation
// with a default message

if (qrCode) {
    console.log(qrCode);
    qrcode.makeCode(data);
}
// Function to generate QR
// code based on user input
// function generateQr() {
//     if (
//         document.querySelector("input")
//             .value === "" ||
//         document.querySelector("input")
//             .value === " ") {
//         alert(
//             "Input Field Can not be blank!"
//         );
//     }
//     else {
//         if (data) {
//             qrcode.makeCode(
//                 data);
//         } else {
//             qrcode.makeCode(
//                 document.querySelector(
//                     "input"
//                 ).value);
//         }
//     }
// }

function generateQrX(datax) {
    data = datax;
    console.log("Test", data);
    qrcode.makeCode(
        data);
}