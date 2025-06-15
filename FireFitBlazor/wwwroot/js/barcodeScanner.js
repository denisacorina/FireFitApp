////window.barcodeScanner = {
////    init: function (dotNetRef) {
////        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
////            console.error('Camera access is not supported in this browser');
////            return;
////        }
////        navigator.mediaDevices.getUserMedia({
////            video: {
////                facingMode: 'environment',
////                width: { ideal: 1280 },
////                height: { ideal: 720 },
////                advanced: [{ torch: true }]
////            }
////        })

////        Quagga.init({
////            inputStream: {
////                name: "Live",
////                type: "LiveStream",
////                target: document.querySelector("#interactive"),
////                constraints: {
////                    facingMode: "environment",
////                    width: { min: 640 },
////                    height: { min: 480 },
////                    aspectRatio: { min: 1, max: 2 }
////                },
////            },
////            decoder: {
////                readers: ["ean_reader", "ean_8_reader", "code_128_reader", "code_39_reader", "upc_reader", "upc_e_reader"],
////                multiple: false
////            },
////            locate: true
////        }, function (err) {
////            if (err) {
////                console.error('Error initializing Quagga:', err);
////                dotNetRef.invokeMethodAsync('OnScannerError', err.message);
////                return;
////            }
////            console.log('Quagga initialized successfully');
////            Quagga.start();
////        });

////        let hasScanned = false;

////        Quagga.onDetected(function (result) {
////            if (hasScanned) return;

////            if (result.codeResult && result.codeResult.code) {
////                hasScanned = true;
////                const barcode = result.codeResult.code;

////                Quagga.stop();
////                console.log('✅ Scanned once:', barcode);

////                dotNetRef.invokeMethodAsync('OnBarcodeDetected', barcode);
////            }
////        });

////        Quagga.onProcessed(function (result) {
////            var drawingCtx = Quagga.canvas.ctx.overlay;
////            var drawingCanvas = Quagga.canvas.dom.overlay;

////            if (result) {
////                if (result.boxes) {
////                    drawingCtx.clearRect(0, 0, parseInt(drawingCanvas.getAttribute("width")), parseInt(drawingCanvas.getAttribute("height")));
////                    result.boxes.filter(function (box) {
////                        return box !== result.box;
////                    }).forEach(function (box) {
////                        Quagga.ImageDebug.drawPath(box, { x: 0, y: 1 }, drawingCtx, { color: "green", lineWidth: 2 });
////                    });
////                }

////                if (result.box) {
////                    Quagga.ImageDebug.drawPath(result.box, { x: 0, y: 1 }, drawingCtx, { color: "blue", lineWidth: 2 });
////                }

////                if (result.codeResult && result.codeResult.code) {
////                    Quagga.ImageDebug.drawPath(result.line, { x: 'x', y: 'y' }, drawingCtx, { color: "red", lineWidth: 3 });
////                }
////            }
////        });
////    },

////    stop: function () {
////        try {
////            Quagga.stop();
////            console.log('Quagga stopped successfully');
////        } catch (err) {
////            console.error('Error stopping Quagga:', err);
////        }
////    }
////};


//window.barcodeScanner = {
//    captureOnce: async function (dotNetRef) {
//        const video = document.createElement('video');
//        video.setAttribute("playsinline", true);
//        document.body.appendChild(video);

//        const stream = await navigator.mediaDevices.getUserMedia({
//            video: {
//                facingMode: "environment",
//                advanced: [{ torch: true }]
//            }
//        });

//        video.srcObject = stream;
//        await video.play();

//        const canvas = document.createElement('canvas');
//        canvas.width = video.videoWidth;
//        canvas.height = video.videoHeight;
//        const ctx = canvas.getContext('2d');

//        setTimeout(async () => {
//            ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
//            const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);

//            Quagga.decodeSingle({
//                src: canvas.toDataURL(),
//                numOfWorkers: 0,
//                inputStream: {
//                    size: 800
//                },
//                decoder: {
//                    readers: ["ean_reader"]
//                }
//            }, function (result) {
//                stream.getTracks().forEach(t => t.stop());
//                document.body.removeChild(video);

//                if (result && result.codeResult) {
//                    dotNetRef.invokeMethodAsync('OnBarcodeDetected', result.codeResult.code);
//                } else {
//                    dotNetRef.invokeMethodAsync('OnScannerError', 'No barcode found');
//                }
//            });
//        }, 1000);
//    }
//};


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