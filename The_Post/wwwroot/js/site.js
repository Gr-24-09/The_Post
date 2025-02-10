﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: "toast-bottom-right",    
    timeOut: 3000,
    extendedTimeOut: 1000,
   
};

// Editor's Choice slider in Jquery
$(document).ready(function () {
    $('.editors-choice-toggle').change(function () {
        var $switch = $(this);
        var articleId = $switch.data('article-id');
        var isChecked = $switch.prop('checked');

        $.ajax({
            url: '/Admin/UpdateEditorsChoice',
            type: 'POST',
            data: {
                articleId: articleId,
                isEditorsChoice: isChecked
            },
            success: function (response) {
                if (response.success) {
                    toastr.success('Editor\'s choice updated successfully');
                }
                else {
                    $switch.prop('checked', !isChecked);
                    toastr.error(response.message);
                }
            },
            error: function () {
                $switch.prop('checked', !isChecked);
                toastr.error('An error occured while updating editor\'s choice');
            }
        });
    });
});

// isArchived checkbox in JavaScript
document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".is-archive-check").forEach((checkbox) => {
        checkbox.addEventListener("change", async function () {
            const articleId = this.dataset.articleId;
            const isChecked = this.checked;

            try {
                const response = await fetch('/Admin/ArchiveArticle', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ articleId: articleId, isArchived: isChecked })
                });

                const result = await response.json();

                if (response.ok && result.success) {
                    toastr.success("Archived status updated successfully");
                }
                else {
                    this.checked = !isChecked;
                    toastr.error(result.message || "An error occurred while updating archived status");
                }
            }
            catch (error) {
                this.checked = !isChecked;
                toastr.error("An unexpected error occurred");
            }
        });
    });
});