function ValidateAddress() {
    $("#vac").hide();
    $("#valid").hide();
    $("#inactive").hide();
    $("#Address_vacant").prop("checked", false);
    $("#Address_validated").prop("checked", false);
    $("#Inactive").prop("checked", false);
    event.preventDefault();
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
                    height: 300,
                    width: 600,
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
                    height: 300,
                    width: 600,
                    resizable: true,
                    modal: true,
                    buttons: {
                        "YES": function () {
                            $("#divProcessing").show();
                            $("#Address_vacant").prop("checked", true);
                            $(this).dialog("close");
                            $.ajax({
                                type: "GET",
                                url: "http://localhost:37234/AddressValidator/RunStreetLevelValidation",
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
                                            height: 300,
                                            width: 600,
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
                                                        height: 300,
                                                        width: 600,
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
                                                        height: 300,
                                                        width: 600,
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
                                                                    var fileInput = document.getElementById('fileInput');
                                                                    for (i = 0; i < fileInput.files.length; i++) {
                                                                        formdata.append(fileInput.files[i].name, fileInput.files[i]);
                                                                    }
                                                                    var other_data = $('form').serializeArray();
                                                                    $.each(other_data, function (key, input) {
                                                                        formdata.append(input.name, input.value);
                                                                    });
                                                                    $.ajax({
                                                                        type: 'POST',
                                                                        dataType: 'json',
                                                                        cache: false,
                                                                        url: "http://localhost:37234/AddressValidator/ManualValidation",
                                                                        processData: false,
                                                                        contentType: false,
                                                                        data: formdata,
                                                                        success: function (response, textStatus, jqXHR) {
                                                                            var titleMsg = "Request sent.";
                                                                            var div = $('<div></div>');
                                                                            var outputMsg = "Your request has been submitted. We will get back to you shortly.";
                                                                            div.html(outputMsg).dialog({
                                                                                title: titleMsg,
                                                                                height: 300,
                                                                                width: 600,
                                                                                autoOpen: true,
                                                                                resizable: true,
                                                                                modal: true,
                                                                                buttons: {
                                                                                    "CLOSE":
                                                                                function () {
                                                                                    $(this).dialog('close');
                                                                                    window.location = "http://localhost:37234/ServiceRequests/Manual_Validate_Thank_You/" + response.id;
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
                                            height: 300,
                                            width: 600,
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
                                                        var fileInput = document.getElementById('fileInput');
                                                        for (i = 0; i < fileInput.files.length; i++) {
                                                            formdata.append(fileInput.files[i].name, fileInput.files[i]);
                                                        }
                                                        var other_data = $('form').serializeArray();
                                                        $.each(other_data, function (key, input) {
                                                            formdata.append(input.name, input.value);
                                                        });
                                                        $.ajax({
                                                            type: 'POST',
                                                            dataType: 'json',
                                                            cache: false,
                                                            url: "http://localhost:37234/AddressValidator/ManualValidation",
                                                            processData: false,
                                                            contentType: false,
                                                            data: formdata,
                                                            success: function (response, textStatus, jqXHR) {
                                                                var titleMsg = "Request sent.";
                                                                var div = $('<div></div>');
                                                                var outputMsg = "Your request has been submitted. We will get back to you shortly.";
                                                                div.html(outputMsg).dialog({
                                                                    title: titleMsg,
                                                                    height: 300,
                                                                    width: 600,
                                                                    autoOpen: true,
                                                                    resizable: true,
                                                                    modal: true,
                                                                    buttons: {
                                                                        "CLOSE":
                                                                    function () {
                                                                        $(this).dialog('close');
                                                                        window.location = "http://localhost:37234/ServiceRequests/Manual_Validate_Thank_You/" + response.id;
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
                                height: 300,
                                width: 600,
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
                                            var fileInput = document.getElementById('fileInput');
                                            for (i = 0; i < fileInput.files.length; i++) {
                                                formdata.append(fileInput.files[i].name, fileInput.files[i]);
                                            }
                                            var other_data = $('form').serializeArray();
                                            $.each(other_data, function (key, input) {
                                                formdata.append(input.name, input.value);
                                            });
                                            $.ajax({
                                                type: 'POST',
                                                dataType: 'json',
                                                cache: false,
                                                url: "http://localhost:37234/AddressValidator/ManualValidation",
                                                processData: false,
                                                contentType: false,
                                                data: formdata,
                                                success: function (response, textStatus, jqXHR) {
                                                    var titleMsg = "Request sent.";
                                                    var div = $('<div></div>');
                                                    var outputMsg = "Your request has been submitted. We will get back to you shortly.";
                                                    div.html(outputMsg).dialog({
                                                        title: titleMsg,
                                                        height: 300,
                                                        width: 600,
                                                        autoOpen: true,
                                                        resizable: true,
                                                        modal: true,
                                                        buttons: {
                                                            "CLOSE":
                                                        function () {
                                                            $(this).dialog('close');
                                                            window.location = "http://localhost:37234/ServiceRequests/Manual_Validate_Thank_You/" + response.id;
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