const TYPE_MULTI_MENU = 'multi_menu'
const TYPE_CHECKBOX = 'checkbox'
const TYPE_RADIO = 'radio'

/**
 * Convert Blob to File
 **/
function blobToFile(theBlob){
    return new File([theBlob], theBlob.name, { lastModified: new Date().getTime(), type: theBlob.type })
}

function initImageViewer (elementId, isGallery = false) {
    var toolBar = {}

    switch (isGallery) {
        case true:
            toolBar = {
                zoomIn: 4,
                zoomOut: 4,
                oneToOne: 4,
                reset: 4,
                prev: 4,
                play: {
                    show: 4,
                    size: 'large',
                },
                next: 4,
                rotateLeft: 4,
                rotateRight: 4,
                flipHorizontal: 4,
                flipVertical: 4
            }
            break;

        case false:
        default:
            toolBar = {
                zoomIn: 4,
                zoomOut: 4,
                oneToOne: 4,
                reset: 4,
                rotateLeft: 4,
                rotateRight: 4,
                flipHorizontal: 4,
                flipVertical: 4
            }
            break;
    }


    const viewer = new Viewer(document.getElementById(`${elementId}`), {
        viewed() {
            viewer.zoomTo(0.6);
        },
        hide() {
            viewer.destroy()
        },
        toolbar: toolBar,
    });

    viewer.show();
}

/**
 * Compress size image and upload to server
 **/
async function compressAndUploadFile(target, requestUrl) {
    if (target.files && target.files[0]) {
        var imageFile = target.files[0];

        var options = {
            maxSizeMB: 0.5,
            maxWidthOrHeight: 1920,
            useWebWorker: true
        }

        try {
            $.LoadingOverlay("show");
            const compressedFile = await imageCompression(imageFile, options);
            var formData = new FormData();
            formData.append('image', blobToFile(compressedFile));

            $.ajaxSetup({
                headers: {
                    'X-CSRF-TOKEN': $('meta[name="csrfToken"]').attr('content')
                }
            });

            $.ajax({
                url: requestUrl,
                method: 'POST',
                data: formData,
                cache: false,
                processData: false,
                contentType: false,
            })
            .done(function (res) {
                /**
                 * Show image after upload at Restaurant questions
                 * */
                $(target).parent().siblings().find('img').attr('src', `/${res.data.path}`)
                $(`input[data-image-id="${target.id}"]`).val(res.data.path)
                $(`input[data-image-id="${target.id}"]`).attr('data-image-name', `${res.data.file_original_name}` + `.${res.data.file_original_extension}`)
                $(target).parent().find('span.file_name').text(`(${res.data.file_original_name}` + `.${res.data.file_original_extension})`)


                /**
                 * Show image after upload at Farm questions
                 * */
                $(`input[name="${target.id}"]`).attr({
                    'data-image-url': res.data.path,
                    'data-image-name': `${res.data.file_original_name}` + `.${res.data.file_original_extension}`
                })

                $(target).parent().addClass('active')
                $.LoadingOverlay("hide");
                showSuccessMessage(res.message)
            })
            .fail(function (error) {
                showErrorMessage(error.message)
                $.LoadingOverlay("hide");
            });

        } catch (error) {
            showErrorMessage(error.message)
            $.LoadingOverlay("hide");
        }
    }
}

/**
 * Send mail function
 **/
function sendMail(requestUrl, data) {
    $.LoadingOverlay("show");

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrfToken"]').attr('content')
        }
    });

    $.ajax({
        url: requestUrl,
        method: 'POST',
        data: data,
        cache: false,
        processData: false,
        contentType: false,
    })
        .done(function (res) {
            $.LoadingOverlay("hide");
            showSuccessMessage(res.message)
        })
        .fail(function (error) {
            $.LoadingOverlay("hide");

            var message = ''
            if (error && error.responseJSON) {
                message = error.responseJSON.message
            }
            showErrorMessage(message)
        });
}

/**
 * Show error message toast
 **/
