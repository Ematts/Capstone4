var service_pics = [];
var contractor_pics = [];
$('.img').each(function (index, value) {
    var _pair = { source: $(this).attr('src'), title: $(this).attr('alt') };
    if ($(this).attr('alt') == "Service Request Photo") {
        service_pics.push(_pair);
        _pair.position = service_pics.indexOf(_pair);
    };
    if ($(this).attr('alt') == "Contractor Photo") {
        contractor_pics.push(_pair);
        _pair.position = contractor_pics.indexOf(_pair);
    };

});
$('.img').each(function (index, value) {
    if ($(this).attr('alt') == "Service Request Photo") {
        for (i = 0; i < service_pics.length; i++) {
            if (service_pics[i].source == $(this).attr('src')) {

                $(this).attr("alt", "Service Request Photo" + " " + (service_pics[i].position + 1) + "/" + service_pics.length);

            }

        };
    }
    if ($(this).attr('alt') == "Contractor Photo") {
        for (i = 0; i < contractor_pics.length; i++) {
            if (contractor_pics[i].source == $(this).attr('src')) {

                $(this).attr("alt", "Contractor Photo" + " " + (contractor_pics[i].position + 1) + "/" + contractor_pics.length);

            }

        };
    }
});