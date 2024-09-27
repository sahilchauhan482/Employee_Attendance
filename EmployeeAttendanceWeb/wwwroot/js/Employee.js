var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    $('#tblData_tbody').html('<tr><td colspan="12" class="text-center"><span class="loader"></span></td></tr>');
    $.ajax({
        url: "Employee/GetAll",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $('#tblData_tbody').html('');
            dataTable = $('#tblData').DataTable({
                "scrollX": true,
                "bDestroy": true,
                "pageLength": 10,
                "drawCallback": function () {
                    $('.btn').addClass('btn-sm'); 
                },
                "fixedColumns": {
                    leftColumns: 1
                },
                "columnDefs": [
                    { "width": "10%", "targets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10] },
                    { "width": "10%", "targets": [11] }
                ],
                "language": {
                    "emptyTable": "No data available in table"
                }
            });

            dataTable.clear().draw();
            $.each(data, function (key, value) {
                let actionButtons =
                    '<div class="btn-group" role="group" aria-label="Basic example">' +
                    '<button type="button" class="btn btn-info" onclick="getEmployee(' + value.id + ')" data-toggle="modal" data-target="#employeeModal"><i class="fa fa-pencil-square-o"></i></button>' +
                    '<button type="button" class="btn btn-danger" onclick="deleteEmployee(' + value.id + ')"><i class="fa fa-trash-o"></i></button></div>'
                let row = dataTable.row.add([
                    value.name,
                    value.gender,
                    value.mobileNumber,
                    value.email,
                    new Date(value.joiningDate).toLocaleDateString('en-GB'),
                    value.salary,
                    new Date(value.dob).toLocaleDateString('en-GB'),
                    value.panNumber,
                    actionButtons,
                ]).draw(false).node();
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401) {
                window.location.href = '/Login/Index';
            }
            
            $('#tblData_tbody').html('<tr><td colspan="12">Error loading data</td></tr>');
            console.error("Error loading data:", errorThrown);
        }
    });
}
function getEmployee(employeeId) {
    employeeId && employeeId > 0 ? $('#employeeModalLabel').text('Update Employee') : $('#employeeModalLabel').text('Add Employee')
    $('#employeeModalBody').html('<div class="text-center"><span class="loader"></span></div>')
    $.ajax({
        url: "Employee/CreateUpdate?employeeId=" + employeeId,
        type: "GET",
        success: function (data) {
            $('#employeeModalBody').html('').html(data);
            attach();
            employeeId && employeeId > 0 ? $('#employee-submit-btn').text('Update') : $('#employee-submit-btn').text('Save')
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    });
}

function deleteEmployee(employeeId) {
    swal({
        title: "Want to delete data..?",
        text: "Delete confirmation",
        icon: "warning",
        buttons: true,
        dangerModel: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: "Employee/Delete?employeeId=" + employeeId,
                type: "DELETE",
                success: function (ok) {
                    loadDataTable()
                    toastr.success("Employee has been deleted successfully.");
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    toastr.error("Something went wrong, please try again.");
                }
            });
        }
    });
}

function countrySelected(self) {
    let countryId = $(self).val();
    $.ajax({
        url: "Employee/GetStatesByCountryId?countryId=" + countryId,
        type: "GET",
        dataType: "json",
        success: function (data) {
            console.log(data);
            $('#state').empty().append('<option value="">--Select State--</option>');
            $('#city').empty().append('<option value="">--Select City--</option>');
            $.each(data, function (key, value) {
                $('#state').append('<option value="' + value.id + '">' + value.name + '</option>');
            })
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    });
}
function stateSelected(self) {
    let stateId = $(self).val();
    $.ajax({
        url: "Employee/GetCitiesByStateId?stateId=" + stateId,
        type: "GET",
        dataType: "json",
        success: function (data) {
            console.log(data);
            $('#city').empty().append('<option value="">--Select City--</option>');
            $.each(data, function (key, value) {
                $('#city').append('<option value="' + value.id + '">' + value.name + '</option>');
            })
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("Status: " + textStatus); alert("Error: " + errorThrown);
        }
    });
}





