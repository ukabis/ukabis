import { axios } from '@/util/axios'

export type Office = {
  id: string
  Address1: string
  Address1Lang?: Address1Lang[]
  Address2: string
  Address2Lang: any
  Address3: string
  Address3Lang: any
  CompanyId: string
  Fax: any
  GlnCode?: string
  Images: any
  IsPublic: number
  OfficeId: string
  OfficeName: string
  OfficeNameKana: string
  OfficeNameLang?: OfficeNameLang[]
  Tel: string
  ZipCode: string
  _Owner_Id: string
}

export type Address1Lang = {
  Address: string
  LocaleCode: string
}

export type OfficeNameLang = {
  Name: string
  LocaleCode: string
}

// Get CompanyList
export const getOfficeList = async (vendor_access_token: string, openidconnect_access_token: string) => {
    // console.debug("run getOfficeList")
    const apiPath = "/API/CompanyMaster/V3/Private/Office/ODataOtherAccessible"
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const companyList = response?.data
    return companyList as Office[]
}
