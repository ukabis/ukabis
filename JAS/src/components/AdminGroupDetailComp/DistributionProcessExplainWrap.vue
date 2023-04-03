<script setup lang="ts">
// Component
import Cushioning from './Cushioning.vue'
import MaFilm from './MaFilm.vue'
import PickedMorning from './PickedMorning.vue'
import TemperatureControlTable from './TemperatureControlTable.vue'

</script>

<template>
    <div class="distribution_process_explain_wrap">
        <div class="distribution_process_explain_item">
            <div class="check_item">予冷管理を行う場合はチェックを入れてください</div>
        </div>
        <div class="distribution_process_explain_item">
            <div class="check_item">低温管理を行う場合はチェックを入れてください</div>
            <TemperatureControlTable />
        </div>
        <div class="distribution_process_explain_item">
            <div class="check_item">朝採れを基準として用いる場合はチェックを入れてください</div>
            <PickedMorning />
        </div>
        <div class="distribution_process_explain_item">
            <div class="check_item">衝撃緩和効果が証明されている緩衝材を用いない場合はチェックを入れてください</div>
            <Cushioning />
        </div>
        <div class="distribution_process_explain_item">
            <div class="check_item">湿度保持等の品質維持効果が証明されているMAフィルムを用いる場合はチェックを入れてください</div>
            <MaFilm />
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