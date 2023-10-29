function modal_control(modal_name, action) {
    document.getElementById(modal_name).style.display = action;
}

function openModalSet(id) {
    modal_control("deleteModal", "block");
    document.getElementById('AdminId').innerHTML = id;
}

function deleteAdmin() {
    let AdminId = document.getElementById("AdminId").innerHTML;
    window.location.href = "/Admin/Delete?adminId=" + AdminId;
}