if (!String.prototype.includes) {
    String.prototype.includes = function (search, start) {
        'use strict';
        if (typeof start !== 'number') {
            start = 0;
        }

        if (start + search.length > this.length) {
            return false;
        } else {
            return this.indexOf(search, start) !== -1;
        }
    };
}

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

(function ($) {
    $.fn.imageBox = function (options) {
        var options = $.extend({
            objClicked: '.img',      // 点击的元素
            rotateDirection: 'right' // 图片旋转方向， 默认是right => 顺时针
        }, options);
        var obj = this, objClicked = options.objClicked, fileName = options.fileName, list_images = [], list_pairs = [];
        //var checker = [];

        initHtml(obj);
        initCss(obj);

        $(objClicked).on('click', function () {

            //var _url = $(this).data("url"), current = 0;
            var _url = $(this).attr("src"), current = 0;
            var _alt = $(this).attr("alt"), current = 0;
            // 清空数组 list_images
            if (list_images.length > 0) {
                list_images.length = 0;
            }
            if (list_pairs.length > 0) {
                list_pairs.length = 0;
            }

            $(objClicked).each(function (index, element) {
                var $img = $(element), _src = $img.attr("src"), _alt = $img.attr("alt");
                //var source = _src;
                //var title = _alt;
                var _pair = { source: _src, title: _alt };
                if (_url == _src) {
                    current = index + 1;
                }
                list_images.push(_src);
                list_pairs.push(_pair);
            });
            if (typeof (fileName) == 'undefined') {
                $('.modal-title').text(_alt);
            } else {
                $('.modal-title').text($(fileName).text());
            }
            $('#img-preview').html('<img src="' + _url + '" width="500px" height="350px" class="image-box" style="cursor: move;"></img>')
            $('#img-preview').attr({ 'current': current });
            $(obj).find('#unbind-pos').modal('show');
            if (_alt.includes("Service")) {
                $("#delete").hide();
            }
            if (_alt.includes("Contractor")) {
                $("#delete").show();
            }
        });

        btnCtrlImgEvent(options, list_images, list_pairs);
    };

    var rotateDeg = 0;
    /**
    *初始化html
    */
    function initHtml(obj) {
        var div = $('<div id="unbind-pos" class="modal fade" style="display:none;" aria-hidden="true"></div>');
        div.append('<div class="modal-dialog">' +
                      '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button aria-hidden="true" data-dismiss="modal" class="close" type="button"><span>&times;</span></button>' +
                                '<h4 class="modal-title"></h4>' +
                            '</div>' +
                            '<div style="min-height: 350px;max-height: 500px;" class="modal-body">' +
                                '<div id="img-preview"></div>' +
                                '<div class="img-op">' +
                                    '<span class="btn btn-primary zoom-in">Zoom In</span>' +
                                    '<span class="btn btn-primary zoom-out">Zoom Out</span>' +
                                    '<span class="btn btn-primary rotate">Rotate</span>' +
                                    '<br>' +
                                    '<span role="prev" class="btn btn-primary switch">Prev</span>' +
                                    '<span role="next" class="btn btn-primary switch">Next</span>' +
                                    '</br>' +
                                    '<span role="delete" id="delete" class="btn btn-primary deletePic">Delete</span>' +
                                '</div>' +
                            '</div>' +
                            '<div class="modal-footer">' +
                                '<button data-dismiss="modal" class="btn btn-default" type="button">Close</button>' +
                            '</div>' +
                      '</div>' +
                    '</div>');
        $(obj).append(div);
    };

    /**
    * 初始化样式
    */
    function initCss(obj) {
        $(obj).find('#img-preview').css({
            'height': '350px',
            'width': 'auto',
            'overflow': 'auto',
            'text-align': 'center'
        });
        $(obj).find('.img-op').css({
            'margin-top': '5px',
            'text-align': 'center'
        });
        $(obj).find('.modal .modal-content .btn').css('border-radius', '0');
        $(obj).find('.img-op .btn').css({
            'margin': '5px',
            'width': '100px',
        });
        $(obj).find('.modal-footer .btn-default').css({
            'background-color': '#fff',
            'background-image': 'none',
            'border': '1px solid #ccc',
        });
    };

    /**
    * 按钮控制图片事件
    */
    function btnCtrlImgEvent(options, list_images, list_pairs) {

        zoomIn();
        zoomOut();
        dragImage();
        deletePic(list_images, list_pairs);
        rotateImage(options);
        switchImage(list_images, list_pairs);

    };

    function deletePic(list_images, list_pairs) {

        $('.deletePic').click(function (e) {
            e.preventDefault();
            var $modal = $('#unbind-pos');
            var formdata = new FormData();
            var fileInput = document.getElementById('fileInput');
            var picToDelete = ($modal.find('#img-preview img').attr('src'));
            for (i = 0; i < fileInput.files.length; i++) {
                formdata.append(fileInput.files[i].name, fileInput.files[i]);
            }
            var other_data = $('#picForm').serializeArray();
            $.each(other_data, function (key, input) {
                formdata.append(input.name, input.value);
            });
            formdata.append("picName", picToDelete);
            var outputMsg = "Do you really want to delete this file?";
            var div = $('<div></div>');
            var titleMsg = "Confirm deletion";
            div.html(outputMsg).dialog({
                title: titleMsg,
                height: 'auto',
                width: 'auto',
                maxWidth: 600,
                fluid: true,
                autoOpen: true,
                open: function (event, ui) {
                    $('.ui-dialog').css('z-index',100000);
                    $('.ui-widget-overlay').css('z-index', 99999);
                },
                resizable: true,
                modal: true,
                buttons: {
                    "YES": function () {
                        $.ajax({
                            url: "/ServiceRequests/DeleteContractorPic",
                            type: 'POST',
                            processData: false,
                            contentType: false,
                            data: formdata,
                        }).done(function (data) {
                            if (data.Result == "OK") {
                                location.reload();
                            }
                            else if (data.Result.Message) {
                                alert(data.Result.Message);
                            }
                        }).fail(function () {
                            alert("There is something wrong. Please try again.");
                        })

                    },
                    "NO":
                        function () {
                            $(this).dialog("close");
                        }
                }   
            })
        });

    };

    //图片放大
    function zoomIn() {
        $('.zoom-in').click(function () {
            var imageHeight = $('#img-preview img').height();
            var imageWidth = $('#img-preview img').width();
            $('#img-preview img').css({
                height: '+=' + imageHeight * 0.1,
                width: '+=' + imageWidth * 0.1
            });
        });
    };

    //图片缩小
    function zoomOut() {
        $('.zoom-out').click(function () {
            var imageHeight = $('#img-preview img').height();
            var imageWidth = $('#img-preview img').width();
            $('#img-preview img').css({
                height: '-=' + imageHeight * 0.1,
                width: '-=' + imageWidth * 0.1
            });
        });
    };

    // 图片预览框中拖拽
    function dragImage() {
        $('#img-preview').on('mousedown', 'img', function (event) {
            var mousePos = { x: event.clientX, y: event.clientY };
            var _this = this;

            var scrollLeft = $(_this).parent().scrollLeft();
            var scrollTop = $(_this).parent().scrollTop();

            $(document).on('mousemove', function (event) {
                var offsetX = event.clientX - mousePos.x;
                var offsetY = event.clientY - mousePos.y;

                $(_this).parent().scrollLeft(scrollLeft - offsetX);
                $(_this).parent().scrollTop(scrollTop - offsetY);
            });

            $(document).on('mouseup', function () {
                $(document).off("mousemove");
            });
            return false;
        });
    };

    //图片旋转，默认方向是右旋转
    function rotateImage(options) {
        $('.rotate').click(function () {
            if (options.rotateDirection == 'right') {
                rotateDeg += 90;
                if (rotateDeg == 360) {
                    rotateDeg = 0;
                }
            }
            if (options.rotateDirection == 'left') {
                rotateDeg -= 90;
                if (rotateDeg == -360) {
                    rotateDeg = 0;
                }
            }
            $('#img-preview img').css({
                'transform': 'rotate(' + rotateDeg + 'deg)',
                '-webkit-transform': 'rotate(' + rotateDeg + 'deg)',
                '-moz-transform': 'rotate(' + rotateDeg + 'deg)',
                '-o-transform': 'rotate(' + rotateDeg + 'deg)',
                '-ms-transform': 'rotate(' + rotateDeg + 'deg)'
            });
        });
    };

    //图片切换
    function switchImage(list_images, list_pairs) {
        var $modal = $('#unbind-pos');
        $('#unbind-pos').on('click', '.switch', function () {
            var _list_images = list_images, _self = this, _role = $(_self).attr('role');
            var _list_pairs = list_pairs, _self = this, _role = $(_self).attr('role');
            var $image_container = $modal.find('#img-preview');
            var _current = parseInt($image_container.attr('current'));
            var _image_count = _list_images.length;
            var _index = _new_current = 0;
            switch (_role) {
                case 'prev':
                    if (_current - 1 > 0) {
                        _new_current = _current - 1;
                    } else {
                        _new_current = _image_count;
                    }
                    break;
                case 'next':
                    if (_current + 1 <= _image_count) {
                        _new_current = _current + 1;
                    } else {
                        _new_current = 1;
                    }
            }
            _index = _new_current - 1;
            $modal.find('#img-preview').attr({ 'current': _new_current });
            $modal.find('#img-preview img').attr({ 'src': _list_images[_index] });
            var x = $modal.find('#img-preview img').attr('src');
            var y = $modal.find('#img-preview img').attr('alt');
            for (i = 0; i < _list_pairs.length; i++) {
                if (_list_pairs[i].source == x) {

                    $('.modal-title').text(_list_pairs[i].title);
                    if(_list_pairs[i].title.includes("Service")){
                        $("#delete").hide();
                    }
                    if (_list_pairs[i].title.includes("Contractor")) {
                        $("#delete").show();
                    }

                }
            }
        });
    };

})(jQuery);