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

const onClick = () => {
    emits("clickBlackBtn")
}

const emits = defineEmits<{
    (e: "clickBlackBtn", value?: string): void
}>()

</script>

<template>
    <div id="black_btn" class="btn_wrap cursor_pointer">
        <div class="btn_inner" @click="onClick">
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
    aspect-ratio: 278 / 60;
    /* min-width: 263px; */
    max-width: 360px;
    padding: 0px 26px;
    background: var(--font-color);
    color: var(--white-color);
    border-radius: 35px;
    font-size: 16px;
    font-weight: 600;
}
</style>