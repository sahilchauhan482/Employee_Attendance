$('#monthInput').change(function () {
    var selectedDate = $(this).val();
    $('#attendance_data').html('<div class="text-center"><span class="loader"></span></div>')
    $.ajax({
        url: '/Payroll/GetByMonth',
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
