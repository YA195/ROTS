    document.addEventListener('DOMContentLoaded', () => {
        const wrapper = document.querySelector('.wrapper');
        const loginLink = document.querySelector('.login-link');
        const registerLink = document.querySelector('.register-link');

        // Check TempData for 'WrapperActive' value
        const wrapperActive = '@TempData["WrapperActive"]';

        // Add 'active' class to the wrapper if 'WrapperActive' is set
        if (wrapperActive === 'active') {
            wrapper.classList.add('active');
        }

        registerLink.addEventListener('click', () => {
            wrapper.classList.add('active');
        });

        loginLink.addEventListener('click', () => {
            wrapper.classList.remove('active');
        });
    });
