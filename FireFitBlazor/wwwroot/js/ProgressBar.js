console.log("ProgressBar.js loaded");

window.initProgressBar = (element, dotnetHelper, targetPercent) => {
    const animateProgress = (target) => {
        let current = 0;
        const interval = setInterval(() => {
            if (current >= target) {
                clearInterval(interval);
                return;
            }
            current++;
            dotnetHelper.invokeMethodAsync('UpdateProgress', current);
        }, 20);
    };

    // Start animation with the passed targetPercent
    animateProgress(targetPercent);
};