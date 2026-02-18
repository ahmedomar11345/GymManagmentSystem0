// Plan toggle confirmation using event delegation
document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        if (e.target.closest('.toggle-plan-btn')) {
            const btn = e.target.closest('.toggle-plan-btn');

            // Get data from dataset
            const name = btn.dataset.name || "this plan";
            const isPlanActive = (btn.dataset.active === 'true');
            const activating = !isPlanActive;

            // Define action function
            const performAction = function () {
                const form = btn.closest('form');
                if (form) {
                    btn.classList.add('btn-loading');
                    form.submit();
                } else {
                    console.error("Critical: Form not found for plan toggle button");
                    alert("System Error: Could not find the action form.");
                }
            };

            // Try SweetAlert first
            if (window.Swal) {
                Swal.fire({
                    title: activating ? 'Activate Plan?' : 'Deactivate Plan?',
                    text: activating
                        ? 'Do you want to make ' + name + ' available for new members?'
                        : 'Deactivating ' + name + ' will prevent new signups for this plan.',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: activating ? 'Yes, Activate' : 'Yes, Deactivate',
                    cancelButtonText: 'Cancel',
                    buttonsStyling: false,
                    customClass: {
                        popup: 'premium-swal-popup',
                        title: 'premium-swal-title',
                        confirmButton: activating ? 'premium-swal-confirm btn btn-primary px-4' : 'premium-swal-confirm btn btn-danger px-4',
                        cancelButton: 'premium-swal-cancel btn btn-outline-secondary px-4'
                    },
                    showClass: {
                        popup: 'animate__animated animate__zoomIn animate__faster'
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        performAction();
                    }
                });
            } else {
                // Fallback to native confirm
                if (confirm(activating ? 'Activate ' + name + '?' : 'Deactivate ' + name + '?')) {
                    performAction();
                }
            }
        }
    });
});
