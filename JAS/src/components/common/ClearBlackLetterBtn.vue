<script setup lang="ts">

const props = defineProps<{
    btnLabel: string
    inputWidth: number | string
}>()

const getBtnWidth = (input: number | string): string => {
    if (typeof input === "number") {
        input = input + "px"
    }
    return input
}
const btnWidth = getBtnWidth(props.inputWidth)


const getLineBreak = (word: string) => {
    // console.log({word})
}
getLineBreak(props.btnLabel)

</script>

<template>
    <div id="clear_black_letter_btn" class="btn_wrap">
        <div class="btn_inner line_break">{{ props.btnLabel }}</div>
    </div>
</template>

<style scoped>
.btn_wrap {
    display: flex;
    justify-content: center;
}

.btn_inner {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-wrap: wrap;
    width: 100%;
    min-width: 263px;
    max-width: 360px;
    height: 70px;
    padding: 8px 26px;
    background: transparent;
    border: 1px solid var(--font-color);
    color: var(--font-color);
    border-radius: 35px;
    font-size: 16px;
    font-weight: 600;
    /* white-space: pre-wrap; */
}
</style>