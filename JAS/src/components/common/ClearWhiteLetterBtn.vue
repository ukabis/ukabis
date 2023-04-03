<script setup lang="ts">

const props = defineProps<{
    btnLabel: string
    inputWidth: number | string
}>()

const getBtnWidth = (input: number | string): string => {
    if(typeof input === "number") {
        input = input + "px"
    }
    return input
}
const btnWidth = getBtnWidth(props.inputWidth)

</script>

<template>
    <div id="clear_white_letter_btn" class="btn_wrap">
        <div class="btn_inner">
            {{ btnLabel }}
        </div>
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
    width: v-bind(btnWidth);
    min-width: 263px;
    max-width: 360px;
    height: 70px;
    padding: 0px 83px;
    background: transparent;
    border: 1px solid var(--white-color);
    color: var(--white-color);
    border-radius: 35px;
    font-size: 16px;
    font-weight: 600;
}
</style>