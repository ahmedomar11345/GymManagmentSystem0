// Membership management scripts
document.addEventListener('DOMContentLoaded', function () {
    // Single delegated click listener for all membership actions
    document.addEventListener('click', function (e) {
        // 1. Cancellation handler
        const cancelBtn = e.target.closest('.cancel-membership-btn');
        if (cancelBtn) {
            e.preventDefault();
            const memberId = cancelBtn.dataset.memberId;
            const planId = cancelBtn.dataset.planId;
            const memberName = cancelBtn.dataset.memberName;

            Swal.fire({
                title: 'Cancel Membership?',
                text: `Are you sure you want to cancel the subscription for "${memberName}"? This cannot be undone.`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#EF4444',
                cancelButtonColor: '#64748B',
                confirmButtonText: 'Yes, Cancel Now',
                cancelButtonText: 'Keep Active',
                buttonsStyling: false,
                customClass: {
                    popup: 'premium-swal-popup shadow-lg border-0',
                    title: 'fw-bold',
                    confirmButton: 'btn btn-danger px-4 py-2 fw-bold me-2',
                    cancelButton: 'btn btn-outline-secondary px-4 py-2 fw-bold'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    const memberInput = document.getElementById('cancelMemberId');
                    const planInput = document.getElementById('cancelPlanId');
                    const form = document.getElementById('cancelForm');

                    if (memberInput) memberInput.value = memberId;
                    if (planInput) planInput.value = planId;
                    if (form) form.submit();
                }
            });
            return;
        }

        // 2. Freeze handler
        const freezeBtn = e.target.closest('.freeze-membership-btn');
        if (freezeBtn) {
            e.preventDefault();
            const memberId = freezeBtn.dataset.memberId;
            const planId = freezeBtn.dataset.planId;
            const memberName = freezeBtn.dataset.memberName;

            const mId = document.getElementById('freezeMemberId');
            const pId = document.getElementById('freezePlanId');
            const tName = document.getElementById('freezeTargetName');
            const modalEl = document.getElementById('freezeModal');

            if (mId) mId.value = memberId;
            if (pId) pId.value = planId;
            if (tName) tName.innerText = memberName;

            if (modalEl && typeof bootstrap !== 'undefined') {
                // Move modal to body to avoid stacking context issues (backdrop blocking clicks)
                if (modalEl.parentElement !== document.body) {
                    document.body.appendChild(modalEl);
                }

                // Get or create modal instance
                let freezeModal = bootstrap.Modal.getInstance(modalEl);
                if (!freezeModal) {
                    freezeModal = new bootstrap.Modal(modalEl, {
                        backdrop: 'static',
                        keyboard: true
                    });
                }
                freezeModal.show();
            } else {
                console.error('Bootstrap or Modal element not found');
            }
            return;
        }

        // 3. Unfreeze handler
        const unfreezeBtn = e.target.closest('.unfreeze-membership-btn');
        if (unfreezeBtn) {
            e.preventDefault();
            const memberId = unfreezeBtn.dataset.memberId;
            const planId = unfreezeBtn.dataset.planId;
            const memberName = unfreezeBtn.dataset.memberName;

            Swal.fire({
                title: 'Unfreeze Membership?',
                text: `Do you want to restore "${memberName}"'s membership now?`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Yes, Restore Access',
                cancelButtonText: 'Keep Frozen',
                buttonsStyling: false,
                customClass: {
                    popup: 'premium-swal-popup shadow-lg border-0',
                    title: 'fw-bold',
                    confirmButton: 'btn btn-success px-4 py-2 fw-bold me-2',
                    cancelButton: 'btn btn-outline-secondary px-4 py-2 fw-bold'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    const uMId = document.getElementById('unfreezeMemberId');
                    const uPId = document.getElementById('unfreezePlanId');
                    const uForm = document.getElementById('unfreezeForm');

                    if (uMId) uMId.value = memberId;
                    if (uPId) uPId.value = planId;
                    if (uForm) uForm.submit();
                }
            });
            return;
        }
    });

    // Form logic for New Membership (if on Create page)
    const planSelect = document.getElementById('PlanId');
    const memberSelect = document.getElementById('MemberId');
    if (planSelect && memberSelect) {
        // Additional logic can go here
    }
});
