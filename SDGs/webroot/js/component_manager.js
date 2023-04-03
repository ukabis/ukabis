/**
 **/
let menuComp = '';
let ingredientComp = '';
let menuMaxItem = $('input[name="menu_max_item"]').val()
let ingredientMaxItem = $('input[name="ingredient_max_item"]').val()
let newMenuFormId = '';

function getMenuComponentTemplate (ingredientComp, extraComp, menuKey, options = {}) {
    return `<form class="menu_form" id="menu_form_${menuKey}" enctype="multipart/form-data">
                <input type="hidden" name="menu_key" value="${menuKey}">
                <div class="wrap_answer_block">
                    <div class="answer_title_sub">
                        <p>メニュー名${menuKey}</p>
                    </div>
                    <div class="wrap_answer_form-btn_box">
                        <div class="answer_form-btn_box_change">
                            <p class="answer_form"><input type="text" maxlength="200" name="menu_name" placeholder="メニュー名${menuKey}"></p>
                        </div>
                        <div class="img_btn_block">
                            <p><img id="menu_image_src_${menuKey}" src="/images/img_no-image.png" alt="メニュー${menuKey}"></p>
                            <p class="answer_btn">
                                <label for="menu_image_${menuKey}">
                                    <i class="fas fa-paperclip"></i>
                                </label>
                                <input type="file" name="answer_file" id="menu_image_${menuKey}" class="answer_menu_file" accept="image/jpeg, image/png" hidden="hidden">
                                <input type="hidden" name="menu_image_path" data-image-id="menu_image_${menuKey}" value="">
                            </p>
                        </div>

                        ${options.calendarPeriodComp}

                        <div class="ingredient_contents">
                            ${ingredientComp}
                        </div>

                        <div class="next_end_btn_box">
                            <div>
                                <button type="button" class="next_btn" onclick="addMenu();">
                                    <p class="btn_text">別のメニューを登録</p>
                                </button>
                            </div>
                            <div>
                                <button type="button" class="end_btn" onclick="removeMenu(this);">
                                    <p class="btn_text">メニューを削除</p>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </form>`
}

function getIngredientComponentTemplate (extraComp, menuKey, ingreKey, options = {}) {
    return `<div class="ingredient_sec" id="menu_${menuKey}_ingredient_${Math.floor(100 + Math.random() * 99999)}" _>
        <div class="inner_form">
            <p class="answer_form"><input type="text" maxlength="200" name="ingredient_name" placeholder="食材その${ingreKey}"></p>
            ${options.supplierIngredientComp}
        </div>

        ${options.localIngredientComp}

        ${extraComp}

        ${options.effortImageIngredientComp}

        <div class="inner_btn_box">
            <div class="wrap_inner_btn">
                <button type="button" class="inner_btn" onclick="addIngredient(this, ${menuKey});">
                    <p class="symbol">＋</p>
                </button>
                <p class="btn_text">食材を追加</p>
            </div>
            <div class="wrap_inner_btn">
                <button type="button" class="inner_btn" onclick="removeIngredient(this);">
                    <p class="symbol">－</p>
                </button>
                <p class="btn_text">食材を削除</p>
            </div>
        </div>
    </div>`;
}

function getCalendarPeriodComp () {
    return ` <div class="calendar_head">
        <div class="calendar_message"><span></span></div>
        <div class="wrap_calendar_title">
            <p>掲載期間（開始）</p>
            <p>掲載期間（終了）</p>
        </div>
        <div class="inner_form wrap_calendar">
            <p class="answer_form calendar start"><input type="date" class="public_period" name="public_period_start" placeholder="掲載期間（1980.1 〜）"></p>
            <p class="wave">～</p>
            <p class="answer_form calendar end"><input type="date" class="public_period" name="public_period_end" placeholder="掲載期間（1980.1 〜）"></p>
        </div>
    </div>`
}

