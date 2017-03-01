$(document).ready(function () {
    var counter = 0;
    $.ajaxSetup({ cache: false });
    $("#divProcessing").hide();
    $("#pending").hide();
    if($('PayPalListenerModel_ID').val("")){
        $("#divProcessing").show();
        $("#details").hide();
        search();
    }
    function search() {
        counter++;
        $.ajax({
            type: "GET",
            url: "/ServiceRequests/getPayPalInfo",
            contentType: "application/json; charset=utf-8",
            data: { ID: '' + $('#ID').val() + ''},
            dataType: "json",
            success: function (response, textStatus, jqXHR) {
                if(response.found == true){
                    $("#divProcessing").hide();
                    //$("#paystatus").val(response.status);
                    document.getElementById("payStatus").innerText = response.status;
                    document.getElementById("payID").innerText = response.tx_id;
                    document.getElementById("payDate").innerText = response.date;
                    document.getElementById("payZone").innerText = response.tZone;
                    $("#details").show();
                }
                if(response.found == false){
                    console.log(counter);
                    if (counter < 5) {
                        setTimeout(search, 5000);
                    }
                    if (counter >= 5) {
                        $("#divProcessing").hide();
                        $("#pending").show();
                    }

                }
            },
            error: function (xhr, textStatus, errorThrown) {
                alert('Error - ' + errorThrown);
            }
        })
    };
})

    
