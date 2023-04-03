<?php
?>
<section class="paging_sec">
    <?= $this->Paginator->prev('prev', ['class' => 'prev']); ?>
    <?= $this->Paginator->next('next', ['class' => 'next']); ?>
</section>
