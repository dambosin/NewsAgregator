let currentAriaLink = document.getElementById(ariaCurrent.toLowerCase());
if (currentAriaLink != null) {
    currentAriaLink.classList.add('active');
    currentAriaLink.setAttribute('aria-current', 'page');
}
if (localStorage.getItem("theme") == null) {
    localStorage.setItem("theme", "light");
}

let check = document.getElementById('darkmode-toggle');
let body = document.getElementsByTagName('body')[0];

if (localStorage.theme == "dark") {
    check.checked = true;
}
setTheme(localStorage.getItem("theme"));

function setTheme(theme) {
    body.setAttribute('data-bs-theme', theme);
}

function changeTheme() {
    if (check.checked) {
        setTheme('dark');
        localStorage.theme = "dark";
    } else {
        setTheme('light');
        localStorage.theme = "light";
    }
}

check.addEventListener('click', changeTheme);