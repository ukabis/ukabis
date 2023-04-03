const regex = new RegExp(/[0-9]{16}[0-9A-Z]{13}/)
export const isIndividualIdentificationNumber = (target: any): boolean => {
    const result = regex.test(target)
    return result
}