function extraOptionCompTemplate (infoItem, randomExtraCompId ,optionKey, optionVal) {
    return `<p>
            <label>
                <input type="${infoItem.type}"
                       name="${infoItem.name + '_' + randomExtraCompId}"
                       data-name="${infoItem.name}"
                       value="${optionKey}">
                <span class="inner_text">${optionVal.label}</span>
            </label>
        </p>`
}

function getLocalIngredientComp () {
    return `<div class="inner_box">
        <div class="input_group">
            <p>
                <label>
                    <input type="checkbox" name="local_ingredient">
                        <span class="inner_text">食材は地元食材である。</span>
                </label>
            </p>
        </div>
    </div>`
}

function getSupplierIngredientComp (ingreKey) {
    return `<p class="answer_form"><input type="text" maxlength="200" name="ingredient_supplier" placeholder="仕入れ先名その${ingreKey}"></p>`
}

function getEffortImageIngredientComp (menuKey, ingKey) {
    return `<div class="inner_box other_box">
        <div class="wrap_menu_img_btn">
            <button type="button" class="menu_img_store">
                <label for="menu_${menuKey}__effort_image_ingredient_${ingKey}">
                    取り組みが確認できる写真添付
                    <span class="file_name"></span>
                </label>
                <input type="file" name="answer_file" id="menu_${menuKey}__effort_image_ingredient_${ingKey}" class="answer_menu_file" accept="image/jpeg, image/png" hidden="hidden">
                <input type="hidden" name="effort_image" data-image-id="menu_${menuKey}__effort_image_ingredient_${ingKey}" value="">
            </button>
        </div>
    </div>`;
}

function getHtmlMenuAppend() {
    let masterData = $('input[name=menuComp]').val()
    let menus = JSON.parse(masterData).menus[1]

    let menu_forms = $('.menu_form')
    let nextMenuKey = parseInt(menu_forms.last().find('input[name="menu_key"]').val()) + 1
    newMenuFormId = `menu_form_${nextMenuKey}`

    let optionHtml = ''
    let extraSec = ''

    if (menu_forms.length < menuMaxItem) {
        Object.values(menus.ingredients[1].extra_information).forEach((infoItem) => {
            let randomExtraCompId = Math.floor(100 + Math.random() * 99999)
            Object.values(infoItem.options).forEach((optionVal, optionKey) => {
                optionHtml += extraOptionCompTemplate(infoItem, randomExtraCompId, optionKey + 1, optionVal)
            })

            extraSec += `<div class="extra_info_sec" id="menu_${nextMenuKey}_extra_info_${Math.floor(100 + Math.random() * 99999)}">
                            <div class="inner_box">
                                <p class="inner_title">${infoItem.title}</p>
                                <div class="input_group">
                                       ${optionHtml}
                                </div>
                            </div>
                        </div>`

            optionHtml = ''
        })

        let calendarPeriodComp = ''
        if (menus.hasOwnProperty('public_period_start') && menus.hasOwnProperty('public_period_end')) {
            calendarPeriodComp = getCalendarPeriodComp()
        }

        let supplierIngredientComp = ''
        if (menus.ingredients[1].hasOwnProperty('supplier')) {
            supplierIngredientComp = getSupplierIngredientComp(1)
        }

        let localIngredientComp = ''
        if (menus.ingredients[1].hasOwnProperty('local_ingredient')) {
            localIngredientComp = getLocalIngredientComp()
        }

        let effortImageIngredientComp = ''
        if (menus.ingredients[1].hasOwnProperty('effort_image')) {
            effortImageIngredientComp = getEffortImageIngredientComp(nextMenuKey, 1)
        }

        ingredientComp = getIngredientComponentTemplate(extraSec, nextMenuKey, 1, {
                supplierIngredientComp: supplierIngredientComp,
                localIngredientComp: localIngredientComp,
                effortImageIngredientComp: effortImageIngredientComp
        })

        menuComp = getMenuComponentTemplate(ingredientComp, extraSec, nextMenuKey, {
                calendarPeriodComp: calendarPeriodComp
        })

        $(`.contents .menu_sec`).append(menuComp)
        $(`#${newMenuFormId}`).find('input[name="answer_file"]') .bind('change', uploadImg)
        $(`#${newMenuFormId} .img_btn_block img`).bind('click', viewImage)
        $(`#${newMenuFormId} .public_period`).bind('change', function () {
            validatePeriodDate(`${newMenuFormId}`)
        })
    }
}

