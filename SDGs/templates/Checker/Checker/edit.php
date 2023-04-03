<?php

/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '認証者 編集');?>
<article class="contents">
    <section class="title_sec">
        <h2>認証者 編集</h2>
    </section>
    <?= $this->Form->create($checker); ?>
    <section class="input_sec">
        <?= $this->Form->hidden('id'); ?>
        <?= $this->Form->control('name', ['placeholder' => '静岡太郎', 'required' => false, 'type' => 'text', 'label' => false]) ?>
        <?= $this->Form->control('email', ['placeholder' => 'xxxxxxxxx@xxx.com', 'required' => false, 'type' => 'text', 'label' => false]) ?>
        <?= $this->Form->control('current_password', ['placeholder' => '現在のパスワード', 'required' => false, 'type' => 'password', 'label' => false]) ?>
        <?= $this->Form->control('new_password', ['required' => false, 'placeholder' => '新しいパスワード', 'label' => false, 'type' => 'password']) ?>
        <?= $this->Form->control('new_password_confirm', ['type' => 'password', 'placeholder' => '新しいパスワード（確認用）', 'label' => false, 'required' => false, 'type' => 'password']); ?>
    </section>

    <section class="btn_sec">
        <?= $this->Form->submit('保存') ?>
        <p>
            <button class="backBtn">
                <?= $this->Html->link('一覧に戻る', $this->Url->build([
                    'prefix' => 'Checker',
                    'controller' => 'Checker',
                    'action' => 'list',
                ], ['fullBase' => true])) ?>
            </button>
        </p>
    </section>
    <?= $this->Form->end(); ?>
</article>
