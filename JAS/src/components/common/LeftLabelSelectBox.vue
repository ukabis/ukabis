<!-- LeftLabelSelectBox -->
<script setup lang="ts">
import { ref, watch } from 'vue';


const props = defineProps<{
    label: string // ラベル
    name: string // 名称
    option: { [key: string]: string } // オプション
}>()


const selectValue = ref<string>("hidden")
watch(selectValue, () => emits("emitsValue", props.name, selectValue.value))

const emits = defineEmits<{
    (e: "emitsValue", name: string, value: string): void
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
                <option v-for="value, key in option" :value="key">
                    {{ value }}
                </option>
            </select>
        </div>
    </div>
</template>

<style scoped lang="scss">
.select_box_wrap {
    max-width: 370px;
    margin: auto;
    margin-bottom: 61px;

    .label {
        display: flex;
        justify-content: start;
        font-weight: 600;
        margin-bottom: 10px;
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