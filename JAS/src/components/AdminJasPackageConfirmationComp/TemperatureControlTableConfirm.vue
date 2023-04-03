<!-- TemperatureControlTable -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Store
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

// 品質を維持するのに適正な温度範囲
const appropriateTemperatureRangeMin = adminJasPackageData.value.temperature_range_min
const appropriateTemperatureRangeMax = adminJasPackageData.value.temperature_range_max

// 品質管理ができない状態で許容される温度範囲 1
const permissibleTemperatureRange1Min = adminJasPackageData.value.uncontrolled_temperature_range_1_min
const permissibleTemperatureRange1Max = adminJasPackageData.value.uncontrolled_temperature_range_1_max

// 品質管理ができない状態で許容される積算時間 1
const allowableIntegratedTime1 = adminJasPackageData.value.uncontrolled_time_1

// 品質管理ができない状態で許容される温度範囲 2
const permissibleTemperatureRange2Min = adminJasPackageData.value.uncontrolled_temperature_range_2_min
const permissibleTemperatureRange2Max = adminJasPackageData.value.uncontrolled_temperature_range_2_max

// 品質管理ができない状態で許容される積算時間 2
const allowableIntegratedTime2 = adminJasPackageData.value.uncontrolled_time_2

// 使用センサー
const useSensor = adminJasPackageData.value.temperature_sensor

</script>

<template>
    <div class="temperature_control_wrap">
        <div class="item">
            <div class="left">
                <div>品質を維持するのに適正な温度範囲</div>
            </div>
            <div class="right">
                <div>
                    <span class="num min">{{ appropriateTemperatureRangeMin }}</span>
                    <span class="string">℃ 〜</span>
                    <span class="num max">{{ appropriateTemperatureRangeMax }}</span>
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
                    <span class="num min">{{ permissibleTemperatureRange1Min }}</span>
                    <span class="string">℃ 〜</span>
                    <span class="num max">{{ permissibleTemperatureRange1Max }}</span>
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
                    <span class="num">{{ allowableIntegratedTime1 }}</span>
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
                    <span class="num min">{{ permissibleTemperatureRange2Min }}</span>
                    <span class="string">℃ 〜</span>
                    <span class="num max">{{ permissibleTemperatureRange2Max }}</span>
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
                    <span class="num">{{ allowableIntegratedTime2 }}</span>
                    <span class="string">時間</span>
                </div>
            </div>
        </div>
        <div class="item">
            <div class="left">
                <div>使用センサー</div>
            </div>
            <div class="right">
                <div>{{ useSensor }}</div>
            </div>
        </div>
    </div>
</template>

<style scoped lang="scss">
.temperature_control_wrap {
    width: 95.8%;
    max-width: 835px;
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

        .left, .right {
            padding-bottom: 7.7px;
        }
        .right {
            min-width: 267px;

            .num {
                display: inline-block;
                font-size: 20px;
                width: 63px;
                text-align: right;
                padding-right: 38px;

                &.max {
                    padding-right: 23px;
                }
            }

            .string {
                font-size: 16px;
                margin-right: 44px;
            }
        }
    }
}
</style>