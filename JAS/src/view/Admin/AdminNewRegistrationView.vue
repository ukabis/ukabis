<!-- AdminNewRegistrationView -->
<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import Breadcrumbs from '@/components/AdminCommon/Breadcrumbs.vue'
import AdminInput from '@/components/AdminCommon/AdminInput.vue'
import LongBtn from '@/components/AdminCommon/LongBtn.vue'
import NewRegistRow from '@/components/AdminCommon/NewRegistRow_old.vue'
import NewRegistRow_New from '@/components/AdminCommon/NewRegistRow.vue'

// Store
import { UseData } from '@/stores/data'

// Util
import { UseUtil } from '@/stores/util'

// Sample
import sample_companyList from '@/sample_data/sample_companyList.json'
import sample_officeList from '@/sample_data/sample_officeList.json'
import sample_staffList from '@/sample_data/sample_staffList.json'

// API
import { UseApi } from '@/stores/api'
import { preRequest } from '@/api/common/preRequest'
import { Company, getCompanyList } from '@/api/company/getCompany'
import { Office, getOfficeList } from '@/api/company/getOffice'
import { checkAuthenticated } from '@/api/common/checkAuthenticated'

// Page Title
const pageTitle = "新規グループ登録"

// Util
const util = UseUtil()
const { newRegistRowNum } = storeToRefs(util)

// Data
const useData = UseData()
const { newRegistration } = storeToRefs(useData)

// API
const api = UseApi()
const { accessToken, openId, companyObj, officeObj, staffObj } = storeToRefs(api)

// Func
const clickConfirmBtn = () => {
    console.log(newRegistration.value)
    util.transition("/admin/confirmation")
}

// Add Row
const clickAddRow = (name: string) => {
    if (!(
        name == "distributionResponsible" ||
        name == "distributionChargeList" ||
        name == "ratingResponsible" ||
        name == "ratingChargeList"
    )) return
    newRegistRowNum.value[name] += 1
    newRegistration.value[name][newRegistRowNum.value[name] - 1] = {
        companyId: "hidden",
        officeId: "hidden",
        staffId: "hidden",
        companyName: "",
        officeName: "",
        staffName: "",
    }
}

// Change GroupName
const changeGroupName = (value: string, name: string) => {
    newRegistration.value.groupName = value
}

const companyRawDatas = ref<Company[]>([])
const officeRawDatas = ref<Office[]>([])

onMounted(() => {
    // Companyデータ取得
    // preRequest()
    //     .then(async (res) => {
    //         accessToken.value = res.access_token
    //         openId.value = res.openid_token
    //         companyRawDatas.value = await getCompanyList(accessToken.value, openId.value)
    //         companyObj.value = companyRawDatas.value.map(({ CompanyId, CompanyName }) => ({
    //             companyId: CompanyId,
    //             companyName: CompanyName,
    //         }))
    //         officeRawDatas.value = await getOfficeList(accessToken.value, openId.value)
    //         officeObj.value = officeRawDatas.value.map(({CompanyId, OfficeId, OfficeName}) => ({
    //             officeId: OfficeId,
    //             officeName: OfficeName,
    //             companyId: CompanyId,
    //         }))
    //     })
    //     .catch(err => console.error(err))


    companyObj.value = sample_companyList
    officeObj.value = sample_officeList
    newRegistration.value.systemUser = "海野 太郎"
})

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
                            <AdminInput name="groupName" input_mode="text" :value="newRegistration.groupName"
                                placeholder="グループ名を入力してください。" @emit-value="changeGroupName" />
                        </td>
                    </tr>
                    <tr>
                        <th>グループ機関代表者</th>
                        <td>
                            <NewRegistRow_New name="groupRepresentative" :company-opts="companyObj" :office-opts="officeObj"
                                :is-show-add-row="false" :value="newRegistration.groupRepresentative" />
                        </td>
                    </tr>
                    <tr>
                        <th>システム利用代表者</th>
                        <td>{{ newRegistration.systemUser }}</td>
                    </tr>
                    <tr>
                        <th>流通行程管理責任者</th>
                        <td>
                            <NewRegistRow_New name="distributionResponsible"
                                v-for="n in newRegistRowNum.distributionResponsible" :num="n"
                                :is-show-add-row="n == newRegistRowNum.distributionResponsible ? true : false"
                                :company-opts="companyObj" :office-opts="officeObj"
                                @click-add-row="clickAddRow('distributionResponsible')"
                                :value="newRegistration.distributionResponsible[n - 1]" />
                        </td>
                    </tr>
                    <tr>
                        <th>流通行程管理担当者</th>
                        <td>
                            <NewRegistRow_New name="distributionChargeList"
                                v-for="n in newRegistRowNum.distributionChargeList" :num="n"
                                :is-show-add-row="n == newRegistRowNum.distributionChargeList ? true : false"
                                :company-opts="companyObj" :office-opts="officeObj"
                                @click-add-row="clickAddRow('distributionChargeList')"
                                :value="newRegistration.distributionChargeList[n - 1]" />
                        </td>
                    </tr>
                    <tr>
                        <th>格付責任者</th>
                        <td>
                            <NewRegistRow_New name="ratingResponsible" v-for="n in newRegistRowNum.ratingResponsible"
                                :num="n" :is-show-add-row="n == newRegistRowNum.ratingResponsible ? true : false"
                                :company-opts="companyObj" :office-opts="officeObj"
                                @click-add-row="clickAddRow('ratingResponsible')"
                                :value="newRegistration.ratingResponsible[n - 1]" />
                        </td>
                    </tr>
                    <tr>
                        <th>格付担当者</th>
                        <td>
                            <NewRegistRow_New name="ratingChargeList" v-for="n in newRegistRowNum.ratingChargeList" :num="n"
                                :is-show-add-row="n == newRegistRowNum.ratingChargeList ? true : false"
                                :company-opts="companyObj" :office-opts="officeObj"
                                @click-add-row="clickAddRow('ratingChargeList')"
                                :value="newRegistration.ratingChargeList[n - 1]" />
                        </td>
                    </tr>
                </table>
            </div>
            <LongBtn label="確認画面にすすむ" btn-type="black" @click-btn="clickConfirmBtn" />
        </div>
    </div>
    <AdminFooter />
</template>

<style scoped lang="scss">
.content_bg {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: center;
    width: 100%;
    background: var(--main-bg-color);
}

.content_wrap {

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

                &:first-child th {
                    border-top: 13px solid var(--allow-orange-color);
                }

                td {
                    background: var(--white-color);
                    padding: 10px 38px;
                    text-align: left;
                    font-size: 16px;
                    font-weight: 300;
                    color: var(--font-color);
                }
            }
        }
    }
}</style>