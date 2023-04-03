<script setup lang="ts">

</script>

<template>
    <div class="admin_footer_bg">
        <div class="admin_footer_wrap">
            <img src="/src/assets/Admin/common/footer_logo.svg" alt="@ukabis">
        </div>
    </div>
</template>

<style scoped lang="scss">
.admin_footer_bg {
    // z-index: 9999;
    // position: fixed;
    // bottom: 0px;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 80px;
    min-height: 80px;
    max-height: 80px;
    background: var(--white-color);

    .admin_footer_wrap {
        max-width: 1920px;
    }
}
</style>