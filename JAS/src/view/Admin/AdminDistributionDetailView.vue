<!-- AdminDistributionDetailView -->
<script setup lang="ts">
import { ref } from 'vue'

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import Breadcrumbs from '@/components/AdminCommon/Breadcrumbs.vue'
import LongBtn from '@/components/AdminCommon/LongBtn.vue'

// Sample Data
import admin_confirmation from '@/sample_data/admin_confirmation.json'
import admin_distribution_detail from '@/sample_data/admin_distribution_detail.json'

// Store
import { UseUtil } from '@/stores/util'

// Util
const util = UseUtil()

// グループ情報（サンプル）
const groupName = admin_confirmation.groupName
const groupRepresentative = admin_confirmation.groupRepresentative
const distributionResponsible = admin_confirmation.distributionResponsible
const ratingChargeList = admin_confirmation.ratingChargeList

// PageTitle
const pageTitle = "グループ詳細確認"

// clickBtn
const clickToListBtn = () => {
    util.transition("/admin/distributionlist")
}
const clickTohomeBtn = () => {
    util.transition("/admin/home")
}

// Distribution Process
const distributionProcessLength = ref<number>(0)
distributionProcessLength.value = admin_distribution_detail.distribution_process.length

// Breadcrumbs
const breadcrumbs = [
    {
        path: "/admin/distributionlist",
        label: "流通工程管理"
    },
    {
        path: "",
        label: "詳細"
    },
]

</script>

<template>
    <AdminHeader :is-show-sub-menu="true" :is-show-bottom-wrap="true" bottom-wrap-label="流通行程グループ・規格管理・流通行程管理"
        :is-show-breadcrumbs="true" :breadcrumbs-item="[{ path: '', label: pageTitle }]" />
    <div class="content_bg">
        <div class="content_wrap">
            <Breadcrumbs :breadcrumbs-item="breadcrumbs" />
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
                        <th>流通行程管理責任者</th>
                        <td>{{ distributionResponsible }}</td>
                    </tr>
                    <tr>
                        <th>格付担当者</th>
                        <td>{{ ratingChargeList }}</td>
                    </tr>
                </table>
                <table class="basic_table distribution_detai">
                    <tr>
                        <th rowspan="4">⽣産・出荷者</th>
                        <td>
                            <span class="subtitle">⽣産者名</span>
                            {{ admin_distribution_detail.resident_name }}
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="subtitle">住所</span>
                            {{ admin_distribution_detail.resident_address }}
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="subtitle">連絡先</span>
                            {{ admin_distribution_detail.resident_contact }}
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="subtitle">その他の情報</span>
                            {{ admin_distribution_detail.resident_information }}
                        </td>
                    </tr>
                    <tr>
                        <th rowspan="4">販売事業者（流通⾏程管理者）</th>
                        <td>
                            <span class="subtitle">事業者名</span>
                            {{ admin_distribution_detail.business_name }}
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="subtitle">住所</span>
                            {{ admin_distribution_detail.business_address }}
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="subtitle">連絡先</span>
                            {{ admin_distribution_detail.business_contact }}
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span class="subtitle">その他の情報</span>
                            {{ admin_distribution_detail.business_information }}
                        </td>
                    </tr>
                    <tr v-for="item, index in admin_distribution_detail.distribution_process">
                        <th :rowspan="distributionProcessLength" v-if="index == 0">流通⾏程</th>
                        <td>
                            <span class="subtitle">{{ item.company_name_label }}</span>
                            {{ item.shipment_date }}
                        </td>
                    </tr>
                </table>
            </div>
            <div class="btn_wrap">
                <LongBtn label="一覧に戻る" btn-type="white" @click="clickToListBtn" />
                <LongBtn label="ホームへ" btn-type="white" @click="clickTohomeBtn" />
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
                    padding: 16.6px 0px 16.6px 24px;
                    border-bottom: 2px solid #DBDBDB;
                    vertical-align: top;
                }

                td {
                    background: var(--white-color);
                    padding: 0px 38px;
                    text-align: left;
                    font-size: 16px;
                    font-weight: 300;
                    color: var(--font-color);
                    min-height: 64.19px;

                    .subtitle {
                        background: #FFF8E5;
                        display: flex;
                        width: 250px;
                        align-items: center;
                        padding: 23px 0px 23px 35px;
                        margin-right: 40px;
                    }
                }
            }

            &.distribution_detai {
                td {
                    padding-left: 0px;
                    display: flex;
                    align-items: center;
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