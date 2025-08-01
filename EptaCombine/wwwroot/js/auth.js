// Toast and UI Busy Functions (identical to your example)
function showToast(message, isSuccess = true) {
    const toastBody = toastBox.querySelector('.toast-body');
    toastBody.textContent = message;
    toastBox.className = 'toast align-items-center text-white border-0';
    toastBox.classList.add(isSuccess ? 'bg-success' : 'bg-danger');
    window.bootstrapToast.show();
}

function setUIBusy(message) {
    const overlay = document.getElementById('overlay');
    const video = document.getElementById('overlay-video');

    overlay.classList.remove('d-none');
    document.getElementById('overlayMessage').textContent = message;

    video.currentTime = 0;
    video.play().catch(e => console.log("Video play blocked:", e));
}

function clearUIBusy() {
    const overlay = document.getElementById('overlay');
    overlay.classList.add('d-none');
    document.getElementById('overlay-video').pause();
}

window.addEventListener('load', () => {
    const toastBox = document.getElementById('toastBox');
    if (toastBox) {
        window.bootstrapToast = new bootstrap.Toast(toastBox);
    }
});

document.getElementById('loginForm')?.addEventListener('submit', async function (e) {
    e.preventDefault();

    const formData = {
        Username: this.querySelector('[name="Username"]').value,
        Password: this.querySelector('[name="Password"]').value,
        RememberMe: this.querySelector('[name="RememberMe"]').checked
    }

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        setUIBusy("Входим в систему...");

        const response = await fetch('/Auth?handler=Login', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });

        if (response.redirected) {
            window.location.href = response.url;
            return;
        }

        if (response.status === 401) {
            showToast('Неверный логин или пароль', false);
            return;
        }

        if (!response.ok) {
            showToast('Ошибка входа', false);
            return;
        }

        showToast("Вход выполнен успешно");

    } catch (err) {
        console.error("Login error:", err);
        showToast(err.message, false);
    } finally {
        clearUIBusy();
    }
});

document.getElementById('registerForm')?.addEventListener('submit', async function(e) {
    e.preventDefault();

    const password = this.querySelector('[name="Password"]').value;
    const confirmPassword = this.querySelector('[name="ConfirmPassword"]').value;

    if (password !== confirmPassword) {
        showToast("Пароли не совпадают", false);
        return;
    }
    
    console.log(password);
    console.log(confirmPassword);

    const formData = {
        Username: this.querySelector('[name="Username"]').value,
        Email: this.querySelector('[name="Email"]').value,
        Password: password
    };
    
    console.log(formData);

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        setUIBusy("Регистрируем аккаунт...");

        const response = await fetch('/Auth?handler=Register', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });

        if (response.status === 400) {
            const error = await response.text();
            showToast(error.includes("already exists") ?
                "Пользователь с таким логином или email уже существует" :
                "Ошибка регистрации", false);
            return;
        }

        if (response.status === 401) {
            showToast('Ошибка создания пользователя', false);
            return;
        }

        if (!response.ok) {
            showToast('Ошибка регистрации', false);
            return;
        }

        showToast("Регистрация прошла успешно");

        if (response.redirected) {
            window.location.href = response.url;
        }

    } catch (err) {
        console.error("Registration error:", err);
        showToast(err.message, false);
    } finally {
        clearUIBusy();
    }
});

document.querySelectorAll('[data-bs-toggle="tab"]').forEach(tab => {
    tab.addEventListener('click', function () {
        document.querySelectorAll('.tab-pane').forEach(pane => {
            pane.classList.remove('show', 'active');
        });
        const target = document.querySelector(this.getAttribute('data-bs-target'));
        target.classList.add('show', 'active');
    });
});

if (window.location.hash === '#register') {
    const registerTab = document.getElementById('register-tab');
    const tabInstance = new bootstrap.Tab(registerTab);
    tabInstance.show();
}