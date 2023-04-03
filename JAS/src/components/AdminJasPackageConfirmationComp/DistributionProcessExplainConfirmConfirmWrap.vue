<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Component
import TemperatureControlTableConfirm from './TemperatureControlTableConfirm.vue'
import PickedMorningConfirm from './PickedMorningConfirm.vue'
import CushioningConfirm from './CushioningConfirm.vue'
import MaFilmConfirm from './MaFilmConfirm.vue'

// Store
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

</script>

<template>
    <div class="distribution_process_explain_wrap">
        <div class="distribution_process_explain_item" v-if="adminJasPackageData.precooling">
            <div class="check_item">予冷管理を行う場合はチェックを入れてください</div>
        </div>
        <div class="distribution_process_explain_item" v-if="adminJasPackageData.temperature_control">
            <div class="check_item">低温管理を行う場合はチェックを入れてください</div>
            <TemperatureControlTableConfirm />
        </div>
        <div class="distribution_process_explain_item" v-if="adminJasPackageData.picked_morning">
            <div class="check_item">朝採れを基準として用いる場合はチェックを入れてください</div>
            <PickedMorningConfirm />
        </div>
        <div class="distribution_process_explain_item" v-if="adminJasPackageData.impact_management">
            <div class="check_item">衝撃緩和効果が証明されている緩衝材を用いない場合はチェックを入れてください</div>
            <CushioningConfirm />
        </div>
        <div class="distribution_process_explain_item" v-if="adminJasPackageData.ma_film">
            <div class="check_item">湿度保持等の品質維持効果が証明されているMAフィルムを用いる場合はチェックを入れてください</div>
            <MaFilmConfirm />
        </div>
    </div>
</template>

<style scoped lang="scss">
.distribution_process_explain_wrap {
    font-size: 16px;
    font-weight: 600;
    color: var(--font-color);
    padding: 36px 0px 93.7px;

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