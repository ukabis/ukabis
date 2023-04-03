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
<?php $this->assign('title', '管理者 新規作成'); ?>
<section class="content">
  <article class="contents">
    <section class="title_sec">
      <h2>管理者 新規作成</h2>
    </section>
    <?= $this->Form->create($admin) ?>
    <section class="input_sec">
      <div class="input">
        <?php
          echo $this->Form->text('name', ['required' => false, 'placeholder' => '氏名', 'label' => false]);
          echo $this->Form->error('name');
        ?>
      </div>
      <div class="input">
        <?php
          echo $this->Form->email('email', ['required' => false, 'placeholder' => 'メールアドレス', 'label' => false]);
          echo $this->Form->error('email');
        ?>
      </div>
      <div class="input">
        <?php
          echo $this->Form->password('password', ['required' => false, 'placeholder' => 'パスワード', 'label' => false]);
          echo $this->Form->error('password');
        ?>
      </div>
      <div class="input">
        <?php
          echo $this->Form->password('password_confirm', ['type' => 'password', 'placeholder' => 'パスワード（確認用）', 'label' => false, 'required' => false]);
          echo $this->Form->error('password_confirm');
        ?>
      </div>

    </section>
    <section class="btn_sec">
      <p><?= $this->Form->submit('新規作成'); ?></p>
      <p>
        <?=
          $this->Html->link('管理者メニューに戻る',
            $this->Url->build(['_name' => 'admin.index'],['fullBase' => true]),
            ['class' => 'btn-back']
          )
        ?>
      </p>
    </section>
    <?= $this->Form->end() ?>
  </article>
</section>
