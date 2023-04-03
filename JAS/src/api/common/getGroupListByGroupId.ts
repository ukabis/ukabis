import { axios } from '@/util/axios'
import { ref } from 'vue'

// Type

export type Group = {
    id: string
    CompanyId?: string
    groupId: string
    groupName: string
    Jas: any
    manager: string
    member: string
    representativeMember: string
    scope: string
    _Owner_Id: string
}

// Get GroupList By GroupId
export const getGroupListByGroupId = async (vendor_access_token: string, openidconnect_access_token: string, groupId: string) => {
    const apiBaseUrl = "/API/Global/Private/Groups/ODataCertifiedApplication"
    // const apiPath = apiBaseUrl + "/Get/" + groupId
    const apiPath = apiBaseUrl
    const options = {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Authorization': vendor_access_token,
            'Authorization': `Bearer ${openidconnect_access_token}`,
        }
    }
    const response = await axios.get(apiPath, options).catch(err => console.error(err))
    const groups: Group[] = response?.data
    const filterdGroups = groups.filter(item => item.groupId == groupId)
    let groupList: {[key: string]: string} = {}
    filterdGroups.forEach(item => {
        groupList[item.groupId] = item.groupName
    })
    return groupList
}
