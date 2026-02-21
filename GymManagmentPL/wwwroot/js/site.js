// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Global Site Scripts
document.addEventListener('DOMContentLoaded', function () {
    // 1. Generic Button/Page Reload handler
    document.addEventListener('click', function (e) {
        if (e.target.closest('.reload-page-btn')) {
            window.location.reload();
        }
    });

    // 2. Initialize Bootstrap Tooltips (if jQuery/Bootstrap loaded)
    if (window.jQuery && window.bootstrap) {
        $('[data-bs-toggle="tooltip"]').tooltip();
    }

    // 3. Generic Form Submission Loading State
    const loadingForms = document.querySelectorAll('form.needs-loading');
    loadingForms.forEach(form => {
        form.addEventListener('submit', function (e) {
            // Check validation if using jquery validation
            if (window.jQuery && $(this).valid && !$(this).valid()) {
                return;
            }

            const submitBtn = form.querySelector('[type="submit"]');
            if (submitBtn) {
                const text = submitBtn.querySelector('.btn-text');
                const spinner = submitBtn.querySelector('.btn-spinner');

                if (text && spinner) {
                    submitBtn.classList.add('disabled');
                    text.classList.add('d-none');
                    spinner.classList.remove('d-none');
                }
            }
        });
    });

    // 4. Real-time Global Search Filtering
    const globalSearchInput = document.getElementById('globalSearchInput');
    if (globalSearchInput) {
        globalSearchInput.addEventListener('input', function (e) {
            const searchTerm = e.target.value.toLowerCase().trim();

            // Search in tables
            const tables = document.querySelectorAll('table');
            tables.forEach(table => {
                const tbody = table.querySelector('tbody');
                if (!tbody) return;

                const rows = tbody.querySelectorAll('tr:not(.no-results-row)');
                let visibleRows = 0;

                rows.forEach(row => {
                    const text = row.textContent.toLowerCase();
                    if (text.includes(searchTerm)) {
                        row.style.setProperty('display', '', 'important');
                        visibleRows++;
                    } else {
                        row.style.setProperty('display', 'none', 'important');
                    }
                });

                // Handle "No results" for table
                let noResultsRow = tbody.querySelector('.no-results-row');
                if (visibleRows === 0 && searchTerm !== "") {
                    if (!noResultsRow) {
                        const config = document.getElementById('global-notifications')?.dataset;
                        const noResultsText = config?.noResults || 'No matching records found';
                        const colCount = table.querySelectorAll('thead th').length || 5;
                        noResultsRow = document.createElement('tr');
                        noResultsRow.className = 'no-results-row';
                        noResultsRow.innerHTML = `<td colspan="${colCount}" class="text-center py-5 text-muted">${noResultsText}</td>`;
                        tbody.appendChild(noResultsRow);
                    }
                } else if (noResultsRow) {
                    noResultsRow.remove();
                }
            });

            // Search in cards (if they have .searchable-card class)
            const cards = document.querySelectorAll('.searchable-card');
            cards.forEach(card => {
                const text = card.textContent.toLowerCase();
                const container = card.closest('[class*="col-"]') || card.parentElement;
                if (text.includes(searchTerm)) {
                    container.style.setProperty('display', '', 'important');
                } else {
                    container.style.setProperty('display', 'none', 'important');
                }
            });

            // If on Dashboard, hide sections that have no visible cards
            if (searchTerm !== "") {
                const dashboardSections = document.querySelectorAll('.animate-fade-in > .row, .animate-fade-in > .welcome-banner');
                dashboardSections.forEach(section => {
                    const cards = section.querySelectorAll('.searchable-card');
                    if (cards.length > 0) {
                        const hasVisible = Array.from(cards).some(c => (c.closest('[class*="col-"]') || c.parentElement).style.display !== 'none');
                        section.style.display = hasVisible ? '' : 'none';
                    }
                });
            } else {
                document.querySelectorAll('.animate-fade-in > .row, .animate-fade-in > .welcome-banner').forEach(s => s.style.display = '');
            }
        });
    }

    // 5. Global Notifications (SweetAlert2)
    const notificationEl = document.getElementById('global-notifications');
    if (notificationEl && window.Swal) {
        const success = notificationEl.dataset.success;
        const error = notificationEl.dataset.error;
        const warning = notificationEl.dataset.warning;

        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            didOpen: (toast) => {
                toast.addEventListener('mouseenter', Swal.stopTimer)
                toast.addEventListener('mouseleave', Swal.resumeTimer)
            }
        });

        if (success) {
            Toast.fire({ icon: 'success', title: success });
        } else if (error) {
            Toast.fire({ icon: 'error', title: error });
        } else if (warning) {
            Toast.fire({ icon: 'warning', title: warning });
        }
    }

    // 6. Bind Mark All Read Button
    const btnMarkAllRead = document.getElementById('btnMarkAllRead');
    if (btnMarkAllRead) {
        btnMarkAllRead.addEventListener('click', markAllAsRead);
    }
});

