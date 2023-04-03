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
<?php $this->assign('title', '生産者申請者の回答一覧');?>
<article class="contents">
  <section class="title_sec">
    <h2>生産者申請者の回答一覧</h2>
  </section>

  <section class="list_sec">
    <table class="table-responsive">
      <tr>
        <th>回答ID</th>
        <th>代表者名</th>
        <th>屋号</th>
        <th>回答年月日日時</th>
        <th>ステータス</th>
        <th>Actions</th>
      </tr>
      <?php foreach ($farmQuestions as $key => $farmQuestion) : ?>
        <tr style="width: 100%">
          <td><?= $farmQuestion->id ?></td>
          <td><?= $this->CustomHtml->renderTextWithMaxLength($farmQuestion->office->business->representative_name) ?></td>
          <td><?= $this->CustomHtml->renderTextWithMaxLength($farmQuestion->office->name) ?></td>
          <td><?= $farmQuestion->modified_at->format('d/m/y h:i A') ?></td>
          <td class="no-wrap"><?= $farmQuestion->status_label ?></td>
          <td>
            <?= $this->Html->link('こちらの回答を採点する', $this->Url->build([
              'prefix' => 'Checker',
              'controller' => 'FarmApplication',
              'action' => 'score',
              'id' => $farmQuestion->id
            ], ['fullBase' => true])) ?>
          </td>
        </tr>
      <?php endforeach; ?>
    </table>
  </section>

  <?= $this->element('paginate/jump_links'); ?>

  <section class="btn_sec">
    <p><button class="backBtn">
        <?= $this->Html->link('ホームに戻る', $this->Url->build([
          'prefix' => 'Checker',
          'controller' => 'Checker',
          'action' => 'index',
        ], ['fullBase' => true])) ?>
      </button></p>
  </section>

  <?= $this->element('paginate/page_counter'); ?>

</article>
