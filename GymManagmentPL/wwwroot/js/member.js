// Member specific scripts
document.addEventListener('DOMContentLoaded', function () {


    // 1. Photo preview logic for Create and Edit
    const photoInput = document.getElementById('PhotoFile') || document.getElementById('photoInput');
    const photoPreview = document.getElementById('photoPreview');

    if (photoInput && photoPreview) {
        photoInput.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (ev) {
                    photoPreview.src = ev.target.result;
                }
                reader.readAsDataURL(file);
            }
        });
    }

    // 2. Member delete confirmation using SweetAlert2
    document.addEventListener('click', function (e) {
        const deleteBtn = e.target.closest('.delete-member-btn');
        if (deleteBtn) {
            const memberId = deleteBtn.dataset.memberId;
            const memberName = deleteBtn.dataset.memberName;

            Swal.fire({
                title: 'Are you sure?',
                text: `You are about to delete member "${memberName}". This action cannot be undone!`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Yes, delete it!',
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
                    const deleteForm = document.getElementById('deleteForm');
                    const deleteInput = document.getElementById('deleteMemberId');

                    if (deleteForm && deleteInput) {
                        deleteInput.value = memberId;
                        deleteForm.submit();
                    }
                }
            });
        }
    });
});
