<!-- AdminJasPackageConfirmationView -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import Breadcrumbs from '@/components/AdminCommon/Breadcrumbs.vue'
import LongBtn from '@/components/AdminCommon/LongBtn.vue'
import DistributionProcessExplainConfirmWrap from '@/components/AdminJasPackageConfirmationComp/DistributionProcessExplainConfirmConfirmWrap.vue'

// Sample Data
import admin_confirmation from '@/sample_data/admin_confirmation.json'

// Store
import { UseData } from '@/stores/data'
import { UseUtil } from '@/stores/util'

const util = UseUtil()
const useData = UseData()
const { adminJasPackageData } = storeToRefs(useData)

// 商品情報
const cropCode = adminJasPackageData.value.cropCode
const typeCode = adminJasPackageData.value.typeCode
const varietyCode = adminJasPackageData.value.varietyCode
const lineageCode = adminJasPackageData.value.lineageCode
const managementCriteria = adminJasPackageData.value.management_criteria
const certificationBody = adminJasPackageData.value.certification_body

const pageTitle = "グループ詳細確認"

const clickModification = () => {
    util.transition("/admin/jaspackage")
}
const clickRegistration = () => {
    // api送信

    // api送信後
    useData.resetAdminJasPackageData()
    util.transition("/admin/jaspackagecomplete")
}

</script>

<template>
    <AdminHeader :is-show-sub-menu="true" :is-show-bottom-wrap="true" bottom-wrap-label="流通行程グループ・規格管理・流通行程管理"
        :is-show-breadcrumbs="true" :breadcrumbs-item="[{ path: '', label: pageTitle }]" />
    <div class="content_bg">
        <div class="content_wrap">
            <Breadcrumbs :breadcrumbs-item="[{path: '#', label: pageTitle}]" />
            <div class="content_body">
                <table class="basic_table product_info">
                    <tr>
                        <th>農作物コード</th>
                        <td>{{ cropCode }}</td>
                    </tr>
                    <tr>
                        <th>種類コード</th>
                        <td>{{ typeCode }}</td>
                    </tr>
                    <tr>
                        <th>品種コード</th>
                        <td>{{ varietyCode }}</td>
                    </tr>
                    <tr>
                        <th>系統コード</th>
                        <td>{{ lineageCode }}</td>
                    </tr>
                    <tr>
                        <th>JAS判定に適用する<br />流通行程管理基準等</th>
                        <td>{{ managementCriteria }}</td>
                    </tr>
                    <tr>
                        <th>登録認証機関</th>
                        <td>{{ certificationBody }}</td>
                    </tr>
                    <tr>
                        <th style="vertical-align: top;">流通行程管理基準の説明</th>
                        <td><DistributionProcessExplainConfirmWrap /></td>
                    </tr>
                </table>
            </div>
            <div class="btn_wrap">
                <LongBtn label="内容修正" btn-type="white" @click-btn="clickModification" />
                <LongBtn label="規格登録にすすむ" btn-type="black" @click-btn="clickRegistration" />
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
                border-bottom: 2px solid #DBDBDB;

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
                    padding: 0px 38px;
                    text-align: left;
                    font-size: 16px;
                    font-weight: 300;
                    color: var(--font-color);

                }
            }
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