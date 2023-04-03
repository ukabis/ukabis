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
      <section class="title_sec">
        <h2>認証者 ホーム</h2>
      </section>

      <section class="admin_link_sec">
        <p>
            <?= $this->Html->link('飲食店の回答一覧を見る', $this->Url->build([
                'prefix' => 'Checker',
                'controller' => 'RestaurantApplication',
                'action' => 'list',
            ], ['fullBase' => true])) ?>
        </p>
        <p>
          <?= $this->Html->link('生産者の回答一覧を見る', $this->Url->build([
              'prefix' => 'Checker',
              'controller' => 'FarmApplication',
              'action' => 'list',
          ], ['fullBase' => true])) ?>
        </p>
        <p><?= $this->Html->link('認証者 新規作成', $this->Url->build([
              'prefix' => 'Checker',
              'controller' => 'Checker',
              'action' => 'register',
          ], ['fullBase' => true]), ['class' => 'button']); ?></p>
        <p><?= $this->Html->link('認証者 一覧', $this->Url->build([
              'prefix' => 'Checker',
              'controller' => 'Checker',
              'action' => 'list',
          ], ['fullBase' => true]), ['class' => 'button']); ?></p>
      </section>

    </article>
    </div>
</section>
