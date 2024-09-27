var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    $('#tblData_tbody').html('<tr><td colspan="12" class="text-center"><span class="loader"></span></td></tr>');
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Employee/GetAll"
        },
        "columns":
            [
                { "data": "name", "width": "10" },
                { "data": "gender", "width": "20" },
                { "data": "mobileNumber", "width": "10%" },
                { "data": "email", "width": "10%" },
                { "data": "salary", "width": "10%" },
                {
                    "data": "dob", "width": "10%",
                    "render": function (data) {
                        return moment(data).format('DD-MMM-YYYY');
                    }
                },
                { "data": "panNumber", "width": "10%" },
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                    <div class="text-center">
                    <a href="/Employee/EmployeeDetails/${data}" title="View Detail" class="btn btn-primary">
                    <i class="fa fa-eye Action"></i>
                    </a>
                    <button type="button" class="btn btn-info" title="Edit" onclick="getEmployee(${data})" 
                    data-toggle="modal" data-target="#employeeModal"><i class="fa fa-pencil-square-o Action"></i></button>
                    </a>
                    <a class="btn btn-danger" title="Delete" onclick=Delete("/Employee/Delete/${data}")>
                    <i class="fas fa-trash Action"></i>
                    </a>
                    
                    </div>


                    `;
                    }
                }
            ]
    }

    )
}

function getEmployee(id) {
    id && id > 0 ? $('#employeeModalLabel').text('Update Employee') : $('#employeeModalLabel').text('Add Employee')
    $('#employeeModalBody').html('<div class="text-center"><span class="loader"></span></div>')
    $.ajax({
        url: "/Employee/CreateUpdate?employeeId=" + id,
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


function Delete(url) {
    swal({
        title: "Are you sure",
        text: "Once deleted , can not be recovered",
        icon: "warning",
        buttons: true,
        dangerModel: true


    }).then((willdelete) => {
        if (willdelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (ok) {
                    loadDataTable()
                    toastr.success("Employee has been deleted successfully.");
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    toastr.error("Something went wrong, please try again.");
                }

                

            })
        }

    })
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