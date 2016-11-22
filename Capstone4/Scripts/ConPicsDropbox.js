if (!Array.prototype.includes) {
    Array.prototype.includes = function (searchElement /*, fromIndex*/) {
        'use strict';
        if (this == null) {
            throw new TypeError('Array.prototype.includes called on null or undefined');
        }

        var O = Object(this);
        var len = parseInt(O.length, 10) || 0;
        if (len === 0) {
            return false;
        }
        var n = parseInt(arguments[1], 10) || 0;
        var k;
        if (n >= 0) {
            k = n;
        } else {
            k = len + n;
            if (k < 0) { k = 0; }
        }
        var currentElement;
        while (k < len) {
            currentElement = O[k];
            if (searchElement === currentElement ||
               (searchElement !== searchElement && currentElement !== currentElement)) { // NaN !== NaN
                return true;
            }
            k++;
        }
        return false;
    };
}

var selDiv = "";
var storedFiles = [];
var dupCheck = [];
var count = $(".conPhotos").children('img').length;
dropbox = document.getElementById("dropbox");
dropbox.addEventListener("dragenter", dragenter, false);
dropbox.addEventListener("dragover", dragover, false);
dropbox.addEventListener("drop", drop, false);

$(document).ready(function () {
    $("#fileInput").on("change", handleFilesFromClick);
    selDiv = $("#dropbox");
    $("body").on("click", ".selFile", removeFile);

});
$('#fileInput').click(function () {
    $("#dropbox").children('img').remove();
    storedFiles = [];
    dupCheck = [];
});

function dragenter(e) {
    e.stopPropagation();
    e.preventDefault();
}

function dragover(e) {
    e.stopPropagation();
    e.preventDefault();
    e.dataTransfer.dropEffect = 'copy';
}
function drop(e) {
    e.stopPropagation();
    e.preventDefault();

    var dt = e.dataTransfer;
    var files = dt.files;

    handleFiles(files);
}
function handleFiles(files) {
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var imageType = /^image\//;

        if (!imageType.test(file.type)) {
            continue;
        }
        var img = document.createElement("img");
        img.classList.add("obj");
        img.file = file;
        $(img).addClass('selFile');
        $(img).attr('data-file', file.name);
        var A = [];
        for (var z = 0; z < dupCheck.length; z++) {
            A.push(dupCheck[z].name)
        }
        if (!A.includes(file.name) && storedFiles.length + count < 4) {
            storedFiles.push(file);
            var _checker = { name: file.name };
            dupCheck.push(_checker);
            dropbox.appendChild(img);
            var reader = new FileReader();
            reader.onload = (function (aImg) { return function (e) { aImg.src = e.target.result; }; })(img);
            reader.readAsDataURL(file);
            A = [];
            if (storedFiles.length + count > 3) {
                $("#max").show();
            }
        };
    }
}
function handleFilesFromClick(e) {
    var nameIndex = 0;
    var files = e.target.files;
    var filesArr = Array.prototype.slice.call(files);
    filesArr.forEach(function (f) {

        if (!f.type.match("image.*")) {
            return;
        }
        f.indexPlace = nameIndex;
        var img = document.createElement("img");
        img.classList.add("obj");
        img.file = f;
        $(img).addClass('selFile');
        $(img).attr('data-file', f.name);
        $(img).attr('alt', f.indexPlace);
        nameIndex++;
        var A = [];
        for (var i = 0; i < dupCheck.length; i++) {
            A.push(dupCheck[i].name);
        }

        if (!A.includes(f.name) && storedFiles.length + count < 4) {
            storedFiles.push(f);
            var _checker = { name: f.name + f.indexPlace };
            dupCheck.push(_checker);
            dropbox.appendChild(img);
            var reader = new FileReader();
            reader.onload = (function (aImg) { return function (e) { aImg.src = e.target.result; }; })(img);
            reader.readAsDataURL(f);
            A = [];
            if (storedFiles.length + count > 3) {
                $("#max").show();
            }
        };

        $('#fileInput').val("");

    });
}

function removeFile(e) {
    var file = $(this).data("file");
    var fname = $(this).attr("data-file");
    var findx = $(this).attr("alt");
    for (var i = 0; i < dupCheck.length; i++) {
        if (dupCheck[i].name === fname && dupCheck[i].indexPlace === findx) {
            dupCheck.splice(i, 1);
            break;
        }
    }
    for (var i = 0; i < storedFiles.length; i++) {
        if (storedFiles[i].name === file && storedFiles[i].indexPlace == findx) {
            storedFiles.splice(i, 1);
            if (storedFiles.length + count < 4) {
                $("#max").hide();
            }
            break;
        }
    }
    $(this).remove();

}
$('#submitPhotos').click(function (e) {
    e.preventDefault();
    var formdata = new FormData();
    var pics = storedFiles;
    var fileInput = document.getElementById('fileInput');
    for (var i = 0, len = pics.length; i < len; i++) {
        formdata.append('files', pics[i]);
    }
    var other_data = $('#picForm').serializeArray();
    $.each(other_data, function (key, input) {
        formdata.append(input.name, input.value);
    });
    var tokenadr = $('form[action="/ServiceRequests/AddContractorPhotos"] input[name="__RequestVerificationToken"]').val();
    var token = tokenadr;
    var headers = {};
    var headersadr = {};
    headers['__RequestVerificationToken'] = token;
    headersadr['__RequestVerificationToken'] = tokenadr;
    $.ajax({
        type: 'POST',
        dataType: 'json',
        headers: headersadr,
        cache: false,
        url: "/ServiceRequests/AddContractorPhotos",
        processData: false,
        contentType: false,
        data: formdata,
        success: function (response, textStatus, jqXHR) {
            if (response.tooManyPics == true) {
                var titleMsg = "Too Many Photo.";
                var div = $('<div></div>');
                var outputMsg = "A service request can only contain a maximum of 4 photos.";
                div.html(outputMsg).dialog({
                    title: titleMsg,
                    height: 'auto',
                    width: 'auto',
                    maxWidth: 600,
                    fluid: true,
                    autoOpen: true,
                    resizable: true,
                    modal: true,
                    buttons: {
                        "CLOSE":
                    function () {
                        $(this).dialog('close');
                    }
                    }
                })

            }

            else {
                window.location = "/ServiceRequests/ConfirmCompletionView/" + response.id;
            }
        },

        error: function (jqXHR, textStatus, errorThrown) {
            alert('Error - ' + errorThrown);
        },

    })
})




