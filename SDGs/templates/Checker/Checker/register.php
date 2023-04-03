<?php
/**
 * @var \App\View\AppView $this
 */
?>
<?php $this->assign('title', '認証者 新規作成');?>
<section class="content">
	<article class="contents">
		<section class="title_sec">
			<h2>認証者 新規作成</h2>
		</section>
        <?= $this->Form->create($checker) ?>
		<section class="input_sec">
			<?= $this->Form->control('name', ['required' => false, 'placeholder' => '氏名', 'label' => false, 'type' => 'text']) ?>
			<?= $this->Form->control('email', ['required' => false, 'placeholder' => 'メールアドレス', 'label' => false, 'type' => 'text']) ?>
			<?= $this->Form->control('password', ['required' => false, 'placeholder' => 'パスワード', 'label' => false]) ?>
            <?= $this->Form->control('password_confirm', ['type' => 'password', 'placeholder' => 'パスワード（確認用）', 'label' => false, 'required' => false]); ?>
		</section>
		<section class="btn_sec">
			<p><?= $this->Form->button('新規作成'); ?></p>
			<p><button class="backBtn"><?= $this->Html->link('ホームに戻る', '/checker'); ?></button></p>
		</section>
		<?= $this->Form->end() ?>
    </article>
</section>

