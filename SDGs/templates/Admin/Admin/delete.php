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
<?php $this->assign('title', '管理者 削除'); ?>
<article class="contents">
  <section class="title_sec">
    <h2>管理者 削除</h2>
  </section>

  <section class="alert_sec">
    <p>こちらのアカウントを削除しますか？</p>
  </section>
  <section class="list_sec remove_table">
    <table class="table-responsive">
      <tr>
        <th>ID</th>
        <th>氏名</th>
        <th>メールアドレス</th>
      </tr>
      <tr style="width: 100%">
        <td><?= $admin->id ?></td>
        <td><?= $this->CustomHtml->renderTextWithMaxLength($admin->name, 15) ?></td>
        <td><?= $this->CustomHtml->renderTextWithMaxLength($admin->email, 20) ?></td>
      </tr>
    </table>
  </section>

  <section class="btn_sec">
    <?= $this->Form->create(
      $admin,
      [
        'type' => 'delete',
        'url' => $this->Url->build([
          '_name' => 'admin.delete',
          'id' => $admin->id
        ], ['fullBase' => true])
      ]
    ) ?>
    <?= $this->Form->submit('削除する', ['class' => 'remove']) ?>
    <?= $this->Form->end(); ?>
    <p>
      <?=
        $this->Html->link('削除しない',
          $this->Url->build(['_name' => 'admin.list'],['fullBase' => true]),
          ['class' => 'btn-back']
        )
      ?>
    </p>
  </section>

</article>
