<script setup lang="ts">
import { UseUtil } from '@/stores/util'
const props = defineProps<{
    breadcrumbsItem?: BreadcrumbsItem[]
}>()

type BreadcrumbsItem = {
    path: string
    label: string
}

const util = UseUtil()
const clickBreadCrumbItem = () => {
    util.transition("/admin/home")
}

</script>

<template>
    <div class="breadcrumbs_wrap">
        <ul class="breadcrumbs">
            <li class="cursor_pointer" @click="clickBreadCrumbItem">HOME</li>
            <li class="cursor_pointer" v-for="item in breadcrumbsItem" @click="() => { util.transition(item.path) }">
                {{ item.label }}
            </li>
        </ul>
    </div>
</template>

<style scoped lang="scss">
.breadcrumbs_wrap {
    width: 100%;
    max-width: var(--admin-max-width);
    margin: 0px auto;
    padding: 20px 10.4% 20px;
    display: flex;
    background: var(--main-bg-color);

    .breadcrumbs {
        display: flex;
        flex-wrap: nowrap;
        align-items: center;

        li {
            display: flex;
            align-items: center;
            position: relative;
            margin-right: 25.9px;
            font-size: 14px;
            font-weight: 600;

            &:first-child::before {
                content: url("/src/assets/Admin/common/home.svg");
                position: relative;
                top: 1px;
                left: 0px;
                margin-right: 7.1px;
            }

            &:last-child {
                margin-right: 0px;
            }

            &::after {
                content: url("/src/assets/Admin/common/breadcrumb_arrow.svg");
                position: relative;
                right: -13px;
            }

            &:last-child::after {
                display: none;
            }
        }
    }
}
</style>