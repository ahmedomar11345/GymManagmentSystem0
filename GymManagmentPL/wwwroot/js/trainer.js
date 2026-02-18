// Trainer delete confirmation using event delegation
document.addEventListener('DOMContentLoaded', function () {
    // Handle delete button clicks using event delegation
    document.addEventListener('click', function (e) {
        if (e.target.closest('.delete-trainer-btn')) {
            const button = e.target.closest('.delete-trainer-btn');
            const trainerId = button.dataset.trainerId;
            const trainerName = button.dataset.trainerName;

            Swal.fire({
                title: 'Confirm Removal',
                text: `Are you sure you want to remove "${trainerName}" from the coaching staff?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, remove them',
                cancelButtonText: 'Cancel',
                buttonsStyling: false,
                customClass: {
                    popup: 'premium-swal-popup',
                    title: 'premium-swal-title',
                    confirmButton: 'premium-swal-confirm btn btn-danger px-4',
                    cancelButton: 'premium-swal-cancel btn btn-outline-secondary px-4'
                },
                showClass: {
                    popup: 'animate__animated animate__zoomIn animate__faster'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    document.getElementById('deleteTrainerId').value = trainerId;
                    document.getElementById('deleteForm').submit();
                }
            });
        }
    });
});
