// wwwroot/js/camera.js
window.cameraInterop = {
    openCamera: async () => {
        const input = document.createElement("input");
        input.type = "file";
        input.accept = "image/*";
        input.capture = "environment";
        input.click();

        return new Promise((resolve) => {
            input.onchange = () => {
                const file = input.files[0];
                const reader = new FileReader();
                reader.onload = (e) => resolve(e.target.result);
                reader.readAsDataURL(file);
            };
        });
    }
};
