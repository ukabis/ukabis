import { axios } from '@/util/axios'

export type Company = {
    id: string
    Address1: string
    Address1Lang?: string
    Address2: string
    Address2Lang?: string
    Address3: string
    Address3Lang?: string
    AddressJoin?: string
    Agreach?: string
    Buyer?: string
    Ceo?: string
    CompanyId: string
    CompanyName: string
    CompanyNameKana?: string
    CompanyNameLang?: CompanyNameLang[]
    CountryCode?: string
    Fax?: string
    GlnCode?: string
    GroupId?: string
    GS1CompanyCode?: string
    HomepageUrl?: string
    Images?: Image[]
    IndustoryTypeCode?: string
    MailAddress?: string
    Producer?: string
    Tel: string
    TermsOfSettlement?: string
    ZipCode: string
    _Owner_Id: string
  }

  export type CompanyNameLang = {
    Name: string
    LocaleCode: string
  }

  export type Image = {
    ImagePath: string
    ImageDescription: string
    DefaultImageFlag: boolean
  }

// Get CompanyList
export const getCompanyList = async (vendor_access_token: string, openidconnect_access_token: string) => {
    // console.debug("run getCompanyList")

    // Main
    const apiPath = "/API/CompanyMaster/V3/Private/Company/ODataOtherAccessible"
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const companyList = response?.data
    return companyList as Company[]
}
