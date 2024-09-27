$(document).ready(function () {
    $('#loginForm').submit(function (e) {
        e.preventDefault();
        $('#loginButton').html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Loading...').prop('disabled', true);
        var data =[] ;
        var username = $('#username').val();
        var password = $('#password').val();
        var data={
            Username: username, Password: password
        };
            $.ajax({
                url: '/Login/Login',
                type: 'POST',
                data: { userDto: data },
                success: function (response) {
                    localStorage.setItem('userData', response); 
                    const responseObject = JSON.parse(response);
                    const token = responseObject.token;
                    localStorage.setItem('token', token);
                    const decodedToken = atob(token.split('.')[1]);
                    const { 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': role } = JSON.parse(decodedToken);
                    console.log(role);
                    const refreshToken = responseObject.refreshToken;
                    localStorage.setItem('RefreshToken', refreshToken);
                    processToken(token);
                    if (role == "Employee")
                        window.location.href = '/IndividualEmployee/Index';
                    else
                    window.location.href = '/Home/Index';
                    toastr.success("Login Successfully.");
                    
            },
                error: function (xhr, status, error) {
                    toastr.error('Login failed: ' + xhr.responseText);
                },
                complete: function () {
                    $('#loginButton').html('Login').prop('disabled', false);
                }
        });
    });

    $(document).on("click", "#logoutForm",function (e) {
        $.ajax({
            url: '/Login/Logout',
            type: 'POST',
            success: function (response) {
                localStorage.clear();
                toastr.success("Logout Successfully.");
                window.location.href = '/Login/Index';
                
            },
            error: function (xhr, status, error) {
                toastr.error('Logout failed: ' + xhr.responseText);
            }
        });
    });
    $(document).on('submit', '#signup', function (e) {
        e.preventDefault();
        var data = [];
        var username = $('#SignupUsername').val();
        var password = $('#SignupPassword').val();
        var data = {
            Username: username, Password: password
        };
        $.ajax({
            url: '/Login/Register',
            type: 'POST',
            data: { userDto: data },
            success: function (response) {
                toastr.success("User has Registered successfully.");
            },
            error: function (xhr, status, error) {
                toastr.error("Registration failed.");

            }
        });
    });
});

