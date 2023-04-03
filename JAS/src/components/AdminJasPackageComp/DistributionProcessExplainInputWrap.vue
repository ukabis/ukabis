<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Component
import CheckInput from './CheckInput.vue'
import TemperatureControlTableInput from '@/components/AdminJasPackageComp/TemperatureControlTableInput.vue'
import PickedMorningInput from '@/components/AdminJasPackageComp/PickedMorningInput.vue'
import NoCushioningInput from '@/components/AdminJasPackageComp/NoCushioningInput.vue'
import MaFilmInput from '@/components/AdminJasPackageComp/MaFilmInput.vue'
import UseCushioningInput from './UseCushioningInput.vue'

const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

const onCheck = (name: string) => {
    switch (name) {
        case "precooling": adminJasPackageData.value.precooling = !adminJasPackageData.value.precooling; break;
        case "temperature_control": adminJasPackageData.value.temperature_control = !adminJasPackageData.value.temperature_control; break;
        case "picked_morning": adminJasPackageData.value.picked_morning = !adminJasPackageData.value.picked_morning; break;
        case "impact_management": adminJasPackageData.value.impact_management = !adminJasPackageData.value.impact_management; break;
        case "use_cushioning": adminJasPackageData.value.use_cushioning = !adminJasPackageData.value.use_cushioning; break;
        case "no_cushioning": adminJasPackageData.value.no_cushioning = !adminJasPackageData.value.no_cushioning; break;
        case "ma_film": adminJasPackageData.value.ma_film = !adminJasPackageData.value.ma_film; break;
        case "reserve1": adminJasPackageData.value.reserve1 = !adminJasPackageData.value.reserve1; break;
        case "reserve2": adminJasPackageData.value.reserve2 = !adminJasPackageData.value.reserve2; break; default:; break;
    }
}

</script>

<template>
    <div class="distribution_process_explain_input_wrap">
        <div class="jas_msg_wrap">
            <div class="jas_msg">
                JAS規格に基づいて、品質を維持するために下記の流通行程管理を行った場合は該当項目にチェックを入れてください。
            </div>
        </div>
        <div class="distribution_process_explain_item">
            <CheckInput name="precooling" :value="adminJasPackageData.precooling" label="予冷管理を行う場合はチェックを入れてください"
                @check="onCheck" />
        </div>
        <div class="distribution_process_explain_item">
            <CheckInput name="temperature_control" :value="adminJasPackageData.temperature_control"
                label="低温管理を行う場合はチェックを入れてください" @check="onCheck" />
            <TemperatureControlTableInput />
        </div>
        <div class="distribution_process_explain_item">
            <CheckInput name="picked_morning" :value="adminJasPackageData.picked_morning"
                label="朝採れを基準として用いる場合はチェックを入れてください" @check="onCheck" />
            <PickedMorningInput :is-grayout="!adminJasPackageData.picked_morning" />
        </div>
        <div class="distribution_process_explain_item">
            <CheckInput name="impact_management" :value="adminJasPackageData.impact_management" label="衝撃管理を行う場合はチェックを入れてください"
                @check="onCheck" />
        </div>
        <div class="distribution_process_explain_item">
            <CheckInput name="use_cushioning" :value="adminJasPackageData.use_cushioning"
                label="衝撃緩和効果が証明されている緩衝材を用いる場合はチェックを入れてください" @check="onCheck" />
            <UseCushioningInput :is-grayout="!adminJasPackageData.use_cushioning" />
        </div>
        <div class="distribution_process_explain_item">
            <CheckInput name="no_cushioning" :value="adminJasPackageData.no_cushioning"
                label="衝撃緩和効果が証明されている緩衝材を用いない場合はチェックを入れてください" @check="onCheck" />
            <NoCushioningInput />
        </div>
        <div class="distribution_process_explain_item">
            <CheckInput name="ma_film" :value="adminJasPackageData.ma_film"
                label="湿度保持等の品質維持効果が証明されているMAフィルムを用いる場合はチェックを入れてください" @check="onCheck" />
            <MaFilmInput @reserve-check="onCheck" />
        </div>
    </div>
</template>

<style scoped lang="scss">
.distribution_process_explain_input_wrap {
    font-size: 16px;
    font-weight: 600;
    color: var(--font-color);
    padding: 36px 0px 93.7px;

    .jas_msg_wrap {
        .jas_msg {
            font-size: 16px;
            font-weight: 600;
            color: var(--notice-font-color);
        }
    }

    .distribution_process_explain_item {
        margin-top: 35px;

        &:first-child {
            margin-top: 0px;
        }
    }

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
</style>