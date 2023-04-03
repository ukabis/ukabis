<!-- AdminJasPackageView -->
<script setup lang="ts">
// Util
import { ref } from 'vue'
import { storeToRefs } from 'pinia'
import { UseData } from '@/stores/data'

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import Breadcrumbs from '@/components/AdminCommon/Breadcrumbs.vue'
import AdminInput from '@/components/AdminCommon/AdminInput.vue'
import LongBtn from '@/components/AdminCommon/LongBtn.vue'
import DistributionProcessExplainInputWrap from '@/components/AdminJasPackageComp/DistributionProcessExplainInputWrap.vue'

// サンプルデータ
import admin_jas_package from '@/sample_data/admin_jas_package.json'
import { UseUtil } from '@/stores/util'
const cropCode: [string, string][] = admin_jas_package.cropCode.map(item => ([item[0], item[1]]))
const typeCode: [string, string][] = admin_jas_package.typeCode.map(item => ([item[0], item[1]]))
const varietyCode: [string, string][] = admin_jas_package.varietyCode.map(item => ([item[0], item[1]]))
const lineageCode: [string, string][] = admin_jas_package.lineageCode.map(item => ([item[0], item[1]]))

// ページタイトル
const pageTitle = "グループ詳細確認"

// Store
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

const util = UseUtil()

const receiveValue = (value: string, name?: string) => {
    console.log(`${name}から受け取った値：${value}`)
    switch (name) {
        case "cropCode":
            adminJasPackageData.value.cropCode = value
            break;
        case "typeCode":
            adminJasPackageData.value.typeCode = value
            break;
        case "varietyCode":
            adminJasPackageData.value.varietyCode = value
            break;
        case "lineageCode":
            adminJasPackageData.value.lineageCode = value
            break;
        case "management_criteria":
            adminJasPackageData.value.management_criteria = value
            break;
        case "certification_body":
            adminJasPackageData.value.certification_body = value
            break;
        default:
            break;
    }
}

const clickConfirmBtn = () => {
    console.log({
        "cropCode": adminJasPackageData.value.cropCode,
        "typeCode": adminJasPackageData.value.typeCode,
        "varietyCode": adminJasPackageData.value.varietyCode,
        "lineageCode": adminJasPackageData.value.lineageCode,
        "management_criteria": adminJasPackageData.value.management_criteria,
        "certification_body": adminJasPackageData.value.certification_body,
        "temperature_range_min": adminJasPackageData.value.temperature_range_min,
        "temperature_range_max": adminJasPackageData.value.temperature_range_max,
        "uncontrolled_temperature_range_1_min": adminJasPackageData.value.uncontrolled_temperature_range_1_min,
        "uncontrolled_temperature_range_1_max": adminJasPackageData.value.uncontrolled_temperature_range_1_max,
        "uncontrolled_time_1": adminJasPackageData.value.uncontrolled_time_1,
        "uncontrolled_temperature_range_2_min": adminJasPackageData.value.uncontrolled_temperature_range_2_min,
        "uncontrolled_temperature_range_2_max": adminJasPackageData.value.uncontrolled_temperature_range_2_max,
        "uncontrolled_time_2": adminJasPackageData.value.uncontrolled_time_2,
        "allowable_g": adminJasPackageData.value.allowable_g,
        "allowable_acceleration": adminJasPackageData.value.allowable_acceleration,
        "allowable_shocks": adminJasPackageData.value.allowable_shocks,
        "precooling": adminJasPackageData.value.precooling,
        "temperature_control": adminJasPackageData.value.temperature_control,
        "picked_morning": adminJasPackageData.value.picked_morning,
        "impact_management": adminJasPackageData.value.impact_management,
        "use_cushioning": adminJasPackageData.value.use_cushioning,
        "no_cushioning": adminJasPackageData.value.no_cushioning,
        "ma_film": adminJasPackageData.value.ma_film,
        "reserve1": adminJasPackageData.value.reserve1,
        "reserve2": adminJasPackageData.value.reserve2,
        "temperature_sensor": adminJasPackageData.value.temperature_sensor,
        "morning_material": adminJasPackageData.value.morning_material,
        "shock_material": adminJasPackageData.value.shock_material,
        "packaging_conditions": adminJasPackageData.value.packaging_conditions,
        "shock_sensor": adminJasPackageData.value.shock_sensor,
        "preliminary_1": adminJasPackageData.value.preliminary_1,
        "preliminary_2": adminJasPackageData.value.preliminary_2
    })
    util.transition("/admin/jaspackageconfirmation")
}

