$(document).ready(function () {
    

    $('.in-time, .out-time').val();
    $('#update-attendance').hide();
    $('#save-attendance').hide();
    showSaveUpdate();
    updateCounts();
    calculateAndDisplayDuration()
    
    $('tbody tr').each(function () {
        var row = $(this);
        var leaveCheckbox = row.find('.leave-checkbox');
        var absentCheckbox = row.find('.absent-checkbox');
        var inTimeInput = row.find('.in-time');
        var outTimeInput = row.find('.out-time');
        var status = row.find('.status')
        var inTimeValue = inTimeInput.val().trim();
        var outTimeValue = outTimeInput.val().trim();
        
        if (inTimeValue) {
            leaveCheckbox.prop('disabled', true).prop('checked', false);
            absentCheckbox.prop('disabled', true).prop('checked', false);
            status.val("Present").css({ "color": "white", "background-color": "green", "font-weight": "bold" });
            updateCounts()
        } else {
            leaveCheckbox.prop('disabled', false);
            absentCheckbox.prop('disabled', false);
        }
        if (leaveCheckbox.prop('checked')) {
            inTimeInput.prop('disabled', true);
            outTimeInput.prop('disabled', true);
            status.val("OnLeave").css({ "color": "black", "background-color": "yellow", "font-weight": "bold" });
            updateCounts()
            absentCheckbox.prop('checked', false).prop('disabled', true);
        } else if (absentCheckbox.prop('checked')) {
            inTimeInput.prop('disabled', true);
            outTimeInput.prop('disabled', true);
            status.val("Absent").css({ "color": "white", "background-color": "red", "font-weight": "bold" });
            updateCounts()
            leaveCheckbox.prop('checked', false).prop('disabled', true);
        } else {
            inTimeInput.prop('disabled', false);
            outTimeInput.prop('disabled', false);

            if (!inTimeValue && !outTimeValue) {
                leaveCheckbox.prop('disabled', false);
                absentCheckbox.prop('disabled', false);
                status.val("").css({ "color": "", "background-color": "" });
                updateCounts()
            }
        }
        
    });
});
$(document).on('change', '.in-time, .out-time', function () {
    calculateAndDisplayDuration();
});

$(document).on('click', '#update-attendance', function () {
    var data = [];
    $('#attendance-table tbody tr').each(function () {
        var row = $(this);
        var employeeId = row.find('td.employeeId').text().trim();
        var id = row.find('td.id').text().trim();
        var inTime = row.find('.in-time').val();
        var outTime = row.find('.out-time').val();
        var duration = row.find('.duration').val();
        var leave = row.find('.leave-checkbox').prop('checked');
        var absent = row.find('.absent-checkbox').prop('checked');
        var date = $('#attendance-date').val();
        data.push({ EmployeeId: employeeId, Id: id, InTime: inTime, OutTime: outTime, Leave: leave, Absent: absent, AttendanceDate: date, Duration: duration });
    });
    $.ajax({
        url: '/Attendance/Edit',
        type: 'PUT',
        data: { attendanceDTO: data },
        success: function (response) {
            toastr.success('Attendance updated successfully!');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401) {
                window.location.href = '/Login/Index';
            }
            toastr.error('Error updating attendance!');
        }
    });
});

