<!-- TemperatureControlTable -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Component
import AdminInput from '../AdminCommon/AdminInput.vue'
import TextInputWrap from '@/components/AdminCommon/TextInputWrap.vue'

// Store
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

// 各入力数値取得
const receiveValue = (value: number, name?: string) => {
    const num = Number(value)
    switch (name) {
        case "temperature_range_min": adminJasPackageData.value.temperature_range_min = num; break;
        case "temperature_range_max": adminJasPackageData.value.temperature_range_max = num; break;
        case "uncontrolled_temperature_range_1_min": adminJasPackageData.value.uncontrolled_temperature_range_1_min = num; break;
        case "uncontrolled_temperature_range_1_max": adminJasPackageData.value.uncontrolled_temperature_range_1_max = num; break;
        case "uncontrolled_time_1": adminJasPackageData.value.uncontrolled_time_1 = num; break;
        case "uncontrolled_temperature_range_2_min": adminJasPackageData.value.uncontrolled_temperature_range_2_min = num; break;
        case "uncontrolled_temperature_range_2_max": adminJasPackageData.value.uncontrolled_temperature_range_2_max = num; break;
        case "uncontrolled_time_2": adminJasPackageData.value.uncontrolled_time_2 = num; break;
        default:; break;
    }
}

// 使用センサー入力値取得
const receiveTemperatureSensor = (value: string) => {
    adminJasPackageData.value.temperature_sensor = value
}

</script>

<template>
    <div class="temperature_control_wrap" :class="{grayout: !adminJasPackageData.temperature_control}">
        <div class="item">
            <div class="left">
                <div>品質を維持するのに適正な温度範囲</div>
            </div>
            <div class="right">
                <div>
                    <span class="num min">
                        <AdminInput name="temperature_range_min" input_mode="text" @emit-num="receiveValue" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.temperature_range_min" />
                    </span>
                    <span class="string">℃ 〜</span>
                    <span class="num max">
                        <AdminInput name="temperature_range_max" input_mode="text" @emit-num="receiveValue" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.temperature_range_max" />
                    </span>
                    <span class="string">℃</span>
                </div>
            </div>
        </div>
        <div class="item">
            <div class="left">
                <div>品質管理ができない状態で許容される温度範囲 1</div>
            </div>
            <div class="right">
                <div>
                    <span class="num min">
                        <AdminInput name="uncontrolled_temperature_range_1_min" input_mode="text"
                            @emit-num="receiveValue" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.uncontrolled_temperature_range_1_min" />
                    </span>
                    <span class="string">℃ 〜</span>
                    <span class="num max">
                        <AdminInput name="uncontrolled_temperature_range_1_max" input_mode="text"
                            @emit-num="receiveValue" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.uncontrolled_temperature_range_1_max" />
                    </span>
                    <span class="string">℃</span>
                </div>
            </div>
        </div>
        <div class="item">
            <div class="left">
                <div>品質管理ができない状態で許容される積算時間 1</div>
            </div>
            <div class="right">
                <div>
                    <span class="num">
                        <AdminInput name="uncontrolled_time_1" input_mode="text" @emit-num="receiveValue" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.uncontrolled_time_1" />
                    </span>
                    <span class="string">時間</span>
                </div>
            </div>
        </div>
        <div class="item">
            <div class="left">
                <div>品質管理ができない状態で許容される温度範囲 2</div>
            </div>
            <div class="right">
                <div>
                    <span class="num min">
                        <AdminInput name="uncontrolled_temperature_range_2_min" input_mode="text"
                            @emit-num="receiveValue" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.uncontrolled_temperature_range_2_min" />
                    </span>
                    <span class="string">℃ 〜</span>
                    <span class="num max">
                        <AdminInput name="uncontrolled_temperature_range_2_max" input_mode="text"
                            @emit-num="receiveValue" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.uncontrolled_temperature_range_2_max" />
                    </span>
                    <span class="string">℃</span>
                </div>
            </div>
        </div>
        <div class="item">
            <div class="left">
                <div>品質管理ができない状態で許容される積算時間 2</div>
            </div>
            <div class="right">
                <div>
                    <span class="num">
                        <AdminInput name="uncontrolled_time_2" input_mode="text" @emit-num="receiveValue" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.uncontrolled_time_2" />
                    </span>
                    <span class="string">時間</span>
                </div>
            </div>
        </div>
        <div class="use_sensor_wrap">
            <TextInputWrap input_mode="text" label="使用センサー" name="temperature_sensor" @receive-value="receiveTemperatureSensor" :is-read-only="!adminJasPackageData.temperature_control" :value="adminJasPackageData.temperature_sensor" />
        </div>
    </div>
</template>

<style scoped lang="scss">
.temperature_control_wrap {
    width: 95.8%;
    max-width: 955px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    margin: 12px 25px 0px;
    font-weight: 600;

    .item {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
        width: 100%;
        max-width: 945px;
        margin-bottom: 10px;
        padding: 7.7px 0px 0px;

        .left,
        .right {
            padding-bottom: 7.7px;
            display: flex;
            align-items: center;
        }

        .right {
            min-width: 334px;

            .num {
                display: inline-block;
                font-size: 20px;
                width: 90px;
                text-align: right;
                padding-right: 10px;

                &.max {
                    padding-right: 23px;
                    width: 103px;
                }
            }

            .string {
                font-size: 16px;
                margin-right: 14px;
            }
        }
    }
}
</style>