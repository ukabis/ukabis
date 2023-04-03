<!-- Cushioning -->
<script setup lang="ts">

// サンプルデータ
import admin_cushioning_data from '@/sample_data/admin_cushioning_data.json'


const upperLimitAllowableImpacts = admin_cushioning_data.upperLimitAllowableImpacts
const acceleration = admin_cushioning_data.acceleration
const allowableShocks = admin_cushioning_data.allowableShocks
const packagingConditions = admin_cushioning_data.packagingConditions
const useSensor = admin_cushioning_data.useSensor

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