$('#attendance-date').change(function () {

    var selectedDate = $(this).val();

    $('#attendance_data').html('<div class="text-center"><span class="loader"></span></div>')
    $.ajax({
        url: '/Attendance/GetByDate',
        type: 'GET',
        data: { date: selectedDate },
        success: function (response) {

            $('#attendance_data').html("").html(response)
            $('tbody tr').each(function () {
                var row = $(this);
                var leaveCheckbox = row.find('.leave-checkbox');
                var absentCheckbox = row.find('.absent-checkbox');
                var inTimeInput = row.find('.in-time');
                var outTimeInput = row.find('.out-time');
                var status = row.find('.status')
                var inTimeValue = inTimeInput.val().trim();
                var outTimeValue = outTimeInput.val().trim();
                if (inTimeValue && outTimeValue) {
                    leaveCheckbox.prop('disabled', true).prop('checked', false);
                    absentCheckbox.prop('disabled', true).prop('checked', false);
                    status.val("Present").css({ "color": "white", "background-color": "green", "font-weight": "bold" });
                    updateCounts()

                } else {
                    leaveCheckbox.prop('disabled', false);
                    absentCheckbox.prop('disabled', false);
                }
                if (leaveCheckbox.prop('checked')) {
                    inTimeInput.prop('disabled', true);
                    outTimeInput.prop('disabled', true);
                    status.val("OnLeave").css({ "color": "black", "background-color": "yellow", "font-weight": "bold" });
                    updateCounts()
                    absentCheckbox.prop('checked', false).prop('disabled', true);
                } else if (absentCheckbox.prop('checked')) {
                    inTimeInput.prop('disabled', true);
                    outTimeInput.prop('disabled', true);
                    status.val("Absent").css({ "color": "white", "background-color": "#ffb3b3", "font-weight": "bold" });
                    updateCounts()
                    leaveCheckbox.prop('checked', false).prop('disabled', true);
                } else {
                    inTimeInput.prop('disabled', false);
                    outTimeInput.prop('disabled', false);

                    if (!inTimeValue && !outTimeValue) {
                        leaveCheckbox.prop('disabled', false);
                        absentCheckbox.prop('disabled', false);
                        status.val("").css({ "color": "", "background-color": "" });
                        updateCounts()
                    }
                }
            });

            showSaveUpdate()
            calculateAndDisplayDuration();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401) {
                window.location.href = '/Login/Index';
            }
        }
    });
});

$(document).on('change', 'tbody tr', function () {
    var row = $(this);
    var leaveCheckbox = row.find('.leave-checkbox');
    var absentCheckbox = row.find('.absent-checkbox');
    var inTimeInput = row.find('.in-time');
    var outTimeInput = row.find('.out-time');
    var status = row.find('.status')
    var inTimeValue = inTimeInput.val().trim();
    var outTimeValue = outTimeInput.val().trim();
    if (inTimeValue && outTimeValue) {
        leaveCheckbox.prop('disabled', true).prop('checked', false);
        absentCheckbox.prop('disabled', true).prop('checked', false);
        status.val("Present").css({ "color": "white", "background-color": "green", "font-weight": "bold" });
        updateCounts()
    } else {
        leaveCheckbox.prop('disabled', false);
        absentCheckbox.prop('disabled', false);
    }
    if (leaveCheckbox.prop('checked')) {
        inTimeInput.prop('disabled', true);
        outTimeInput.prop('disabled', true);
        status.val("OnLeave").css({ "color": "black", "background-color": "yellow", "font-weight": "bold" });
        updateCounts()
        absentCheckbox.prop('checked', false).prop('disabled', true);
    } else if (absentCheckbox.prop('checked')) {
        inTimeInput.prop('disabled', true);
        outTimeInput.prop('disabled', true);
        status.val("Absent").css({ "color": "white", "background-color": "#ffb3b3", "font-weight": "bold" });
        updateCounts()
        leaveCheckbox.prop('checked', false).prop('disabled', true);
    } else {
        inTimeInput.prop('disabled', false);
        outTimeInput.prop('disabled', false);
        
        if (!inTimeValue && !outTimeValue) {
            leaveCheckbox.prop('disabled', false);
            absentCheckbox.prop('disabled', false);
            status.val("").css({ "color": "", "background-color": "" });
            updateCounts()
        }
    }
});

