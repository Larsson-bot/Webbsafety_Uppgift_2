// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function GetFileName() {
    // Get your file input (by it's id)
    var fileInput = document.getElementById('FileUpload_FormFile');
    // Update filename in UI.
    var filename = document.getElementById('FileName');
    filename.innerText = "Selected File: " + fileInput.files[0].name;
}