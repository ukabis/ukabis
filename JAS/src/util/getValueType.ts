export const getValueType = (value: string | number): string => {
    if(isNaN(Number(value))) {
        return "string"
    } else {
        return "number"
    }
}