import { axios } from '@/util/axios'

export type Staff = {
  id: string
  CompanyId: string
  IsPublic: number
  MailAddress: string
  Offices?: string[]
  Roles: string[]
  StaffId: string
  StaffName: string
  StaffNameLang: any
  _Owner_Id: string
}

// Get Staff
export const getStaff = async (vendor_access_token: string, openidconnect_access_token: string, officeId?: string) => {
    console.log("run getStaff")
    const apiPath = "/API/CompanyMaster/V3/Private/Staff/ODataCertifiedApplication"
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const staffList = response?.data
    return staffList as Staff[]
}
