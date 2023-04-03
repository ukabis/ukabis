<!-- Header -->
<script setup lang="ts">
import { ref } from 'vue'

// Component
import Menu from '@/components/common/Menu.vue'
import { UseUtil } from '@/stores/util'

const util = UseUtil()

// Props
const props = defineProps<{
    titleLabel: string
    lineType?: string
}>()

// Logo
const clickLogo = () => {
    util.transition("/home")
}
// Menu
const isMenuOpen = ref<boolean>(false)
const clickMenu = () => {
    isMenuOpen.value = true
}
const clicClose = () => {
    isMenuOpen.value = false
}



// LineColor
const getLineColor = (lineType?: string) => {
    switch (lineType) {
        case "shipment":
            return " shipment"
        case "incoming":
            return " incoming"
        case "progress":
            return " progress"
        default:
            return
    }
}

</script>

<template>
    <div class="header_wrap">
        <Menu v-if="isMenuOpen" @click-close="clicClose" />
        <div class="header_top_bg">
            <div class="header_top inner">
                <span @click="clickLogo"><img src="/src/assets/common/logo.svg" alt="ukabisロゴ" width="123" height="31"></span>
                <span @click="clickMenu"><img src="/src/assets/common/burger_menu.svg" alt="バーガーメニュー" width="26"></span>
            </div>
        </div>
        <div class="title_label_bg" :class="getLineColor(lineType)">
            <div class="title_label inner">
                {{ titleLabel }}
            </div>
        </div>
    </div>
</template>

<style scoped lang="scss">
.header_wrap {
    width: 100%;
    position: fixed;
    top: 0px;
    background: var(--white-color);
    z-index: 10000;
}

.header_top_bg {
    display: flex;
    align-items: end;
    justify-content: center;
    width: 100%;
    height: 97px;
}

.header_top {
    &.inner {
        display: flex;
        align-items: center;
        justify-content: space-between;
        flex: 1;
        margin: 0px 22px 14px;
    }
}

.title_label_bg {
    background-color: var(--main-yellow-color);

    &.shipment {
        border-top: 5px solid var(--shipping-bg-color);
    }

    &.incoming {
        border-top: 5px solid var(--received-bg-color);
    }

    &.progress {
        border-top: 5px solid var(--progress-bg-color);
    }

    &.shipment,
    &.incoming,
    &.progress {
        .title_label {
            &.inner {
                height: 60px;
            }
        }
    }

    .title_label {
        &.inner {
            display: flex;
            width: 100%;
            height: 65px;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            font-weight: 600;
            text-align: center;
        }
    }
}
</style>