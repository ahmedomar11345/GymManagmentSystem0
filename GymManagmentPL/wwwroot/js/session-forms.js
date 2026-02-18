// Session form handling (Create and Edit)
document.addEventListener("DOMContentLoaded", function () {
    const startInput = document.querySelector('input[name="StartDate"]');
    const endInput = document.querySelector('input[name="EndDate"]');

    if (!startInput || !endInput) return;

    const now = new Date();
    const localNow = new Date(now.getTime() - now.getTimezoneOffset() * 60000)
        .toISOString()
        .slice(0, 16); // yyyy-MM-ddTHH:mm

    startInput.min = localNow;
    endInput.min = localNow;

    if (!startInput.value || startInput.value.startsWith("0001")) {
        startInput.value = localNow;
    }

    if (!endInput.value || endInput.value.startsWith("0001")) {
        const oneHourLater = new Date(now.getTime() + 60 * 60 * 1000);
        const localOneHourLater = new Date(oneHourLater.getTime() - now.getTimezoneOffset() * 60000)
            .toISOString()
            .slice(0, 16);
        endInput.value = localOneHourLater;
    }

    startInput.addEventListener("change", function () {
        endInput.min = startInput.value;
        if (endInput.value && endInput.value < startInput.value) {
            endInput.value = "";
        }
    });

    // Handle cancel button (from CreateSessionViewModel it seems it just goes to Index)
});
