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
<?php $this->assign('title', '管理者用 認証者アカウント 一覧'); ?>
<article class="contents">
  <section class="title_sec">
    <h2>管理者用 認証者アカウント 一覧</h2>
  </section>

  <section class="list_sec">
    <table class="table-responsive">
      <tr>
        <th>ID</th>
        <th>氏名</th>
        <th>メールアドレス</th>
        <th>Actions</th>
      </tr>
      <?php foreach ($checkers as $key => $item) : ?>
        <tr style="width: 100%">
          <td><?= $item->id ?></td>
          <td><?= $this->CustomHtml->renderTextWithMaxLength($item->name, 15) ?></td>
          <td><?= $this->CustomHtml->renderTextWithMaxLength($item->email, 20) ?></td>
          <td>
            <?= $this->Html->link('閲覧', $this->Url->build([
              '_name' => 'admin.checker.detail',
              'id' => $item->id
            ], ['fullBase' => true])) ?>
            <?= $this->Html->link('編集', $this->Url->build([
                '_name' => 'admin.checker.edit',
                'id' => $item->id
            ], ['fullBase' => true])) ?>
            <?php if ($user->id != $item->id): ?>
                <?= $this->Html->link('削除', $this->Url->build([
                    '_name' => 'admin.checker.delete',
                    'id' => $item->id
                ], ['fullBase' => true])) ?>
            <?php else: ?>
                <span></span>
            <?php endif; ?>
          </td>
        </tr>
      <?php endforeach ?>
    </table>
  </section>

  <?= $this->element('paginate/jump_links'); ?>

  <section class="btn_sec">
    <p>
      <?=
        $this->Html->link('管理者メニューに戻る',
          $this->Url->build(['_name' => 'admin.index'],['fullBase' => true]),
          ['class' => 'btn-back']
        )
      ?>
    </p>
  </section>

  <?= $this->element('paginate/page_counter'); ?>

</article>
