import { calcCheckDigit } from "./calcCheckDigit"
import { isIndividualIdentificationNumber } from "./isIndividualIdentificationNumber"

export const parityCheck = (code: any) => {
    console.log("チェックを開始します。")

    // CodeData
    const codeData = code?.data

    // 個体識別番号チェック
    if(!isIndividualIdentificationNumber(codeData)) return

    // チェック準備
    const gtinCode = codeData.slice(2, 16)
    const gtinNum = Number(gtinCode)
    const checkDigit = gtinNum % 10

    console.log({
        gtinCode: gtinCode,
        gtinNum: gtinNum,
        checkDigit: checkDigit,
    })

    return isIndividualIdentificationNumber(codeData) && calcCheckDigit(gtinCode) == checkDigit
}
