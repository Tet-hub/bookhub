function modal_control(modal_name, action) {
    document.getElementById(modal_name).style.display = action;
}

function openModalSet(id, adminName) {
    modal_control("deleteModal", "block");
    document.getElementById('AdminId').innerHTML = id;
    document.getElementById('adminName').innerHTML = adminName + "?";
}

function deleteAdmin() {
    let AdminId = document.getElementById("AdminId").innerHTML;
    window.location.href = "/Admin/Delete?adminId=" + AdminId;
}

window.onclick = function(event) {
    let modal = document.getElementById('deleteModal');
  if (event.target == modal) {
    modal.style.display = 'none';
  }
}