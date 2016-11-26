// on window resize run function
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

$.ajaxSetup({ cache: false });
$('#submitRequest').click(function (e) {
    $("#vac").hide();
    $("#valid").hide();
    $("#inactive").hide();
    $("#vacant").prop("checked", false);
    $("#validated").prop("checked", false);
    $("#Inactive").prop("checked", false);
    e.preventDefault();
    $("#divProcessing").show();
    $.ajax({
        type: "GET",
        url: "http://localhost:37234/AddressValidator/getAddValStatus",
        contentType: "application/json; charset=utf-8",
        data: { street: '' + $('#Street').val() + '', City: '' + $('#City').val() + '', state: '' + $('#State').val() + '', zip: '' + $('#Zip').val() + '' },
        dataType: "json",
        success: function (response, textStatus, jqXHR) {
            $("#divProcessing").hide();
            if (response.validated == true) {
                $("#validated").prop("checked", true);
                $("#vacant").prop("checked", response.vacant);
                var titleMsg = "Your address has been validated!"
                var div = $('<div></div>');
                var outputMsg = "Are you ready to submit your info?";
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


                            $(this).dialog('close');
                            $("#requestForm").submit();
                            if ($("#requestForm").valid()) {
                                $("#divProcessing").show();
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
                            $("#vacant").prop("checked", true);
                            $(this).dialog("close");
                            $.ajax({
                                type: "GET",
                                url: "http://localhost:37234/AddressValidator/RunStreetLevelValidation",
                                contentType: "application/json; charset=utf-8",
                                data: { street: '' + $('#Street').val() + '', City: '' + $('#City').val() + '', state: '' + $('#State').val() + '', zip: '' + $('#Zip').val() + '' },
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
                                                    $("#validated").prop("checked", true);
                                                    $(this).dialog('close');
                                                    var titleMsg = "Your address has been validated!"
                                                    var div = $('<div></div>');
                                                    var outputMsg = "Are you ready to submit your info?";
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


                                                                $(this).dialog('close');
                                                                $("#requestForm").submit();
                                                                if ($("#requestForm").valid()) {
                                                                    $("#divProcessing").show();
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
                                                                    var other_data = $('#requestForm').serializeArray();
                                                                    $.each(other_data, function (key, input) {
                                                                        formdata.append(input.name, input.value);
                                                                    });
                                                                    $.ajax({
                                                                        type: 'POST',
                                                                        dataType: 'json',
                                                                        cache: false,
                                                                        url: "http://localhost:37234/AddressValidator/ManualValidationContractorCreate",
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
                                                                                    window.location = "http://localhost:37234/Contractors/Manual_Validate_Thank_You/" + response.id;
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
                                                        var other_data = $('#requestForm').serializeArray();
                                                        $.each(other_data, function (key, input) {
                                                            formdata.append(input.name, input.value);
                                                        });
                                                        $.ajax({
                                                            type: 'POST',
                                                            dataType: 'json',
                                                            cache: false,
                                                            url: "http://localhost:37234/AddressValidator/ManualValidationContractorCreate",
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
                                                                        window.location = "http://localhost:37234/Contractors/Manual_Validate_Thank_You/" + response.id;
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
                                            var other_data = $('#requestForm').serializeArray();
                                            $.each(other_data, function (key, input) {
                                                formdata.append(input.name, input.value);
                                            });
                                            $.ajax({
                                                type: 'POST',
                                                dataType: 'json',
                                                cache: false,
                                                url: "http://localhost:37234/AddressValidator/ManualValidationContractorCreate",
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
                                                            window.location = "http://localhost:37234/Contractors/Manual_Validate_Thank_You/" + response.id;
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
});