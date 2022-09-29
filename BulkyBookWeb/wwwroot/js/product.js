var dataTable;

$(document).ready(function () {
    loadDataTable();

});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/Product/GetAll"
        },
        //The data columns are case sensitive. Title won't work. title would as displayed when GetAll method is called. The resultant json file changed the name
        //to lowercase
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "15%" },

            {
                "data": "id",
                "render": function (data) {
                    return `

                             <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary mx-2"> 
                                <i class="bi bi-pencil-square"></i> Edit </a>
                            
                            <a onClick=Delete('/Admin/Product/Delete/${data}') class="btn btn-danger mx-2">
                                <i class="bi bi-trash"></i> Delete
                            </a>
                            </div>`
                },
                "width": "15%"
            },

         

        ]
    });
}

function Delete(url) {
    //Sweet alert for delete. Following code from sweetalert2.com

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {


                    if (data.success == true) {

                        dataTable.ajax.reload();
                        toastr.success(data.message);

                    }
                    else {
                        toastr.error(data.message);
                    }

                            
                }
            })
        }
    })
}