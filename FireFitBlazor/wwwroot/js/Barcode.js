// wwwroot/js/barcode.js
window.barcodeInterop = {
    startScan: (dotNetHelper) => {
        const qrCode = new Html5Qrcode("reader");
        qrCode.start(
            { facingMode: "environment" },
            {
                fps: 10,
                qrbox: 250
            },
            (decodedText) => {
                dotNetHelper.invokeMethodAsync("OnBarcodeScanned", decodedText);
                qrCode.stop();
            },
            (errorMessage) => {
                console.warn("Scan error", errorMessage);
            }
        );
    }
};
