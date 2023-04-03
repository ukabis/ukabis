<!-- Cushioning -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Component
import AdminInput from '../AdminCommon/AdminInput.vue'
import TextInputWrap from '@/components/AdminCommon/TextInputWrap.vue'

// Store
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

const receiveValue = (value: string | number, name?: string) => {
    if (isNaN(Number(value))) {
        value = value.toString()
        switch (name) {
            case "packaging_conditions": adminJasPackageData.value.packaging_conditions = value; break;
            case "shock_sensor": adminJasPackageData.value.shock_sensor = value; break;
            default: break;
        }
    } else {
        value = Number(value)
        switch (name) {
            case "allowable_g": adminJasPackageData.value.allowable_g = value; break;
            case "allowable_acceleration": adminJasPackageData.value.allowable_acceleration = value; break;
            case "allowable_shocks": adminJasPackageData.value.allowable_shocks = value; break;
            default: break;
        }
    }
}

</script>

<template>
    <div class="cushioning_wrap" :class="{grayout: !adminJasPackageData.no_cushioning}">
        <div class="item">
            <div class="left">
                <div>許容される衝撃の上限</div>
            </div>
            <div class="right">
                <div>
                    <span class="num">
                        <AdminInput :value="adminJasPackageData.allowable_g" input_mode="text" name="allowable_g" @emit-num="receiveValue" :is-read-only="!adminJasPackageData.no_cushioning" />
                    </span>
                    <span class="string">G （</span>
                    <span class="num">
                        <AdminInput :value="adminJasPackageData.allowable_acceleration" input_mode="text" name="allowable_acceleration" @emit-num="receiveValue" :is-read-only="!adminJasPackageData.no_cushioning" />
                    </span>
                    <span class="string">m/s<sup>2</sup>）</span>
                </div>
            </div>
        </div>
        <div class="item">
            <div class="left">
                <div>許容される衝撃の回数</div>
            </div>
            <div class="right">
                <div>
                    <span class="num">
                        <AdminInput :value="adminJasPackageData.allowable_shocks" input_mode="text" name="allowable_shocks" @emit-num="receiveValue" :is-read-only="!adminJasPackageData.no_cushioning" />
                    </span>
                    <span class="string">回</span>
                </div>
            </div>
        </div>
        <div class="packaging_conditions_wrap text_wrap">
            <TextInputWrap input_mode="text" label="包装条件" name="packaging_conditions" @receive-value="receiveValue" :is-read-only="!adminJasPackageData.no_cushioning" />
        </div>
        <div class="use_sensor_wrap text_wrap">
            <TextInputWrap input_mode="text" label="使用センサー" name="shock_sensor" @receive-value="receiveValue" :is-read-only="!adminJasPackageData.no_cushioning" />
        </div>
</div>
</template>

<style scoped lang="scss">
.cushioning_wrap {
    width: 95.8%;
    max-width: 895px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    margin: 12px 25px 0px;
    font-weight: 600;

    &>div {
        padding-top: 12px;
    }

    .item {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
        width: 100%;
        max-width: 945px;
        margin-bottom: 10px;

        .right {
            text-align: left;
            min-width: 333px;

            .num {
                display: inline-block;
                font-size: 20px;
                text-align: right;
                margin: 0px 20px;
                width: 80px;
            }

            .string {
                font-size: 16px;
            }
        }
    }
}
</style>