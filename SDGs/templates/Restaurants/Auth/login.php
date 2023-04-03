<?php
/**
 * CakePHP(tm) : Rapid Development Framework (https://cakephp.org)
 * Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 *
 * Licensed under The MIT License
 * For full copyright and license information, please see the LICENSE.txt
 * Redistributions of files must retain the above copyright notice.
 *
 * @copyright Copyright (c) Cake Software Foundation, Inc. (https://cakefoundation.org)
 * @link      https://cakephp.org CakePHP(tm) Project
 * @since     0.10.0
 * @license   https://opensource.org/licenses/mit-license.php MIT License
 * @var \App\View\AppView $this
 */
?>
<section class="content">
    <div class="wrap_page">
        <article class="main_area">
			<?= $this->Form->create() ?>
			<section class="title_sec">
				<h1>ふじのくにSDGs認証<br>申請システム</h1>
			</section>

			<?= $this->Flash->render() ?>
			<section class="input_sec">
				<h3>飲食店申請者</h3>
				<?= $this->Form->control('email', ['required' => false, 'placeholder' => 'ログインID', 'label' => false, 'type' => 'text']) ?>
				<?= $this->Form->control('password', ['required' => false, 'placeholder' => 'パスワード', 'label' => false]) ?>
			</section>

			<section class="common_btn_sec mb50">
				<p><?= $this->Form->button('ログイン', ['class' => 'store_btn login_btn']); ?></p>
			</section>
			<?= $this->Form->end() ?>
			<section class="common_btn_sec">
				<h3>ログインIDをお持ちでない方は<br>こちらから登録ください。</h3>
				<p><button class="store_btn new_registration"><a href="./register" class="fs16">新規登録</a></button></p>
			</section>
			</article>
    </div>
</section>

