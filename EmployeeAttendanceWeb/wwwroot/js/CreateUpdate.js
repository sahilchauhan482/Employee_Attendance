$(document).ready(function () {

});
$(document).on('submit', '#registerForm', function (e) {
    e.preventDefault();
    const form = document.getElementById('registerForm');
    const formData = new FormData(form);
    $.ajax({
        url: $(this).attr('action'),
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            window.location.href = '/Employee/Index';
            toastr.success("Employee has been Added successfully.");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401) {
                window.location.href = '/Login/Index';
            }
            alert('Login failed: ' + xhr.responseText);
            toastr.error("Something went wrong, please try again.");
        }
    });
});

function sendEmailValue() {
    var email = document.getElementById("email").value;
    var empid = document.getElementById("employeeid").value;
    $.ajax({
        url: '/Employee/EmailCheck/',
        type: 'POST',
        data: { email: email, empid: empid },
        success: function (response) {
            if (email) {
                $('#error').text('Email Id Already Exist').css('color', 'red').show();
                $('#employee-submit-btn').prop('disabled', true);
            }
        },
        error: function (xhr, status, error) {
            $('#error').hide();
            $('#employee-submit-btn').prop('disabled', false);
        }
    });
}
function attach() {
    document.getElementById('mobile').addEventListener('input', function (event) {
        var inputValue = event.target.value;
        var regex = /^[0-9]*$/;
        if (!regex.test(inputValue)) {
            event.target.value = inputValue.slice(0, -1);
        }
    });
    $("#mobile").on('keyup', function () {
        var phoneNumberInput = document.getElementById("mobile");
        var phonenumber = phoneNumberInput.value;
        var employeeId = document.getElementById("employeeid").value;
        if (phonenumber.length !== 10 && phonenumber.length > 0) {
            $('#errorPhone').text('Mobile number must be 10 digits').css('color', 'red').show();
            return;
        } else {
            $('#errorPhone').hide();
        }

        $.ajax({
            url: '/Employee/PhoneNoCheck/',
            type: 'POST',
            data: { phonenumber: phonenumber, empid: employeeId },
            success: function (response) {
                if (phonenumber) {
                    $('#errorPhone').text('Mobile Number Already Exist').css('color', 'red').show();
                    $('#employee-submit-btn').prop('disabled', true);
                }
            },
            error: function (xhr, status, error) {
                $('#errorPhone').hide();
                $('#employee-submit-btn').prop('disabled', false);
            }
        });
    });
}


function CheckAge() {
    var dateOfBirth = document.getElementById("age").value;
    var today = new Date();
    var birthDate = new Date(dateOfBirth);
    var age = today.getFullYear() - birthDate.getFullYear();
    var m = today.getMonth() - birthDate.getMonth();

    if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }

    if (age < 18) {
        $('#agechecking').text('Age must be 18 or older.').css('color', 'red').show();
        $('#employee-submit-btn').prop('disabled', true);

    }
    else {
        $('#employee-submit-btn').prop('disabled', false);
        $('#agechecking').hide();
    }
}