<script setup lang="ts">
import { ref } from 'vue'
import { storeToRefs } from 'pinia'
import { UseUtil } from '@/stores/util'

// Util
const util = UseUtil()
const {
    pageMaxNum,
    currentPageNum
} = storeToRefs(util)

// First Num
const leftNum = ref<number>(1)

// Position
const currentPos = ref<number>(1)

// Slide Page Num
const slidePageNumPrev = () => {
    if (leftNum.value <= 1) return
    leftNum.value--
}
const slidePageNumNext = () => {
    if (leftNum.value >= pageMaxNum.value - 6) return
    leftNum.value++
}

// Move Pos
const prevCurrentPos = () => {
    if (currentPos.value <= 1) return
    currentPos.value--
}
const nextCurrentPos = () => {
    if (currentPos.value >= pageMaxNum.value) return
    currentPos.value++
}

// Click Prev Btn
const prevPos = () => {
    if (currentPageNum.value <= 1) return
    if (currentPos.value == 1) {
        slidePageNumPrev()
    } else {
        prevCurrentPos()
    }
    currentPageNum.value--
}
// Click Next Btn
const nextPos = () => {
    if (currentPageNum.value >= pageMaxNum.value) return
    if (currentPos.value >= 5) {
        if (currentPageNum.value >= pageMaxNum.value - 2) {
            nextCurrentPos()
        } else {
            slidePageNumNext()
        }
    } else {
        nextCurrentPos()
    }
    currentPageNum.value++
}

// Click Page Num
const clickNum = (pos: number, pageNum: number) => {
    currentPos.value = pos
    currentPageNum.value = pageNum
    if(pageNum == pageMaxNum.value) {
        leftNum.value = pageMaxNum.value - 6
    }
}

/*
leftNum: 一番左の数字。最低値は1
currentPageNum: 現在のページ番号。最低値は1、最大値はpageMaxNum
currentPos: 1~5を移動。

prevPos:
    currentPageNum > 1 のとき
        currentPos = 1 のとき
            leftNum > 1 のとき
                leftNum-- currentPageNum--
        currentPos > 1 のとき
            currentPos-- currentPageNum--

nextPos:
    currentPageNum < pageMaxNum のとき
        currentPos = 5 のとき
            leftNum < pageMaxNum - 4 のとき
                leftNum++ currentPageNum++
        currentPos < 5 のとき
            currentPos++ currentPageNum++
*/

</script>

<template>
    <ul class="pagination_items" v-if="pageMaxNum > 1">
        <li @click="prevPos"><img src="/src/assets/Admin/common/pagination_arrow.svg" alt="prev"></li>
        <li @click="clickNum(1, leftNum + 0)" :class="{ current: currentPos == 1 }">{{ leftNum + 0 }}</li>
        <li v-if="pageMaxNum > 1" @click="clickNum(2, leftNum + 1)" :class="{ current: currentPos == 2 }">{{ leftNum + 1 }}</li>
        <li v-if="pageMaxNum > 2" @click="clickNum(3, leftNum + 2)" :class="{ current: currentPos == 3 }">{{ leftNum + 2 }}</li>
        <li v-if="pageMaxNum > 3" @click="clickNum(4, leftNum + 3)" :class="{ current: currentPos == 4 }">{{ leftNum + 3 }}</li>
        <li v-if="pageMaxNum > 4" @click="clickNum(5, leftNum + 4)" :class="{ current: currentPos == 5 }">{{ leftNum + 4 }}</li>
        <li v-if="leftNum + 4 == pageMaxNum - 2" @click="clickNum(6, leftNum + 5)" :class="{ current: currentPos == 6 }">{{ leftNum + 5 }}</li>
        <li v-if="(pageMaxNum > 6) && (leftNum + 4 != pageMaxNum - 2)">...</li>
        <li @click="clickNum(7, pageMaxNum)" v-if="pageMaxNum > 5" :class="{ current: currentPageNum == pageMaxNum }"
            :data-pos-num="pageMaxNum">{{ pageMaxNum }}</li>
        <li @click="nextPos"><img src="/src/assets/Admin/common/pagination_arrow.svg" alt="next"></li>
    </ul>
</template>

<style scoped lang="scss">
.pagination_items {
    display: flex;
    flex-wrap: nowrap;
    width: 264.1px;
    align-items: center;
    justify-content: start;
    padding: 0px;
    margin-top: 16px;

    li {
        cursor: pointer;
        width: 28px;
        height: 27px;
        background: var(--white-color);
        color: var(--font-color);
        font-size: 12px;
        font-weight: 300;
        border-radius: 5px;
        margin-right: 3px;

        &:first-child,
        &:last-child {
            background: transparent;
        }

        &:last-child img {
            transform: scaleX(-1);
        }

        &.current {
            background: var(--font-color);
            color: var(--white-color);
        }
    }
}
</style>