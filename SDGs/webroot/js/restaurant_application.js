let uploadRequest = '/restaurants/upload-file'
let appEnv = $('input[name="env"]').val()
let officeId = $('input[name="office_id"]').val()
let arrKey = $('input[name="key"]').val()
let questionType = $('input[name="type"]').val()
let localStorageItemName = `${appEnv}_${officeId}_restaurant_questions`

/**
 * Put data from Form to LocalStorage
 **/
function putFormDataToObj() {
    let answerLocalStorage = JSON.parse(localStorage.getItem(localStorageItemName))

    if  (!answerLocalStorage) {
        answerLocalStorage = {}
    }

    if (answerLocalStorage &&
        answerLocalStorage[`${arrKey}`] &&
        Object.keys(answerLocalStorage[`${arrKey}`]).length > 0
    ) {
        delete answerLocalStorage[`${arrKey}`]
    }

    switch (questionType) {
        case TYPE_MULTI_MENU:
            let dataMenu = {
                type: questionType,
                menus: [],
            }

            let menuTableImgInput = $('input[name="menu_table_image"]')
            let menuTableImgObj = {}
            if (menuTableImgInput.attr('data-image-name')) {
                Object.assign(menuTableImgObj, {
                    name : $.trim( menuTableImgInput.data("imageName") ),
                    path : $.trim( menuTableImgInput.val() )
                })
            }

            if (menuTableImgInput.length > 0) {
                dataMenu['menu_table_image'] = !$.isEmptyObject(menuTableImgObj) ? menuTableImgObj : $.trim( menuTableImgInput.val() ) ?? ''
            }

            let storeImageInput = $('input[name="store_image"]')
            let storeImageObj = {}
            if (storeImageInput.attr('data-image-name')) {
                Object.assign(storeImageObj, {
                    name : $.trim( storeImageInput.data("imageName") ),
                    path : $.trim( storeImageInput.val() )
                })
            }

            if (storeImageInput.length > 0) {
                dataMenu['store_image'] = !$.isEmptyObject(storeImageObj) ? storeImageObj : $.trim( storeImageInput.val() ) ?? ''
            }

            $('.menu_form').each(function (index, element) {
                let menuFormId = `#${element.id}`
                let formData = $(menuFormId).serializeArray()
                let tmpObj = {}
                let menuImageObj = {}

                /*
                * Check if the form has any value
                * */
                formData = formData.filter(item => {
                    return item.value && item.name !== 'menu_key'
                })

                if (formData.length > 0) {
                    let imageInput = $(menuFormId).find('input[name="menu_image_path"]')
                    if (imageInput.attr('data-image-name')) {
                        Object.assign(menuImageObj, {
                            name : $.trim( imageInput.data("imageName") ),
                            path : $.trim( imageInput.val() )
                        })
                    }

                    tmpObj = {
                        name: $.trim( $(menuFormId).find('input[name="menu_name"]').val() ),
                        image: !$.isEmptyObject(menuImageObj) ? menuImageObj : $.trim( imageInput.val() ),
                        ingredients: []
                    }

                    let isValidPeriod = validatePeriodDate(element.id)
                    if ($(menuFormId).find('input[name="public_period_start"]').length > 0 && isValidPeriod) {
                        tmpObj['public_period_start'] = $.trim( $(menuFormId).find('input[name="public_period_start"]').val() )
                    }

                    if ($(menuFormId).find('input[name="public_period_end"]').length > 0 && isValidPeriod) {
                        tmpObj['public_period_end'] = $.trim( $(menuFormId).find('input[name="public_period_end"]').val() )
                    }

                    $(menuFormId).find('.ingredient_sec').each(function (index, ingredientEl) {
                        let ingredientSecId = `#${ingredientEl.id}`
                        let extraInfoObj = {
                            certification_ingredients: [],
                            region_ingredients: [],
                            ethical_ingredients: [],
                            appropriate_initiative: []
                        }

                        Object.keys(extraInfoObj).forEach((item) => {
                            $(ingredientSecId).find(`input[data-name=${item}]`).each(function (i, e) {
                                if (e.checked === true) {
                                    extraInfoObj[item].push(e.value)
                                }
                            })
                        })

                        /*
                        * Only get item in extra info array contains values
                        * */
                        const extraInfoArr = Object.entries(extraInfoObj)
                            .filter(([key, value]) => value && value.length > 0)

                        let tmpIngredient = {}

                        tmpIngredient = {
                            name: $.trim( $(ingredientSecId).find('input[name="ingredient_name"]').val() ),
                            extra_information: Object.fromEntries(extraInfoArr)
                        }

                        if ($(ingredientSecId).find('input[name="ingredient_supplier"]').length > 0) {
                            tmpIngredient['supplier'] = $.trim( $(ingredientSecId).find('input[name="ingredient_supplier"]').val() )
                        }

                        if ($(ingredientSecId).find('input[name="local_ingredient"]').length > 0) {
                            tmpIngredient['local_ingredient'] = $(ingredientSecId).find('input[name="local_ingredient"]:checked').length > 0 ? 1 : 0
                        }

                        if ($(ingredientSecId).find('input[name="effort_image"]').length > 0) {
                            let tmpEffortImg = {}
                            let effortImgInput = $(ingredientSecId).find('input[name="effort_image"]')

                            if (effortImgInput.attr('data-image-name')) {
                                Object.assign(tmpEffortImg, {
                                    name : $.trim( effortImgInput.data("imageName") ),
                                    path : $.trim( effortImgInput.val() )
                                })
                            }

                            tmpIngredient['effort_image'] = !$.isEmptyObject(tmpEffortImg) ? tmpEffortImg : $.trim( effortImgInput.val() )
                        }

                        tmpObj.ingredients.push(tmpIngredient)
                    })

                    dataMenu.menus.push(tmpObj)
                }
            })

            if (dataMenu.menus.length > 0 ||
                dataMenu.menu_table_image ||
                dataMenu.store_image
            ) {
                dataMenu.menus = dataMenu.menus.filter(el => el)
                answerLocalStorage[`${arrKey}`] = dataMenu
            }
            break;

        case TYPE_CHECKBOX:
            let formData = $('#select_form').serializeArray()
            let dataSelect = {
                type: questionType,
                answers: [],
            }

            formData = formData.filter(item => {
                return item.value && item.value.replace(/\s/g, '').length > 0
            })

            if (formData.length > 0) {
                formData.map((item) => {
                    if (item.name === 'answers') {
                        dataSelect.answers.push(item.value)
                    } else if (item.name === 'menu_table_image' || item.name === 'effort_image') {
                        let imgInput = $(`input[name="${item.name}"]`)
                        let imgObj = {}

                        if (imgInput.attr('data-image-name')) {
                            Object.assign(imgObj, {
                                name : $.trim( imgInput.data("imageName") ),
                                path : $.trim( item.value )
                            })
                        }

                        dataSelect[`${item.name}`] = !$.isEmptyObject(imgObj) ? imgObj : $.trim( item.value ) ?? ''
                    } else {
                        dataSelect[`${item.name}`] = $.trim(item.value)
                    }
                })

                answerLocalStorage[`${arrKey}`] = dataSelect
            }
            break;
    }

    localStorage.setItem(localStorageItemName, JSON.stringify(answerLocalStorage));
};

