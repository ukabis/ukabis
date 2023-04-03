<!-- AdminGroupDetailView -->
<script setup lang="ts">

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import Breadcrumbs from '@/components/AdminCommon/Breadcrumbs.vue'
import LongBtn from '@/components/AdminCommon/LongBtn.vue'
import DistributionProcessExplainWrap from '@/components/AdminGroupDetailComp/DistributionProcessExplainWrap.vue'

// Sample Data
import admin_confirmation from '@/sample_data/admin_confirmation.json'
import admin_group_detail from '@/sample_data/admin_group_detail.json'

// Store
import { UseUtil } from '@/stores/util'

// Util
const util = UseUtil()

// グループ情報取得API（サンプル）
const getGroupDetailApi = () => {
    return {
        groupName: admin_confirmation.groupName, // グループ名
        groupRepresentative: admin_confirmation.groupRepresentative, // グループ機関代表者
        systemUser: admin_confirmation.systemUser, // システム利用代表者
        distributionResponsible: admin_confirmation.distributionResponsible.join(" / "), // 流通行程管理責任者
        distributionChargeList: admin_confirmation.distributionChargeList.join(" / "), // 流通行程管理担当者
        ratingResponsible: admin_confirmation.ratingResponsible.join(" / "), // 格付責任者
        ratingChargeList: admin_confirmation.ratingChargeList.join(" / "), // 格付担当者
    }
}
const getItemDetailApi = () => {
    return {
        cropCode: admin_group_detail.crop_code,
        typeCode: admin_group_detail.type_code,
        varietyCode: admin_group_detail.variety_code,
        lineageCode: admin_group_detail.lineage_code,
        managementCriteria: admin_group_detail.management_criteria,
        certificationBody: admin_group_detail.certification_body,
    }
}

// 各情報へ代入
const {
    groupName,
    groupRepresentative,
    systemUser,
    distributionResponsible,
    distributionChargeList,
    ratingResponsible,
    ratingChargeList
} = getGroupDetailApi()

// 商品情報（サンプル）
const {
    cropCode,
    typeCode,
    varietyCode,
    lineageCode,
    managementCriteria,
    certificationBody
} = getItemDetailApi()

const clickToHomeBtn = () => {
    util.transition("/admin/home")
}

// Page Title
const pageTitle = "グループ詳細確認"

</script>

<template>
    <AdminHeader :is-show-sub-menu="true" :is-show-bottom-wrap="true" bottom-wrap-label="流通行程グループ・規格管理・流通行程管理"
        :is-show-breadcrumbs="true" :breadcrumbs-item="[{ path: '', label: pageTitle }]" />
    <div class="content_bg">
        <div class="content_wrap">
            <Breadcrumbs :breadcrumbs-item="[{ path: '#', label: pageTitle }]" />
            <div class="content_body">
                <table class="basic_table group_info">
                    <tr>
                        <th>グループ名</th>
                        <td>{{ groupName }}</td>
                    </tr>
                    <tr>
                        <th>グループ機関代表者</th>
                        <td>{{ groupRepresentative }}</td>
                    </tr>
                    <tr>
                        <th>システム利用代表者</th>
                        <td>{{ systemUser }}</td>
                    </tr>
                    <tr>
                        <th>流通行程管理責任者</th>
                        <td>{{ distributionResponsible }}</td>
                    </tr>
                    <tr>
                        <th>流通行程管理担当者</th>
                        <td>{{ distributionChargeList }}</td>
                    </tr>
                    <tr>
                        <th>格付責任者</th>
                        <td>{{ ratingResponsible }}</td>
                    </tr>
                    <tr>
                        <th>格付担当者</th>
                        <td>{{ ratingChargeList }}</td>
                    </tr>
                </table>
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
                        <td>
                            <DistributionProcessExplainWrap />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="btn_wrap">
                <LongBtn label="ホームへ" btn-type="white" @click="clickToHomeBtn" />
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