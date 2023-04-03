export const calcCheckDigit = (target: string) => {
    // input
    const code = target.slice(0, 13)
    const numArray: number[] = [...code].map(str => Number(str))

    // Calc even, odd
    const even = numArray.filter((num, index) => index % 2 == 0)
    const odd = numArray.filter((num, index) => index % 2 == 1)

    // Calc evenSum, oddSum
    const evenSum = even.reduce((prev, num) => prev + num, 0)
    const oddSum = odd.reduce((prev, num) => prev + num, 0)

    // Calc Total
    const sum = evenSum * 3 + oddSum

    // Calc Check Digit
    const checkDigit = 10 - sum % 10

    return checkDigit
}