/**
 * Move to next question
 **/
function nextQuestion($event) {
    putFormDataToObj()
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
        url: '/restaurants/answer/apply',
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

            window.location.replace('/restaurants/application/complete')
        })
        .fail(function (error) {
            $.LoadingOverlay("hide");

            var message = ''
            if (error && error.responseJSON) {
                message = error.responseJSON.message
            }
            showErrorMessage(message)
        });
};

function loadAnswerQuestions(callbackFunc) {
    $.ajax({
        url: '/restaurants/answer/show',
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

function uploadImg($event) {
    compressAndUploadFile($event.target, uploadRequest)
}

function validatePeriodDate(formId) {
    let formElement = `#${formId}`
    let startElement = $(formElement).find('input[name="public_period_start"]')
    let endElement = $(formElement).find('input[name="public_period_end"]')

    let startDate = new Date(startElement.val()).getTime()
    let endDate = new Date(endElement.val()).getTime()
    let messageSec = $(formElement).find('.calendar_message span')

    if (!isNaN(startDate) && startDate) {
        endElement.attr('min', startElement.val())
    } else {
        endElement.removeAttr('min')
    }

    if (endDate < startDate) {
        messageSec.text('日付の範囲が無効です。').fadeIn()

        return false;
    } else {
        messageSec.text('').fadeOut()
    }

    return true;
}

$(document).ready(function () {
    let items = JSON.parse(localStorage.getItem(localStorageItemName))

    loadAnswerQuestions(function (data) {
        var queryParam = $.urlParam("mode")
        if (queryParam !== 'process') {
            if (!items && data) {
                localStorage.setItem(localStorageItemName, JSON.stringify(data));
            }
            items = data
        }

        if ((queryParam === 'process' && !items)) {
            items = data
            localStorage.setItem(localStorageItemName, JSON.stringify(data));
        }

        /**
         * Load data from LocalStorage to DOM
         **/
        if (items && items[`${arrKey}`]) {
            let questionObj = items[`${arrKey}`]

            switch (questionObj.type) {
                case TYPE_MULTI_MENU:

                    if (questionObj.menus && questionObj.menus.length > 0) {
                        for (let i = 1; i <= questionObj.menus.length - 1; i++) {
                            getHtmlMenuAppend()
                        }
                    }

                    let menuTableImgInput = $('input[name="menu_table_image"]')
                    let storeImgInput = $('input[name="store_image"]')

                    if (questionObj.menu_table_image) {
                        let isObjectImage = $.isPlainObject(questionObj.menu_table_image) && !$.isEmptyObject(questionObj.menu_table_image)

                        menuTableImgInput.val(isObjectImage ? questionObj.menu_table_image.path : questionObj.menu_table_image)
                        menuTableImgInput.attr('data-image-name', isObjectImage ? questionObj.menu_table_image.name : questionObj.menu_table_image.replace("uploads/restaurants/", ""))
                        menuTableImgInput.siblings().find('span.file_name').text(isObjectImage ? `(${questionObj.menu_table_image.name})` : `(${questionObj.menu_table_image.replace("uploads/restaurants/", "")})`)
                    }

                    if (questionObj.store_image) {
                        let isObjectImage = $.isPlainObject(questionObj.store_image) && !$.isEmptyObject(questionObj.store_image)

                        storeImgInput.val(isObjectImage ? questionObj.store_image.path : questionObj.store_image)
                        storeImgInput.attr('data-image-name', isObjectImage ? questionObj.store_image.name : questionObj.store_image.replace("uploads/restaurants/", ""))
                        storeImgInput.siblings().find('span.file_name').text(isObjectImage ? `(${questionObj.store_image.name})` : `(${questionObj.store_image.replace("uploads/restaurants/", "")})`)
                    }

                    if (questionObj.menus.length > 0) {
                        $('.menu_form').each((index, element) => {
                            let menuFormId = `#${element.id}`
                            let menuKey = $(menuFormId).find('input[name="menu_key"]').val()
                            let menu = questionObj.menus[index]

                            $(menuFormId).find('input[name="menu_name"]').val(menu.name)

                            if (menu.hasOwnProperty('public_period_start')) {
                                $(menuFormId).find('input[name="public_period_start"]').val(menu.public_period_start)
                            }

                            if (menu.hasOwnProperty('public_period_end')) {
                                $(menuFormId).find('input[name="public_period_end"]').val(menu.public_period_end)
                            }

                            validatePeriodDate(element.id)

                            /**
                             * Display image and add class active to label file icon
                             **/
                            if (menu.image) {
                                let isObjectImage = $.isPlainObject(menu.image) && !$.isEmptyObject(menu.image)
                                let imageSec = $(menuFormId).find('.answer_btn')

                                imageSec.addClass('active')
                                imageSec.siblings().find('img').addClass('pic_menus')
                                imageSec.siblings().find('img').attr('src', isObjectImage ? `/${menu.image.path}` : `/${menu.image}`)
                                imageSec.find('input[name="menu_image_path"]').val(isObjectImage ? menu.image.path : menu.image)
                                imageSec.find('input[name="menu_image_path"]')
                                            .attr('data-image-name', isObjectImage ? menu.image.name
                                                                                   : menu.image.replace("uploads/restaurants/", "") ?? '')
                            }

                            if (menu.ingredients.length > 0) {
                                /*
                                * Ingredients
                                * */
                                for (let i = 1; i <= menu.ingredients.length - 1; i++) {
                                    getHtmlIngredientAppend(menuFormId, menuKey)
                                }

                                $(menuFormId).find('.ingredient_sec').each((ingredientIdx, ingredientEl) => {
                                    let ingredientSecId = `#${ingredientEl.id}`

                                    $(ingredientSecId).find('input[name="ingredient_name"]').val(menu.ingredients[ingredientIdx].name)

                                    if (menu.ingredients[ingredientIdx].hasOwnProperty('supplier')) {
                                        $(ingredientSecId).find('input[name="ingredient_supplier"]').val(menu.ingredients[ingredientIdx].supplier)
                                    }

                                    if (menu.ingredients[ingredientIdx].hasOwnProperty('local_ingredient')) {
                                        $(ingredientSecId).find('input[name="local_ingredient"]').prop('checked', menu.ingredients[ingredientIdx].local_ingredient === 1);
                                    }

                                    if (menu.ingredients[ingredientIdx].hasOwnProperty('effort_image') && menu.ingredients[ingredientIdx].effort_image) {
                                        let isObjectImage = $.isPlainObject(menu.ingredients[ingredientIdx].effort_image) &&
                                                            !$.isEmptyObject(menu.ingredients[ingredientIdx].effort_image)

                                        let effortImgInput = $(ingredientSecId).find('input[name="effort_image"]')

                                        effortImgInput
                                            .val(isObjectImage ? menu.ingredients[ingredientIdx].effort_image.path : menu.ingredients[ingredientIdx].effort_image);

                                        effortImgInput
                                            .attr('data-image-name', isObjectImage ? menu.ingredients[ingredientIdx].effort_image.name : menu.ingredients[ingredientIdx].effort_image.replace("uploads/restaurants/", ""));

                                        effortImgInput
                                            .siblings()
                                            .find('span.file_name')
                                            .text(isObjectImage ? `(${menu.ingredients[ingredientIdx].effort_image.name})` : `(${menu.ingredients[ingredientIdx].effort_image.replace("uploads/restaurants/", "")})`);
                                    }

                                    /*
                                    * Extra information
                                    * */
                                    $(ingredientSecId).find('.extra_info_sec').each((infoIdx, infoEl) => {
                                        let infoSecId = `#${infoEl.id}`
                                        let inputs = $(infoSecId).find('input')

                                        $(inputs).each((i, e) => {
                                            if (menu.ingredients[ingredientIdx].extra_information &&
                                                Object.keys(menu.ingredients[ingredientIdx].extra_information).length > 0 &&
                                                menu.ingredients[ingredientIdx].extra_information[e.dataset.name] &&
                                                menu.ingredients[ingredientIdx].extra_information[e.dataset.name].includes(e.value)
                                            ){
                                                e.checked = true
                                            }
                                        })
                                    })
                                })
                            }
                        })
                    }
                    break;

                case TYPE_CHECKBOX:
                case TYPE_RADIO:
                    let selectFormId = $('#select_form')
                    let inputs = $(selectFormId).find(':input')

                    $(inputs).each((i, e) => {
                        if ([TYPE_CHECKBOX, TYPE_RADIO].includes(e.type)) {
                            e.checked = questionObj.answers.includes(e.value)
                        } else {
                            if (e.name === Object.keys(questionObj).find(item => item === e.name)) {

                                if (e.name === 'menu_table_image' || e.name === 'effort_image') {
                                    let isObjectImage = $.isPlainObject(questionObj[`${e.name}`]) &&
                                        !$.isEmptyObject(questionObj[`${e.name}`])

                                    e.value = isObjectImage ? questionObj[`${e.name}`]['path'] :  questionObj[`${e.name}`]
                                    $(e).attr('data-image-name', isObjectImage ? questionObj[`${e.name}`]['name'] : questionObj[`${e.name}`])
                                    $(e).siblings().find('span.file_name').text(isObjectImage ? questionObj[`${e.name}`]['name'] : `(${questionObj[`${e.name}`].replace("uploads/restaurants/", "")})`)
                                } else {
                                    e.value = questionObj[`${e.name}`]
                                }

                            }
                        }
                    })
                    break;
            }
        }

        /**
         * Confirmation page
         **/
        if (items && Object.keys(items).length > 0) {
            $('#btn_apply').removeClass('btn-disabled')
            for (var i in items) {
                if (items[i] && Object.keys(items[i]).length > 0) {
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
    $('input[name="answer_file"]').change(function (e) {
        compressAndUploadFile(e.target, uploadRequest)
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

    $('.next_question').bind('click', nextQuestion);
    $('.prev_question').bind('click', nextQuestion);
    $('.btn_to_confirm').bind('click', nextQuestion);
    $('.public_period').bind('change', function (e) {
        let formId = $(e.target).closest('form').attr('id')
        validatePeriodDate(formId)
    })
})


