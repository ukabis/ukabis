<!-- AdminConfirmationView -->
<script setup lang="ts">
import { storeToRefs } from 'pinia'

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import Breadcrumbs from '@/components/AdminCommon/Breadcrumbs.vue'
import LongBtn from '@/components/AdminCommon/LongBtn.vue'

// Sample Data
import admin_confirmation from '@/sample_data/admin_confirmation.json'

// Store
import { StaffInfo, UseData } from '@/stores/data'
import { UseUtil } from '@/stores/util'
import { UseApi } from '@/stores/api'

// Data
const useData = UseData()
const { newRegistration } = storeToRefs(useData)

// Util
const util = UseUtil()

// API
const api = UseApi()
const { groupRegisterItem } = storeToRefs(api)

// Func
const getNameWithTitle = (data: StaffInfo | null) => {
    return data?.companyName + " " + data?.officeName + " " + data?.staffName
}

const registrationConfirmData = {
    groupName: newRegistration.value.groupName,
    groupRepresentative: getNameWithTitle(newRegistration.value.groupRepresentative),
    systemUser: newRegistration.value.systemUser,
    distributionResponsible: newRegistration.value.distributionResponsible.map(getNameWithTitle).join(" / "),
    distributionChargeList: newRegistration.value.distributionChargeList.map(getNameWithTitle).join(" / "),
    ratingResponsible: newRegistration.value.ratingResponsible.map(getNameWithTitle).join(" / "),
    ratingChargeList: newRegistration.value.ratingChargeList.map(getNameWithTitle).join(" / "),
}

// Page Title
const pageTitle = "新規グループ登録確認"

const clickModification = () => {
    util.transition("/admin/newregistration")
}
const clickRegistration = () => {
    // api送信

    // api送信後
    useData.resetNewRegistration()
    util.transition("/admin/confirmed")
}

const setGroupRegisterItem = () => {
    // groupRegisterItem.value.groupName = newRegistration.value.groupName
    // groupRegisterItem.value.manager = ""
    // groupRegisterItem.value.CompanyId =""
    // groupRegisterItem.value.JasGroup.push({
    //         "CompanyId": "",
    //         "OfficeId": [],
    //         "MemberOpenId": "",
    //         "JasRoleType": ""
    // })
}

</script>

<template>
    <AdminHeader :is-show-sub-menu="true" :is-show-bottom-wrap="true" bottom-wrap-label="流通行程グループ・規格管理・流通行程管理"
        :is-show-breadcrumbs="true" :breadcrumbs-item="[{ path: '', label: pageTitle }]" />
    <div class="content_bg">
        <div class="content_wrap">
            <Breadcrumbs :breadcrumbs-item="[{ path: '#', label: pageTitle }]" />
            <h2>{{ pageTitle }}</h2>
            <div class="content_body">
                <table>
                    <tr>
                        <th>グループ名</th>
                        <td>
                            {{ registrationConfirmData.groupName }}
                        </td>
                    </tr>
                    <tr>
                        <th>グループ機関代表者</th>
                        <td>
                            {{ registrationConfirmData.groupRepresentative }}
                        </td>
                    </tr>
                    <tr>
                        <th>システム利用代表者</th>
                        <td>
                            {{ registrationConfirmData.systemUser }}
                        </td>
                    </tr>
                    <tr>
                        <th>流通行程管理責任者</th>
                        <td>
                            {{ registrationConfirmData.distributionResponsible }}
                        </td>
                    </tr>
                    <tr>
                        <th>流通行程管理担当者</th>
                        <td>
                            {{ registrationConfirmData.distributionChargeList }}
                        </td>
                    </tr>
                    <tr>
                        <th>格付責任者</th>
                        <td>
                            {{ registrationConfirmData.ratingResponsible }}
                        </td>
                    </tr>
                    <tr>
                        <th>格付担当者</th>
                        <td>
                            {{ registrationConfirmData.ratingChargeList }}
                        </td>
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
        padding: 0px 10.4%;

        table {
            border-collapse: collapse;
            width: 100%;

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
                    padding: 25px 24px;
                    border-bottom: 2px solid #DBDBDB;
                }

                // &:first-child th {
                //     border-top: 13px solid var(--allow-orange-color);
                // }

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