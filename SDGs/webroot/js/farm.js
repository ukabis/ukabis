let inquiryRequest = '/farms/restaurant-requests/inquiry'

$(document).ready(function() {
    $('.logout').click(function () {
        localStorage.clear();
    })

    /**
     * Send mail inquiry
     **/
    $('#btn_send_mail').click(function (e) {
        e.preventDefault()
        var destinationOfficeId = $('input[name="destination_office_id"]').val()
        var formData = new FormData()
        formData.append('destination_office_id', destinationOfficeId)

        sendMail(inquiryRequest, formData)
    })
})
