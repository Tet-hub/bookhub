let fragmentIdentifier = window.location.hash
let acc_gen_button = document.getElementById("acc-gen")
let acc_change_pass_button = document.getElementById("acc-change-pass")
let account_general_page = document.getElementById("account-general")
let account_change_password_page = document.getElementById("account-change-password")

if (fragmentIdentifier == "#admin-change-password") {
    acc_gen_button.classList.remove("active")
    acc_change_pass_button.classList.add("active")
    account_general_page.classList.remove("show")
    account_general_page.classList.remove("active")
    account_change_password_page.classList.add("show")
    account_change_password_page.classList.add("active")

}