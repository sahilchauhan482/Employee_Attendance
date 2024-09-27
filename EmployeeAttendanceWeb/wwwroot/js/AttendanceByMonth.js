$('#monthInput').change(function () {
    var selectedDate = $(this).val();
    $('#attendance_data').html('<div class="text-center"><span class="loader"></span></div>')
    $.ajax({
        url: '/AttendanceMonthly/GetByMonth',
        type: 'GET',
        data: { date: selectedDate },
        success: function (response) {

            $('#attendance_data').html("").html(response)
            if ($('#attendance_data').html(response)) {
                $('#error_message').hide();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            if ($('#attendance_data').html("")) {
                $('#error_message').text('No Record Found').show();
            }

            else
                alert('Error fetching attendance details!');
        }
    });
});

document.querySelector('#Report').addEventListener('click', function () {
    var selectedDate = document.getElementById('monthInput').value;
    $.ajax({
        url: '/AttendanceMonthly/MonthlyReport',
        type: 'POST',
        data: { date: selectedDate },
        xhrFields: {
            responseType: 'blob' 
        },
        success: function (response, status, xhr) {
            var filename = 'AttendanceMonthly.xlsx';
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                var matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) {
                    filename = matches[1].replace(/['"]/g, '');
                }
            }
            var blob = new Blob([response], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement('a');
            a.href = url;
            a.download = filename; 
            a.style.display = 'none';
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error in downloading the report!');
        }
    });
});