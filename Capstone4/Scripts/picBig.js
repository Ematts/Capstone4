     $(function () {
        var showModal = function () {
            var $input = $(this);
            var imgAlt = $input.attr("alt");
            $("#theModal h4.modal-title").html(imgAlt);
            var img = this;
            var imageHeight = $input.height();
            var imagWidth = $input.width();
            var NewimgWidth = imagWidth * 4;
            var NewImgHeight = imageHeight * 4;
            var picSrc = $input.attr("src");
            $("#theModal img").attr('src', picSrc);
            //set new image width
            $("div.modal-dialog").css("width", NewimgWidth);
            $("#theModal img").width(NewimgWidth);
            //set new image height
            $("#theModal img").height(NewImgHeight);
            $("#theModal").modal("show");
        };
        var MyHtml = '<div id="theModal" class="modal fade">' +
                ' <div class="modal-dialog ">' +
                    '<div class="modal-content">' +
                        ' <div class="modal-header">' +
                            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                            '<h4 class="modal-title">Hello!</h4>' +
                         '</div>' +
                        '<div class="modal-body">' +
                           '  <img not-to-enlarge="true" class="img-responsive" + src=""alt="...">' +
                        '</div>' +
                         '<div class="modal-footer">' +
                             '<button type="button" class="btn btn-default" data-dismiss="modal">' +
                                'Close' +
                             '</button>' +
                         '</div>' +
                     '</div>' +
                 '</div>' +
             '</div>';
        $("div.body-content").append(MyHtml);
        $("img[not-to-enlarge!=true]").click(showModal);
        $("img[not-to-enlarge!=true]").css("cursor", "pointer");
    });
