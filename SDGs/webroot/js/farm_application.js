let uploadRequest = '/farms/upload-file'
let answerData = []
let appEnv = $('input[name="env"]').val()
let officeId = $('input[name="office_id"]').val()
let arrKey = $('input[name="key"]').val()
let localStorageItemName = `${appEnv}_${officeId}_farm_questions`

/**
 * Put data from Form to LocalStorage
 **/
function putFormDataToObj(formData) {
    let answerLocalStorage = JSON.parse(localStorage.getItem(localStorageItemName))

    if  (!answerLocalStorage) {
        answerLocalStorage = {}
    }

    if (answerLocalStorage && answerLocalStorage[`${arrKey}`] && answerLocalStorage[`${arrKey}`].length > 0) {
        delete answerLocalStorage[`${arrKey}`]
    }

    for(var key in formData) {
        var obj = {}
        var formInput = $(`input[name="${formData[key].name}"]`)
        var imageObj = {}

        if (formData[key].value && formData[key].value.replace(/\s/g, '').length > 0) {
            if ($(formInput).attr('data-image-name')) {
                Object.assign(imageObj, {
                    name : $(formInput).data("imageName"),
                    path : $(formInput).data("imageUrl")
                })
            }
            obj = {
                question     : formData[key].name,
                answer       : $.trim(formData[key].value),
                sub_answer   : $(formInput).data("subAnswer"),
                image        : !$.isEmptyObject(imageObj) ? imageObj : $(formInput).data("imageUrl")
            }

            answerData.push(obj)
        }
    }

    if (answerData && answerData.length > 0) {
        answerLocalStorage[`${arrKey}`] = answerData
    }

    localStorage.setItem(localStorageItemName, JSON.stringify(answerLocalStorage));
    answerData = []
};

/**
 * Move to next question
 **/
function nextQuestion($event) {
    var formData = $('#form1').serializeArray()

    putFormDataToObj(formData)
};

/**
 * Handle submit data questions
 **/
function handleSubmit() {
    $.LoadingOverlay("show");
    var form = new FormData()
    form.append("answer_questions", localStorage.getItem(localStorageItemName))

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrfToken"]').attr('content')
        }
    });

    $.ajax({
        url: '/farms/answer/apply',
        method: 'POST',
        data: form,
        cache: false,
        processData: false,
        contentType: false,
    })
        .done(function (res) {
            $.LoadingOverlay("hide");
            showSuccessMessage(res.message)
            localStorage.removeItem(localStorageItemName);

            window.location.replace('/farms/application/complete')
        })
        .fail(function (error) {
            $.LoadingOverlay("hide");
            console.log(error)
            var message = ''
            if (error && error.responseJSON) {
                message = error.responseJSON.message
            }
            showErrorMessage(message)
        });
};

function loadAnswerQuestions(callbackFunc) {
    $.ajax({
        url: '/farms/answer/show',
        method: 'GET',
        contentType: 'application/json',
        cache: false,
        processData: false,
        success: function (res) {
            callbackFunc(res.data)
        },
        error: function (error) {
            var message = ''
            if (error && error.responseJSON) {
                message = error.responseJSON.message
            }
            showErrorMessage(message)
        }
    })
}

$(document).ready(function() {
    let items = JSON.parse(localStorage.getItem(localStorageItemName))

    loadAnswerQuestions(function (data) {
        var queryParam = $.urlParam("mode")
        if (queryParam !== 'process') {
            if (!items && data) {
                localStorage.setItem(localStorageItemName, JSON.stringify(data));
            }
            items = data
        }

        if ((queryParam === 'process' && Object.keys(items).length === 0)) {
            items = data
            localStorage.setItem(localStorageItemName, JSON.stringify(data));
        }

        /**
         * Load data from LocalStorage to DOM
         **/
        var question_key = $('input[name="key"]').val()

        if (items && items[`${question_key}`]) {
            items[`${question_key}`].forEach((item) => {
                var input = $(`input[name="${item.question}"`)

                /**
                 * Fill data from Localstorage to input
                 **/
                switch (input.attr('type')) {
                    case 'text':
                        $(input).val(item.answer)
                        break;

                    case 'radio':
                        $(input).each(function(i, e) {
                            if (e.value === item.answer) {
                                e.checked = true
                                var name = $(e).attr('name');
                                $(e).closest(`button.${name}`).addClass('active');
                                $(`input[data-sub-question=${item.question}`).attr('disabled', false)
                            }
                        });
                        break;

                    case 'checkbox':
                        $(input).each(function(i, e) {
                            if (e.value === item.answer) {
                                e.checked = true
                            }
                        });
                        break;

                }

                /**
                 * Add class active to label file icon
                 **/
                if (item.image && (($.isPlainObject(item.image) && !$.isEmptyObject(item.image)) || typeof item.image == 'string')) {
                    var isObjectImage = $.isPlainObject(item.image) && !$.isEmptyObject(item.image)
                    input.attr({
                        'data-image-url': isObjectImage ? item.image.path : item.image,
                        'data-image-name': isObjectImage ? item.image.name : item.image.replace("uploads/producers/", "") ?? ''
                    })
                    $(`input[id="${item.question}"]`).parent().addClass('active')
                    $(`label[for="${item.question}"]`).text(`${isObjectImage ? item.image.name : item.image.replace("uploads/producers/", "")}`)
                }

                /**
                 * Fill data sub answer input
                 **/
                if (item.sub_answer) {
                    $(input).attr('data-sub-answer', item.sub_answer)
                    $(`input[data-sub-question=${item.question}`).val(item.sub_answer)
                }
            })
        }

        /**
         * Confirmation page
         **/
        if (items && Object.keys(items).length > 0) {
            $('#btn_apply').removeClass('btn-disabled')
            for (var i in items) {
                if (items[i] && items[i].length > 0) {
                    var disp = $(`.confirmation_page .wrap_disp .disp_box[data-question=${i}]`).find('.disp_answer p')
                    disp.removeClass('text_red')
                    disp.text('入力済み')
                }
            }
        }
    });

    /**
     * Input files
     **/
    $('input[type="file"]').change(function (e) {
        if (e.target.className === 'answer_menu_file') {
            compressAndUploadFile(e.target, uploadRequest)
            let id = $(this).attr('id')
            $(`label[for="${id}"]`).text(e.target.files[0].name)
        }
    });

    /**
     * Submit answers
     **/
    $('#btn_apply').click(function (e) {
        e.preventDefault();
        if (items && Object.keys(items).length > 0) {
            handleSubmit();
        }

        return false;
    })

    /**
     * View answer question image
     **/
    $('.file_name a').click(function (e) {
        initImageViewer('answer-img')
    })

    $('.next_question').bind('click', nextQuestion);
    $('.prev_question').bind('click', nextQuestion);
    $('.btn_to_confirm').bind('click', nextQuestion);
})
