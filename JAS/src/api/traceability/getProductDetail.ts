import { axios } from '@/util/axios'

// Type

export type ProductDetail = {
    BoxNumber: any
    DelicaScopeURL: any
    FDA: any
    GtinCode: string
    Jas?: JasProductDetail
    LotNumber: any
    ProductCode: string
    Quantity: number
    RetailStoreOrderNumber: any
}

export type JasProductDetail = {
    Buffer: boolean
    CropLineageCode: string
    IsMaFilm: boolean
    IsPreCooling: boolean
    JasGroupId: string
    JudgmentItems: JudgmentItems
}

export type JudgmentItems = {
    Description: string
    Flag: boolean
    ItemName: string
}

// Get ProductDetail By GTINCode
export const getProductDetailByGTINCode = async (vendor_access_token: string, openidconnect_access_token: string, GtinCode: string) => {
    const apiBaseUrl = "/API/Traceability/V3/Private/ProductDetail"
    const apiPath = apiBaseUrl + "/OData?GtinCode=" + GtinCode
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const ProductDetail = response?.data
    return ProductDetail as ProductDetail[]
}
