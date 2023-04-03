export const getCompanyListFromOptionObjJson = (optionObjJson: any) => {

    // Types
    type CompanyWithId = [
        string,
        CompanyData
    ]
    type CompanyData = {
        company: string
        office: string[]
        staff: string[]
    }

    // Prepare optionObj, companyIds, companyLabels
    const optionObj: CompanyWithId[] = Object.entries(optionObjJson)

    const companyIds: string[] = Object.keys(optionObjJson)
    const companyLabels: string[] = optionObj.map(item => item[1].company)

    // Generate companyValueAndLabelsObj
    const companyValueAndLabelsObj: {[key: string]: string} = {}
    for(let i = 0; i < companyIds.length; i++) {
        companyValueAndLabelsObj[companyIds[i]] = companyLabels[i]
    }

    return companyValueAndLabelsObj
}
