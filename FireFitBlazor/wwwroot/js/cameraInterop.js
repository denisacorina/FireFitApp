window.cameraInterop = {
    openCamera: function () {
        const input = document.createElement("input");
        input.type = "file";
        input.accept = "image/*";
        input.capture = "environment";
        input.style.display = "none";

        document.body.appendChild(input);

        input.onchange = () => {
            const file = input.files[0];
            if (!file) return;

            const reader = new FileReader();
            reader.onload = () => {
                // Send the base64 image back to C#
                DotNet.invokeMethodAsync('FireFitBlazor', 'ReceiveCameraImage', reader.result);
                input.remove();
            };
            reader.readAsDataURL(file);
        };

        input.click(); // ✅ MUST be called from a direct user gesture context!
    }
};