$(document).ready(function () {
    var week = ['SUN', 'MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT'];
    var clockElement = document.getElementById('clock');
    var dateElement = clockElement.querySelector('.date');
    var timeElement = clockElement.querySelector('.time');
    updateTime();
    setInterval(updateTime, 1000);
    function updateTime() {
        var cd = new Date();
        var timeString = zeroPadding(cd.getHours(), 2) + ':' + zeroPadding(cd.getMinutes(), 2) + ':' + zeroPadding(cd.getSeconds(), 2);
        var dateString = zeroPadding(cd.getFullYear(), 4) + '-' + zeroPadding(cd.getMonth() + 1, 2) + '-' + zeroPadding(cd.getDate(), 2) + ' ' + week[cd.getDay()];
        timeElement.textContent = timeString;
        dateElement.textContent = dateString;
    }

    function zeroPadding(num, digit) {
        var zero = '';
        for (var i = 0; i < digit; i++) {
            zero += '0';
        }
        return (zero + num).slice(-digit);
    }

    var inTime = $('#inTime').text().trim();
    var outTime = $('#outTime').text().trim();

    if (outTime == "" && inTime == "") {
        $('#punch-in').show();
        $('#punch-out').hide();
    } else if (inTime !== "" && outTime == "") {
        $('#punch-in').hide();
        $('#punch-out').show();
    } else if (outTime != "" && inTime != "") {
        $('#punch-out').hide();
        $('#punch-in').hide();
    }
    $('#clock').show();

    $(document).on('click', '#punch-in', function (e) {
        e.preventDefault();
        var inTime = new Date();
        var currentTimeString = inTime.toISOString();
        localStorage.setItem('PunchInTime', currentTimeString);
        var employeeId = document.getElementById('empid').value;
        var attendanceDto = { EmployeeId: employeeId };
        $.ajax({
            url: '/IndividualEmployee/PunchIn',
            type: 'POST',
            data: JSON.stringify(attendanceDto),
            processData: false,
            contentType: 'application/json',
            success: function (response) {
                toastr.success("Marked-In successfully.");
                $('#punch-in').hide();
                $('#punch-out').show();
                location.reload();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                toastr.error("Something went wrong, please try again.");
            }
        });
    });

    $(document).on('click', '#punch-out', function (e) {
        e.preventDefault();
        var employeeId = document.getElementById('empid').value;
        var attendanceDto = { EmployeeId: employeeId };

        var timePart = $('#inTime').text();
        var isPM = timePart.includes('PM');
        timePart = timePart.replace('AM', '').replace('PM', '').trim();
        var parts = timePart.split(':');
        var hours = parseInt(parts[0]);
        var minutes = parseInt(parts[1]);
        if (isPM && hours < 12) {
            hours += 12;
        }
        var inTime = new Date();
        inTime.setHours(hours, minutes, 0, 0);

        var outTime = new Date();
        var timeDifference = (outTime - inTime) / 1000;

        if (timeDifference < (9 * 60 * 60)) {
            bootbox.confirm({
                title: "Are you sure ?",
                message: "Note that a half-day might be considered as 9 hours are not completed yet.",
                buttons: {
                    confirm: {
                        label: '<i class="fa fa-check"></i> Confirm',
                        className: 'btn-danger'
                    },
                    cancel: {
                        label: '<i class="fa fa-times"></i> Cancel',
                        className: 'btn-secondary'
                    }
                },
                callback: function (result) {
                    if (result) {
                        $.ajax({
                            url: "/IndividualEmployee/PunchOut",
                            type: 'POST',
                            data: JSON.stringify(attendanceDto),
                            processData: false,
                            contentType: 'application/json',
                            success: function (response) {
                                toastr.success("Marked-Out successfully.");
                                $('#punch-in').hide();
                                $('#punch-out').hide();
                                location.reload();
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                toastr.error("Something went wrong, please try again.");
                            }
                        });
                    }
                }
            });
        } else {
            $.ajax({
                url: "/IndividualEmployee/PunchOut",
                type: 'POST',
                data: JSON.stringify(attendanceDto),
                processData: false,
                contentType: 'application/json',
                success: function (response) {
                    toastr.success("Marked-Out successfully.");
                    $('#punch-in').hide();
                    $('#punch-out').hide();
                    location.reload();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    toastr.error("Something went wrong, please try again.");
                }
            });
        }
    });
});