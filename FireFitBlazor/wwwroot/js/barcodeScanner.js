window.barcodeScannerManual = {
    stream: null,
    video: null,
    dotNetRef: null,

    startCamera: async function (dotNetRef) {
        this.dotNetRef = dotNetRef;

        try {
            this.stream = await navigator.mediaDevices.getUserMedia({
                video: {
                    facingMode: "environment",
                    advanced: [{ torch: true }]
                }
            });

            this.video = document.getElementById("cameraPreview");
            this.video.srcObject = this.stream;
            await this.video.play();
        } catch (err) {
            console.error("Camera error", err);
            dotNetRef.invokeMethodAsync('OnScannerError', err.message);
        }
    },

    captureBarcode: function () {
        if (!this.video) return;

        const canvas = document.createElement("canvas");
        canvas.width = this.video.videoWidth;
        canvas.height = this.video.videoHeight;
        const ctx = canvas.getContext("2d");
        ctx.drawImage(this.video, 0, 0, canvas.width, canvas.height);

        const dataUrl = canvas.toDataURL("image/png");

        Quagga.decodeSingle({
            src: dataUrl,
            numOfWorkers: 0,
            inputStream: { size: 800 },
            decoder: { readers: ["ean_reader", "code_128_reader"] }
        }, result => {
            this.stream.getTracks().forEach(t => t.stop());
            this.video.srcObject = null;

            if (result && result.codeResult) {
                this.dotNetRef.invokeMethodAsync('OnBarcodeDetected', result.codeResult.code);
            } else {
                this.dotNetRef.invokeMethodAsync('OnScannerError', 'No barcode detected.');
            }
        });
    },

    stop: function () {
        if (this.stream) {
            this.stream.getTracks().forEach(track => track.stop());
            this.stream = null;
        }
        if (this.video) {
            this.video.srcObject = null;
        }
    }
};