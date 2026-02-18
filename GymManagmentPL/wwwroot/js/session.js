// Member removal from session confirmation using event delegation
document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        if (e.target.closest('.remove-member-btn')) {
            const button = e.target.closest('.remove-member-btn');
            const memberId = button.dataset.memberId;
            const memberName = button.dataset.memberName;

            Swal.fire({
                title: 'Remove Member?',
                text: 'Are you sure you want to remove ' + memberName + ' from this session?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#7f8c8d',
                confirmButtonText: 'Yes, remove them!',
                cancelButtonText: 'Cancel',
                customClass: {
                    popup: 'premium-swal-popup',
                    title: 'premium-swal-title',
                    confirmButton: 'premium-swal-confirm',
                    cancelButton: 'premium-swal-cancel'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    const removeMemberId = document.getElementById('removeMemberId');
                    const removeForm = document.getElementById('removeForm');
                    if (removeMemberId) removeMemberId.value = memberId;
                    if (removeForm) removeForm.submit();
                }
            });
        }
    });
});