function showErrorMessage(error) {
    let errorMessage = error || 'エラーが発生しました。';

    $.toast({
        text : errorMessage,
        showHideTransition : 'slide',
        bgColor : '#DA678C',
        textColor : '#fff',
        allowToastClose : true,
        hideAfter : 5000,
        stack : 5,
        textAlign : 'left',
        position : 'top-right'
    });
}

/**
 * Show success message toast
 **/
function showSuccessMessage(message) {
    $.toast({
        text : message,
        showHideTransition : 'slide',
        bgColor : '#00D100',
        textColor : '#fff',
        allowToastClose : true,
        hideAfter : 5000,
        stack : 5,
        textAlign : 'left',
        position : 'top-right'
    });
}

function toASCII(chars) {
    var ascii = '';
    for (var i = 0, l = chars.length; i < l; i++) {
      var c = chars[i].charCodeAt(0);

      // make sure we only convert half-full width char
      if (c >= 0xFF00 && c <= 0xFFEF) {
        c = 0xFF & (c + 0x20);
      }

      ascii += String.fromCharCode(c);
    }
    return ascii;
}

function scrollToElement(elementId) {
    const element =  document.getElementById(elementId)
    element.scrollIntoView({ behavior: "smooth", block: "start"})
}

$(function () {
    $.urlParam = function (name) {
        var results = new RegExp('[\?&]' + name + '=([^&#]*)')
            .exec(window.location.search);
        return (results !== null) ? results[1] || 0 : false;
    }

    $('.nav_toggle, .nav_menu_li a').on('click', function () {
        $('.nav_toggle, .nav').toggleClass('show');
    });

    $("#btn_01").click(function(){
        $("#btn_01").removeClass("active");
        $("#btn_02").removeClass("active");
        $("#btn_03").removeClass("active");
        $("#btn_04").removeClass("active");
        $("#btn_05").removeClass("active");
        $("#btn_01").addClass("active");
    });
    $("#btn_02").click(function(){
        $("#btn_01").removeClass("active");
        $("#btn_02").removeClass("active");
        $("#btn_03").removeClass("active");
        $("#btn_04").removeClass("active");
        $("#btn_05").removeClass("active");
        $("#btn_02").addClass("active");
    });
    $("#btn_03").click(function(){
        $("#btn_01").removeClass("active");
        $("#btn_02").removeClass("active");
        $("#btn_03").removeClass("active");
        $("#btn_04").removeClass("active");
        $("#btn_05").removeClass("active");
        $("#btn_03").addClass("active");
    });
    $("#btn_04").click(function(){
        $("#btn_01").removeClass("active");
        $("#btn_02").removeClass("active");
        $("#btn_03").removeClass("active");
        $("#btn_04").removeClass("active");
        $("#btn_05").removeClass("active");
        $("#btn_04").addClass("active");
    });
    $("#btn_05").click(function(){
        $("#btn_01").removeClass("active");
        $("#btn_02").removeClass("active");
        $("#btn_03").removeClass("active");
        $("#btn_04").removeClass("active");
        $("#btn_05").removeClass("active");
        $("#btn_05").addClass("active");
    });
    $('#ragistration-img-btn-1').change(function() {
        var fileName = $('#ragistration-img-btn-1')[0].files[0].name;
        if (fileName) {
            $(this).parent().prev().find('label').text('店舗自己紹介画像 1（' + fileName + '）');
        }
    });
    $('#ragistration-img-btn-2').change(function() {
        var fileName = $('#ragistration-img-btn-2')[0].files[0].name;
        if (fileName) {
            $(this).parent().prev().find('label').text('店舗自己紹介画像 2（' + fileName + '）');
        }
    });
    $('.input-number').on('input change', function (evt) {
        $(this).val(toASCII($(this).val()).replace(/[^0-9０-９]/gi, ''));
    });
    $('input[type="text"]').on('change input', function(){
        this.value = $.trim(this.value);
    });
    $('.login_btn').click(function () {
        localStorage.clear();
    })
    /**
     * View profile images
     **/
    $('.pic_block img').click(function (e) {
        initImageViewer('pic_block_images', true)
    })
    $('.pic_block_single img').click(function (e) {
        initImageViewer('pic_block_images')
    })
})
