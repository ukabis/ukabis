import { axios } from '@/util/axios'

export interface ProductSize {
  id: string
  GtinCode: any
  ProductSizeCode: string
  ProductSizeName: string
  SizeLang: any
  _Owner_Id: string
}


// Get ProductSizeList
export const getProductSizeList = async (vendor_access_token: string, openidconnect_access_token: string) => {
    const apiPath = "/API/Traceability/V3/Master/ProductSize"
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const productSizeList = response?.data
    return productSizeList as ProductSize[]
}

// Get ProductSizeName By ProductSizeCode
export const getProductSizeNameByProductSizeCode = async (vendor_access_token: string, openidconnect_access_token: string, productSizeCode: string) => {
    const apiBaseUrl = "/API/Traceability/V3/Master/ProductSize"
    const apiPath = apiBaseUrl + "/Get/" + productSizeCode
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const productSizeName: string = response?.data.ProductSizeName
    return productSizeName
}
