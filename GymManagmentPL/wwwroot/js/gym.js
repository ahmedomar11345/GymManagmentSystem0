// Core Gym Management Logic
document.addEventListener('DOMContentLoaded', function () {
    // 1. Toast/Alert Initialization from Data Attributes
    const notificationEl = document.getElementById('global-notifications');

    // Notification Sound Function
    const playNotificationSound = (type) => {
        const sounds = {
            success: 'https://assets.mixkit.co/active_storage/sfx/2869/2869-preview.mp3',
            error: 'https://assets.mixkit.co/active_storage/sfx/2180/2180-preview.mp3',
            warning: 'https://assets.mixkit.co/active_storage/sfx/2358/2358-preview.mp3'
        };
        const audio = new Audio(sounds[type]);
        audio.play().catch(e => console.log('Autoplay blocked or sound failed'));
    };

    if (notificationEl && window.Swal) {
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 4000,
            timerProgressBar: true,
            showClass: {
                popup: 'animate__animated animate__fadeInRight'
            },
            hideClass: {
                popup: 'animate__animated animate__fadeOutRight'
            },
            customClass: {
                popup: 'premium-swal-popup shadow-lg border-0'
            },
            didOpen: (toast) => {
                toast.addEventListener('mouseenter', Swal.stopTimer)
                toast.addEventListener('mouseleave', Swal.resumeTimer)
            }
        });

        const success = notificationEl.dataset.success;
        const error = notificationEl.dataset.error;
        const warning = notificationEl.dataset.warning;
        const failTitle = notificationEl.dataset.failTitle || 'Operation Failed';
        const confirmText = notificationEl.dataset.confirmText || 'Ok';

        if (success && success.trim().length > 0) {
            playNotificationSound('success');
            Toast.fire({
                icon: 'success',
                title: success
            });
        }

        if (error && error.trim().length > 0) {
            playNotificationSound('error');
            Swal.fire({
                icon: 'error',
                title: failTitle,
                text: error,
                confirmButtonText: confirmText,
                buttonsStyling: false,
                customClass: {
                    popup: 'premium-swal-popup animate__animated animate__headShake',
                    title: 'premium-swal-title',
                    confirmButton: 'premium-swal-confirm btn btn-danger',
                    htmlContainer: 'swal2-html-container'
                }
            });
        }

        if (warning && warning.trim().length > 0) {
            playNotificationSound('warning');
            Toast.fire({
                icon: 'warning',
                title: warning,
                customClass: {
                    popup: 'premium-swal-popup'
                }
            });
        }
    }
});

/**
 * Global Generic Confirmation function
 */
function confirmAction(id, title, text, icon, confirmText, formId, inputId) {
    if (!window.Swal) return;

    Swal.fire({
        title: title,
        text: text,
        icon: icon || 'question',
        showCancelButton: true,
        confirmButtonText: confirmText || 'Confirm',
        cancelButtonText: 'Cancel',
        buttonsStyling: false,
        customClass: {
            popup: 'premium-swal-popup',
            title: 'premium-swal-title',
            confirmButton: 'premium-swal-confirm btn btn-primary',
            cancelButton: 'premium-swal-cancel btn btn-outline-secondary'
        },
        showClass: {
            popup: 'animate__animated animate__zoomIn animate__faster'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const form = document.getElementById(formId);
            const input = document.getElementById(inputId);

            if (input) input.value = id;
            if (form) form.submit();
        }
    });
}
/**
 * Premium Form Confirmation using SweetAlert2
 * @param {HTMLFormElement} form - The form to submit
 * @param {string} title - Dialog title
 * @param {string} text - Dialog text
 * @param {string} icon - icon type (warning, info, etc.)
 */
function confirmSubmit(form, title, text, icon = 'warning') {
    if (!window.Swal) {
        if (confirm(text)) form.submit();
        return;
    }

    Swal.fire({
        title: title,
        text: text,
        icon: icon,
        showCancelButton: true,
        confirmButtonText: 'Yes, Proceed',
        cancelButtonText: 'Cancel',
        buttonsStyling: false,
        customClass: {
            popup: 'premium-swal-popup',
            title: 'premium-swal-title',
            confirmButton: 'premium-swal-confirm btn btn-primary px-4',
            cancelButton: 'premium-swal-cancel btn btn-outline-secondary px-4'
        },
        showClass: {
            popup: 'animate__animated animate__zoomIn animate__faster'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            form.submit();
        }
    });
}
