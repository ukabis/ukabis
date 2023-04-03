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
<?php $this->assign('title', '生産者 認定／否認メール');?>
<article class="contents">
    <section class="title_sec">
        <h2>生産者 認定／否認メール</h2>
    </section>

    <section class="list_sec">
        <table class="table-responsive">
            <tr>
                <th>回答ID</th>
                <th>代表者名</th>
                <th>屋号</th>
                <th>回答年月日日時</th>
                <th>ステータス</th>
            </tr>
            <tr style="width: 100%">
                <td><?= $farmQuestion->id ?></td>
                <td><?= $this->CustomHtml->renderTextWithMaxLength($farmQuestion->office->business->representative_name) ?></td>
                <td><?= $this->CustomHtml->renderTextWithMaxLength($farmQuestion->office->name) ?></td>
                <td><?= $farmQuestion->modified_at->format('d/m/y h:i A') ?></td>
                <td class="no-wrap"><?= $farmQuestion->status_label ?></td>
            </tr>
        </table>
    </section>

    <section class="disp_mailaddress_sec">
        <p class="disp_mail_title">送信先のメールアドレス</p>
        <p class="disp_mail"><?= h($farmQuestion->office->business->email) ?></p>
      </section>

    <?= $this->Form->create(); ?>
    <section class="select_sec">
        <p class="select_title">SDGsランクを付ける</p>
        <div class="wrap_mail_rank_select_block">
            <div class="mail_rank_select_block">
                <select name="offices_certified_rank">
                    <option value="" selected disabled style="display:none;">SDGsランク</option>
                    <option value="1" <?= h($farmQuestion->office->offices_certified_rank) === '1' ? 'selected' : '' ?>>ゴールド</option>
                    <option value="2" <?= h($farmQuestion->office->offices_certified_rank) === '2' ? 'selected' : '' ?>>シルバー</option>
                    <option value="3" <?= h($farmQuestion->office->offices_certified_rank) === '3' ? 'selected' : '' ?>>ブロンズ</option>
                    <option value="4" <?= h($farmQuestion->office->offices_certified_rank) === '4' ? 'selected' : '' ?>>否認</option>
                </select>
            </div>
        </div>
    </section>

    <section class="btn_sec">
        <p>
            <?= $this->Form->submit('メール送信') ?>
        </p>
        <p><button class="backBtn" type="button">
            <?= $this->Html->link('合計点のページに戻る', $this->Url->build([
                'prefix' => 'Checker',
                'controller' => 'FarmApplication',
                'action' => 'scoreTotal',
                'id' => $farmQuestion->id
            ], ['fullBase' => true])) ?>
        </button></p>
    </section>
    <?= $this->Form->end() ?>
</article>
