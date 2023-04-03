import { axios } from '@/util/axios'

// Get PreLinkedSensorIDs
export const getPreLinkedSensorIDs = async (vendor_access_token: string, openidconnect_access_token: string) => {
    const apiPath = "/API/Traceability/V3/Private/ProductDetailSensorMap"
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const preLinkedSensorIDs = response?.data
    return preLinkedSensorIDs
}
