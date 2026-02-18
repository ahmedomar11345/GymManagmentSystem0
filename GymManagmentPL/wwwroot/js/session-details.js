/* Session Details specific logic */
document.addEventListener('DOMContentLoaded', function () {
    const timerContainer = document.getElementById('sessionTimer');
    if (!timerContainer) return;

    const startDateStr = timerContainer.dataset.start;
    const endDateStr = timerContainer.dataset.end;

    if (!startDateStr || !endDateStr) return;

    const startDate = new Date(startDateStr);
    const endDate = new Date(endDateStr);
    const totalDuration = endDate - startDate;

    function updateRealTimeDuration() {
        const now = new Date();
        const elapsed = now - startDate;
        const remaining = endDate - now;

        // Calculate elapsed time
        const elapsedHours = Math.floor(elapsed / (1000 * 60 * 60));
        const elapsedMinutes = Math.floor((elapsed % (1000 * 60 * 60)) / (1000 * 60));
        const elapsedSeconds = Math.floor((elapsed % (1000 * 60)) / 1000);

        // Calculate remaining time
        const remainingHours = Math.floor(remaining / (1000 * 60 * 60));
        const remainingMinutes = Math.floor((remaining % (1000 * 60 * 60)) / (1000 * 60));
        const remainingSeconds = Math.floor((remaining % (1000 * 60)) / 1000);

        // Update DOM elements if they exist
        const h = document.getElementById('elapsedHours');
        const m = document.getElementById('elapsedMinutes');
        const s = document.getElementById('elapsedSeconds');
        const pb = document.getElementById('progressBar');
        const rt = document.getElementById('remainingTime');

        if (h) h.textContent = String(elapsedHours).padStart(2, '0');
        if (m) m.textContent = String(elapsedMinutes).padStart(2, '0');
        if (s) s.textContent = String(elapsedSeconds).padStart(2, '0');

        // Update progress bar
        if (pb) {
            const progress = Math.min(100, Math.max(0, (elapsed / totalDuration) * 100));
            pb.style.width = progress + '%';

            if (remaining <= 0) {
                pb.classList.remove('bg-success');
                pb.classList.add('bg-danger');
            }
        }

        // Update remaining time text
        if (rt) {
            if (remaining > 0) {
                rt.textContent = `${remainingHours}h ${remainingMinutes}m ${remainingSeconds}s remaining`;
            } else {
                rt.textContent = 'Session completed!';
            }
        }
    }

    // Update every second
    updateRealTimeDuration();
    setInterval(updateRealTimeDuration, 1000);
});
