<script setup lang="ts">
// Util
import { onMounted, ref } from 'vue'
import { UseData } from '@/stores/data'
import { storeToRefs } from 'pinia'

// Component
import AdminInput from '@/components/AdminCommon/AdminInput.vue'

// API
import { UseApi } from '@/stores/api'
import { getOfficeList } from '@/api/company/getOffice'
import { getStaff } from '@/api/company/getStaff'

// Data
const useData = UseData()
const { newRegistration } = storeToRefs(useData)

// API
const api = UseApi()
const { companyObj, officeObj, staffObj } = storeToRefs(api)

// Props
const props = defineProps<{
    name: string
    num?: number
    options: [string, string][] | [string, string, string][] | undefined
    isAddRowBtn: boolean
}>()

onMounted(() => {
    // Get OfficeList
    // getOfficeList(api.accessToken, api.openId)
    //     .then(data => {
    //         officeObj.value = data?.map(({ CompanyId, OfficeName }) => ([CompanyId, OfficeName]))
    //         console.log({
    //             "officeObj": officeObj.value
    //         })
    //     })

    // Get StaffList
    // getStaff(api.accessToken, api.openId)
    //     .then(data => {
    //         staffObj.value = data?.map(({ Offices, StaffName }) => ([Offices, StaffName]))
    //     })

})

const targetOffices = ref<[string, string, string][]>()
const targetStaffs = ref<[string, string, string[]][]>()

// Func
const setValueToNewRegistration = (name: string, value: string, type: string, num?: number) => {
    // newRegistration

    switch (name) {
        case "groupRepresentative":
            switch (type) {
                case "companyName":
                    newRegistration.value.groupRepresentative.companyId = value
                    break;
                case "officeName":
                    newRegistration.value.groupRepresentative.officeId = value
                    break;
                case "staffName":
                    newRegistration.value.groupRepresentative.staffId = value
                    break;
                default:
                    break;
            }
            break
        case "distributionResponsible":
            if (!num) return
            switch (type) {
                case "companyName":
                    newRegistration.value.distributionResponsible[num - 1].companyId = value
                    break;
                case "officeName":
                    newRegistration.value.distributionResponsible[num - 1].officeId = value
                    break;
                case "staffName":
                    newRegistration.value.distributionResponsible[num - 1].staffId = value
                    break;
                default:
                    break;
            }
            break
        case "distributionChargeList":
            if (!num) return
            switch (type) {
                case "companyName":
                    newRegistration.value.distributionChargeList[num - 1].companyId = value
                    break;
                case "officeName":
                    newRegistration.value.distributionChargeList[num - 1].companyId = value
                    break;
                case "staffName":
                    newRegistration.value.distributionChargeList[num - 1].companyId = value
                    break;
                default:
                    break;
            }
            break
        case "ratingResponsible":
            if (!num) return
            switch (type) {
                case "companyName":
                    newRegistration.value.ratingResponsible[num - 1].companyId = value
                    break;
                case "officeName":
                    newRegistration.value.ratingResponsible[num - 1].companyId = value
                    break;
                case "staffName":
                    newRegistration.value.ratingResponsible[num - 1].companyId = value
                    break;
                default:
                    break;
            }
            break
        case "ratingChargeList":
            if (!num) return
            switch (type) {
                case "companyName":
                    newRegistration.value.ratingChargeList[num - 1].companyId = value
                    break;
                case "officeName":
                    newRegistration.value.ratingChargeList[num - 1].companyId = value
                    break;
                case "staffName":
                    newRegistration.value.ratingChargeList[num - 1].companyId = value
                    break;
                default:
                    break;
            }
            break
        default:
            break
    }
}

// Ref
const companyRef = ref()
const officeRef = ref()
const staffRef = ref()

// resetSelectValue
const resetSelectValue = (name: string) => {
    console.log({
        "companyList": companyRef.value,
        "officeList": officeRef.value,
        "staffList": staffRef.value,
    })
    officeRef.value.sampleFunc()
}

// Emit Value Func
const changeCompany = (companyId: string, name: string) => {
    console.log("run changeCompany")
    console.log({
        "name": name,
        "companyId": companyId,
        "num": props.num
    })

    // setValueToNewRegistration(name, companyId, "companyId")

    // 選択されたcompanyIdをもつOfficeリストを取得し更新
    // targetOffices.value = officeObj.value?.filter(item => item[2] == companyId)

    // resetSelectValue
    resetSelectValue(name)
}
const changeOffice = (officeId: string, name: string) => {
    console.log("run changeOffice")
    console.log({
        "name": name,
        "officeId": officeId,
        "num": props.num
    })

    // 選択されたOfficeIDをもつStaffリストを取得し更新（ただし、Staffは複数のOfficeIDを持つ可能性がある）
    // targetStaffs.value = staffObj.value?.filter(staffItem => staffItem[2].includes(officeId))

    console.log({
        "targetStaffs.value:": targetStaffs.value
    })
}
const changeStaff = (staffId: string, name: string) => {
    console.log("run changeStaff")
    console.log({
        "name": name,
        "staffId": staffId,
        "num": props.num
    })
    // targetOffices.value = officeObj.value?.filter(item => item[0] == inputValue)
    // console.log("targetOffices:", targetOffices.value)
}

// CLick AddRow Btn
const clickAddRowBtn = () => {
    emits("clickAddRowBtn", props.name, props.num)
}

// Define Emits
const emits = defineEmits<{
    (e: "emitValue", value: string, name?: string): void
    (e: "clickAddRowBtn", name: string, num?: number): void
}>()

/*
DefineEmitsとしては、
changeCompany
changeOffice
changeStaff
の3つ揃った状態 + props.nameを返す

{
    name: props.name,
    body: {
        company: 〜〜
        office: 〜〜
        staff: 〜〜
    }
}

*/

</script>

<template>
    <div class="td_content">
        <AdminInput ref="companyRef" :name="props.name" option-name="companyList" input_mode="select"
            :is-default-item="true" default-label="事業会社名選択" :options="props.options" @emit-value="changeCompany" />
        <AdminInput ref="officeRef" :name="props.name" option-name="officeList" input_mode="select" :is-default-item="true"
            default-label="事業所名選択" :options="targetOffices" @emit-value="changeOffice" />
        <AdminInput ref="staffRef" :name="props.name" option-name="staffList" input_mode="select" :is-default-item="true"
            default-label="所属スタッフ名" :options="targetStaffs" @emit-value="changeStaff" />
        <span v-if="isAddRowBtn" class="plus_btn" @click="clickAddRowBtn">
            <img src="/src/assets/Admin/common/plus_btn.svg" alt="枠を追加">
        </span>
    </div>
</template>

<style scoped lang="scss">
.td_content {
    display: flex;
    justify-content: stretch;
    padding-right: 60px;
    position: relative;

    &>.admin_input_wrap {
        margin-right: 15px;
    }

    .plus_btn {
        cursor: pointer;
        position: absolute;
        top: 5px;
        right: 29px;
    }

    &+.td_content {
        margin-top: 20px;
    }
}
</style>
