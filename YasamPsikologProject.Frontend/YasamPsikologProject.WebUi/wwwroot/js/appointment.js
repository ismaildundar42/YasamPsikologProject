// ============================================
// MHRS Tarzı Randevu Sistemi - JavaScript
// ============================================

const AppointmentSystem = {
    // API Base URL
    apiBaseUrl: '/api',

    // Seçili değerler
    selectedPsychologist: null,
    selectedDate: null,
    selectedTime: null,
    selectedDuration: 50,

    // ============================================
    // Psikolog Listesi
    // ============================================

    async loadPsychologists(filters = {}) {
        try {
            this.showLoading();

            const response = await fetch(`${this.apiBaseUrl}/psychologists`);
            const data = await response.json();

            if (data.success && data.data) {
                let psychologists = data.data;

                // Filtreleme
                if (filters.search) {
                    const searchLower = filters.search.toLowerCase();
                    psychologists = psychologists.filter(p =>
                        p.user?.firstName.toLowerCase().includes(searchLower) ||
                        p.user?.lastName.toLowerCase().includes(searchLower) ||
                        p.specialization.toLowerCase().includes(searchLower)
                    );
                }

                if (filters.specialization) {
                    psychologists = psychologists.filter(p =>
                        p.specialization === filters.specialization
                    );
                }

                if (filters.onlineOnly) {
                    psychologists = psychologists.filter(p => p.isOnlineConsultationAvailable);
                }

                this.renderPsychologists(psychologists);
            }

            this.hideLoading();
        } catch (error) {
            console.error('Psikolog yükleme hatası:', error);
            this.showToast('Psikologlar yüklenirken bir hata oluştu', 'error');
            this.hideLoading();
        }
    },

    renderPsychologists(psychologists) {
        const container = document.getElementById('psychologistGrid');
        if (!container) return;

        container.innerHTML = psychologists.map(p => `
            <div class="psychologist-card fade-in" onclick="AppointmentSystem.selectPsychologist(${p.id})">
                <div class="psychologist-header">
                    <div class="psychologist-avatar" style="background: ${p.calendarColor || '#4CAF50'}">
                        ${this.getInitials(p.user?.firstName, p.user?.lastName)}
                    </div>
                    <h3 class="psychologist-name">${p.user?.firstName} ${p.user?.lastName}</h3>
                </div>
                <div class="psychologist-body">
                    <div class="psychologist-info">
                        <i class="fas fa-briefcase"></i>
                        <span>${p.experienceYears} yıl deneyim</span>
                    </div>
                    <div class="psychologist-info">
                        <i class="fas fa-graduation-cap"></i>
                        <span>${p.education || 'Bilgi yok'}</span>
                    </div>
                    <div class="psychologist-info">
                        <i class="fas fa-video"></i>
                        <span>${p.isOnlineConsultationAvailable ? 'Online' : ''} ${p.isInPersonConsultationAvailable ? 'Yüz Yüze' : ''}</span>
                    </div>
                    <button class="btn-appointment-primary w-100">Randevu Al</button>
                </div>
            </div>
        `).join('');
    },

    getInitials(firstName, lastName) {
        return `${firstName?.[0] || ''}${lastName?.[0] || ''}`.toUpperCase();
    },

    // ============================================
    // Tarih ve Saat Seçimi
    // ============================================

    async loadAvailableSlots(psychologistId, date, duration) {
        try {
            this.showLoading();

            const dateStr = this.formatDate(date);
            const response = await fetch(
                `${this.apiBaseUrl}/appointments/available-slots?psychologistId=${psychologistId}&date=${dateStr}&duration=${duration}`
            );
            const data = await response.json();

            if (data.success && data.data) {
                this.renderTimeSlots(data.data);
            } else {
                this.renderTimeSlots([]);
                this.showToast('Bu tarih için uygun saat bulunamadı', 'warning');
            }

            this.hideLoading();
        } catch (error) {
            console.error('Uygun saatler yüklenirken hata:', error);
            this.showToast('Uygun saatler yüklenirken bir hata oluştu', 'error');
            this.hideLoading();
        }
    },

    renderTimeSlots(slots) {
        const container = document.getElementById('timeSlotsGrid');
        if (!container) return;

        if (slots.length === 0) {
            container.innerHTML = '<p class="text-muted text-center">Bu tarih için uygun saat bulunmamaktadır.</p>';
            return;
        }

        container.innerHTML = slots.map(slot => {
            const time = new Date(slot);
            const timeStr = this.formatTime(time);

            return `
                <div class="time-slot fade-in" onclick="AppointmentSystem.selectTime('${slot}')">
                    ${timeStr}
                </div>
            `;
        }).join('');
    },

    selectDate(date) {
        this.selectedDate = date;

        // Tüm tarih hücrelerinden selected class'ını kaldır
        document.querySelectorAll('.date-cell').forEach(cell => {
            cell.classList.remove('selected');
        });

        // Seçili tarihe selected class'ı ekle
        event.target.closest('.date-cell')?.classList.add('selected');

        // Uygun saatleri yükle
        if (this.selectedPsychologist) {
            this.loadAvailableSlots(this.selectedPsychologist, date, this.selectedDuration);
        }
    },

    selectTime(time) {
        this.selectedTime = time;

        // Tüm saat slotlarından selected class'ını kaldır
        document.querySelectorAll('.time-slot').forEach(slot => {
            slot.classList.remove('selected');
        });

        // Seçili saate selected class'ı ekle
        event.target.classList.add('selected');

        // Devam butonunu aktif et
        const continueBtn = document.getElementById('continueBtn');
        if (continueBtn) {
            continueBtn.disabled = false;
        }
    },

    selectPsychologist(id) {
        this.selectedPsychologist = id;
        window.location.href = `/PublicAppointment/SelectDateTime?psychologistId=${id}`;
    },

    // ============================================
    // Randevu Oluşturma
    // ============================================

    async createAppointment(appointmentData) {
        try {
            this.showLoading();

            const response = await fetch(`${this.apiBaseUrl}/appointments`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(appointmentData)
            });

            const data = await response.json();

            if (data.success) {
                this.showToast('Randevunuz başarıyla oluşturuldu!', 'success');
                setTimeout(() => {
                    window.location.href = '/PublicAppointment/MyAppointments';
                }, 1500);
            } else {
                this.showToast(data.message || 'Randevu oluşturulamadı', 'error');
            }

            this.hideLoading();
        } catch (error) {
            console.error('Randevu oluşturma hatası:', error);
            this.showToast('Randevu oluşturulurken bir hata oluştu', 'error');
            this.hideLoading();
        }
    },

    // ============================================
    // Randevu İşlemleri (Onaylama, İptal vb.)
    // ============================================

    async approveAppointment(appointmentId) {
        if (!confirm('Bu randevuyu onaylamak istediğinizden emin misiniz?')) {
            return;
        }

        try {
            this.showLoading();

            const response = await fetch(`${this.apiBaseUrl}/appointments/${appointmentId}/approve`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                }
            });

            const data = await response.json();

            if (data.success) {
                this.showToast('Randevu onaylandı', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                this.showToast(data.message || 'Randevu onaylanamadı', 'error');
            }

            this.hideLoading();
        } catch (error) {
            console.error('Randevu onaylama hatası:', error);
            this.showToast('Randevu onaylanırken bir hata oluştu', 'error');
            this.hideLoading();
        }
    },

    async cancelAppointment(appointmentId) {
        const reason = prompt('İptal sebebini giriniz:');
        if (!reason) return;

        try {
            this.showLoading();

            const response = await fetch(`${this.apiBaseUrl}/appointments/${appointmentId}/cancel`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ reason })
            });

            const data = await response.json();

            if (data.success) {
                this.showToast('Randevu iptal edildi', 'success');
                setTimeout(() => location.reload(), 1000);
            } else {
                this.showToast(data.message || 'Randevu iptal edilemedi', 'error');
            }

            this.hideLoading();
        } catch (error) {
            console.error('Randevu iptal hatası:', error);
            this.showToast('Randevu iptal edilirken bir hata oluştu', 'error');
            this.hideLoading();
        }
    },

    // ============================================
    // Yardımcı Fonksiyonlar
    // ============================================

    formatDate(date) {
        const d = new Date(date);
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    },

    formatTime(date) {
        const d = new Date(date);
        const hours = String(d.getHours()).padStart(2, '0');
        const minutes = String(d.getMinutes()).padStart(2, '0');
        return `${hours}:${minutes}`;
    },

    formatDateTime(date) {
        const d = new Date(date);
        return `${this.formatDate(d)} ${this.formatTime(d)}`;
    },

    // ============================================
    // UI Yardımcıları
    // ============================================

    showLoading() {
        let overlay = document.getElementById('loadingOverlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'loadingOverlay';
            overlay.className = 'loading-overlay';
            overlay.innerHTML = '<div class="loading-spinner"></div>';
            document.body.appendChild(overlay);
        }
        overlay.style.display = 'flex';
    },

    hideLoading() {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.display = 'none';
        }
    },

    showToast(message, type = 'info') {
        let container = document.getElementById('toastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toastContainer';
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        const toast = document.createElement('div');
        toast.className = `toast ${type}`;

        const icon = {
            success: 'fa-check-circle',
            error: 'fa-times-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        }[type] || 'fa-info-circle';

        toast.innerHTML = `
            <i class="fas ${icon}"></i>
            <span>${message}</span>
        `;

        container.appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '0';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    },

    // ============================================
    // Form Validasyonu
    // ============================================

    validateAppointmentForm(formData) {
        const errors = [];

        if (!formData.psychologistId) {
            errors.push('Psikolog seçimi zorunludur');
        }

        if (!formData.appointmentDate) {
            errors.push('Randevu tarihi seçimi zorunludur');
        }

        if (!formData.duration || formData.duration < 15) {
            errors.push('Geçerli bir süre seçiniz');
        }

        if (formData.clientNotes && formData.clientNotes.length > 1000) {
            errors.push('Notlar en fazla 1000 karakter olabilir');
        }

        return errors;
    },

    // ============================================
    // Takvim Oluşturma
    // ============================================

    generateCalendar(year, month) {
        const firstDay = new Date(year, month, 1);
        const lastDay = new Date(year, month + 1, 0);
        const daysInMonth = lastDay.getDate();
        const startingDayOfWeek = firstDay.getDay();

        const calendar = [];
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        // Önceki ayın günleri
        for (let i = 0; i < startingDayOfWeek; i++) {
            calendar.push({ day: '', disabled: true });
        }

        // Bu ayın günleri
        for (let day = 1; day <= daysInMonth; day++) {
            const date = new Date(year, month, day);
            const isPast = date < today;

            calendar.push({
                day: day,
                date: date,
                disabled: isPast,
                isToday: date.getTime() === today.getTime()
            });
        }

        return calendar;
    },

    renderCalendar(year, month) {
        const container = document.getElementById('dateGrid');
        if (!container) return;

        const calendar = this.generateCalendar(year, month);
        const weekDays = ['Paz', 'Pzt', 'Sal', 'Çar', 'Per', 'Cum', 'Cmt'];

        let html = weekDays.map(day => `
            <div class="date-cell disabled">
                <div class="date-day">${day}</div>
            </div>
        `).join('');

        html += calendar.map(item => {
            if (!item.day) {
                return '<div class="date-cell disabled"></div>';
            }

            const dateStr = this.formatDate(item.date);
            const classes = ['date-cell'];
            if (item.disabled) classes.push('disabled');
            if (item.isToday) classes.push('today');

            return `
                <div class="${classes.join(' ')}" ${!item.disabled ? `onclick="AppointmentSystem.selectDate('${dateStr}')"` : ''}>
                    <div class="date-number">${item.day}</div>
                </div>
            `;
        }).join('');

        container.innerHTML = html;
    }
};

// ============================================
// Sayfa Yüklendiğinde
// ============================================

document.addEventListener('DOMContentLoaded', function () {
    // Filtreleme
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const filters = {
                search: this.value,
                specialization: document.getElementById('specializationFilter')?.value,
                onlineOnly: document.getElementById('onlineOnlyFilter')?.checked
            };
            AppointmentSystem.loadPsychologists(filters);
        });
    }

    // Takvim oluştur
    const dateGrid = document.getElementById('dateGrid');
    if (dateGrid) {
        const today = new Date();
        AppointmentSystem.renderCalendar(today.getFullYear(), today.getMonth());
    }
});
