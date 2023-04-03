<!-- AdminHomeView -->
<script setup lang="ts">
import { UseUtil } from '@/stores/util'

// Component
import AdminHeader from '@/components/AdminCommon/AdminHeader.vue'
import AdminFooter from '@/components/AdminCommon/AdminFooter.vue'
import Breadcrumbs from '@/components/AdminCommon/Breadcrumbs.vue'
import AdminContentHead from '@/components/AdminCommon/AdminContentHead.vue'

// Sample
import group_data from '@/sample_data/group_data.json'
import Pagination from '@/components/AdminCommon/Pagination.vue'

// Group Data API
const getGroupData = () => {
    return group_data
}

// Util
const util = UseUtil()
const clickGroupName = () => {
    console.log("clickGroupName!")
    util.transition("/admin/groupdetail")
}

// clickFormCheck
const clickFormCheck = () => {
    console.log("clickFormCheck!")
}

</script>

<template>
    <AdminHeader :is-show-sub-menu="true" :is-show-bottom-wrap="true" bottom-wrap-label="流通行程グループ・規格管理・流通行程管理"
        :is-show-breadcrumbs="true" />
    <div class="content_bg">
        <div class="content_wrap">
            <Breadcrumbs />
            <AdminContentHead :is-show-register-group-btn="true" />
            <div class="admin_content body">
                <table>
                    <tr>
                        <th>グループ名</th>
                        <th>品目名</th>
                        <th>流通行程管理責任者</th>
                        <th>格付責任者</th>
                        <th>流通行程管理</th>
                    </tr>
                    <tr v-for="item in getGroupData()">
                        <td><span class="cursor_pointer underline" @click="clickGroupName">{{ item.group_name }}</span></td>
                        <td><span class="">{{ item.item_name }}</span></td>
                        <td><span class="">{{ item.distribution_manager }}</span></td>
                        <td><span class="">{{ item.grading_person }}</span></td>
                        <td><span class="check_form_btn cursor_pointer" @click="clickFormCheck">帳票を確認する</span></td>
                    </tr>
                </table>
            </div>
            <div class="admin_content foot">
                <Pagination />
            </div>
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
    .admin_content {
        width: 100%;
        max-width: var(--admin-max-width);
        padding: 0px 10.4%;

        &.body {
            padding-top: 17.6px;

            table {
                width: 100%;
                border-collapse: collapse;

                tr {
                    th {
                        text-align: left;
                        padding-left: 7.1px;
                        height: 26px;
                        background: var(--input-btn-color);
                        font-size: 14px;
                        font-weight: 300;
                        color: var(--white-color);
                        border-right: 1px solid var(--white-color);
                    }

                    border: solid var(--main-bg-color);
                    border-width: 3px 3px 3px 3px;

                    &:first-child {
                        border-width: 0px;
                    }

                    td {
                        text-align: left;
                        padding-left: 7.1px;
                        height: 60px;
                        background: var(--white-color);
                        font-size: 16px;
                        font-weight: 300;
                        color: var(--font-color);
                        width: 21.8%;

                        &:first-child {
                            padding-left: 13px;
                            // color: #3E61B1;
                            // text-decoration: underline;
                        }

                        &:last-child {
                            text-align: center;
                            padding-right: 7.1px;
                        }
                    }
                }
            }
        }

        &.foot {
            padding-bottom: 20px;
        }
    }
}

.check_form_btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 58%;
    max-width: 170px;
    min-width: 150px;
    max-height: 35px;
    font-size: 16px;
    font-weight: 300;
    color: var(--input-btn-color);
    border: 2px solid var(--input-btn-color);
    border-radius: 5px;
    padding-right: 0px;
    margin: 0 auto;

    &::after {
        content: url("/src/assets/Admin/common/check_form_btn_arrow.svg");
        position: relative;
        top: -0.6px;
        right: -8px;
    }
}
</style>