$(document).on('click', '#save-attendance', function () {
    var data = [];
    $('#attendance-table tbody tr').each(function () {
        var row = $(this);
        var employeeId = row.find('td.employeeId').text().trim();
        var id = row.find('td.id').text().trim();
        var inTime = row.find('.in-time').val();
        var outTime = row.find('.out-time').val();
        var duration = row.find('.duration').val();
        var leave = row.find('.leave-checkbox').prop('checked');
        var absent = row.find('.absent-checkbox').prop('checked');
        var date = $('#attendance-date').val();
        data.push({ EmployeeId: employeeId, Id: id, InTime: inTime, OutTime: outTime, Leave: leave, Absent: absent, AttendanceDate: date, Duration: duration });
    });
    $.ajax({
        url: '/Attendance/Create',
        type: 'POST',
        data: { attendanceDto: data },
        success: function (response) {
            toastr.success('Attendance saved successfully!');
        },
        error: function () {
            toastr.error('Error saving attendance!');
        }
    });
});

function showSaveUpdate() {
    let haveUpdate = false;
    $('#attendance-table tbody tr').each(function () {
        var id = $(this).find('td.id').text().trim();
        if (id && parseInt(id) > 0) {
            $('#update-attendance').show();
            $('#save-attendance').hide();
            haveUpdate = true;
        }
    })
    if (!haveUpdate) {
        $('#update-attendance').hide();
        $('#save-attendance').show();
    }
}

function calculateAndDisplayDuration() {
    $('#attendance-table tbody tr').each(function () {
        var row = $(this);
        var inTime = row.find('.in-time').val();
        var outTime = row.find('.out-time').val();
        var durationField = row.find('.duration');
        var status1 = row.find('.status1')
        if (inTime.trim() !== '' && outTime.trim() !== '') {
            var duration = calculateDuration(inTime, outTime);
            durationField.val(duration);
            var parts = duration.split(':');
            var hours = parseInt(parts[0], 10);
            var minutes = parseInt(parts[1], 10);
            var totalMinutes = hours * 60 + minutes;
            if (totalMinutes < 270)
            { status1.val("Unpaid L").css({ "color": "white", "background-color": "red", "font-weight": "bold" }); }
            else if (totalMinutes > 270 && totalMinutes < 420) {
                status1.val("Half Day").css({ "color": "white", "background-color": "orange", "font-weight": "bold" });
            } else if (totalMinutes >= 420 && totalMinutes < 540) {
                status1.val("S.Leave").css({ "color": "white", "background-color": "blue", "font-weight": "bold" });
            } else {
                status1.val("").css({ "color":"", "background-color": "", "font-weight": "" });
            }
        } else {
            durationField.val('hh:mm');
            status1.val("");
        }
        
        

    });
}

function calculateDuration(inTime, outTime) {
    var inDateTime = new Date("2000-01-01T" + inTime + ":00");
    var outDateTime = new Date("2000-01-01T" + outTime + ":00");
    var durationMs = outDateTime - inDateTime;
    var hours = Math.floor(durationMs / 3600000);
    var minutes = Math.floor((durationMs % 3600000) / 60000);
    return hours.toString().padStart(2, '0') + ':' + minutes.toString().padStart(2, '0');
}

function updateCounts() {
    var totalEmployee = $('tbody tr').length;
    var markedEmployee = $('tbody tr').filter(function () {
        return $(this).find('.status').val().trim() !== '';
    }).length;
    var unmarkedEmployee = totalEmployee - markedEmployee;

    $('.total_employee').text(totalEmployee);
    $('.marked_employee').text(markedEmployee);
    $('.unmarked_employee').text(unmarkedEmployee);
}


$(document).on('input','#inTimeInput', function () {
    document.getElementById('inTimeInput').value
    var minTime = '09:00';
    if (this.value < minTime) {
        this.value = minTime;
    }
});

$(document).on('input', '#outTimeInput', function () {
    document.getElementById('outTimeInput').value
    var maxtime = '22:00';
    if (this.value > maxtime) {
        this.value = maxtime;
    }
});


$(document).on('click','.erase-button',function() {
        $('.erase-button').click(function () {
            $(this).closest('.input-group').find('input[type="time"]').val('');
        });
    });




