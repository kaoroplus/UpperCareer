﻿//Fix compatibility issue with CKEditor in a Bootstrap Modal.
$.fn.modal.Constructor.prototype.enforceFocus = function () {
    $(document)
        .off('focusin.bs.modal') // guard against infinite focus loop
        .on('focusin.bs.modal', $.proxy(function (e) {
            if (
                this.$element[0] !== e.target && !this.$element.has(e.target).length
                // CKEditor compatibility fix start.
                && !$(e.target).closest('.cke_dialog, .cke').length
                // CKEditor compatibility fix end.
            ) {
                this.$element.trigger('focus');
            }
        }, this));
};
//Trigger Bootstrap tooltip when page load.
$(function () {
    TriggerTooltip();
})
//Function to trigger Bootstrap tooltip.
function TriggerTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
}

function UpdateNewMsgCount() {
    $.ajax({
        url: updateMsgCount,
        type: 'POST',
        success: function (result) {
            $('#divNavBar').html(result);
        },
        error: function (e) {
            alert(e.responseText);
        }
    });
}

function UpdateCmtCount() {
    //For QuestionDetail page.
    if ($('.CmtCount').length) {
        $('.CmtCount').each(function () {
            var cmtCount = $(this).closest('.body').find('.comment-list').find('.box').length;
            if (cmtCount > 0) {
                $(this).find('span').html('(' + cmtCount + ')');
            }
        });
    }
        //For AnswerDetail page.
    else if ($('span.cmt-count')) {
        var count = $('span.cmt-count').parent().siblings('div[id^="divCmtList"]').first().children('div[id^="blkCmt"]:visible').length;
        $('span.cmt-count').text(count);
    }
}

function WrapImageContent(content) {
    //alert($(this).find('img').length)

    return content;
}

function DisplaySuccessInfo(infoText) {
    var successTab = $('#divInfoSuccess').find('p');
    $('div[id^="divInfo"]').stop(true, true).hide();
    successTab.html(infoText);
    successTab.closest('.alert-tag').fadeIn('slow').delay(2000).fadeOut('slow');
}

function DisplayErrorInfo(infoText) {
    var errorTab = $('#divInfoError').find('p');
    $('div[id^="divInfo"]').stop(true, true).hide();
    errorTab.html(infoText);
    errorTab.closest('.alert-tag').fadeIn('slow').delay(2000).fadeOut('slow');
}

function RemoveHtml(originString) {
    var container = document.createElement('div');
    var text = document.createTextNode(originString);
    container.appendChild(text);
    return container.innerHTML.replace(/&nbsp;/g, '').trim(); // innerHTML will be a xss safe string
}

$(document).ready(function () {
    $(document).on('click', 'button.close', function () {
        $(this).closest('.alert-tag').stop(true, true).fadeOut('fast');
    });

    $('#btnPostQuestion').click(function (e) {
        $.ajax({
            url: loadQuestionForm,
            type: 'POST',
            success: function (result) {
                $('#divModal').children().html(result);
            },
            error: function (e) {
                alert(e.responseText);
            }
        });
    });

    $(document.body).on('click', '#btnWriteLetter', function () {
        var targetId = $('#hdnSender').val();

        $.ajax({
            url: '/Message/CreateLetterForm',
            data: { targetId: targetId },
            dataType: 'html',
            type: 'POST',
            success: function (result) {
                $('#divModal').children().html(result);
            },
            error: function (e) {
                alert(e.responseText);
            }
        });
    })

    //$(document).on('submit', '.include-textarea', function (e) {
    //    e.preventDefault();
    //    var textArea = $(this).find('textarea');
    //    textArea.val(textArea.val().replace(/\r?\n/g, '&#13;&#10;'))
    //    alert(textArea.val())
    //});

    //Enable nav bar dropdown list with hover event.
    //$('ul.nav li.dropdown').hover(function () {
    //    $(this).find('.dropdown-menu').stop(true, true).delay(200).slideDown(200);
    //}, function () {
    //    $(this).find('.dropdown-menu').stop(true, true).delay(200).slideUp(200);
    //});
});
