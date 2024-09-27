let countdownInterval;
function processToken(token) {
    if (countdownInterval) {
        clearInterval(countdownInterval);
    }
    const expirationTime = extractTokenExpiration(token);
    localStorage.setItem('expirationTime', expirationTime.getTime());

    if (expirationTime > new Date()) {
        startCountdown(expirationTime);
    }
}

function startCountdown(expirationTime) {
    countdownInterval = setInterval(() => {
        const remainingTime = expirationTime - new Date();

        if (remainingTime <= 0) {
            clearInterval(countdownInterval);
        } else {
            console.log('Token expiration in', remainingTime / 1000, 'seconds');
            if (remainingTime <= 60000) {
                clearInterval(countdownInterval);
                showPopupWithCountdown();
            }
        }
    }, 1000);
}

const token = localStorage.getItem('token');
const expirationTime = new Date(parseInt(localStorage.getItem('expirationTime')));
if (expirationTime && expirationTime > new Date()) {
    startCountdown(expirationTime);
}

function extractTokenExpiration(token) {
    const tokenParts = token.split('.');
    const payload = JSON.parse(atob(tokenParts[1]));
    const expirationTime = new Date(payload.exp * 1000);
    return expirationTime;
}
function showPopupWithCountdown() {
    const expirationTime = parseInt(localStorage.getItem('expirationTime'));
    const currentTime = new Date().getTime();
    let timeLeft = Math.floor((expirationTime - currentTime) / 1000);

    if (timeLeft <= 0) {
        return; 
    }

    const popup = document.createElement('div');
    popup.classList.add('popup');
    document.body.appendChild(popup);

    const popupHeader = document.createElement('div');
    popupHeader.classList.add('popup-header');
    popup.appendChild(popupHeader);

    const logo = document.createElement('img');
    logo.src = '/images/Warning.jpg';
    logo.alt = 'Warning Logo';
    logo.classList.add('popup-logo');
    popupHeader.appendChild(logo);

    const title = document.createElement('h1');
    title.classList.add('popup-title');
    title.textContent = 'Warning!';
    popupHeader.appendChild(title);

    const countdown = document.createElement('p1');
    countdown.classList.add('countdown');
    countdown.innerHTML = 'Oh no..! Session is going to expire in <span id="countdown" class="countdown-text">' + timeLeft + ' seconds </span>. Please click on Extend button to continue..';
    popup.appendChild(countdown);

    const buttonContainer = document.createElement('div');
    buttonContainer.classList.add('button-container');
    popup.appendChild(buttonContainer);

    const continueButton = document.createElement('button');
    continueButton.textContent = 'Extend';
    continueButton.onclick = function () {
        const refreshToken = localStorage.getItem('RefreshToken');
        $.ajax({
            url: "/Login/RefreshToken",
            type: "POST",
            data: { refreshtoken: refreshToken },
            success: function (response) {
                toastr.success("Session Extended")
                clearInterval(interval);
                localStorage.setItem('userData', response);
                const responseObject = JSON.parse(response);
                const token = responseObject.token;
                localStorage.setItem('token', token);
                const refreshToken = responseObject.refreshToken;
                localStorage.setItem('RefreshToken', refreshToken);
                document.body.removeChild(popup);
                processToken(token);
                return;
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("Status: " + textStatus); alert("Error: " + errorThrown);
            }
        });
    };
    buttonContainer.appendChild(continueButton);

    const cancelButton = document.createElement('button');
    cancelButton.textContent = 'Cancel';
    cancelButton.onclick = function () {
        $.ajax({
            url: '/Login/Logout',
            type: 'POST',
            success: function (response) {
                clearInterval(interval);
                localStorage.removeItem('userData');
                document.body.removeChild(popup);
                window.location.href = '/Login/Index';
                toastr.success("Logout Successfully.");
               
            },
            error: function (xhr, status, error) {
                toastr.error('Logout failed: ' + xhr.responseText);
            }
        });
        
    };
    buttonContainer.appendChild(cancelButton);

    const interval = setInterval(() => {
        timeLeft--;
        document.getElementById('countdown').textContent = timeLeft + ' seconds';
        if (timeLeft <= 0) {
            clearInterval(interval);
            window.location.href = '/Login/Index';
            document.body.removeChild(popup);
        }
    }, 1000);
}