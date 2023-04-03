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
<?php $this->assign('title', '認証者 ホーム');?>
<section class="content">
    <div class="wrap_page">
    <article class="contents">
      <section class="title_sec admin_menu_title">
        <h2>管理者メニュー</h2>
      </section>

      <section class="admin_link_sec">
        <div class="admin_link_box">
          <h2>管理者アカウント</h2>
          <p>
            <?= $this->Html->link('管理者アカウント新規作成', $this->Url->build([
                '_name' => 'admin.register',
              ], ['fullBase' => true])) ?>
            </p>
          <p>
            <?= $this->Html->link('管理者アカウント一覧', $this->Url->build([
              '_name' => 'admin.list',
            ], ['fullBase' => true])) ?>
          </p>
        </div>
        <div class="admin_link_box">
          <h2>認証者アカウント</h2>
          <p>
            <?= $this->Html->link('認証者アカウント 新規作成', $this->Url->build([
                '_name' => 'admin.checker.register',
              ], ['fullBase' => true])) ?>
            </p>
          <p>
            <?= $this->Html->link('認証者アカウント 一覧', $this->Url->build([
              '_name' => 'admin.checker.list',
            ], ['fullBase' => true])) ?>
          </p>
        </div>
      </section>

    </article>
    </div>
</section>
