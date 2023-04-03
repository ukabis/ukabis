import { axios } from '@/util/axios'

// Get Shipment By ProductCode
export const getShipmentByProductCode = async (vendor_access_token: string, openidconnect_access_token: string, productCode: string) => {
    // console.debug("run getShipmentByProductCode")

    // console.log("入力されたQRコード:", productCode)

    // Main
    const apiBaseUrl = "/API/Traceability/V3/Private/Shipment/GetByProductCode"
    const param = "ProductCode=" + productCode
    const apiPath = apiBaseUrl + "?" + param
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const shipmentData = response?.data
    // console.log("shipmentData:", shipmentData)
    return shipmentData
}