// 6. Real-time Notifications Handling
function loadNotifications() {
    const list = document.getElementById('notificationList');

    fetch('/Notification/GetRecent')
        .then(response => {
            if (!response.ok) throw new Error("Not logged in or error");
            return response.json();
        })
        .then(data => {
            const badge = document.getElementById('notificationBadge');

            // Update badge
            if (badge) {
                if (data.unreadCount > 0) {
                    badge.innerText = data.unreadCount > 9 ? '9+' : data.unreadCount;
                    badge.classList.remove('d-none');
                } else {
                    badge.classList.add('d-none');
                }
            }

            // Populate list if it exists
            if (list) {
                if (data.notifications.length === 0) {
                    const emptyText = list.getAttribute('data-empty-text') || 'All caught up!';
                    list.innerHTML = `
                        <div class="text-center py-4 text-muted small">
                            <i class="fas fa-check-circle d-block fs-3 mb-2 opacity-25"></i>
                            ${emptyText}
                        </div>`;
                } else {
                    let html = '';
                    data.notifications.forEach(n => {
                        html += createNotificationItem(n);
                    });
                    list.innerHTML = html;
                }
            }
        })
        .catch(err => {
            // silent fail likely due to not logged in
        });
}

function createNotificationItem(n) {
    const icon = n.type === 'success' ? 'check-circle' :
        n.type === 'danger' ? 'exclamation-triangle' :
            n.type === 'warning' ? 'bell' : 'info-circle';

    const bgClass = n.type === 'success' ? 'success' :
        n.type === 'danger' ? 'danger' :
            n.type === 'warning' ? 'warning' : 'primary';

    const date = new Date(n.createdAt);
    const dateStr = date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    const url = n.url || '#';

    return `
        <div class="dropdown-item p-0 border-bottom d-flex align-items-stretch notification-item-wrapper hover-bg-light">
            <a class="flex-grow-1 p-3 d-flex align-items-start gap-3 text-decoration-none" 
               href="${url}" onclick="markAsRead(${n.id}, '${url}'); return false;">
                <div class="bg-${bgClass} bg-opacity-10 text-${bgClass} rounded-circle d-flex align-items-center justify-content-center flex-shrink-0" style="width: 35px; height: 35px;">
                    <i class="fas fa-${icon} fa-sm"></i>
                </div>
                <div class="flex-grow-1 overflow-hidden">
                    <div class="d-flex justify-content-between align-items-start">
                        <div class="small fw-bold mb-1 text-dark text-truncate" style="max-width: 150px;">${n.title}</div>
                    </div>
                    <div class="x-small text-muted text-wrap mb-1" style="line-height: 1.3;">${n.message}</div>
                    <div class="x-small text-secondary opacity-75"><i class="far fa-clock me-1"></i>${dateStr}</div>
                </div>
            </a>
            <button class="btn btn-link text-danger border-0 px-3 d-flex align-items-center justify-content-center" 
                    title="${document.getElementById('global-notifications')?.dataset?.delete || 'Delete'}" onclick="deleteNotificationOnly(${n.id}, event)">
                <i class="fas fa-trash-alt fa-xs"></i>
            </button>
        </div>
    `;
}

async function markAsRead(id, url) {
    if (id) {
        const success = await deleteNotificationOnly(id, null, true);
        if (success && url && url !== '#' && url !== 'null') {
            window.location.href = url;
        }
    } else if (url && url !== '#') {
        window.location.href = url;
    }
}

async function deleteNotificationOnly(id, event, silent = false) {
    if (event) {
        event.stopPropagation();
        event.preventDefault();
    }

    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const response = await fetch(`/Notification/MarkAsRead/${id}`, {
            method: 'POST',
            headers: {
                'X-XSRF-TOKEN': token,
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (response.ok) {
            loadNotifications();
            return true;
        } else {
            if (!silent) {
                const config = document.getElementById('global-notifications')?.dataset;
                alert(config?.failedNotification || 'Failed to delete notification. Please refresh.');
            }
        }
    } catch (error) {
        console.error('Error deleting notification:', error);
        return false;
    }
}

async function markAllAsRead(event) {
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }

    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const response = await fetch('/Notification/MarkAllAsRead', {
            method: 'POST',
            headers: {
                'X-XSRF-TOKEN': token,
                'X-Requested-With': 'XMLHttpRequest'
            }
        });
        if (response.ok) {
            loadNotifications();
        } else {
            console.error('Failed to clear notifications', response.status);
        }
    } catch (err) {
        console.error('Error clearing all notifications:', err);
    }
}

// Initial load to set badge
document.addEventListener('DOMContentLoaded', () => {
    if (document.getElementById('notificationBadge')) {
        loadNotifications();
        setInterval(loadNotifications, 60000);
    }
});
