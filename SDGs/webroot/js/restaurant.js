let inquiryRequest = '/restaurants/farm-requests/inquiry'

function viewImage(e) {
    let id = e.target.id

    if (id) {
        initImageViewer(id)
    }
}

$(document).ready(function() {
    /**
     * View menu images
     **/
    $('.img_btn_block img').click(function (e) {
        let id = e.target.id

        if (id) {
            initImageViewer(id)
        }
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

    $('.logout').click(function () {
        localStorage.clear();
    })
})
