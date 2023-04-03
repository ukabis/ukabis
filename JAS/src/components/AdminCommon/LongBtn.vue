<script setup lang="ts">

const props = defineProps<{
    label: string
    btnType: string // black or white
}>()

const clickBtn = () => {
    emits("clickBtn")
}

const emits = defineEmits<{
    (e: "clickBtn", value?: string): void
}>()

</script>

<template>
    <div class="confirm_btn_wrap">
        <div class="confirm_btn cursor_pointer" :class="`${props.btnType}`" @click="clickBtn">{{ label }}</div>
    </div>
</template>

<style scoped lang="scss">
.confirm_btn_wrap {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 137px;
    min-height: 137px;
    max-height: 137px;

    .confirm_btn {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 100%;
        max-width: 366px;
        max-height: 57px;
        font-size: 16px;
        font-weight: 600;
        color: var(--white-color);
        background: var(--font-color);
        border-radius: 29px;

        &.white {
            color: var(--font-color);
            background: transparent;
            border: 2px solid var(--font-color);
        }
    }
}
</style>