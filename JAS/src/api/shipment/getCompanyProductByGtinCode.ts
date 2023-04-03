import { axios } from '@/util/axios'

// Type
export type CompanyProduct = {
    CodeType: string
    CompanyId: string
    GtinCode: string
    Images: string[] | undefined
    IsOrganic: boolean | number
    ProductName: string[] | undefined
    Profile: Profile
    RegistrationDate: Date[] | undefined
}

export type Profile = {
    BrandCode: string // ブランドコード
    BreedCode: string[] | undefined // 品種コード
    CropCode: string // 農作物コード
    CropTypeCode: string[] | undefined // 種類コード
    GradeCode: string[] | undefined // 等級コード
    ProducingAreaCode: string[] | undefined // 産地コード
    SizeCode: string[] | undefined // サイズコード
}

// Get CompanyProduct By GtinCode
export const getCompanyProductByGtinCode = async (vendor_access_token: string, openidconnect_access_token: string, gtinCode: string) => {
    // console.debug("run getCompanyProductByGtinCode")

    // Main
    const apiBaseUrl = "/API/Traceability/V3/Private/CompanyProduct"
    const apiPath = apiBaseUrl + "/Get/" + gtinCode
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const companyProduct = response?.data
    return companyProduct as CompanyProduct
}
