﻿// on window resize run function
$(window).resize(function () {
    fluidDialog();
});

// catch dialog if opened within a viewport smaller than the dialog width
$(document).on("dialogopen", ".ui-dialog", function (event, ui) {
    fluidDialog();
});

function fluidDialog() {
    var $visible = $(".ui-dialog:visible");
    // each open dialog
    $visible.each(function () {
        var $this = $(this);
        var dialog = $this.find(".ui-dialog-content").data("ui-dialog");
        // if fluid option == true
        if (dialog.options.fluid) {
            var wWidth = $(window).width();
            // check window width against dialog width
            if (wWidth < (parseInt(dialog.options.maxWidth) + 50)) {
                // keep dialog from filling entire screen
                $this.css("max-width", "90%");
            } else {
                // fix maxWidth bug
                $this.css("max-width", dialog.options.maxWidth + "px");
            }
            //reposition dialog
            dialog.option("position", dialog.options.position);
        }
    });

}

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
    dropbox = document.getElementById("dropbox");
    dropbox.addEventListener("dragenter", dragenter, false);
    dropbox.addEventListener("dragover", dragover, false);
    dropbox.addEventListener("drop", drop, false);

    //dropbox.addEventListener("drop", function (e) {
    //    e.stopPropagation();
    //    e.preventDefault();
    //    //var dt = e.dataTransfer;
    //    //var files = dt.files;
    //    setTimeout(function () {
    //        var dt = e.dataTransfer;
    //        var files = dt.files;
    //        handleFiles(files);

    //    }, 1000)
    //});


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
    window.addEventListener("dragover", function (e) {
        if (e.target.id != dropbox) {
            e.preventDefault();
            e.dataTransfer.effectAllowed = "none";
            e.dataTransfer.dropEffect = "none";
        }
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
            if (!A.includes(file.name) && storedFiles.length < 4) {
                storedFiles.push(file);
                var _checker = { name: file.name };
                dupCheck.push(_checker);
                dropbox.appendChild(img);
                var reader = new FileReader();
                reader.onload = (function (aImg) { return function (e) { aImg.src = e.target.result; }; })(img);
                reader.readAsDataURL(file);
                A = [];
                if (storedFiles.length > 3) {
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

                if (!A.includes(f.name) && storedFiles.length < 4) {
                storedFiles.push(f);
                var _checker = { name: f.name + f.indexPlace };
                dupCheck.push(_checker);
                dropbox.appendChild(img);
                var reader = new FileReader();
                reader.onload = (function (aImg) { return function (e) { aImg.src = e.target.result; }; })(img);
                reader.readAsDataURL(f);
                A = [];
                if(storedFiles.length > 3){
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
                if (storedFiles.length < 4) {
                    $("#max").hide();
                }
                break;
            }
        }
        $(this).remove();

    }
    $.ajaxSetup({ cache: false });
    $('#submitRequest').click(function (e) {

        e.preventDefault();
        $("#requestForm :input").prop("readonly", true);

        var intervalId = null;

        function pendingValidationComplete() {

            var $ValidationForm = $("#requestForm");
            var c = $ValidationForm.data("validator").pendingRequest
            console.log(c);
            if ($ValidationForm.data("validator").pendingRequest === 0) {

                clearInterval(intervalId);
                $("#requestForm :input").prop("readonly", false);

                if ($ValidationForm.valid()) {

                    popups();
                }

            }

        };

        function trigger() {

            $("#CompletionDeadline").removeData("previousValue");

            $("#requestForm").valid();

            intervalId = setInterval(pendingValidationComplete, 00050);
        };

        trigger();

        function popups() {
            //if (($("#Proceed").val() == "OK") && ($("#requestForm").valid())) {
            $("#vac").hide();
            $("#valid").hide();
            $("#inactive").hide();
            $("#Address_vacant").prop("checked", false);
            $("#Address_validated").prop("checked", false);
            $("#Inactive").prop("checked", false);
            //e.preventDefault();
            $("#divProcessing").show();
            $.ajax({
                type: "GET",
                url: "/AddressValidator/getAddValStatus",
                contentType: "application/json; charset=utf-8",
                data: { street: '' + $('#Address_Street').val() + '', City: '' + $('#Address_City').val() + '', state: '' + $('#Address_State').val() + '', zip: '' + $('#Address_Zip').val() + '' },
                dataType: "json",
                success: function (response, textStatus, jqXHR) {
                    $("#divProcessing").hide();
                    if (response.validated == true) {
                        $("#Address_validated").prop("checked", true);
                        $("#Address_vacant").prop("checked", response.vacant);
                        var titleMsg = "Your address has been validated!"
                        var div = $('<div></div>');
                        var outputMsg = "Are you ready to submit your service request?";
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
                                "YES":
                                function () {
                                    if ($("#requestForm").valid()) {
                                        $(this).dialog('close');
                                        $("#divProcessing").show();
                                        $("#Inactive").prop("checked", false);
                                        var formdata = new FormData();
                                        var pics = storedFiles;
                                        var fileInput = document.getElementById('fileInput');
                                        for (var i = 0, len = pics.length; i < len; i++) {
                                            formdata.append('files', pics[i]);
                                        }
                                        var other_data = $('#requestForm').serializeArray();
                                        $.each(other_data, function (key, input) {
                                            formdata.append(input.name, input.value);
                                        });
                                        var tokenadr = $('form[action="/ServiceRequests/CreateRequest"] input[name="__RequestVerificationToken"]').val();
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
                                            url: "/ServiceRequests/CreateRequest",
                                            processData: false,
                                            contentType: false,
                                            data: formdata,
                                            success: function (response, textStatus, jqXHR) {
                                                if (response.noService == true) {
                                                    window.location = "/ServiceRequests/noService/" + response.id;
                                                }
                                                else if (response.LateDate == true) {
                                                    window.location = "/ServiceRequests/Date_Issue/";
                                                }
                                                else {
                                                    window.location = "/ServiceRequests/Details/" + response.id;
                                                }
                                            },

                                            error: function (jqXHR, textStatus, errorThrown) {
                                                alert('Error - ' + errorThrown);
                                            },

                                        })
                                    }
                                    else {
                                        $(this).dialog('close');
                                    }
                                },
                                "NO":
                                function () {
                                    $(this).dialog("close");
                                }
                            }
                        });
                    }
                    else {
                        var outputMsg = "Do any of the following apply? <br> <br> 1) This property is a vacant house. <br><br> 2) This property is a vacant lot. <br><br> 3) This property does not receive door mail delivery, i.e., PO Box service only."
                        var div = $('<div></div>');
                        var titleMsg = "A few questions while we validate your address";
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
                                "YES": function () {
                                    $("#divProcessing").show();
                                    $("#Address_vacant").prop("checked", true);
                                    $(this).dialog("close");
                                    $.ajax({
                                        type: "GET",
                                        url: "/AddressValidator/RunStreetLevelValidation",
                                        contentType: "application/json; charset=utf-8",
                                        data: { street: '' + $('#Address_Street').val() + '', City: '' + $('#Address_City').val() + '', state: '' + $('#Address_State').val() + '', zip: '' + $('#Address_Zip').val() + '' },
                                        dataType: "json",
                                        success: function (response, textStatus, jqXHR) {
                                            $("#divProcessing").hide();
                                            if (response.validated == true) {
                                                var listOfAddresses = [];
                                                for (var i = 0; i < response.results.length; i++) {
                                                    listOfAddresses.push([i + 1] + ")" + " " + response.results[i].AddressLine1 + "<br><br>")
                                                };
                                                var titleMsg = "Please confirm";
                                                var div = $('<div></div>');
                                                var outputMsg = "Is this address in the vicinity of one of the following address ranges?" + "<br><br>" + (listOfAddresses.join('\r\n'));
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
                                                        "YES":
                                                        function () {
                                                            $("#Address_validated").prop("checked", true);
                                                            $(this).dialog('close');
                                                            var titleMsg = "Your address has been validated!"
                                                            var div = $('<div></div>');
                                                            var outputMsg = "Are you ready to submit your service request?";
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
                                                                    "YES":
                                                                    function () {
                                                                        if ($("#requestForm").valid()) {
                                                                            $(this).dialog('close');
                                                                            $("#divProcessing").show();
                                                                            $("#Inactive").prop("checked", false);
                                                                            var formdata = new FormData();
                                                                            var pics = storedFiles;
                                                                            var fileInput = document.getElementById('fileInput');
                                                                            for (var i = 0, len = pics.length; i < len; i++) {
                                                                                formdata.append('files', pics[i]);
                                                                            }
                                                                            var other_data = $('#requestForm').serializeArray();
                                                                            $.each(other_data, function (key, input) {
                                                                                formdata.append(input.name, input.value);
                                                                            });
                                                                            var tokenadr = $('form[action="/ServiceRequests/CreateRequest"] input[name="__RequestVerificationToken"]').val();
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
                                                                                url: "/ServiceRequests/CreateRequest",
                                                                                processData: false,
                                                                                contentType: false,
                                                                                data: formdata,
                                                                                success: function (response, textStatus, jqXHR) {
                                                                                    if (response.noService == true) {
                                                                                        window.location = "/ServiceRequests/noService/" + response.id;
                                                                                    }
                                                                                    else if (response.LateDate == true) {
                                                                                        window.location = "/ServiceRequests/Date_Issue/";
                                                                                    }
                                                                                    else {
                                                                                        window.location = "/ServiceRequests/Details/" + response.id;
                                                                                    }
                                                                                },

                                                                                error: function (jqXHR, textStatus, errorThrown) {
                                                                                    alert('Error - ' + errorThrown);
                                                                                },

                                                                            })
                                                                        }
                                                                        else {
                                                                            $(this).dialog('close');
                                                                        }


                                                                    },
                                                                    "NO":
                                                                    function () {
                                                                        $(this).dialog("close");
                                                                    }
                                                                }
                                                            });

                                                        },
                                                        "NO":
                                                        function () {
                                                            $(this).dialog("close");
                                                            var titleMsg = "We're sorry, but we were unable to validate your address.";
                                                            var div = $('<div></div>');
                                                            var outputMsg = "Would you like to request manual address validation?";
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
                                                                    "YES":
                                                                    function () {
                                                                        if ($("#requestForm").valid()) {
                                                                            $(this).dialog('close');
                                                                            $("#Inactive").prop("checked", true);
                                                                            var formdata = new FormData();
                                                                            var pics = storedFiles;
                                                                            var fileInput = document.getElementById('fileInput');
                                                                            for (i = 0; i < pics.length; i++) {
                                                                                formdata.append('files', pics[i]);
                                                                            }
                                                                            var other_data = $('#requestForm').serializeArray();
                                                                            $.each(other_data, function (key, input) {
                                                                                formdata.append(input.name, input.value);
                                                                            });
                                                                            var tokenadr = $('form[action="/ServiceRequests/CreateRequest"] input[name="__RequestVerificationToken"]').val();
                                                                            var token = tokenadr;
                                                                            var headers = {};
                                                                            var headersadr = {};
                                                                            headers['__RequestVerificationToken'] = token;
                                                                            headersadr['__RequestVerificationToken'] = tokenadr;
                                                                            $.ajax({
                                                                                type: 'POST',
                                                                                headers: headersadr,
                                                                                dataType: 'json',
                                                                                cache: false,
                                                                                url: "/AddressValidator/ManualValidation",
                                                                                processData: false,
                                                                                contentType: false,
                                                                                data: formdata,
                                                                                success: function (response, textStatus, jqXHR) {
                                                                                    var titleMsg = "Request sent.";
                                                                                    var div = $('<div></div>');
                                                                                    var outputMsg = "Your request has been submitted. We will get back to you shortly.";
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
                                                                                            window.location = "/ServiceRequests/Manual_Validate_Thank_You/" + response.id;
                                                                                        }
                                                                                        }
                                                                                    })
                                                                                },
                                                                                error: function (jqXHR, textStatus, errorThrown) {
                                                                                    alert('Error - ' + errorThrown);
                                                                                },
                                                                            })

                                                                        }
                                                                        else {
                                                                            $(this).dialog('close');
                                                                        }
                                                                    },
                                                                    "NO":
                                                                    function () {
                                                                        $(this).dialog("close");
                                                                    }
                                                                }
                                                            });

                                                        }
                                                    }
                                                });
                                            }
                                            else {
                                                var titleMsg = "We're sorry, but we were unable to validate your address.";
                                                var div = $('<div></div>');
                                                var outputMsg = "Would you like to request manual address validation?";
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
                                                        "YES":
                                                        function () {
                                                            if ($("#requestForm").valid()) {
                                                                $(this).dialog('close');
                                                                $("#Inactive").prop("checked", true);
                                                                var formdata = new FormData();
                                                                var pics = storedFiles;
                                                                var fileInput = document.getElementById('fileInput');
                                                                for (i = 0; i < pics.length; i++) {
                                                                    formdata.append('files', pics[i]);
                                                                }
                                                                var other_data = $('#requestForm').serializeArray();
                                                                $.each(other_data, function (key, input) {
                                                                    formdata.append(input.name, input.value);
                                                                });
                                                                var tokenadr = $('form[action="/ServiceRequests/CreateRequest"] input[name="__RequestVerificationToken"]').val();
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
                                                                    url: "/AddressValidator/ManualValidation",
                                                                    processData: false,
                                                                    contentType: false,
                                                                    data: formdata,
                                                                    success: function (response, textStatus, jqXHR) {
                                                                        var titleMsg = "Request sent.";
                                                                        var div = $('<div></div>');
                                                                        var outputMsg = "Your request has been submitted. We will get back to you shortly.";
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
                                                                                window.location = "/ServiceRequests/Manual_Validate_Thank_You/" + response.id;
                                                                            }
                                                                            }
                                                                        })
                                                                    },
                                                                    error: function (jqXHR, textStatus, errorThrown) {
                                                                        alert('Error - ' + errorThrown);
                                                                    },
                                                                })

                                                            }
                                                            else {
                                                                $(this).dialog('close');
                                                            }
                                                        },
                                                        "NO":
                                                        function () {
                                                            $(this).dialog("close");
                                                        }
                                                    }
                                                });

                                            }
                                        },
                                        failure: function (jqXHR, textStatus, errorThrown) {
                                            alert('Error - ' + errorThrown);
                                        }
                                    });
                                },
                                "NO": function () {
                                    $(this).dialog("close");
                                    var titleMsg = "We're sorry, but we were unable to validate your address.";
                                    var div = $('<div></div>');
                                    var listOfErrors = [];
                                    for (var i = 0; i < response.errors.length; i++) {
                                        listOfErrors.push([i + 1] + ")" + " " + response.errors[i].message + "<br><br>")

                                    }
                                    var outputMsg = "Address validation failed for the following reason(s):" + "<br><br>" + (listOfErrors.join('\r\n')) + "<br><br>" + "Would you like to request manual validation?";
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
                                            "YES":
                                            function () {
                                                if ($("#requestForm").valid()) {
                                                    $(this).dialog('close');
                                                    $("#Inactive").prop("checked", true);
                                                    var formdata = new FormData();
                                                    var pics = storedFiles;
                                                    var fileInput = document.getElementById('fileInput');
                                                    for (var i = 0, len = pics.length; i < len; i++) {
                                                        formdata.append('files', pics[i]);
                                                    }
                                                    var other_data = $('#requestForm').serializeArray();
                                                    $.each(other_data, function (key, input) {
                                                        formdata.append(input.name, input.value);
                                                    });
                                                    var tokenadr = $('form[action="/ServiceRequests/CreateRequest"] input[name="__RequestVerificationToken"]').val();
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
                                                        url: "/AddressValidator/ManualValidation",
                                                        processData: false,
                                                        contentType: false,
                                                        data: formdata,
                                                        success: function (response, textStatus, jqXHR) {
                                                            var titleMsg = "Request sent.";
                                                            var div = $('<div></div>');
                                                            var outputMsg = "Your request has been submitted. We will get back to you shortly.";
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
                                                                    window.location = "/ServiceRequests/Manual_Validate_Thank_You/" + response.id;
                                                                }
                                                                }
                                                            })
                                                        },
                                                        error: function (jqXHR, textStatus, errorThrown) {
                                                            alert('Error - ' + errorThrown);
                                                        },
                                                    })

                                                }
                                                else {
                                                    $(this).dialog('close');
                                                }
                                            },
                                            "NO":
                                            function () {
                                                $(this).dialog("close");
                                            }
                                        }
                                    });
                                },


                            }

                        });

                    }
                },
                failure: function (jqXHR, textStatus, errorThrown) {
                    alert('Error - ' + errorThrown);
                }
            });
        }
       
    });