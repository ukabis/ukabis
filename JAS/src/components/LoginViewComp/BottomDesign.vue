<script setup lang="ts">
</script>

<template>
    <div class="bottom_design">
        <div class="left box">
            <div class="triangle"></div>
        </div>
        <div class="right box">
            <div class="triangle"></div>
        </div>
    </div>
</template>

<style scoped>
.bottom_design {
    display: flex;
    place-content: stretch;
    width: 100%;
    height: 10%;
    position: fixed;
    bottom: 0px;
}

.bottom_design .box {
    position: relative;
    flex: 1;
}

.left.box {
    border: 0px solid transparent;
    left: 1px;
    margin-left: -1px;
}

.right.box {
    border: 0px solid transparent;
    right: 1px;
    margin-right: -1px;
}

.box .triangle {
    position: absolute;
    width: 200%;
    height: 100%;
    min-height: 300px;
    background: var(--main-yellow-color);
    top: 52%;
}

.left.box .triangle {
    transform: rotate(15deg);
    left: -30%;
}

.right.box .triangle {
    transform: rotate(-15deg);
    right: -30%;
}
</style>