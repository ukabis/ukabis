import { axios } from '@/util/axios'

// Get BrandName By BrandCode
export const getBrandNameByBrandCode = async (vendor_access_token: string, openidconnect_access_token: string, brandCode: string) => {
    // console.debug("run getBrandNameByBrandCode")

    // Main
    const apiBaseUrl = "/API/Traceability/V3/Master/CropBrand"
    const apiPath = apiBaseUrl + "/Get/" + brandCode
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const brandObj = response?.data

    const brandName: string = brandObj.BrandName
    return brandName
}
