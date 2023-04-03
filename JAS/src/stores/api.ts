import { CropCodeList } from '@/api/master/getCropNameByCropCode'
import { Grade } from '@/api/traceability/getGrade'
import { defineStore } from 'pinia'

// Type
export type UseApi = {
    accessToken: string
    openId: string
    companyObj: CompanyOpt[]
    officeObj: OfficeOpt[]
    staffObj: StaffOpt[]
    groupRegisterItem: groupRegisterItem
    destinationCompany: {[key: string]: string}
    companyIdAndNames: {[key: string]: string}
    cropCodeList: CropCodeList[]
    gradeList: Grade[]
}

export type groupRegisterItem = {
    groupName: string
    manager: string[]
    CompanyId: string[]
    JasGroup: string[]
}

export type CompanyOpt = {
    companyId: string
    companyName: string
}
export type OfficeOpt = {
    officeId: string
    officeName: string
    companyId: string
}
export type StaffOpt = {
    staffId: string
    staffName: string
    officeId: string[]
}

// UseApi
export const UseApi = defineStore('api', {
    state: () => <UseApi>({
        accessToken: "",
        openId: "",
        companyObj: [],
        officeObj: [],
        staffObj: [],
        groupRegisterItem: {
            groupName: "",
            manager: [],
            CompanyId: [],
            JasGroup: []
        },
        destinationCompany: {
            companyId: "",
            companyName: ""
        },
        companyIdAndNames: {},
        cropCodeList: [],
        gradeList: [],
    }),
    getters: {},
    actions: {}
})
