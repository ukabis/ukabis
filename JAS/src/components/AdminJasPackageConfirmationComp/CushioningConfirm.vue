<!-- Cushioning -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Store
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

// 許容される衝撃の上限
const upperLimitAllowableImpacts = adminJasPackageData.value.allowable_g
const acceleration = adminJasPackageData.value.allowable_acceleration

// 許容される衝撃の回数
const allowableShocks = adminJasPackageData.value.allowable_shocks

// 包装条件
const packagingConditions = adminJasPackageData.value.packaging_conditions

// 使用センサー
const useSensor = adminJasPackageData.value.shock_sensor

</script>


<template>
    <div class="cushioning_wrap">
        <div class="item">
            <div class="left">
                <div>許容される衝撃の上限</div>
            </div>
            <div class="right">
                <div>
                    <span class="num">{{ upperLimitAllowableImpacts }}</span>
                    <span class="string">G （</span>
                    <span class="num">{{ acceleration }}</span>
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
                    <span class="num">{{ allowableShocks }}</span>
                    <span class="string">回</span>
                </div>
            </div>
        </div>
        <div class="item">
            <div class="left">
                <div>包装条件</div>
            </div>
            <div class="right">
                <div>
                    <span class="string">{{ packagingConditions }}</span>
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
.cushioning_wrap {
    width: 95.8%;
    max-width: 835px;
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
            text-align: right;

            .num {
                display: inline-block;
                font-size: 20px;
                text-align: right;
                margin: 0px 20px;
            }

            .string {
                font-size: 16px;
            }
        }
    }
}
</style>