<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Component
import CheckInput from './CheckInput.vue'
import TextInputWrap from '@/components/AdminCommon/TextInputWrap.vue'

const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

const onCheck = (name: string) => {
    emits("reserveCheck", name)
}

const emits = defineEmits<{
    (e: 'reserveCheck', name: string): void
}>()

const receiveValue = (value: string, name?: string) => {
    switch (name) {
        case "preliminary_1": adminJasPackageData.value.preliminary_1 = value; break;
        case "preliminary_2": adminJasPackageData.value.preliminary_2 = value; break;
        default:break;
    }
}

</script>

<template>
    <div class="ma_film_wrap">
        <div class="item">
            <div class="left">
                <CheckInput name="reserve1" :value="adminJasPackageData.reserve1" label="予備項目 1"
                    @check="onCheck('reserve1')" />
            </div>
            <div class="right" :class="{grayout: !adminJasPackageData.reserve1}">
                <TextInputWrap :is-read-only="!adminJasPackageData.reserve1" :value="adminJasPackageData.preliminary_1" input_mode="textarea" name="preliminary_1" @receive-value="receiveValue" />
            </div>
        </div>
        <div class="item">
            <div class="left">
                <CheckInput name="reserve2" :value="adminJasPackageData.reserve2" label="予備項目２"
                    @check="onCheck('reserve2')" />
            </div>
            <div class="right" :class="{grayout: !adminJasPackageData.reserve2}">
                <TextInputWrap :is-read-only="!adminJasPackageData.reserve2" :value="adminJasPackageData.preliminary_2"  input_mode="textarea" name="preliminary_2" @receive-value="receiveValue" />
            </div>
        </div>
</div>
</template>

<style scoped lang="scss">
.ma_film_wrap {
    margin-top: 48px;

    .item {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
        margin-bottom: 11px;
        width: 100%;
        max-width: 905px;

        &:last-child {
            margin-bottom: 0px;
        }

        .left {
            .check_item {
                margin-top: 0px;
                margin-bottom: 11px;
                display: flex;
                justify-content: start;
                text-align: left;

                &.hidden {
                    display: none;
                }

                &::before {
                    content: url('/src/assets/common/checkmark.svg');
                    padding-top: 3px;
                    margin-right: 4px;
                }
            }
        }

        .right {
            width: 100%;
            max-width: 375px;
            font-weight: 300;
        }
    }
}
</style>
