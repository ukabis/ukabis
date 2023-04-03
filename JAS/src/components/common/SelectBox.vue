<!-- SelectBox -->
<script setup lang="ts">import { onMounted, ref, watch, watchEffect } from 'vue'


const props = defineProps<{
    label: string
    option: {[key: string]: string}
}>()

const selectValue = ref<string>("hidden")
watch(selectValue, () => emits("emitsValue", selectValue.value))

// Emits
const emits = defineEmits<{
    (e: "emitsValue", value: string): void
}>()

</script>

<template>
    <div class="select_box_wrap">
        <span class="label">
            {{ props.label }}
        </span>
        <div class="select_box">
            <select v-model="selectValue">
                <option selected disabled value="hidden">選択してください</option>
                <option v-for="companyName, companyId in option" :value="companyId">
                    {{ companyName }}
                </option>
            </select>
        </div>
    </div>
</template>

<style scoped lang="scss">
.select_box_wrap {
    padding: 0px 35px;

    .label {
        display: flex;
        justify-content: center;
        font-weight: 600;
        margin-bottom: 20px;
    }

    select {
        width: 100%;
        height: 51px;
        border-radius: 7px;
        padding: 0px 10px;
        -webkit-appearance: none;
        appearance: none;

        /* デフォルトの矢印を無効 */
        select::-ms-expand {
            display: none;
            /* デフォルトの矢印を無効(IE用) */
        }
    }

    .select_box {
        display: flex;
        justify-content: center;
        align-items: center;
        margin: 0 auto 30px;
        position: relative;
        max-width: 360px;

        &::before {
            content: '';
            width: 1px;
            height: 84%;
            background: #797979;
            position: absolute;
            right: 14%;
        }

        &::after {
            content: url('/src/assets/common/arrow_orange.svg');
            width: auto;
            height: auto;
            position: absolute;
            right: 4%;
            pointer-events: none;
        }
    }
}
</style>