</script>

<template>
    <AdminHeader :is-show-sub-menu="true" :is-show-bottom-wrap="true" bottom-wrap-label="流通行程グループ・規格管理・流通行程管理"
        :is-show-breadcrumbs="true" :breadcrumbs-item="[{ path: '', label: pageTitle }]" />
    <div class="content_bg">
        <div class="content_wrap">
            <Breadcrumbs :breadcrumbs-item="[{ path: '#', label: pageTitle }]" />
            <div class="content_body">
                <table class="basic_table product_info">
                    <tr>
                        <th>農作物コード</th>
                        <td>
                            <AdminInput name="cropCode" input_mode="select" :options="cropCode" :is-default-item="true" default-label="プルダウンから選択" @emit-value="receiveValue" />
                        </td>
                    </tr>
                    <tr>
                        <th>種類コード</th>
                        <td>
                            <AdminInput name="typeCode" input_mode="select" :options="typeCode" :is-default-item="true" default-label="プルダウンから選択" @emit-value="receiveValue" />
                        </td>
                    </tr>
                    <tr>
                        <th>品種コード</th>
                        <td>
                            <AdminInput name="varietyCode" input_mode="select" :options="varietyCode" :is-default-item="true" default-label="プルダウンから選択" @emit-value="receiveValue" />
                        </td>
                    </tr>
                    <tr>
                        <th>系統コード</th>
                        <td>
                            <AdminInput name="lineageCode" input_mode="select" :options="lineageCode" :is-default-item="true" default-label="プルダウンから選択" @emit-value="receiveValue" />
                        </td>
                    </tr>
                    <tr>
                        <th>JAS判定に適用する<br />流通行程管理基準等</th>
                        <td>
                            <AdminInput name="management_criteria" input_mode="textarea" @emit-value="receiveValue" />
                        </td>
                    </tr>
                    <tr>
                        <th>登録認証機関</th>
                        <td>
                            <AdminInput name="certification_body" input_mode="text" @emit-value="receiveValue" />
                        </td>
                    </tr>
                    <tr>
                        <th style="vertical-align: top;">流通行程管理基準の説明</th>
                        <td>
                            <DistributionProcessExplainInputWrap />
                        </td>
                    </tr>
                    </table>
            </div>
            <div class="application_sample_wrap">
                <a href="#" target="_blank">申請書のサンプルはこちら</a>
            </div>
            <div class="btn_wrap">
                <LongBtn label="確認画面にすすむ" btn-type="black" @click="clickConfirmBtn" />
            </div>
        </div>
    </div>
    <AdminFooter />
</template>

<style scoped lang="scss">
.content_bg {
    display: flex;
    flex-direction: column;
    justify-content: center;
    width: 100%;
    background: var(--main-bg-color);
}

.content_wrap {
    // margin: 0 auto;
    // display: flex;
    // flex-direction: column;
    // width: 100%;
    // height: 100%;
    // min-height: calc(100vh - 202px);
    // max-width: var(--admin-max-width);

    h2 {
        margin: 67px auto 66px;
        font-size: 26px;
        font-weight: 600;
        color: var(--font-color);
    }

    .content_body {
        padding: 101.3px 10.4% 0px;

        .basic_table {
            border-collapse: collapse;
            width: 100%;
            margin-bottom: 106.5px;
            line-height: 30px;

            &:last-child {
                margin-bottom: 0px;
            }

            tr {
                // border-bottom: 2px solid #DBDBDB;
                &:first-child th {
                    border-top: 13px solid var(--allow-orange-color);
                }
                &:first-child td {
                    padding-top: 20px;
                }

                &:last-child {
                    border-bottom: none;
                }

                th {
                    width: 250px;
                    background: var(--input-btn-color);
                    font-size: 16px;
                    font-weight: 600;
                    color: var(--white-color);
                    text-align: left;
                    padding: 16.6px 24px;
                    border-bottom: 2px solid #DBDBDB;
                }

                td {
                    background: var(--white-color);
                    padding: 8px 38px;
                    text-align: left;
                    font-size: 16px;
                    font-weight: 300;
                    color: var(--font-color);

                }
            }
        }
    }

    .application_sample_wrap {
        margin-top: 88.7px;
        a {
            font-size: 16px;
            color: var(--notice-font-color);
        }
    }

    .btn_wrap {
        flex: 1;
        display: flex;
        align-items: center;
        justify-content: space-between;
        width: 40%;
        margin: 0 auto;

        &>div:first-child {
            margin-right: 20px;
        }
    }
}
</style>