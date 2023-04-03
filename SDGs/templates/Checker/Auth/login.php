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
<?php $this->assign('title', '認証者トップページ');?>
<section class="content">
    <article class="contents">
        <?= $this->Form->create() ?>
        <section class="title_sec">
            <h2>認証者トップページ</h2>
        </section>
        <?= $this->Flash->render() ?>
        <section class="input_sec">
			<?= $this->Form->control('email', ['required' => false, 'placeholder' => 'メールアドレス', 'label' => false, 'type' => 'text']) ?>
			<?= $this->Form->control('password', ['required' => false, 'placeholder' => 'パスワード', 'label' => false]) ?>
        </section>
        <section class="btn_sec">
			<p><?= $this->Form->button('ログイン'); ?></p>
        </section>
			<?= $this->Form->end() ?>
    </article>
</section>
