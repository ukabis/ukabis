import { axios } from '@/util/axios'

// cropCodeList: CropCodeList

// CropCodeList
export type CropCodeList = {
    id: string
    CropCode: string
    CropLang: CropLang[]
    CropName: string
    DownLevelCrop?: DownLevelCrop[]
}
export type CropLang = {
    LocaleCode: string
    CropName: string
}
export type DownLevelCrop = {
    CropCode: string
    CropName: string
    CropLang: CropLang2[]
    DownLevelCrop: DownLevelCrop2[]
}
export type CropLang2 = {
    LocaleCode: string
    CropName: string
}
export type DownLevelCrop2 = {
    CropCode: string
    CropName: string
    CropLang: CropLang3[]
    DownLevelCrop: any[]
}
export type CropLang3 = {
    LocaleCode: string
    CropName: string
}

// Get CropName By CropCode
export const getCropNameByCropCode = async (vendor_access_token: string, openidconnect_access_token: string, cropCode: string) => {
    // console.debug("run getCropNameByCropCode")

    // Main
    const apiBaseUrl = "/API/Master/Crop"
    const apiPath = apiBaseUrl + "/Get/" + cropCode
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const cropObj = response?.data
    const cropName: string = cropObj.CropName
    return cropName
}
