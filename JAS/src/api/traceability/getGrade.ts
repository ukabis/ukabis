import { axios } from '@/util/axios'

export type Grade = {
  id: string
  GradeLang: string[]
  GtinCode: string
  ProductGradeCode: string
  ProductGradeName: string
  _Owner_Id: string
}

// Get GradeList
export const getGradeList = async (vendor_access_token: string, openidconnect_access_token: string) => {
    const apiPath = "/API/Traceability/V3/Master/Grade"
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const gradeList = response?.data
    return gradeList as Grade[]
}

// Get GradeName By GradeCode
export const getGradeNameByGradeCode = async (vendor_access_token: string, openidconnect_access_token: string, gradeCode: string) => {
    const apiBaseUrl = "/API/Traceability/V3/Master/Grade"
    const apiPath = apiBaseUrl + "/Get/" + gradeCode
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const gradeName: string = response?.data.ProductGradeName
    return gradeName
}