function addMenu() {
    getHtmlMenuAppend()

    if ($(`#${newMenuFormId}`).length > 0) {
        scrollToElement(newMenuFormId)
    }
}

function removeMenu($event) {
    if ($('.menu_form').length > 1) {
        let currentForm = $($event).parents('form')
        let elementScrollTarget = currentForm.prev().length > 0 ? currentForm.prev() : currentForm.next()

        currentForm.remove()

        if (elementScrollTarget.length > 0) {
            scrollToElement(elementScrollTarget.attr('id'))
        }
    }
}

/*
* Add ingredients
* */
function getHtmlIngredientAppend(el, menuKey) {
    let masterData = $('input[name=menuComp]').val()
    let ingredient = JSON.parse(masterData).menus[1].ingredients[1]

    let parentForm;
    if (el instanceof HTMLElement) {
        parentForm = $(el).parents('form')
    } else {
        parentForm = $(el)
    }

    let ingredientContents = parentForm.find('.ingredient_contents')
    let ingredientSec = parentForm.find('.ingredient_sec')
    let nextIngredientNum = parseInt(ingredientSec.length) + 1

    let optionHtml = ''
    let extraSec = ''

    let localIngredientComp = ''

    if (ingredient.hasOwnProperty('local_ingredient')) {
        localIngredientComp = getLocalIngredientComp()
    }

    let supplierIngredientComp = ''
    if (ingredient.hasOwnProperty('supplier')) {
        supplierIngredientComp = getSupplierIngredientComp(nextIngredientNum)
    }

    let effortImageIngredientComp = ''
    if (ingredient.hasOwnProperty('effort_image')) {
        effortImageIngredientComp = getEffortImageIngredientComp(menuKey, nextIngredientNum)
    }

    if (ingredientSec.length < ingredientMaxItem) {
        Object.values(ingredient.extra_information).forEach((infoItem) => {
            let randomExtraCompId = Math.floor(100 + Math.random() * 99999)
            Object.values(infoItem.options).forEach((optionVal, optionKey) => {
                optionHtml += extraOptionCompTemplate(infoItem, randomExtraCompId, optionKey + 1, optionVal)
            })

            extraSec += `<div class="extra_info_sec" id="menu_${menuKey}_extra_info_${Math.floor(100 + Math.random() * 99999)}">
                <div class="inner_box">
                    <p class="inner_title">${infoItem.title}</p>
                    <div class="input_group">
                           ${optionHtml}
                    </div>
                </div>
            </div>`

            optionHtml = ''
        })

        ingredientComp = getIngredientComponentTemplate(extraSec, menuKey, nextIngredientNum, {
            supplierIngredientComp: supplierIngredientComp,
            localIngredientComp: localIngredientComp,
            effortImageIngredientComp: effortImageIngredientComp
        })
        ingredientContents.append(ingredientComp)
        ingredientContents.find('input[name="answer_file"]') .bind('change', uploadImg)
    }
}

function addIngredient(el, menuKey) {
    getHtmlIngredientAppend(el, menuKey)

    if (el instanceof HTMLElement) {
        let parentSec = $(el).parents('.ingredient_sec')
        let nextIngredientSec = parentSec.next()

        if (nextIngredientSec.length > 0) {
            scrollToElement(nextIngredientSec.attr('id'))
        }
    }
}

function removeIngredient($event) {
    if ($($event).closest('.ingredient_contents').find('.ingredient_sec').length > 1) {
        let currentSec = $($event).parents('.ingredient_sec')
        let elementScrollTarget = currentSec.prev().length > 0 ? currentSec.prev() : currentSec.next()

        currentSec.remove()

        if (elementScrollTarget.length > 0) {
            scrollToElement(elementScrollTarget.attr('id'))
        }
    }
}


